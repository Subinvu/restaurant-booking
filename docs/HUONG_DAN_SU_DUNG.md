# HƯỚNG DẪN SỬ DỤNG ỨNG DỤNG NHÀ HÀNG HƯƠNG VIỆT

Ứng dụng desktop quản lý chuỗi **Nhà hàng Hương Việt** (Đặt món giao hàng, Đặt bàn trực tuyến, Quản lý đơn hàng, Đánh giá & Quản trị hệ thống), xây dựng trên **.NET 9 WinForms**, lưu trữ dữ liệu chính bằng **MongoDB** (`db_booking_nhahang`) và tăng tốc xử lý thời gian thực bằng **Memurai / Redis** (`127.0.0.1:6379`).

---

## 1. Yêu cầu hệ thống & Khởi chạy

### 1.1. Dịch vụ nền cần bật trước khi mở ứng dụng
1. **MongoDB Server** đang chạy tại `127.0.0.1:27017` (dịch vụ Windows `MongoDB`).
2. **Memurai Developer** đang chạy tại `127.0.0.1:6379` (dịch vụ Windows `Memurai`).
3. *(Tùy chọn cho chức năng Sao lưu / Phục hồi của Quản trị viên)*: **MongoDB Database Tools** (`mongodump`, `mongorestore`, `mongoexport`, `mongoimport`) đã được cài đặt.

### 1.2. Cách mở & chạy ứng dụng
- **Cách 1 (Visual Studio 2022):** Mở giải pháp `restaurant-booking\RestaurantBooking.sln` và nhấn **F5** (hoặc `Ctrl + F5`).
- **Cách 2 (Dòng lệnh PowerShell / Terminal):**
  ```powershell
  cd c:\HUIT\Semeter1_2026-2027\NOSQL\restaurant-booking
  dotnet run --project RestaurantBooking\RestaurantBooking.csproj
  ```
- **Khôi phục lại bộ dữ liệu mẫu chuẩn (khi cần làm mới dữ liệu kiểm thử):**
  ```powershell
  cd c:\HUIT\Semeter1_2026-2027\NOSQL\restaurant-booking
  dotnet run --project RestaurantBooking\RestaurantBooking.csproj -- --seed
  ```
  *(Lần đầu khởi chạy nếu cơ sở dữ liệu còn trống, ứng dụng sẽ tự động nạp sẵn 3 chi nhánh / 36 bàn, 30 món ăn, 20 tài khoản, 500 đơn hàng và 100 đánh giá).*

---

## 2. Danh sách tài khoản có sẵn

Tất cả tài khoản mẫu dùng chung mật khẩu **`123456`** (mã hóa BCrypt trong MongoDB):

| Vai trò | Họ và tên | Email đăng nhập | Số điện thoại | Quyền hạn chính |
| :--- | :--- | :--- | :--- | :--- |
| **Khách hàng** | Nguyễn Văn An | `an.nguyen@huit.edu.vn` | `0901234501` | Xem thực đơn, đặt món mang về/giao hàng, đặt bàn, hủy đơn cá nhân, gửi đánh giá |
| **Khách hàng** | Trần Ngọc Bích | `khach002@huit.edu.vn` | `0908000002` | Tương tự khách hàng |
| **Nhân viên** | Trần Thị Bình | `binh.tran@nhahang.vn` | `0901234502` | Quyền khách hàng + duyệt/cập nhật trạng thái đơn, xử lý danh sách chờ, phản hồi đánh giá |
| **Quản trị viên** | Lê Hoàng Minh | `admin@nhahang.vn` | `0901234500` | Toàn quyền + Quản trị chi nhánh/bàn/thực đơn, Thống kê doanh thu, Sao lưu/Phục hồi, Quản trị tài khoản |

### Mã giảm giá đang áp dụng
- **`HUIT10`**: Giảm **10%** tổng tiền món ăn (tối đa **100.000 đ**).
- **`VIP50K`**: Giảm trực tiếp **50.000 đ** cho đơn từ **250.000 đ** trở lên.

---

## 3. Luồng điều hướng tổng quan

Ứng dụng hiển thị **một màn hình tập trung tại mỗi thời điểm** — khi chuyển sang trang con, trang trước tự động ẩn đi và hiện lại khi bạn nhấn nút **`← Quay lại`** ở góc trên bên phải:

