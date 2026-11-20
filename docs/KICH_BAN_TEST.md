# KỊCH BẢN KIỂM THỬ CHI TIẾT (TEST SCENARIOS)

Tài liệu mô tả đầy đủ các ca kiểm thử thủ công (Manual UI Test) và kiểm chứng dữ liệu NoSQL (MongoDB `db_booking_nhahang` + Memurai `127.0.0.1:6379`) cho toàn bộ chức năng của hệ thống **Nhà hàng Hương Việt**.

> **Chuẩn bị trước khi test:**
> 1. Đảm bảo dịch vụ **MongoDB** (`127.0.0.1:27017`) và **Memurai** (`127.0.0.1:6379`) đang chạy.
> 2. Khôi phục dữ liệu sạch nếu đã thao tác trước đó:
>    ```powershell
>    cd c:\HUIT\Semeter1_2026-2027\NOSQL\restaurant-booking
>    dotnet run --project RestaurantBooking\RestaurantBooking.csproj -- --seed
>    ```

---

## NHÓM 1: KHỞI ĐỘNG, ĐĂNG KÝ, ĐĂNG NHẬP & PHÂN QUYỀN (RBAC)

| STT | Mã TC | Chức năng | Các bước thực hiện | Kết quả mong đợi (UI) | Kiểm chứng dữ liệu (MongoDB / Memurai) |
| :-: | :--- | :--- | :--- | :--- | :--- |
| 1 | TC01 | Khởi động sạch & giao diện chính thức | Mở ứng dụng (`F5`). Quan sát `FrmMain`. | - Trạng thái **chưa đăng nhập** (`Chào mừng quý khách...`).<br>- Góc phải hiện nút **`Đăng nhập / Đăng ký`**.<br>- Chỉ hiển thị 3 nút khách hàng, ẩn 2 nút Admin.<br>- **Không** có chế độ Demo hay thông tin kỹ thuật. | `SessionContext.IsLoggedIn == false`. |
| 2 | TC02 | Đăng ký tài khoản mới | Bấm **`Đăng nhập / Đăng ký`** → tab **`Đăng ký tài khoản mới`** → nhập: Tên `Phạm Bảo Long`, Email `long.pham@huit.edu.vn`, SĐT `0909888777`, MK `123456`, Xác nhận `123456`, Địa chỉ `140 Lê Trọng Tấn`, Quận `Tân Phú` → bấm **`Đăng ký tài khoản`**. | Thông báo đăng ký thành công, tự chuyển sang tab **`Đăng nhập`** và điền sẵn Email. | MongoDB `users` có thêm bản ghi mới với `role: "customer"`, `password_hash` dạng `$2a$10$...`, mảng `addresses` chứa địa chỉ mặc định. |
| 3 | TC03 | Chặn đăng ký trùng Email / SĐT | Lặp lại đăng ký với Email `long.pham@huit.edu.vn` hoặc SĐT `0909888777`. | Cảnh báo email/SĐT đã tồn tại, không tạo bản ghi trùng. | Số lượng document trong `users` không tăng. |
| 4 | TC04 | Đăng nhập sai mật khẩu | Tab **`Đăng nhập`** → nhập `long.pham@huit.edu.vn` / `sai123` → bấm **`Đăng nhập`**. | Cảnh báo `"Sai thông tin tài khoản hoặc mật khẩu."`, giữ nguyên form. | Không tạo key `session:*` mới trên Memurai. |
| 5 | TC05 | Đăng nhập thành công & tự ẩn nút Tài khoản | Nhập `long.pham@huit.edu.vn` (hoặc SĐT `0909888777`) + MK `123456` → bấm **`Đăng nhập`**. | - Form đăng nhập tự đóng quay về `FrmMain`.<br>- Thanh chào hiện `"Xin chào, Phạm Bảo Long"` (không hiện mã tài khoản).<br>- Nút góc trên phải chỉ còn chữ **`Đăng xuất`**. | Memurai xuất hiện key `session:{token}` kiểu `Hash` gồm `user_code`, `full_name`, `role`, `login_at` với `TTL ≈ 86400s` (24 giờ). |
| 6 | TC06 | Đăng xuất trực tiếp | Tại `FrmMain`, bấm nút **`Đăng xuất`** → chọn **Yes**. | Trở về trạng thái khách chưa đăng nhập; nút góc phải đổi lại thành **`Đăng nhập / Đăng ký`**. | Key `session:{token}` bị xóa (`DEL`) khỏi Memurai. |
| 7 | TC07 | Ẩn trang chủ & quay lại mượt mà | Từ `FrmMain`, bấm **`Thực đơn & Đặt món`** → chọn 1 chi nhánh → bấm **`← Đổi chi nhánh`** → bấm **`← Trang chủ`**. | - Khi mở `FrmBranchSelect`, `FrmMain` tự ẩn đi.<br>- Khi mở `FrmHome`, `FrmBranchSelect` tự ẩn đi.<br>- Bấm **`← Đổi chi nhánh`** quay lại `FrmBranchSelect`; bấm **`← Trang chủ`** quay thẳng về `FrmMain`. | Không để nhiều cửa sổ chồng lên nhau. |

