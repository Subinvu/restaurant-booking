using RestaurantBooking.Config;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV1 - Giai đoạn 1]
/// Xử lý sơ đồ bàn (MongoDB + Memurai lock TTL 15p), chốt đơn, hàng đợi Waitlist (LPUSH/RPOP) và duyệt trạng thái đơn.
/// </summary>
public class BookingService
{
    private readonly DbContext _db = DbContext.Instance;

    // TODO (TV1 - Giai đoạn 1):
    // 1. GetTableStatusesAsync(restaurantCode, bookingDate, timeSlot) -> gộp restaurants.tables + bookings_orders + lock:table:* + TTL
    // 2. LockTableAsync(restaurantCode, tableId, bookingDate, timeSlot, userCode) -> SET NX EX 900
    // 3. ReleaseTableLockAsync(...) -> DEL lock nếu đúng chủ sở hữu
    // 4. JoinWaitlistAsync(...) -> LPUSH waitlist:...
    // 5. CreateBookingOrderAsync(...) -> Đọc giỏ cart:{userCode}, lưu bookings_orders, xóa lock & cart, ZINCRBY ranking
    // 6. CancelOrderAndPopWaitlistAsync(orderCode) -> Cập nhật cancelled + RPOP waitlist
    // 7. UpdateOrderStatusAsync(orderCode, newStatus)
}
