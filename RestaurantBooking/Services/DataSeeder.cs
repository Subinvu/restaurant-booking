using System.Text.Encodings.Web;
using System.Text.Json;
using MongoDB.Driver;
using RestaurantBooking.Config;
using RestaurantBooking.Models;
using StackExchange.Redis;

namespace RestaurantBooking.Services;

/// <summary>
/// [TV1 - Giai đoạn 0] Sinh dữ liệu mẫu chuẩn cho 5 collections MongoDB
/// (3 nhà hàng, 36 bàn, 30 món, 20 users, 500 đơn đặt, 100 đánh giá) và các key mẫu Memurai.
/// </summary>
public static class DataSeeder
{
    private static readonly JsonSerializerOptions ReadableJsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private static readonly string[] TimeSlots = ["11:00", "13:00", "17:30", "19:00", "20:30"];
    private static readonly string[] PaymentMethods = ["cash", "momo", "banking"];
    private static readonly string[] CompletedComments =
    [
        "Món ăn lên rất nhanh, hương vị đậm đà, phục vụ nhiệt tình.",
        "Không gian sạch sẽ thoáng mát, đặt bàn trước không phải chờ.",
        "Lẩu rất ngon, nước dùng vừa miệng, sẽ quay lại cùng gia đình.",
        "Nhân viên chu đáo, món nướng thơm ngon, giá cả hợp lý.",
        "Đặt bàn cuối tuần khá đông nhưng vẫn được xếp chỗ đúng giờ.",
        "Đồ uống pha chế ngon, tráng miệng mát lạnh.",
        "Món khai vị giòn ngon, thích hợp đi nhóm bạn đông người."
    ];

    /// <summary>
    /// Nạp dữ liệu nếu DB chưa có dữ liệu, hoặc xóa nạp lại hoàn toàn khi forceReset = true.
    /// </summary>
    public static async Task<string> SeedAsync(bool forceReset = false)
    {
        var db = DbContext.Instance;
        db.Initialize();

        var existingUsers = await db.Users.CountDocumentsAsync(FilterDefinition<UserModel>.Empty);
        var existingOrders = await db.BookingsOrders.CountDocumentsAsync(FilterDefinition<BookingOrderModel>.Empty);
        if (!forceReset && existingUsers > 0 && existingOrders >= 500)
        {
            await EnsureSampleCartAndDemoKeysAsync(db);
            return $"CSDL đã có sẵn dữ liệu ({existingUsers} users, {existingOrders} đơn hàng).";
        }

        await ClearCollectionsAsync(db);

        var users = BuildUsers();
        var restaurants = BuildRestaurants();
        var menuItems = BuildMenuItems(restaurants);
        var orders = BuildOrders(users, restaurants, menuItems, totalOrders: 500);
        var reviews = BuildReviews(orders, users, totalReviews: 100);

        // Tính lại avg_rating thực tế từ tập review vừa sinh
        RecalculateMenuRatings(menuItems, reviews);

        await db.Users.InsertManyAsync(users);
        await db.Restaurants.InsertManyAsync(restaurants);
        await db.MenuItems.InsertManyAsync(menuItems);
        await db.BookingsOrders.InsertManyAsync(orders);
        await db.Reviews.InsertManyAsync(reviews);

        await SeedRedisKeysAsync(db, orders, menuItems, restaurants, users);

        return $"Đã nạp thành công: {restaurants.Count} nhà hàng (36 bàn), {menuItems.Count} món ăn, {users.Count} users, {orders.Count} đơn hàng, {reviews.Count} đánh giá + Memurai keys.";
    }

    private static async Task ClearCollectionsAsync(DbContext db)
    {
        await db.Users.DeleteManyAsync(FilterDefinition<UserModel>.Empty);
        await db.Restaurants.DeleteManyAsync(FilterDefinition<RestaurantModel>.Empty);
        await db.MenuItems.DeleteManyAsync(FilterDefinition<MenuItemModel>.Empty);
        await db.BookingsOrders.DeleteManyAsync(FilterDefinition<BookingOrderModel>.Empty);
        await db.Reviews.DeleteManyAsync(FilterDefinition<ReviewModel>.Empty);
    }

