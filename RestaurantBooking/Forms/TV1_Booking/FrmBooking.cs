using RestaurantBooking.Config;

namespace RestaurantBooking.Forms.TV1_Booking;

/// <summary>
/// [Độc quyền TV1 - Giai đoạn 1]
/// Tab 1: Sơ đồ bàn (Xanh/Vàng TTL 15p/Đỏ), Waitlist và Chốt đơn kèm giỏ hàng Memurai.
/// Tab 2: Quản lý & Duyệt trạng thái đơn (chỉ mở khi Staff hoặc Admin).
/// </summary>
public partial class FrmBooking : Form
{
    public FrmBooking()
    {
        InitializeComponent();
        Load += (_, _) =>
        {
            lblInfo.Text = $"Phân hệ TV1 • Người dùng hiện tại: {SessionContext.FullName} ({SessionContext.UserCode} - {SessionContext.Role})";
            if (!SessionContext.IsStaffOrAdmin && tabMain.TabPages.Contains(tabManageOrders))
            {
                tabMain.TabPages.Remove(tabManageOrders);
            }
        };
    }
}
