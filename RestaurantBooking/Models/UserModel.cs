using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RestaurantBooking.Models;

/// <summary>
/// Đại diện cho một tài khoản người dùng trong collection "users".
/// </summary>
[BsonIgnoreExtraElements]
public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("user_code")]
    public string UserCode { get; set; } = string.Empty;

    [BsonElement("full_name")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Giá trị hợp lệ: "admin" | "staff" | "customer".</summary>
    [BsonElement("role")]
    public string Role { get; set; } = "customer";

    [BsonElement("addresses")]
    public List<AddressItem> Addresses { get; set; } = new();

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Địa chỉ giao hàng/liên hệ nhúng bên trong UserModel.
/// </summary>
[BsonIgnoreExtraElements]
public class AddressItem
{
    [BsonElement("label")]
    public string Label { get; set; } = string.Empty;

    [BsonElement("street")]
    public string Street { get; set; } = string.Empty;

    [BsonElement("district")]
    public string District { get; set; } = string.Empty;

    [BsonElement("city")]
    public string City { get; set; } = string.Empty;

    [BsonElement("is_default")]
    public bool IsDefault { get; set; }
}
