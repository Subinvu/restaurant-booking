using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using RestaurantBooking.Config;
using RestaurantBooking.Models;
using StackExchange.Redis;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV3 - Giai đoạn 1]
/// Xử lý Đăng ký, Đăng nhập (BCrypt + lưu Hash session:{token} TTL 24h trên Memurai),
/// Đăng xuất và Quản trị Người dùng (phân quyền & khóa/mở tài khoản).
/// </summary>
public class AuthService
{
    public const int SessionTtlSeconds = 86_400; // 24 giờ theo quy ước Mục 4.2

    public static readonly string[] AllowedRoles = ["customer", "staff", "admin"];

    private static readonly Regex EmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhonePattern = new(@"^0[1-9]\d{7,9}$", RegexOptions.Compiled);

    private readonly DbContext _db = DbContext.Instance;

    public async Task<ServiceResult<UserModel>> RegisterAsync(
        string fullName,
        string email,
        string phone,
        string password,
        string? street = null,
        string? district = null,
        string city = "TP.HCM")
    {
        _db.Initialize();

        var cleanName = fullName?.Trim() ?? string.Empty;
        var cleanEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        var cleanPhone = NormalizePhone(phone);

        if (cleanName.Length < 2 || cleanName.Length > 80)
            return ServiceResult<UserModel>.Fail("Họ tên phải từ 2 đến 80 ký tự.");
        if (!EmailPattern.IsMatch(cleanEmail))
            return ServiceResult<UserModel>.Fail("Địa chỉ email không đúng định dạng (ví dụ: ten@huit.edu.vn).");
        if (!PhonePattern.IsMatch(cleanPhone))
            return ServiceResult<UserModel>.Fail("Số điện thoại không hợp lệ (cần 9–11 chữ số bắt đầu bằng 0).");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return ServiceResult<UserModel>.Fail("Mật khẩu phải có ít nhất 6 ký tự.");

        var emailRegex = new BsonRegularExpression($"^{Regex.Escape(cleanEmail)}$", "i");
        var emailExists = await _db.Users
            .Find(Builders<UserModel>.Filter.Regex(u => u.Email, emailRegex))
            .AnyAsync();
        if (emailExists)
            return ServiceResult<UserModel>.Fail($"Email '{cleanEmail}' đã được đăng ký trước đó.");

        var phoneExists = await _db.Users.Find(u => u.Phone == cleanPhone).AnyAsync();
        if (phoneExists)
            return ServiceResult<UserModel>.Fail($"Số điện thoại '{cleanPhone}' đã được sử dụng bởi tài khoản khác.");

        var nextCode = await GenerateNextUserCodeAsync("USR");
        var addresses = new List<AddressItem>();
        if (!string.IsNullOrWhiteSpace(street) || !string.IsNullOrWhiteSpace(district))
        {
            addresses.Add(new AddressItem
            {
                Label = "Nhà riêng",
                Street = string.IsNullOrWhiteSpace(street) ? "Chưa cập nhật" : street.Trim(),
                District = string.IsNullOrWhiteSpace(district) ? "Tân Phú" : district.Trim(),
                City = string.IsNullOrWhiteSpace(city) ? "TP.HCM" : city.Trim(),
                IsDefault = true
            });
        }

        var user = new UserModel
        {
            UserCode = nextCode,
            FullName = cleanName,
            Email = cleanEmail,
            Phone = cleanPhone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10),
            Role = "customer",
            Addresses = addresses,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Users.InsertOneAsync(user);
        return ServiceResult<UserModel>.Ok("Đăng ký tài khoản thành công! Bạn có thể đăng nhập ngay bây giờ.", user);
    }

