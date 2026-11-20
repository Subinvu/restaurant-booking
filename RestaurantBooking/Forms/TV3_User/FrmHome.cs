using RestaurantBooking.Config;
using RestaurantBooking.Forms.TV1_Booking;
using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// Màn hình Thực đơn chi nhánh & Giỏ hàng dành cho khách hàng.
/// Đã tách chọn chi nhánh ra FrmBranchSelect và tách thanh toán mang về/giao hàng ra FrmTakeawayCheckout.
/// </summary>
public partial class FrmHome : Form
{
    private sealed record SizeChoice(SizeOption Option)
    {
        public override string ToString() => $"{Option.SizeCode} — {Option.Label} (+{Option.ExtraPrice:N0} đ)";
    }

    private sealed record ToppingChoice(ToppingOption Option)
    {
        public override string ToString() => $"{Option.Name} (+{Option.Price:N0} đ)";
    }

    private readonly MenuAdminService _menuService = new();
    private readonly CartReviewService _cartService = new();

    private RestaurantModel? _restaurant;
    private Dictionary<string, string> _branchNameMap = new(StringComparer.OrdinalIgnoreCase);
    private List<MenuItemModel> _branchMenu = [];
    private List<MenuItemModel> _filteredMenu = [];
    private List<CartItemDto> _cartItems = [];
    private MenuItemModel? _selectedDish;

    public bool ReturnToMainRequested { get; private set; }

    public FrmHome() : this(null)
    {
    }

    public FrmHome(RestaurantModel? selectedRestaurant)
    {
        _restaurant = selectedRestaurant;
        InitializeComponent();
        ApplyCleanTheme();
        Load += FrmHome_Load;
    }

    private void ApplyCleanTheme()
    {
        UiTheme.StyleHeaderBackButton(btnChangeBranch);
        UiTheme.StyleHeaderBackButton(btnBackMain);
        UiTheme.StyleSecondaryButton(btnLoadMenu);
        UiTheme.StyleSecondaryButton(btnQtyPlus);
        UiTheme.StyleSecondaryButton(btnQtyMinus);
        UiTheme.StyleDangerButton(btnRemoveItem);
        UiTheme.StyleSecondaryButton(btnClearCart);
        UiTheme.StylePrimaryButton(btnAddToCart);
        UiTheme.StylePrimaryButton(btnBookTable, useGreen: true);
        UiTheme.StylePrimaryButton(btnTakeawayCheckout);
        UiTheme.StyleGrid(dgvMenu);
        UiTheme.StyleGrid(dgvTopHot);
        UiTheme.StyleGrid(dgvCart);
    }

    private void cboCategory_SelectedIndexChanged(object? sender, EventArgs e) => ApplyMenuFilter();
    private void txtSearch_TextChanged(object? sender, EventArgs e) => ApplyMenuFilter();
    private void dgvMenu_SelectionChanged(object? sender, EventArgs e) => OnMenuSelected();
    private void cboSize_SelectedIndexChanged(object? sender, EventArgs e) => UpdateDishUnitPreview();
    private void nudQty_ValueChanged(object? sender, EventArgs e) => UpdateDishUnitPreview();
    private void clbToppings_ItemCheck(object? sender, ItemCheckEventArgs e) => BeginInvoke(UpdateDishUnitPreview);

    private void UpdateHeaderBanner()
    {
        var branchName = _restaurant?.Name ?? "Nhà hàng Hương Việt";
        lblInfo.Text = SessionContext.IsLoggedIn
            ? $"Thực đơn — {branchName}  ({SessionContext.FullName})"
            : $"Thực đơn — {branchName}";
        lblBranchAddress.Text = _restaurant != null
            ? $"{_restaurant.Address}  •  Mở cửa {_restaurant.OpenTime} – {_restaurant.CloseTime}"
            : "Chọn món yêu thích để đặt bàn tại quán hoặc đặt mang về / giao hàng";
    }

