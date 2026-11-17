// [TV2 - Giai đoạn 1] Tạo 4 loại Index (Single, Compound, Unique, Sparse) & đo .explain("executionStats") trên MongoDB Compass
use("db_booking_nhahang");

// TODO (TV2):
// 1. Đo explain("executionStats") TRƯỚC khi đánh Index
// 2. Tạo Unique Index (users.email, bookings_orders.order_code, ...)
// 3. Tạo Single Index, Compound Index (restaurant_code + booking_date + time_slot), Sparse Index (voucher_code)
// 4. Đo explain("executionStats") SAU khi đánh Index để chụp ảnh vào Chương 3 báo cáo
