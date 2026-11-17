using RestaurantBooking.Config;

namespace RestaurantBooking.Forms.TV2_Admin;

/// <summary>
/// [Độc quyền TV2 - Giai đoạn 1]
/// Quản trị Nhà hàng/Bàn/Thực đơn (xóa cache Memurai), Thống kê Aggregation Dashboard và Backup/Restore/Export.
/// </summary>
public partial class FrmAdmin : Form
{
    public FrmAdmin()
    {
        InitializeComponent();
        Load += (_, _) =>
        {
            lblInfo.Text = $"Phân hệ TV2 • Quản trị viên: {SessionContext.FullName} ({SessionContext.UserCode})";
        };
    }
}