```text
Trang chủ (FrmMain)
 ├── Đăng nhập / Đăng ký (FrmLogin)
 ├── Thực đơn & Đặt món
 │    └── Chọn chi nhánh (FrmBranchSelect)
 │         └── Thực đơn chi nhánh & Giỏ hàng (FrmHome)
 │              ├── Đặt bàn tại quán ──────────► Sơ đồ bàn & Đặt bàn (FrmBooking)
 │              └── Đặt mang về / Giao hàng ───► Thanh toán & Địa chỉ giao hàng (FrmTakeawayCheckout)
 ├── Đặt bàn trực tuyến (FrmBooking) [Nhân viên/Admin có thêm Tab Quản lý & Duyệt đơn]
 ├── Đơn hàng & Đánh giá (FrmHistory) [Nhân viên/Admin có thêm Tab Quản lý & Phản hồi đánh giá]
 ├── [Admin] Quản trị Nhà hàng & Thống kê (FrmAdmin)
 └── [Admin] Quản trị Người dùng (FrmUserAdmin)
```

---

## 4. Hướng dẫn chi tiết theo từng vai trò

### 4.1. Khách tham quan & Khách hàng

#### Bước 1: Đăng nhập hoặc Đăng ký tài khoản
1. Tại màn hình chính, nhấn **`Đăng nhập / Đăng ký`** ở góc trên bên phải.
2. **Đăng nhập:** Nhập Email hoặc Số điện thoại + Mật khẩu → nhấn **`Đăng nhập`**. Cửa sổ tự đóng và quay về Trang chủ; nút góc trên phải chuyển thành **`Đăng xuất`**.
3. **Đăng ký mới:** Chuyển sang tab **`Đăng ký tài khoản mới`**, điền Họ tên, Email, SĐT, Mật khẩu (≥ 6 ký tự) và Địa chỉ mặc định (dùng để tự điền khi đặt giao hàng) → nhấn **`Đăng ký tài khoản`**.
4. **Đăng xuất:** Nhấn nút **`Đăng xuất`** ở góc trên bên phải màn hình chính và xác nhận.

#### Bước 2: Chọn chi nhánh & Xem thực đơn, thêm vào giỏ hàng
1. Tại Trang chủ, nhấn **`Thực đơn & Đặt món`**.
2. Màn hình **Chọn chi nhánh** hiển thị các cơ sở của Nhà hàng Hương Việt (tên, địa chỉ, giờ mở cửa, số điện thoại, số bàn). Bấm vào thẻ chi nhánh muốn đặt (hoặc nút **`Xem thực đơn →`**).
3. Tại màn hình **Thực đơn chi nhánh**:
   - Lọc theo **Nhóm món** hoặc gõ từ khóa vào ô **Tìm kiếm**.
   - Xem bảng **Thực đơn chi nhánh** và bảng **Món được yêu thích nhất** bên dưới.
   - Bấm vào một món để chọn **Kích cỡ**, **Số lượng**, và tích chọn **Topping dùng kèm** ở khung bên phải → nhấn **`Thêm vào giỏ hàng`**.
   - Điều chỉnh giỏ hàng bằng các nút **`+1 SL`**, **`-1 SL`**, **`Xóa món`**, **`Xóa hết giỏ`**.
   - Nếu giỏ hàng đang có món của chi nhánh khác, hệ thống sẽ hỏi xác nhận xóa giỏ cũ trước khi thêm món ở chi nhánh mới.
   - Nhấn **`← Đổi chi nhánh`** để chọn chi nhánh khác hoặc **`← Trang chủ`** để về màn hình chính.

#### Bước 3A: Đặt mang về / Giao hàng tận nơi
1. Sau khi đã có món trong giỏ hàng tại màn hình Thực đơn, nhấn **`Đặt mang về / Giao hàng`**.
2. Tại màn hình **Thanh toán đơn mang về / giao hàng**:
   - Kiểm tra lại chi nhánh phục vụ và danh sách món ăn bên cột trái.
   - Chọn **`Giao tận nơi`** (nhập/kiểm tra Họ tên, SĐT, Địa chỉ giao hàng — hệ thống tự điền địa chỉ mặc định của bạn) hoặc **`Tự đến lấy tại quán`**.
   - Chọn **Ngày nhận**, **Khung giờ**, nhập **Ghi chú**, **Mã giảm giá** (nếu có) và **Phương thức thanh toán**.
   - Nhấn **`Xác nhận đặt hàng`** để hoàn tất.

#### Bước 3B: Đặt bàn ăn tại quán
1. Từ màn hình Thực đơn nhấn **`Đặt bàn tại quán`** (hoặc từ Trang chủ nhấn **`Đặt bàn trực tuyến`**).
2. Chọn **Chi nhánh**, **Ngày** (chỉ cho phép từ hôm nay trở đi), **Khung giờ** (trong giờ mở cửa 09:00–22:30), và **Số khách**.
3. Quan sát **Sơ đồ bàn**:
   - **Xanh lá (`TRỐNG`)**: Bàn sẵn sàng đặt.
   - **Cam (`ĐANG GIỮ CHỖ`)**: Bàn đang được giữ tạm tối đa 15 phút (có đồng hồ đếm ngược thời gian thực).
   - **Đỏ (`ĐÃ ĐẶT`)**: Bàn đã có khách đặt trong khung giờ này.
