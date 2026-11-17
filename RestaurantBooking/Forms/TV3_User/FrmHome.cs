using RestaurantBooking.Config;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// [Độc quyền TV3 - Giai đoạn 1] Trang chủ chọn Chi nhánh, Thực đơn Cache-Aside, Top món hot & Giỏ hàng Memurai Hash.
/// </summary>
public partial class FrmHome : Form
{
    public FrmHome()
    {
        InitializeComponent();
        Load += (_, _) =>
        {
            lblPlaceholder.Text = $"[TV3 - Giai đoạn 1] Trang chủ Thực đơn & Giỏ hàng Memurai ({SessionContext.FullName} - {SessionContext.UserCode}).";
        };
    }
}
