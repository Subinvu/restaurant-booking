using RestaurantBooking.Config;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// [Độc quyền TV3 - Giai đoạn 1] Lịch sử đơn đặt bàn/món của khách, Hủy đơn & Đánh giá sao.
/// </summary>
public partial class FrmHistory : Form
{
    public FrmHistory()
    {
        InitializeComponent();
        Load += (_, _) =>
        {
            lblPlaceholder.Text = $"[TV3 - Giai đoạn 1] Lịch sử đặt bàn/món & Đánh giá của {SessionContext.FullName} ({SessionContext.UserCode}).";
        };
    }
}
