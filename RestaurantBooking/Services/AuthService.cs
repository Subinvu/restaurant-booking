using RestaurantBooking.Config;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV3 - Giai đoạn 1]
/// Xử lý Đăng ký, Đăng nhập (BCrypt + lưu Hash session:{token} TTL 24h trên Memurai), Đăng xuất và Quản trị User.
/// </summary>
public class AuthService
{
    private readonly DbContext _db = DbContext.Instance;

    // TODO (TV3 - Giai đoạn 1):
    // 1. RegisterAsync(fullName, email, phone, password) -> tạo UserModel mới vào MongoDB users
    // 2. LoginAsync(emailOrPhone, password) -> kiểm tra BCrypt, tạo session:{token} Hash + EXPIRE 86400 trên Memurai, cập nhật SessionContext
    // 3. LogoutAsync() -> KeyDelete(session:{token}) + SessionContext.Clear()
    // 4. GetAllUsersAsync(keyword) -> cho FrmUserAdmin
    // 5. UpdateUserRoleAndStatusAsync(userCode, role, isActive) -> cho FrmUserAdmin
}