    private static List<UserModel> BuildUsers()
    {
        var sharedHash = BCrypt.Net.BCrypt.HashPassword("123456", workFactor: 10);
        var now = DateTime.UtcNow;

        var users = new List<UserModel>
        {
            CreateUser("ADM001", "Lê Hoàng Minh", "admin@nhahang.vn", "0901234500", "admin", sharedHash, "140 Lê Trọng Tấn", "Tân Phú", now.AddDays(-90)),
            CreateUser("STF001", "Trần Thị Bình", "binh.tran@nhahang.vn", "0901234502", "staff", sharedHash, "88 Nguyễn Huệ", "Quận 1", now.AddDays(-85)),
            CreateUser("STF002", "Phạm Quốc Cường", "cuong.pham@nhahang.vn", "0901234503", "staff", sharedHash, "215 Võ Văn Tần", "Quận 3", now.AddDays(-80))
        };

        string[] customerNames =
        [
            "Nguyễn Văn An", "Trần Ngọc Bích", "Lê Minh Châu", "Phạm Gia Dũng",
            "Hoàng Thu Hà", "Vũ Đức Hải", "Đặng Khánh Linh", "Bùi Tấn Lộc",
            "Đỗ Thanh Mai", "Hồ Quang Nam", "Ngô Phương Nghi", "Dương Tuấn Phát",
            "Lý Kim Quyên", "Võ Thành Tài", "Phan Bảo Trâm", "Trương Minh Tuấn",
            "Huỳnh Thảo Vy"
        ];
        string[] districts = ["Tân Phú", "Quận 1", "Quận 3", "Tân Bình", "Bình Thạnh", "Phú Nhuận"];

        for (int i = 0; i < customerNames.Length; i++)
        {
            var idx = i + 1;
            var code = $"USR{idx:000}";
            var emailPrefix = idx == 1 ? "an.nguyen" : $"khach{idx:000}";
            var phone = idx == 1 ? "0901234501" : $"0908{idx:000000}";
            users.Add(CreateUser(
                code,
                customerNames[i],
                $"{emailPrefix}@huit.edu.vn",
                phone,
                "customer",
                sharedHash,
                $"{15 + idx * 7} Đường số {(idx % 9) + 1}",
                districts[i % districts.Length],
                now.AddDays(-75 + i)));
        }

        return users;
    }

    private static UserModel CreateUser(
        string code, string name, string email, string phone,
        string role, string hash, string street, string district, DateTime createdAt) =>
        new()
        {
            UserCode = code,
            FullName = name,
            Email = email,
            Phone = phone,
            PasswordHash = hash,
            Role = role,
            IsActive = true,
            CreatedAt = createdAt,
            Addresses =
            [
                new AddressItem
                {
                    Label = "Nhà riêng",
                    Street = street,
                    District = district,
                    City = "TP.HCM",
                    IsDefault = true
                }
            ]
        };

    private static List<RestaurantModel> BuildRestaurants()
    {
        var definitions = new (string Code, string Name, string District, string Address, string Phone)[]
        {
            ("RES01", "Hương Việt - Tân Phú", "Tân Phú", "140 Lê Trọng Tấn, P. Tây Thạnh, Q. Tân Phú", "02838123401"),
            ("RES02", "Hương Việt - Quận 1", "Quận 1", "88 Nguyễn Huệ, P. Bến Nghé, Q. 1", "02838123402"),
            ("RES03", "Hương Việt - Quận 3", "Quận 3", "215 Võ Văn Tần, P. 5, Q. 3", "02838123403")
        };

        var list = new List<RestaurantModel>();
        foreach (var def in definitions)
        {
            var tables = new List<TableItem>();
            for (int t = 1; t <= 12; t++)
            {
                string area = t <= 6 ? "indoor" : t <= 9 ? "outdoor" : "vip";
                int capacity = area switch
                {
                    "vip" => t == 12 ? 10 : 8,
                    "outdoor" => 4,
                    _ => t <= 2 ? 2 : 4
                };

                tables.Add(new TableItem
                {
                    TableId = $"T{t:00}",
                    TableName = $"Bàn {t:00}",
                    Area = area,
                    Capacity = capacity,
                    IsAvailable = true
                });
            }

            list.Add(new RestaurantModel
            {
                RestaurantCode = def.Code,
                Name = def.Name,
                District = def.District,
                Address = def.Address,
                Phone = def.Phone,
                OpenTime = "09:00",
                CloseTime = "22:30",
                IsActive = true,
                Tables = tables
            });
        }

        return list;
    }

