using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using RestaurantBooking.Config;
using RestaurantBooking.Models;
using StackExchange.Redis;

namespace RestaurantBooking.Services;

/// <summary>
/// [Độc quyền TV2 - Giai đoạn 1]
/// Xử lý CRUD Nhà hàng/Bàn/Món ăn, Cache-Aside + Random TTL + Anti-Penetration,
/// Bảng xếp hạng Sorted Set và các Aggregation Pipeline thống kê Dashboard.
/// </summary>
public class MenuAdminService
{
    public const int MinTableCapacity = 1;
    public const int MaxTableCapacity = 10;
    public const double MinMenuBasePrice = 1_000;
    public const double MaxMenuBasePrice = 5_000_000;
    public const double MaxMenuExtraPrice = 2_000_000;
    public const int MaxToppingsPerItem = 15;

    public static readonly string[] AllowedAreas = ["indoor", "outdoor", "vip"];
    public static readonly string[] AllowedCategories =
        ["Khai vị", "Món chính", "Lẩu & Nướng", "Đồ uống", "Tráng miệng"];

    private static readonly Regex CodePattern = new("^[A-Z0-9_-]{2,15}$", RegexOptions.Compiled);
    private static readonly Regex BranchOrItemCodePattern = new("^[A-Z0-9_-]{3,15}$", RegexOptions.Compiled);
    private static readonly Regex PhonePattern = new(@"^0[1-9]\d{7,9}$", RegexOptions.Compiled);

    private const string EmptyMenuSentinel = "__EMPTY__";
    private const int CacheBaseTtlSeconds = 300;
    private const int CacheJitterMaxExclusive = 61; // 300..360s chống Cache Avalanche
    private const int EmptySentinelTtlSeconds = 60;  // 60s chống Cache Penetration

    private static readonly JsonSerializerOptions ReadableJsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly DbContext _db = DbContext.Instance;

    private void EnsureInitialized() => _db.Initialize();

    #region Restaurants & Views (INCR views:restaurant:{code})

    public async Task<List<RestaurantModel>> GetRestaurantsAsync(bool activeOnly = false)
    {
        EnsureInitialized();
        var filter = activeOnly
            ? Builders<RestaurantModel>.Filter.Eq(r => r.IsActive, true)
            : FilterDefinition<RestaurantModel>.Empty;

        return await _db.Restaurants
            .Find(filter)
            .SortBy(r => r.RestaurantCode)
            .ToListAsync();
    }

    public async Task<RestaurantModel?> GetRestaurantByCodeAsync(string restaurantCode)
    {
        EnsureInitialized();
        var code = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(code)) return null;

