namespace RestaurantBooking.Forms.TV3_User;

partial class FrmLogin
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblHeaderTitle = null!;
    private Button btnBack = null!;
    private TabControl tabAuth = null!;
    private TabPage tabLogin = null!;
    private TabPage tabRegister = null!;

    // Login tab controls
    private Panel pnlLoginBox = null!;
    private Label lblAccount = null!;
    private TextBox txtLoginAccount = null!;
    private Label lblPassword = null!;
    private TextBox txtLoginPassword = null!;
    private CheckBox chkShowPassword = null!;
    private Button btnLogin = null!;

    // Register tab controls
    private Label lblRegName = null!;
    private TextBox txtRegName = null!;
    private Label lblRegEmail = null!;
    private TextBox txtRegEmail = null!;
    private Label lblRegPhone = null!;
    private TextBox txtRegPhone = null!;
    private Label lblRegPass = null!;
    private TextBox txtRegPassword = null!;
    private Label lblRegConfirm = null!;
    private TextBox txtRegConfirm = null!;
    private Label lblRegStreet = null!;
    private TextBox txtRegStreet = null!;
    private Label lblRegDistrict = null!;
    private TextBox txtRegDistrict = null!;
    private Button btnRegister = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblHeaderTitle = new Label();
        btnBack = new Button();
        tabAuth = new TabControl();
        tabLogin = new TabPage();
        tabRegister = new TabPage();

        pnlLoginBox = new Panel();
        lblAccount = new Label();
        txtLoginAccount = new TextBox();
        lblPassword = new Label();
        txtLoginPassword = new TextBox();
        chkShowPassword = new CheckBox();
        btnLogin = new Button();

        lblRegName = new Label();
        txtRegName = new TextBox();
        lblRegEmail = new Label();
        txtRegEmail = new TextBox();
        lblRegPhone = new Label();
        txtRegPhone = new TextBox();
        lblRegPass = new Label();
        txtRegPassword = new TextBox();
        lblRegConfirm = new Label();
        txtRegConfirm = new TextBox();
        lblRegStreet = new Label();
        txtRegStreet = new TextBox();
        lblRegDistrict = new Label();
        txtRegDistrict = new TextBox();
        btnRegister = new Button();

        pnlHeader.SuspendLayout();
        tabAuth.SuspendLayout();
        tabLogin.SuspendLayout();
        tabRegister.SuspendLayout();
        pnlLoginBox.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(640, 54);

        lblHeaderTitle.AutoSize = true;
        lblHeaderTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblHeaderTitle.ForeColor = Color.White;
        lblHeaderTitle.Location = new Point(16, 16);
        lblHeaderTitle.Name = "lblHeaderTitle";
        lblHeaderTitle.Text = "Nhà hàng Hương Việt — Đăng nhập & Đăng ký";
        lblHeaderTitle.UseMnemonic = false;

        btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBack.BackColor = Color.FromArgb(51, 65, 85);
        btnBack.FlatStyle = FlatStyle.Flat;
        btnBack.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnBack.ForeColor = Color.White;
        btnBack.Location = new Point(512, 10);
        btnBack.Name = "btnBack";
        btnBack.Size = new Size(112, 34);
        btnBack.Text = "← Quay lại";
        btnBack.Click += btnBack_Click;

        pnlHeader.Controls.Add(lblHeaderTitle);
        pnlHeader.Controls.Add(btnBack);

        // === TAB 1: LOGIN ===
        pnlLoginBox.Location = new Point(120, 42);
        pnlLoginBox.Name = "pnlLoginBox";
        pnlLoginBox.Size = new Size(390, 240);

        lblAccount.AutoSize = true;
        lblAccount.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblAccount.Location = new Point(8, 8);
        lblAccount.Name = "lblAccount";
        lblAccount.Text = "Email hoặc số điện thoại:";

        txtLoginAccount.Font = new Font("Segoe UI", 10F);
        txtLoginAccount.Location = new Point(8, 34);
        txtLoginAccount.Name = "txtLoginAccount";
        txtLoginAccount.PlaceholderText = "Nhập email hoặc số điện thoại";
        txtLoginAccount.Size = new Size(370, 27);

        lblPassword.AutoSize = true;
        lblPassword.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblPassword.Location = new Point(8, 76);
        lblPassword.Name = "lblPassword";
        lblPassword.Text = "Mật khẩu:";

        txtLoginPassword.Font = new Font("Segoe UI", 10F);
        txtLoginPassword.Location = new Point(8, 102);
        txtLoginPassword.Name = "txtLoginPassword";
        txtLoginPassword.Size = new Size(370, 27);
        txtLoginPassword.UseSystemPasswordChar = true;

        chkShowPassword.AutoSize = true;
        chkShowPassword.Font = new Font("Segoe UI", 9F);
        chkShowPassword.Location = new Point(8, 138);
        chkShowPassword.Name = "chkShowPassword";
        chkShowPassword.Text = "Hiện mật khẩu";
        chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;

        btnLogin.BackColor = Color.FromArgb(29, 78, 216);
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(8, 176);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(370, 44);
        btnLogin.Text = "Đăng nhập";
        btnLogin.Click += btnLogin_Click;

        pnlLoginBox.Controls.Add(lblAccount);
        pnlLoginBox.Controls.Add(txtLoginAccount);
        pnlLoginBox.Controls.Add(lblPassword);
        pnlLoginBox.Controls.Add(txtLoginPassword);
        pnlLoginBox.Controls.Add(chkShowPassword);
        pnlLoginBox.Controls.Add(btnLogin);

        tabLogin.BackColor = Color.White;
        tabLogin.Controls.Add(pnlLoginBox);
        tabLogin.Location = new Point(4, 34);
        tabLogin.Name = "tabLogin";
        tabLogin.Size = new Size(632, 392);
        tabLogin.Text = "Đăng nhập";

        // === TAB 2: REGISTER ===
        lblRegName.AutoSize = true;
        lblRegName.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblRegName.Location = new Point(28, 24);
        lblRegName.Name = "lblRegName";
        lblRegName.Text = "Họ và tên (*):";
        txtRegName.Location = new Point(204, 20);
        txtRegName.Name = "txtRegName";
        txtRegName.Size = new Size(390, 25);

        lblRegEmail.AutoSize = true;
        lblRegEmail.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblRegEmail.Location = new Point(28, 64);
        lblRegEmail.Name = "lblRegEmail";
        lblRegEmail.Text = "Email (*):";
        txtRegEmail.Location = new Point(204, 60);
        txtRegEmail.Name = "txtRegEmail";
        txtRegEmail.Size = new Size(390, 25);

        lblRegPhone.AutoSize = true;
        lblRegPhone.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblRegPhone.Location = new Point(28, 104);
        lblRegPhone.Name = "lblRegPhone";
        lblRegPhone.Text = "Số điện thoại (*):";
        txtRegPhone.Location = new Point(204, 100);
        txtRegPhone.Name = "txtRegPhone";
        txtRegPhone.Size = new Size(390, 25);

        lblRegPass.AutoSize = true;
        lblRegPass.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblRegPass.Location = new Point(28, 144);
        lblRegPass.Name = "lblRegPass";
        lblRegPass.Text = "Mật khẩu (>= 6 ký tự) (*):";
        txtRegPassword.Location = new Point(204, 140);
        txtRegPassword.Name = "txtRegPassword";
        txtRegPassword.Size = new Size(390, 25);
        txtRegPassword.UseSystemPasswordChar = true;

        lblRegConfirm.AutoSize = true;
        lblRegConfirm.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblRegConfirm.Location = new Point(28, 184);
        lblRegConfirm.Name = "lblRegConfirm";
        lblRegConfirm.Text = "Nhập lại mật khẩu (*):";
        txtRegConfirm.Location = new Point(204, 180);
        txtRegConfirm.Name = "txtRegConfirm";
        txtRegConfirm.Size = new Size(390, 25);
        txtRegConfirm.UseSystemPasswordChar = true;

        lblRegStreet.AutoSize = true;
        lblRegStreet.Font = new Font("Segoe UI", 9.5F);
        lblRegStreet.Location = new Point(28, 224);
        lblRegStreet.Name = "lblRegStreet";
        lblRegStreet.Text = "Địa chỉ (số nhà, đường):";
        txtRegStreet.Location = new Point(204, 220);
        txtRegStreet.Name = "txtRegStreet";
        txtRegStreet.Size = new Size(390, 25);

        lblRegDistrict.AutoSize = true;
        lblRegDistrict.Font = new Font("Segoe UI", 9.5F);
        lblRegDistrict.Location = new Point(28, 264);
        lblRegDistrict.Name = "lblRegDistrict";
        lblRegDistrict.Text = "Quận / Huyện:";
        txtRegDistrict.Location = new Point(204, 260);
        txtRegDistrict.Name = "txtRegDistrict";
        txtRegDistrict.Size = new Size(390, 25);
        txtRegDistrict.Text = "Tân Phú";

        btnRegister.BackColor = Color.FromArgb(29, 78, 216);
        btnRegister.FlatStyle = FlatStyle.Flat;
        btnRegister.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnRegister.ForeColor = Color.White;
        btnRegister.Location = new Point(204, 310);
        btnRegister.Name = "btnRegister";
        btnRegister.Size = new Size(390, 44);
        btnRegister.Text = "Đăng ký tài khoản";
        btnRegister.Click += btnRegister_Click;

        tabRegister.AutoScroll = true;
        tabRegister.BackColor = Color.White;
        tabRegister.Controls.Add(lblRegName);
        tabRegister.Controls.Add(txtRegName);
        tabRegister.Controls.Add(lblRegEmail);
        tabRegister.Controls.Add(txtRegEmail);
        tabRegister.Controls.Add(lblRegPhone);
        tabRegister.Controls.Add(txtRegPhone);
        tabRegister.Controls.Add(lblRegPass);
        tabRegister.Controls.Add(txtRegPassword);
        tabRegister.Controls.Add(lblRegConfirm);
        tabRegister.Controls.Add(txtRegConfirm);
        tabRegister.Controls.Add(lblRegStreet);
        tabRegister.Controls.Add(txtRegStreet);
        tabRegister.Controls.Add(lblRegDistrict);
        tabRegister.Controls.Add(txtRegDistrict);
        tabRegister.Controls.Add(btnRegister);
        tabRegister.Location = new Point(4, 34);
        tabRegister.Name = "tabRegister";
        tabRegister.Size = new Size(632, 392);
        tabRegister.Text = "Đăng ký tài khoản mới";

        tabAuth.Controls.Add(tabLogin);
        tabAuth.Controls.Add(tabRegister);
        tabAuth.Dock = DockStyle.Fill;
        tabAuth.Location = new Point(0, 54);
        tabAuth.Name = "tabAuth";
        tabAuth.Size = new Size(640, 430);
        tabAuth.SelectedIndexChanged += tabAuth_SelectedIndexChanged;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(640, 484);
        Controls.Add(tabAuth);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmLogin";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Đăng nhập & Đăng ký — Nhà hàng Hương Việt";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        tabAuth.ResumeLayout(false);
        tabLogin.ResumeLayout(false);
        tabRegister.ResumeLayout(false);
        tabRegister.PerformLayout();
        pnlLoginBox.ResumeLayout(false);
        pnlLoginBox.PerformLayout();
        ResumeLayout(false);
    }
}
