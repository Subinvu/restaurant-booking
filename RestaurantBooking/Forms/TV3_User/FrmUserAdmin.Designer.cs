namespace RestaurantBooking.Forms.TV3_User;

partial class FrmUserAdmin
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblInfo = null!;
    private Button btnBack = null!;
    private Panel pnlFilter = null!;
    private Label lblSearch = null!;
    private TextBox txtSearch = null!;
    private Label lblRole = null!;
    private ComboBox cboRoleFilter = null!;
    private Label lblActive = null!;
    private ComboBox cboActiveFilter = null!;
    private Button btnSearch = null!;
    private Label lblStats = null!;
    private DataGridView dgvUsers = null!;
    private GroupBox grpEdit = null!;
    private Label lblSelectedSummary = null!;
    private Label lblEditRole = null!;
    private ComboBox cboEditRole = null!;
    private CheckBox chkEditActive = null!;
    private Button btnSaveUser = null!;
    private Label lblResetPass = null!;
    private TextBox txtResetPass = null!;
    private Button btnResetPass = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblInfo = new Label();
        btnBack = new Button();
        pnlFilter = new Panel();
        lblSearch = new Label();
        txtSearch = new TextBox();
        lblRole = new Label();
        cboRoleFilter = new ComboBox();
        lblActive = new Label();
        cboActiveFilter = new ComboBox();
        btnSearch = new Button();
        lblStats = new Label();
        dgvUsers = new DataGridView();
        grpEdit = new GroupBox();
        lblSelectedSummary = new Label();
        lblEditRole = new Label();
        cboEditRole = new ComboBox();
        chkEditActive = new CheckBox();
        btnSaveUser = new Button();
        lblResetPass = new Label();
        txtResetPass = new TextBox();
        btnResetPass = new Button();

        pnlHeader.SuspendLayout();
        pnlFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
        grpEdit.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1040, 52);

        lblInfo.AutoSize = true;
        lblInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblInfo.ForeColor = Color.White;
        lblInfo.Location = new Point(16, 14);
        lblInfo.Name = "lblInfo";
        lblInfo.Text = "Quản trị tài khoản người dùng & Phân quyền";
        lblInfo.UseMnemonic = false;

        btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBack.BackColor = Color.FromArgb(51, 65, 85);
        btnBack.FlatStyle = FlatStyle.Flat;
        btnBack.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnBack.ForeColor = Color.White;
        btnBack.Location = new Point(904, 9);
        btnBack.Name = "btnBack";
        btnBack.Size = new Size(120, 34);
        btnBack.Text = "← Quay lại";
        btnBack.Click += btnBack_Click;

        pnlHeader.Controls.Add(lblInfo);
        pnlHeader.Controls.Add(btnBack);

        pnlFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlFilter.BackColor = Color.FromArgb(248, 250, 252);
        pnlFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlFilter.Location = new Point(12, 60);
        pnlFilter.Name = "pnlFilter";
        pnlFilter.Size = new Size(1016, 52);

        lblSearch.AutoSize = true;
        lblSearch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSearch.Location = new Point(10, 16);
        lblSearch.Name = "lblSearch";
        lblSearch.Text = "Tìm kiếm:";

        txtSearch.Location = new Point(78, 12);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Mã, tên, email hoặc SĐT...";
        txtSearch.Size = new Size(230, 25);

        lblRole.AutoSize = true;
        lblRole.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblRole.Location = new Point(324, 16);
        lblRole.Name = "lblRole";
        lblRole.Text = "Vai trò:";

        cboRoleFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRoleFilter.Location = new Point(380, 12);
        cboRoleFilter.Name = "cboRoleFilter";
        cboRoleFilter.Size = new Size(160, 25);

        lblActive.AutoSize = true;
        lblActive.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblActive.Location = new Point(554, 16);
        lblActive.Name = "lblActive";
        lblActive.Text = "Trạng thái:";

        cboActiveFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cboActiveFilter.Location = new Point(628, 12);
        cboActiveFilter.Name = "cboActiveFilter";
        cboActiveFilter.Size = new Size(150, 25);

        btnSearch.BackColor = Color.White;
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSearch.Location = new Point(794, 9);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(124, 32);
        btnSearch.Text = "Lọc tài khoản";
        btnSearch.Click += btnSearch_Click;

        pnlFilter.Controls.Add(lblSearch);
        pnlFilter.Controls.Add(txtSearch);
        pnlFilter.Controls.Add(lblRole);
        pnlFilter.Controls.Add(cboRoleFilter);
        pnlFilter.Controls.Add(lblActive);
        pnlFilter.Controls.Add(cboActiveFilter);
        pnlFilter.Controls.Add(btnSearch);

        lblStats.AutoSize = true;
        lblStats.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStats.ForeColor = Color.FromArgb(51, 65, 85);
        lblStats.Location = new Point(14, 118);
        lblStats.Name = "lblStats";
        lblStats.Text = "Thống kê người dùng: ...";

        dgvUsers.AllowUserToAddRows = false;
        dgvUsers.AllowUserToDeleteRows = false;
        dgvUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvUsers.BackgroundColor = Color.White;
        dgvUsers.Location = new Point(12, 142);
        dgvUsers.MultiSelect = false;
        dgvUsers.Name = "dgvUsers";
        dgvUsers.ReadOnly = true;
        dgvUsers.RowHeadersVisible = false;
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvUsers.Size = new Size(1016, 324);
        dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;

        grpEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpEdit.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpEdit.Location = new Point(12, 474);
        grpEdit.Name = "grpEdit";
        grpEdit.Size = new Size(1016, 152);
        grpEdit.Text = "Cập nhật phân quyền, trạng thái && mật khẩu";

        lblSelectedSummary.AutoSize = true;
        lblSelectedSummary.Font = new Font("Segoe UI", 9F);
        lblSelectedSummary.ForeColor = Color.FromArgb(15, 23, 42);
        lblSelectedSummary.Location = new Point(14, 26);
        lblSelectedSummary.Name = "lblSelectedSummary";
        lblSelectedSummary.Text = "Chọn một tài khoản trong bảng để chỉnh sửa.";
        lblSelectedSummary.UseMnemonic = false;

        lblEditRole.AutoSize = true;
        lblEditRole.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEditRole.Location = new Point(14, 62);
        lblEditRole.Name = "lblEditRole";
        lblEditRole.Text = "Vai trò:";

        cboEditRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cboEditRole.Location = new Point(74, 58);
        cboEditRole.Name = "cboEditRole";
        cboEditRole.Size = new Size(180, 25);

        chkEditActive.AutoSize = true;
        chkEditActive.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        chkEditActive.Location = new Point(272, 61);
        chkEditActive.Name = "chkEditActive";
        chkEditActive.Text = "Đang hoạt động (bỏ tích để khóa tài khoản)";

        btnSaveUser.BackColor = Color.FromArgb(21, 128, 61);
        btnSaveUser.FlatStyle = FlatStyle.Flat;
        btnSaveUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSaveUser.ForeColor = Color.White;
        btnSaveUser.Location = new Point(576, 54);
        btnSaveUser.Name = "btnSaveUser";
        btnSaveUser.Size = new Size(216, 34);
        btnSaveUser.Text = "Lưu phân quyền && trạng thái";
        btnSaveUser.Click += btnSaveUser_Click;

        lblResetPass.AutoSize = true;
        lblResetPass.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblResetPass.Location = new Point(14, 106);
        lblResetPass.Name = "lblResetPass";
        lblResetPass.Text = "Đặt lại mật khẩu mới:";

        txtResetPass.Location = new Point(164, 102);
        txtResetPass.Name = "txtResetPass";
        txtResetPass.PlaceholderText = "Nhập mật khẩu mới (>= 6 ký tự)";
        txtResetPass.Size = new Size(260, 25);
        txtResetPass.UseSystemPasswordChar = true;

        btnResetPass.BackColor = Color.White;
        btnResetPass.FlatStyle = FlatStyle.Flat;
        btnResetPass.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnResetPass.Location = new Point(440, 99);
        btnResetPass.Name = "btnResetPass";
        btnResetPass.Size = new Size(190, 32);
        btnResetPass.Text = "Đặt lại mật khẩu";
        btnResetPass.Click += btnResetPass_Click;

        grpEdit.Controls.Add(lblSelectedSummary);
        grpEdit.Controls.Add(lblEditRole);
        grpEdit.Controls.Add(cboEditRole);
        grpEdit.Controls.Add(chkEditActive);
        grpEdit.Controls.Add(btnSaveUser);
        grpEdit.Controls.Add(lblResetPass);
        grpEdit.Controls.Add(txtResetPass);
        grpEdit.Controls.Add(btnResetPass);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        BackColor = Color.White;
        ClientSize = new Size(1040, 638);
        Controls.Add(grpEdit);
        Controls.Add(dgvUsers);
        Controls.Add(lblStats);
        Controls.Add(pnlFilter);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimumSize = new Size(920, 580);
        Name = "FrmUserAdmin";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quản trị tài khoản người dùng — Nhà hàng Hương Việt";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlFilter.ResumeLayout(false);
        pnlFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
        grpEdit.ResumeLayout(false);
        grpEdit.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
