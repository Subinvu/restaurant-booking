using RestaurantBooking.Config;
using RestaurantBooking.Forms.TV3_User;
using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV1_Booking;

/// <summary>
/// Màn hình Đặt bàn trực tuyến (Sơ đồ bàn, Giữ bàn 15 phút, Danh sách chờ)
/// và Tab Quản lý & Duyệt đơn dành cho Nhân viên / Quản trị viên.
/// </summary>
public partial class FrmBooking : Form
{
    private sealed record ComboOption(string Value, string Label)
    {
        public override string ToString() => Label;
    }

    private readonly BookingService _service = new();
    private readonly Dictionary<string, Button> _tableButtons = new();
    private readonly string? _preselectedRestaurantCode;

    private List<RestaurantModel> _restaurants = [];
    private List<TableStatusDto> _currentTables = [];
    private List<CartItemDto> _currentCart = [];
    private List<BookingOrderModel> _manageOrders = [];
    private string? _selectedTableId;
    private bool _isInitializing = true;
    private bool _isReloadingMap;

    public FrmBooking() : this(null)
    {
    }

    public FrmBooking(string? preselectedRestaurantCode)
    {
        _preselectedRestaurantCode = preselectedRestaurantCode;
        InitializeComponent();
        ApplyCleanTheme();
        Load += FrmBooking_Load;
        FormClosing += (_, _) => tmrCountdown.Stop();
    }

    private void ApplyCleanTheme()
    {
        UiTheme.StyleTabControl(tabMain);
        UiTheme.StyleHeaderBackButton(btnBack);
        UiTheme.StyleSecondaryButton(btnReloadTables);
        UiTheme.StyleSecondaryButton(btnSwitchTakeaway);
        UiTheme.StyleSecondaryButton(btnJoinWaitlist);
        UiTheme.StyleSecondaryButton(btnLockTable);
        UiTheme.StyleSecondaryButton(btnReleaseLock);
        UiTheme.StyleSecondaryButton(btnReloadCart);
        UiTheme.StylePrimaryButton(btnConfirmBooking, useGreen: true);
        UiTheme.StyleSecondaryButton(btnFilterOrders);
        UiTheme.StyleSecondaryButton(btnConfirmOrder);
        UiTheme.StyleSecondaryButton(btnServingOrder);
        UiTheme.StylePrimaryButton(btnCompleteOrder, useGreen: true);
        UiTheme.StyleDangerButton(btnCancelOrder);
        UiTheme.StyleGrid(dgvCart);
        UiTheme.StyleGrid(dgvOrders);
        UiTheme.StyleGrid(dgvManageWaitlist);
    }

