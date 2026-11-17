using RestaurantBooking.Config;
using RestaurantBooking.Forms;
using RestaurantBooking.Services;

namespace RestaurantBooking;

internal static class Program
{
    /// <summary>
    /// [Cố định Ngày 1 - TV1] Điểm khởi chạy chính của phần mềm WinForms.
    /// Hỗ trợ tham số "--seed" để kiểm thử nạp dữ liệu mẫu qua dòng lệnh.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Contains("--seed", StringComparer.OrdinalIgnoreCase))
        {
            RunSeedCli();
            return;
        }

        ApplicationConfiguration.Initialize();

        try
        {
            if (DbContext.Instance.TestConnections(out _))
            {
                // Chạy trên thread pool để tránh deadlock SynchronizationContext của WinForms.
                Task.Run(() => DataSeeder.SeedAsync(forceReset: false)).GetAwaiter().GetResult();
            }
            else
            {
                MessageBox.Show(
                    "Không kết nối được MongoDB (127.0.0.1:27017) hoặc Memurai (127.0.0.1:6379).\n" +
                    "Hãy kiểm tra Windows Services (services.msc) rồi bấm 'Kiểm tra lại kết nối' trên màn hình chính.",
                    "Cảnh báo kết nối CSDL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Lỗi khi khởi tạo dữ liệu mẫu: {ex.Message}",
                "Khởi tạo CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        Application.Run(new FrmMain());
    }

    private static void RunSeedCli()
    {
        DbContext.Instance.TestConnections(out var connMsg);
        Console.WriteLine(connMsg);
        var seedMsg = Task.Run(() => DataSeeder.SeedAsync(forceReset: true)).GetAwaiter().GetResult();
        Console.WriteLine(seedMsg);
    }
}
