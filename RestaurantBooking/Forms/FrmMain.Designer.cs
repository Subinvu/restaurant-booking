namespace RestaurantBooking.Forms;

partial class FrmMain
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Panel pnlHeaderInner = null!;
    private Label lblTitle = null!;
    private Label lblSubtitle = null!;
    private Button btnAuthAction = null!;
    private Panel pnlBody = null!;
    private Panel pnlDashboard = null!;
    private Panel pnlWelcome = null!;
    private Panel pnlWelcomeDivider = null!;
    private Label lblCurrentUser = null!;
    private Label lblQuickHint = null!;
    private Panel pnlModulesArea = null!;
    private Label lblSectionTitle = null!;
    private Button btnOpenHome = null!;
    private Button btnOpenBooking = null!;
    private Button btnOpenHistory = null!;
    private Button btnOpenAdmin = null!;
    private Button btnOpenUserAdmin = null!;
    private Panel pnlFooterBar = null!;
    private Panel pnlFooterDivider = null!;
    private Label lblSystemInfo = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        pnlHeaderInner = new Panel();
        lblSubtitle = new Label();
        lblTitle = new Label();
        btnAuthAction = new Button();
        pnlBody = new Panel();
        pnlDashboard = new Panel();
        pnlWelcome = new Panel();
        pnlWelcomeDivider = new Panel();
        lblCurrentUser = new Label();
        lblQuickHint = new Label();
        pnlModulesArea = new Panel();
        lblSectionTitle = new Label();
        btnOpenHome = new Button();
        btnOpenBooking = new Button();
        btnOpenHistory = new Button();
        btnOpenAdmin = new Button();
        btnOpenUserAdmin = new Button();
        pnlFooterBar = new Panel();
        pnlFooterDivider = new Panel();
        lblSystemInfo = new Label();

        pnlHeader.SuspendLayout();
        pnlHeaderInner.SuspendLayout();
        pnlBody.SuspendLayout();
        pnlDashboard.SuspendLayout();
        pnlWelcome.SuspendLayout();
        pnlModulesArea.SuspendLayout();
        pnlFooterBar.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(920, 82);

        // pnlHeaderInner
        pnlHeaderInner.BackColor = Color.Transparent;
        pnlHeaderInner.Location = new Point(22, 0);
        pnlHeaderInner.Name = "pnlHeaderInner";
        pnlHeaderInner.Size = new Size(876, 82);

        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(0, 14);
        lblTitle.Name = "lblTitle";
        lblTitle.Text = "NHÀ HÀNG HƯƠNG VIỆT";

        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 9.5F);
        lblSubtitle.ForeColor = Color.FromArgb(203, 213, 225);
        lblSubtitle.Location = new Point(2, 48);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Text = "Đặt bàn trực tuyến && Đặt món ăn — Hệ thống chi nhánh TP.HCM";

        btnAuthAction.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAuthAction.BackColor = Color.White;
        btnAuthAction.Cursor = Cursors.Hand;
        btnAuthAction.FlatStyle = FlatStyle.Flat;
        btnAuthAction.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnAuthAction.ForeColor = Color.FromArgb(15, 23, 42);
        btnAuthAction.Location = new Point(692, 22);
        btnAuthAction.Name = "btnAuthAction";
        btnAuthAction.Size = new Size(184, 38);
        btnAuthAction.Text = "Đăng nhập / Đăng ký";
        btnAuthAction.Click += btnAuthAction_Click;

        pnlHeaderInner.Controls.Add(lblTitle);
        pnlHeaderInner.Controls.Add(lblSubtitle);
        pnlHeaderInner.Controls.Add(btnAuthAction);
        pnlHeader.Controls.Add(pnlHeaderInner);

        // pnlBody
        pnlBody.AutoScroll = true;
        pnlBody.BackColor = Color.FromArgb(241, 245, 249);
        pnlBody.Dock = DockStyle.Fill;
        pnlBody.Location = new Point(0, 82);
        pnlBody.Name = "pnlBody";
        pnlBody.Size = new Size(920, 340);

        // pnlDashboard
        pnlDashboard.BackColor = Color.White;
        pnlDashboard.BorderStyle = BorderStyle.FixedSingle;
        pnlDashboard.Location = new Point(22, 18);
        pnlDashboard.Name = "pnlDashboard";
        pnlDashboard.Size = new Size(876, 304);

        // pnlWelcome
        pnlWelcome.BackColor = Color.FromArgb(248, 250, 252);
        pnlWelcome.Dock = DockStyle.Top;
        pnlWelcome.Location = new Point(0, 0);
        pnlWelcome.Name = "pnlWelcome";
        pnlWelcome.Size = new Size(874, 72);

        lblCurrentUser.AutoSize = true;
        lblCurrentUser.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblCurrentUser.ForeColor = Color.FromArgb(15, 23, 42);
        lblCurrentUser.Location = new Point(24, 14);
        lblCurrentUser.Name = "lblCurrentUser";
        lblCurrentUser.Text = "Chào mừng quý khách đến với Nhà hàng Hương Việt";
        lblCurrentUser.UseMnemonic = false;

        lblQuickHint.AutoSize = true;
        lblQuickHint.Font = new Font("Segoe UI", 9.5F);
        lblQuickHint.ForeColor = Color.FromArgb(71, 85, 105);
        lblQuickHint.Location = new Point(24, 41);
        lblQuickHint.Name = "lblQuickHint";
        lblQuickHint.Text = "Xem thực đơn ngay hoặc đăng nhập để đặt bàn và giao hàng tận nơi.";
        lblQuickHint.UseMnemonic = false;

        pnlWelcomeDivider.BackColor = Color.FromArgb(226, 232, 240);
        pnlWelcomeDivider.Dock = DockStyle.Bottom;
        pnlWelcomeDivider.Location = new Point(0, 71);
        pnlWelcomeDivider.Name = "pnlWelcomeDivider";
        pnlWelcomeDivider.Size = new Size(874, 1);

        pnlWelcome.Controls.Add(pnlWelcomeDivider);
        pnlWelcome.Controls.Add(lblCurrentUser);
        pnlWelcome.Controls.Add(lblQuickHint);

        // pnlFooterBar
        pnlFooterBar.BackColor = Color.FromArgb(248, 250, 252);
        pnlFooterBar.Dock = DockStyle.Bottom;
        pnlFooterBar.Location = new Point(0, 264);
        pnlFooterBar.Name = "pnlFooterBar";
        pnlFooterBar.Size = new Size(874, 38);

        pnlFooterDivider.BackColor = Color.FromArgb(226, 232, 240);
        pnlFooterDivider.Dock = DockStyle.Top;
        pnlFooterDivider.Location = new Point(0, 0);
        pnlFooterDivider.Name = "pnlFooterDivider";
        pnlFooterDivider.Size = new Size(874, 1);

        lblSystemInfo.AutoSize = true;
        lblSystemInfo.Font = new Font("Segoe UI", 8.5F);
        lblSystemInfo.ForeColor = Color.FromArgb(71, 85, 105);
        lblSystemInfo.Location = new Point(24, 11);
        lblSystemInfo.Name = "lblSystemInfo";
        lblSystemInfo.Text = "Hệ thống 3 chi nhánh TP.HCM: Tân Phú  •  Quận 1  •  Quận 3    |    Giờ phục vụ: 09:00 – 22:30";
        lblSystemInfo.UseMnemonic = false;

        pnlFooterBar.Controls.Add(lblSystemInfo);
        pnlFooterBar.Controls.Add(pnlFooterDivider);

        // pnlModulesArea
        pnlModulesArea.BackColor = Color.White;
        pnlModulesArea.Dock = DockStyle.Fill;
        pnlModulesArea.Location = new Point(0, 72);
        pnlModulesArea.Name = "pnlModulesArea";
        pnlModulesArea.Size = new Size(874, 192);

        lblSectionTitle.AutoSize = true;
        lblSectionTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSectionTitle.ForeColor = Color.FromArgb(51, 65, 85);
        lblSectionTitle.Location = new Point(24, 16);
        lblSectionTitle.Name = "lblSectionTitle";
        lblSectionTitle.Text = "Chức năng chính";

        btnOpenHome.BackColor = Color.White;
        btnOpenHome.Cursor = Cursors.Hand;
        btnOpenHome.FlatStyle = FlatStyle.Flat;
        btnOpenHome.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenHome.ForeColor = Color.FromArgb(15, 23, 42);
        btnOpenHome.Location = new Point(24, 46);
        btnOpenHome.Name = "btnOpenHome";
        btnOpenHome.Size = new Size(264, 92);
        btnOpenHome.Text = "Thực đơn && Đặt món\nXem món chi nhánh && giao hàng";
        btnOpenHome.Click += btnOpenHome_Click;

        btnOpenBooking.BackColor = Color.White;
        btnOpenBooking.Cursor = Cursors.Hand;
        btnOpenBooking.FlatStyle = FlatStyle.Flat;
        btnOpenBooking.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenBooking.ForeColor = Color.FromArgb(15, 23, 42);
        btnOpenBooking.Location = new Point(304, 46);
        btnOpenBooking.Name = "btnOpenBooking";
        btnOpenBooking.Size = new Size(264, 92);
        btnOpenBooking.Text = "Đặt bàn trực tuyến\nChọn sơ đồ bàn && giữ chỗ 15 phút";
        btnOpenBooking.Click += btnOpenBooking_Click;

        btnOpenHistory.BackColor = Color.White;
        btnOpenHistory.Cursor = Cursors.Hand;
        btnOpenHistory.FlatStyle = FlatStyle.Flat;
        btnOpenHistory.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenHistory.ForeColor = Color.FromArgb(15, 23, 42);
        btnOpenHistory.Location = new Point(584, 46);
        btnOpenHistory.Name = "btnOpenHistory";
        btnOpenHistory.Size = new Size(264, 92);
        btnOpenHistory.Text = "Đơn hàng && Đánh giá\nTheo dõi đơn, hủy bàn && đánh giá";
        btnOpenHistory.Click += btnOpenHistory_Click;

        btnOpenAdmin.BackColor = Color.White;
        btnOpenAdmin.Cursor = Cursors.Hand;
        btnOpenAdmin.FlatStyle = FlatStyle.Flat;
        btnOpenAdmin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenAdmin.ForeColor = Color.FromArgb(15, 23, 42);
        btnOpenAdmin.Location = new Point(164, 152);
        btnOpenAdmin.Name = "btnOpenAdmin";
        btnOpenAdmin.Size = new Size(264, 92);
        btnOpenAdmin.Text = "Quản trị Nhà hàng && Thống kê\nChi nhánh, Thực đơn && Sao lưu";
        btnOpenAdmin.Click += btnOpenAdmin_Click;

        btnOpenUserAdmin.BackColor = Color.White;
        btnOpenUserAdmin.Cursor = Cursors.Hand;
        btnOpenUserAdmin.FlatStyle = FlatStyle.Flat;
        btnOpenUserAdmin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenUserAdmin.ForeColor = Color.FromArgb(15, 23, 42);
        btnOpenUserAdmin.Location = new Point(444, 152);
        btnOpenUserAdmin.Name = "btnOpenUserAdmin";
        btnOpenUserAdmin.Size = new Size(264, 92);
        btnOpenUserAdmin.Text = "Quản trị Người dùng\nPhân quyền && quản lý tài khoản";
        btnOpenUserAdmin.Click += btnOpenUserAdmin_Click;

        pnlModulesArea.Controls.Add(lblSectionTitle);
        pnlModulesArea.Controls.Add(btnOpenHome);
        pnlModulesArea.Controls.Add(btnOpenBooking);
        pnlModulesArea.Controls.Add(btnOpenHistory);
        pnlModulesArea.Controls.Add(btnOpenAdmin);
        pnlModulesArea.Controls.Add(btnOpenUserAdmin);

        pnlDashboard.Controls.Add(pnlModulesArea);
        pnlDashboard.Controls.Add(pnlWelcome);
        pnlDashboard.Controls.Add(pnlFooterBar);

        pnlBody.Controls.Add(pnlDashboard);

        // FrmMain
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(960, 422);
        Controls.Add(pnlBody);
        Controls.Add(pnlHeader);
        MinimumSize = new Size(880, 420);
        Name = "FrmMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Nhà hàng Hương Việt — Đặt bàn & Đặt món trực tuyến";

        pnlHeaderInner.ResumeLayout(false);
        pnlHeaderInner.PerformLayout();
        pnlHeader.ResumeLayout(false);
        pnlWelcome.ResumeLayout(false);
        pnlWelcome.PerformLayout();
        pnlFooterBar.ResumeLayout(false);
        pnlFooterBar.PerformLayout();
        pnlModulesArea.ResumeLayout(false);
        pnlModulesArea.PerformLayout();
        pnlDashboard.ResumeLayout(false);
        pnlBody.ResumeLayout(false);
        ResumeLayout(false);
    }
}
