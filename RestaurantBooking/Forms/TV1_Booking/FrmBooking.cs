using RestaurantBooking.Config;
using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV1_Booking;

/// <summary>
/// [Độc quyền TV1 - Giai đoạn 1]
/// Tab 1: Sơ đồ bàn (Xanh/Vàng TTL 15p/Đỏ), Waitlist và Chốt đơn kèm giỏ hàng Memurai.
/// Tab 2: Quản lý & Duyệt trạng thái đơn (chỉ mở khi Staff hoặc Admin).
/// </summary>
public partial class FrmBooking : Form
{
    private sealed record ComboOption(string Value, string Label)
    {
        public override string ToString() => Label;
    }

    private readonly BookingService _service = new();
    private readonly Dictionary<string, Button> _tableButtons = new();

    private List<TableStatusDto> _currentTables = new();
    private List<CartItemDto> _currentCart = new();
    private List<BookingOrderModel> _manageOrders = new();
    private string? _selectedTableId;
    private bool _isInitializing = true;
    private bool _isReloadingMap;

    public FrmBooking()
    {
        InitializeComponent();
        Load += FrmBooking_Load;
        FormClosing += (_, _) => tmrCountdown.Stop();
    }

    private async void FrmBooking_Load(object? sender, EventArgs e)
    {
        try
        {
            _isInitializing = true;
            lblInfo.Text =
                $"Phân hệ TV1 • Người dùng: {SessionContext.FullName} ({SessionContext.UserCode} - {SessionContext.Role.ToUpperInvariant()})";

            if (!SessionContext.IsStaffOrAdmin && tabMain.TabPages.Contains(tabManageOrders))
            {
                tabMain.TabPages.Remove(tabManageOrders);
            }

            PopulateStaticCombos();
            await LoadRestaurantsAsync();
            _isInitializing = false;

            await ReloadTableMapAsync();
            await LoadCartAsync();

            if (SessionContext.IsStaffOrAdmin)
            {
                await LoadManageOrdersAsync();
            }

            tmrCountdown.Start();
        }
        catch (Exception ex)
        {
            _isInitializing = false;
            MessageBox.Show($"Lỗi khởi tạo màn hình Đặt bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PopulateStaticCombos()
    {
        cboTimeSlot.Items.Clear();
        cboTimeSlot.Items.AddRange(["11:00", "13:00", "17:30", "19:00", "20:30"]);
        cboTimeSlot.SelectedItem = "19:00";

        cboPaymentMethod.Items.Clear();
        cboPaymentMethod.Items.AddRange([
            new ComboOption("cash", "Tiền mặt (cash)"),
            new ComboOption("momo", "Ví MoMo (momo)"),
            new ComboOption("banking", "Chuyển khoản (banking)")
        ]);
        cboPaymentMethod.SelectedIndex = 0;

        cboManageStatus.Items.Clear();
        cboManageStatus.Items.AddRange([
            new ComboOption("ALL", "Tất cả trạng thái"),
            new ComboOption("pending", "Chờ xác nhận (pending)"),
            new ComboOption("confirmed", "Đã xác nhận (confirmed)"),
            new ComboOption("serving", "Đang phục vụ (serving)"),
            new ComboOption("completed", "Hoàn tất (completed)"),
            new ComboOption("cancelled", "Đã hủy (cancelled)")
        ]);
        cboManageStatus.SelectedIndex = 0;

        dtpBookingDate.Value = DateTime.Today;
        dtpManageDate.Value = DateTime.Today;
    }

    private async Task LoadRestaurantsAsync()
    {
        var restaurants = await _service.GetActiveRestaurantsAsync();

        cboRestaurant.Items.Clear();
        cboManageRestaurant.Items.Clear();
        cboManageRestaurant.Items.Add(new ComboOption("ALL", "Tất cả chi nhánh"));

        foreach (var r in restaurants)
        {
            var opt = new ComboOption(r.RestaurantCode, $"{r.RestaurantCode} — {r.Name}");
            cboRestaurant.Items.Add(opt);
            cboManageRestaurant.Items.Add(opt);
        }

        if (cboRestaurant.Items.Count > 0) cboRestaurant.SelectedIndex = 0;
        if (cboManageRestaurant.Items.Count > 0) cboManageRestaurant.SelectedIndex = 0;
    }

    private (string RestaurantCode, string BookingDate, string TimeSlot) GetSelectedFilterSlot()
    {
        var resCode = (cboRestaurant.SelectedItem as ComboOption)?.Value ?? "RES01";
        var dateStr = BookingService.NormalizeDate(dtpBookingDate.Value);
        var slot = cboTimeSlot.SelectedItem?.ToString() ?? "19:00";
        return (resCode, dateStr, slot);
    }

    private async void FilterInputs_Changed(object? sender, EventArgs e)
    {
        if (_isInitializing) return;
        _selectedTableId = null;
        await ReloadTableMapAsync();
    }

    private void OrderType_Changed(object? sender, EventArgs e)
    {
        bool isDineIn = rdoDineIn.Checked;
        flpTables.Enabled = isDineIn;
        nudGuestCount.Enabled = isDineIn;
        btnJoinWaitlist.Enabled = isDineIn;
        UpdateSelectedTablePanel();
        btnConfirmBooking.Text = isDineIn
            ? "CHỐT ĐƠN ĐẶT BÀN && CẬP NHẬT XẾP HẠNG"
            : "CHỐT ĐƠN MANG VỀ && CẬP NHẬT XẾP HẠNG";
    }

    private async Task ReloadTableMapAsync()
    {
        if (IsDisposed || _isReloadingMap) return;

        _isReloadingMap = true;
        try
        {
            var (resCode, dateStr, slot) = GetSelectedFilterSlot();
            var tables = await _service.GetTableStatusesAsync(resCode, dateStr, slot);
            var waitlist = await _service.GetWaitlistAsync(resCode, dateStr, slot);
            if (IsDisposed) return;

            _currentTables = tables;
            if (_selectedTableId != null && _currentTables.All(t => t.TableId != _selectedTableId))
                _selectedTableId = null;

            RenderTableButtons();
            UpdateSelectedTablePanel();
            lblWaitlistCount.Text = $"Hàng đợi Waitlist ({resCode} • {slot}): {waitlist.Count} khách đang chờ";
        }
        finally
        {
            _isReloadingMap = false;
        }
    }

    private void RenderTableButtons()
    {
        flpTables.SuspendLayout();
        flpTables.Controls.Clear();
        _tableButtons.Clear();

        foreach (var table in _currentTables)
        {
            var btn = new Button
            {
                Size = new Size(146, 92),
                Margin = new Padding(8),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Tag = table.TableId,
                Cursor = Cursors.Hand
            };
            btn.Click += TableButton_Click;
            ApplyTableButtonVisual(btn, table);

            _tableButtons[table.TableId] = btn;
            flpTables.Controls.Add(btn);
        }

        flpTables.ResumeLayout();
    }

    private void ApplyTableButtonVisual(Button btn, TableStatusDto table)
    {
        var areaText = table.Area switch
        {
            "vip" => "VIP",
            "outdoor" => "Sân vườn",
            _ => "Trong nhà"
        };

        string statusLine = table.State switch
        {
            TableVisualState.Available => "TRỐNG",
            TableVisualState.Locked =>
                $"GIỮ ({table.LockedByUserCode})\n{FormatMmSs(table.RemainingLockSeconds)}",
            TableVisualState.Booked => $"ĐÃ ĐẶT\n{table.BookedOrderCode}",
            _ => "NGƯNG PHỤC VỤ"
        };

        btn.Text = $"{table.TableName} ({table.Capacity} chỗ)\n[{areaText}]\n{statusLine}";

        (Color back, Color fore, Color border) = table.State switch
        {
            TableVisualState.Available => (Color.FromArgb(220, 252, 231), Color.FromArgb(20, 83, 45), Color.SeaGreen),
            TableVisualState.Locked => (Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14), Color.DarkOrange),
            TableVisualState.Booked => (Color.FromArgb(254, 226, 226), Color.FromArgb(153, 27, 27), Color.Firebrick),
            _ => (Color.Gainsboro, Color.DimGray, Color.Gray)
        };

        btn.BackColor = back;
        btn.ForeColor = fore;

        bool isSelected = string.Equals(_selectedTableId, table.TableId, StringComparison.Ordinal);
        btn.FlatAppearance.BorderSize = isSelected ? 3 : 1;
        btn.FlatAppearance.BorderColor = isSelected ? Color.FromArgb(30, 58, 95) : border;
    }

    private void TableButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not string tableId) return;
        _selectedTableId = tableId;

        foreach (var t in _currentTables)
        {
            if (_tableButtons.TryGetValue(t.TableId, out var tbBtn))
                ApplyTableButtonVisual(tbBtn, t);
        }

        UpdateSelectedTablePanel();
    }

