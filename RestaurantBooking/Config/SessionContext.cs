using RestaurantBooking.Models;

namespace RestaurantBooking.Config;

/// <summary>
/// Quản lý phiên đăng nhập hiện tại trên WinForms.
/// Mặc định khởi động ở trạng thái chưa đăng nhập (như app thương mại),
/// đồng thời cung cấp các hàm Dev Fallback khi bật Chế độ Demo Giảng viên.
/// </summary>
public static class SessionContext
{
    public static string UserCode { get; private set; } = string.Empty;
    public static string FullName { get; private set; } = string.Empty;
    public static string Email { get; private set; } = string.Empty;
    public static string Phone { get; private set; } = string.Empty;
    public static string Role { get; private set; } = string.Empty;
    public static string SessionToken { get; private set; } = string.Empty;
    public static bool IsDevFallback { get; private set; }

    public static bool IsLoggedIn => !string.IsNullOrWhiteSpace(UserCode);
    public static bool IsAdmin => string.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase);
    public static bool IsStaffOrAdmin =>
        IsAdmin || string.Equals(Role, "staff", StringComparison.OrdinalIgnoreCase);

    public static void SetAuthenticatedUser(UserModel user, string token)
    {
        UserCode = user.UserCode;
        FullName = user.FullName;
        Email = user.Email;
        Phone = user.Phone;
        Role = user.Role;
        SessionToken = token;
        IsDevFallback = false;
    }

    public static void UseDevCustomer() =>
        ApplyDevAccount("USR001", "Nguyễn Văn An", "an.nguyen@huit.edu.vn", "0901234501", "customer");

    public static void UseDevStaff() =>
        ApplyDevAccount("STF001", "Trần Thị Bình", "binh.tran@nhahang.vn", "0901234502", "staff");

    public static void UseDevAdmin() =>
        ApplyDevAccount("ADM001", "Lê Hoàng Minh", "admin@nhahang.vn", "0901234500", "admin");

    public static void Clear()
    {
        UserCode = string.Empty;
        FullName = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Role = string.Empty;
        SessionToken = string.Empty;
        IsDevFallback = false;
    }

    private static void ApplyDevAccount(string code, string name, string email, string phone, string role)
    {
        UserCode = code;
        FullName = name;
        Email = email;
        Phone = phone;
        Role = role;
        SessionToken = $"dev-fallback-{code.ToLowerInvariant()}";
        IsDevFallback = true;
    }
}
