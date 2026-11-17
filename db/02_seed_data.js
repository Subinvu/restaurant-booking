// [TV1 - Giai đoạn 0] Script dự phòng chạy trực tiếp trong MONGOSH (MongoDB Compass)
// Chú ý: App C# WinForms (Services/DataSeeder.cs) cũng tự động nạp tập dữ liệu chuẩn này khi khởi chạy lần đầu.

use("db_booking_nhahang");

db.users.drop();
db.restaurants.drop();
db.menu_items.drop();
db.bookings_orders.drop();
db.reviews.drop();

const now = new Date();
const defaultHash = "$2a$10$gxBqrTdI17qGQ/F/fNdnt.onuZskoU4X/xTQXtgC7TyBst01BLXhe"; // Mật khẩu mẫu: "123456"

// 1. Nạp 20 users (1 admin, 2 staff, 17 customer)
const users = [
  { user_code: "ADM001", full_name: "Lê Hoàng Minh", email: "admin@nhahang.vn", phone: "0901234500", password_hash: defaultHash, role: "admin", addresses: [{ label: "Nhà riêng", street: "140 Lê Trọng Tấn", district: "Tân Phú", city: "TP.HCM", is_default: true }], is_active: true, created_at: now },
  { user_code: "STF001", full_name: "Trần Thị Bình", email: "binh.tran@nhahang.vn", phone: "0901234502", password_hash: defaultHash, role: "staff", addresses: [{ label: "Nhà riêng", street: "88 Nguyễn Huệ", district: "Quận 1", city: "TP.HCM", is_default: true }], is_active: true, created_at: now },
  { user_code: "STF002", full_name: "Phạm Quốc Cường", email: "cuong.pham@nhahang.vn", phone: "0901234503", password_hash: defaultHash, role: "staff", addresses: [{ label: "Nhà riêng", street: "215 Võ Văn Tần", district: "Quận 3", city: "TP.HCM", is_default: true }], is_active: true, created_at: now }
];
const customerNames = [
  "Nguyễn Văn An", "Trần Ngọc Bích", "Lê Minh Châu", "Phạm Gia Dũng",
  "Hoàng Thu Hà", "Vũ Đức Hải", "Đặng Khánh Linh", "Bùi Tấn Lộc",
  "Đỗ Thanh Mai", "Hồ Quang Nam", "Ngô Phương Nghi", "Dương Tuấn Phát",
  "Lý Kim Quyên", "Võ Thành Tài", "Phan Bảo Trâm", "Trương Minh Tuấn", "Huỳnh Thảo Vy"
];
const districts = ["Tân Phú", "Quận 1", "Quận 3", "Tân Bình", "Bình Thạnh", "Phú Nhuận"];
for (let i = 0; i < customerNames.length; i++) {
  const idx = String(i + 1).padStart(3, "0");
  users.push({
    user_code: `USR${idx}`,
    full_name: customerNames[i],
    email: i === 0 ? "an.nguyen@huit.edu.vn" : `khach${idx}@huit.edu.vn`,
    phone: i === 0 ? "0901234501" : `0908${String(i + 1).padStart(6, "0")}`,
    password_hash: defaultHash,
    role: "customer",
    addresses: [{ label: "Nhà riêng", street: `${15 + (i + 1) * 7} Đường số ${(i % 9) + 1}`, district: districts[i % districts.length], city: "TP.HCM", is_default: true }],
    is_active: true,
    created_at: now
  });
}
db.users.insertMany(users);

// 2. Nạp 3 nhà hàng & 36 bàn nhúng
const resDefs = [
  { code: "RES01", name: "Hương Việt - Tân Phú", district: "Tân Phú", address: "140 Lê Trọng Tấn, P. Tây Thạnh, Q. Tân Phú", phone: "02838123401" },
  { code: "RES02", name: "Hương Việt - Quận 1", district: "Quận 1", address: "88 Nguyễn Huệ, P. Bến Nghé, Q. 1", phone: "02838123402" },
  { code: "RES03", name: "Hương Việt - Quận 3", district: "Quận 3", address: "215 Võ Văn Tần, P. 5, Q. 3", phone: "02838123403" }
];
const restaurants = resDefs.map(r => {
  const tables = [];
  for (let t = 1; t <= 12; t++) {
    const area = t <= 6 ? "indoor" : (t <= 9 ? "outdoor" : "vip");
    const capacity = area === "vip" ? (t === 12 ? 10 : 8) : (area === "outdoor" ? 4 : (t <= 2 ? 2 : 4));
    tables.push({ table_id: `T${String(t).padStart(2, "0")}`, table_name: `Bàn ${String(t).padStart(2, "0")}`, area, capacity, is_available: true });
  }
  return { restaurant_code: r.code, name: r.name, district: r.district, address: r.address, phone: r.phone, open_time: "09:00", close_time: "22:30", is_active: true, tables };
});
db.restaurants.insertMany(restaurants);