    private void UpdateSelectedTablePanel()
    {
        if (!rdoDineIn.Checked)
        {
            lblSelectedTableInfo.Text = "Hình thức MANG VỀ: Không cần chọn bàn, đơn lấy món từ giỏ hàng Memurai.";
            btnLockTable.Enabled = false;
            btnReleaseLock.Enabled = false;
            btnConfirmBooking.Enabled = true;
            return;
        }

        var table = _currentTables.FirstOrDefault(t => t.TableId == _selectedTableId);
        if (table == null)
        {
            lblSelectedTableInfo.Text = "Chưa chọn bàn. Bấm một bàn Trống (Xanh lá) ở sơ đồ bên trái.";
            btnLockTable.Enabled = false;
            btnReleaseLock.Enabled = false;
            btnConfirmBooking.Enabled = false;
            return;
        }

        bool lockedByMe = table.State == TableVisualState.Locked &&
                          string.Equals(table.LockedByUserCode, SessionContext.UserCode, StringComparison.OrdinalIgnoreCase);
        var capacityWarning = nudGuestCount.Value > table.Capacity
            ? $" (Lưu ý: Số khách {(int)nudGuestCount.Value} > sức chứa {table.Capacity})"
            : string.Empty;

        lblSelectedTableInfo.Text = table.State switch
        {
            TableVisualState.Available =>
                $"Đang chọn: {table.TableName} ({table.Capacity} chỗ - {table.Area}){capacityWarning} • TRỐNG.\nBấm 'Giữ bàn 15p' để khóa chỗ hoặc bấm thẳng 'Chốt đơn'.",
            TableVisualState.Locked when lockedByMe =>
                $"Bàn {table.TableName} đang được CHÍNH BẠN giữ chỗ{capacityWarning} • Còn lại {FormatMmSs(table.RemainingLockSeconds)}.\nBạn có thể bấm 'Chốt đơn' hoặc 'Hủy giữ bàn'.",
            TableVisualState.Locked =>
                $"Bàn {table.TableName} đang bị {table.LockedByUserCode} giữ tạm (còn {FormatMmSs(table.RemainingLockSeconds)}).\nVui lòng chọn bàn khác hoặc đăng ký Waitlist.",
            TableVisualState.Booked =>
                $"Bàn {table.TableName} đã có đơn đặt {table.BookedOrderCode} ({table.BookedStatus}).",
            _ => $"Bàn {table.TableName} hiện ngưng phục vụ."
        };

        btnLockTable.Enabled = table.State == TableVisualState.Available;
        btnReleaseLock.Enabled = table.State == TableVisualState.Locked && (lockedByMe || SessionContext.IsAdmin);
        btnConfirmBooking.Enabled = table.State == TableVisualState.Available || lockedByMe;
    }