    public async Task<UserModel?> GetUserByCodeAsync(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode)) return null;
        _db.Initialize();
        return await _db.Users.Find(u => u.UserCode == userCode).FirstOrDefaultAsync();
    }

    public async Task<ServiceResult<UserModel>> LoginAsync(string emailOrPhone, string password)
    {
        _db.Initialize();

        var input = emailOrPhone?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(password))
            return ServiceResult<UserModel>.Fail("Vui lòng nhập đầy đủ Email/SĐT và Mật khẩu.");

        var normalizedPhone = NormalizePhone(input);
        var emailRegex = new BsonRegularExpression($"^{Regex.Escape(input)}$", "i");
        var filter = Builders<UserModel>.Filter.Or(
            Builders<UserModel>.Filter.Regex(u => u.Email, emailRegex),
            Builders<UserModel>.Filter.Eq(u => u.Phone, normalizedPhone),
            Builders<UserModel>.Filter.Eq(u => u.UserCode, input.ToUpperInvariant()));

        var user = await _db.Users.Find(filter).FirstOrDefaultAsync();
        if (user == null || !VerifyPasswordSafe(password, user.PasswordHash))
            return ServiceResult<UserModel>.Fail("Sai thông tin tài khoản hoặc mật khẩu.");

        if (!user.IsActive)
            return ServiceResult<UserModel>.Fail($"Tài khoản {user.UserCode} hiện đang bị khóa. Vui lòng liên hệ Quản trị viên.");

        // Xóa session cũ trên Memurai nếu trước đó đang có phiên đăng nhập thật
        if (!SessionContext.IsDevFallback && !string.IsNullOrWhiteSpace(SessionContext.SessionToken))
        {
            await DeleteSessionKeySafeAsync(SessionContext.SessionToken);
        }

        var token = Guid.NewGuid().ToString("N");
        var sessionKey = RedisKeys.Session(token);
        var hashEntries = new HashEntry[]
        {
            new("user_code", user.UserCode),
            new("full_name", user.FullName),
            new("role", user.Role),
            new("email", user.Email)
        };

        await _db.RedisDb.HashSetAsync(sessionKey, hashEntries);
        await _db.RedisDb.KeyExpireAsync(sessionKey, TimeSpan.FromSeconds(SessionTtlSeconds));

        SessionContext.SetAuthenticatedUser(user, token);
        return ServiceResult<UserModel>.Ok(
            $"Đăng nhập thành công! Xin chào {user.FullName} ({user.Role.ToUpperInvariant()}).\nSession Memurai đã tạo với TTL 24 giờ.",
            user);
    }

    public async Task<ServiceResult> LogoutAsync()
    {
        _db.Initialize();

        var oldToken = SessionContext.SessionToken;
        bool wasRealSession = !SessionContext.IsDevFallback && !string.IsNullOrWhiteSpace(oldToken);
        if (wasRealSession)
        {
            await DeleteSessionKeySafeAsync(oldToken);
        }

        SessionContext.Clear();
        return ServiceResult.Ok(wasRealSession
            ? "Đã đăng xuất và xóa phiên session:{token} khỏi Memurai."
            : "Đã xóa phiên hiện tại.");
    }

    public async Task<SessionInspectionDto> InspectCurrentSessionAsync()
    {
        _db.Initialize();
        if (string.IsNullOrWhiteSpace(SessionContext.SessionToken) || SessionContext.IsDevFallback)
        {
            return new SessionInspectionDto
            {
                ExistsInMemurai = false,
                SessionKey = SessionContext.IsDevFallback ? "(Đang dùng Dev Fallback — chưa lưu key session:*)" : "(Chưa đăng nhập)"
            };
        }

        var key = RedisKeys.Session(SessionContext.SessionToken);
        var entries = await _db.RedisDb.HashGetAllAsync(key);
        if (entries.Length == 0)
        {
            return new SessionInspectionDto { ExistsInMemurai = false, SessionKey = key };
        }

        var ttl = await _db.RedisDb.KeyTimeToLiveAsync(key);
        return new SessionInspectionDto
        {
            ExistsInMemurai = true,
            SessionKey = key,
            RemainingTtl = ttl,
            Fields = entries.ToDictionary(e => e.Name.ToString(), e => e.Value.ToString())
        };
    }

    public async Task<List<UserModel>> GetAllUsersAsync(
        string? keyword = null,
        string? roleFilter = null,
        bool? activeFilter = null)
    {
        _db.Initialize();
        var fb = Builders<UserModel>.Filter;
        var filters = new List<FilterDefinition<UserModel>>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var regex = new BsonRegularExpression(Regex.Escape(keyword.Trim()), "i");
            filters.Add(fb.Or(
                fb.Regex(u => u.UserCode, regex),
                fb.Regex(u => u.FullName, regex),
                fb.Regex(u => u.Email, regex),
                fb.Regex(u => u.Phone, regex)));
        }

        if (!string.IsNullOrWhiteSpace(roleFilter) && !string.Equals(roleFilter, "ALL", StringComparison.OrdinalIgnoreCase))
            filters.Add(fb.Eq(u => u.Role, roleFilter.Trim().ToLowerInvariant()));

        if (activeFilter.HasValue)
            filters.Add(fb.Eq(u => u.IsActive, activeFilter.Value));

        var combined = filters.Count > 0 ? fb.And(filters) : FilterDefinition<UserModel>.Empty;
        return await _db.Users
            .Find(combined)
            .SortBy(u => u.Role)
            .ThenBy(u => u.UserCode)
            .ToListAsync();
    }

    public async Task<ServiceResult> UpdateUserRoleAndStatusAsync(
        string userCode,
        string newRole,
        bool isActive,
        string actingAdminCode)
    {
        _db.Initialize();
        var cleanCode = userCode?.Trim().ToUpperInvariant() ?? string.Empty;
        var cleanRole = newRole?.Trim().ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrEmpty(cleanCode))
            return ServiceResult.Fail("Chưa chọn tài khoản cần cập nhật.");
        if (!AllowedRoles.Contains(cleanRole, StringComparer.Ordinal))
            return ServiceResult.Fail("Vai trò chỉ nhận một trong ba giá trị: customer, staff, admin.");
        if (!await IsActiveAdminInDbAsync(actingAdminCode))
            return ServiceResult.Fail("Chỉ Quản trị viên đang hoạt động mới có quyền cập nhật phân quyền và khóa tài khoản.");

        var user = await _db.Users.Find(u => u.UserCode == cleanCode).FirstOrDefaultAsync();
        if (user == null)
            return ServiceResult.Fail($"Không tìm thấy tài khoản '{cleanCode}'.");

        if (string.Equals(cleanCode, actingAdminCode, StringComparison.OrdinalIgnoreCase))
        {
            if (!isActive)
                return ServiceResult.Fail("Không thể tự khóa tài khoản Quản trị viên đang đăng nhập.");
            if (cleanRole != "admin")
                return ServiceResult.Fail("Không thể tự hạ quyền Quản trị viên của chính mình.");
        }

        // Bảo vệ hệ thống luôn còn ít nhất 1 tài khoản Admin đang hoạt động.
        if (user.Role == "admin" && (cleanRole != "admin" || !isActive))
        {
            var otherActiveAdmins = await _db.Users.CountDocumentsAsync(
                u => u.Role == "admin" && u.IsActive && u.UserCode != cleanCode);
            if (otherActiveAdmins == 0)
                return ServiceResult.Fail("Hệ thống phải duy trì ít nhất 1 tài khoản Admin đang hoạt động.");
        }

        var update = Builders<UserModel>.Update
            .Set(u => u.Role, cleanRole)
            .Set(u => u.IsActive, isActive);
        await _db.Users.UpdateOneAsync(u => u.UserCode == cleanCode, update);

        // Nếu tài khoản vừa bị khóa, thu hồi ngay các phiên session:* của tài khoản đó trên Memurai.
        int revokedSessions = 0;
        if (!isActive)
            revokedSessions = await RevokeUserSessionsAsync(cleanCode);

        var statusText = isActive ? "Hoạt động" : "Đã khóa";
        var extra = revokedSessions > 0 ? $" (đã thu hồi {revokedSessions} phiên đăng nhập trên Memurai)" : string.Empty;
        return ServiceResult.Ok($"Đã cập nhật tài khoản {cleanCode} → Vai trò: {cleanRole}, Trạng thái: {statusText}{extra}.");
    }

    public async Task<ServiceResult> ResetPasswordAsync(string userCode, string newPassword, string? actingAdminCode = null)
    {
        _db.Initialize();
        var adminCode = actingAdminCode ?? SessionContext.UserCode;
        if (!await IsActiveAdminInDbAsync(adminCode))
            return ServiceResult.Fail("Chỉ Quản trị viên đang hoạt động mới có quyền đặt lại mật khẩu người dùng.");

        var cleanCode = userCode?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrEmpty(cleanCode))
            return ServiceResult.Fail("Chưa chọn tài khoản.");
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            return ServiceResult.Fail("Mật khẩu mới phải có ít nhất 6 ký tự.");

        var hash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 10);
        var res = await _db.Users.UpdateOneAsync(
            u => u.UserCode == cleanCode,
            Builders<UserModel>.Update.Set(u => u.PasswordHash, hash));

        return res.MatchedCount > 0
            ? ServiceResult.Ok($"Đã đặt lại mật khẩu cho tài khoản {cleanCode}.")
            : ServiceResult.Fail($"Không tìm thấy tài khoản '{cleanCode}'.");
    }

    private async Task<bool> IsActiveAdminInDbAsync(string? userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode)) return false;
        var clean = userCode.Trim().ToUpperInvariant();
        return await _db.Users
            .Find(u => u.UserCode == clean && u.Role == "admin" && u.IsActive)
            .AnyAsync();
    }

    private async Task<int> RevokeUserSessionsAsync(string userCode)
    {
        int removed = 0;
        try
        {
            var server = _db.GetRedisServer();
            if (server == null) return 0;

            foreach (var key in server.Keys(pattern: "session:*"))
            {
                var owner = await _db.RedisDb.HashGetAsync(key, "user_code");
                if (owner.HasValue && string.Equals(owner.ToString(), userCode, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _db.RedisDb.KeyDeleteAsync(key))
                        removed++;
                }
            }
        }
        catch (RedisException) { }

        return removed;
    }

    private async Task DeleteSessionKeySafeAsync(string token)
    {
        try
        {
            await _db.RedisDb.KeyDeleteAsync(RedisKeys.Session(token));
        }
        catch (RedisException) { }
    }

    private async Task<string> GenerateNextUserCodeAsync(string prefix)
    {
        var codes = await _db.Users
            .Find(FilterDefinition<UserModel>.Empty)
            .Project(u => u.UserCode)
            .ToListAsync();

        int maxNum = 0;
        foreach (var c in codes)
        {
            if (c.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(c[prefix.Length..], out var num) &&
                num > maxNum)
            {
                maxNum = num;
            }
        }

        return $"{prefix}{maxNum + 1:000}";
    }

    private static bool VerifyPasswordSafe(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash)) return false;
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
        catch
        {
            return false;
        }
    }

    private static string NormalizePhone(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var compact = Regex.Replace(raw.Trim(), @"[\s.-]+", string.Empty);
        if (compact.StartsWith("+84", StringComparison.Ordinal))
            compact = "0" + compact[3..];
        return compact;
    }
}

public sealed class SessionInspectionDto
{
    public bool ExistsInMemurai { get; init; }
    public string SessionKey { get; init; } = string.Empty;
    public TimeSpan? RemainingTtl { get; init; }
    public Dictionary<string, string> Fields { get; init; } = new();
}