    private static List<MenuItemModel> BuildMenuItems(List<RestaurantModel> restaurants)
    {
        var templates = new (string Name, string Category, double BasePrice)[]
        {
            ("Gỏi cuốn tôm thịt", "Khai vị", 55000),
            ("Chả giò hải sản giòn", "Khai vị", 68000),
            ("Cơm chiên hải sản", "Món chính", 115000),
            ("Bò lúc lắc khoai tây", "Món chính", 165000),
            ("Cá lóc nướng trui cuốn bánh tráng", "Món chính", 185000),
            ("Lẩu Thái Tomyum hải sản", "Lẩu & Nướng", 295000),
            ("Sườn non nướng mật ong", "Lẩu & Nướng", 225000),
            ("Lẩu riêu cua bắp bò", "Lẩu & Nướng", 315000),
            ("Trà đào cam sả", "Đồ uống", 42000),
            ("Chè khúc bạch hạnh nhân", "Tráng miệng", 48000)
        };

        var items = new List<MenuItemModel>();
        int counter = 1;

        foreach (var res in restaurants)
        {
            for (int j = 0; j < templates.Length; j++)
            {
                var tpl = templates[j];
                var priceAdjust = res.RestaurantCode == "RES02" ? 10000 : 0; // Q1 giá cao hơn nhẹ 10k
                var code = $"MN{counter:000}";
                counter++;

                items.Add(new MenuItemModel
                {
                    ItemCode = code,
                    RestaurantCode = res.RestaurantCode,
                    Name = tpl.Name,
                    Category = tpl.Category,
                    BasePrice = tpl.BasePrice + priceAdjust,
                    ImageUrl = $"images/menu/{code.ToLowerInvariant()}.jpg",
                    IsAvailable = true,
                    AvgRating = 4.6,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    Sizes =
                    [
                        new SizeOption { SizeCode = "S", Label = "Phần vừa", ExtraPrice = 0 },
                        new SizeOption { SizeCode = "L", Label = "Phần lớn", ExtraPrice = tpl.Category == "Đồ uống" ? 12000 : 45000 }
                    ],
                    Toppings = BuildToppingsForCategory(tpl.Category)
                });
            }
        }

        return items;
    }

    private static List<ToppingOption> BuildToppingsForCategory(string category) =>
        category switch
        {
            "Lẩu & Nướng" =>
            [
                new ToppingOption { ToppingCode = "TOP01", Name = "Thêm nấm tổng hợp", Price = 35000 },
                new ToppingOption { ToppingCode = "TOP02", Name = "Thêm ba chỉ bò Mỹ", Price = 65000 }
            ],
            "Đồ uống" =>
            [
                new ToppingOption { ToppingCode = "TOP03", Name = "Trân châu trắng", Price = 10000 },
                new ToppingOption { ToppingCode = "TOP04", Name = "Kem cheese", Price = 15000 }
            ],
            _ =>
            [
                new ToppingOption { ToppingCode = "TOP05", Name = "Thêm nước chấm đặc biệt", Price = 10000 },
                new ToppingOption { ToppingCode = "TOP06", Name = "Thêm rau sống / bún", Price = 15000 }
            ]
        };