---

## NHÓM 2: CHỌN CHI NHÁNH, THỰC ĐƠN & GIỎ HÀNG (MEMURAI HASH + CACHE-ASIDE)

| STT | Mã TC | Chức năng | Các bước thực hiện | Kết quả mong đợi (UI) | Kiểm chứng dữ liệu (MongoDB / Memurai) |
| :-: | :--- | :--- | :--- | :--- | :--- |
| 8 | TC08 | Chọn chi nhánh ở form riêng | Đăng nhập khách `an.nguyen@huit.edu.vn` (`123456`). Bấm **`Thực đơn & Đặt món`**. | - Mở `FrmBranchSelect` với 3 thẻ chi nhánh: *Hương Việt - Tân Phú*, *Quận 1*, *Quận 3* kèm địa chỉ, giờ mở cửa, SĐT, số bàn.<br>- **Không hiển thị** mã `RES01`/`RES02`/`RES03`. | Đọc từ MongoDB `restaurants` (`is_active: true`). |
| 9 | TC09 | Hiển thị thực đơn sạch cho khách hàng | Chọn thẻ **Hương Việt - Tân Phú**. Quan sát `FrmHome`. | - **Không** có dòng `"Lượt xem"`, **không** có banner Cache, **không** có nút `"Làm mới từ hệ thống"`, **không** có cột `Mã món` hay mã `RES01` ở cả 3 bảng (`Thực đơn`, `Món yêu thích nhất`, `Giỏ hàng`).<br>- Lọc theo nhóm `Lẩu & Nướng` và tìm `"Lẩu Thái"` hoạt động tức thì. | Memurai tự lưu cache `cache:menu:RES01` (TTL 600s) và tự tăng `views:restaurant:RES01` ngầm cho báo cáo Admin. |
| 10 | TC10 | Thêm món tùy chọn Size & Topping vào giỏ | Chọn món *Lẩu Thái Tomyum hải sản* → Size `L (+45,000 đ)` → tích topping *Thêm ba chỉ bò Mỹ (+65,000 đ)* → SL `2` → bấm **`Thêm vào giỏ hàng`**. | Giỏ hàng hiện dòng món với SL `2`, tổng cộng thêm `810,000 đ` (đơn giá `405,000 đ`). Thử bấm `+1 SL`, `-1 SL`, `Xóa món` cập nhật ngay. | Memurai Hash `cart:USR001` lưu field `MN006_L` có `restaurant_code: "RES01"`, `qty`, `unit_price: 405000`, kèm TTL 7 ngày. |
| 11 | TC11 | Xung đột giỏ hàng khác chi nhánh | Bấm **`← Đổi chi nhánh`** → chọn **Hương Việt - Quận 1** → chọn 1 món → bấm **`Thêm vào giỏ hàng`**. | Hộp thoại hỏi: *"Giỏ hàng hiện đang chứa món của chi nhánh Hương Việt - Tân Phú. Bạn có muốn xóa giỏ cũ để đặt món tại chi nhánh Hương Việt - Quận 1 không?"*. Chọn **Yes** sẽ thay giỏ cũ bằng món mới; chọn **No** giữ nguyên giỏ cũ. | Memurai Hash `cart:USR001` chỉ chứa món thuộc đúng 1 chi nhánh duy nhất. |

