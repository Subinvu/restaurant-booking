// [TV2 - Giai đoạn 1] 10 câu truy vấn Aggregation Pipeline nâng cao chạy trên MongoDB Compass (MONGOSH)
use("db_booking_nhahang");

// ---------------------------------------------------------------------
// CÂU 1: Doanh thu & giá trị trung bình đơn hoàn tất theo Tháng và Chi nhánh
// Toán tử: $match, $addFields ($substrCP), $group, $lookup, $unwind, $sort
// ---------------------------------------------------------------------
print("\n=== Q1. DOANH THU HOAN TAT THEO THANG & CHI NHANH ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: "completed" } },
    { $addFields: { year_month: { $substrCP: ["$booking_date", 0, 7] } } },
    {
      $group: {
        _id: { year_month: "$year_month", restaurant_code: "$restaurant_code" },
        order_count: { $sum: 1 },
        total_revenue: { $sum: "$total_amount" },
        avg_order_value: { $avg: "$total_amount" }
      }
    },
    {
      $lookup: {
        from: "restaurants",
        localField: "_id.restaurant_code",
        foreignField: "restaurant_code",
        as: "branch"
      }
    },
    { $unwind: "$branch" },
    {
      $project: {
        _id: 0,
        year_month: "$_id.year_month",
        restaurant_code: "$_id.restaurant_code",
        restaurant_name: "$branch.name",
        order_count: 1,
        total_revenue: { $round: ["$total_revenue", 0] },
        avg_order_value: { $round: ["$avg_order_value", 0] }
      }
    },
    { $sort: { year_month: -1, total_revenue: -1 } }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 2: Top 10 món ăn mang lại doanh thu cao nhất (tách mảng items)
// Toán tử: $match, $unwind, $group, $lookup, $sort, $limit
// ---------------------------------------------------------------------
print("\n=== Q2. TOP 10 MON AN DOANH THU CAO NHAT ($unwind) ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: { $ne: "cancelled" } } },
    { $unwind: "$items" },
    {
      $group: {
        _id: "$items.item_code",
        item_name: { $first: "$items.name" },
        total_qty: { $sum: "$items.qty" },
        total_revenue: { $sum: "$items.subtotal" },
        order_appearances: { $sum: 1 }
      }
    },
    {
      $lookup: {
        from: "menu_items",
        localField: "_id",
        foreignField: "item_code",
        as: "menu_doc"
      }
    },
    { $unwind: { path: "$menu_doc", preserveNullAndEmptyArrays: true } },
    {
      $project: {
        _id: 0,
        item_code: "$_id",
        item_name: 1,
        category: "$menu_doc.category",
        restaurant_code: "$menu_doc.restaurant_code",
        total_qty: 1,
        order_appearances: 1,
        total_revenue: { $round: ["$total_revenue", 0] }
      }
    },
    { $sort: { total_revenue: -1 } },
    { $limit: 10 }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 3: Phân tích khung giờ cao điểm (tách tỷ trọng dine_in vs takeaway)
// Toán tử: $match, $group, $cond, $project, $sort
// ---------------------------------------------------------------------
print("\n=== Q3. THONG KE KHUNG GIO CAO DIEM ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: { $ne: "cancelled" } } },
    {
      $group: {
        _id: "$time_slot",
        total_orders: { $sum: 1 },
        dine_in_orders: {
          $sum: { $cond: [{ $eq: ["$order_type", "dine_in"] }, 1, 0] }
        },
        takeaway_orders: {
          $sum: { $cond: [{ $eq: ["$order_type", "takeaway"] }, 1, 0] }
        },
        total_guests: { $sum: "$guest_count" },
        total_revenue: { $sum: "$total_amount" }
      }
    },
    {
      $project: {
        _id: 0,
        time_slot: "$_id",
        total_orders: 1,
        dine_in_orders: 1,
        takeaway_orders: 1,
        total_guests: 1,
        total_revenue: { $round: ["$total_revenue", 0] }
      }
    },
    { $sort: { total_orders: -1, total_revenue: -1 } }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 4: Top 10 khách hàng VIP & số chi nhánh khác nhau đã ghé ($addToSet)
// Toán tử: $match, $group, $addToSet, $project ($size), $sort, $limit
// ---------------------------------------------------------------------
print("\n=== Q4. TOP 10 KHACH HANG VIP ($addToSet) ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: "completed" } },
    {
      $group: {
        _id: "$customer.user_code",
        full_name: { $first: "$customer.full_name" },
        phone: { $first: "$customer.phone" },
        order_count: { $sum: 1 },
        total_spent: { $sum: "$total_amount" },
        visited_branches: { $addToSet: "$restaurant_code" },
        last_order_at: { $max: "$created_at" }
      }
    },
    {
      $project: {
        _id: 0,
        user_code: "$_id",
        full_name: 1,
        phone: 1,
        order_count: 1,
        branch_count: { $size: "$visited_branches" },
        visited_branches: 1,
        total_spent: { $round: ["$total_spent", 0] },
        last_order_at: 1
      }
    },
    { $sort: { total_spent: -1, order_count: -1 } },
    { $limit: 10 }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 5: Nối đơn hàng đã hoàn tất với đánh giá (reviews) & tên chi nhánh
// Toán tử: $match, $lookup (reviews & restaurants), $project, $limit
// ---------------------------------------------------------------------
print("\n=== Q5. NOI DON HANG HOAN TAT VOI DANH GIA ($lookup) ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: "completed" } },
    {
      $lookup: {
        from: "reviews",
        localField: "order_code",
        foreignField: "order_code",
        as: "review_list"
      }
    },
    { $match: { "review_list.0": { $exists: true } } },
    { $unwind: "$review_list" },
    {
      $lookup: {
        from: "restaurants",
        localField: "restaurant_code",
        foreignField: "restaurant_code",
        as: "branch"
      }
    },
    { $unwind: "$branch" },
    {
      $project: {
        _id: 0,
        order_code: 1,
        restaurant_name: "$branch.name",
        customer_name: "$customer.full_name",
        total_amount: 1,
        rating: "$review_list.rating",
        comment: "$review_list.comment",
        has_staff_reply: { $ne: ["$review_list.reply", null] }
      }
    },
    { $sort: { rating: -1, total_amount: -1 } },
    { $limit: 10 }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 6: Doanh thu theo Nhóm món (Category) sử dụng $project + $multiply
// Toán tử: $match, $unwind, $project ($multiply), $lookup, $group, $sort
// ---------------------------------------------------------------------
print("\n=== Q6. DOANH THU THEO NHOM MON ($project/$multiply + $lookup) ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: "completed" } },
    { $unwind: "$items" },
    {
      $project: {
        item_code: "$items.item_code",
        qty: "$items.qty",
        computed_line_total: { $multiply: ["$items.qty", "$items.unit_price"] }
      }
    },
    {
      $lookup: {
        from: "menu_items",
        localField: "item_code",
        foreignField: "item_code",
        as: "menu"
      }
    },
    { $unwind: "$menu" },
    {
      $group: {
        _id: "$menu.category",
        total_qty_sold: { $sum: "$qty" },
        gross_revenue: { $sum: "$computed_line_total" },
        distinct_items: { $addToSet: "$item_code" }
      }
    },
    {
      $project: {
        _id: 0,
        category: "$_id",
        distinct_item_count: { $size: "$distinct_items" },
        total_qty_sold: 1,
        gross_revenue: { $round: ["$gross_revenue", 0] }
      }
    },
    { $sort: { gross_revenue: -1 } }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 7: Thống kê doanh thu & số đơn theo Chi nhánh và Phương thức thanh toán
// Toán tử: $match, $group, $lookup, $sort
// ---------------------------------------------------------------------
print("\n=== Q7. DOANH THU THEO CHI NHANH & PHUONG THUC THANH TOAN ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: { $ne: "cancelled" } } },
    {
      $group: {
        _id: { restaurant_code: "$restaurant_code", payment_method: "$payment_method" },
        order_count: { $sum: 1 },
        total_revenue: { $sum: "$total_amount" }
      }
    },
    {
      $lookup: {
        from: "restaurants",
        localField: "_id.restaurant_code",
        foreignField: "restaurant_code",
        as: "branch"
      }
    },
    { $unwind: "$branch" },
    {
      $project: {
        _id: 0,
        restaurant_code: "$_id.restaurant_code",
        restaurant_name: "$branch.name",
        payment_method: "$_id.payment_method",
        order_count: 1,
        total_revenue: { $round: ["$total_revenue", 0] }
      }
    },
    { $sort: { restaurant_code: 1, total_revenue: -1 } }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 8: Đánh giá hiệu quả Voucher HUIT10 (so sánh đơn dùng mã vs không mã)
// Toán tử: $match, $group ($cond), $project
// ---------------------------------------------------------------------
print("\n=== Q8. SO SANH DON AP DUNG VOUCHER VS KHONG VOUCHER ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: { $ne: "cancelled" } } },
    {
      $group: {
        _id: {
          $cond: [{ $ne: ["$voucher_code", null] }, "Có dùng Voucher", "Không dùng Voucher"]
        },
        order_count: { $sum: 1 },
        total_discount: { $sum: "$discount_amount" },
        total_net_revenue: { $sum: "$total_amount" },
        avg_order_value: { $avg: "$total_amount" }
      }
    },
    {
      $project: {
        _id: 0,
        group_type: "$_id",
        order_count: 1,
        total_discount: { $round: ["$total_discount", 0] },
        total_net_revenue: { $round: ["$total_net_revenue", 0] },
        avg_order_value: { $round: ["$avg_order_value", 0] }
      }
    }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 9: Thống kê chất lượng phục vụ (reviews): Điểm TB, phân bố sao & tỷ lệ phản hồi
// Toán tử: $group, $cond, $lookup, $project
// ---------------------------------------------------------------------
print("\n=== Q9. THONG KE DANH GIA & TY LE PHAN HOI THEO CHI NHANH ===");
printjson(
  db.reviews.aggregate([
    {
      $group: {
        _id: "$restaurant_code",
        review_count: { $sum: 1 },
        avg_rating: { $avg: "$rating" },
        five_star_count: { $sum: { $cond: [{ $eq: ["$rating", 5] }, 1, 0] } },
        replied_count: { $sum: { $cond: [{ $ne: ["$reply", null] }, 1, 0] } }
      }
    },
    {
      $lookup: {
        from: "restaurants",
        localField: "_id",
        foreignField: "restaurant_code",
        as: "branch"
      }
    },
    { $unwind: "$branch" },
    {
      $project: {
        _id: 0,
        restaurant_code: "$_id",
        restaurant_name: "$branch.name",
        review_count: 1,
        avg_rating: { $round: ["$avg_rating", 2] },
        five_star_count: 1,
        replied_count: 1,
        reply_rate_percent: {
          $round: [{ $multiply: [{ $divide: ["$replied_count", "$review_count"] }, 100] }, 1]
        }
      }
    },
    { $sort: { avg_rating: -1 } }
  ]).toArray()
);

// ---------------------------------------------------------------------
// CÂU 10: Báo cáo tổng hợp đa chiều Dashboard trong 1 truy vấn duy nhất ($facet)
// Toán tử: $match, $facet, $group, $unwind, $sort, $limit
// ---------------------------------------------------------------------
print("\n=== Q10. TONG HOP DASHBOARD DA CHIEU ($facet) ===");
printjson(
  db.bookings_orders.aggregate([
    { $match: { status: "completed" } },
    {
      $facet: {
        overview: [
          {
            $group: {
              _id: null,
              completed_orders: { $sum: 1 },
              total_revenue: { $sum: "$total_amount" },
              total_discount: { $sum: "$discount_amount" }
            }
          }
        ],
        top_3_branches: [
          {
            $group: {
              _id: "$restaurant_code",
              orders: { $sum: 1 },
              revenue: { $sum: "$total_amount" }
            }
          },
          { $sort: { revenue: -1 } },
          { $limit: 3 }
        ],
        top_3_items: [
          { $unwind: "$items" },
          {
            $group: {
              _id: "$items.item_code",
              name: { $first: "$items.name" },
              qty: { $sum: "$items.qty" },
              revenue: { $sum: "$items.subtotal" }
            }
          },
          { $sort: { revenue: -1 } },
          { $limit: 3 }
        ]
      }
    }
  ]).toArray()
);
