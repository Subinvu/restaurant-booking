using System.Globalization;
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

    [JsonPropertyName("restaurant_code")]
    public string RestaurantCode { get; set; } = string.Empty;

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

        var restaurant = await _db.Restaurants.Find(r => r.RestaurantCode == restaurantCode && r.IsActive).FirstOrDefaultAsync();
        if (restaurant == null)
            return ServiceResult<int>.Fail($"Chi nhánh {restaurantCode} không tồn tại hoặc đã ngưng hoạt động.");

        var slotCheck = ValidateSlotDateTimeAndHours(restaurant, bookingDate, timeSlot);
        if (!slotCheck.Success)
            return ServiceResult<int>.Fail(slotCheck.Message);

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

        return ServiceResult<int>.Fail(
            $"Bàn này đang được khách khác giữ tạm (còn {remaining / 60:00}:{remaining % 60:00}).");
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

        var restaurant = await _db.Restaurants.Find(r => r.RestaurantCode == restaurantCode && r.IsActive).FirstOrDefaultAsync();
        if (restaurant == null)
            return ServiceResult<long>.Fail($"Chi nhánh {restaurantCode} không tồn tại hoặc đã ngưng hoạt động.");

        var slotCheck = ValidateSlotDateTimeAndHours(restaurant, bookingDate, timeSlot);
        if (!slotCheck.Success)
            return ServiceResult<long>.Fail(slotCheck.Message);

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
        string customerPhone,
        string? deliveryAddress = null,
        string? note = null)
    {
        if (string.IsNullOrWhiteSpace(customerUserCode))
            return ServiceResult<BookingOrderModel>.Fail("Chưa có thông tin khách hàng.");

        _db.Initialize();

        var restaurant = await _db.Restaurants.Find(r => r.RestaurantCode == restaurantCode && r.IsActive).FirstOrDefaultAsync();
        if (restaurant == null)
            return ServiceResult<BookingOrderModel>.Fail($"Chi nhánh {restaurantCode} không tồn tại hoặc đã ngưng hoạt động.");

        var slotCheck = ValidateSlotDateTimeAndHours(restaurant, bookingDate, timeSlot);
        if (!slotCheck.Success)
            return ServiceResult<BookingOrderModel>.Fail(slotCheck.Message);

        var rawCartItems = await GetCartItemsAsync(customerUserCode);
        if (orderType == "takeaway" && rawCartItems.Count == 0)
            return ServiceResult<BookingOrderModel>.Fail("Đơn mang về cần có ít nhất 1 món ăn trong giỏ hàng.");

        // Đối chiếu lại từng món trong giỏ với MongoDB menu_items: đúng chi nhánh, còn mở bán và dùng giá mới nhất.
        var verifiedCart = await VerifyAndRepriceCartAsync(rawCartItems, restaurantCode);
        if (!verifiedCart.Success)
            return ServiceResult<BookingOrderModel>.Fail(verifiedCart.Message);
        var cartItems = verifiedCart.Data!;

        string? acquiredLockKey = null;
        bool acquiredTempLockHere = false;

        if (orderType == "dine_in")
        {
            if (string.IsNullOrWhiteSpace(tableId))
                return ServiceResult<BookingOrderModel>.Fail("Vui lòng chọn một bàn trống hoặc bàn bạn đang giữ.");

            var table = restaurant.Tables.FirstOrDefault(t => t.TableId == tableId);
            if (table == null || !table.IsAvailable)
                return ServiceResult<BookingOrderModel>.Fail($"Bàn {tableId} không tồn tại hoặc đang ngưng phục vụ.");

            if (guestCount > table.Capacity)
                return ServiceResult<BookingOrderModel>.Fail($"Số khách ({guestCount}) vượt quá sức chứa của {table.TableName} ({table.Capacity} chỗ).");

            acquiredLockKey = RedisKeys.LockTable(restaurantCode, tableId, bookingDate, timeSlot);
            var existingHolder = await _db.RedisDb.StringGetAsync(acquiredLockKey);

            if (!existingHolder.IsNullOrEmpty && existingHolder != customerUserCode)
                return ServiceResult<BookingOrderModel>.Fail($"Bàn {table.TableName} đang được khách hàng khác giữ chỗ.");

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
                DeliveryAddress = string.IsNullOrWhiteSpace(deliveryAddress) ? null : deliveryAddress.Trim(),
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
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

    public async Task<ServiceResult> UpdateOrderStatusAsync(
        string orderCode,
        string newStatus,
        bool? actingIsStaffOrAdmin = null)
    {
        if (!(actingIsStaffOrAdmin ?? SessionContext.IsStaffOrAdmin))
            return ServiceResult.Fail("Chỉ Nhân viên hoặc Quản trị viên mới có quyền duyệt trạng thái đơn hàng.");

        if (string.Equals(newStatus, "cancelled", StringComparison.OrdinalIgnoreCase))
            return ServiceResult.Fail("Vui lòng dùng chức năng Hủy đơn để hệ thống tự động rút danh sách chờ.");

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
    /// Hủy đơn trên MongoDB (kiểm tra quyền sở hữu / vai trò) và tự động RPOP khách chờ đầu tiên từ Waitlist trên Memurai.
    /// </summary>
    public async Task<ServiceResult<WaitlistEntryDto?>> CancelOrderAndPopWaitlistAsync(
        string orderCode,
        string? actingUserCode = null,
        bool? actingIsStaffOrAdmin = null)
    {
        _db.Initialize();
        var order = await _db.BookingsOrders.Find(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
        if (order == null)
            return ServiceResult<WaitlistEntryDto?>.Fail($"Không tìm thấy đơn {orderCode}.");

        bool isStaff = actingIsStaffOrAdmin ?? SessionContext.IsStaffOrAdmin;
        var actorCode = actingUserCode ?? SessionContext.UserCode;

        if (!isStaff)
        {
            if (string.IsNullOrWhiteSpace(actorCode) ||
                !string.Equals(order.Customer.UserCode, actorCode, StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<WaitlistEntryDto?>.Fail("Bạn chỉ có thể hủy đơn hàng do chính tài khoản của mình đặt.");
            }

            if (TryParseSlotDateTime(order.BookingDate, order.TimeSlot, out var slotDt) && slotDt <= DateTime.Now)
            {
                return ServiceResult<WaitlistEntryDto?>.Fail(
                    $"Đã quá khung giờ {order.TimeSlot} ngày {order.BookingDate}, khách hàng không thể tự hủy. Vui lòng liên hệ nhân viên nhà hàng.");
            }
        }

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
            ? $"Đã hủy đơn {orderCode}. Khách chờ tiếp theo trong danh sách: {nextWaitingCustomer.FullName} — SĐT: {nextWaitingCustomer.Phone} ({nextWaitingCustomer.GuestCount} khách)."
            : $"Đã hủy đơn {orderCode}. Hiện chưa có khách nào trong danh sách chờ cho khung giờ {order.TimeSlot}.";

        return ServiceResult<WaitlistEntryDto?>.Ok(msg, nextWaitingCustomer);
    }

    /// <summary>
    /// Tự động hủy các đơn ở trạng thái "pending" đã quá giờ đặt quá 30 phút (no-show) để nhả bàn.
    /// </summary>
    public async Task<int> CancelExpiredPendingOrdersAsync()
    {
        _db.Initialize();
        var pendingOrders = await _db.BookingsOrders
            .Find(o => o.Status == "pending")
            .ToListAsync();

        var cutoff = DateTime.Now.AddMinutes(-30);
        var expiredCodes = new List<string>();
        foreach (var o in pendingOrders)
        {
            if (TryParseSlotDateTime(o.BookingDate, o.TimeSlot, out var slotDt) && slotDt < cutoff)
                expiredCodes.Add(o.OrderCode);
        }

        if (expiredCodes.Count == 0) return 0;

        var res = await _db.BookingsOrders.UpdateManyAsync(
            Builders<BookingOrderModel>.Filter.In(o => o.OrderCode, expiredCodes),
            Builders<BookingOrderModel>.Update.Set(o => o.Status, "cancelled"));

        return (int)res.ModifiedCount;
    }

    public static ServiceResult ValidateSlotDateTimeAndHours(
        RestaurantModel restaurant,
        string bookingDate,
        string timeSlot)
    {
        if (!TryParseSlotDateTime(bookingDate, timeSlot, out var slotDt))
            return ServiceResult.Fail("Ngày đặt hoặc khung giờ không đúng định dạng hợp lệ.");

        if (slotDt.Date < DateTime.Today)
            return ServiceResult.Fail("Không thể đặt bàn cho ngày trong quá khứ.");

        if (slotDt <= DateTime.Now)
            return ServiceResult.Fail($"Khung giờ {timeSlot} ngày {bookingDate} đã qua. Vui lòng chọn khung giờ tiếp theo hoặc ngày khác.");

        if (TimeOnly.TryParseExact(timeSlot, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var chosenTime) &&
            TimeOnly.TryParseExact(restaurant.OpenTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var openTime) &&
            TimeOnly.TryParseExact(restaurant.CloseTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var closeTime))
        {
            if (chosenTime < openTime || chosenTime >= closeTime)
            {
                return ServiceResult.Fail(
                    $"Khung giờ {timeSlot} nằm ngoài giờ mở cửa ({restaurant.OpenTime} – {restaurant.CloseTime}) của {restaurant.Name}.");
            }
        }

        return ServiceResult.Ok("Hợp lệ");
    }

    private async Task<ServiceResult<List<CartItemDto>>> VerifyAndRepriceCartAsync(
        List<CartItemDto> rawCart,
        string targetRestaurantCode)
    {
        if (rawCart.Count == 0)
            return ServiceResult<List<CartItemDto>>.Ok("Giỏ trống", []);

        var codes = rawCart.Select(c => c.ItemCode).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var liveDocs = await _db.MenuItems
            .Find(Builders<MenuItemModel>.Filter.In(m => m.ItemCode, codes))
            .ToListAsync();
        var liveMap = liveDocs.ToDictionary(m => m.ItemCode, StringComparer.OrdinalIgnoreCase);

        var repriced = new List<CartItemDto>(rawCart.Count);
        foreach (var cartItem in rawCart)
        {
            if (!liveMap.TryGetValue(cartItem.ItemCode, out var liveMenu))
                return ServiceResult<List<CartItemDto>>.Fail($"Món '{cartItem.Name}' ({cartItem.ItemCode}) không còn tồn tại trong thực đơn.");

            if (!liveMenu.IsAvailable)
                return ServiceResult<List<CartItemDto>>.Fail($"Món '{liveMenu.Name}' hiện đã tạm ngưng phục vụ. Vui lòng xóa khỏi giỏ hàng trước khi đặt.");

            if (!string.Equals(liveMenu.RestaurantCode, targetRestaurantCode, StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<List<CartItemDto>>.Fail(
                    $"Món '{liveMenu.Name}' trong giỏ thuộc chi nhánh {liveMenu.RestaurantCode}, không khớp với chi nhánh {targetRestaurantCode} bạn đang đặt. Vui lòng đổi đúng chi nhánh hoặc cập nhật lại giỏ hàng.");
            }

            var sizeObj = liveMenu.Sizes.FirstOrDefault(s =>
                string.Equals(s.SizeCode, cartItem.SizeCode, StringComparison.OrdinalIgnoreCase));
            double sizeExtra = sizeObj?.ExtraPrice ?? 0;

            double toppingExtra = 0;
            foreach (var topName in cartItem.Toppings)
            {
                var matchedTop = liveMenu.Toppings.FirstOrDefault(t =>
                    string.Equals(t.Name.Trim(), topName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (matchedTop != null)
                    toppingExtra += matchedTop.Price;
            }

            repriced.Add(new CartItemDto
            {
                HashField = cartItem.HashField,
                ItemCode = liveMenu.ItemCode,
                RestaurantCode = liveMenu.RestaurantCode,
                Name = liveMenu.Name,
                SizeCode = cartItem.SizeCode,
                Toppings = cartItem.Toppings,
                Qty = cartItem.Qty,
                UnitPrice = liveMenu.BasePrice + sizeExtra + toppingExtra
            });
        }

        return ServiceResult<List<CartItemDto>>.Ok("Hợp lệ", repriced);
    }

    private static bool TryParseSlotDateTime(string bookingDate, string timeSlot, out DateTime slotDateTime)
    {
        slotDateTime = default;
        if (!DateOnly.TryParseExact(bookingDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return false;
        if (!TimeOnly.TryParseExact(timeSlot, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            return false;

        slotDateTime = date.ToDateTime(time);
        return true;
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
