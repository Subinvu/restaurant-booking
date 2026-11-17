using RestaurantBooking.Config;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// [Độc quyền TV3 - Giai đoạn 1] Quản trị danh sách User, đổi Role và Khóa/Mở tài khoản (Admin).
/// Tách riêng khỏi FrmAdmin của TV2 để không bao giờ xung đột code.
/// </summary>
public partial class FrmUserAdmin : Form
{
    public FrmUserAdmin()
    {
        InitializeComponent();
        Load += (_, _) =>
        {
            lblPlaceholder.Text = $"[TV3 - Giai đoạn 1] Quản trị Người dùng — Phiên Admin: {SessionContext.FullName} ({SessionContext.UserCode}).";
        };
    }
}