    private async void FrmHome_Load(object? sender, EventArgs e)
    {
        try
        {
            var restaurants = await _menuService.GetRestaurantsAsync(activeOnly: true);
            _branchNameMap = restaurants.ToDictionary(r => r.RestaurantCode, r => r.Name, StringComparer.OrdinalIgnoreCase);

            _restaurant ??= restaurants.FirstOrDefault();
            UpdateHeaderBanner();

            cboCategory.Items.Clear();
            cboCategory.Items.AddRange(["Tất cả nhóm món", "Khai vị", "Món chính", "Lẩu & Nướng", "Đồ uống", "Tráng miệng"]);
            cboCategory.SelectedIndex = 0;

            await LoadRestaurantMenuAsync(incrementViews: true);
            await LoadTopHotAsync();
            await LoadCartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khởi tạo màn hình Thực đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string CurrentRestaurantCode => _restaurant?.RestaurantCode ?? "RES01";

    private string GetBranchDisplayName(string? resCode)
    {
        if (string.IsNullOrWhiteSpace(resCode)) return string.Empty;
        return _branchNameMap.TryGetValue(resCode, out var name) ? name : resCode;
    }

    private async void btnLoadMenu_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadRestaurantMenuAsync(incrementViews: false);
            await LoadTopHotAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải lại thực đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadRestaurantMenuAsync(bool incrementViews)
    {
        var resCode = CurrentRestaurantCode;
        if (incrementViews)
        {
            // Ghi nhận lượt xem ngầm phục vụ báo cáo thống kê của quản trị viên
            await _menuService.IncrementViewsAsync(resCode);
        }

        var cacheRes = await _menuService.GetMenuByRestaurantWithCacheAsync(resCode, includeUnavailable: false);
        if (IsDisposed) return;

        _branchMenu = cacheRes.Items;
        ApplyMenuFilter();
    }

    private void ApplyMenuFilter()
    {
        var cat = cboCategory.SelectedIndex > 0 ? cboCategory.SelectedItem?.ToString() : null;
        var keyword = txtSearch.Text.Trim();

        _filteredMenu = _branchMenu
            .Where(m => string.IsNullOrEmpty(cat) || string.Equals(m.Category, cat, StringComparison.OrdinalIgnoreCase))
            .Where(m => string.IsNullOrEmpty(keyword) ||
                        m.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        dgvMenu.DataSource = _filteredMenu.Select(m => new
        {
            TenMon = m.Name,
            Nhom = m.Category,
            GiaGoc = $"{m.BasePrice:N0} đ",
            DanhGia = $"{m.AvgRating:0.0} ★",
            Topping = m.Toppings.Count > 0 ? $"{m.Toppings.Count} lựa chọn" : "Không"
        }).ToList();

        if (dgvMenu.Columns["TenMon"] is { } c1) { c1.HeaderText = "Tên món"; c1.FillWeight = 185; }
        if (dgvMenu.Columns["Nhom"] is { } c2) { c2.HeaderText = "Nhóm món"; c2.FillWeight = 95; }
        if (dgvMenu.Columns["GiaGoc"] is { } c3) { c3.HeaderText = "Giá"; c3.FillWeight = 82; }
        if (dgvMenu.Columns["DanhGia"] is { } c4) { c4.HeaderText = "Đánh giá"; c4.FillWeight = 66; }
        if (dgvMenu.Columns["Topping"] is { } c5) { c5.HeaderText = "Topping"; c5.FillWeight = 78; }

        OnMenuSelected();
    }

    private void OnMenuSelected()
    {
        int idx = dgvMenu.CurrentRow?.Index ?? -1;
        if (idx < 0 || idx >= _filteredMenu.Count)
        {
            _selectedDish = null;
            lblSelectedDish.Text = "Chưa chọn món ăn.";
            cboSize.Items.Clear();
            clbToppings.Items.Clear();
            btnAddToCart.Enabled = false;
            lblUnitPreview.Text = "Tạm tính: 0 đ";
            return;
        }

        _selectedDish = _filteredMenu[idx];
        lblSelectedDish.Text = $"{_selectedDish.Name}  •  {_selectedDish.BasePrice:N0} đ";

        cboSize.Items.Clear();
        var sizes = _selectedDish.Sizes.Count > 0
            ? _selectedDish.Sizes
            : [new SizeOption { SizeCode = "S", Label = "Phần vừa", ExtraPrice = 0 }];
        foreach (var s in sizes)
            cboSize.Items.Add(new SizeChoice(s));
        cboSize.SelectedIndex = 0;

        clbToppings.Items.Clear();
        foreach (var top in _selectedDish.Toppings)
            clbToppings.Items.Add(new ToppingChoice(top));

        nudQty.Value = 1;
        btnAddToCart.Enabled = true;
        UpdateDishUnitPreview();
    }

    private void UpdateDishUnitPreview()
    {
        if (_selectedDish == null) return;

        double sizeExtra = (cboSize.SelectedItem as SizeChoice)?.Option.ExtraPrice ?? 0;
        double topSum = GetSelectedToppings().Sum(t => t.Price);
        double unit = _selectedDish.BasePrice + sizeExtra + topSum;
        double line = unit * (int)nudQty.Value;

        lblUnitPreview.Text = $"Đơn giá: {unit:N0} đ  •  Thành tiền ({(int)nudQty.Value} phần): {line:N0} đ";
    }

    private List<ToppingOption> GetSelectedToppings()
    {
        var list = new List<ToppingOption>();
        foreach (var item in clbToppings.CheckedItems)
        {
            if (item is ToppingChoice tc)
                list.Add(tc.Option);
        }
        return list;
    }

    private async Task LoadTopHotAsync()
    {
        var topItems = await _menuService.GetTopRankedItemsAsync(5);
        if (IsDisposed) return;

        dgvTopHot.DataSource = topItems.Select(t => new
        {
            Hang = $"#{t.Rank}",
            TenMon = t.Name,
            ChiNhanh = GetBranchDisplayName(t.RestaurantCode),
            GiaGoc = $"{t.BasePrice:N0} đ",
            LuotDat = $"{t.Score:0}"
        }).ToList();

        if (dgvTopHot.Columns["Hang"] is { } c1) { c1.HeaderText = "Hạng"; c1.FillWeight = 46; }
        if (dgvTopHot.Columns["TenMon"] is { } c2) { c2.HeaderText = "Tên món"; c2.FillWeight = 185; }
        if (dgvTopHot.Columns["ChiNhanh"] is { } c3) { c3.HeaderText = "Chi nhánh"; c3.FillWeight = 130; }
        if (dgvTopHot.Columns["GiaGoc"] is { } c4) { c4.HeaderText = "Giá"; c4.FillWeight = 78; }
        if (dgvTopHot.Columns["LuotDat"] is { } c5) { c5.HeaderText = "Lượt đặt"; c5.FillWeight = 62; }
    }

    private async Task LoadCartAsync()
    {
        if (!SessionContext.IsLoggedIn)
        {
            _cartItems = [];
            grpCart.Text = "Giỏ hàng của bạn (đăng nhập để lưu món)";
            dgvCart.DataSource = null;
            lblCartTotal.Text = "Tổng giỏ hàng: 0 đ";
            return;
        }

        var userCode = SessionContext.UserCode;
        _cartItems = await _cartService.GetCartAsync(userCode);
        if (IsDisposed) return;

        var cartBranchCode = await _cartService.GetCartBranchCodeAsync(userCode);
        grpCart.Text = string.IsNullOrEmpty(cartBranchCode)
            ? "Giỏ hàng của bạn"
            : $"Giỏ hàng của bạn — {GetBranchDisplayName(cartBranchCode)}";

        dgvCart.DataSource = _cartItems.Select(c => new
        {
            TenMon = c.Name,
            Size = c.SizeCode,
            SL = c.Qty,
            Topping = c.Toppings.Count > 0 ? string.Join(", ", c.Toppings) : "-",
            ThanhTien = $"{c.Subtotal:N0} đ"
        }).ToList();

        if (dgvCart.Columns["TenMon"] is { } c1) { c1.HeaderText = "Tên món"; c1.FillWeight = 155; }
        if (dgvCart.Columns["Size"] is { } c2) { c2.HeaderText = "Cỡ"; c2.FillWeight = 36; }
        if (dgvCart.Columns["SL"] is { } c3) { c3.HeaderText = "SL"; c3.FillWeight = 34; }
        if (dgvCart.Columns["Topping"] is { } c4) { c4.HeaderText = "Topping"; c4.FillWeight = 95; }
        if (dgvCart.Columns["ThanhTien"] is { } c5) { c5.HeaderText = "Thành tiền"; c5.FillWeight = 95; }

        double total = _cartItems.Sum(i => i.Subtotal);
        lblCartTotal.Text = $"Tổng giỏ hàng ({_cartItems.Sum(i => i.Qty)} phần): {total:N0} đ";
    }

    private async Task<bool> EnsureLoggedInAsync(string actionText)
    {
        if (SessionContext.IsLoggedIn) return true;

        var prompt = MessageBox.Show(
            $"Vui lòng đăng nhập tài khoản để {actionText}.\nMở màn hình đăng nhập ngay?",
            "Yêu cầu đăng nhập",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);
        if (prompt != DialogResult.Yes) return false;

        using var loginForm = new FrmLogin();
        loginForm.ShowDialog(this);
        UpdateHeaderBanner();
        await LoadCartAsync();
        return SessionContext.IsLoggedIn;
    }

    private async void btnAddToCart_Click(object? sender, EventArgs e)
    {
        if (_selectedDish == null) return;
        try
        {
            if (!await EnsureLoggedInAsync("thêm món vào giỏ hàng")) return;

            var sizeCode = (cboSize.SelectedItem as SizeChoice)?.Option.SizeCode ?? "S";
            var toppings = GetSelectedToppings();
            int qty = (int)nudQty.Value;

            var res = await _cartService.AddOrUpdateCartItemAsync(
                SessionContext.UserCode,
                _selectedDish,
                sizeCode,
                toppings,
                qty);

            if (!res.Success && res.Message.StartsWith("BRANCH_CONFLICT|", StringComparison.Ordinal))
            {
                var existingCode = await _cartService.GetCartBranchCodeAsync(SessionContext.UserCode);
                var existingName = GetBranchDisplayName(existingCode);
                var targetName = GetBranchDisplayName(_selectedDish.RestaurantCode);

                var confirmClear = MessageBox.Show(
                    $"Giỏ hàng hiện đang chứa món của chi nhánh {existingName}.\n" +
                    $"Bạn có muốn xóa giỏ cũ để đặt món tại chi nhánh {targetName} không?",
                    "Đổi chi nhánh trong giỏ hàng",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmClear == DialogResult.Yes)
                {
                    res = await _cartService.AddOrUpdateCartItemAsync(
                        SessionContext.UserCode,
                        _selectedDish,
                        sizeCode,
                        toppings,
                        qty,
                        clearDifferentBranch: true);
                }
                else
                {
                    return;
                }
            }

            if (res.Success)
                await LoadCartAsync();
            else
                MessageBox.Show(res.Message, "Không thể thêm món", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi thêm vào giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private CartItemDto? GetSelectedCartItem()
    {
        int idx = dgvCart.CurrentRow?.Index ?? -1;
        return idx >= 0 && idx < _cartItems.Count ? _cartItems[idx] : null;
    }

    private async void btnQtyPlus_Click(object? sender, EventArgs e)
    {
        try
        {
            await AdjustSelectedCartQtyAsync(+1);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi cập nhật giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnQtyMinus_Click(object? sender, EventArgs e)
    {
        try
        {
            await AdjustSelectedCartQtyAsync(-1);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi cập nhật giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AdjustSelectedCartQtyAsync(int delta)
    {
        if (!SessionContext.IsLoggedIn) return;
        var item = GetSelectedCartItem();
        if (item == null) return;

        await _cartService.UpdateCartItemQtyAsync(SessionContext.UserCode, item.HashField, item.Qty + delta);
        await LoadCartAsync();
    }

    private async void btnRemoveItem_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!SessionContext.IsLoggedIn) return;
            var item = GetSelectedCartItem();
            if (item == null) return;

            await _cartService.RemoveCartItemAsync(SessionContext.UserCode, item.HashField);
            await LoadCartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa món khỏi giỏ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnClearCart_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!SessionContext.IsLoggedIn || _cartItems.Count == 0) return;

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa toàn bộ món trong giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            await _cartService.ClearCartAsync(SessionContext.UserCode);
            await LoadCartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi xóa giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnBookTable_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!await EnsureLoggedInAsync("đặt bàn tại quán")) return;

            Hide();
            try
            {
                using var bookingForm = new FrmBooking(CurrentRestaurantCode);
                bookingForm.ShowDialog(this);
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
            await LoadTopHotAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi mở màn hình đặt bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnTakeawayCheckout_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!await EnsureLoggedInAsync("đặt mang về / giao hàng")) return;

            await LoadCartAsync();
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Giỏ hàng của bạn đang trống. Vui lòng chọn ít nhất 1 món ăn trước khi thanh toán.", "Giỏ hàng trống", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Đảm bảo giỏ hàng khớp với chi nhánh đang xem
            var cartBranch = await _cartService.GetCartBranchCodeAsync(SessionContext.UserCode);
            var checkoutRestaurant = _restaurant;
            if (!string.IsNullOrEmpty(cartBranch) &&
                !string.Equals(cartBranch, CurrentRestaurantCode, StringComparison.OrdinalIgnoreCase))
            {
                var allBranches = await _menuService.GetRestaurantsAsync(activeOnly: true);
                checkoutRestaurant = allBranches.FirstOrDefault(r =>
                    string.Equals(r.RestaurantCode, cartBranch, StringComparison.OrdinalIgnoreCase)) ?? _restaurant;
            }

            Hide();
            try
            {
                using var checkoutForm = new FrmTakeawayCheckout(checkoutRestaurant);
                checkoutForm.ShowDialog(this);
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
            await LoadTopHotAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi mở trang thanh toán: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnChangeBranch_Click(object? sender, EventArgs e)
    {
        ReturnToMainRequested = false;
        Close();
    }

    private void btnBackMain_Click(object? sender, EventArgs e)
    {
        ReturnToMainRequested = true;
        Close();
    }
}