// 3. Nạp 30 món ăn
const menuTemplates = [
  ["Gỏi cuốn tôm thịt", "Khai vị", 55000], ["Chả giò hải sản giòn", "Khai vị", 68000],
  ["Cơm chiên hải sản", "Món chính", 115000], ["Bò lúc lắc khoai tây", "Món chính", 165000],
  ["Cá lóc nướng trui cuốn bánh tráng", "Món chính", 185000], ["Lẩu Thái Tomyum hải sản", "Lẩu & Nướng", 295000],
  ["Sườn non nướng mật ong", "Lẩu & Nướng", 225000], ["Lẩu riêu cua bắp bò", "Lẩu & Nướng", 315000],
  ["Trà đào cam sả", "Đồ uống", 42000], ["Chè khúc bạch hạnh nhân", "Tráng miệng", 48000]
];
const menuItems = [];
let itemSeq = 1;
for (const r of restaurants) {
  for (const tpl of menuTemplates) {
    const code = `MN${String(itemSeq++).padStart(3, "0")}`;
    menuItems.push({
      item_code: code, restaurant_code: r.restaurant_code, name: tpl[0], category: tpl[1],
      base_price: tpl[2] + (r.restaurant_code === "RES02" ? 10000 : 0),
      image_url: `images/menu/${code.toLowerCase()}.jpg`,
      sizes: [{ size_code: "S", label: "Phần vừa", extra_price: 0 }, { size_code: "L", label: "Phần lớn", extra_price: tpl[1] === "Đồ uống" ? 12000 : 45000 }],
      toppings: [{ topping_code: "TOP01", name: "Thêm món kèm", price: 15000 }],
      is_available: true, avg_rating: 4.6, created_at: now
    });
  }
}
db.menu_items.insertMany(menuItems);

// 4. Nạp 500 bookings_orders
const slots = ["11:00", "13:00", "17:30", "19:00", "20:30"];
const payMethods = ["cash", "momo", "banking"];
const orders = [];
for (let i = 1; i <= 500; i++) {
  const res = restaurants[i % restaurants.length];
  const resMenu = menuItems.filter(m => m.restaurant_code === res.restaurant_code);
  const cust = users[3 + (i % 17)];
  const dayOffset = i > 485 ? 0 : -((i % 55) + 1);
  const dt = new Date(now.getTime() + dayOffset * 86400000);
  const dateStr = dt.toISOString().slice(0, 10);
  const m = resMenu[i % resMenu.length];
  const qty = (i % 3) + 1;
  const sub = m.base_price * qty;
  orders.push({
    order_code: `ORD${dateStr.replace(/-/g, "")}${String(i).padStart(3, "0")}`,
    order_type: i % 5 === 0 ? "takeaway" : "dine_in",
    customer: { user_code: cust.user_code, full_name: cust.full_name, phone: cust.phone },
    restaurant_code: res.restaurant_code,
    table_id: i % 5 === 0 ? null : `T${String(((i % 12) + 1)).padStart(2, "0")}`,
    booking_date: dateStr,
    time_slot: slots[i % slots.length],
    guest_count: 2 + (i % 5),
    items: [{ item_code: m.item_code, name: m.name, size_code: "S", toppings: [], qty, unit_price: m.base_price, subtotal: sub }],
    voucher_code: null, discount_amount: 0, total_amount: sub,
    payment_method: payMethods[i % payMethods.length],
    status: dayOffset < 0 ? (i % 7 === 0 ? "cancelled" : "completed") : "confirmed",
    created_at: dt
  });
}
db.bookings_orders.insertMany(orders);

// 5. Nạp 100 reviews
const completed = orders.filter(o => o.status === "completed");
const reviews = [];
for (let i = 0; i < 100; i++) {
  const o = completed[i % completed.length];
  reviews.push({
    review_code: `REV${String(i + 1).padStart(3, "0")}`,
    order_code: o.order_code, user_code: o.customer.user_code, full_name: o.customer.full_name,
    restaurant_code: o.restaurant_code, rating: i % 8 === 0 ? 3 : (i % 2 === 0 ? 5 : 4),
    comment: "Món ăn ngon, đặt bàn nhanh chóng, nhân viên phục vụ chu đáo.",
    liked_items: o.items.map(it => it.item_code),
    reply: i % 2 === 0 ? { staff_name: "Trần Thị Bình", content: "Cảm ơn quý khách đã ủng hộ!", replied_at: now } : null,
    created_at: now
  });
}
db.reviews.insertMany(reviews);

print("Seed completed:", {
  users: db.users.countDocuments(),
  restaurants: db.restaurants.countDocuments(),
  menu_items: db.menu_items.countDocuments(),
  bookings_orders: db.bookings_orders.countDocuments(),
  reviews: db.reviews.countDocuments()
});