        return await _db.Restaurants
            .Find(r => r.RestaurantCode == code)
            .FirstOrDefaultAsync();
    }

    public async Task<string> GenerateNextRestaurantCodeAsync()
    {
        var restaurants = await GetRestaurantsAsync();
        var maxNumber = restaurants
            .Select(r => ParseCodeSuffix(r.RestaurantCode, "RES"))
            .DefaultIfEmpty(0)
            .Max();
        return $"RES{maxNumber + 1:00}";
    }

    public async Task<long> IncrementViewsAsync(string restaurantCode)
    {
        EnsureInitialized();
        var code = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(code)) return 0;

        try
        {
            return await _db.RedisDb.StringIncrementAsync(RedisKeys.RestaurantViews(code));
        }
        catch (RedisException)
        {
            return 0;
        }
    }

    public async Task<long> GetRestaurantViewsAsync(string restaurantCode)
    {
        EnsureInitialized();
        var code = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(code)) return 0;

        try
        {
            var raw = await _db.RedisDb.StringGetAsync(RedisKeys.RestaurantViews(code));
            return raw.HasValue && long.TryParse(raw.ToString(), out var views) ? views : 0;
        }
        catch (RedisException)
        {
            return 0;
        }
    }

    public async Task<Dictionary<string, long>> GetRestaurantViewsMapAsync(IEnumerable<string> restaurantCodes)
    {
        EnsureInitialized();
        var codes = restaurantCodes
            .Select(NormalizeCode)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .ToList();

        var result = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        if (codes.Count == 0) return result;

        try
        {
            var keys = codes.Select(c => (RedisKey)RedisKeys.RestaurantViews(c)).ToArray();
            var values = await _db.RedisDb.StringGetAsync(keys);

            for (int i = 0; i < codes.Count; i++)
            {
                long views = values[i].HasValue && long.TryParse(values[i].ToString(), out var parsed) ? parsed : 0;
                result[codes[i]] = views;
            }
        }
        catch (RedisException)
        {
            foreach (var code in codes)
                result[code] = 0;
        }

        return result;
    }

    public async Task SaveRestaurantAsync(RestaurantModel restaurant, bool isNew)
    {
        EnsureInitialized();
        ValidateRestaurant(restaurant);

        restaurant.RestaurantCode = NormalizeCode(restaurant.RestaurantCode);
        restaurant.Name = restaurant.Name.Trim();
        restaurant.District = restaurant.District.Trim();
        restaurant.Address = restaurant.Address.Trim();
        restaurant.Phone = NormalizePhone(restaurant.Phone);
        restaurant.OpenTime = restaurant.OpenTime.Trim();
        restaurant.CloseTime = restaurant.CloseTime.Trim();

        var sameNameExists = await _db.Restaurants
            .Find(r => r.RestaurantCode != restaurant.RestaurantCode)
            .ToListAsync();
        if (sameNameExists.Any(r => string.Equals(r.Name.Trim(), restaurant.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Tên chi nhánh '{restaurant.Name}' đã tồn tại ở một chi nhánh khác.");

        if (isNew)
        {
            var existing = await GetRestaurantByCodeAsync(restaurant.RestaurantCode);
            if (existing != null)
                throw new InvalidOperationException($"Mã chi nhánh '{restaurant.RestaurantCode}' đã tồn tại ({existing.Name}).");

            await _db.Restaurants.InsertOneAsync(restaurant);
            await InvalidateMenuCacheAsync(restaurant.RestaurantCode);
            return;
        }

        var update = Builders<RestaurantModel>.Update
            .Set(r => r.Name, restaurant.Name)
            .Set(r => r.District, restaurant.District)
            .Set(r => r.Address, restaurant.Address)
            .Set(r => r.Phone, restaurant.Phone)
            .Set(r => r.OpenTime, restaurant.OpenTime)
            .Set(r => r.CloseTime, restaurant.CloseTime)
            .Set(r => r.IsActive, restaurant.IsActive);

        var res = await _db.Restaurants.UpdateOneAsync(
            r => r.RestaurantCode == restaurant.RestaurantCode,
            update);

        if (res.MatchedCount == 0)
            throw new InvalidOperationException($"Không tìm thấy chi nhánh {restaurant.Name} ({restaurant.RestaurantCode}) để cập nhật.");

        await InvalidateMenuCacheAsync(restaurant.RestaurantCode);
    }

    public async Task DeleteRestaurantAsync(string restaurantCode)
    {
        EnsureInitialized();
        var code = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Mã chi nhánh không hợp lệ.");

        var existing = await GetRestaurantByCodeAsync(code)
            ?? throw new InvalidOperationException($"Không tìm thấy chi nhánh '{code}'.");

        var itemCount = await _db.MenuItems.CountDocumentsAsync(m => m.RestaurantCode == code);
        var orderCount = await _db.BookingsOrders.CountDocumentsAsync(o => o.RestaurantCode == code);
        var reviewCount = await _db.Reviews.CountDocumentsAsync(r => r.RestaurantCode == code);
        if (itemCount > 0 || orderCount > 0 || reviewCount > 0)
        {
            throw new InvalidOperationException(
                $"Chi nhánh {existing.Name} đang có {itemCount} món ăn, {orderCount} đơn hàng và {reviewCount} đánh giá liên quan.\n" +
                "Để ngừng phục vụ mà vẫn giữ lịch sử đơn hàng, hãy bỏ chọn 'Đang hoạt động' rồi bấm 'Cập nhật', hoặc xóa hết các món trước khi xóa chi nhánh.");
        }

        var deleted = await _db.Restaurants.DeleteOneAsync(r => r.RestaurantCode == code);
        if (deleted.DeletedCount == 0)
            throw new InvalidOperationException($"Không tìm thấy chi nhánh {existing.Name}.");

        await InvalidateMenuCacheAsync(code);
        try
        {
            await _db.RedisDb.KeyDeleteAsync(RedisKeys.RestaurantViews(code));
        }
        catch (RedisException)
        {
            // Không chặn thao tác nếu Memurai tạm ngưng.
        }
    }

    #endregion

    #region Tables CRUD (Embedded in restaurants.tables)

    public string GenerateNextTableId(RestaurantModel restaurant)
    {
        var maxNum = restaurant.Tables
            .Select(t => ParseCodeSuffix(t.TableId, "T"))
            .DefaultIfEmpty(0)
            .Max();
        return $"T{maxNum + 1:00}";
    }

    public async Task UpsertTableAsync(string restaurantCode, TableItem table, bool isNew)
    {
        EnsureInitialized();
        var resCode = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(resCode))
            throw new ArgumentException("Chưa chọn chi nhánh cho bàn.");

        table.TableId = NormalizeCode(table.TableId);
        table.TableName = table.TableName.Trim();
        table.Area = string.IsNullOrWhiteSpace(table.Area) ? string.Empty : table.Area.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(table.TableId) || string.IsNullOrEmpty(table.TableName))
            throw new ArgumentException("Mã bàn và tên bàn không được để trống.");
        if (!CodePattern.IsMatch(table.TableId))
            throw new ArgumentException("Mã bàn chỉ được gồm 2 đến 15 ký tự chữ in hoa, số, gạch dưới hoặc gạch nối (ví dụ: T01).");
        if (table.TableName.Length < 2 || table.TableName.Length > 50)
            throw new ArgumentException("Tên bàn phải từ 2 đến 50 ký tự.");
        if (!AllowedAreas.Contains(table.Area, StringComparer.Ordinal))
            throw new ArgumentException("Khu vực bàn phải là Trong nhà, Ngoài trời hoặc Phòng VIP.");
        if (table.Capacity < MinTableCapacity || table.Capacity > MaxTableCapacity)
            throw new ArgumentException($"Số ghế của bàn phải lớn hơn 0 và nhỏ hơn hoặc bằng {MaxTableCapacity}.");

        var restaurant = await GetRestaurantByCodeAsync(resCode)
            ?? throw new InvalidOperationException($"Không tìm thấy chi nhánh '{resCode}'.");

        var exists = restaurant.Tables.Any(t =>
            string.Equals(t.TableId, table.TableId, StringComparison.OrdinalIgnoreCase));

        var duplicateTableName = restaurant.Tables.Any(t =>
            !string.Equals(t.TableId, table.TableId, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(t.TableName.Trim(), table.TableName, StringComparison.OrdinalIgnoreCase));
        if (duplicateTableName)
            throw new InvalidOperationException($"Tên bàn '{table.TableName}' đã tồn tại tại chi nhánh {restaurant.Name}.");

        if (isNew)
        {
            if (exists)
                throw new InvalidOperationException($"Bàn '{table.TableId}' đã tồn tại ở chi nhánh {restaurant.Name}.");

            var push = Builders<RestaurantModel>.Update.Push(r => r.Tables, table);
            await _db.Restaurants.UpdateOneAsync(r => r.RestaurantCode == resCode, push);
            return;
        }

        if (!exists)
            throw new InvalidOperationException($"Không tìm thấy bàn '{table.TableId}' ở chi nhánh {restaurant.Name}.");

        var filter = Builders<RestaurantModel>.Filter.Eq(r => r.RestaurantCode, resCode) &
                     Builders<RestaurantModel>.Filter.ElemMatch(r => r.Tables, t => t.TableId == table.TableId);
        var update = Builders<RestaurantModel>.Update.Set("tables.$", table);
        await _db.Restaurants.UpdateOneAsync(filter, update);
    }

    public async Task DeleteTableAsync(string restaurantCode, string tableId)
    {
        EnsureInitialized();
        var resCode = NormalizeCode(restaurantCode);
        var tId = NormalizeCode(tableId);
        if (string.IsNullOrEmpty(resCode) || string.IsNullOrEmpty(tId))
            throw new ArgumentException("Thiếu thông tin chi nhánh hoặc bàn cần xóa.");

        var restaurant = await GetRestaurantByCodeAsync(resCode);
        var branchName = restaurant?.Name ?? resCode;
        var tableObj = restaurant?.Tables.FirstOrDefault(t => string.Equals(t.TableId, tId, StringComparison.OrdinalIgnoreCase));
        var tableLabel = tableObj != null ? $"{tableObj.TableName} ({tId})" : tId;

        var activeStatuses = new[] { "pending", "confirmed", "serving" };
        var activeOrderCount = await _db.BookingsOrders.CountDocumentsAsync(
            o => o.RestaurantCode == resCode &&
                 o.TableId == tId &&
                 activeStatuses.Contains(o.Status));

        if (activeOrderCount > 0)
        {
            throw new InvalidOperationException(
                $"{tableLabel} tại chi nhánh {branchName} đang có {activeOrderCount} đơn đặt chưa hoàn tất.\n" +
                "Hãy bỏ chọn ô 'Bàn sẵn sàng' rồi bấm 'Sửa bàn' thay vì xóa.");
        }

        var pull = Builders<RestaurantModel>.Update.PullFilter(
            r => r.Tables,
            t => t.TableId == tId);

        var result = await _db.Restaurants.UpdateOneAsync(r => r.RestaurantCode == resCode, pull);
        if (result.ModifiedCount == 0)
            throw new InvalidOperationException($"Không tìm thấy {tableLabel} tại chi nhánh {branchName} để xóa.");
    }

    #endregion

    #region Menu Items CRUD + Cache-Aside (Random TTL & Anti-Penetration)

    public async Task<string> GenerateNextItemCodeAsync()
    {
        EnsureInitialized();
        var items = await _db.MenuItems
            .Find(FilterDefinition<MenuItemModel>.Empty)
            .Project(m => m.ItemCode)
            .ToListAsync();

        var maxNum = items
            .Select(code => ParseCodeSuffix(code, "MN"))
            .DefaultIfEmpty(0)
            .Max();
        return $"MN{maxNum + 1:000}";
    }

    /// <summary>
    /// Đọc thực đơn chi nhánh theo chiến lược Cache-Aside trên Memurai:
    /// - HIT: trả về ngay từ key cache:menu:{restaurantCode}.
    /// - MISS: truy vấn MongoDB, nạp cache với Random TTL 300..360s (chống Cache Avalanche)
    ///   hoặc sentinel "__EMPTY__" 60s nếu chi nhánh không có món (chống Cache Penetration).
    /// </summary>
    public async Task<MenuCacheResult> GetMenuByRestaurantWithCacheAsync(
        string restaurantCode,
        bool includeUnavailable = false,
        bool bypassCache = false)
    {
        EnsureInitialized();
        var code = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Mã chi nhánh không được để trống.");

        var cacheKey = RedisKeys.CacheMenu(code);

        if (!bypassCache)
        {
            try
            {
                var cached = await _db.RedisDb.StringGetAsync(cacheKey);
                if (cached.HasValue)
                {
                    var ttl = await _db.RedisDb.KeyTimeToLiveAsync(cacheKey);
                    var raw = cached.ToString();

                    if (raw == EmptyMenuSentinel)
                    {
                        return new MenuCacheResult
                        {
                            Items = [],
                            IsCacheHit = true,
                            CacheKey = cacheKey,
                            RemainingTtl = ttl,
                            SourceLabel = "Memurai Cache (HIT - Null Sentinel chống Penetration)"
                        };
                    }

                    try
                    {
                        var cachedList = JsonSerializer.Deserialize<List<MenuItemModel>>(raw, ReadableJsonOptions) ?? [];
                        return new MenuCacheResult
                        {
                            Items = FilterAvailability(cachedList, includeUnavailable),
                            IsCacheHit = true,
                            CacheKey = cacheKey,
                            RemainingTtl = ttl,
                            SourceLabel = "Memurai Cache (HIT)"
                        };
                    }
                    catch (JsonException)
                    {
                        // Cache JSON bị hỏng -> xóa key lỗi và fallback đọc lại từ MongoDB.
                        await _db.RedisDb.KeyDeleteAsync(cacheKey);
                    }
                }
            }
            catch (RedisException)
            {
                // Memurai tạm gián đoạn -> fallback an toàn sang MongoDB bên dưới.
            }
        }

        var dbItems = await _db.MenuItems
            .Find(m => m.RestaurantCode == code)
            .SortBy(m => m.Category)
            .ThenBy(m => m.ItemCode)
            .ToListAsync();

        if (dbItems.Count == 0)
        {
            try
            {
                await _db.RedisDb.StringSetAsync(
                    cacheKey,
                    EmptyMenuSentinel,
                    TimeSpan.FromSeconds(EmptySentinelTtlSeconds));
            }
            catch (RedisException) { }

            return new MenuCacheResult
            {
                Items = [],
                IsCacheHit = false,
                CacheKey = cacheKey,
                RemainingTtl = TimeSpan.FromSeconds(EmptySentinelTtlSeconds),
                AppliedTtlSeconds = EmptySentinelTtlSeconds,
                SourceLabel = $"MongoDB (MISS -> Đã ghi Null Sentinel TTL {EmptySentinelTtlSeconds}s)"
            };
        }

        int randomTtl = CacheBaseTtlSeconds + Random.Shared.Next(0, CacheJitterMaxExclusive);
        var json = JsonSerializer.Serialize(dbItems, ReadableJsonOptions);
        bool cacheSaved = false;
        try
        {
            await _db.RedisDb.StringSetAsync(cacheKey, json, TimeSpan.FromSeconds(randomTtl));
            cacheSaved = true;
        }
        catch (RedisException) { }

        return new MenuCacheResult
        {
            Items = FilterAvailability(dbItems, includeUnavailable),
            IsCacheHit = false,
            CacheKey = cacheKey,
            RemainingTtl = cacheSaved ? TimeSpan.FromSeconds(randomTtl) : null,
            AppliedTtlSeconds = cacheSaved ? randomTtl : 0,
            SourceLabel = cacheSaved
                ? $"MongoDB (MISS -> Đã nạp Memurai Cache TTL {randomTtl}s)"
                : "MongoDB (Memurai tạm không khả dụng)"
        };
    }

    public async Task<List<MenuItemModel>> GetAllMenuItemsAsync(
        string? restaurantCode = null,
        string? category = null,
        string? keyword = null,
        bool onlyAvailable = false)
    {
        EnsureInitialized();
        var fb = Builders<MenuItemModel>.Filter;
        var filters = new List<FilterDefinition<MenuItemModel>>();

        var resCode = NormalizeCode(restaurantCode);
        if (!string.IsNullOrEmpty(resCode))
            filters.Add(fb.Eq(m => m.RestaurantCode, resCode));

        if (!string.IsNullOrWhiteSpace(category))
            filters.Add(fb.Eq(m => m.Category, category.Trim()));

        if (onlyAvailable)
            filters.Add(fb.Eq(m => m.IsAvailable, true));

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var escaped = System.Text.RegularExpressions.Regex.Escape(keyword.Trim());
            var regex = new BsonRegularExpression(escaped, "i");
            filters.Add(fb.Or(
                fb.Regex(m => m.Name, regex),
                fb.Regex(m => m.ItemCode, regex)));
        }

        var combined = filters.Count > 0 ? fb.And(filters) : FilterDefinition<MenuItemModel>.Empty;
        return await _db.MenuItems
            .Find(combined)
            .SortBy(m => m.RestaurantCode)
            .ThenBy(m => m.ItemCode)
            .ToListAsync();
    }

    public async Task SaveMenuItemAsync(MenuItemModel item, bool isNew)
    {
        EnsureInitialized();
        ValidateMenuItem(item);

        item.ItemCode = NormalizeCode(item.ItemCode);
        item.RestaurantCode = NormalizeCode(item.RestaurantCode);
        item.Name = item.Name.Trim();
        item.Category = item.Category.Trim();
        item.ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl)
            ? $"images/menu/{item.ItemCode.ToLowerInvariant()}.jpg"
            : item.ImageUrl.Trim();
        foreach (var top in item.Toppings)
            top.Name = top.Name.Trim();
        foreach (var sz in item.Sizes)
        {
            sz.SizeCode = NormalizeCode(sz.SizeCode);
            sz.Label = sz.Label.Trim();
        }

        var restaurantExists = await GetRestaurantByCodeAsync(item.RestaurantCode) != null;
        if (!restaurantExists)
            throw new InvalidOperationException($"Chi nhánh '{item.RestaurantCode}' không tồn tại.");

        var duplicateDishInBranch = await _db.MenuItems
            .Find(m => m.RestaurantCode == item.RestaurantCode && m.ItemCode != item.ItemCode)
            .ToListAsync();
        if (duplicateDishInBranch.Any(m => string.Equals(m.Name.Trim(), item.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Món '{item.Name}' đã có trong thực đơn của chi nhánh '{item.RestaurantCode}'.");

        if (item.Sizes.Count == 0)
        {
            item.Sizes =
            [
                new SizeOption { SizeCode = "S", Label = "Phần vừa", ExtraPrice = 0 },
                new SizeOption { SizeCode = "L", Label = "Phần lớn", ExtraPrice = item.Category == "Đồ uống" ? 12000 : 45000 }
            ];
        }

        if (isNew)
        {
            var dup = await _db.MenuItems.Find(m => m.ItemCode == item.ItemCode).AnyAsync();
            if (dup)
                throw new InvalidOperationException($"Mã món '{item.ItemCode}' đã tồn tại.");

            item.CreatedAt = DateTime.UtcNow;
            await _db.MenuItems.InsertOneAsync(item);
            await InvalidateMenuCacheAsync(item.RestaurantCode);
            return;
        }

        var existing = await _db.MenuItems
            .Find(m => m.ItemCode == item.ItemCode)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Không tìm thấy món '{item.ItemCode}' để cập nhật.");

        var oldRestaurantCode = existing.RestaurantCode;

        var update = Builders<MenuItemModel>.Update
            .Set(m => m.RestaurantCode, item.RestaurantCode)
            .Set(m => m.Name, item.Name)
            .Set(m => m.Category, item.Category)
            .Set(m => m.BasePrice, item.BasePrice)
            .Set(m => m.ImageUrl, item.ImageUrl)
            .Set(m => m.Sizes, item.Sizes)
            .Set(m => m.Toppings, item.Toppings)
            .Set(m => m.IsAvailable, item.IsAvailable);

        await _db.MenuItems.UpdateOneAsync(m => m.ItemCode == item.ItemCode, update);

        await InvalidateMenuCacheAsync(item.RestaurantCode);
        if (!string.Equals(oldRestaurantCode, item.RestaurantCode, StringComparison.OrdinalIgnoreCase))
            await InvalidateMenuCacheAsync(oldRestaurantCode);
    }

    public async Task DeleteMenuItemAsync(string itemCode)
    {
        EnsureInitialized();
        var code = NormalizeCode(itemCode);
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Mã món không hợp lệ.");

        var existing = await _db.MenuItems
            .Find(m => m.ItemCode == code)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Không tìm thấy món '{code}'.");

        await _db.MenuItems.DeleteOneAsync(m => m.ItemCode == code);
        await InvalidateMenuCacheAsync(existing.RestaurantCode);
    }

    public async Task<bool> InvalidateMenuCacheAsync(string restaurantCode)
    {
        EnsureInitialized();
        var code = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(code)) return false;

        try
        {
            return await _db.RedisDb.KeyDeleteAsync(RedisKeys.CacheMenu(code));
        }
        catch (RedisException)
        {
            return false;
        }
    }

    public async Task<long> InvalidateAllMenuCachesAsync()
    {
        EnsureInitialized();
        long deleted = 0;
        try
        {
            var server = _db.GetRedisServer();
            if (server != null)
            {
                var keys = server.Keys(pattern: "cache:menu:*").ToArray();
                if (keys.Length > 0)
                    deleted += await _db.RedisDb.KeyDeleteAsync(keys);
            }

            if (deleted == 0)
            {
                var restaurants = await GetRestaurantsAsync();
                foreach (var r in restaurants)
                {
                    if (await _db.RedisDb.KeyDeleteAsync(RedisKeys.CacheMenu(r.RestaurantCode)))
                        deleted++;
                }
            }
        }
        catch (RedisException)
        {
            return 0;
        }

        return deleted;
    }

    public async Task<(bool Exists, TimeSpan? RemainingTtl, bool IsSentinel)> InspectMenuCacheAsync(string restaurantCode)
    {
        EnsureInitialized();
        var code = NormalizeCode(restaurantCode);
        if (string.IsNullOrEmpty(code)) return (false, null, false);

        try
        {
            var key = RedisKeys.CacheMenu(code);
            var val = await _db.RedisDb.StringGetAsync(key);
            if (!val.HasValue) return (false, null, false);

            var ttl = await _db.RedisDb.KeyTimeToLiveAsync(key);
            return (true, ttl, val.ToString() == EmptyMenuSentinel);
        }
        catch (RedisException)
        {
            return (false, null, false);
        }
    }

    #endregion

    #region Sorted Set Rankings (ranking:menu_items & ranking:restaurants)

    public async Task<List<RankedMenuItemDto>> GetTopRankedItemsAsync(int topN = 10)
    {
        EnsureInitialized();
        if (topN <= 0) topN = 10;

        var entries = await _db.RedisDb.SortedSetRangeByRankWithScoresAsync(
            RedisKeys.RankingMenuItems,
            0,
            topN - 1,
            Order.Descending);

        if (entries.Length == 0) return [];

        var codes = entries.Select(e => e.Element.ToString()).ToList();
        var menuDocs = await _db.MenuItems
            .Find(Builders<MenuItemModel>.Filter.In(m => m.ItemCode, codes))
            .ToListAsync();

        var menuMap = menuDocs.ToDictionary(m => m.ItemCode, StringComparer.OrdinalIgnoreCase);
        var result = new List<RankedMenuItemDto>(entries.Length);

        for (int i = 0; i < entries.Length; i++)
        {
            var code = entries[i].Element.ToString();
            menuMap.TryGetValue(code, out var item);
            result.Add(new RankedMenuItemDto
            {
                Rank = i + 1,
                ItemCode = code,
                Name = item?.Name ?? $"(Món {code})",
                RestaurantCode = item?.RestaurantCode ?? "-",
                Category = item?.Category ?? "-",
                BasePrice = item?.BasePrice ?? 0,
                Score = entries[i].Score
            });
        }

        return result;
    }

    public async Task<List<RankedRestaurantDto>> GetTopRestaurantsAsync(int topN = 10)
    {
        EnsureInitialized();
        if (topN <= 0) topN = 10;

        var entries = await _db.RedisDb.SortedSetRangeByRankWithScoresAsync(
            RedisKeys.RankingRestaurants,
            0,
            topN - 1,
            Order.Descending);

        if (entries.Length == 0) return [];

        var codes = entries.Select(e => e.Element.ToString()).ToList();
        var resDocs = await _db.Restaurants
            .Find(Builders<RestaurantModel>.Filter.In(r => r.RestaurantCode, codes))
            .ToListAsync();

        var resMap = resDocs.ToDictionary(r => r.RestaurantCode, StringComparer.OrdinalIgnoreCase);
        var viewsMap = await GetRestaurantViewsMapAsync(codes);
        var result = new List<RankedRestaurantDto>(entries.Length);

        for (int i = 0; i < entries.Length; i++)
        {
            var code = entries[i].Element.ToString();
            resMap.TryGetValue(code, out var res);
            viewsMap.TryGetValue(code, out var views);

            result.Add(new RankedRestaurantDto
            {
                Rank = i + 1,
                RestaurantCode = code,
                Name = res?.Name ?? code,
                District = res?.District ?? "-",
                Score = entries[i].Score,
                Views = views
            });
        }

        return result;
    }

    #endregion

    #region MongoDB Aggregation Pipelines (Dashboard Statistics)

    public async Task<AdminDashboardSummaryDto> GetAdminDashboardSummaryAsync()
    {
        EnsureInitialized();
        var restaurants = await GetRestaurantsAsync();
        var totalTables = restaurants.Sum(r => r.Tables.Count);
        var totalMenu = await _db.MenuItems.CountDocumentsAsync(FilterDefinition<MenuItemModel>.Empty);
        var totalOrders = await _db.BookingsOrders.CountDocumentsAsync(FilterDefinition<BookingOrderModel>.Empty);

        var pipeline = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "completed_orders", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { "$status", "completed" }),
                        1,
                        0
                    }))
                },
                { "completed_revenue", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { "$status", "completed" }),
                        "$total_amount",
                        0
                    }))
                },
                { "active_revenue", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$ne", new BsonArray { "$status", "cancelled" }),
                        "$total_amount",
                        0
                    }))
                }
            })
        };

        var agg = await _db.BookingsOrders.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

        return new AdminDashboardSummaryDto
        {
            TotalRestaurants = restaurants.Count,
            TotalTables = totalTables,
            TotalMenuItems = totalMenu,
            TotalOrders = totalOrders,
            CompletedOrders = agg != null ? agg["completed_orders"].ToInt64() : 0,
            CompletedRevenue = agg != null ? agg["completed_revenue"].ToDouble() : 0,
            NonCancelledRevenue = agg != null ? agg["active_revenue"].ToDouble() : 0
        };
    }

    /// <summary>
    /// Pipeline 1: Thống kê doanh thu theo Tháng & Chi nhánh ($match -> $addFields -> $group -> $lookup -> $sort).
    /// </summary>
    public async Task<List<RevenueByMonthBranchDto>> GetRevenueByMonthAndBranchAsync(string? restaurantCode = null)
    {
        EnsureInitialized();
        var matchDoc = new BsonDocument("status", "completed");
        var code = NormalizeCode(restaurantCode);
        if (!string.IsNullOrEmpty(code))
            matchDoc["restaurant_code"] = code;

        var pipeline = new[]
        {
            new BsonDocument("$match", matchDoc),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "safe_booking_date", new BsonDocument("$ifNull", new BsonArray { "$booking_date", "" }) }
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "year_month", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$gte", new BsonArray { new BsonDocument("$strLenCP", "$safe_booking_date"), 7 }),
                        new BsonDocument("$substrCP", new BsonArray { "$safe_booking_date", 0, 7 }),
                        "Không rõ"
                    })
                }
            }),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument { { "year_month", "$year_month" }, { "restaurant_code", "$restaurant_code" } } },
                { "order_count", new BsonDocument("$sum", 1) },
                { "total_revenue", new BsonDocument("$sum", "$total_amount") },
                { "avg_order", new BsonDocument("$avg", "$total_amount") }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "restaurants" },
                { "localField", "_id.restaurant_code" },
                { "foreignField", "restaurant_code" },
                { "as", "res_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$res_info" },
                { "preserveNullAndEmptyArrays", true }
            }),
            new BsonDocument("$sort", new BsonDocument
            {
                { "_id.year_month", -1 },
                { "total_revenue", -1 }
            })
        };

        var docs = await _db.BookingsOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return docs.Select(d =>
        {
            var id = d["_id"].AsBsonDocument;
            var resCode = SafeString(id, "restaurant_code");
            var resName = d.Contains("res_info") && d["res_info"].IsBsonDocument
                ? SafeString(d["res_info"].AsBsonDocument, "name", resCode)
                : resCode;

            return new RevenueByMonthBranchDto
            {
                YearMonth = SafeString(id, "year_month", "Không rõ"),
                RestaurantCode = resCode,
                RestaurantName = resName,
                OrderCount = d["order_count"].ToInt32(),
                TotalRevenue = Math.Round(d["total_revenue"].ToDouble(), 0),
                AvgOrderValue = Math.Round(d["avg_order"].ToDouble(), 0)
            };
        }).ToList();
    }

    /// <summary>
    /// Pipeline 2: Top món ăn doanh thu cao nhất ($match -> $unwind $items -> $group -> $lookup menu_items -> $sort -> $limit).
    /// </summary>
    public async Task<List<TopRevenueItemDto>> GetTopItemsByRevenueAsync(int limit = 10, string? restaurantCode = null)
    {
        EnsureInitialized();
        var matchDoc = new BsonDocument("status", new BsonDocument("$ne", "cancelled"));
        var code = NormalizeCode(restaurantCode);
        if (!string.IsNullOrEmpty(code))
            matchDoc["restaurant_code"] = code;

        var pipeline = new[]
        {
            new BsonDocument("$match", matchDoc),
            new BsonDocument("$unwind", "$items"),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$items.item_code" },
                { "name", new BsonDocument("$first", "$items.name") },
                { "total_qty", new BsonDocument("$sum", "$items.qty") },
                { "total_revenue", new BsonDocument("$sum", "$items.subtotal") },
                { "order_appearances", new BsonDocument("$sum", 1) }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "menu_items" },
                { "localField", "_id" },
                { "foreignField", "item_code" },
                { "as", "menu_info" }
            }),
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$menu_info" },
                { "preserveNullAndEmptyArrays", true }
            }),
            new BsonDocument("$sort", new BsonDocument("total_revenue", -1)),
            new BsonDocument("$limit", Math.Max(1, limit))
        };

        var docs = await _db.BookingsOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return docs.Select(d => new TopRevenueItemDto
        {
            ItemCode = SafeString(d, "_id"),
            Name = SafeString(d, "name"),
            Category = d.Contains("menu_info") && d["menu_info"].IsBsonDocument
                ? SafeString(d["menu_info"].AsBsonDocument, "category")
                : "-",
            TotalQty = d["total_qty"].ToInt32(),
            TotalRevenue = Math.Round(d["total_revenue"].ToDouble(), 0),
            OrderAppearances = d["order_appearances"].ToInt32()
        }).ToList();
    }

    /// <summary>
    /// Pipeline 3: Phân tích khung giờ cao điểm ($match -> $group với $cond -> $sort).
    /// </summary>
    public async Task<List<PeakTimeSlotDto>> GetPeakTimeSlotsAsync(string? restaurantCode = null)
    {
        EnsureInitialized();
        var matchDoc = new BsonDocument("status", new BsonDocument("$ne", "cancelled"));
        var code = NormalizeCode(restaurantCode);
        if (!string.IsNullOrEmpty(code))
            matchDoc["restaurant_code"] = code;

        var pipeline = new[]
        {
            new BsonDocument("$match", matchDoc),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument("$ifNull", new BsonArray { "$time_slot", "Không rõ" }) },
                { "total_orders", new BsonDocument("$sum", 1) },
                { "dine_in_orders", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { "$order_type", "dine_in" }),
                        1,
                        0
                    }))
                },
                { "takeaway_orders", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                    {
                        new BsonDocument("$eq", new BsonArray { "$order_type", "takeaway" }),
                        1,
                        0
                    }))
                },
                { "total_guests", new BsonDocument("$sum", "$guest_count") },
                { "total_revenue", new BsonDocument("$sum", "$total_amount") }
            }),
            new BsonDocument("$sort", new BsonDocument { { "total_orders", -1 }, { "total_revenue", -1 } })
        };

        var docs = await _db.BookingsOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return docs.Select(d => new PeakTimeSlotDto
        {
            TimeSlot = SafeString(d, "_id", "Không rõ"),
            TotalOrders = d["total_orders"].ToInt32(),
            DineInOrders = d["dine_in_orders"].ToInt32(),
            TakeawayOrders = d["takeaway_orders"].ToInt32(),
            TotalGuests = d["total_guests"].ToInt32(),
            TotalRevenue = Math.Round(d["total_revenue"].ToDouble(), 0)
        }).ToList();
    }

    /// <summary>
    /// Pipeline 4: Top Khách hàng VIP dùng $addToSet đếm chi nhánh đã ghé ($match -> $group -> $addFields -> $sort -> $limit).
    /// </summary>
    public async Task<List<VipCustomerDto>> GetVipCustomersAsync(int limit = 15, string? restaurantCode = null)
    {
        EnsureInitialized();
        var matchDoc = new BsonDocument("status", "completed");
        var code = NormalizeCode(restaurantCode);
        if (!string.IsNullOrEmpty(code))
            matchDoc["restaurant_code"] = code;

        var pipeline = new[]
        {
            new BsonDocument("$match", matchDoc),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$customer.user_code" },
                { "full_name", new BsonDocument("$first", "$customer.full_name") },
                { "phone", new BsonDocument("$first", "$customer.phone") },
                { "order_count", new BsonDocument("$sum", 1) },
                { "total_spent", new BsonDocument("$sum", "$total_amount") },
                { "visited_restaurants", new BsonDocument("$addToSet", "$restaurant_code") },
                { "last_order_at", new BsonDocument("$max", "$created_at") }
            }),
            new BsonDocument("$addFields", new BsonDocument("branch_count", new BsonDocument("$size", "$visited_restaurants"))),
            new BsonDocument("$sort", new BsonDocument { { "total_spent", -1 }, { "order_count", -1 } }),
            new BsonDocument("$limit", Math.Max(1, limit))
        };

        var docs = await _db.BookingsOrders.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return docs.Select(d =>
        {
            var visited = d["visited_restaurants"].IsBsonArray
                ? d["visited_restaurants"].AsBsonArray
                    .Where(x => x.IsString)
                    .Select(x => x.AsString)
                    .OrderBy(x => x)
                    .ToList()
                : [];

            var lastOrder = d.Contains("last_order_at") && d["last_order_at"].IsValidDateTime
                ? d["last_order_at"].ToUniversalTime().ToLocalTime()
                : DateTime.MinValue;

            return new VipCustomerDto
            {
                UserCode = SafeString(d, "_id"),
                FullName = SafeString(d, "full_name"),
                Phone = SafeString(d, "phone"),
                OrderCount = d["order_count"].ToInt32(),
                TotalSpent = Math.Round(d["total_spent"].ToDouble(), 0),
                BranchCount = d["branch_count"].ToInt32(),
                VisitedRestaurants = string.Join(", ", visited),
                LastOrderAt = lastOrder
            };
        }).ToList();
    }

    /// <summary>
    /// Pipeline 5: Thống kê đánh giá & tỷ lệ phản hồi theo từng chi nhánh ($group -> $lookup restaurants -> $sort).
    /// </summary>
    public async Task<List<RestaurantReviewStatsDto>> GetRestaurantReviewStatsAsync(string? restaurantCode = null)
    {
        EnsureInitialized();
        var stages = new List<BsonDocument>();
        var code = NormalizeCode(restaurantCode);
        if (!string.IsNullOrEmpty(code))
            stages.Add(new BsonDocument("$match", new BsonDocument("restaurant_code", code)));

        stages.Add(new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$restaurant_code" },
            { "review_count", new BsonDocument("$sum", 1) },
            { "avg_rating", new BsonDocument("$avg", "$rating") },
            { "replied_count", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray
                {
                    new BsonDocument("$ne", new BsonArray { "$reply", BsonNull.Value }),
                    1,
                    0
                }))
            }
        }));
        stages.Add(new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "restaurants" },
            { "localField", "_id" },
            { "foreignField", "restaurant_code" },
            { "as", "res_info" }
        }));
        stages.Add(new BsonDocument("$unwind", new BsonDocument
        {
            { "path", "$res_info" },
            { "preserveNullAndEmptyArrays", true }
        }));
        stages.Add(new BsonDocument("$sort", new BsonDocument { { "avg_rating", -1 }, { "review_count", -1 } }));

        var docs = await _db.Reviews.Aggregate<BsonDocument>(stages).ToListAsync();
        return docs.Select(d =>
        {
            var resCode = SafeString(d, "_id");
            var resName = d.Contains("res_info") && d["res_info"].IsBsonDocument
                ? SafeString(d["res_info"].AsBsonDocument, "name", resCode)
                : resCode;
            int count = d["review_count"].ToInt32();
            int replied = d["replied_count"].ToInt32();

            return new RestaurantReviewStatsDto
            {
                RestaurantCode = resCode,
                RestaurantName = resName,
                ReviewCount = count,
                AvgRating = Math.Round(d["avg_rating"].ToDouble(), 2),
                RepliedCount = replied,
                ReplyRatePercent = count > 0 ? Math.Round(replied * 100.0 / count, 1) : 0
            };
        }).ToList();
    }

    #endregion

    #region Helpers & Validation

    private static string SafeString(BsonDocument doc, string field, string fallback = "-")
    {
        if (!doc.TryGetValue(field, out var val) || val == null || val.IsBsonNull)
            return fallback;
        var s = val.ToString();
        return string.IsNullOrWhiteSpace(s) ? fallback : s;
    }

    private static string NormalizeCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();

    private static int ParseCodeSuffix(string code, string prefix)
    {
        if (string.IsNullOrWhiteSpace(code)) return 0;
        var normalized = code.Trim().ToUpperInvariant();
        if (!normalized.StartsWith(prefix, StringComparison.Ordinal)) return 0;
        return int.TryParse(normalized[prefix.Length..], out var num) ? num : 0;
    }

    private static List<MenuItemModel> FilterAvailability(List<MenuItemModel> items, bool includeUnavailable) =>
        includeUnavailable ? items : items.Where(m => m.IsAvailable).ToList();

    private static string NormalizePhone(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var compact = Regex.Replace(raw.Trim(), @"[\s.-]+", string.Empty);
        if (compact.StartsWith("+84", StringComparison.Ordinal))
            compact = "0" + compact[3..];
        return compact;
    }

    private static void ValidateRestaurant(RestaurantModel r)
    {
        var code = NormalizeCode(r.RestaurantCode);
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Mã chi nhánh không được để trống.");
        if (!BranchOrItemCodePattern.IsMatch(code))
            throw new ArgumentException("Mã chi nhánh chỉ được gồm 3 đến 15 ký tự chữ in hoa, số, gạch dưới hoặc gạch nối (ví dụ: RES01).");

        var name = r.Name?.Trim() ?? string.Empty;
        if (name.Length < 2 || name.Length > 100)
            throw new ArgumentException("Tên chi nhánh phải từ 2 đến 100 ký tự.");

        var district = r.District?.Trim() ?? string.Empty;
        if (district.Length < 2 || district.Length > 60)
            throw new ArgumentException("Quận/huyện phải từ 2 đến 60 ký tự.");

        var address = r.Address?.Trim() ?? string.Empty;
        if (address.Length < 5 || address.Length > 200)
            throw new ArgumentException("Địa chỉ chi nhánh phải từ 5 đến 200 ký tự.");

        var phone = NormalizePhone(r.Phone);
        if (!PhonePattern.IsMatch(phone))
            throw new ArgumentException("Số điện thoại không hợp lệ (cần 9 đến 11 chữ số bắt đầu bằng 0, ví dụ: 02838123401 hoặc 0901234567).");

        var openStr = r.OpenTime?.Trim() ?? string.Empty;
        var closeStr = r.CloseTime?.Trim() ?? string.Empty;
        if (!TimeOnly.TryParseExact(openStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var openTime))
            throw new ArgumentException("Giờ mở cửa phải đúng định dạng HH:mm (ví dụ: 09:00).");
        if (!TimeOnly.TryParseExact(closeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var closeTime))
            throw new ArgumentException("Giờ đóng cửa phải đúng định dạng HH:mm (ví dụ: 22:30).");
        if (closeTime <= openTime)
            throw new ArgumentException("Giờ đóng cửa phải sau giờ mở cửa.");
    }

    private static void ValidateMenuItem(MenuItemModel m)
    {
        var itemCode = NormalizeCode(m.ItemCode);
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new ArgumentException("Mã món không được để trống.");
        if (!BranchOrItemCodePattern.IsMatch(itemCode))
            throw new ArgumentException("Mã món chỉ được gồm 3 đến 15 ký tự chữ in hoa, số, gạch dưới hoặc gạch nối (ví dụ: MN001).");

        if (string.IsNullOrWhiteSpace(m.RestaurantCode))
            throw new ArgumentException("Phải chọn chi nhánh cho món ăn.");

        var name = m.Name?.Trim() ?? string.Empty;
        if (name.Length < 2 || name.Length > 100)
            throw new ArgumentException("Tên món ăn phải từ 2 đến 100 ký tự.");

        var category = m.Category?.Trim() ?? string.Empty;
        if (!AllowedCategories.Contains(category, StringComparer.Ordinal))
            throw new ArgumentException("Nhóm món ăn phải thuộc danh mục: Khai vị, Món chính, Lẩu & Nướng, Đồ uống hoặc Tráng miệng.");

        if (double.IsNaN(m.BasePrice) || double.IsInfinity(m.BasePrice) ||
            m.BasePrice < MinMenuBasePrice || m.BasePrice > MaxMenuBasePrice)
            throw new ArgumentException($"Giá gốc của món phải từ {MinMenuBasePrice:N0} đ đến {MaxMenuBasePrice:N0} đ.");

        if (double.IsNaN(m.AvgRating) || double.IsInfinity(m.AvgRating) || m.AvgRating < 0 || m.AvgRating > 5)
            throw new ArgumentException("Điểm đánh giá trung bình phải nằm trong khoảng từ 0 đến 5.");

        var img = m.ImageUrl?.Trim() ?? string.Empty;
        if (img.Length > 255)
            throw new ArgumentException("Đường dẫn ảnh không được vượt quá 255 ký tự.");
        if (!string.IsNullOrEmpty(img) && img.IndexOfAny(['<', '>', '"', '|', '*', '?']) >= 0)
            throw new ArgumentException("Đường dẫn ảnh chứa ký tự không hợp lệ.");

        if (m.Sizes != null)
        {
            var seenSizes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var sz in m.Sizes)
            {
                var sc = sz.SizeCode?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(sc) || string.IsNullOrWhiteSpace(sz.Label))
                    throw new ArgumentException("Mã và tên kích cỡ món ăn không được để trống.");
                if (!seenSizes.Add(sc))
                    throw new ArgumentException($"Bị trùng kích cỡ '{sc}' trong cùng món ăn.");
                if (double.IsNaN(sz.ExtraPrice) || double.IsInfinity(sz.ExtraPrice) ||
                    sz.ExtraPrice < 0 || sz.ExtraPrice > MaxMenuExtraPrice)
                    throw new ArgumentException($"Giá cộng thêm cho kích cỡ '{sc}' phải từ 0 đ đến {MaxMenuExtraPrice:N0} đ.");
            }
        }

        if (m.Toppings != null)
        {
            if (m.Toppings.Count > MaxToppingsPerItem)
                throw new ArgumentException($"Mỗi món ăn chỉ được cấu hình tối đa {MaxToppingsPerItem} món kèm.");

            var seenToppings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var top in m.Toppings)
            {
                var topName = top.Name?.Trim() ?? string.Empty;
                if (topName.Length < 2 || topName.Length > 80)
                    throw new ArgumentException("Tên mỗi món kèm phải từ 2 đến 80 ký tự.");
                if (!seenToppings.Add(topName))
                    throw new ArgumentException($"Món kèm '{topName}' bị trùng lặp.");
                if (double.IsNaN(top.Price) || double.IsInfinity(top.Price) ||
                    top.Price < 0 || top.Price > MaxMenuExtraPrice)
                    throw new ArgumentException($"Giá món kèm '{topName}' phải từ 0 đ đến {MaxMenuExtraPrice:N0} đ.");
            }
        }
    }

    #endregion
}

