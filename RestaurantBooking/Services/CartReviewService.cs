using RestaurantBooking.Config;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV3 - Giai đoạn 1]
/// Xử lý Giỏ hàng trên Memurai Hash (cart:{user_code}), Lịch sử đơn của khách và Đánh giá sao (reviews).
/// </summary>
public class CartReviewService
{
    private readonly DbContext _db = DbContext.Instance;

    // TODO (TV3 - Giai đoạn 1):
    // 1. GetCartAsync(userCode) -> đọc HashGetAll cart:{userCode}
    // 2. AddOrUpdateCartItemAsync(userCode, itemCode, sizeCode, ...) -> HashSet cart:{userCode}
    // 3. RemoveCartItemAsync(userCode, fieldKey) -> HashDelete
    // 4. GetCustomerOrdersAsync(userCode) -> đọc lịch sử đơn từ bookings_orders cho FrmHistory
    // 5. SubmitReviewAsync(...) -> thêm vào reviews và cập nhật avg_rating trong menu_items
    // 6. ReplyToReviewAsync(reviewCode, staffName, content)
}
