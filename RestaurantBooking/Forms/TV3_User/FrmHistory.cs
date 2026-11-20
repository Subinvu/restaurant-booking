using RestaurantBooking.Config;
using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// Màn hình Lịch sử đơn đặt bàn/giao hàng, Hủy đơn & Đánh giá trải nghiệm / Phản hồi khách hàng.
/// </summary>
public partial class FrmHistory : Form
{
    private sealed record FilterChoice(string Value, string Label)
    {
        public override string ToString() => Label;
    }

    private sealed record LikedItemChoice(string ItemCode, string Label)
    {
        public override string ToString() => Label;
    }

    private readonly CartReviewService _cartReviewService = new();
    private readonly BookingService _bookingService = new();
    private readonly MenuAdminService _menuService = new();

    private Dictionary<string, string> _branchNameMap = new(StringComparer.OrdinalIgnoreCase);
    private List<BookingOrderModel> _orders = [];
    private Dictionary<string, ReviewModel> _reviewByOrder = new(StringComparer.OrdinalIgnoreCase);
    private List<ReviewModel> _allReviews = [];
    private bool _isInitializing = true;

    public FrmHistory()
    {
        InitializeComponent();
        ApplyCleanTheme();
        Load += FrmHistory_Load;
    }

    private void ApplyCleanTheme()
    {
        UiTheme.StyleTabControl(tabMain);
        UiTheme.StyleHeaderBackButton(btnBack);
        UiTheme.StyleSecondaryButton(btnRefreshOrders);
        UiTheme.StyleDangerButton(btnCancelOrder);
        UiTheme.StylePrimaryButton(btnSubmitReview, useGreen: true);
        UiTheme.StyleSecondaryButton(btnLoadReviews);
        UiTheme.StylePrimaryButton(btnSendReply);
        UiTheme.StyleGrid(dgvOrders);
        UiTheme.StyleGrid(dgvOrderItems);
        UiTheme.StyleGrid(dgvAllReviews);
    }

    private void dgvOrders_SelectionChanged(object? sender, EventArgs e) => OnOrderSelected();
    private void dgvAllReviews_SelectionChanged(object? sender, EventArgs e) => OnReviewModerationSelected();
    private void btnBack_Click(object? sender, EventArgs e) => Close();

    private string GetBranchDisplayName(string? resCode)
    {
        if (string.IsNullOrWhiteSpace(resCode)) return "-";
        return _branchNameMap.TryGetValue(resCode, out var name) ? name : resCode;
    }

    private static string FormatStatus(string status) => status.ToLowerInvariant() switch
    {
        "pending" => "Chờ xác nhận",
        "confirmed" => "Đã xác nhận",
        "serving" => "Đang phục vụ",
        "completed" => "Hoàn tất",
        "cancelled" => "Đã hủy",
        _ => status
    };

    private static string FormatOrderType(string orderType) => orderType.ToLowerInvariant() switch
    {
        "dine_in" => "Ăn tại quán",
        "takeaway" => "Mang về / Ship",
        _ => orderType
    };

