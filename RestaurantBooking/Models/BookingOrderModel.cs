using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RestaurantBooking.Models;

/// <summary>
/// Đại diện cho một đơn đặt bàn hoặc đặt món trong collection "bookings_orders".
/// </summary>
[BsonIgnoreExtraElements]
public class BookingOrderModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("order_code")]
    public string OrderCode { get; set; } = string.Empty;

    /// <summary>Giá trị hợp lệ: "dine_in" | "takeaway".</summary>
    [BsonElement("order_type")]
    public string OrderType { get; set; } = "dine_in";

    [BsonElement("customer")]
    public CustomerSnapshot Customer { get; set; } = new();

    [BsonElement("restaurant_code")]
    public string RestaurantCode { get; set; } = string.Empty;

    [BsonElement("table_id")]
    public string? TableId { get; set; }

    [BsonElement("delivery_address")]
    [BsonIgnoreIfNull]
    public string? DeliveryAddress { get; set; }

    [BsonElement("note")]
    [BsonIgnoreIfNull]
    public string? Note { get; set; }

    /// <summary>Định dạng chuẩn "YYYY-MM-DD".</summary>
    [BsonElement("booking_date")]
    public string BookingDate { get; set; } = string.Empty;

    /// <summary>Khung giờ: "11:00" | "13:00" | "17:30" | "19:00" | "20:30".</summary>
    [BsonElement("time_slot")]
    public string TimeSlot { get; set; } = string.Empty;

    [BsonElement("guest_count")]
    public int GuestCount { get; set; }

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();

    [BsonElement("voucher_code")]
    public string? VoucherCode { get; set; }

    [BsonElement("discount_amount")]
    public double DiscountAmount { get; set; }

    [BsonElement("total_amount")]
    public double TotalAmount { get; set; }

    /// <summary>Giá trị hợp lệ: "cash" | "momo" | "banking".</summary>
    [BsonElement("payment_method")]
    public string PaymentMethod { get; set; } = "cash";

    /// <summary>Trạng thái: "pending" | "confirmed" | "serving" | "completed" | "cancelled".</summary>
    [BsonElement("status")]
    public string Status { get; set; } = "pending";

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Thông tin khách hàng chụp nhanh tại thời điểm đặt đơn.
/// </summary>
[BsonIgnoreExtraElements]
public class CustomerSnapshot
{
    [BsonElement("user_code")]
    public string UserCode { get; set; } = string.Empty;

    [BsonElement("full_name")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;
}

/// <summary>
/// Chi tiết từng món ăn trong đơn đặt hàng.
/// </summary>
[BsonIgnoreExtraElements]
public class OrderItem
{
    [BsonElement("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("size_code")]
    public string SizeCode { get; set; } = "S";

    [BsonElement("toppings")]
    public List<string> Toppings { get; set; } = new();

    [BsonElement("qty")]
    public int Qty { get; set; }

    [BsonElement("unit_price")]
    public double UnitPrice { get; set; }

    [BsonElement("subtotal")]
    public double Subtotal { get; set; }
}