    private async void FrmBooking_Load(object? sender, EventArgs e)
    {
        try
        {
            _isInitializing = true;
            lblInfo.Text = $"Đặt bàn trực tuyến — {SessionContext.FullName}";

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
                await _service.CancelExpiredPendingOrdersAsync();
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
        string[] slots = ["11:00", "13:00", "17:30", "19:00", "20:30"];
        cboTimeSlot.Items.Clear();
        cboTimeSlot.Items.AddRange(slots);

        dtpBookingDate.MinDate = DateTime.Today;
        var nowTime = TimeOnly.FromDateTime(DateTime.Now);
        var defaultSlot = slots.FirstOrDefault(s => TimeOnly.Parse(s) > nowTime && s == "19:00")
                          ?? slots.FirstOrDefault(s => TimeOnly.Parse(s) > nowTime);

        if (defaultSlot != null)
        {
            dtpBookingDate.Value = DateTime.Today;
            cboTimeSlot.SelectedItem = defaultSlot;
        }
        else
        {
            dtpBookingDate.Value = DateTime.Today.AddDays(1);
            cboTimeSlot.SelectedItem = "19:00";
        }

        cboPaymentMethod.Items.Clear();
        cboPaymentMethod.Items.AddRange([
            new ComboOption("cash", "Tiền mặt tại quán"),
            new ComboOption("momo", "Ví MoMo"),
            new ComboOption("banking", "Chuyển khoản ngân hàng")
        ]);
        cboPaymentMethod.SelectedIndex = 0;

        cboManageStatus.Items.Clear();
        cboManageStatus.Items.AddRange([
            new ComboOption("ALL", "Tất cả trạng thái"),
            new ComboOption("pending", "Chờ xác nhận"),
            new ComboOption("confirmed", "Đã xác nhận"),
            new ComboOption("serving", "Đang phục vụ"),
            new ComboOption("completed", "Hoàn tất"),
            new ComboOption("cancelled", "Đã hủy")
        ]);
        cboManageStatus.SelectedIndex = 0;

        dtpManageDate.Value = DateTime.Today;
    }

    private async Task LoadRestaurantsAsync()
    {
        _restaurants = await _service.GetActiveRestaurantsAsync();

        cboRestaurant.Items.Clear();
        cboManageRestaurant.Items.Clear();
        cboManageRestaurant.Items.Add(new ComboOption("ALL", "Tất cả chi nhánh"));

        int selectedIdx = 0;
        for (int i = 0; i < _restaurants.Count; i++)
        {
            var r = _restaurants[i];
            var opt = new ComboOption(r.RestaurantCode, r.Name);
            cboRestaurant.Items.Add(opt);
            cboManageRestaurant.Items.Add(opt);

            if (!string.IsNullOrEmpty(_preselectedRestaurantCode) &&
                string.Equals(r.RestaurantCode, _preselectedRestaurantCode, StringComparison.OrdinalIgnoreCase))
            {
                selectedIdx = i;
            }
        }

        if (cboRestaurant.Items.Count > 0) cboRestaurant.SelectedIndex = selectedIdx;
        if (cboManageRestaurant.Items.Count > 0) cboManageRestaurant.SelectedIndex = 0;
    }

    private (string RestaurantCode, string RestaurantName, string BookingDate, string TimeSlot) GetSelectedFilterSlot()
    {
        var opt = cboRestaurant.SelectedItem as ComboOption;
        var resCode = opt?.Value ?? "RES01";
        var resName = opt?.Label ?? "Nhà hàng Hương Việt";
        var dateStr = BookingService.NormalizeDate(dtpBookingDate.Value);
        var slot = cboTimeSlot.SelectedItem?.ToString() ?? "19:00";
        return (resCode, resName, dateStr, slot);
    }

    private string GetBranchDisplayName(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return "-";
        return _restaurants.FirstOrDefault(r =>
            string.Equals(r.RestaurantCode, code, StringComparison.OrdinalIgnoreCase))?.Name ?? code;
    }

    private async void FilterInputs_Changed(object? sender, EventArgs e)
    {
        if (_isInitializing) return;
        try
        {
            _selectedTableId = null;
            await ReloadTableMapAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải sơ đồ bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task ReloadTableMapAsync()
    {
        if (IsDisposed || _isReloadingMap) return;

        _isReloadingMap = true;
        try
        {
            var (resCode, resName, dateStr, slot) = GetSelectedFilterSlot();
            var tables = await _service.GetTableStatusesAsync(resCode, dateStr, slot);
            var waitlist = await _service.GetWaitlistAsync(resCode, dateStr, slot);
            if (IsDisposed) return;

            _currentTables = tables;
            if (_selectedTableId != null && _currentTables.All(t => t.TableId != _selectedTableId))
                _selectedTableId = null;

            RenderTableButtons();
            UpdateSelectedTablePanel();
            lblWaitlistCount.Text = $"Danh sách chờ khung {slot}: {waitlist.Count} khách đang chờ";
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
            "vip" => "Phòng VIP",
            "outdoor" => "Sân vườn",
            _ => "Trong nhà"
        };

        bool lockedByMe = table.State == TableVisualState.Locked &&
                          string.Equals(table.LockedByUserCode, SessionContext.UserCode, StringComparison.OrdinalIgnoreCase);

        string statusLine = table.State switch
        {
            TableVisualState.Available => "TRỐNG",
            TableVisualState.Locked when lockedByMe => $"BẠN ĐANG GIỮ\n{FormatMmSs(table.RemainingLockSeconds)}",
            TableVisualState.Locked => $"ĐANG GIỮ CHỖ\n{FormatMmSs(table.RemainingLockSeconds)}",
            TableVisualState.Booked when SessionContext.IsStaffOrAdmin => $"ĐÃ ĐẶT\n{table.BookedOrderCode}",
            TableVisualState.Booked => "ĐÃ ĐẶT",
            _ => "NGƯNG PHỤC VỤ"
        };

        btn.Text = $"{table.TableName} ({table.Capacity} chỗ)\n[{areaText}]\n{statusLine}";

        (Color back, Color fore, Color border) = table.State switch
        {
            TableVisualState.Available => (Color.FromArgb(240, 253, 244), Color.FromArgb(22, 101, 52), Color.FromArgb(134, 239, 172)),
            TableVisualState.Locked => (Color.FromArgb(255, 251, 235), Color.FromArgb(146, 64, 14), Color.FromArgb(252, 211, 77)),
            TableVisualState.Booked => (Color.FromArgb(254, 242, 242), Color.FromArgb(153, 27, 27), Color.FromArgb(252, 165, 165)),
            _ => (Color.FromArgb(241, 245, 249), Color.FromArgb(100, 116, 139), Color.FromArgb(203, 213, 225))
        };

        btn.BackColor = back;
        btn.ForeColor = fore;

        bool isSelected = string.Equals(_selectedTableId, table.TableId, StringComparison.Ordinal);
        btn.FlatAppearance.BorderSize = isSelected ? 3 : 1;
        btn.FlatAppearance.BorderColor = isSelected ? UiTheme.HeaderBg : border;
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
        var table = _currentTables.FirstOrDefault(t => t.TableId == _selectedTableId);
        if (table == null)
        {
            lblSelectedTableInfo.Text = "Chưa chọn bàn. Bấm vào một bàn Trống ở sơ đồ bên trái.";
            btnLockTable.Enabled = false;
            btnReleaseLock.Enabled = false;
            btnConfirmBooking.Enabled = false;
            return;
        }

        bool lockedByMe = table.State == TableVisualState.Locked &&
                          string.Equals(table.LockedByUserCode, SessionContext.UserCode, StringComparison.OrdinalIgnoreCase);
        var capacityWarning = nudGuestCount.Value > table.Capacity
            ? $" (Lưu ý: số khách {(int)nudGuestCount.Value} > sức chứa {table.Capacity})"
            : string.Empty;

        lblSelectedTableInfo.Text = table.State switch
        {
            TableVisualState.Available =>
                $"Đang chọn: {table.TableName} ({table.Capacity} chỗ){capacityWarning} — Còn trống.\nBạn có thể bấm 'Giữ bàn (15 phút)' hoặc 'Xác nhận đặt bàn' ngay.",
            TableVisualState.Locked when lockedByMe =>
                $"{table.TableName} đang được bạn giữ chỗ{capacityWarning} (còn {FormatMmSs(table.RemainingLockSeconds)}).\nBấm 'Xác nhận đặt bàn' để hoàn tất.",
            TableVisualState.Locked =>
                $"{table.TableName} đang có khách giữ tạm (còn {FormatMmSs(table.RemainingLockSeconds)}).\nVui lòng chọn bàn khác hoặc đăng ký danh sách chờ.",
            TableVisualState.Booked =>
                $"{table.TableName} đã có khách đặt trong khung giờ này.",
            _ => $"{table.TableName} hiện đang tạm ngưng phục vụ."
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

        if (anyExpired)
        {
            try
            {
                await ReloadTableMapAsync();
            }
            catch
            {
            }
        }
    }

    private async void btnLockTable_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedTableId)) return;

        btnLockTable.Enabled = false;
        try
        {
            var (resCode, _, dateStr, slot) = GetSelectedFilterSlot();
            var result = await _service.LockTableAsync(resCode, _selectedTableId, dateStr, slot, SessionContext.UserCode);

            await ReloadTableMapAsync();
            MessageBox.Show(
                result.Message,
                result.Success ? "Giữ bàn thành công" : "Không thể giữ bàn",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi giữ bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) UpdateSelectedTablePanel();
        }
    }

    private async void btnReleaseLock_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedTableId)) return;

        btnReleaseLock.Enabled = false;
        try
        {
            var (resCode, _, dateStr, slot) = GetSelectedFilterSlot();
            var result = await _service.ReleaseTableLockAsync(
                resCode, _selectedTableId, dateStr, slot, SessionContext.UserCode, SessionContext.IsAdmin);

            await ReloadTableMapAsync();
            if (!result.Success)
                MessageBox.Show(result.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi bỏ giữ bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) UpdateSelectedTablePanel();
        }
    }

    private async void btnJoinWaitlist_Click(object? sender, EventArgs e)
    {
        btnJoinWaitlist.Enabled = false;
        try
        {
            var (resCode, _, dateStr, slot) = GetSelectedFilterSlot();
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
                result.Success ? "Đã đăng ký danh sách chờ" : "Không thể đăng ký danh sách chờ",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi đăng ký danh sách chờ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnJoinWaitlist.Enabled = true;
        }
    }

    private async Task LoadCartAsync()
    {
        _currentCart = await _service.GetCartItemsAsync(SessionContext.UserCode);
        if (IsDisposed) return;

        dgvCart.DataSource = _currentCart.Select(c => new
        {
            TenMon = c.Name,
            Size = c.SizeCode,
            SL = c.Qty,
            DonGia = FormatVnd(c.UnitPrice),
            ThanhTien = FormatVnd(c.Subtotal)
        }).ToList();

        if (dgvCart.Columns.Count >= 5)
        {
            dgvCart.Columns[0].HeaderText = "Tên món";
            dgvCart.Columns[0].FillWeight = 148;
            dgvCart.Columns[1].HeaderText = "Cỡ";
            dgvCart.Columns[1].FillWeight = 50;
            dgvCart.Columns[2].HeaderText = "SL";
            dgvCart.Columns[2].FillWeight = 44;
            dgvCart.Columns[3].HeaderText = "Đơn giá";
            dgvCart.Columns[3].FillWeight = 84;
            dgvCart.Columns[4].HeaderText = "Thành tiền";
            dgvCart.Columns[4].FillWeight = 94;
        }

        UpdateTotalsPreview();
    }

    private void UpdateTotalsPreview()
    {
        double subtotal = _currentCart.Sum(i => i.Subtotal);
        var (_, discount, total) = BookingService.CalculatePricing(_currentCart, txtVoucher.Text);

        lblSubtotal.Text = $"Tạm tính món ăn ({_currentCart.Sum(x => x.Qty)} phần): {FormatVnd(subtotal)}";
        lblDiscount.Text = discount > 0 ? $"Giảm giá: -{FormatVnd(discount)}" : "Giảm giá: 0 đ";
        lblTotalAmount.Text = $"Tổng thanh toán: {FormatVnd(total)}";
    }

    private async void btnConfirmBooking_Click(object? sender, EventArgs e)
    {
        var (resCode, _, dateStr, slot) = GetSelectedFilterSlot();
        string payment = (cboPaymentMethod.SelectedItem as ComboOption)?.Value ?? "cash";

        btnConfirmBooking.Enabled = false;
        try
        {
            var result = await _service.CreateBookingOrderAsync(
                resCode,
                "dine_in",
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
                result.Success ? "Đặt bàn thành công" : "Không thể đặt bàn",
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
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi xác nhận đặt bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) UpdateSelectedTablePanel();
        }
    }

    private async void btnSwitchTakeaway_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadCartAsync();
            if (_currentCart.Count == 0)
            {
                MessageBox.Show(
                    "Giỏ hàng của bạn đang trống. Vui lòng chọn món ở mục 'Thực đơn & Đặt món' trước khi đặt mang về / giao hàng.",
                    "Chưa có món ăn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var (resCode, _, _, _) = GetSelectedFilterSlot();
            var branch = _restaurants.FirstOrDefault(r =>
                string.Equals(r.RestaurantCode, resCode, StringComparison.OrdinalIgnoreCase));

            Hide();
            try
            {
                using var checkout = new FrmTakeawayCheckout(branch);
                checkout.ShowDialog(this);
            }
            finally
            {
                if (!IsDisposed)
                {
                    Show();
                    Activate();
                }
            }

            await LoadCartAsync();
            if (SessionContext.IsStaffOrAdmin) await LoadManageOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi mở trang thanh toán mang về: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Loai = o.OrderType == "dine_in" ? "Tại bàn" : "Mang về",
            ChiNhanh = GetBranchDisplayName(o.RestaurantCode),
            Ban = o.TableId ?? (string.IsNullOrWhiteSpace(o.DeliveryAddress) ? "Mang về" : "Giao hàng"),
            Ngay = o.BookingDate,
            Gio = o.TimeSlot,
            KhachHang = o.Customer.FullName,
            SDT = o.Customer.Phone,
            TongTien = FormatVnd(o.TotalAmount),
            TrangThai = FormatOrderStatus(o.Status),
            ThanhToan = FormatPaymentMethod(o.PaymentMethod)
        }).ToList();

        if (dgvOrders.Columns.Count >= 11)
        {
            dgvOrders.Columns[0].HeaderText = "Mã đơn";
            dgvOrders.Columns[0].FillWeight = 138;
            dgvOrders.Columns[1].HeaderText = "Hình thức";
            dgvOrders.Columns[1].FillWeight = 78;
            dgvOrders.Columns[2].HeaderText = "Chi nhánh";
            dgvOrders.Columns[2].FillWeight = 122;
            dgvOrders.Columns[3].HeaderText = "Bàn";
            dgvOrders.Columns[3].FillWeight = 62;
            dgvOrders.Columns[4].HeaderText = "Ngày đặt";
            dgvOrders.Columns[4].FillWeight = 86;
            dgvOrders.Columns[5].HeaderText = "Giờ";
            dgvOrders.Columns[5].FillWeight = 56;
            dgvOrders.Columns[6].HeaderText = "Khách hàng";
            dgvOrders.Columns[6].FillWeight = 114;
            dgvOrders.Columns[7].HeaderText = "SĐT";
            dgvOrders.Columns[7].FillWeight = 90;
            dgvOrders.Columns[8].HeaderText = "Tổng tiền";
            dgvOrders.Columns[8].FillWeight = 88;
            dgvOrders.Columns[9].HeaderText = "Trạng thái";
            dgvOrders.Columns[9].FillWeight = 94;
            dgvOrders.Columns[10].HeaderText = "Thanh toán";
            dgvOrders.Columns[10].FillWeight = 88;
        }

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
        try
        {
            await RefreshSelectedOrderWaitlistAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách chờ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task RefreshSelectedOrderWaitlistAsync()
    {
        var order = GetSelectedManageOrder();
        if (order == null || order.OrderType != "dine_in")
        {
            lblManageWaitlistSlot.Text = "Chọn một đơn ăn tại bàn ở trên để xem danh sách chờ cùng khung giờ.";
            dgvManageWaitlist.DataSource = null;
            return;
        }

        var waitlist = await _service.GetWaitlistAsync(order.RestaurantCode, order.BookingDate, order.TimeSlot);
        if (IsDisposed) return;

        lblManageWaitlistSlot.Text =
            $"Danh sách chờ tại {GetBranchDisplayName(order.RestaurantCode)} • {order.TimeSlot} ngày {order.BookingDate} ({waitlist.Count} khách):";

        dgvManageWaitlist.DataSource = waitlist.Select((w, idx) => new
        {
            ThuTuFIFO = $"#{idx + 1}",
            HoTen = w.FullName,
            SDT = w.Phone,
            SoKhach = w.GuestCount,
            ThoiGianDangKy = w.RequestedAt
        }).ToList();

        if (dgvManageWaitlist.Columns.Count >= 5)
        {
            dgvManageWaitlist.Columns[0].HeaderText = "Thứ tự";
            dgvManageWaitlist.Columns[1].HeaderText = "Họ và tên";
            dgvManageWaitlist.Columns[2].HeaderText = "Số điện thoại";
            dgvManageWaitlist.Columns[3].HeaderText = "Số khách";
            dgvManageWaitlist.Columns[4].HeaderText = "Thời điểm đăng ký";
        }
    }

    private async Task ChangeSelectedOrderStatusAsync(string newStatus)
    {
        var order = GetSelectedManageOrder();
        if (order == null)
        {
            MessageBox.Show("Vui lòng chọn một đơn trong bảng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            var result = await _service.UpdateOrderStatusAsync(order.OrderCode, newStatus, SessionContext.IsStaffOrAdmin);
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
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi cập nhật trạng thái đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            $"Xác nhận hủy đơn {order.OrderCode} ({(string.IsNullOrEmpty(order.TableId) ? "Mang về" : $"Bàn {order.TableId}")} - {order.Customer.FullName})?",
            "Xác nhận hủy đơn",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            var result = await _service.CancelOrderAndPopWaitlistAsync(
                order.OrderCode,
                SessionContext.UserCode,
                SessionContext.IsStaffOrAdmin);
            MessageBox.Show(
                result.Message,
                result.Success ? "Đã hủy đơn" : "Không thể hủy",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (result.Success)
            {
                await LoadManageOrdersAsync();
                await ReloadTableMapAsync();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi hủy đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnReloadTables_Click(object? sender, EventArgs e)
    {
        try
        {
            await ReloadTableMapAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi làm mới sơ đồ bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnReloadCart_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadCartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void txtVoucher_TextChanged(object? sender, EventArgs e) => UpdateTotalsPreview();

    private void chkFilterDate_CheckedChanged(object? sender, EventArgs e) =>
        dtpManageDate.Enabled = chkFilterDate.Checked;

    private async void btnFilterOrders_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadManageOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lọc đơn đặt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnConfirmOrder_Click(object? sender, EventArgs e) =>
        await ChangeSelectedOrderStatusAsync("confirmed");

    private async void btnServingOrder_Click(object? sender, EventArgs e) =>
        await ChangeSelectedOrderStatusAsync("serving");

    private async void btnCompleteOrder_Click(object? sender, EventArgs e) =>
        await ChangeSelectedOrderStatusAsync("completed");

    private async void tabMain_SelectedIndexChanged(object? sender, EventArgs e)
    {
        try
        {
            if (tabMain.SelectedTab == tabManageOrders)
                await LoadManageOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnBack_Click(object? sender, EventArgs e) => Close();

    private static string FormatOrderStatus(string status) => status switch
    {
        "pending" => "Chờ xác nhận",
        "confirmed" => "Đã xác nhận",
        "serving" => "Đang phục vụ",
        "completed" => "Hoàn tất",
        "cancelled" => "Đã hủy",
        _ => status
    };

    private static string FormatPaymentMethod(string payment) => payment switch
    {
        "momo" => "Ví MoMo",
        "banking" => "Chuyển khoản",
        _ => "Tiền mặt"
    };

    private static string FormatMmSs(int totalSeconds)
    {
        var safe = Math.Max(0, totalSeconds);
        return $"{safe / 60:00}:{safe % 60:00}";
    }

    private static string FormatVnd(double amount) => $"{amount:N0} đ";
}
