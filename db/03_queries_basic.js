// [TV3 - Giai đoạn 1] 12 câu truy vấn cơ bản chạy trực tiếp trên MongoDB Compass (MONGOSH)
// Bao phủ đầy đủ: find, $in, $gt/$lte, projection, $regex, $elemMatch, sort/skip/limit,
// countDocuments/distinct, dot notation + $or, insertOne, $set, $push, $addToSet, $pull, deleteOne.
use("db_booking_nhahang");

// ---------------------------------------------------------------------
// CÂU 1: Tìm món ăn đang mở bán thuộc nhóm "Lẩu & Nướng" hoặc "Món chính" ($in)
// ---------------------------------------------------------------------
print("\n=== Q1. LOC MON AN THEO NHOM ($in) ===");
printjson(
  db.menu_items
    .find({
      restaurant_code: "RES01",
      is_available: true,
      category: { $in: ["Lẩu & Nướng", "Món chính"] }
    })
    .toArray()
);

// ---------------------------------------------------------------------
// CÂU 2: Lọc món theo khoảng giá ($gt, $lte) kết hợp Projection (ẩn _id)
// ---------------------------------------------------------------------
print("\n=== Q2. LOC KHOANG GIA ($gt / $lte) + PROJECTION ===");
printjson(
  db.menu_items
    .find(
      { base_price: { $gt: 100000, $lte: 250000 } },
      { _id: 0, item_code: 1, restaurant_code: 1, name: 1, category: 1, base_price: 1 }
    )
    .sort({ base_price: 1 })
    .toArray()
);

// ---------------------------------------------------------------------
// CÂU 3: Tìm kiếm gần đúng món ăn theo từ khóa "Lẩu" không phân biệt hoa/thường ($regex)
// ---------------------------------------------------------------------
print("\n=== Q3. TIM KIEM MON AN BANG REGEX ($regex, $options: 'i') ===");
printjson(
  db.menu_items
    .find(
      { name: { $regex: "lẩu", $options: "i" } },
      { _id: 0, item_code: 1, restaurant_code: 1, name: 1, base_price: 1 }
    )
    .toArray()
);

// ---------------------------------------------------------------------
// CÂU 4: Tìm chi nhánh có bàn VIP sức chứa >= 8 chỗ đang sẵn sàng ($elemMatch)
// ---------------------------------------------------------------------
print("\n=== Q4. TIM NHA HANG CO BAN VIP >= 8 CHO ($elemMatch) ===");
printjson(
  db.restaurants
    .find(
      {
        is_active: true,
        tables: {
          $elemMatch: { area: "vip", capacity: { $gte: 8 }, is_available: true }
        }
      },
      { _id: 0, restaurant_code: 1, name: 1, district: 1, "tables.$": 1 }
    )
    .toArray()
);

// ---------------------------------------------------------------------
// CÂU 5: Phân trang danh sách đơn hàng mới nhất (sort + skip + limit — Trang 2, 5 đơn/trang)
// ---------------------------------------------------------------------
print("\n=== Q5. PHAN TRANG DON HANG MOI NHAT (sort / skip / limit) ===");
printjson(
  db.bookings_orders
    .find(
      { status: { $ne: "cancelled" } },
      { _id: 0, order_code: 1, restaurant_code: 1, booking_date: 1, time_slot: 1, total_amount: 1, status: 1 }
    )
    .sort({ created_at: -1 })
    .skip(5)
    .limit(5)
    .toArray()
);

// ---------------------------------------------------------------------
// CÂU 6: Đếm số đơn hoàn tất & liệt kê các nhóm món / phương thức thanh toán phân biệt (countDocuments & distinct)
// ---------------------------------------------------------------------
print("\n=== Q6. DEM SO DON & LIET KE GIA TRI PHAN BIET (countDocuments / distinct) ===");
printjson({
  completed_order_count: db.bookings_orders.countDocuments({ status: "completed" }),
  distinct_categories: db.menu_items.distinct("category"),
  distinct_payment_methods: db.bookings_orders.distinct("payment_method")
});