    private async void tmrCountdown_Tick(object? sender, EventArgs e)
    {
        if (_currentTables.Count == 0 || IsDisposed) return;

        bool anyExpired = false;
        foreach (var table in _currentTables)
        {
            if (table.State != TableVisualState.Locked) continue;

            if (table.RemainingLockSeconds > 0)
            {
                table.RemainingLockSeconds--;
                if (_tableButtons.TryGetValue(table.TableId, out var btn))
                    ApplyTableButtonVisual(btn, table);
            }

            if (table.RemainingLockSeconds <= 0)
                anyExpired = true;
        }

        if (_selectedTableId != null)
            UpdateSelectedTablePanel();

        // Khi khóa TTL hết hạn (= 0s), tải lại từ Memurai để nhả bàn về màu Xanh tự động.
        if (anyExpired)
            await ReloadTableMapAsync();
    }

    private async void btnLockTable_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedTableId)) return;

        var (resCode, dateStr, slot) = GetSelectedFilterSlot();
        var result = await _service.LockTableAsync(resCode, _selectedTableId, dateStr, slot, SessionContext.UserCode);

        await ReloadTableMapAsync();
        MessageBox.Show(
            result.Message,
            result.Success ? "Giữ bàn thành công (Memurai TTL)" : "Không thể giữ bàn",
            MessageBoxButtons.OK,
            result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }

    private async void btnReleaseLock_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedTableId)) return;

        var (resCode, dateStr, slot) = GetSelectedFilterSlot();
        var result = await _service.ReleaseTableLockAsync(
            resCode, _selectedTableId, dateStr, slot, SessionContext.UserCode, SessionContext.IsAdmin);

        await ReloadTableMapAsync();
        if (!result.Success)
            MessageBox.Show(result.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private async void btnJoinWaitlist_Click(object? sender, EventArgs e)
    {
        var (resCode, dateStr, slot) = GetSelectedFilterSlot();
        var result = await _service.JoinWaitlistAsync(
            resCode,
            dateStr,
            slot,
            SessionContext.UserCode,
            SessionContext.FullName,
            SessionContext.Phone,
            (int)nudGuestCount.Value);

        await ReloadTableMapAsync();
        MessageBox.Show(
            result.Message,
            result.Success ? "Đã vào hàng đợi Waitlist" : "Không thể đăng ký Waitlist",
            MessageBoxButtons.OK,
            result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }

    private async Task LoadCartAsync()
    {
        _currentCart = await _service.GetCartItemsAsync(SessionContext.UserCode);
        if (IsDisposed) return;

        lblCartSource.Text = $"Giỏ món đặt trước từ Memurai Hash ({RedisKeys.Cart(SessionContext.UserCode)}):";
        dgvCart.DataSource = _currentCart.Select(c => new
        {
            MaMon = c.ItemCode,
            TenMon = c.Name,
            Size = c.SizeCode,
            SL = c.Qty,
            DonGia = FormatVnd(c.UnitPrice),
            ThanhTien = FormatVnd(c.Subtotal)
        }).ToList();

        UpdateTotalsPreview();
    }

    private void UpdateTotalsPreview()
    {
        double subtotal = _currentCart.Sum(i => i.Subtotal);
        var (_, discount, total) = BookingService.CalculatePricing(_currentCart, txtVoucher.Text);

        lblSubtotal.Text = $"Tạm tính món ăn ({_currentCart.Sum(x => x.Qty)} món): {FormatVnd(subtotal)}";
        lblDiscount.Text = $"Giảm giá: -{FormatVnd(discount)}";
        lblTotalAmount.Text = $"Tổng thanh toán: {FormatVnd(total)}";
    }

    private async void btnConfirmBooking_Click(object? sender, EventArgs e)
    {
        var (resCode, dateStr, slot) = GetSelectedFilterSlot();
        string orderType = rdoDineIn.Checked ? "dine_in" : "takeaway";
        string payment = (cboPaymentMethod.SelectedItem as ComboOption)?.Value ?? "cash";

        btnConfirmBooking.Enabled = false;
        try
        {
            var result = await _service.CreateBookingOrderAsync(
                resCode,
                orderType,
                _selectedTableId,
                dateStr,
                slot,
                (int)nudGuestCount.Value,
                payment,
                txtVoucher.Text,
                SessionContext.UserCode,
                SessionContext.FullName,
                SessionContext.Phone);

            MessageBox.Show(
                result.Message,
                result.Success ? "Đặt bàn / Chốt đơn thành công" : "Chốt đơn thất bại",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (result.Success)
            {
                _selectedTableId = null;
                txtVoucher.Clear();
                await ReloadTableMapAsync();
                await LoadCartAsync();
                if (SessionContext.IsStaffOrAdmin) await LoadManageOrdersAsync();
            }
            else
            {
                await ReloadTableMapAsync();
            }
        }
        finally
        {
            if (!IsDisposed) UpdateSelectedTablePanel();
        }
    }

    // === TAB 2: QUẢN LÝ & DUYỆT ĐƠN ===

    private async Task LoadManageOrdersAsync()
    {
        var resCode = (cboManageRestaurant.SelectedItem as ComboOption)?.Value ?? "ALL";
        var status = (cboManageStatus.SelectedItem as ComboOption)?.Value ?? "ALL";
        string? dateStr = chkFilterDate.Checked ? BookingService.NormalizeDate(dtpManageDate.Value) : null;

        _manageOrders = await _service.GetOrdersForManageAsync(resCode, dateStr, status);
        if (IsDisposed) return;

        dgvOrders.DataSource = _manageOrders.Select(o => new
        {
            MaDon = o.OrderCode,
            Loai = o.OrderType,
            ChiNhanh = o.RestaurantCode,
            Ban = o.TableId ?? "-",
            Ngay = o.BookingDate,
            Gio = o.TimeSlot,
            KhachHang = o.Customer.FullName,
            SDT = o.Customer.Phone,
            TongTien = FormatVnd(o.TotalAmount),
            TrangThai = o.Status,
            ThanhToan = o.PaymentMethod
        }).ToList();

        await RefreshSelectedOrderWaitlistAsync();
    }

    private BookingOrderModel? GetSelectedManageOrder()
    {
        if (dgvOrders.CurrentRow == null || dgvOrders.CurrentRow.Index < 0 || dgvOrders.CurrentRow.Index >= _manageOrders.Count)
            return null;

        return _manageOrders[dgvOrders.CurrentRow.Index];
    }

    private async void dgvOrders_SelectionChanged(object? sender, EventArgs e)
    {
        if (_isInitializing) return;
        await RefreshSelectedOrderWaitlistAsync();
    }

    private async Task RefreshSelectedOrderWaitlistAsync()
    {
        var order = GetSelectedManageOrder();
        if (order == null || order.OrderType != "dine_in")
        {
            lblManageWaitlistSlot.Text = "Chưa chọn đơn 'ăn tại bàn' để hiển thị hàng đợi Waitlist tương ứng.";
            dgvManageWaitlist.DataSource = null;
            return;
        }

        var waitlist = await _service.GetWaitlistAsync(order.RestaurantCode, order.BookingDate, order.TimeSlot);
        if (IsDisposed) return;

        lblManageWaitlistSlot.Text =
            $"Hàng đợi Waitlist tại {order.RestaurantCode} • {order.TimeSlot} ngày {order.BookingDate} ({waitlist.Count} khách — thứ tự RPOP từ trên xuống):";

        dgvManageWaitlist.DataSource = waitlist.Select((w, idx) => new
        {
            ThuTuFIFO = $"#{idx + 1}",
            MaKH = w.UserCode,
            HoTen = w.FullName,
            SDT = w.Phone,
            SoKhach = w.GuestCount,
            ThoiGianDangKy = w.RequestedAt
        }).ToList();
    }

    private async Task ChangeSelectedOrderStatusAsync(string newStatus)
    {
        var order = GetSelectedManageOrder();
        if (order == null)
        {
            MessageBox.Show("Vui lòng chọn một đơn trong bảng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = await _service.UpdateOrderStatusAsync(order.OrderCode, newStatus);
        MessageBox.Show(
            result.Message,
            result.Success ? "Cập nhật thành công" : "Không hợp lệ",
            MessageBoxButtons.OK,
            result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (result.Success)
        {
            await LoadManageOrdersAsync();
            await ReloadTableMapAsync();
        }
    }

    private async void btnCancelOrder_Click(object? sender, EventArgs e)
    {
        var order = GetSelectedManageOrder();
        if (order == null)
        {
            MessageBox.Show("Vui lòng chọn đơn cần hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Xác nhận hủy đơn {order.OrderCode} (Bàn {order.TableId ?? "Mang về"} - {order.Customer.FullName})?\nHệ thống sẽ tự động rút khách đầu tiên từ Memurai Waitlist (RPOP) nếu có.",
            "Xác nhận hủy đơn",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        var result = await _service.CancelOrderAndPopWaitlistAsync(order.OrderCode);
        MessageBox.Show(
            result.Message,
            result.Success ? "Kết quả hủy đơn & Waitlist" : "Không thể hủy",
            MessageBoxButtons.OK,
            result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (result.Success)
        {
            await LoadManageOrdersAsync();
            await ReloadTableMapAsync();
        }
    }

    private static string FormatMmSs(int totalSeconds)
    {
        var safe = Math.Max(0, totalSeconds);
        return $"{safe / 60:00}:{safe % 60:00}";
    }

    private static string FormatVnd(double amount) => $"{amount:N0} đ";
}
