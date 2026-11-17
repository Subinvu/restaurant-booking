using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RestaurantBooking.Models;

/// <summary>
/// Đại diện cho một đánh giá đơn hàng/món ăn trong collection "reviews".
/// </summary>
[BsonIgnoreExtraElements]
public class ReviewModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("review_code")]
    public string ReviewCode { get; set; } = string.Empty;

    [BsonElement("order_code")]
    public string OrderCode { get; set; } = string.Empty;

    [BsonElement("user_code")]
    public string UserCode { get; set; } = string.Empty;

    [BsonElement("full_name")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("restaurant_code")]
    public string RestaurantCode { get; set; } = string.Empty;

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("comment")]
    public string Comment { get; set; } = string.Empty;

    [BsonElement("liked_items")]
    public List<string> LikedItems { get; set; } = new();

    [BsonElement("reply")]
    public ReplyInfo? Reply { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Phản hồi của nhân viên/quản lý nhà hàng nhúng trong ReviewModel.
/// </summary>
[BsonIgnoreExtraElements]
public class ReplyInfo
{
    [BsonElement("staff_name")]
    public string StaffName { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("replied_at")]
    public DateTime RepliedAt { get; set; } = DateTime.UtcNow;
}