    private async void FrmHistory_Load(object? sender, EventArgs e)
    {
        try
        {
            _isInitializing = true;
            lblInfo.Text = $"Đơn hàng & Đánh giá — {SessionContext.FullName}";

            var branches = await _menuService.GetRestaurantsAsync();
            _branchNameMap = branches.ToDictionary(b => b.RestaurantCode, b => b.Name, StringComparer.OrdinalIgnoreCase);

            if (!SessionContext.IsStaffOrAdmin && tabMain.TabPages.Contains(tabReplyReviews))
                tabMain.TabPages.Remove(tabReplyReviews);

            cboStatusFilter.Items.Clear();
            cboStatusFilter.Items.AddRange([
                new FilterChoice("ALL", "Tất cả trạng thái"),
                new FilterChoice("pending", "Chờ xác nhận"),
                new FilterChoice("confirmed", "Đã xác nhận"),
                new FilterChoice("serving", "Đang phục vụ"),
                new FilterChoice("completed", "Hoàn tất"),
                new FilterChoice("cancelled", "Đã hủy")
            ]);
            cboStatusFilter.SelectedIndex = 0;

            cboRating.Items.Clear();
            cboRating.Items.AddRange(["5 ★ (Xuất sắc)", "4 ★ (Hài lòng)", "3 ★ (Bình thường)", "2 ★ (Chưa tốt)", "1 ★ (Rất tệ)"]);
            cboRating.SelectedIndex = 0;

            if (SessionContext.IsStaffOrAdmin)
            {
                cboRevRestaurant.Items.Clear();
                cboRevRestaurant.Items.Add(new FilterChoice("ALL", "Tất cả chi nhánh"));
                foreach (var b in branches)
                    cboRevRestaurant.Items.Add(new FilterChoice(b.RestaurantCode, b.Name));
                cboRevRestaurant.SelectedIndex = 0;
            }

            _isInitializing = false;
            await LoadCustomerOrdersAsync();
        }
        catch (Exception ex)
        {
            _isInitializing = false;
            MessageBox.Show($"Lỗi tải lịch sử đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void cboStatusFilter_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isInitializing) return;
        try
        {
            await LoadCustomerOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lọc đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnRefreshOrders_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadCustomerOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi làm mới danh sách đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void tabMain_SelectedIndexChanged(object? sender, EventArgs e)
    {
        try
        {
            if (tabMain.SelectedTab == tabReplyReviews)
                await LoadReviewsForReplyAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách đánh giá: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadCustomerOrdersAsync()
    {
        var status = (cboStatusFilter.SelectedItem as FilterChoice)?.Value ?? "ALL";
        _orders = await _cartReviewService.GetCustomerOrdersAsync(SessionContext.UserCode, status);
        _reviewByOrder = await _cartReviewService.GetReviewsByOrderCodesAsync(_orders.Select(o => o.OrderCode));
        if (IsDisposed) return;

        dgvOrders.DataSource = _orders.Select(o =>
        {
            _reviewByOrder.TryGetValue(o.OrderCode, out var rev);
            string placeInfo = !string.IsNullOrWhiteSpace(o.TableId)
                ? $"Bàn {o.TableId}"
                : (!string.IsNullOrWhiteSpace(o.DeliveryAddress) ? o.DeliveryAddress : "Mang về");

            return new
            {
                MaDon = o.OrderCode,
                Loai = FormatOrderType(o.OrderType),
                ChiNhanh = GetBranchDisplayName(o.RestaurantCode),
                DiaDiem = placeInfo,
                Ngay = o.BookingDate,
                Gio = o.TimeSlot,
                TongTien = $"{o.TotalAmount:N0} đ",
                TrangThai = FormatStatus(o.Status),
                DanhGia = rev != null ? $"{rev.Rating} ★" : (o.Status == "completed" ? "Chưa đánh giá" : "-")
            };
        }).ToList();

        if (dgvOrders.Columns["MaDon"] is { } c1) { c1.HeaderText = "Mã đơn"; c1.FillWeight = 130; }
        if (dgvOrders.Columns["Loai"] is { } c2) { c2.HeaderText = "Hình thức"; c2.FillWeight = 86; }
        if (dgvOrders.Columns["ChiNhanh"] is { } c3) { c3.HeaderText = "Chi nhánh"; c3.FillWeight = 115; }
        if (dgvOrders.Columns["DiaDiem"] is { } c4) { c4.HeaderText = "Bàn / Nhận hàng"; c4.FillWeight = 115; }
        if (dgvOrders.Columns["Ngay"] is { } c5) { c5.HeaderText = "Ngày"; c5.FillWeight = 80; }
        if (dgvOrders.Columns["Gio"] is { } c6) { c6.HeaderText = "Giờ"; c6.FillWeight = 55; }
        if (dgvOrders.Columns["TongTien"] is { } c7) { c7.HeaderText = "Tổng tiền"; c7.FillWeight = 85; }
        if (dgvOrders.Columns["TrangThai"] is { } c8) { c8.HeaderText = "Trạng thái"; c8.FillWeight = 90; }
        if (dgvOrders.Columns["DanhGia"] is { } c9) { c9.HeaderText = "Đánh giá"; c9.FillWeight = 80; }

        OnOrderSelected();
    }

    private BookingOrderModel? GetSelectedOrder()
    {
        int idx = dgvOrders.CurrentRow?.Index ?? -1;
        return idx >= 0 && idx < _orders.Count ? _orders[idx] : null;
    }

    private void OnOrderSelected()
    {
        var order = GetSelectedOrder();
        if (order == null)
        {
            dgvOrderItems.DataSource = null;
            clbLikedItems.Items.Clear();
            txtComment.Clear();
            btnCancelOrder.Enabled = false;
            btnSubmitReview.Enabled = false;
            lblReviewStatus.Text = "Chưa có đơn hàng nào được chọn.";
            return;
        }

        dgvOrderItems.DataSource = order.Items.Select(i => new
        {
            TenMon = i.Name,
            Size = i.SizeCode,
            SL = i.Qty,
            DonGia = $"{i.UnitPrice:N0} đ",
            ThanhTien = $"{i.Subtotal:N0} đ"
        }).ToList();

        if (dgvOrderItems.Columns["TenMon"] is { } c1) { c1.HeaderText = "Tên món"; c1.FillWeight = 165; }
        if (dgvOrderItems.Columns["Size"] is { } c2) { c2.HeaderText = "Cỡ"; c2.FillWeight = 38; }
        if (dgvOrderItems.Columns["SL"] is { } c3) { c3.HeaderText = "SL"; c3.FillWeight = 36; }
        if (dgvOrderItems.Columns["DonGia"] is { } c4) { c4.HeaderText = "Đơn giá"; c4.FillWeight = 85; }
        if (dgvOrderItems.Columns["ThanhTien"] is { } c5) { c5.HeaderText = "Thành tiền"; c5.FillWeight = 92; }

        btnCancelOrder.Enabled = order.Status is "pending" or "confirmed";

        clbLikedItems.Items.Clear();
        foreach (var distinctItem in order.Items.DistinctBy(i => i.ItemCode))
        {
            clbLikedItems.Items.Add(new LikedItemChoice(distinctItem.ItemCode, distinctItem.Name), isChecked: true);
        }

        if (_reviewByOrder.TryGetValue(order.OrderCode, out var existingRev))
        {
            var replyInfo = existingRev.Reply != null
                ? $" | Phản hồi ({existingRev.Reply.StaffName}): \"{existingRev.Reply.Content}\""
                : " | Chưa có phản hồi từ nhà hàng.";
            lblReviewStatus.Text = $"Đã đánh giá {existingRev.Rating} ★{replyInfo}";
            txtComment.Text = existingRev.Comment;
            txtComment.ReadOnly = true;
            cboRating.Enabled = false;
            clbLikedItems.Enabled = false;
            btnSubmitReview.Enabled = false;
        }
        else if (order.Status == "completed")
        {
            lblReviewStatus.Text = $"Đơn {order.OrderCode} đã hoàn tất — hãy chia sẻ trải nghiệm của bạn!";
            txtComment.Clear();
            txtComment.ReadOnly = false;
            cboRating.Enabled = true;
            clbLikedItems.Enabled = true;
            btnSubmitReview.Enabled = true;
        }
        else
        {
            lblReviewStatus.Text = $"Đơn {order.OrderCode} đang ở trạng thái '{FormatStatus(order.Status)}' — chỉ gửi đánh giá sau khi hoàn tất.";
            txtComment.Clear();
            txtComment.ReadOnly = true;
            cboRating.Enabled = false;
            clbLikedItems.Enabled = false;
            btnSubmitReview.Enabled = false;
        }
    }

    private async void btnCancelOrder_Click(object? sender, EventArgs e)
    {
        var order = GetSelectedOrder();
        if (order == null) return;

        var confirm = MessageBox.Show(
            $"Bạn có chắc chắn muốn hủy đơn {order.OrderCode} ({(string.IsNullOrEmpty(order.TableId) ? "Mang về" : $"Bàn {order.TableId}")} • {order.BookingDate} {order.TimeSlot}) không?",
            "Xác nhận hủy đơn",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        btnCancelOrder.Enabled = false;
        try
        {
            var res = await _bookingService.CancelOrderAndPopWaitlistAsync(
                order.OrderCode,
                SessionContext.UserCode,
                SessionContext.IsStaffOrAdmin);

            MessageBox.Show(
                res.Message,
                res.Success ? "Đã hủy đơn hàng" : "Không thể hủy đơn",
                MessageBoxButtons.OK,
                res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (res.Success)
                await LoadCustomerOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi hủy đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) OnOrderSelected();
        }
    }

    private async void btnSubmitReview_Click(object? sender, EventArgs e)
    {
        var order = GetSelectedOrder();
        if (order == null) return;

        int rating = 5 - Math.Clamp(cboRating.SelectedIndex, 0, 4);
        var likedCodes = new List<string>();
        foreach (var item in clbLikedItems.CheckedItems)
        {
            if (item is LikedItemChoice choice)
                likedCodes.Add(choice.ItemCode);
        }

        btnSubmitReview.Enabled = false;
        try
        {
            var res = await _cartReviewService.SubmitReviewAsync(
                order.OrderCode,
                SessionContext.UserCode,
                SessionContext.FullName,
                rating,
                txtComment.Text,
                likedCodes);

            MessageBox.Show(
                res.Message,
                res.Success ? "Cảm ơn đánh giá của bạn" : "Không thể gửi đánh giá",
                MessageBoxButtons.OK,
                res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (res.Success)
                await LoadCustomerOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi gửi đánh giá: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) OnOrderSelected();
        }
    }

    // === TAB 2: STAFF/ADMIN REPLY TO REVIEWS ===

    private async void btnLoadReviews_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadReviewsForReplyAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách đánh giá: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadReviewsForReplyAsync()
    {
        var resFilter = (cboRevRestaurant.SelectedItem as FilterChoice)?.Value ?? "ALL";
        _allReviews = await _cartReviewService.GetReviewsForModerationAsync(resFilter, chkOnlyUnreplied.Checked);
        if (IsDisposed) return;

        dgvAllReviews.DataSource = _allReviews.Select(r => new
        {
            MaDon = r.OrderCode,
            ChiNhanh = GetBranchDisplayName(r.RestaurantCode),
            KhachHang = r.FullName,
            Sao = $"{r.Rating} ★",
            NhanXet = r.Comment,
            PhanHoi = r.Reply != null ? $"[{r.Reply.StaffName}] {r.Reply.Content}" : "(Chưa phản hồi)"
        }).ToList();

        if (dgvAllReviews.Columns["MaDon"] is { } c1) { c1.HeaderText = "Mã đơn"; c1.FillWeight = 115; }
        if (dgvAllReviews.Columns["ChiNhanh"] is { } c2) { c2.HeaderText = "Chi nhánh"; c2.FillWeight = 105; }
        if (dgvAllReviews.Columns["KhachHang"] is { } c3) { c3.HeaderText = "Khách hàng"; c3.FillWeight = 110; }
        if (dgvAllReviews.Columns["Sao"] is { } c4) { c4.HeaderText = "Số sao"; c4.FillWeight = 55; }
        if (dgvAllReviews.Columns["NhanXet"] is { } c5) { c5.HeaderText = "Nhận xét"; c5.FillWeight = 175; }
        if (dgvAllReviews.Columns["PhanHoi"] is { } c6) { c6.HeaderText = "Phản hồi của nhà hàng"; c6.FillWeight = 180; }

        OnReviewModerationSelected();
    }

    private ReviewModel? GetSelectedModerationReview()
    {
        int idx = dgvAllReviews.CurrentRow?.Index ?? -1;
        return idx >= 0 && idx < _allReviews.Count ? _allReviews[idx] : null;
    }

    private void OnReviewModerationSelected()
    {
        var rev = GetSelectedModerationReview();
        if (rev == null)
        {
            lblSelectedReview.Text = "Chưa chọn đánh giá.";
            txtReplyContent.Clear();
            btnSendReply.Enabled = false;
            return;
        }

        lblSelectedReview.Text = $"Đang chọn đơn {rev.OrderCode} • {rev.FullName} • {rev.Rating}★: \"{rev.Comment}\"";
        txtReplyContent.Text = rev.Reply?.Content ?? "Cảm ơn quý khách đã ủng hộ Nhà hàng Hương Việt!";
        btnSendReply.Enabled = true;
    }

    private async void btnSendReply_Click(object? sender, EventArgs e)
    {
        var rev = GetSelectedModerationReview();
        if (rev == null) return;

        btnSendReply.Enabled = false;
        try
        {
            var res = await _cartReviewService.ReplyToReviewAsync(
                rev.ReviewCode,
                SessionContext.FullName,
                txtReplyContent.Text,
                SessionContext.IsStaffOrAdmin);

            MessageBox.Show(
                res.Message,
                res.Success ? "Phản hồi thành công" : "Không thể gửi phản hồi",
                MessageBoxButtons.OK,
                res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (res.Success)
                await LoadReviewsForReplyAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi gửi phản hồi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnSendReply.Enabled = GetSelectedModerationReview() != null;
        }
    }
}