4. Bấm chọn một bàn trống:
   - Nhấn **`Giữ bàn (15 phút)`** để khóa chỗ tạm trong lúc suy nghĩ / chọn thêm món (hoặc **`Bỏ giữ bàn`** nếu đổi ý).
   - Nhập **Mã giảm giá** (nếu có món đặt trước trong giỏ), chọn **Phương thức thanh toán** và nhấn **`Xác nhận đặt bàn`**.
5. **Nếu khung giờ đã hết bàn phù hợp:** Nhấn **`Đăng ký danh sách chờ`** ở thanh phía dưới sơ đồ bàn. Khi có đơn hủy trong cùng khung giờ, hệ thống sẽ tự động gọi khách đứng đầu danh sách chờ.

#### Bước 4: Theo dõi đơn hàng, Hủy đơn & Gửi đánh giá
1. Từ Trang chủ nhấn **`Đơn hàng & Đánh giá`**.
2. Lọc đơn theo trạng thái (`Chờ xác nhận`, `Đã xác nhận`, `Đang phục vụ`, `Hoàn tất`, `Đã hủy`) và bấm vào một đơn để xem chi tiết món ăn.
3. **Hủy đơn:** Chọn đơn ở trạng thái *Chờ xác nhận* hoặc *Đã xác nhận* (của khung giờ chưa qua) → nhấn **`Hủy đơn hàng đã chọn`**.
4. **Đánh giá trải nghiệm:** Chọn đơn ở trạng thái **Hoàn tất** chưa đánh giá → chọn **Số sao (1–5 ★)**, tích các **Món ăn bạn yêu thích**, nhập nhận xét → nhấn **`Gửi đánh giá`**.

---

### 4.2. Nhân viên (Staff)
Ngoài các chức năng trên, Nhân viên có thêm 2 tab quản lý:
1. **Trong `Sơ đồ bàn & Duyệt đơn` → Tab `Quản lý & Duyệt đơn`:**
   - Lọc đơn theo chi nhánh, ngày, trạng thái.
   - Chọn đơn và chuyển bước lần lượt: **`1. Xác nhận đơn`** → **`2. Bắt đầu phục vụ`** → **`3. Hoàn tất đơn`**.
   - Nhấn **`Hủy đơn & Gọi khách chờ kế tiếp`** để hủy đơn và tự động rút người đứng đầu hàng đợi chờ bàn.
2. **Trong `Đơn hàng & Phản hồi` → Tab `Quản lý & Phản hồi đánh giá`:**
   - Lọc đánh giá theo chi nhánh hoặc tích **`Chỉ xem đánh giá chưa phản hồi`**.
   - Chọn đánh giá, nhập lời cảm ơn/phản hồi và nhấn **`Gửi phản hồi`**.

---

### 4.3. Quản trị viên (Admin)
Quản trị viên có thêm 2 phân hệ riêng hiển thị tại Trang chủ:

1. **`Quản trị Nhà hàng & Thống kê`:**
   - **Tab 1 — Nhà hàng, bàn & thực đơn:** Thêm/Sửa/Xóa chi nhánh, quản lý danh sách bàn của từng chi nhánh, Thêm/Sửa/Xóa món ăn (giá, size L, topping, trạng thái còn bán). Mọi cập nhật thực đơn tự động làm mới bộ nhớ đệm.
   - **Tab 2 — Báo cáo & thống kê kinh doanh:** Xem 4 thẻ KPI tổng quan và 7 báo cáo phân tích (Doanh thu theo tháng & chi nhánh, Top 10 món doanh thu cao nhất, Khung giờ cao điểm, Top khách VIP, Thống kê đánh giá, Top món đặt nhiều nhất, Xếp hạng chi nhánh) kèm biểu đồ cột trực quan.
   - **Tab 3 — Sao lưu & phục hồi dữ liệu:** Sao lưu nhị phân (`mongodump`), Khôi phục (`mongorestore`), Xuất/Nhập JSON từng collection (`mongoexport`/`mongoimport`), Lưu nhanh & sao lưu tệp bền vững Memurai (`BGSAVE` / `RDB` + `AOF`). Sau khi khôi phục hoặc nhập JSON, bộ nhớ đệm thực đơn tự động được làm mới.
2. **`Quản trị Người dùng`:**
   - Tìm kiếm tài khoản theo mã, tên, email, SĐT; lọc theo vai trò và trạng thái.
   - Đổi vai trò (`Khách hàng` / `Nhân viên` / `Quản trị viên`), **Khóa / Mở khóa tài khoản** (khóa tài khoản tự động thu hồi phiên đăng nhập hiện tại ngay lập tức; chặn Admin tự khóa chính mình) và **Đặt lại mật khẩu**.