// ---------------------------------------------------------------------
// CÂU 7: Truy vấn trường nhúng (dot notation) kết hợp toán tử $or
// Tìm các đơn của khách USR001 thanh toán bằng momo hoặc banking
// ---------------------------------------------------------------------
print("\n=== Q7. LOC DON CUA KHACH USR001 THANH TOAN MOMO HOAC BANKING (dot notation + $or) ===");
printjson(
  db.bookings_orders
    .find(
      {
        "customer.user_code": "USR001",
        $or: [{ payment_method: "momo" }, { payment_method: "banking" }]
      },
      { _id: 0, order_code: 1, "customer.full_name": 1, booking_date: 1, payment_method: 1, total_amount: 1, status: 1 }
    )
    .sort({ created_at: -1 })
    .limit(10)
    .toArray()
);

// ---------------------------------------------------------------------
// CÂU 8: Thêm mới một khách hàng thử nghiệm bằng insertOne()
// ---------------------------------------------------------------------
print("\n=== Q8. THEM MOI NGUOI DUNG THU NGHIEM (insertOne) ===");
db.users.deleteOne({ user_code: "USR999" });
printjson(
  db.users.insertOne({
    user_code: "USR999",
    full_name: "Nguyễn Minh Thử Nghiệm",
    email: "test.tv3@huit.edu.vn",
    phone: "0909999999",
    password_hash: "$2a$10$gxBqrTdI17qGQ/F/fNdnt.onuZskoU4X/xTQXtgC7TyBst01BLXhe",
    role: "customer",
    addresses: [
      { label: "Nhà riêng", street: "140 Lê Trọng Tấn", district: "Tân Phú", city: "TP.HCM", is_default: true }
    ],
    is_active: true,
    created_at: new Date()
  })
);

// ---------------------------------------------------------------------
// CÂU 9: Cập nhật thông tin đơn trường bằng $set (khóa tạm thời hoặc đổi role user USR999)
// ---------------------------------------------------------------------
print("\n=== Q9. CAP NHAT VAI TRO & TRANG THAI USER ($set) ===");
printjson(
  db.users.updateOne(
    { user_code: "USR999" },
    { $set: { role: "staff", is_active: false } }
  )
);
printjson(db.users.findOne({ user_code: "USR999" }, { _id: 0, user_code: 1, full_name: 1, role: 1, is_active: 1 }));

// ---------------------------------------------------------------------
// CÂU 10: Thêm địa chỉ giao hàng mới vào mảng nhúng users.addresses bằng $push
// ---------------------------------------------------------------------
print("\n=== Q10. THEM DIA CHI MOI VAO MANG NHUNG ($push) ===");
printjson(
  db.users.updateOne(
    { user_code: "USR999" },
    {
      $push: {
        addresses: {
          label: "Công ty",
          street: "88 Nguyễn Huệ",
          district: "Quận 1",
          city: "TP.HCM",
          is_default: false
        }
      }
    }
  )
);
printjson(db.users.findOne({ user_code: "USR999" }, { _id: 0, user_code: 1, addresses: 1 }));

// ---------------------------------------------------------------------
// CÂU 11: Thêm món ăn yêu thích không trùng lặp vào mảng reviews.liked_items bằng $addToSet
// ---------------------------------------------------------------------
print("\n=== Q11. THEM MON YEU THICH KHONG TRUNG LAP ($addToSet) ===");
printjson(
  db.reviews.updateOne(
    { review_code: "REV001" },
    { $addToSet: { liked_items: "MN001" } }
  )
);
printjson(db.reviews.findOne({ review_code: "REV001" }, { _id: 0, review_code: 1, order_code: 1, liked_items: 1 }));

// ---------------------------------------------------------------------
// CÂU 12: Xóa phần tử trong mảng nhúng bằng $pull & dọn bản ghi thử nghiệm bằng deleteOne()
// ---------------------------------------------------------------------
print("\n=== Q12. XOA DIA CHI TRONG MANG ($pull) & DON BAN GHI TEST (deleteOne) ===");
printjson(
  db.users.updateOne(
    { user_code: "USR999" },
    { $pull: { addresses: { label: "Công ty" } } }
  )
);
printjson(db.users.findOne({ user_code: "USR999" }, { _id: 0, user_code: 1, addresses: 1 }));
printjson(db.users.deleteOne({ user_code: "USR999" }));
