using RestaurantBooking.Config;
using RestaurantBooking.Forms.TV1_Booking;
using RestaurantBooking.Forms.TV2_Admin;
using RestaurantBooking.Forms.TV3_User;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms;

/// <summary>
/// Màn hình chính của ứng dụng Nhà hàng Hương Việt — điều hướng theo vai trò,
/// tự động ẩn trang chủ khi mở màn hình chức năng, hiện lại gọn gàng khi quay về
/// và tự căn giữa nổi bật cả ở cửa sổ thường lẫn chế độ toàn màn hình.
/// </summary>
public partial class FrmMain : Form
{
    private readonly AuthService _authService = new();

    private bool _baseCaptured;
    private int _baseBodyWidth;
    private int _baseCardHeight;
    private int _baseMinWidth;
    private int _baseMinHeight;
    private Font _normalCardFont = null!;
    private Font _largeCardFont = null!;

    public FrmMain()
    {
        InitializeComponent();
        ApplyCleanTheme();
        Load += FrmMain_Load;
        Resize += (_, _) =>
        {
            if (_baseCaptured && Visible)
                LayoutDashboard(adjustNormalWindowHeight: false);
        };
        FormClosed += (_, _) =>
        {
            _normalCardFont?.Dispose();
            _largeCardFont?.Dispose();
        };
    }

    private void FrmMain_Load(object? sender, EventArgs e)
    {
        CaptureBaseMetricsOnce();
        UpdateRoleBasedUi();
    }

    private void CaptureBaseMetricsOnce()
    {
        if (_baseCaptured) return;
        _baseCaptured = true;
        _baseBodyWidth = Math.Max(ClientSize.Width, 640);
        _baseCardHeight = Math.Max(btnOpenHome.Height, 84);
        _baseMinWidth = MinimumSize.Width;
        _baseMinHeight = MinimumSize.Height;
        _normalCardFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        _largeCardFont = new Font("Segoe UI", 11F, FontStyle.Bold);
    }

    private int DpiX(int px96) => (int)Math.Round(px96 * (DeviceDpi / 96f));
    private int DpiY(int px96) => (int)Math.Round(px96 * (DeviceDpi / 96f));

    private void ApplyCleanTheme()
    {
        UiTheme.StyleCardButton(btnOpenHome);
        UiTheme.StyleCardButton(btnOpenBooking);
        UiTheme.StyleCardButton(btnOpenHistory);
        UiTheme.StyleCardButton(btnOpenAdmin);
        UiTheme.StyleCardButton(btnOpenUserAdmin);
    }

