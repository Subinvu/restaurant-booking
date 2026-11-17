using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RestaurantBooking.Models;

/// <summary>
/// Đại diện cho một món ăn trong collection "menu_items".
/// </summary>
[BsonIgnoreExtraElements]
public class MenuItemModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [BsonElement("restaurant_code")]
    public string RestaurantCode { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Nhóm món: "Khai vị" | "Món chính" | "Lẩu & Nướng" | "Đồ uống" | "Tráng miệng".</summary>
    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("base_price")]
    public double BasePrice { get; set; }

    [BsonElement("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [BsonElement("sizes")]
    public List<SizeOption> Sizes { get; set; } = new();

    [BsonElement("toppings")]
    public List<ToppingOption> Toppings { get; set; } = new();

    [BsonElement("is_available")]
    public bool IsAvailable { get; set; } = true;

    [BsonElement("avg_rating")]
    public double AvgRating { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Tùy chọn kích cỡ món ăn nhúng trong MenuItemModel.
/// </summary>
[BsonIgnoreExtraElements]
public class SizeOption
{
    [BsonElement("size_code")]
    public string SizeCode { get; set; } = "S";

    [BsonElement("label")]
    public string Label { get; set; } = string.Empty;

    [BsonElement("extra_price")]
    public double ExtraPrice { get; set; }
}

/// <summary>
/// Tùy chọn topping/món kèm nhúng trong MenuItemModel.
/// </summary>
[BsonIgnoreExtraElements]
public class ToppingOption
{
    [BsonElement("topping_code")]
    public string ToppingCode { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("price")]
    public double Price { get; set; }
}
