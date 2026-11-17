namespace RestaurantBooking.Forms;

partial class FrmMain
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private Label lblSubtitle = null!;
    private GroupBox grpStatus = null!;
    private Label lblCurrentUser = null!;
    private Label lblDbStatus = null!;
    private GroupBox grpDevMode = null!;
    private Button btnDevCustomer = null!;
    private Button btnDevStaff = null!;
    private Button btnDevAdmin = null!;
    private GroupBox grpModules = null!;
    private Button btnOpenHome = null!;
    private Button btnOpenBooking = null!;
    private Button btnOpenHistory = null!;
    private Button btnOpenAdmin = null!;
    private Button btnOpenUserAdmin = null!;
    private Button btnOpenLogin = null!;
    private Button btnRefreshStatus = null!;
    private Button btnReseedData = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblSubtitle = new Label();
        lblTitle = new Label();
        grpStatus = new GroupBox();
        lblDbStatus = new Label();
        lblCurrentUser = new Label();
        grpDevMode = new GroupBox();
        btnDevAdmin = new Button();
        btnDevStaff = new Button();
        btnDevCustomer = new Button();
        grpModules = new GroupBox();
        btnOpenLogin = new Button();
        btnOpenUserAdmin = new Button();
        btnOpenAdmin = new Button();
        btnOpenHistory = new Button();
        btnOpenBooking = new Button();
        btnOpenHome = new Button();
        btnRefreshStatus = new Button();
        btnReseedData = new Button();

        pnlHeader.SuspendLayout();
        grpStatus.SuspendLayout();
        grpDevMode.SuspendLayout();
        grpModules.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 58, 95);
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(860, 82);

        // lblTitle
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(20, 14);
        lblTitle.Name = "lblTitle";
        lblTitle.Text = "HỆ THỐNG ĐẶT BÀN NHÀ HÀNG & ĐẶT MÓN TRỰC TUYẾN";

        // lblSubtitle
        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 9.5F);
        lblSubtitle.ForeColor = Color.FromArgb(200, 220, 245);
        lblSubtitle.Location = new Point(22, 48);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Text = "Đồ án CSDL NoSQL (HUIT)  •  MongoDB (db_booking_nhahang) + Memurai (127.0.0.1:6379)  •  C# WinForms .NET 9";

        // grpStatus
        grpStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpStatus.Location = new Point(20, 96);
        grpStatus.Name = "grpStatus";
        grpStatus.Size = new Size(820, 96);
        grpStatus.TabStop = false;
        grpStatus.Text = "Trạng thái Phiên & Kết nối CSDL";
        grpStatus.Controls.Add(lblCurrentUser);
        grpStatus.Controls.Add(lblDbStatus);

        lblCurrentUser.AutoSize = true;
        lblCurrentUser.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblCurrentUser.ForeColor = Color.FromArgb(30, 58, 95);
        lblCurrentUser.Location = new Point(14, 24);
        lblCurrentUser.Name = "lblCurrentUser";
        lblCurrentUser.Text = "Đang khởi tạo phiên...";

        lblDbStatus.AutoSize = true;
        lblDbStatus.Font = new Font("Segoe UI", 9F);
        lblDbStatus.Location = new Point(14, 48);
        lblDbStatus.Name = "lblDbStatus";
        lblDbStatus.Text = "Đang kiểm tra kết nối MongoDB & Memurai...";

        // grpDevMode
        grpDevMode.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpDevMode.Location = new Point(20, 202);
        grpDevMode.Name = "grpDevMode";
        grpDevMode.Size = new Size(820, 74);
        grpDevMode.TabStop = false;
        grpDevMode.Text = "Chế độ Dev Fallback (Chuyển nhanh vai trò để test độc lập từng phân hệ)";
        grpDevMode.Controls.Add(btnDevCustomer);
        grpDevMode.Controls.Add(btnDevStaff);
        grpDevMode.Controls.Add(btnDevAdmin);

        btnDevCustomer.Font = new Font("Segoe UI", 9.5F);
        btnDevCustomer.Location = new Point(16, 26);
        btnDevCustomer.Name = "btnDevCustomer";
        btnDevCustomer.Size = new Size(240, 36);
        btnDevCustomer.Text = "Khách hàng (USR001)";
        btnDevCustomer.Click += btnDevCustomer_Click;

        btnDevStaff.Font = new Font("Segoe UI", 9.5F);
        btnDevStaff.Location = new Point(288, 26);
        btnDevStaff.Name = "btnDevStaff";
        btnDevStaff.Size = new Size(240, 36);
        btnDevStaff.Text = "Nhân viên (STF001)";
        btnDevStaff.Click += btnDevStaff_Click;

        btnDevAdmin.Font = new Font("Segoe UI", 9.5F);
        btnDevAdmin.Location = new Point(560, 26);
        btnDevAdmin.Name = "btnDevAdmin";
        btnDevAdmin.Size = new Size(240, 36);
        btnDevAdmin.Text = "Quản trị viên (ADM001)";
        btnDevAdmin.Click += btnDevAdmin_Click;

        // grpModules
        grpModules.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpModules.Location = new Point(20, 286);
        grpModules.Name = "grpModules";
        grpModules.Size = new Size(820, 170);
        grpModules.TabStop = false;
        grpModules.Text = "Phân hệ Chức năng (Tách Form độc quyền cho 3 Thành viên)";
        grpModules.Controls.Add(btnOpenHome);
        grpModules.Controls.Add(btnOpenBooking);
        grpModules.Controls.Add(btnOpenHistory);
        grpModules.Controls.Add(btnOpenAdmin);
        grpModules.Controls.Add(btnOpenUserAdmin);
        grpModules.Controls.Add(btnOpenLogin);

        btnOpenHome.BackColor = Color.FromArgb(240, 247, 255);
        btnOpenHome.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenHome.Location = new Point(16, 30);
        btnOpenHome.Name = "btnOpenHome";
        btnOpenHome.Size = new Size(252, 54);
        btnOpenHome.Text = "[TV3] Trang chủ, Thực đơn && Giỏ hàng";
        btnOpenHome.Click += btnOpenHome_Click;

        btnOpenBooking.BackColor = Color.FromArgb(236, 253, 245);
        btnOpenBooking.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenBooking.Location = new Point(284, 30);
        btnOpenBooking.Name = "btnOpenBooking";
        btnOpenBooking.Size = new Size(252, 54);
        btnOpenBooking.Text = "[TV1] Đặt bàn TTL, Waitlist && Duyệt đơn";
        btnOpenBooking.Click += btnOpenBooking_Click;

        btnOpenHistory.BackColor = Color.FromArgb(240, 247, 255);
        btnOpenHistory.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenHistory.Location = new Point(552, 30);
        btnOpenHistory.Name = "btnOpenHistory";
        btnOpenHistory.Size = new Size(252, 54);
        btnOpenHistory.Text = "[TV3] Lịch sử đơn && Đánh giá";
        btnOpenHistory.Click += btnOpenHistory_Click;

        btnOpenAdmin.BackColor = Color.FromArgb(255, 247, 237);
        btnOpenAdmin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenAdmin.Location = new Point(16, 98);
        btnOpenAdmin.Name = "btnOpenAdmin";
        btnOpenAdmin.Size = new Size(252, 54);
        btnOpenAdmin.Text = "[TV2] Quản trị Menu, Thống kê && Backup";
        btnOpenAdmin.Click += btnOpenAdmin_Click;

        btnOpenUserAdmin.BackColor = Color.FromArgb(240, 247, 255);
        btnOpenUserAdmin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenUserAdmin.Location = new Point(284, 98);
        btnOpenUserAdmin.Name = "btnOpenUserAdmin";
        btnOpenUserAdmin.Size = new Size(252, 54);
        btnOpenUserAdmin.Text = "[TV3] Quản trị Người dùng (Admin)";
        btnOpenUserAdmin.Click += btnOpenUserAdmin_Click;

        btnOpenLogin.BackColor = Color.FromArgb(245, 245, 245);
        btnOpenLogin.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnOpenLogin.Location = new Point(552, 98);
        btnOpenLogin.Name = "btnOpenLogin";
        btnOpenLogin.Size = new Size(252, 54);
        btnOpenLogin.Text = "[TV3] Đăng nhập / Đăng ký";
        btnOpenLogin.Click += btnOpenLogin_Click;

        // Bottom utilities
        btnRefreshStatus.Font = new Font("Segoe UI", 9.5F);
        btnRefreshStatus.Location = new Point(20, 470);
        btnRefreshStatus.Name = "btnRefreshStatus";
        btnRefreshStatus.Size = new Size(210, 38);
        btnRefreshStatus.Text = "Kiểm tra lại kết nối && số liệu";
        btnRefreshStatus.Click += btnRefreshStatus_Click;

        btnReseedData.Font = new Font("Segoe UI", 9.5F);
        btnReseedData.Location = new Point(600, 470);
        btnReseedData.Name = "btnReseedData";
        btnReseedData.Size = new Size(240, 38);
        btnReseedData.Text = "[TV1] Nạp lại dữ liệu mẫu (500 đơn)";
        btnReseedData.Click += btnReseedData_Click;

        // FrmMain
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(860, 524);
        Controls.Add(btnReseedData);
        Controls.Add(btnRefreshStatus);
        Controls.Add(grpModules);
        Controls.Add(grpDevMode);
        Controls.Add(grpStatus);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "FrmMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hương Việt Booking — NoSQL (MongoDB + Memurai)";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        grpStatus.ResumeLayout(false);
        grpStatus.PerformLayout();
        grpDevMode.ResumeLayout(false);
        grpModules.ResumeLayout(false);
        ResumeLayout(false);
    }
}
