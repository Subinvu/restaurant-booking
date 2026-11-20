using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// Màn hình Đăng nhập & Đăng ký tài khoản chính thức của ứng dụng Nhà hàng Hương Việt.
/// </summary>
public partial class FrmLogin : Form
{
    private readonly AuthService _authService = new();

    public FrmLogin()
    {
        InitializeComponent();
        UiTheme.StyleTabControl(tabAuth);
        UiTheme.StyleHeaderBackButton(btnBack);
        UiTheme.StylePrimaryButton(btnLogin);
        UiTheme.StylePrimaryButton(btnRegister);
        Load += (_, _) => CenterLoginBox();
        AcceptButton = btnLogin;
    }

    private void CenterLoginBox()
    {
        pnlLoginBox.Left = Math.Max(16, (tabLogin.ClientSize.Width - pnlLoginBox.Width) / 2);
    }

    private void chkShowPassword_CheckedChanged(object? sender, EventArgs e) =>
        txtLoginPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

    private void tabAuth_SelectedIndexChanged(object? sender, EventArgs e) =>
        AcceptButton = tabAuth.SelectedTab == tabRegister ? btnRegister : btnLogin;

    private void btnBack_Click(object? sender, EventArgs e) => Close();

    private async void btnLogin_Click(object? sender, EventArgs e)
    {
        btnLogin.Enabled = false;
        try
        {
            var result = await _authService.LoginAsync(txtLoginAccount.Text, txtLoginPassword.Text);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtLoginPassword.Clear();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnLogin.Enabled = true;
        }
    }

    private async void btnRegister_Click(object? sender, EventArgs e)
    {
        if (txtRegPassword.Text != txtRegConfirm.Text)
        {
            MessageBox.Show("Mật khẩu nhập lại không khớp.", "Kiểm tra mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnRegister.Enabled = false;
        try
        {
            var result = await _authService.RegisterAsync(
                txtRegName.Text,
                txtRegEmail.Text,
                txtRegPhone.Text,
                txtRegPassword.Text,
                txtRegStreet.Text,
                txtRegDistrict.Text);

            MessageBox.Show(
                result.Message,
                result.Success ? "Đăng ký thành công" : "Đăng ký thất bại",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (result.Success && result.Data != null)
            {
                txtLoginAccount.Text = result.Data.Email;
                txtLoginPassword.Clear();
                txtRegName.Clear();
                txtRegEmail.Clear();
                txtRegPhone.Clear();
                txtRegPassword.Clear();
                txtRegConfirm.Clear();
                txtRegStreet.Clear();
                tabAuth.SelectedTab = tabLogin;
                txtLoginPassword.Focus();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi đăng ký: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnRegister.Enabled = true;
        }
    }
}
