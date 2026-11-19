using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using RestaurantBooking.Config;
using RestaurantBooking.Models;
using StackExchange.Redis;

namespace RestaurantBooking.Services;

public enum TableVisualState
{
    Available,
    Locked,
    Booked,
    Disabled
}

public sealed class TableStatusDto
{
    public string TableId { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string Area { get; set; } = "indoor";
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
    public TableVisualState State { get; set; }
    public string? LockedByUserCode { get; set; }
    public int RemainingLockSeconds { get; set; }
    public string? BookedOrderCode { get; set; }
    public string? BookedStatus { get; set; }
}

public sealed class CartItemDto
{
    [JsonIgnore]
    public string HashField { get; set; } = string.Empty;

    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("size_code")]
    public string SizeCode { get; set; } = "S";

    [JsonPropertyName("toppings")]
    public List<string> Toppings { get; set; } = new();

    [JsonPropertyName("qty")]
    public int Qty { get; set; }

    [JsonPropertyName("unit_price")]
    public double UnitPrice { get; set; }

    [JsonIgnore]
    public double Subtotal => Qty * UnitPrice;
}

public sealed class WaitlistEntryDto
{
    [JsonPropertyName("user_code")]
    public string UserCode { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("guest_count")]
    public int GuestCount { get; set; }

    [JsonPropertyName("requested_at")]
    public string RequestedAt { get; set; } = string.Empty;
}

public sealed class ServiceResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ServiceResult Ok(string message) => new() { Success = true, Message = message };
    public static ServiceResult Fail(string message) => new() { Success = false, Message = message };
}

public sealed class ServiceResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ServiceResult<T> Ok(string message, T? data = default) =>
        new() { Success = true, Message = message, Data = data };

    public static ServiceResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}

