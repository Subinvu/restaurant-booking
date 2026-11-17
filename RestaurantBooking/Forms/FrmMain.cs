using MongoDB.Driver;
using RestaurantBooking.Config;
using RestaurantBooking.Forms.TV1_Booking;
using RestaurantBooking.Forms.TV2_Admin;
using RestaurantBooking.Forms.TV3_User;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms;

/// <summary>
/// [Cố định Ngày 1 - TV1] Form điều hướng trung tâm của ứng dụng WinForms.
/// Liên kết tới cả 6 Form phân hệ của 3 thành viên và hỗ trợ chuyển vai trò Dev Mode.
/// </summary>
public partial class FrmMain : Form
{
    public FrmMain()
    {
        InitializeComponent();
        Load += async (_, _) => await RefreshDashboardStatusAsync();
    }

    private void OpenChildForm<TForm>(bool requireStaff = false, bool requireAdmin = false)
        where TForm : Form, new()
    {
        if (requireAdmin && !SessionContext.IsAdmin)
        {
            MessageBox.Show(
                "Chức năng này yêu cầu quyền Admin. Bạn có thể chuyển nhanh sang vai trò Admin (ADM001) ở khung Dev Mode hoặc đăng nhập tài khoản Admin.",
                "Phân quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (requireStaff && !SessionContext.IsStaffOrAdmin && typeof(TForm) != typeof(FrmBooking))
        {
            MessageBox.Show("Chức năng này yêu cầu quyền Nhân viên hoặc Admin.", "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var form = new TForm();
        form.ShowDialog(this);
        UpdateUserBanner();
    }

    private async Task RefreshDashboardStatusAsync()
    {
        UpdateUserBanner();

        if (!DbContext.Instance.TestConnections(out var connMessage))
        {
            lblDbStatus.ForeColor = Color.Firebrick;
            lblDbStatus.Text = connMessage;
            return;
        }

        try
        {
            var db = DbContext.Instance;
            var resCount = await db.Restaurants.CountDocumentsAsync(FilterDefinition<Models.RestaurantModel>.Empty);
            var menuCount = await db.MenuItems.CountDocumentsAsync(FilterDefinition<Models.MenuItemModel>.Empty);
            var userCount = await db.Users.CountDocumentsAsync(FilterDefinition<Models.UserModel>.Empty);
            var orderCount = await db.BookingsOrders.CountDocumentsAsync(FilterDefinition<Models.BookingOrderModel>.Empty);
            var reviewCount = await db.Reviews.CountDocumentsAsync(FilterDefinition<Models.ReviewModel>.Empty);
            var cartLen = await db.RedisDb.HashLengthAsync(RedisKeys.Cart(SessionContext.UserCode));

            lblDbStatus.ForeColor = Color.DarkGreen;
            lblDbStatus.Text =
                $"{connMessage}\n" +
                $"Dữ liệu MongoDB: {resCount} chi nhánh | {menuCount} món | {userCount} users | {orderCount} đơn hàng | {reviewCount} đánh giá  •  " +
                $"Memurai giỏ {SessionContext.UserCode}: {cartLen} món";
        }
        catch (Exception ex)
        {
            lblDbStatus.ForeColor = Color.DarkOrange;
            lblDbStatus.Text = $"Đã kết nối nhưng chưa đọc được thống kê: {ex.Message}";
        }
    }

    private void UpdateUserBanner()
    {
        var modeTag = SessionContext.IsDevFallback ? "DEV FALLBACK" : "ĐĂNG NHẬP THẬT";
        lblCurrentUser.Text = SessionContext.IsLoggedIn
            ? $"[{modeTag}] {SessionContext.FullName} ({SessionContext.UserCode}) — Vai trò: {SessionContext.Role.ToUpperInvariant()}"
            : "Chưa đăng nhập";

        btnOpenAdmin.Enabled = SessionContext.IsAdmin;
        btnOpenUserAdmin.Enabled = SessionContext.IsAdmin;
    }

    private void btnOpenHome_Click(object? sender, EventArgs e) => OpenChildForm<FrmHome>();
    private void btnOpenBooking_Click(object? sender, EventArgs e) => OpenChildForm<FrmBooking>();
    private void btnOpenHistory_Click(object? sender, EventArgs e) => OpenChildForm<FrmHistory>();
    private void btnOpenAdmin_Click(object? sender, EventArgs e) => OpenChildForm<FrmAdmin>(requireAdmin: true);
    private void btnOpenUserAdmin_Click(object? sender, EventArgs e) => OpenChildForm<FrmUserAdmin>(requireAdmin: true);
    private void btnOpenLogin_Click(object? sender, EventArgs e) => OpenChildForm<FrmLogin>();

    private async void btnDevCustomer_Click(object? sender, EventArgs e)
    {
        SessionContext.UseDevCustomer();
        await RefreshDashboardStatusAsync();
    }

    private async void btnDevStaff_Click(object? sender, EventArgs e)
    {
        SessionContext.UseDevStaff();
        await RefreshDashboardStatusAsync();
    }

    private async void btnDevAdmin_Click(object? sender, EventArgs e)
    {
        SessionContext.UseDevAdmin();
        await RefreshDashboardStatusAsync();
    }

    private async void btnRefreshStatus_Click(object? sender, EventArgs e) =>
        await RefreshDashboardStatusAsync();

    private async void btnReseedData_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Thao tác này sẽ xóa dữ liệu cũ trong 5 collections và nạp lại bộ dữ liệu mẫu chuẩn (500 đơn hàng + Memurai keys). Tiếp tục?",
            "Xác nhận nạp lại dữ liệu mẫu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            btnReseedData.Enabled = false;
            var result = await DataSeeder.SeedAsync(forceReset: true);
            MessageBox.Show(result, "Nạp dữ liệu thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RefreshDashboardStatusAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi nạp dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnReseedData.Enabled = true;
        }
    }
}
