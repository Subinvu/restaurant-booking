using RestaurantBooking.Config;
using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// Quản trị danh sách tài khoản, phân quyền, khóa/mở tài khoản và đặt lại mật khẩu (dành cho Admin).
/// </summary>
public partial class FrmUserAdmin : Form
{
    private sealed record RoleChoice(string Value, string Label)
    {
        public override string ToString() => Label;
    }

    private readonly AuthService _authService = new();
    private List<UserModel> _users = [];
    private UserModel? _selectedUser;

    public FrmUserAdmin()
    {
        InitializeComponent();
        UiTheme.StyleHeaderBackButton(btnBack);
        UiTheme.StyleSecondaryButton(btnSearch);
        UiTheme.StylePrimaryButton(btnSaveUser, useGreen: true);
        UiTheme.StyleSecondaryButton(btnResetPass);
        UiTheme.StyleGrid(dgvUsers);
        Load += FrmUserAdmin_Load;
        txtSearch.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoadUsersAsync();
            }
        };
    }

    private void dgvUsers_SelectionChanged(object? sender, EventArgs e) => OnUserSelected();
    private void btnBack_Click(object? sender, EventArgs e) => Close();

    private static string FormatRole(string role) => role.ToLowerInvariant() switch
    {
        "admin" => "Quản trị viên",
        "staff" => "Nhân viên",
        "customer" => "Khách hàng",
        _ => role
    };

    private async void FrmUserAdmin_Load(object? sender, EventArgs e)
    {
        try
        {
            lblInfo.Text = $"Quản trị người dùng — Quản trị viên: {SessionContext.FullName}";

            cboRoleFilter.Items.Clear();
            cboRoleFilter.Items.AddRange([
                new RoleChoice("ALL", "Tất cả vai trò"),
                new RoleChoice("admin", "Quản trị viên"),
                new RoleChoice("staff", "Nhân viên"),
                new RoleChoice("customer", "Khách hàng")
            ]);
            cboRoleFilter.SelectedIndex = 0;

            cboActiveFilter.Items.Clear();
            cboActiveFilter.Items.AddRange(["Tất cả trạng thái", "Đang hoạt động", "Đang bị khóa"]);
            cboActiveFilter.SelectedIndex = 0;

            cboEditRole.Items.Clear();
            cboEditRole.Items.AddRange([
                new RoleChoice("customer", "Khách hàng"),
                new RoleChoice("staff", "Nhân viên"),
                new RoleChoice("admin", "Quản trị viên")
            ]);
            cboEditRole.SelectedIndex = 0;

            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khởi tạo Quản trị người dùng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnSearch_Click(object? sender, EventArgs e)
    {
        try
        {
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi lọc tài khoản: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadUsersAsync(string? selectUserCode = null)
    {
        try
        {
            bool? activeFilter = cboActiveFilter.SelectedIndex switch
            {
                1 => true,
                2 => false,
                _ => null
            };
            var roleFilter = (cboRoleFilter.SelectedItem as RoleChoice)?.Value ?? "ALL";

            _users = await _authService.GetAllUsersAsync(txtSearch.Text, roleFilter, activeFilter);
            if (IsDisposed) return;

            dgvUsers.DataSource = _users.Select(u => new
            {
                MaTK = u.UserCode,
                HoTen = u.FullName,
                Email = u.Email,
                SDT = u.Phone,
                VaiTro = FormatRole(u.Role),
                TrangThai = u.IsActive ? "Hoạt động" : "Đã khóa",
                DiaChi = u.Addresses.FirstOrDefault() is { } addr
                    ? $"{addr.Street}, {addr.District}"
                    : "-",
                NgayTao = u.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy")
            }).ToList();

            ApplyUserGridHeaders();

            int adminCnt = _users.Count(u => u.Role == "admin");
            int staffCnt = _users.Count(u => u.Role == "staff");
            int custCnt = _users.Count(u => u.Role == "customer");
            int lockedCnt = _users.Count(u => !u.IsActive);
            lblStats.Text = $"Hiển thị: {_users.Count} tài khoản  •  Quản trị: {adminCnt}  |  Nhân viên: {staffCnt}  |  Khách hàng: {custCnt}  |  Đang khóa: {lockedCnt}";

            if (!string.IsNullOrEmpty(selectUserCode))
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    if (string.Equals(_users[i].UserCode, selectUserCode, StringComparison.OrdinalIgnoreCase))
                    {
                        dgvUsers.Rows[i].Selected = true;
                        dgvUsers.CurrentCell = dgvUsers.Rows[i].Cells[0];
                        break;
                    }
                }
            }

            OnUserSelected();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải danh sách người dùng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ApplyUserGridHeaders()
    {
        if (dgvUsers.Columns["MaTK"] is { } c1) { c1.HeaderText = "Mã TK"; c1.FillWeight = 62; }
        if (dgvUsers.Columns["HoTen"] is { } c2) { c2.HeaderText = "Họ và tên"; c2.FillWeight = 120; }
        if (dgvUsers.Columns["Email"] is { } c3) { c3.HeaderText = "Email"; c3.FillWeight = 148; }
        if (dgvUsers.Columns["SDT"] is { } c4) { c4.HeaderText = "Số điện thoại"; c4.FillWeight = 92; }
        if (dgvUsers.Columns["VaiTro"] is { } c5) { c5.HeaderText = "Vai trò"; c5.FillWeight = 95; }
        if (dgvUsers.Columns["TrangThai"] is { } c6) { c6.HeaderText = "Trạng thái"; c6.FillWeight = 78; }
        if (dgvUsers.Columns["DiaChi"] is { } c7) { c7.HeaderText = "Địa chỉ"; c7.FillWeight = 145; }
        if (dgvUsers.Columns["NgayTao"] is { } c8) { c8.HeaderText = "Ngày tạo"; c8.FillWeight = 82; }
    }

    private void OnUserSelected()
    {
        int idx = dgvUsers.CurrentRow?.Index ?? -1;
        if (idx < 0 || idx >= _users.Count)
        {
            _selectedUser = null;
            lblSelectedSummary.Text = "Chưa chọn tài khoản.";
            btnSaveUser.Enabled = false;
            btnResetPass.Enabled = false;
            return;
        }

        _selectedUser = _users[idx];
        var addrCount = _selectedUser.Addresses.Count;
        lblSelectedSummary.Text =
            $"Đang chọn: {_selectedUser.UserCode} — {_selectedUser.FullName} | {_selectedUser.Email} | {_selectedUser.Phone} ({addrCount} địa chỉ)";

        for (int i = 0; i < cboEditRole.Items.Count; i++)
        {
            if (cboEditRole.Items[i] is RoleChoice rc &&
                string.Equals(rc.Value, _selectedUser.Role, StringComparison.OrdinalIgnoreCase))
            {
                cboEditRole.SelectedIndex = i;
                break;
            }
        }

        chkEditActive.Checked = _selectedUser.IsActive;
        btnSaveUser.Enabled = true;
        btnResetPass.Enabled = true;
    }

    private async void btnSaveUser_Click(object? sender, EventArgs e)
    {
        if (_selectedUser == null) return;

        btnSaveUser.Enabled = false;
        try
        {
            var newRole = (cboEditRole.SelectedItem as RoleChoice)?.Value ?? "customer";
            var res = await _authService.UpdateUserRoleAndStatusAsync(
                _selectedUser.UserCode,
                newRole,
                chkEditActive.Checked,
                SessionContext.UserCode);

            MessageBox.Show(
                res.Message,
                res.Success ? "Cập nhật thành công" : "Không hợp lệ",
                MessageBoxButtons.OK,
                res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (res.Success)
                await LoadUsersAsync(_selectedUser.UserCode);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi cập nhật tài khoản: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnSaveUser.Enabled = _selectedUser != null;
        }
    }

    private async void btnResetPass_Click(object? sender, EventArgs e)
    {
        if (_selectedUser == null) return;

        btnResetPass.Enabled = false;
        try
        {
            var res = await _authService.ResetPasswordAsync(
                _selectedUser.UserCode,
                txtResetPass.Text,
                SessionContext.UserCode);

            MessageBox.Show(
                res.Message,
                res.Success ? "Đặt lại mật khẩu thành công" : "Không thể đặt lại mật khẩu",
                MessageBoxButtons.OK,
                res.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (res.Success)
                txtResetPass.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi đặt lại mật khẩu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (!IsDisposed) btnResetPass.Enabled = _selectedUser != null;
        }
    }
}
