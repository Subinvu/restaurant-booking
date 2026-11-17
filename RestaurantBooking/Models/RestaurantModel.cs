using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RestaurantBooking.Models;

/// <summary>
/// Đại diện cho một chi nhánh nhà hàng và danh sách bàn nhúng trong collection "restaurants".
/// </summary>
[BsonIgnoreExtraElements]
public class RestaurantModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("restaurant_code")]
    public string RestaurantCode { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("district")]
    public string District { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("open_time")]
    public string OpenTime { get; set; } = "09:00";

    [BsonElement("close_time")]
    public string CloseTime { get; set; } = "22:30";

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;

    [BsonElement("tables")]
    public List<TableItem> Tables { get; set; } = new();
}

/// <summary>
/// Thông tin bàn ăn nhúng trực tiếp trong mỗi chi nhánh nhà hàng.
/// </summary>
[BsonIgnoreExtraElements]
public class TableItem
{
    [BsonElement("table_id")]
    public string TableId { get; set; } = string.Empty;

    [BsonElement("table_name")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>Giá trị hợp lệ: "indoor" | "outdoor" | "vip".</summary>
    [BsonElement("area")]
    public string Area { get; set; } = "indoor";

    [BsonElement("capacity")]
    public int Capacity { get; set; }

    [BsonElement("is_available")]
    public bool IsAvailable { get; set; } = true;
}