public sealed class MenuCacheResult
{
    public List<MenuItemModel> Items { get; init; } = [];
    public bool IsCacheHit { get; init; }
    public string CacheKey { get; init; } = string.Empty;
    public TimeSpan? RemainingTtl { get; init; }
    public int AppliedTtlSeconds { get; init; }
    public string SourceLabel { get; init; } = string.Empty;
}

public sealed class RankedMenuItemDto
{
    public long Rank { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string RestaurantCode { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public double BasePrice { get; init; }
    public double Score { get; init; }
}

public sealed class RankedRestaurantDto
{
    public long Rank { get; init; }
    public string RestaurantCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public double Score { get; init; }
    public long Views { get; init; }
}

public sealed class AdminDashboardSummaryDto
{
    public long TotalRestaurants { get; init; }
    public long TotalTables { get; init; }
    public long TotalMenuItems { get; init; }
    public long TotalOrders { get; init; }
    public long CompletedOrders { get; init; }
    public double CompletedRevenue { get; init; }
    public double NonCancelledRevenue { get; init; }
}

public sealed class RevenueByMonthBranchDto
{
    public string YearMonth { get; init; } = string.Empty;
    public string RestaurantCode { get; init; } = string.Empty;
    public string RestaurantName { get; init; } = string.Empty;
    public int OrderCount { get; init; }
    public double TotalRevenue { get; init; }
    public double AvgOrderValue { get; init; }
}

public sealed class TopRevenueItemDto
{
    public string ItemCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int TotalQty { get; init; }
    public double TotalRevenue { get; init; }
    public int OrderAppearances { get; init; }
}

public sealed class PeakTimeSlotDto
{
    public string TimeSlot { get; init; } = string.Empty;
    public int TotalOrders { get; init; }
    public int DineInOrders { get; init; }
    public int TakeawayOrders { get; init; }
    public int TotalGuests { get; init; }
    public double TotalRevenue { get; init; }
}

public sealed class VipCustomerDto
{
    public string UserCode { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public int OrderCount { get; init; }
    public double TotalSpent { get; init; }
    public int BranchCount { get; init; }
    public string VisitedRestaurants { get; init; } = string.Empty;
    public DateTime LastOrderAt { get; init; }
}

public sealed class RestaurantReviewStatsDto
{
    public string RestaurantCode { get; init; } = string.Empty;
    public string RestaurantName { get; init; } = string.Empty;
    public int ReviewCount { get; init; }
    public double AvgRating { get; init; }
    public int RepliedCount { get; init; }
    public double ReplyRatePercent { get; init; }
}