---

## NHÓM 3: THANH TOÁN ĐƠN MANG VỀ / GIAO HÀNG (`FrmTakeawayCheckout`)

| STT | Mã TC | Chức năng | Các bước thực hiện | Kết quả mong đợi (UI) | Kiểm chứng dữ liệu (MongoDB / Memurai) |
| :-: | :--- | :--- | :--- | :--- | :--- |
| 12 | TC12 | Chặn thanh toán khi giỏ trống | Xóa hết giỏ hàng ở `FrmHome` → bấm **`Đặt mang về / Giao hàng`**. | Thông báo giỏ hàng đang trống, yêu cầu chọn ít nhất 1 món. | Không tạo đơn mới. |
| 13 | TC13 | Mở trang thanh toán giao hàng & tự điền địa chỉ | Thêm *Lẩu Thái Tomyum hải sản* (Size L, 1 phần) ở **Hương Việt - Tân Phú** vào giỏ → bấm **`Đặt mang về / Giao hàng`**. | - Ẩn `FrmHome`, mở màn hình **`Thanh toán đơn mang về / giao hàng`**.<br>- Tự điền sẵn Tên (`Nguyễn Văn An`), SĐT (`0901234501`), Địa chỉ mặc định (`22 Đường số 2, Tân Phú, TP.HCM`).<br>- Bảng món bên trái không hiện mã món. | Đọc thông tin địa chỉ từ `users.addresses` trên MongoDB và giỏ hàng từ Memurai `cart:USR001`. |
| 14 | TC14 | Chuyển chế độ Giao tận nơi ↔ Tự đến lấy | Thử chọn radio **`Tự đến lấy tại quán`**, sau đó chọn lại **`Giao tận nơi`**, xóa trắng ô địa chỉ và bấm **`Xác nhận đặt hàng`**. | - Chọn `Tự đến lấy`: ô địa chỉ khóa đọc và hiện địa chỉ chi nhánh.<br>- Chọn `Giao tận nơi` mà bỏ trống địa chỉ: báo lỗi `"Vui lòng nhập địa chỉ giao hàng chi tiết."` | Validate đầy đủ trước khi ghi DB. |
| 15 | TC15 | Chốt đơn giao hàng kèm Voucher `HUIT10` | Điền địa chỉ `140 Lê Trọng Tấn, P. Tây Thạnh, Q. Tân Phú`, ghi chú `Gọi trước khi giao`, chọn ngày mai `19:00`, nhập voucher `HUIT10`, chọn thanh toán `Ví MoMo` → bấm **`Xác nhận đặt hàng`**. | - Tự tính giảm 10%, hiển thị đúng tổng thanh toán.<br>- Báo đặt hàng thành công kèm Mã đơn, chi nhánh và địa chỉ giao hàng.<br>- Quay lại `FrmHome`, giỏ hàng đã trống. | - MongoDB `bookings_orders` có đơn `order_type: "takeaway"`, `table_id: null`, `delivery_address`, `note`, `voucher_code: "HUIT10"`, `status: "pending"`.<br>- Memurai `cart:USR001` đã bị xóa; `ZSET ranking:menu_items` tăng điểm cho `MN006`. |

---

## NHÓM 4: ĐẶT BÀN TRỰC TUYẾN, GIỮ BÀN 15 PHÚT & WAITLIST (`FrmBooking`)

