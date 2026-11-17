using RestaurantBooking.Config;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV2 - Giai đoạn 1]
/// Xử lý CRUD Nhà hàng/Bàn/Món ăn, Cache-Aside + Random TTL, Bảng xếp hạng Sorted Set và Thống kê Aggregation.
/// </summary>
public class MenuAdminService
{
    private readonly DbContext _db = DbContext.Instance;

    // TODO (TV2 - Giai đoạn 1):
    // 1. GetRestaurantsAsync() & IncrementViewsAsync(restaurantCode) -> INCR views:restaurant:{code}
    // 2. GetMenuByRestaurantWithCacheAsync(restaurantCode) -> Đọc cache:menu:{code}, nếu MISS đọc MongoDB + ghi Cache với TTL 300..360s
    // 3. InvalidateMenuCacheAsync(restaurantCode) -> KeyDelete(cache:menu:{code}) khi CRUD món
    // 4. GetTopRankedItemsAsync(topN) & GetTopRestaurantsAsync() -> SortedSetRangeByRankWithScores
    // 5. CRUD MenuItemModel & RestaurantModel/TableItem
    // 6. Các hàm Aggregation Pipeline cho Tab Thống kê (Doanh thu tháng/chi nhánh, Top món, Giờ cao điểm, Khách VIP)
}