    private bool EnsureLoggedInIfRequired(bool requireLogin)
    {
        if (!requireLogin || SessionContext.IsLoggedIn) return true;

        var ask = MessageBox.Show(
            "Vui lòng đăng nhập để sử dụng chức năng này. Mở màn hình Đăng nhập ngay?",
            "Yêu cầu đăng nhập",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

        if (ask != DialogResult.Yes) return false;

        using var loginForm = new FrmLogin();
        loginForm.ShowDialog(this);
        UpdateRoleBasedUi();
        return SessionContext.IsLoggedIn;
    }

    private void NavigateToChildForm<TForm>(bool requireLogin = false, bool requireAdmin = false)
        where TForm : Form, new()
    {
        if (!EnsureLoggedInIfRequired(requireLogin || requireAdmin)) return;

        if (requireAdmin && !SessionContext.IsAdmin)
        {
            MessageBox.Show(
                "Chức năng này chỉ dành cho Quản trị viên.",
                "Không đủ quyền truy cập",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        Hide();
        try
        {
            using var form = new TForm();
            form.ShowDialog(this);
        }
        finally
        {
            if (!IsDisposed)
            {
                // Hiện Form trước rồi mới cập nhật UI để tránh lỗi Control.Visible trả false khi cha đang ẩn
                Show();
                UpdateRoleBasedUi();
                Activate();
            }
        }
    }

    private void UpdateRoleBasedUi()
    {
        CaptureBaseMetricsOnce();

        if (!SessionContext.IsLoggedIn)
        {
            lblCurrentUser.Text = "Chào mừng quý khách đến với Nhà hàng Hương Việt";
            lblQuickHint.Text = "Xem thực đơn ngay hoặc đăng nhập để đặt bàn và giao hàng tận nơi.";

            btnAuthAction.Text = "Đăng nhập / Đăng ký";
            btnAuthAction.BackColor = Color.White;
            btnAuthAction.ForeColor = UiTheme.TextPrimary;
            btnAuthAction.FlatAppearance.BorderSize = 0;

            btnOpenBooking.Text = "Đặt bàn trực tuyến\nChọn sơ đồ bàn && giữ chỗ 15 phút";
            btnOpenHistory.Text = "Đơn hàng && Đánh giá\nTheo dõi đơn, hủy bàn && đánh giá";
        }
        else
        {
            btnAuthAction.Text = "Đăng xuất";
            UiTheme.StyleDangerButton(btnAuthAction);

            if (SessionContext.IsAdmin)
            {
                lblCurrentUser.Text = $"Xin chào, {SessionContext.FullName} — Quản trị viên";
                lblQuickHint.Text = "Quản lý chi nhánh, thực đơn, đơn đặt bàn, thống kê doanh thu, sao lưu và người dùng.";
                btnOpenBooking.Text = "Sơ đồ bàn && Duyệt đơn\nXử lý giữ chỗ, đơn && hàng đợi";
                btnOpenHistory.Text = "Đơn hàng && Phản hồi\nXem đơn && trả lời đánh giá khách";
            }
            else if (SessionContext.IsStaffOrAdmin)
            {
                lblCurrentUser.Text = $"Xin chào, {SessionContext.FullName} — Nhân viên";
                lblQuickHint.Text = "Theo dõi sơ đồ bàn, xác nhận/hủy đơn đặt và phản hồi đánh giá của khách hàng.";
                btnOpenBooking.Text = "Sơ đồ bàn && Duyệt đơn\nXác nhận, phục vụ, hoàn tất hoặc hủy đơn";
                btnOpenHistory.Text = "Đơn hàng && Phản hồi\nXem && phản hồi đánh giá khách hàng";
            }
            else
            {
                lblCurrentUser.Text = $"Xin chào, {SessionContext.FullName}";
                lblQuickHint.Text = "Chọn chi nhánh để đặt món giao hàng, đặt bàn giữ chỗ 15 phút hoặc xem lại đơn hàng.";
                btnOpenBooking.Text = "Đặt bàn trực tuyến\nChọn sơ đồ bàn && giữ chỗ 15 phút";
                btnOpenHistory.Text = "Đơn hàng && Đánh giá\nXem lịch sử, hủy đơn && đánh giá";
            }
        }

        LayoutDashboard(adjustNormalWindowHeight: WindowState == FormWindowState.Normal);
    }

    private List<Button> ResolveRoleModuleButtons()
    {
        bool showAdminCards = SessionContext.IsLoggedIn && SessionContext.IsAdmin;

        btnOpenHome.Visible = true;
        btnOpenBooking.Visible = true;
        btnOpenHistory.Visible = true;
        btnOpenAdmin.Visible = showAdminCards;
        btnOpenUserAdmin.Visible = showAdminCards;

        var list = new List<Button> { btnOpenHome, btnOpenBooking, btnOpenHistory };
        if (showAdminCards)
        {
            list.Add(btnOpenAdmin);
            list.Add(btnOpenUserAdmin);
        }

        return list;
    }

    private void LayoutDashboard(bool adjustNormalWindowHeight)
    {
        CaptureBaseMetricsOnce();

        var visibleButtons = ResolveRoleModuleButtons();
        int rows = visibleButtons.Count > 3 ? 2 : 1;

        int padX = DpiX(22);
        int padY = DpiY(18);
        int innerPadX = DpiX(24);
        int innerPadBottom = DpiY(22);
        int gapX = DpiX(16);
        int gapY = DpiY(14);
        int areaTopPad = lblSectionTitle.Top + lblSectionTitle.Height + DpiY(12);

        int bodyW = Math.Max(pnlBody.ClientSize.Width, _baseBodyWidth);
        int baseDashW = _baseBodyWidth - padX * 2;
        bool isExpandedWidth = bodyW > _baseBodyWidth + DpiX(120);

        int dashW = isExpandedWidth
            ? Math.Clamp((int)(bodyW * 0.84), baseDashW, Math.Min(bodyW - DpiX(72), DpiX(1380)))
            : bodyW - padX * 2;

        // Tính chiều cao thẻ cân đối: vừa gọn ở cửa sổ thường, mở rộng tối ưu diện tích khi toàn màn hình
        int availableBodyH = Math.Max(pnlBody.ClientSize.Height, DpiY(260));
        int cardH = _baseCardHeight;
        if (WindowState == FormWindowState.Maximized || availableBodyH > DpiY(480))
        {
            int maxCardH = rows == 1 ? DpiY(205) : DpiY(155);
            int targetDashH = (int)(availableBodyH * (rows == 1 ? 0.68 : 0.80));
            int availForCards = targetDashH - pnlWelcome.Height - pnlFooterBar.Height - areaTopPad - innerPadBottom - (rows - 1) * gapY;
            int computedCardH = availForCards / rows;
            cardH = Math.Clamp(computedCardH, _baseCardHeight, maxCardH);
        }

        int modulesAreaH = areaTopPad + rows * cardH + (rows - 1) * gapY + innerPadBottom;
        int dashH = pnlWelcome.Height + modulesAreaH + pnlFooterBar.Height + 2;

        if (adjustNormalWindowHeight && WindowState == FormWindowState.Normal)
        {
            int maxAllowedClientH = Screen.FromControl(this).WorkingArea.Height - DpiY(48);
            int targetClientH = Math.Min(maxAllowedClientH, pnlHeader.Height + dashH + padY * 2);
            MinimumSize = new Size(_baseMinWidth, Math.Min(_baseMinHeight, targetClientH));
            if (ClientSize.Height != targetClientH)
            {
                ClientSize = new Size(ClientSize.Width, targetClientH);
                bodyW = Math.Max(pnlBody.ClientSize.Width, _baseBodyWidth);
                availableBodyH = pnlBody.ClientSize.Height;
            }
        }

        int dashLeft = Math.Max(padX, (bodyW - dashW) / 2);
        int dashTop = isExpandedWidth || availableBodyH > dashH + padY * 2 + DpiY(16)
            ? Math.Max(padY, (availableBodyH - dashH) / 2)
            : padY;

        pnlDashboard.SuspendLayout();
        pnlModulesArea.SuspendLayout();

        pnlHeaderInner.Bounds = new Rectangle(dashLeft, 0, dashW, pnlHeader.Height);
        btnAuthAction.Location = new Point(Math.Max(0, pnlHeaderInner.Width - btnAuthAction.Width), btnAuthAction.Top);

        pnlDashboard.Bounds = new Rectangle(dashLeft, dashTop, dashW, dashH);

        // Tính trực tiếp từ dashW (trừ 2px viền FixedSingle) vì Dock=Fill chưa resize khi đang SuspendLayout
        int cardsUsableW = Math.Max(DpiX(640), (dashW - 2) - innerPadX * 2);
        int cardW = Math.Max(DpiX(210), (cardsUsableW - gapX * 2) / 3);
        var activeCardFont = cardH >= _baseCardHeight + DpiY(24) ? _largeCardFont : _normalCardFont;

        // Hàng 1: 3 thẻ căn đều
        for (int i = 0; i < Math.Min(3, visibleButtons.Count); i++)
        {
            var btn = visibleButtons[i];
            btn.Font = activeCardFont;
            btn.Bounds = new Rectangle(innerPadX + i * (cardW + gapX), areaTopPad, cardW, cardH);
        }

        // Hàng 2 (Admin): 2 thẻ căn giữa cân đối bên dưới hàng 1
        if (visibleButtons.Count > 3)
        {
            int row2Count = visibleButtons.Count - 3;
            int row2TotalW = row2Count * cardW + (row2Count - 1) * gapX;
            int row2StartX = innerPadX + Math.Max(0, (cardsUsableW - row2TotalW) / 2);
            int row2Y = areaTopPad + cardH + gapY;

            for (int j = 0; j < row2Count; j++)
            {
                var btn = visibleButtons[3 + j];
                btn.Font = activeCardFont;
                btn.Bounds = new Rectangle(row2StartX + j * (cardW + gapX), row2Y, cardW, cardH);
            }
        }

        pnlModulesArea.ResumeLayout(true);
        pnlDashboard.ResumeLayout(true);
    }

    private async void btnAuthAction_Click(object? sender, EventArgs e)
    {
        if (!SessionContext.IsLoggedIn)
        {
            using var loginForm = new FrmLogin();
            loginForm.ShowDialog(this);
            UpdateRoleBasedUi();
            return;
        }

        var confirm = MessageBox.Show(
            "Bạn có muốn đăng xuất khỏi tài khoản hiện tại không?",
            "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        btnAuthAction.Enabled = false;
        try
        {
            await _authService.LogoutAsync();
            UpdateRoleBasedUi();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnAuthAction.Enabled = true;
        }
    }

    private void btnOpenHome_Click(object? sender, EventArgs e) => NavigateToChildForm<FrmBranchSelect>();
    private void btnOpenBooking_Click(object? sender, EventArgs e) => NavigateToChildForm<FrmBooking>(requireLogin: true);
    private void btnOpenHistory_Click(object? sender, EventArgs e) => NavigateToChildForm<FrmHistory>(requireLogin: true);
    private void btnOpenAdmin_Click(object? sender, EventArgs e) => NavigateToChildForm<FrmAdmin>(requireAdmin: true);
    private void btnOpenUserAdmin_Click(object? sender, EventArgs e) => NavigateToChildForm<FrmUserAdmin>(requireAdmin: true);
}
