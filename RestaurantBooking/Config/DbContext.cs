using MongoDB.Bson;
using MongoDB.Driver;
using StackExchange.Redis;

namespace RestaurantBooking.Config;

/// <summary>
/// Singleton quản lý kết nối đồng thời tới MongoDB (tài liệu) và Memurai/Redis (in-memory).
/// File dùng chung cố định từ Giai đoạn 0.
/// </summary>
public sealed class DbContext
{
    private static readonly Lazy<DbContext> _instance = new(() => new DbContext());

    public static DbContext Instance => _instance.Value;

    public const string MongoConnectionString = "mongodb://127.0.0.1:27017";
    public const string MongoDatabaseName = "db_booking_nhahang";
    public const string RedisConnectionString = "127.0.0.1:6379,abortConnect=false";

    public IMongoClient MongoClient { get; private set; } = null!;
    public IMongoDatabase MongoDb { get; private set; } = null!;
    public ConnectionMultiplexer RedisMux { get; private set; } = null!;
    public IDatabase RedisDb => RedisMux.GetDatabase();

    public bool IsInitialized { get; private set; }

    private DbContext() { }

    /// <summary>
    /// Khởi tạo kết nối MongoDB và Memurai một lần khi khởi động ứng dụng.
    /// </summary>
    public void Initialize()
    {
        if (IsInitialized) return;

        MongoClient = new MongoClient(MongoConnectionString);
        MongoDb = MongoClient.GetDatabase(MongoDatabaseName);
        RedisMux = ConnectionMultiplexer.Connect(RedisConnectionString);
        IsInitialized = true;
    }

    public IMongoCollection<Models.UserModel> Users =>
        MongoDb.GetCollection<Models.UserModel>("users");

    public IMongoCollection<Models.RestaurantModel> Restaurants =>
        MongoDb.GetCollection<Models.RestaurantModel>("restaurants");

    public IMongoCollection<Models.MenuItemModel> MenuItems =>
        MongoDb.GetCollection<Models.MenuItemModel>("menu_items");

    public IMongoCollection<Models.BookingOrderModel> BookingsOrders =>
        MongoDb.GetCollection<Models.BookingOrderModel>("bookings_orders");

    public IMongoCollection<Models.ReviewModel> Reviews =>
        MongoDb.GetCollection<Models.ReviewModel>("reviews");

    public IServer? GetRedisServer()
    {
        var endpoints = RedisMux.GetEndPoints();
        return endpoints.Length > 0 ? RedisMux.GetServer(endpoints[0]) : null;
    }

    /// <summary>
    /// Kiểm tra nhanh trạng thái kết nối tới cả MongoDB (27017) và Memurai (6379).
    /// </summary>
    public bool TestConnections(out string message)
    {
        try
        {
            Initialize();
            MongoDb.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            var latency = RedisDb.Ping();
            message = $"MongoDB ({MongoDatabaseName}) & Memurai (ping {latency.TotalMilliseconds:0.#} ms): Kết nối ổn định.";
            return true;
        }
        catch (Exception ex)
        {
            message = $"Lỗi kết nối CSDL: {ex.Message}\nKiểm tra dịch vụ MongoDB Server (27017) và Memurai (6379) trong services.msc.";
            return false;
        }
    }
}

/// <summary>
/// Chuẩn hóa toàn bộ 8 mẫu Key trên Memurai theo quy ước chung của nhóm.
/// </summary>
public static class RedisKeys
{
    public const string RankingMenuItems = "ranking:menu_items";
    public const string RankingRestaurants = "ranking:restaurants";

    public static string LockTable(string restaurantCode, string tableId, string bookingDate, string timeSlot) =>
        $"lock:table:{restaurantCode}:{tableId}:{bookingDate}:{timeSlot}";

    public static string Waitlist(string restaurantCode, string bookingDate, string timeSlot) =>
        $"waitlist:{restaurantCode}:{bookingDate}:{timeSlot}";

    public static string CacheMenu(string restaurantCode) =>
        $"cache:menu:{restaurantCode}";

    public static string RestaurantViews(string restaurantCode) =>
        $"views:restaurant:{restaurantCode}";

    public static string Session(string token) =>
        $"session:{token}";

    public static string Cart(string userCode) =>
        $"cart:{userCode}";
}
