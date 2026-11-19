// [TV2 - Giai đoạn 1] Tạo 4 loại Index (Unique, Single, Compound, Sparse) & đo .explain("executionStats") trên MongoDB Compass (MONGOSH)
use("db_booking_nhahang");

function extractStages(plan) {
  const stages = [];
  let cur = plan;
  while (cur) {
    if (cur.stage) stages.push(cur.stage);
    cur = cur.inputStage || (cur.inputStages && cur.inputStages[0]) || null;
  }
  return stages.join(" -> ");
}

function showExplain(label, exp) {
  const stats = exp.executionStats;
  const stage = extractStages(exp.queryPlanner.winningPlan);
  print(`\n[${label}]`);
  printjson({
    winningPlan: stage,
    nReturned: stats.nReturned,
    totalKeysExamined: stats.totalKeysExamined,
    totalDocsExamined: stats.totalDocsExamined,
    executionTimeMillis: stats.executionTimeMillis
  });
}

// =====================================================================
// BƯỚC 0: Đưa index về trạng thái mặc định (_id_) để đo chuẩn TRƯỚC Index
// =====================================================================
db.users.dropIndexes();
db.restaurants.dropIndexes();
db.menu_items.dropIndexes();
db.bookings_orders.dropIndexes();
db.reviews.dropIndexes();

const nowLocal = new Date();
const todayStr = [
  nowLocal.getFullYear(),
  String(nowLocal.getMonth() + 1).padStart(2, "0"),
  String(nowLocal.getDate()).padStart(2, "0")
].join("-");

print("================ 1. DO .explain('executionStats') TRUOC KHI DANH INDEX ================");

const q1Before = db.bookings_orders
  .find({ restaurant_code: "RES01", booking_date: todayStr, time_slot: "19:00" })
  .explain("executionStats");
showExplain("TRƯỚC Index - Q1: Tìm đơn theo chi nhánh + ngày + khung giờ", q1Before);

const q2Before = db.users
  .find({ email: "an.nguyen@huit.edu.vn" })
  .explain("executionStats");
showExplain("TRƯỚC Index - Q2: Tìm user theo email đăng nhập", q2Before);

const q3Before = db.bookings_orders
  .find({ voucher_code: "HUIT10" })
  .explain("executionStats");
showExplain("TRƯỚC Index - Q3: Lọc các đơn có áp dụng voucher_code = HUIT10", q3Before);

// =====================================================================
// BƯỚC 2: TẠO ĐỦ 4 LOẠI INDEX (Unique, Single, Compound, Sparse)
// =====================================================================
print("\n================ 2. TAO 4 LOAI INDEX TREN 5 COLLECTIONS ================");

// (A) UNIQUE INDEX - Chống trùng mã định danh & email đăng nhập
db.users.createIndex({ user_code: 1 }, { unique: true, name: "idx_users_user_code_unique" });
db.users.createIndex({ email: 1 }, { unique: true, name: "idx_users_email_unique" });
db.restaurants.createIndex({ restaurant_code: 1 }, { unique: true, name: "idx_restaurants_code_unique" });
db.menu_items.createIndex({ item_code: 1 }, { unique: true, name: "idx_menu_item_code_unique" });
db.bookings_orders.createIndex({ order_code: 1 }, { unique: true, name: "idx_orders_code_unique" });
db.reviews.createIndex({ review_code: 1 }, { unique: true, name: "idx_reviews_code_unique" });

// (B) SINGLE FIELD INDEX - Tăng tốc lọc theo 1 trường thường xuyên truy vấn
db.menu_items.createIndex({ restaurant_code: 1 }, { name: "idx_menu_restaurant_single" });
db.menu_items.createIndex({ category: 1 }, { name: "idx_menu_category_single" });
db.bookings_orders.createIndex({ "customer.user_code": 1 }, { name: "idx_orders_customer_single" });
db.reviews.createIndex({ restaurant_code: 1 }, { name: "idx_reviews_restaurant_single" });

// (C) COMPOUND INDEX - Tối ưu sơ đồ bàn theo slot & Aggregation theo trạng thái/thời gian
db.bookings_orders.createIndex(
  { restaurant_code: 1, booking_date: 1, time_slot: 1 },
  { name: "idx_orders_res_date_slot_compound" }
);
db.bookings_orders.createIndex(
  { status: 1, booking_date: -1, restaurant_code: 1 },
  { name: "idx_orders_status_date_res_compound" }
);
db.menu_items.createIndex(
  { restaurant_code: 1, category: 1, base_price: 1 },
  { name: "idx_menu_res_cat_price_compound" }
);

// (D) SPARSE INDEX - Chỉ lập chỉ mục cho các document thực sự có trường tùy chọn
// Lưu ý: Trong thực tế nếu field được gán tường minh giá trị null thì có thể kết hợp thêm partialFilterExpression.
db.bookings_orders.createIndex(
  { voucher_code: 1 },
  { sparse: true, name: "idx_orders_voucher_sparse" }
);
db.reviews.createIndex(
  { "reply.replied_at": 1 },
  { sparse: true, name: "idx_reviews_reply_sparse" }
);

print("Danh sách Index trên bookings_orders:");
printjson(db.bookings_orders.getIndexes());

// =====================================================================
// BƯỚC 3: ĐO .explain("executionStats") SAU KHI ĐÁNH INDEX
// =====================================================================
print("\n================ 3. DO .explain('executionStats') SAU KHI DANH INDEX ================");

const q1After = db.bookings_orders
  .find({ restaurant_code: "RES01", booking_date: todayStr, time_slot: "19:00" })
  .explain("executionStats");
showExplain("SAU Index - Q1: Compound Index (res + date + slot)", q1After);

const q2After = db.users
  .find({ email: "an.nguyen@huit.edu.vn" })
  .explain("executionStats");
showExplain("SAU Index - Q2: Unique Index (email)", q2After);

const q3After = db.bookings_orders
  .find({ voucher_code: "HUIT10" })
  .explain("executionStats");
showExplain("SAU Index - Q3: Sparse Index (voucher_code)", q3After);