| STT | Mã TC | Chức năng | Các bước thực hiện | Kết quả mong đợi (UI) | Kiểm chứng dữ liệu (MongoDB / Memurai) |
| :-: | :--- | :--- | :--- | :--- | :--- |
| 16 | TC16 | Kiểm tra ngày/giờ & bảo mật sơ đồ bàn | Mở **`Đặt bàn trực tuyến`** (từ `FrmHome` hoặc `FrmMain`). Thử chọn ngày hôm qua trên lịch. Quan sát tên chi nhánh và nút bàn. | - Không chọn được ngày quá khứ (`MinDate = Today`).<br>- Dropdown chi nhánh chỉ hiện tên (*Hương Việt - Tân Phú*).<br>- Bàn bị người khác giữ chỉ ghi `ĐANG GIỮ CHỖ` + đếm ngược (không lộ mã tài khoản người khác). | `ValidateSlotDateTimeAndHours` chặn mọi slot quá khứ và ngoài giờ mở cửa (09:00–22:30). |
| 17 | TC17 | Giữ bàn 15 phút & chống trùng giữa 2 khách | Chọn ngày mai `19:00`, bấm 1 bàn Trống (ví dụ `Bàn 04`, 4 chỗ) → bấm **`Giữ bàn (15 phút)`**. Sau đó đăng xuất, đăng nhập `khach002@huit.edu.vn` và chọn lại đúng `Bàn 04` khung đó. | - Với khách 1: bàn đổi sang màu cam `BẠN ĐANG GIỮ` và đồng hồ đếm ngược từ `15:00` từng giây.<br>- Với khách 2: bàn hiện `ĐANG GIỮ CHỖ`, nút Giữ bàn & Xác nhận bị vô hiệu hóa. | Memurai tồn tại key `lock:table:RES01:T04:YYYY-MM-DD:19:00` (SET NX EX 900) với giá trị `USR001`. |
| 18 | TC18 | Bỏ giữ bàn & kiểm tra sức chứa | Đăng nhập lại `USR001` → bấm **`Bỏ giữ bàn`**. Sau đó chọn `Bàn 01 (2 chỗ)`, tăng **Số khách = 6** → bấm **`Xác nhận đặt bàn`**. | - Bỏ giữ bàn trả bàn về màu xanh ngay lập tức.<br>- Đặt 6 khách vào bàn 2 chỗ bị chặn kèm cảnh báo vượt sức chứa. | Key `lock:table:...` bị `DEL` khỏi Memurai khi bỏ giữ bàn. |
| 19 | TC19 | Chốt đơn đặt bàn + đối chiếu giá thực tế | Thêm 1 món vào giỏ ở **Hương Việt - Tân Phú** → bấm **`Đặt bàn tại quán`** → chọn `Bàn 04 (4 chỗ)` ngày mai `19:00`, 4 khách → bấm **`Xác nhận đặt bàn`**. | Đặt bàn thành công; bàn `Bàn 04` chuyển ngay sang màu đỏ (`ĐÃ ĐẶT`), giỏ hàng trống. | MongoDB `bookings_orders` lưu đơn `order_type: "dine_in"`, `table_id: "T04"`, giá tính theo giá mới nhất trong `menu_items`; Memurai xóa khóa bàn & giỏ hàng. |
| 20 | TC20 | Đăng ký danh sách chờ & tự gọi khi hủy đơn (FIFO) | Đăng nhập `khach002@huit.edu.vn`, chọn cùng ngày mai `19:00` ở **Hương Việt - Tân Phú** → bấm **`Đăng ký danh sách chờ`**. Sau đó đăng nhập lại `an.nguyen@huit.edu.vn` → vào **`Đơn hàng & Đánh giá`** → chọn đơn `Bàn 04` vừa đặt → bấm **`Hủy đơn hàng đã chọn`**. | - Khách 2 vào hàng đợi vị trí `#1`.<br>- Khi khách 1 hủy đơn, hệ thống thông báo hủy thành công và gọi khách chờ kế tiếp (`Trần Ngọc Bích`). | Memurai List `waitlist:RES01:YYYY-MM-DD:19:00` đẩy vào bằng `LPUSH` và rút ra đúng thứ tự FIFO bằng `RPOP`. Đơn chuyển `status: "cancelled"` trong MongoDB. |

---

## NHÓM 5: DUYỆT ĐƠN (STAFF), ĐÁNH GIÁ SAO & PHẢN HỒI (`FrmBooking` + `FrmHistory`)

