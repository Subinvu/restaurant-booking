using RestaurantBooking.Config;
using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// Màn hình thanh toán riêng cho đơn Mang về / Giao hàng tận nơi:
/// xác nhận danh sách món, thông tin người nhận, địa chỉ giao hàng và phương thức thanh toán.
/// </summary>
public partial class FrmTakeawayCheckout : Form
{
    private sealed record PaymentChoice(string Value, string Label)
    {
        public override string ToString() => Label;
    }

    private readonly BookingService _bookingService = new();
    private readonly AuthService _authService = new();
    private readonly MenuAdminService _menuService = new();

    private RestaurantModel? _restaurant;
    private List<CartItemDto> _cartItems = [];
    private string _savedUserAddress = string.Empty;

    public bool OrderCompleted { get; private set; }

    public FrmTakeawayCheckout() : this(null)
    {
    }

    public FrmTakeawayCheckout(RestaurantModel? restaurant)
    {
        _restaurant = restaurant;
        InitializeComponent();
        UiTheme.StyleHeaderBackButton(btnBack);
        UiTheme.StyleGrid(dgvItems);
        UiTheme.StylePrimaryButton(btnConfirmOrder, useGreen: true);
        Load += FrmTakeawayCheckout_Load;
    }

    private async void FrmTakeawayCheckout_Load(object? sender, EventArgs e)
    {
        try
        {
            if (_restaurant == null)
            {
                var list = await _menuService.GetRestaurantsAsync(activeOnly: true);
                _restaurant = list.FirstOrDefault();
            }

            if (_restaurant == null)
            {
                MessageBox.Show("Không tìm thấy chi nhánh phục vụ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            lblBranchSummary.Text = $"Chi nhánh phục vụ: {_restaurant.Name}\n{_restaurant.Address}  •  SĐT: {_restaurant.Phone}";

            // Nạp khung giờ hợp lệ
            string[] slots = ["11:00", "13:00", "17:30", "19:00", "20:30"];
            cboTimeSlot.Items.Clear();
            cboTimeSlot.Items.AddRange(slots);

            dtpReceiveDate.MinDate = DateTime.Today;
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);
            var defaultSlot = slots.FirstOrDefault(s => TimeOnly.Parse(s) > nowTime);
            if (defaultSlot != null)
            {
                dtpReceiveDate.Value = DateTime.Today;
                cboTimeSlot.SelectedItem = defaultSlot;
            }
            else
            {
                dtpReceiveDate.Value = DateTime.Today.AddDays(1);
                cboTimeSlot.SelectedItem = "11:00";
            }

            cboPaymentMethod.Items.Clear();
            cboPaymentMethod.Items.AddRange([
                new PaymentChoice("cash", "Tiền mặt khi nhận hàng"),
                new PaymentChoice("momo", "Ví MoMo"),
                new PaymentChoice("banking", "Chuyển khoản ngân hàng")
            ]);
            cboPaymentMethod.SelectedIndex = 0;

            // Điền sẵn thông tin người nhận & địa chỉ mặc định của khách hàng
            txtReceiverName.Text = SessionContext.FullName;
            txtReceiverPhone.Text = SessionContext.Phone;

            var user = await _authService.GetUserByCodeAsync(SessionContext.UserCode);
            if (user != null)
            {
                var defAddr = user.Addresses.FirstOrDefault(a => a.IsDefault) ?? user.Addresses.FirstOrDefault();
                if (defAddr != null)
                {
                    _savedUserAddress = $"{defAddr.Street}, {defAddr.District}, {defAddr.City}";
                    txtDeliveryAddress.Text = _savedUserAddress;
                }
            }

            await LoadCartAsync();
            ApplyReceiveModeUi();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khởi tạo trang thanh toán: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadCartAsync()
    {
        _cartItems = await _bookingService.GetCartItemsAsync(SessionContext.UserCode);
        if (IsDisposed) return;

        dgvItems.DataSource = _cartItems.Select(c => new
        {
            TenMon = c.Name,
            Size = c.SizeCode,
            SL = c.Qty,
            Topping = c.Toppings.Count > 0 ? string.Join(", ", c.Toppings) : "-",
            DonGia = $"{c.UnitPrice:N0} đ",
            ThanhTien = $"{c.Subtotal:N0} đ"
        }).ToList();

        if (dgvItems.Columns["TenMon"] is { } c1) { c1.HeaderText = "Tên món"; c1.FillWeight = 150; }
        if (dgvItems.Columns["Size"] is { } c2) { c2.HeaderText = "Cỡ"; c2.FillWeight = 50; }
        if (dgvItems.Columns["SL"] is { } c3) { c3.HeaderText = "SL"; c3.FillWeight = 46; }
        if (dgvItems.Columns["Topping"] is { } c4) { c4.HeaderText = "Topping"; c4.FillWeight = 110; }
        if (dgvItems.Columns["DonGia"] is { } c5) { c5.HeaderText = "Đơn giá"; c5.FillWeight = 84; }
        if (dgvItems.Columns["ThanhTien"] is { } c6) { c6.HeaderText = "Thành tiền"; c6.FillWeight = 92; }

        UpdateTotals();
        btnConfirmOrder.Enabled = _cartItems.Count > 0;
    }

    private void UpdateTotals()
    {
        double subtotal = _cartItems.Sum(i => i.Subtotal);
        var (_, discount, total) = BookingService.CalculatePricing(_cartItems, txtVoucher.Text);

        lblSubtotal.Text = $"Tạm tính ({_cartItems.Sum(i => i.Qty)} phần): {subtotal:N0} đ";
        lblDiscount.Text = discount > 0 ? $"Giảm giá: -{discount:N0} đ" : "Giảm giá: 0 đ";
        lblTotalAmount.Text = $"Tổng thanh toán: {total:N0} đ";
    }

    private void ApplyReceiveModeUi()
    {
        bool isDelivery = rdoDelivery.Checked;
        txtDeliveryAddress.ReadOnly = !isDelivery;
        txtDeliveryAddress.BackColor = isDelivery ? Color.White : UiTheme.SubtleBg;

        if (isDelivery)
        {
            lblAddress.Text = "Địa chỉ giao hàng (*):";
            if (string.IsNullOrWhiteSpace(txtDeliveryAddress.Text) ||
                txtDeliveryAddress.Text.StartsWith("Nhận trực tiếp tại:", StringComparison.Ordinal))
            {
                txtDeliveryAddress.Text = _savedUserAddress;
            }
        }
        else
        {
            lblAddress.Text = "Địa điểm nhận:";
            txtDeliveryAddress.Text = _restaurant != null
                ? $"Nhận trực tiếp tại: {_restaurant.Name} — {_restaurant.Address}"
                : "Nhận trực tiếp tại chi nhánh";
        }
    }

    private void ReceiveMode_Changed(object? sender, EventArgs e) => ApplyReceiveModeUi();

    private void txtVoucher_TextChanged(object? sender, EventArgs e) => UpdateTotals();

    private async void btnConfirmOrder_Click(object? sender, EventArgs e)
    {
        if (_restaurant == null) return;

        var receiverName = txtReceiverName.Text.Trim();
        var receiverPhone = txtReceiverPhone.Text.Trim();
        var address = txtDeliveryAddress.Text.Trim();

        if (receiverName.Length < 2)
        {
            MessageBox.Show("Vui lòng nhập họ tên người nhận.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtReceiverName.Focus();
            return;
        }

        if (receiverPhone.Length < 9)
        {
            MessageBox.Show("Vui lòng nhập số điện thoại liên hệ hợp lệ.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtReceiverPhone.Focus();
            return;
        }

        if (rdoDelivery.Checked && address.Length < 5)
        {
            MessageBox.Show("Vui lòng nhập địa chỉ giao hàng chi tiết.", "Thiếu địa chỉ giao hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtDeliveryAddress.Focus();
            return;
        }

        btnConfirmOrder.Enabled = false;
        try
        {
            var dateStr = BookingService.NormalizeDate(dtpReceiveDate.Value);
            var slot = cboTimeSlot.SelectedItem?.ToString() ?? "19:00";
            var payment = (cboPaymentMethod.SelectedItem as PaymentChoice)?.Value ?? "cash";

            var result = await _bookingService.CreateBookingOrderAsync(
                _restaurant.RestaurantCode,
                "takeaway",
                tableId: null,
                dateStr,
                slot,
                guestCount: 1,
                payment,
                txtVoucher.Text,
                SessionContext.UserCode,
                receiverName,
                receiverPhone,
                deliveryAddress: address,
                note: txtNote.Text);

            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Không thể đặt hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OrderCompleted = true;
            MessageBox.Show(
                $"Đặt hàng thành công!\n\n" +
                $"Mã đơn: {result.Data?.OrderCode}\n" +
                $"Chi nhánh: {_restaurant.Name}\n" +
                $"Địa điểm nhận: {address}\n" +
                $"Tổng thanh toán: {result.Data?.TotalAmount:N0} đ",
                "Đặt hàng thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi xác nhận đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnConfirmOrder.Enabled = _cartItems.Count > 0;
        }
    }

    private void btnBack_Click(object? sender, EventArgs e) => Close();
}