/// <summary>
/// [Độc quyền TV1 - Giai đoạn 1]
/// Xử lý sơ đồ bàn (MongoDB + Memurai lock TTL 15p), chốt đơn, hàng đợi Waitlist (LPUSH/RPOP) và duyệt trạng thái đơn.
/// </summary>
public class BookingService
{
    public const int DefaultLockSeconds = 900;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true
    };

    private static readonly Dictionary<string, HashSet<string>> AllowedTransitions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pending"] = new(StringComparer.OrdinalIgnoreCase) { "confirmed", "serving", "completed" },
        ["confirmed"] = new(StringComparer.OrdinalIgnoreCase) { "serving", "completed" },
        ["serving"] = new(StringComparer.OrdinalIgnoreCase) { "completed" }
    };

    private readonly DbContext _db = DbContext.Instance;

    public static string NormalizeDate(DateTime date) => date.ToString("yyyy-MM-dd");

    public async Task<List<RestaurantModel>> GetActiveRestaurantsAsync()
    {
        _db.Initialize();
        return await _db.Restaurants
            .Find(r => r.IsActive)
            .SortBy(r => r.RestaurantCode)
            .ToListAsync();
    }

    /// <summary>
    /// Gộp trạng thái bàn từ 3 nguồn: restaurants.tables (MongoDB), bookings_orders (MongoDB) và lock:table:* (Memurai).
    /// </summary>
    public async Task<List<TableStatusDto>> GetTableStatusesAsync(
        string restaurantCode,
        string bookingDate,
        string timeSlot)
    {
        _db.Initialize();

        var restaurant = await _db.Restaurants
            .Find(r => r.RestaurantCode == restaurantCode)
            .FirstOrDefaultAsync();
        if (restaurant == null) return new List<TableStatusDto>();

        var activeOrders = await _db.BookingsOrders
            .Find(o =>
                o.RestaurantCode == restaurantCode &&
                o.BookingDate == bookingDate &&
                o.TimeSlot == timeSlot &&
                o.OrderType == "dine_in" &&
                o.TableId != null &&
                o.Status != "cancelled")
            .ToListAsync();

        var bookedByTable = activeOrders
            .GroupBy(o => o.TableId!)
            .ToDictionary(g => g.Key, g => g.First());

        // Dùng StringGetWithExpiryAsync để lấy cả chủ khóa và TTL còn lại trong 1 round-trip tới Memurai.
        var lockTasks = restaurant.Tables.Select(t =>
            _db.RedisDb.StringGetWithExpiryAsync(RedisKeys.LockTable(restaurantCode, t.TableId, bookingDate, timeSlot)))
            .ToArray();
        var lockResults = await Task.WhenAll(lockTasks);

        var statuses = new List<TableStatusDto>(restaurant.Tables.Count);
        for (int i = 0; i < restaurant.Tables.Count; i++)
        {
            var table = restaurant.Tables[i];
            var dto = new TableStatusDto
            {
                TableId = table.TableId,
                TableName = table.TableName,
                Area = table.Area,
                Capacity = table.Capacity,
                IsAvailable = table.IsAvailable
            };

            if (!table.IsAvailable)
            {
                dto.State = TableVisualState.Disabled;
            }
            else if (bookedByTable.TryGetValue(table.TableId, out var bookedOrder))
            {
                dto.State = TableVisualState.Booked;
                dto.BookedOrderCode = bookedOrder.OrderCode;
                dto.BookedStatus = bookedOrder.Status;
            }
            else if (!lockResults[i].Value.IsNullOrEmpty)
            {
                dto.State = TableVisualState.Locked;
                dto.LockedByUserCode = lockResults[i].Value.ToString();
                dto.RemainingLockSeconds = Math.Max(0, (int)Math.Ceiling((lockResults[i].Expiry ?? TimeSpan.Zero).TotalSeconds));
            }
            else
            {
                dto.State = TableVisualState.Available;
            }

            statuses.Add(dto);
        }

        return statuses;
    }

    /// <summary>
    /// Khóa giữ bàn nguyên tử 15 phút bằng lệnh Redis SET ... NX EX 900.
    /// </summary>
    public async Task<ServiceResult<int>> LockTableAsync(
        string restaurantCode,
        string tableId,
        string bookingDate,
        string timeSlot,
        string userCode,
        int ttlSeconds = DefaultLockSeconds)
    {
        if (string.IsNullOrWhiteSpace(userCode))
            return ServiceResult<int>.Fail("Chưa xác định người dùng đang đăng nhập.");

        _db.Initialize();

        var isTableBooked = await HasActiveMongoBookingAsync(restaurantCode, tableId, bookingDate, timeSlot);
        if (isTableBooked)
            return ServiceResult<int>.Fail($"Bàn {tableId} đã được đặt trong khung giờ {timeSlot} ngày {bookingDate}.");

        var lockKey = RedisKeys.LockTable(restaurantCode, tableId, bookingDate, timeSlot);
        var acquired = await _db.RedisDb.StringSetAsync(
            lockKey,
            userCode,
            TimeSpan.FromSeconds(ttlSeconds),
            When.NotExists);

        if (acquired)
            return ServiceResult<int>.Ok($"Đã giữ bàn {tableId} trong {ttlSeconds / 60} phút.", ttlSeconds);

        var currentLock = await _db.RedisDb.StringGetWithExpiryAsync(lockKey);
        int remaining = Math.Max(0, (int)Math.Ceiling((currentLock.Expiry ?? TimeSpan.Zero).TotalSeconds));

        if (!currentLock.Value.IsNullOrEmpty && currentLock.Value == userCode)
        {
            return ServiceResult<int>.Ok(
                $"Bạn đang giữ bàn {tableId} (còn {remaining / 60:00}:{remaining % 60:00}).",
                remaining);
        }

        var holderText = currentLock.Value.IsNullOrEmpty ? "người khác" : currentLock.Value.ToString();
        return ServiceResult<int>.Fail(
            $"Bàn {tableId} đang được {holderText} giữ tạm (còn {remaining / 60:00}:{remaining % 60:00}).");
    }

    /// <summary>
    /// Hủy khóa giữ bàn khi khách đổi ý (chỉ chủ sở hữu khóa hoặc Admin mới được xóa).
    /// </summary>
    public async Task<ServiceResult> ReleaseTableLockAsync(
        string restaurantCode,
        string tableId,
        string bookingDate,
        string timeSlot,
        string userCode,
        bool isAdminOverride = false)
    {
        _db.Initialize();
        var lockKey = RedisKeys.LockTable(restaurantCode, tableId, bookingDate, timeSlot);
        var currentHolder = await _db.RedisDb.StringGetAsync(lockKey);

        if (currentHolder.IsNullOrEmpty)
            return ServiceResult.Ok($"Bàn {tableId} hiện không bị khóa.");

        if (!isAdminOverride && currentHolder != userCode)
            return ServiceResult.Fail($"Không thể nhả bàn {tableId} vì đang được {currentHolder} giữ.");

        await _db.RedisDb.KeyDeleteAsync(lockKey);
        return ServiceResult.Ok($"Đã hủy giữ bàn {tableId}.");
    }

    public async Task<List<CartItemDto>> GetCartItemsAsync(string userCode)
    {
        if (string.IsNullOrWhiteSpace(userCode)) return new List<CartItemDto>();

        _db.Initialize();
        var entries = await _db.RedisDb.HashGetAllAsync(RedisKeys.Cart(userCode));
        var items = new List<CartItemDto>(entries.Length);

        foreach (var entry in entries)
        {
            if (entry.Value.IsNullOrEmpty) continue;
            var item = JsonSerializer.Deserialize<CartItemDto>(entry.Value.ToString(), JsonOptions);
            if (item == null) continue;
            item.HashField = entry.Name.ToString();
            items.Add(item);
        }

        return items;
    }

    /// <summary>
    /// Đưa khách vào hàng đợi FIFO trên Memurai List (LPUSH) khi hết bàn trong khung giờ.
    /// </summary>
    public async Task<ServiceResult<long>> JoinWaitlistAsync(
        string restaurantCode,
        string bookingDate,
        string timeSlot,
        string userCode,
        string fullName,
        string phone,
        int guestCount)
    {
        if (string.IsNullOrWhiteSpace(userCode))
            return ServiceResult<long>.Fail("Chưa xác định tài khoản khách hàng.");

        _db.Initialize();
        var waitlistKey = RedisKeys.Waitlist(restaurantCode, bookingDate, timeSlot);

        var existingEntries = await _db.RedisDb.ListRangeAsync(waitlistKey);
        foreach (var raw in existingEntries)
        {
            var entry = DeserializeWaitlist(raw);
            if (entry != null && string.Equals(entry.UserCode, userCode, StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<long>.Fail(
                    $"Bạn ({userCode}) đã nằm trong hàng đợi Waitlist khung giờ {timeSlot} ngày {bookingDate}.");
            }
        }

        var payload = JsonSerializer.Serialize(new WaitlistEntryDto
        {
            UserCode = userCode,
            FullName = fullName,
            Phone = phone,
            GuestCount = guestCount,
            RequestedAt = DateTime.UtcNow.ToString("O")
        }, JsonOptions);

        long queueLength = await _db.RedisDb.ListLeftPushAsync(waitlistKey, payload);
        return ServiceResult<long>.Ok(
            $"Đã đăng ký hàng đợi Waitlist thành công (Vị trí ưu tiên số #{queueLength} cho {timeSlot} ngày {bookingDate}).",
            queueLength);
    }

    /// <summary>
    /// Trả về hàng đợi theo đúng thứ tự FIFO (người đứng đầu danh sách là người RPOP sẽ rút ra tiếp theo).
    /// </summary>
    public async Task<List<WaitlistEntryDto>> GetWaitlistAsync(
        string restaurantCode,
        string bookingDate,
        string timeSlot)
    {
        _db.Initialize();
        var waitlistKey = RedisKeys.Waitlist(restaurantCode, bookingDate, timeSlot);
        var rawItems = await _db.RedisDb.ListRangeAsync(waitlistKey);

        // Vì đẩy vào đầu bằng LPUSH và lấy ra ở cuối bằng RPOP, phần tử ở cuối mảng ListRange chính là người chờ lâu nhất.
        var list = new List<WaitlistEntryDto>(rawItems.Length);
        for (int i = rawItems.Length - 1; i >= 0; i--)
        {
            var entry = DeserializeWaitlist(rawItems[i]);
            if (entry != null) list.Add(entry);
        }

        return list;
    }

    /// <summary>
    /// Chốt đơn đặt bàn / mang về: kiểm tra khóa Memurai + chống trùng MongoDB, lưu đơn,
    /// xóa khóa bàn & giỏ hàng, và cộng điểm bảng xếp hạng Sorted Set (ZINCRBY).
    /// </summary>
    public async Task<ServiceResult<BookingOrderModel>> CreateBookingOrderAsync(
        string restaurantCode,
        string orderType,
        string? tableId,
        string bookingDate,
        string timeSlot,
        int guestCount,
        string paymentMethod,
        string? voucherCode,
        string customerUserCode,
        string customerFullName,
        string customerPhone)
    {
        if (string.IsNullOrWhiteSpace(customerUserCode))
            return ServiceResult<BookingOrderModel>.Fail("Chưa có thông tin khách hàng.");

        _db.Initialize();

        var cartItems = await GetCartItemsAsync(customerUserCode);
        if (orderType == "takeaway" && cartItems.Count == 0)
            return ServiceResult<BookingOrderModel>.Fail("Đơn mang về cần có ít nhất 1 món ăn trong giỏ hàng Memurai.");

        string? acquiredLockKey = null;
        bool acquiredTempLockHere = false;

        if (orderType == "dine_in")
        {
            if (string.IsNullOrWhiteSpace(tableId))
                return ServiceResult<BookingOrderModel>.Fail("Vui lòng chọn một bàn trống hoặc bàn bạn đang giữ.");

            var restaurant = await _db.Restaurants.Find(r => r.RestaurantCode == restaurantCode).FirstOrDefaultAsync();
            var table = restaurant?.Tables.FirstOrDefault(t => t.TableId == tableId);
            if (table == null || !table.IsAvailable)
                return ServiceResult<BookingOrderModel>.Fail($"Bàn {tableId} không tồn tại hoặc đang ngưng phục vụ.");

            if (guestCount > table.Capacity)
                return ServiceResult<BookingOrderModel>.Fail($"Số khách ({guestCount}) vượt quá sức chứa của {table.TableName} ({table.Capacity} chỗ).");

            acquiredLockKey = RedisKeys.LockTable(restaurantCode, tableId, bookingDate, timeSlot);
            var existingHolder = await _db.RedisDb.StringGetAsync(acquiredLockKey);

            if (!existingHolder.IsNullOrEmpty && existingHolder != customerUserCode)
                return ServiceResult<BookingOrderModel>.Fail($"Bàn {tableId} đang được người dùng {existingHolder} giữ chỗ.");

            // Nếu khách bấm Chốt đơn ngay trên bàn Xanh chưa khóa, vẫn chiếm khóa tạm ngắn để ngăn race condition.
            if (existingHolder.IsNullOrEmpty)
            {
                acquiredTempLockHere = await _db.RedisDb.StringSetAsync(
                    acquiredLockKey,
                    customerUserCode,
                    TimeSpan.FromSeconds(120),
                    When.NotExists);

                if (!acquiredTempLockHere)
                    return ServiceResult<BookingOrderModel>.Fail($"Bàn {tableId} vừa có người khác chọn giữ chỗ trước đó.");
            }

            if (await HasActiveMongoBookingAsync(restaurantCode, tableId, bookingDate, timeSlot))
            {
                if (acquiredTempLockHere) await _db.RedisDb.KeyDeleteAsync(acquiredLockKey);
                return ServiceResult<BookingOrderModel>.Fail($"Bàn {tableId} đã có đơn đặt trong khung giờ {timeSlot} ngày {bookingDate}.");
            }
        }

        try
        {
            var (normalizedVoucher, discount, finalTotal) = CalculatePricing(cartItems, voucherCode);

            var order = new BookingOrderModel
            {
                OrderCode = GenerateOrderCode(bookingDate),
                OrderType = orderType,
                Customer = new CustomerSnapshot
                {
                    UserCode = customerUserCode,
                    FullName = customerFullName,
                    Phone = customerPhone
                },
                RestaurantCode = restaurantCode,
                TableId = orderType == "dine_in" ? tableId : null,
                BookingDate = bookingDate,
                TimeSlot = timeSlot,
                GuestCount = guestCount,
                VoucherCode = normalizedVoucher,
                DiscountAmount = discount,
                TotalAmount = finalTotal,
                PaymentMethod = paymentMethod,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                Items = cartItems.Select(c => new OrderItem
                {
                    ItemCode = c.ItemCode,
                    Name = c.Name,
                    SizeCode = c.SizeCode,
                    Toppings = c.Toppings,
                    Qty = c.Qty,
                    UnitPrice = c.UnitPrice,
                    Subtotal = c.Subtotal
                }).ToList()
            };

            await _db.BookingsOrders.InsertOneAsync(order);

            // Giải phóng khóa giữ bàn, xóa giỏ hàng tạm và cập nhật bảng xếp hạng Sorted Set.
            if (!string.IsNullOrEmpty(acquiredLockKey))
                await _db.RedisDb.KeyDeleteAsync(acquiredLockKey);

            if (cartItems.Count > 0)
                await _db.RedisDb.KeyDeleteAsync(RedisKeys.Cart(customerUserCode));

            await _db.RedisDb.SortedSetIncrementAsync(RedisKeys.RankingRestaurants, restaurantCode, 1);
            foreach (var item in order.Items)
            {
                await _db.RedisDb.SortedSetIncrementAsync(RedisKeys.RankingMenuItems, item.ItemCode, item.Qty);
            }

            return ServiceResult<BookingOrderModel>.Ok(
                $"Chốt đơn thành công! Mã đơn: {order.OrderCode}" +
                (order.TableId != null ? $" • Bàn {order.TableId}" : " • Mang về"),
                order);
        }
        catch (Exception ex)
        {
            if (acquiredTempLockHere && !string.IsNullOrEmpty(acquiredLockKey))
                await _db.RedisDb.KeyDeleteAsync(acquiredLockKey);

            return ServiceResult<BookingOrderModel>.Fail($"Không thể lưu đơn đặt hàng: {ex.Message}");
        }
    }

    public async Task<List<BookingOrderModel>> GetOrdersForManageAsync(
        string? restaurantCode,
        string? bookingDate,
        string? status,
        int limit = 150)
    {
        _db.Initialize();
        var builder = Builders<BookingOrderModel>.Filter;
        var filters = new List<FilterDefinition<BookingOrderModel>>();

        if (!string.IsNullOrWhiteSpace(restaurantCode) && restaurantCode != "ALL")
            filters.Add(builder.Eq(o => o.RestaurantCode, restaurantCode));

        if (!string.IsNullOrWhiteSpace(bookingDate))
            filters.Add(builder.Eq(o => o.BookingDate, bookingDate));

        if (!string.IsNullOrWhiteSpace(status) && status != "ALL")
            filters.Add(builder.Eq(o => o.Status, status));

        var combinedFilter = filters.Count > 0 ? builder.And(filters) : builder.Empty;

        return await _db.BookingsOrders
            .Find(combinedFilter)
            .SortByDescending(o => o.CreatedAt)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<ServiceResult> UpdateOrderStatusAsync(string orderCode, string newStatus)
    {
        if (string.Equals(newStatus, "cancelled", StringComparison.OrdinalIgnoreCase))
            return ServiceResult.Fail("Vui lòng dùng chức năng Hủy đơn để hệ thống tự động rút Waitlist.");

        _db.Initialize();
        var order = await _db.BookingsOrders.Find(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
        if (order == null)
            return ServiceResult.Fail($"Không tìm thấy đơn {orderCode}.");

        if (!AllowedTransitions.TryGetValue(order.Status, out var validNext) || !validNext.Contains(newStatus))
        {
            return ServiceResult.Fail(
                $"Không thể chuyển trạng thái đơn {orderCode} từ '{order.Status}' sang '{newStatus}'.");
        }

        var update = Builders<BookingOrderModel>.Update.Set(o => o.Status, newStatus);
        await _db.BookingsOrders.UpdateOneAsync(o => o.OrderCode == orderCode, update);

        return ServiceResult.Ok($"Đã cập nhật đơn {orderCode} sang trạng thái '{newStatus}'.");
    }

    /// <summary>
    /// Hủy đơn trên MongoDB và tự động RPOP khách chờ đầu tiên từ Waitlist trên Memurai để nhường bàn.
    /// </summary>
    public async Task<ServiceResult<WaitlistEntryDto?>> CancelOrderAndPopWaitlistAsync(string orderCode)
    {
        _db.Initialize();
        var order = await _db.BookingsOrders.Find(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
        if (order == null)
            return ServiceResult<WaitlistEntryDto?>.Fail($"Không tìm thấy đơn {orderCode}.");

        if (order.Status is "completed" or "cancelled")
            return ServiceResult<WaitlistEntryDto?>.Fail($"Đơn {orderCode} đang ở trạng thái '{order.Status}', không thể hủy.");

        var update = Builders<BookingOrderModel>.Update.Set(o => o.Status, "cancelled");
        await _db.BookingsOrders.UpdateOneAsync(o => o.OrderCode == orderCode, update);

        WaitlistEntryDto? nextWaitingCustomer = null;
        if (order.OrderType == "dine_in" && !string.IsNullOrWhiteSpace(order.TableId))
        {
            await _db.RedisDb.KeyDeleteAsync(
                RedisKeys.LockTable(order.RestaurantCode, order.TableId, order.BookingDate, order.TimeSlot));

            var poppedRaw = await _db.RedisDb.ListRightPopAsync(
                RedisKeys.Waitlist(order.RestaurantCode, order.BookingDate, order.TimeSlot));

            nextWaitingCustomer = DeserializeWaitlist(poppedRaw);
        }

        var msg = nextWaitingCustomer != null
            ? $"Đã hủy đơn {orderCode}. Khách Waitlist tiếp theo (RPOP): {nextWaitingCustomer.FullName} — SĐT: {nextWaitingCustomer.Phone} ({nextWaitingCustomer.GuestCount} khách)."
            : $"Đã hủy đơn {orderCode}. Hiện chưa có khách nào trong hàng đợi Waitlist cho khung giờ {order.TimeSlot}.";

        return ServiceResult<WaitlistEntryDto?>.Ok(msg, nextWaitingCustomer);
    }

    private async Task<bool> HasActiveMongoBookingAsync(
        string restaurantCode,
        string tableId,
        string bookingDate,
        string timeSlot)
    {
        var count = await _db.BookingsOrders.CountDocumentsAsync(o =>
            o.RestaurantCode == restaurantCode &&
            o.TableId == tableId &&
            o.BookingDate == bookingDate &&
            o.TimeSlot == timeSlot &&
            o.OrderType == "dine_in" &&
            o.Status != "cancelled");

        return count > 0;
    }

    private static WaitlistEntryDto? DeserializeWaitlist(RedisValue raw)
    {
        if (raw.IsNullOrEmpty) return null;
        try
        {
            return JsonSerializer.Deserialize<WaitlistEntryDto>(raw.ToString(), JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static (string? VoucherCode, double Discount, double Total) CalculatePricing(
        List<CartItemDto> items,
        string? rawVoucher)
    {
        double subtotal = items.Sum(i => i.Subtotal);
        if (subtotal <= 0) return (null, 0, 0);

        var code = rawVoucher?.Trim().ToUpperInvariant();
        if (code == "HUIT10" && subtotal >= 100_000)
        {
            double discount = Math.Round(subtotal * 0.1, 0);
            return (code, discount, Math.Max(0, subtotal - discount));
        }

        return (null, 0, subtotal);
    }

    private static string GenerateOrderCode(string bookingDate)
    {
        var datePart = bookingDate.Replace("-", string.Empty);
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
        return $"ORD{datePart}{DateTime.Now:HHmmss}{uniqueSuffix}";
    }
}