    private static List<BookingOrderModel> BuildOrders(
        List<UserModel> users,
        List<RestaurantModel> restaurants,
        List<MenuItemModel> menuItems,
        int totalOrders)
    {
        var rng = new Random(20261117);
        var customers = users.Where(u => u.Role == "customer").ToList();
        var orders = new List<BookingOrderModel>(totalOrders);
        var today = DateTime.Today;
        var usedTodaySlots = new HashSet<string>();

        for (int i = 1; i <= totalOrders; i++)
        {
            var res = restaurants[rng.Next(restaurants.Count)];
            var resMenu = menuItems.Where(m => m.RestaurantCode == res.RestaurantCode).ToList();
            var cust = customers[rng.Next(customers.Count)];

            // 15 đơn cuối là đơn hôm nay để tiện demo sơ đồ bàn & duyệt đơn ngay lập tức
            int dayOffset = i > totalOrders - 15 ? 0 : -rng.Next(1, 60);
            var bookingDt = today.AddDays(dayOffset);
            var bookingDate = bookingDt.ToString("yyyy-MM-dd");
            var timeSlot = dayOffset == 0 && i > totalOrders - 6
                ? "19:00"
                : TimeSlots[rng.Next(TimeSlots.Length)];

            bool isDineIn = rng.NextDouble() < 0.78;
            string? tableId = null;

            if (isDineIn)
            {
                if (dayOffset == 0)
                {
                    // Chọn bàn trống cho slot hôm nay (tránh trùng T03 để chừa làm bàn đang giữ TTL demo)
                    for (int attempt = 0; attempt < 12; attempt++)
                    {
                        var candidate = $"T{rng.Next(1, 13):00}";
                        if (res.RestaurantCode == "RES01" && candidate == "T03" && timeSlot == "19:00")
                            continue;
                        var slotKey = $"{res.RestaurantCode}:{candidate}:{bookingDate}:{timeSlot}";
                        if (usedTodaySlots.Add(slotKey))
                        {
                            tableId = candidate;
                            break;
                        }
                    }

                    if (tableId == null) isDineIn = false;
                }
                else
                {
                    tableId = res.Tables[rng.Next(res.Tables.Count)].TableId;
                }
            }

            string status;
            if (dayOffset < 0)
            {
                status = rng.NextDouble() < 0.85 ? "completed" : "cancelled";
            }
            else
            {
                status = (i % 3) switch
                {
                    0 => "confirmed",
                    1 => "pending",
                    _ => "serving"
                };
            }

            int itemCount = rng.Next(1, 4);
            var orderItems = new List<OrderItem>();
            double subtotalSum = 0;

            for (int k = 0; k < itemCount; k++)
            {
                var menu = resMenu[rng.Next(resMenu.Count)];
                var size = menu.Sizes[rng.Next(menu.Sizes.Count)];
                var topping = rng.NextDouble() < 0.4 && menu.Toppings.Count > 0
                    ? menu.Toppings[rng.Next(menu.Toppings.Count)]
                    : null;

                int qty = rng.Next(1, 4);
                double unitPrice = menu.BasePrice + size.ExtraPrice + (topping?.Price ?? 0);
                double lineSubtotal = unitPrice * qty;
                subtotalSum += lineSubtotal;

                orderItems.Add(new OrderItem
                {
                    ItemCode = menu.ItemCode,
                    Name = menu.Name,
                    SizeCode = size.SizeCode,
                    Toppings = topping != null ? [topping.Name] : [],
                    Qty = qty,
                    UnitPrice = unitPrice,
                    Subtotal = lineSubtotal
                });
            }

            bool hasVoucher = subtotalSum >= 350000 && rng.NextDouble() < 0.3;
            string? voucher = hasVoucher ? "HUIT10" : null;
            double discount = hasVoucher ? Math.Round(subtotalSum * 0.1, 0) : 0;

            orders.Add(new BookingOrderModel
            {
                OrderCode = $"ORD{bookingDt:yyyyMMdd}{i:000}",
                OrderType = isDineIn ? "dine_in" : "takeaway",
                Customer = new CustomerSnapshot
                {
                    UserCode = cust.UserCode,
                    FullName = cust.FullName,
                    Phone = cust.Phone
                },
                RestaurantCode = res.RestaurantCode,
                TableId = tableId,
                BookingDate = bookingDate,
                TimeSlot = timeSlot,
                GuestCount = isDineIn ? rng.Next(2, 8) : 1,
                Items = orderItems,
                VoucherCode = voucher,
                DiscountAmount = discount,
                TotalAmount = subtotalSum - discount,
                PaymentMethod = PaymentMethods[rng.Next(PaymentMethods.Length)],
                Status = status,
                CreatedAt = bookingDt.AddHours(10 + rng.Next(0, 11)).ToUniversalTime()
            });
        }

        return orders;
    }