| STT | Mã TC | Chức năng | Các bước thực hiện | Kết quả mong đợi (UI) | Kiểm chứng dữ liệu (MongoDB / Memurai) |
| :-: | :--- | :--- | :--- | :--- | :--- |
| 21 | TC21 | Chặn đánh giá đơn chưa hoàn tất | Đăng nhập `an.nguyen@huit.edu.vn` → vào **`Đơn hàng & Đánh giá`** → chọn 1 đơn đang `Chờ xác nhận`. | Nút **`Gửi đánh giá`** bị khóa (disabled), hiện nhắc chỉ gửi đánh giá sau khi đơn hoàn tất. | Không thể tạo review khi `order.status != "completed"`. |
| 22 | TC22 | Quy trình duyệt đơn của Nhân viên | Đăng nhập `binh.tran@nhahang.vn` (`123456`) → vào **`Sơ đồ bàn & Duyệt đơn`** → tab **`Quản lý & Duyệt đơn`** → chọn đơn `Chờ xác nhận` → bấm lần lượt **`1. Xác nhận đơn`** → **`2. Bắt đầu phục vụ`** → **`3. Hoàn tất đơn`**. | Trạng thái đơn chuyển chuẩn từng bước: *Chờ xác nhận* → *Đã xác nhận* → *Đang phục vụ* → *Hoàn tất*. (Tab `Quản lý & Duyệt đơn` bị ẩn hoàn toàn đối với Khách hàng). | MongoDB `bookings_orders` cập nhật `status: "completed"`. |
| 23 | TC23 | Khách gửi đánh giá sao & cập nhật điểm món | Đăng nhập lại khách chủ đơn vừa hoàn tất → vào **`Đơn hàng & Đánh giá`** → chọn đơn `Hoàn tất` → chọn `5 ★`, giữ tích món yêu thích, nhập nhận xét → bấm **`Gửi đánh giá`**. | Thông báo cảm ơn; đơn hiển thị `5 ★`, khóa nút gửi lần 2 (chống đánh giá trùng). | - MongoDB `reviews` thêm document mới.<br>- `menu_items.avg_rating` được tính lại.<br>- Cache `cache:menu:{res_code}` trên Memurai tự xóa để lần đọc sau cập nhật điểm mới. |
| 24 | TC24 | Nhân viên phản hồi đánh giá | Đăng nhập `binh.tran@nhahang.vn` → vào **`Đơn hàng & Phản hồi`** → tab **`Quản lý & Phản hồi đánh giá`** → tích **`Chỉ xem đánh giá chưa phản hồi`** → chọn đánh giá vừa gửi → nhập phản hồi → bấm **`Gửi phản hồi`**. | Cột *Phản hồi của nhà hàng* cập nhật ngay `[Trần Thị Bình] ...`; khi khách đăng nhập xem lại đơn sẽ thấy câu phản hồi của nhà hàng. | MongoDB `reviews` lưu object nhúng `reply: { staff_name, content, replied_at }`. |

---

## NHÓM 6: QUẢN TRỊ HỆ THỐNG, THỐNG KÊ, SAO LƯU & NGƯỜI DÙNG (ADMIN)

