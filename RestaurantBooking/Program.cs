using RestaurantBooking.Config;
using RestaurantBooking.Forms;
using RestaurantBooking.Services;

namespace RestaurantBooking;

internal static class Program
{
    /// <summary>
    /// Điểm khởi chạy chính của ứng dụng Nhà hàng Hương Việt (WinForms .NET 9).
    /// Hỗ trợ tham số "--seed" để nạp dữ liệu mẫu qua dòng lệnh.
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
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => ShowFriendlyUnhandledError(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                ShowFriendlyUnhandledError(ex);
        };

        try
        {
            if (DbContext.Instance.TestConnections(out _))
            {
                Task.Run(() => DataSeeder.SeedAsync(forceReset: false)).GetAwaiter().GetResult();
            }
            else
            {
                MessageBox.Show(
                    "Không kết nối được cơ sở dữ liệu MongoDB (127.0.0.1:27017) hoặc Memurai (127.0.0.1:6379).\n" +
                    "Vui lòng kiểm tra Windows Services (services.msc) rồi thử lại.",
                    "Cảnh báo kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Không thể khởi tạo dữ liệu ban đầu: {ex.Message}",
                "Khởi tạo ứng dụng",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        Application.Run(new FrmMain());
    }

    private static void ShowFriendlyUnhandledError(Exception ex)
    {
        // Bỏ qua lỗi vẽ lại thanh chia SplitContainer tạm thời của WinForms .NET 9 khi cửa sổ đổi kích thước/handle.
        if (ex is System.Runtime.InteropServices.ExternalException &&
            ex.StackTrace?.Contains("RepaintSplitterRect", StringComparison.Ordinal) == true)
        {
            return;
        }

        MessageBox.Show(
            $"Đã xảy ra sự cố kết nối hoặc xử lý tạm thời:\n{ex.Message}\n\nVui lòng kiểm tra kết nối MongoDB / Memurai và thử thao tác lại.",
            "Thông báo hệ thống",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private static void RunSeedCli()
    {
        DbContext.Instance.TestConnections(out var connMsg);
        Console.WriteLine(connMsg);
        var seedMsg = Task.Run(() => DataSeeder.SeedAsync(forceReset: true)).GetAwaiter().GetResult();
        Console.WriteLine(seedMsg);
    }
}