    private static List<ReviewModel> BuildReviews(List<BookingOrderModel> orders, List<UserModel> users, int totalReviews)
    {
        var rng = new Random(20261118);
        var completedOrders = orders.Where(o => o.Status == "completed").ToList();
        var staffNames = users.Where(u => u.Role == "staff" || u.Role == "admin").Select(u => u.FullName).ToList();
        var reviews = new List<ReviewModel>(totalReviews);

        for (int i = 0; i < totalReviews && completedOrders.Count > 0; i++)
        {
            var ord = completedOrders[i % completedOrders.Count];
            int rating = rng.NextDouble() < 0.12 ? rng.Next(2, 4) : rng.Next(4, 6);
            var likedItems = ord.Items.Select(x => x.ItemCode).Distinct().ToList();

            ReplyInfo? reply = rng.NextDouble() < 0.55
                ? new ReplyInfo
                {
                    StaffName = staffNames[rng.Next(staffNames.Count)],
                    Content = "Cảm ơn quý khách đã ủng hộ nhà hàng Hương Việt!",
                    RepliedAt = ord.CreatedAt.AddHours(4)
                }
                : null;

            reviews.Add(new ReviewModel
            {
                ReviewCode = $"REV{i + 1:000}",
                OrderCode = ord.OrderCode,
                UserCode = ord.Customer.UserCode,
                FullName = ord.Customer.FullName,
                RestaurantCode = ord.RestaurantCode,
                Rating = rating,
                Comment = CompletedComments[rng.Next(CompletedComments.Length)],
                LikedItems = likedItems,
                Reply = reply,
                CreatedAt = ord.CreatedAt.AddHours(2)
            });
        }

        return reviews;
    }

    private static void RecalculateMenuRatings(List<MenuItemModel> menuItems, List<ReviewModel> reviews)
    {
        foreach (var item in menuItems)
        {
            var matched = reviews.Where(r => r.LikedItems.Contains(item.ItemCode)).ToList();
            if (matched.Count > 0)
            {
                item.AvgRating = Math.Round(matched.Average(r => r.Rating), 2);
            }
        }
    }