| STT | Mã TC | Chức năng | Các bước thực hiện | Kết quả mong đợi (UI) | Kiểm chứng dữ liệu (MongoDB / Memurai) |
| :-: | :--- | :--- | :--- | :--- | :--- |
| 25 | TC25 | CRUD Thực đơn & đồng bộ Cache tức thì | Đăng nhập `admin@nhahang.vn` (`123456`) → mở **`Quản trị Nhà hàng & Thống kê`** → Tab **`Nhà hàng, bàn & thực đơn`**: chọn 1 món (ví dụ *Gỏi cuốn tôm thịt*), đổi giá gốc từ `55,000` sang `60,000` → bấm **`Cập nhật món`**. Sau đó quay lại Trang chủ → vào **Thực đơn** của **Hương Việt - Tân Phú**. | Giao diện khách hàng hiển thị ngay giá mới **`60,000 đ`** (không bị dính giá cũ trong cache). | MongoDB `menu_items` cập nhật `base_price: 60000`; Memurai `cache:menu:RES01` tự bị xóa khi cập nhật món. |
| 26 | TC26 | Ngưng phục vụ món & kiểm tra lúc chốt đơn | Thêm 1 món vào giỏ của khách, sau đó Admin bỏ tích **`Đang phục vụ`** của món đó và lưu. Khách tiến hành chốt đơn. | Hệ thống từ chối chốt đơn và thông báo món đang tạm ngưng phục vụ. | `BookingService.VerifyAndRepriceCartAsync` đối chiếu `is_available` trực tiếp trên MongoDB. |
| 27 | TC27 | Xem 7 báo cáo thống kê & biểu đồ cột | Mở **`Quản trị Nhà hàng & Thống kê`** → Tab **`Báo cáo & thống kê kinh doanh`** → chọn lần lượt 7 báo cáo trong combobox và bấm **`Làm mới số liệu`**. | - 4 thẻ KPI hiển thị đúng tổng chi nhánh/bàn, số món, tổng đơn/hoàn tất, doanh thu.<br>- Bảng số liệu và biểu đồ cột vẽ sắc nét cho cả 7 báo cáo. | Chạy MongoDB Aggregation Pipeline trực tiếp trên `bookings_orders`, `reviews` và đọc Memurai `ZSET` / `views`. |
| 28 | TC28 | Sao lưu, Xuất JSON & Khôi phục + làm mới Cache | Mở Tab **`Sao lưu & phục hồi dữ liệu`** → bấm **`Sao lưu dữ liệu chính`**, **`Xuất 5 bảng ra tập tin (JSON)`**, **`Lưu trạng thái bộ nhớ tạm`**, **`Kiểm tra chế độ tự động lưu`**. Thử khôi phục lại từ bản sao lưu vừa tạo. | - Khung nhật ký hiển thị `ExitCode: 0 (SUCCESS)` cho từng thao tác.<br>- Thư mục sao lưu có đầy đủ file `.bson`/`.json`/RDB-AOF.<br>- Sau khi khôi phục, bộ nhớ đệm thực đơn tự làm mới. | Kiểm tra file trong `Documents\RestaurantBookingBackup` và xác nhận `cache:menu:*` được xóa sau `mongorestore`/`mongoimport`. |
| 29 | TC29 | Khóa tài khoản & thu hồi phiên ngay lập tức | Mở **`Quản trị Người dùng`**:<br>1) Chọn chính `ADM001`, bỏ tích `Đang hoạt động` → bấm **Lưu**.<br>2) Chọn `USR018` (đang có phiên đăng nhập), bỏ tích `Đang hoạt động` → bấm **Lưu**.<br>3) Thử đăng nhập lại bằng `USR018`. | - Bước 1: bị chặn (không cho Admin tự khóa mình).<br>- Bước 2: cập nhật thành công trạng thái `Đã khóa`.<br>- Bước 3: bị từ chối đăng nhập vì tài khoản đang bị khóa. | MongoDB `users.is_active = false`; toàn bộ key `session:*` của `USR018` trên Memurai bị thu hồi (`DEL`) ngay lập tức. |
| 30 | TC30 | Đặt lại mật khẩu người dùng | Trong **`Quản trị Người dùng`**, chọn `USR018`, mở khóa lại, nhập mật khẩu mới `654321` → bấm **`Đặt lại mật khẩu`**. Thử đăng nhập với mật khẩu mới. | Đăng nhập thành công với mật khẩu mới `654321`, mật khẩu cũ `123456` không còn hợp lệ. | MongoDB `users.password_hash` cập nhật chuỗi băm BCrypt mới. |

---

## Kiểm thử tự động toàn hệ thống (Regression Check)

Ngoài kiểm thử giao diện theo 30 kịch bản trên, toàn bộ tầng nghiệp vụ (`AuthService`, `CartReviewService`, `MenuAdminService`, `BookingService`, `BackupService`) đã được kiểm chứng tự động qua **45 kịch bản tích hợp** (bao gồm phân quyền RBAC tầng Service, ngày/giờ quá khứ, xung đột chi nhánh giỏ hàng, tính lại giá trực tiếp từ MongoDB, hủy đơn quá hạn, `RPOP` Waitlist và thu hồi session Memurai).
