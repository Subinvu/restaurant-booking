using System.Text.Encodings.Web;
using System.Text.Json;
using MongoDB.Driver;
using RestaurantBooking.Config;
using RestaurantBooking.Models;
using StackExchange.Redis;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV3 - Giai đoạn 1]
/// Xử lý Giỏ hàng trên Memurai Hash (cart:{user_code}), Lịch sử đơn của khách và Đánh giá sao (reviews).
/// Tương thích 100% với CartItemDto của TV1 (BookingService) và Cache invalidation của TV2 (MenuAdminService).
/// </summary>
public class CartReviewService
{
    private static readonly JsonSerializerOptions ReadableJsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true
    };

    private readonly DbContext _db = DbContext.Instance;
    private readonly MenuAdminService _menuAdminService = new();

    #region Memurai Hash Cart (cart:{user_code})

    public async Task<List<CartItemDto>> GetCartAsync(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode)) return [];
        _db.Initialize();

        var entries = await _db.RedisDb.HashGetAllAsync(RedisKeys.Cart(userCode));
        var list = new List<CartItemDto>(entries.Length);

        foreach (var entry in entries)
        {
            if (entry.Value.IsNullOrEmpty) continue;
            try
            {
                var item = JsonSerializer.Deserialize<CartItemDto>(entry.Value.ToString(), ReadableJsonOptions);
                if (item == null) continue;
                item.HashField = entry.Name.ToString();
                list.Add(item);
            }
            catch (JsonException) { }
        }

        return list.OrderBy(i => i.ItemCode).ThenBy(i => i.SizeCode).ToList();
    }

    public async Task<ServiceResult<CartItemDto>> AddOrUpdateCartItemAsync(
        string userCode,
        MenuItemModel menuItem,
        string sizeCode,
        List<ToppingOption>? selectedToppings,
        int qtyToAdd = 1,
        bool clearDifferentBranch = false)
    {
        if (string.IsNullOrWhiteSpace(userCode))
            return ServiceResult<CartItemDto>.Fail("Vui lòng đăng nhập trước khi thêm món vào giỏ hàng.");
        if (!menuItem.IsAvailable)
            return ServiceResult<CartItemDto>.Fail($"Món '{menuItem.Name}' hiện đang tạm ngưng phục vụ.");
        if (qtyToAdd <= 0)
            return ServiceResult<CartItemDto>.Fail("Số lượng thêm vào giỏ phải lớn hơn 0.");

        _db.Initialize();

        var existingBranch = await GetCartBranchCodeAsync(userCode);
        if (!string.IsNullOrEmpty(existingBranch) &&
            !string.Equals(existingBranch, menuItem.RestaurantCode, StringComparison.OrdinalIgnoreCase))
        {
            if (!clearDifferentBranch)
            {
                return ServiceResult<CartItemDto>.Fail(
                    $"BRANCH_CONFLICT|Giỏ hàng hiện đang chứa món của chi nhánh {existingBranch}. Bạn có muốn xóa giỏ cũ để đặt món tại chi nhánh {menuItem.RestaurantCode} không?");
            }

            await _db.RedisDb.KeyDeleteAsync(RedisKeys.Cart(userCode));
        }

        var cleanSize = string.IsNullOrWhiteSpace(sizeCode) ? "S" : sizeCode.Trim().ToUpperInvariant();
        var sizeObj = menuItem.Sizes.FirstOrDefault(s =>
            string.Equals(s.SizeCode, cleanSize, StringComparison.OrdinalIgnoreCase));
        double sizeExtra = sizeObj?.ExtraPrice ?? 0;

        var toppings = selectedToppings ?? [];
        double toppingTotal = toppings.Sum(t => t.Price);
        double unitPrice = menuItem.BasePrice + sizeExtra + toppingTotal;

        var cartKey = RedisKeys.Cart(userCode);
        var hashField = $"{menuItem.ItemCode}_{cleanSize}";

        int finalQty = qtyToAdd;
        var existingRaw = await _db.RedisDb.HashGetAsync(cartKey, hashField);
        if (existingRaw.HasValue)
        {
            try
            {
                var existing = JsonSerializer.Deserialize<CartItemDto>(existingRaw.ToString(), ReadableJsonOptions);
                if (existing != null)
                    finalQty = existing.Qty + qtyToAdd;
            }
            catch (JsonException) { }
        }

        var dto = new CartItemDto
        {
            HashField = hashField,
            ItemCode = menuItem.ItemCode,
            RestaurantCode = menuItem.RestaurantCode,
            Name = menuItem.Name,
            SizeCode = cleanSize,
            Toppings = toppings.Select(t => t.Name).ToList(),
            Qty = finalQty,
            UnitPrice = unitPrice
        };

        var json = JsonSerializer.Serialize(dto, ReadableJsonOptions);
        await _db.RedisDb.HashSetAsync(cartKey, hashField, json);
        await _db.RedisDb.KeyExpireAsync(cartKey, TimeSpan.FromDays(7));

        return ServiceResult<CartItemDto>.Ok(
            $"Đã thêm {qtyToAdd}x '{menuItem.Name}' (Size {cleanSize}) vào giỏ hàng.",
            dto);
    }

    public async Task<string?> GetCartBranchCodeAsync(string userCode)
    {
        var items = await GetCartAsync(userCode);
        if (items.Count == 0) return null;

        var firstWithBranch = items.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.RestaurantCode));
        if (firstWithBranch != null) return firstWithBranch.RestaurantCode;

        // Fallback cho dữ liệu giỏ hàng mẫu cũ chưa lưu trường restaurant_code
        var firstCode = items[0].ItemCode;
        var menuDoc = await _db.MenuItems.Find(m => m.ItemCode == firstCode).FirstOrDefaultAsync();
        return menuDoc?.RestaurantCode;
    }

    public async Task<ServiceResult> UpdateCartItemQtyAsync(string userCode, string hashField, int newQty)
    {
        if (string.IsNullOrWhiteSpace(userCode) || string.IsNullOrWhiteSpace(hashField))
            return ServiceResult.Fail("Thiếu thông tin sản phẩm trong giỏ.");

        _db.Initialize();
        var cartKey = RedisKeys.Cart(userCode);

        if (newQty <= 0)
        {
            await _db.RedisDb.HashDeleteAsync(cartKey, hashField);
            return ServiceResult.Ok("Đã xóa món khỏi giỏ hàng.");
        }

        var existingRaw = await _db.RedisDb.HashGetAsync(cartKey, hashField);
        if (!existingRaw.HasValue)
            return ServiceResult.Fail("Không tìm thấy món này trong giỏ hàng.");

        var item = JsonSerializer.Deserialize<CartItemDto>(existingRaw.ToString(), ReadableJsonOptions);
        if (item == null)
            return ServiceResult.Fail("Dữ liệu giỏ hàng không hợp lệ.");

        item.Qty = newQty;
        var json = JsonSerializer.Serialize(item, ReadableJsonOptions);
        await _db.RedisDb.HashSetAsync(cartKey, hashField, json);
        return ServiceResult.Ok($"Đã cập nhật số lượng '{item.Name}' thành {newQty}.");
    }

    public async Task<ServiceResult> RemoveCartItemAsync(string userCode, string hashField)
    {
        if (string.IsNullOrWhiteSpace(userCode) || string.IsNullOrWhiteSpace(hashField))
            return ServiceResult.Fail("Chưa chọn món cần xóa.");

        _db.Initialize();
        var removed = await _db.RedisDb.HashDeleteAsync(RedisKeys.Cart(userCode), hashField);
        return removed
            ? ServiceResult.Ok("Đã xóa món khỏi giỏ hàng Memurai.")
            : ServiceResult.Fail("Món không tồn tại trong giỏ hàng.");
    }

    public async Task<ServiceResult> ClearCartAsync(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode)) return ServiceResult.Fail("Chưa xác định người dùng.");
        _db.Initialize();
        await _db.RedisDb.KeyDeleteAsync(RedisKeys.Cart(userCode));
        return ServiceResult.Ok($"Đã xóa toàn bộ giỏ hàng {RedisKeys.Cart(userCode)}.");
    }

    #endregion

    #region Order History & Reviews

    public async Task<List<BookingOrderModel>> GetCustomerOrdersAsync(
        string userCode,
        string? statusFilter = null,
        bool includeAllForStaff = false,
        int limit = 120)
    {
        _db.Initialize();
        var fb = Builders<BookingOrderModel>.Filter;
        var filters = new List<FilterDefinition<BookingOrderModel>>();

        if (!includeAllForStaff)
            filters.Add(fb.Eq(o => o.Customer.UserCode, userCode));

        if (!string.IsNullOrWhiteSpace(statusFilter) && !string.Equals(statusFilter, "ALL", StringComparison.OrdinalIgnoreCase))
            filters.Add(fb.Eq(o => o.Status, statusFilter.Trim().ToLowerInvariant()));

        var combined = filters.Count > 0 ? fb.And(filters) : FilterDefinition<BookingOrderModel>.Empty;
        return await _db.BookingsOrders
            .Find(combined)
            .SortByDescending(o => o.CreatedAt)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<Dictionary<string, ReviewModel>> GetReviewsByOrderCodesAsync(IEnumerable<string> orderCodes)
    {
        _db.Initialize();
        var codes = orderCodes.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
        if (codes.Count == 0) return new Dictionary<string, ReviewModel>(StringComparer.OrdinalIgnoreCase);

        var reviews = await _db.Reviews
            .Find(Builders<ReviewModel>.Filter.In(r => r.OrderCode, codes))
            .ToListAsync();

        var dict = new Dictionary<string, ReviewModel>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in reviews)
        {
            dict.TryAdd(r.OrderCode, r);
        }
        return dict;
    }

    public async Task<List<ReviewModel>> GetReviewsForModerationAsync(string? restaurantCode = null, bool onlyUnreplied = false)
    {
        _db.Initialize();
        var fb = Builders<ReviewModel>.Filter;
        var filters = new List<FilterDefinition<ReviewModel>>();

        if (!string.IsNullOrWhiteSpace(restaurantCode) && !string.Equals(restaurantCode, "ALL", StringComparison.OrdinalIgnoreCase))
            filters.Add(fb.Eq(r => r.RestaurantCode, restaurantCode.Trim().ToUpperInvariant()));

        if (onlyUnreplied)
            filters.Add(fb.Eq(r => r.Reply, null));

        var combined = filters.Count > 0 ? fb.And(filters) : FilterDefinition<ReviewModel>.Empty;
        return await _db.Reviews
            .Find(combined)
            .SortByDescending(r => r.CreatedAt)
            .Limit(150)
            .ToListAsync();
    }

    /// <summary>
    /// Lưu đánh giá sao cho đơn đã hoàn tất, tự động tính lại avg_rating của các món liên quan
    /// trong menu_items và vô hiệu hóa cache:menu:{restaurantCode} để cập nhật điểm sao mới.
    /// </summary>
    public async Task<ServiceResult<ReviewModel>> SubmitReviewAsync(
        string orderCode,
        string userCode,
        string fullName,
        int rating,
        string comment,
        List<string>? likedItems = null)
    {
        _db.Initialize();

        if (rating < 1 || rating > 5)
            return ServiceResult<ReviewModel>.Fail("Số sao đánh giá phải từ 1 đến 5.");

        var cleanComment = comment?.Trim() ?? string.Empty;
        if (cleanComment.Length < 5 || cleanComment.Length > 500)
            return ServiceResult<ReviewModel>.Fail("Nội dung nhận xét phải từ 5 đến 500 ký tự.");

        var order = await _db.BookingsOrders
            .Find(o => o.OrderCode == orderCode)
            .FirstOrDefaultAsync();
        if (order == null)
            return ServiceResult<ReviewModel>.Fail($"Không tìm thấy đơn hàng '{orderCode}'.");

        if (!string.Equals(order.Customer.UserCode, userCode, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<ReviewModel>.Fail("Bạn chỉ có thể đánh giá đơn hàng do chính mình đặt.");

        if (!string.Equals(order.Status, "completed", StringComparison.OrdinalIgnoreCase))
            return ServiceResult<ReviewModel>.Fail($"Chỉ được đánh giá các đơn đã hoàn tất (completed). Trạng thái hiện tại: {order.Status}.");

        var alreadyReviewed = await _db.Reviews.Find(r => r.OrderCode == orderCode).AnyAsync();
        if (alreadyReviewed)
            return ServiceResult<ReviewModel>.Fail($"Đơn hàng {orderCode} đã được gửi đánh giá trước đó.");

        var distinctLiked = (likedItems != null && likedItems.Count > 0)
            ? likedItems.Distinct(StringComparer.OrdinalIgnoreCase).ToList()
            : order.Items.Select(i => i.ItemCode).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var review = new ReviewModel
        {
            ReviewCode = await GenerateNextReviewCodeAsync(),
            OrderCode = order.OrderCode,
            UserCode = userCode,
            FullName = fullName,
            RestaurantCode = order.RestaurantCode,
            Rating = rating,
            Comment = cleanComment,
            LikedItems = distinctLiked,
            Reply = null,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Reviews.InsertOneAsync(review);

        // Cập nhật lại avg_rating cho các món trong đơn/liked_items và làm mới cache thực đơn TV2.
        await RecalculateAvgRatingsAsync(distinctLiked);
        await _menuAdminService.InvalidateMenuCacheAsync(order.RestaurantCode);

        return ServiceResult<ReviewModel>.Ok(
            $"Cảm ơn bạn đã gửi đánh giá {rating}★ ({review.ReviewCode})! Điểm trung bình các món đã được cập nhật.",
            review);
    }

    public async Task<ServiceResult> ReplyToReviewAsync(
        string reviewCode,
        string staffName,
        string content,
        bool? actingIsStaffOrAdmin = null)
    {
        if (!(actingIsStaffOrAdmin ?? SessionContext.IsStaffOrAdmin))
            return ServiceResult.Fail("Chỉ Nhân viên hoặc Quản trị viên mới có quyền phản hồi đánh giá khách hàng.");

        _db.Initialize();
        var cleanCode = reviewCode?.Trim().ToUpperInvariant() ?? string.Empty;
        var cleanContent = content?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(cleanCode))
            return ServiceResult.Fail("Chưa chọn đánh giá cần phản hồi.");
        if (cleanContent.Length < 3 || cleanContent.Length > 400)
            return ServiceResult.Fail("Nội dung phản hồi phải từ 3 đến 400 ký tự.");

        var reply = new ReplyInfo
        {
            StaffName = string.IsNullOrWhiteSpace(staffName) ? "Nhân viên Hương Việt" : staffName.Trim(),
            Content = cleanContent,
            RepliedAt = DateTime.UtcNow
        };

        var res = await _db.Reviews.UpdateOneAsync(
            r => r.ReviewCode == cleanCode,
            Builders<ReviewModel>.Update.Set(r => r.Reply, reply));

        return res.MatchedCount > 0
            ? ServiceResult.Ok($"Đã gửi phản hồi cho đánh giá {cleanCode}.")
            : ServiceResult.Fail($"Không tìm thấy đánh giá '{cleanCode}'.");
    }

    private async Task RecalculateAvgRatingsAsync(List<string> itemCodes)
    {
        foreach (var itemCode in itemCodes)
        {
            var matchedReviews = await _db.Reviews
                .Find(Builders<ReviewModel>.Filter.AnyEq(r => r.LikedItems, itemCode))
                .ToListAsync();

            if (matchedReviews.Count == 0) continue;

            double avg = Math.Round(matchedReviews.Average(r => r.Rating), 2);
            await _db.MenuItems.UpdateOneAsync(
                m => m.ItemCode == itemCode,
                Builders<MenuItemModel>.Update.Set(m => m.AvgRating, avg));
        }
    }

    private async Task<string> GenerateNextReviewCodeAsync()
    {
        var codes = await _db.Reviews
            .Find(FilterDefinition<ReviewModel>.Empty)
            .Project(r => r.ReviewCode)
            .ToListAsync();

        int maxNum = 0;
        foreach (var c in codes)
        {
            if (c.StartsWith("REV", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(c[3..], out var num) &&
                num > maxNum)
            {
                maxNum = num;
            }
        }

        return $"REV{maxNum + 1:000}";
    }

    #endregion
}