    private static async Task SeedRedisKeysAsync(
        DbContext db,
        List<BookingOrderModel> orders,
        List<MenuItemModel> menuItems,
        List<RestaurantModel> restaurants,
        List<UserModel> users)
    {
        var redis = db.RedisDb;

        // Xóa sạch các key thuộc quy ước cũ (an toàn chỉ xóa prefix của đồ án)
        var server = db.GetRedisServer();
        if (server != null)
        {
            string[] prefixes = ["lock:table:*", "waitlist:*", "cache:menu:*", "views:restaurant:*", "cart:*", "ranking:*"];
            foreach (var pattern in prefixes)
            {
                foreach (var k in server.Keys(pattern: pattern))
                {
                    await redis.KeyDeleteAsync(k);
                }
            }
        }

        // 1. Nạp bảng xếp hạng món ăn & chi nhánh từ 500 đơn hàng (Sorted Set)
        var itemScores = orders
            .Where(o => o.Status != "cancelled")
            .SelectMany(o => o.Items)
            .GroupBy(it => it.ItemCode)
            .Select(g => new SortedSetEntry(g.Key, g.Sum(x => x.Qty)))
            .ToArray();

        if (itemScores.Length > 0)
            await redis.SortedSetAddAsync(RedisKeys.RankingMenuItems, itemScores);

        var resScores = orders
            .Where(o => o.Status != "cancelled")
            .GroupBy(o => o.RestaurantCode)
            .Select(g => new SortedSetEntry(g.Key, g.Count()))
            .ToArray();

        if (resScores.Length > 0)
            await redis.SortedSetAddAsync(RedisKeys.RankingRestaurants, resScores);

        // 2. Nạp bộ đếm lượt xem chi nhánh (String INCR)
        int[] initViews = [158, 124, 96];
        for (int i = 0; i < restaurants.Count; i++)
        {
            await redis.StringSetAsync(RedisKeys.RestaurantViews(restaurants[i].RestaurantCode), initViews[i % initViews.Length]);
        }

        await EnsureSampleCartAndDemoKeysAsync(db, menuItems, users);
    }

    private static async Task EnsureSampleCartAndDemoKeysAsync(
        DbContext db,
        List<MenuItemModel>? menuItems = null,
        List<UserModel>? users = null)
    {
        var redis = db.RedisDb;
        var todayStr = DateTime.Today.ToString("yyyy-MM-dd");

        // 3. Nạp sẵn giỏ hàng mẫu cho USR001 để TV1 test độc lập chốt đơn kèm món ăn
        var cartKey = RedisKeys.Cart("USR001");
        if (await redis.HashLengthAsync(cartKey) == 0)
        {
            menuItems ??= await db.MenuItems.Find(m => m.RestaurantCode == "RES01").Limit(2).ToListAsync();
            var entries = new List<HashEntry>();
            foreach (var item in menuItems.Take(2))
            {
                var size = item.Sizes.FirstOrDefault()?.SizeCode ?? "S";
                var payload = JsonSerializer.Serialize(new
                {
                    item_code = item.ItemCode,
                    name = item.Name,
                    size_code = size,
                    toppings = Array.Empty<string>(),
                    qty = 2,
                    unit_price = item.BasePrice
                }, ReadableJsonOptions);
                entries.Add(new HashEntry($"{item.ItemCode}_{size}", payload));
            }

            if (entries.Count > 0)
                await redis.HashSetAsync(cartKey, entries.ToArray());
        }

        // 4. Nạp 1 lock giữ bàn TTL 15 phút demo trên RES01 bàn T03 hôm nay khung 19:00
        var demoLockKey = RedisKeys.LockTable("RES01", "T03", todayStr, "19:00");
        await redis.StringSetAsync(demoLockKey, "USR002", TimeSpan.FromSeconds(900), When.NotExists);

        // 5. Nạp 1 khách trong hàng đợi Waitlist mẫu để TV1 test & Redis Insight hiển thị đủ kiểu dữ liệu
        var demoWaitlistKey = RedisKeys.Waitlist("RES01", todayStr, "19:00");
        if (await redis.ListLengthAsync(demoWaitlistKey) == 0)
        {
            users ??= await db.Users.Find(u => u.UserCode == "USR003").ToListAsync();
            var sampleUser = users.FirstOrDefault(u => u.UserCode == "USR003");
            var waitPayload = JsonSerializer.Serialize(new
            {
                user_code = sampleUser?.UserCode ?? "USR003",
                full_name = sampleUser?.FullName ?? "Lê Minh Châu",
                phone = sampleUser?.Phone ?? "0908000003",
                guest_count = 4,
                requested_at = DateTime.UtcNow.ToString("O")
            }, ReadableJsonOptions);
            await redis.ListLeftPushAsync(demoWaitlistKey, waitPayload);
        }
    }
}
