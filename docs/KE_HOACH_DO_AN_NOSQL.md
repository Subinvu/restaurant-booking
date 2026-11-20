# KẾ HOẠCH HOÀN THIỆN ĐỒ ÁN MÔN HỌC: DỮ LIỆU NOSQL (01201073 - 14DH)

> **Tên đề tài (đề xuất gửi GV duyệt)**: *"Thiết kế cơ sở dữ liệu NoSQL kết hợp MongoDB và Redis (Memurai) xây dựng phần mềm Quản lý Đặt bàn Nhà hàng và Đặt món ăn trực tuyến"*
>
> **Quy mô nhóm**: 3 thành viên (**Thành viên 1 — Nhóm trưởng** | **Thành viên 2** | **Thành viên 3**)
>
> **Công nghệ chốt (Chạy trực tiếp trên Windows — Không dùng Docker)**:
> - **CSDL Tài liệu (Lưu trữ chính)**: **MongoDB Community Server** (`127.0.0.1:27017`) + **MongoDB Compass** + **MongoDB Database Tools**
> - **CSDL Key-Value In-Memory (Thời gian thực & Cache)**: **Memurai Developer Edition** (tương thích 100% Redis trên Windows, `127.0.0.1:6379`) + **Redis Insight**
> - **Ứng dụng Desktop**: **C# WinForms (.NET 9.0 / Visual Studio 2022)**
> - **Thư viện NuGet dùng chung**: `MongoDB.Driver`, `StackExchange.Redis`, `BCrypt.Net-Next`

---

## 1. Mục tiêu Chuẩn đầu ra (CLOs) & Chiến lược Đạt 10,0 Điểm

### 1.1. Ánh xạ Thang điểm 10,0 của Khoa CNTT – HUIT

| STT | Nội dung đánh giá (theo văn bản của Khoa) | Điểm | Cách nhóm thực hiện để đạt điểm tối đa |
| :-: | :--- | :-: | :--- |
| **1–4** | Lý thuyết & cài đặt Tool; Chức năng Tool; So sánh với GUI Tool khác; Điểm mạnh theo loại ứng dụng *(Nếu không tìm hiểu GUI Tool mới ngoài 4 hệ đã học thì tăng thêm chức năng DB & ứng dụng 2,0 điểm)* | **2,0** | **Làm đầy đủ cả 2 vế để an toàn tuyệt đối**:<br>1) Trong Chương 1 trình bày đủ 4 ý: cài đặt & chức năng **MongoDB Compass + Memurai/Redis Insight**; so sánh Compass với **Studio 3T** và Redis Insight với **Another Redis Desktop Manager**; nêu bật loại ứng dụng phù hợp từng hệ.<br>2) **Mở rộng DB & chức năng ứng dụng (+2,0đ)**: Kết hợp đa mô hình **MongoDB + Memurai (Redis)** giải quyết giữ chỗ bàn `TTL`, hàng đợi `Waitlist`, giỏ hàng `Hash`, bảng xếp hạng `Sorted Set`, và `Cache-Aside` chống *Cache Avalanche / Penetration*. |
| **5** | Xây dựng Database cho ứng dụng:<br>- Tạo Database (0,5)<br>- Insert dữ liệu đầy đủ (0,5)<br>- Đáp ứng truy vấn cơ bản (0,5)<br>- Đáp ứng truy vấn nâng cao (0,5) | **2,0** | Thiết kế 5 Collections trên MongoDB (`users`, `restaurants`, `menu_items`, `bookings_orders`, `reviews`) + 8 mẫu Key trên Memurai. Viết `DataSeeder.cs` (và script `db/02_seed_data.js`) tự động nạp 3 chi nhánh, 36 bàn, 30 món ăn, 20 users, **500 đơn đặt bàn/món** và 100 đánh giá. |
| **6** | Import/Export dữ liệu; Backup và Restore dữ liệu | **0,5** | - **MongoDB**: Dùng `mongoexport` / `mongoimport` (JSON/CSV) và `mongodump` / `mongorestore`.<br>- **Memurai**: Cấu hình Persistence **RDB (`dump.rdb`, `BGSAVE`)** và **AOF (`appendonly.aof`)** trong `memurai.conf`; kiểm chứng Restore bằng cách sao lưu/khôi phục file RDB/AOF và restart service `Memurai`.<br>- Tích hợp nút **Backup / Export / Restore** (mở `FolderBrowserDialog` / `OpenFileDialog`) ngay trên `FrmAdmin`. |
| **7** | Triển khai truy vấn trên GUI Tool:<br>- Truy vấn cơ bản (0,5)<br>- Truy vấn nâng cao (0,5) | **1,0** | - **Cơ bản**: 12 câu CRUD, lọc `$in`, `$gt/$lte`, regex, `$elemMatch`, projection, `sort/limit/skip` trên **MongoDB Compass** + lệnh thao tác key trên **Redis Insight**.<br>- **Nâng cao**: 8–10 câu **Aggregation Pipeline** (`$match`, `$unwind`, `$group`, `$lookup`, `$addToSet`) + đánh **4 loại Index** (Single, Compound, Unique, Sparse) và đo hiệu năng **`.explain("executionStats")`** trước/sau Index *(chạy trực tiếp & chụp ảnh trên GUI Tool)*. |
| **8** | Triển khai kết nối CSDL với phần mềm ứng dụng | **0,5** | Lớp `Config/DbContext.cs` trong C# kết nối đồng thời `MongoClient` (`127.0.0.1:27017`) và `ConnectionMultiplexer` của `StackExchange.Redis` (`127.0.0.1:6379`) có xử lý lỗi & kiểm tra kết nối. |
| **9** | Demo ứng dụng:<br>- Quản trị người dùng (0,5)<br>- Thao tác CSDL thêm, xóa, sửa, backup (0,5)<br>- Quản lý theo mô tả bài toán (0,5)<br>- Chức năng hỗ trợ khác (0,5) | **2,0** | App C# WinForms có đủ 4 phân hệ tách Form độc lập:<br>1) Đăng ký, Đăng nhập (`FrmLogin` — Session Memurai TTL), Phân quyền `admin`/`staff`/`customer`, Quản lý & Khóa user (`FrmUserAdmin`).<br>2) CRUD Nhà hàng, Bàn, Thực đơn (`FrmAdmin`), Duyệt đơn (`FrmBooking` tab Quản lý) + Nút Backup/Restore/Export (`FrmAdmin`).<br>3) Sơ đồ bàn trực quan bằng mảng `Button` (Xanh/Vàng đếm ngược `Timer` TTL 15p/Đỏ), Giữ bàn chống trùng, Đặt món Size/Topping, Hàng đợi `Waitlist`.<br>4) Thực đơn `Cache-Aside`, Bảng xếp hạng món hot (`ZSET`), Thống kê doanh thu (`DataGridView` + biểu đồ cột WinForms), Đánh giá sao. |
| **10** | Báo cáo Word (0,5) + Thuyết trình & trả lời câu hỏi (0,5) + Nộp đúng hạn đủ Word/PPT/Code (0,5) + Phân công nhóm (0,5) | **2,0** | Báo cáo **50–60 trang**, font **Times New Roman, size 13, giãn dòng 1.5 line**, bảng phân công rõ ràng cho 3 thành viên, không sao chép. |
| **TỔNG** | *(Bao quát toàn bộ `CLO1.1` → `CLO4.2`)* | **10,0** | |

---

## 2. Hướng dẫn Cài đặt Môi trường Chi tiết trên Windows (Ai đọc cũng tự làm được)

> **Lưu ý quan trọng**: Cả 3 thành viên đều cài đúng thứ tự **6 bước** dưới đây và **giữ nguyên cổng mặc định** (`27017` cho MongoDB, `6379` cho Memurai) để Solution C# chạy giống nhau trên cả 3 máy.

### Bước 1: Kiểm tra Visual Studio 2022 (.NET Desktop) & Cài đặt Git
1. **Visual Studio 2022 Community**:
   - Mở **Visual Studio Installer** trên máy → kiểm tra đã tích chọn workload **`.NET desktop development`** (hỗ trợ WinForms và `.NET 9.0`). Nếu chưa có thì tải miễn phí tại [visualstudio.microsoft.com](https://visualstudio.microsoft.com/vs/community/).
2. **Cài Git for Windows (để kéo/đẩy code nhóm)**:
   - Vào [git-scm.com/downloads/win](https://git-scm.com/downloads/win), tải bản *64-bit Git for Windows Setup*.
   - Mở file `.exe` → bấm **Next** giữ nguyên mặc định cho tới **Install** → **Finish**.
3. **Kiểm tra thành công**:
   - Mở **PowerShell** hoặc **Command Prompt (CMD)** mới hoàn toàn, gõ:
     ```powershell
     dotnet --version
     git --version
     ```
   - Cả 2 lệnh đều hiện số phiên bản (ví dụ `9.0.xxx` và `git version 2.x.x`) là đạt.

### Bước 2: Cài đặt MongoDB Community Server & MongoDB Compass
1. Truy cập [mongodb.com/try/download/community](https://www.mongodb.com/try/download/community), chọn:
   - **Platform**: `Windows`
   - **Package**: `msi`
   - Bấm **Download**.
2. Chạy file `.msi` vừa tải:
   - Chọn kiểu cài đặt **Complete**.
   - Ở màn hình *Service Configuration*: **Bắt buộc tích chọn** `Install MongoDB as a Service` (và giữ `Run service as Network Service user`) → MongoDB sẽ tự chạy ngầm mỗi khi mở máy.
   - Ở màn hình *Install MongoDB Compass*: **Tích chọn** `Install MongoDB Compass` để cài luôn giao diện quản trị trực quan.
3. **Kiểm tra thành công**:
   - Mở phần mềm **MongoDB Compass** vừa cài.
   - Ô *URI* điền `mongodb://127.0.0.1:27017` → bấm **Connect**.
   - Vào được màn hình thấy các DB mặc định (`admin`, `config`, `local`) và bấm được thanh **`>_ MONGOSH`** ở góc dưới cùng bên trái để gõ lệnh là thành công.

### Bước 3: Cài đặt MongoDB Database Tools (để chạy `mongodump`, `mongorestore`, `mongoexport`, `mongoimport`)
> *Lưu ý*: Bộ cài MongoDB Server ở Bước 2 **không bao gồm sẵn** các lệnh backup/export này, nên bắt buộc cài thêm ở Bước 3 (đặc biệt để chạy chức năng Backup/Restore trên `FrmAdmin` và làm Mục 6 của thang điểm).

1. Truy cập [mongodb.com/try/download/database-tools](https://www.mongodb.com/try/download/database-tools), chọn **Platform**: `Windows x86_64`, **Package**: `msi` → bấm **Download** và chạy cài đặt (bấm Next đến Finish).
2. Kiểm tra đường dẫn thư mục `bin` vừa cài (thường là `C:\Program Files\MongoDB\Tools\100\bin`). Mở File Explorer kiểm tra chắc chắn trong đó có các file `mongodump.exe`, `mongorestore.exe`, `mongoexport.exe`, `mongoimport.exe`.
3. **Thêm thư mục này vào biến môi trường `PATH` của Windows**:
   - Nhấn phím **Windows**, gõ `env`, chọn **Edit the system environment variables**.
   - Bấm nút **Environment Variables...** (góc dưới bên phải).
   - Ở khung **System variables** (phần dưới), tìm dòng **`Path`** → bấm **Edit...**
   - Bấm **New** → dán đường dẫn vừa kiểm tra (ví dụ: `C:\Program Files\MongoDB\Tools\100\bin`) → bấm **OK** → **OK** → **OK**.
4. **Kiểm tra thành công**:
   - Mở một cửa sổ **CMD / PowerShell mới**, gõ:
     ```powershell
     mongodump --version
     mongoexport --version
     ```
   - Hiện phiên bản (không báo lỗi không tìm thấy lệnh) là hoàn tất!

### Bước 4: Cài đặt Memurai Developer Edition (thay thế Redis trên Windows)
1. Truy cập [memurai.com/get-memurai](https://www.memurai.com/get-memurai), tải bản **Memurai Developer Edition** (miễn phí, chọn file `.msi` cho Windows).
2. Mở file `.msi` → chấp nhận điều khoản → chọn cài đặt **Typical / mặc định** (tự động cài làm Windows Service và mở cổng **`6379`**) → bấm **Install** → **Finish**.
3. **Bật chế độ lưu trữ bền vững (AOF + RDB) cho phần Backup/Persistence (Mục 6)**:
   - Mở thư mục cài đặt Memurai (thường là `C:\Program Files\Memurai\`).
   - Mở file `memurai.conf` bằng Notepad (chạy Notepad quyền Administrator nếu không lưu được).
   - Tìm dòng `appendonly` và đảm bảo đặt thành:
     ```text
     appendonly yes
     ```
     *(Snapshot RDB với `save ...` và `dbfilename dump.rdb` thường đã bật sẵn trong file cấu hình).*
   - Mở ứng dụng **Services** của Windows (nhấn phím Windows → gõ `services.msc`) → tìm dịch vụ tên **`Memurai`** → chuột phải chọn **Restart** để áp dụng cấu hình.
4. **Kiểm tra thành công**:
   - Mở CMD / PowerShell mới, gõ:
     ```powershell
     memurai-cli ping
     ```
   - Nếu màn hình trả về **`PONG`** là Memurai đã chạy chuẩn! *(Lưu ý bản Developer miễn phí giới hạn chạy liên tục 10 ngày/lần bật — nếu quá 10 ngày thấy không kết nối được, chỉ cần vào `services.msc` chuột phải `Memurai` chọn **Start** lại là chạy tiếp bình thường).*

### Bước 5: Cài đặt Redis Insight (GUI quản lý Memurai trực quan)
1. Vào [redis.io/insight](https://redis.io/insight/) (hoặc tìm *Redis Insight Download Windows*), tải file cài đặt `.exe` cho Windows và cài đặt như bình thường.
2. Mở **Redis Insight**:
   - Bấm nút **+ Add Redis database** (giống hình minh họa ở slide 73 & 81 Chương 5).
   - Nhập:
     - **Host**: `127.0.0.1`
     - **Port**: `6379`
     - **Database Alias**: `Booking_Memurai` (để trống Username và Password).
   - Bấm **Add Redis Database** rồi bấm vào dòng vừa tạo để mở.
3. **Kiểm tra thành công**: Bấm tab **`>_ CLI`** ở thanh công cụ phía dưới, gõ `PING` và nhấn Enter → trả về `"PONG"` là xong!

### Bước 6: Kiểm tra tổng thể & Xử lý nhanh lỗi hay gặp

**Bảng kiểm tra nhanh trước khi mở Visual Studio chạy dự án** (mở PowerShell mới chạy 4 lệnh sau):
```powershell
dotnet --version      # Hiện 9.0.xxx
git --version         # Hiện git version ...
mongodump --version   # Hiện mongodump version ...
memurai-cli ping      # Trả về PONG
```
Và kiểm tra **2 phần mềm GUI** đều kết nối được:
- **MongoDB Compass** vào được `mongodb://127.0.0.1:27017`
- **Redis Insight** vào được `127.0.0.1:6379`

**3 lỗi hay gặp và cách xử lý trong 10 giây**:
- *Lỗi `'mongodump' is not recognized...` hoặc `'memurai-cli' is not recognized...`*: Quên thêm `bin` vào `PATH` (xem lại Bước 3) hoặc **chưa tắt cửa sổ CMD / Visual Studio cũ để mở lại** sau khi sửa `PATH`.
- *Lỗi không kết nối được `127.0.0.1:27017` hoặc `6379`*: Dịch vụ nền chưa chạy. Nhấn phím Windows → gõ `services.msc` → tìm **`MongoDB Server`** hoặc **`Memurai`** → chuột phải chọn **Start**.
- *Lỗi thiếu thư viện NuGet khi mở Solution lần đầu*: Trong Visual Studio, chuột phải vào **Solution `'RestaurantBooking'`** ở cửa sổ Solution Explorer → chọn **Restore NuGet Packages**.

---

## 3. Cấu trúc Thư mục Dự án WinForms & Phân quyền File (Không đụng code nhau)

Thư mục dự án đặt tại: `c:\HUIT\Semeter1_2026-2027\NOSQL\restaurant-booking\`

```text
restaurant-booking/
├── RestaurantBooking.sln                # [Cố định Ngày 1 - TV1 tạo] Solution Visual Studio 2022
├── db/                                  # Thư mục chứa các file kịch bản truy vấn GUI (nộp kèm đồ án)
│   ├── 01_indexes.js                    # [TV2] Tạo Single/Compound/Unique/Sparse Index & đo explain() trên Compass
│   ├── 02_seed_data.js                  # [TV1] Script dự phòng nạp dữ liệu qua MONGOSH (ngoài DataSeeder.cs)
│   ├── 03_queries_basic.js              # [TV3] 12 câu truy vấn cơ bản chạy trên MongoDB Compass
│   ├── 04_queries_advanced.js           # [TV2] 8-10 câu Aggregation nâng cao chạy trên MongoDB Compass
│   ├── 05_redis_commands.txt            # [TV2 + TV3] Bộ lệnh thực hành trên Redis Insight (Memurai)
│   └── backup/                          # [TV2] Thư mục mặc định lưu file JSON export & thư mục mongodump
└── RestaurantBooking/                   # Project C# WinForms (.NET 9.0 - SDK-style .csproj)
    ├── RestaurantBooking.csproj         # [Cố định Ngày 1] Đã cài sẵn MongoDB.Driver, StackExchange.Redis, BCrypt.Net-Next
    ├── Program.cs                       # [Cố định Ngày 1] Khởi tạo kết nối DB, nạp Seed nếu trống, mở FrmMain
    ├── Config/
    │   ├── DbContext.cs                 # [Cố định Ngày 1] Singleton quản lý MongoClient (27017) & ConnectionMultiplexer (6379)
    │   └── SessionContext.cs            # [Cố định Ngày 1] Lưu User hiện tại + chế độ Dev Fallback (USR001 / ADM001)
    ├── Models/                          # [Cố định Ngày 1 - TV1 tạo sẵn 5 class chuẩn snake_case]
    │   ├── UserModel.cs                 # Ánh xạ collection users
    │   ├── RestaurantModel.cs           # Ánh xạ collection restaurants (nhúng mảng TableItem)
    │   ├── MenuItemModel.cs             # Ánh xạ collection menu_items (nhúng mảng SizeOption, ToppingOption)
    │   ├── BookingOrderModel.cs         # Ánh xạ collection bookings_orders (nhúng CustomerSnapshot, OrderItem)
    │   └── ReviewModel.cs               # Ánh xạ collection reviews (nhúng ReplyInfo)
    ├── Services/
    │   ├── DataSeeder.cs                # [TV1] Tự động sinh 3 nhà hàng, 36 bàn, 30 món, 20 users, 500 đơn, 100 đánh giá + key Memurai
    │   ├── BookingService.cs            # [Độc quyền TV1] Tìm bàn trống, giữ chỗ TTL 15p (SET NX EX 900), chốt đơn, Waitlist (LPUSH/RPOP)
    │   ├── MenuAdminService.cs          # [Độc quyền TV2] CRUD Nhà hàng/Bàn/Món, Cache-Aside + Random TTL, ZSET Ranking, Aggregation
    │   ├── BackupService.cs             # [Độc quyền TV2] Gọi Process mongodump/mongorestore, mongoexport/mongoimport & Memurai BGSAVE
    │   ├── AuthService.cs               # [Độc quyền TV3] Đăng ký, Đăng nhập (BCrypt + lưu Hash session:{token} TTL 24h), Quản lý User
    │   └── CartReviewService.cs         # [Độc quyền TV3] Giỏ hàng trên Memurai Hash (cart:{user_code}) & Đánh giá sao (reviews)
    └── Forms/
        ├── FrmMain.cs                   # [Cố định Ngày 1] Thanh điều hướng mở cả 6 Form con + nút đổi nhanh vai trò Dev Mode
        ├── TV1_Booking/
        │   ├── FrmBooking.cs            # [Độc quyền TV1] Tab 1: Sơ đồ bàn (Button Xanh/Vàng/Đỏ), Timer 15:00, Waitlist, Chốt đơn; Tab 2: Duyệt đơn (Staff/Admin)
        │   └── FrmBooking.Designer.cs   # [Độc quyền TV1]
        ├── TV2_Admin/
        │   ├── FrmAdmin.cs              # [Độc quyền TV2] Tab Quản lý Nhà hàng/Bàn/Menu; Tab Thống kê Aggregation (DataGridView + biểu đồ cột WinForms); Tab Backup/Restore/Export
        │   └── FrmAdmin.Designer.cs     # [Độc quyền TV2]
        └── TV3_User/
            ├── FrmLogin.cs              # [Độc quyền TV3] Đăng nhập / Đăng ký tài khoản
            ├── FrmHome.cs               # [Độc quyền TV3] Chọn chi nhánh, xem Thực đơn (nhãn Cache/MongoDB), Top món hot, chọn Size/Topping vào Giỏ
            ├── FrmHistory.cs            # [Độc quyền TV3] Xem lịch sử đơn đặt, bấm Hủy đơn, gửi Đánh giá sao
            └── FrmUserAdmin.cs          # [Độc quyền TV3] Quản trị danh sách User, đổi Role, Khóa/Mở tài khoản (chỉ hiện với Admin)
```

---

## 4. Quy ước Đặt tên Chuẩn (Chống Conflict 100%)

### 4.1. Tên Database & 5 Collections trong MongoDB (`snake_case`)
- **Tên Database**: `db_booking_nhahang`
- **Mọi tên trường (field) lưu trong MongoDB đều dùng `snake_case`** (trong C# Model gắn `[BsonElement("tên_trường_snake_case")]` để cả MongoDB Compass lẫn code C# đồng nhất 100%):

1. **`users`**:
   - `user_code` (`"USR001"`), `full_name`, `email`, `phone`, `password_hash`, `role` (`"admin"` | `"staff"` | `"customer"`), `addresses` (`[{ label, street, district, city, is_default }]`), `is_active` (`true`/`false`), `created_at`.
2. **`restaurants`**:
   - `restaurant_code` (`"RES01"`), `name`, `district`, `address`, `phone`, `open_time`, `close_time`, `is_active`, `tables` (`[{ table_id: "T01", table_name: "Bàn 01", area: "indoor"|"outdoor"|"vip", capacity: 4, is_available: true }]`).
3. **`menu_items`**:
   - `item_code` (`"MN001"`), `restaurant_code` (`"RES01"`), `name`, `category` (`"Khai vị"` | `"Món chính"` | `"Lẩu & Nướng"` | `"Đồ uống"` | `"Tráng miệng"`), `base_price`, `image_url`, `sizes` (`[{ size_code: "S"|"L", label, extra_price }]`), `toppings` (`[{ topping_code, name, price }]`), `is_available`, `avg_rating`, `created_at`.
4. **`bookings_orders`**:
   - `order_code` (`"ORD20261117001"`), `order_type` (`"dine_in"` | `"takeaway"`), `customer` (`{ user_code, full_name, phone }`), `restaurant_code`, `table_id` (`"T01"` hoặc `null`), `booking_date` (`"YYYY-MM-DD"`), `time_slot` (`"11:00"` | `"13:00"` | `"17:30"` | `"19:00"` | `"20:30"`), `guest_count`, `items` (`[{ item_code, name, size_code, toppings, qty, unit_price, subtotal }]`), `voucher_code` (tùy chọn), `discount_amount`, `total_amount`, `payment_method` (`"cash"` | `"momo"` | `"banking"`), `status` (`"pending"` | `"confirmed"` | `"serving"` | `"completed"` | `"cancelled"`), `created_at`.
5. **`reviews`**:
   - `review_code` (`"REV001"`), `order_code`, `user_code`, `full_name`, `restaurant_code`, `rating` (`1..5`), `comment`, `liked_items` (`["MN001"]`), `reply` (`{ staff_name, content, replied_at }`), `created_at`.

### 4.2. Tên Key chuẩn trên Memurai (`StackExchange.Redis`)

| Mẫu Key | Kiểu dữ liệu | Người dùng | Quy ước giá trị & Lệnh trong `StackExchange.Redis` |
| :--- | :---: | :---: | :--- |
| `lock:table:{restaurant_code}:{table_id}:{booking_date}:{time_slot}` | **String + TTL** | **TV1** | Giá trị = `user_code`. Hàm C#: `db.StringSet(key, userCode, TimeSpan.FromSeconds(900), When.NotExists)` (giữ bàn 15 phút). |
| `waitlist:{restaurant_code}:{booking_date}:{time_slot}` | **List** | **TV1** | Phần tử = JSON `{ user_code, full_name, phone, guest_count, requested_at }`. Hàm C#: `ListLeftPush` (`LPUSH`), `ListRightPop` (`RPOP`). |
| `cache:menu:{restaurant_code}` | **String + TTL** | TV2 | Giá trị = JSON mảng món ăn. TTL = `300 + random(0..60)` giây (chống Cache Avalanche). Hàm C#: `StringGet`, `StringSet`, `KeyDelete`. |
| `ranking:menu_items` | **Sorted Set** | TV1 + TV2 | `member` = `item_code`, `score` = tổng số lượng đã đặt (`SortedSetIncrement`, `SortedSetRangeByRankWithScores`). |
| `ranking:restaurants` | **Sorted Set** | TV1 + TV2 | `member` = `restaurant_code`, `score` = tổng số đơn đặt. |
| `views:restaurant:{restaurant_code}` | **String** | TV2 | Bộ đếm lượt xem chi nhánh (`StringIncrement` / `INCR`). |
| `session:{token}` | **Hash + TTL** | TV3 | Field: `user_code`, `full_name`, `role`, `email`. `KeyExpire` = `86400` giây (24 giờ). |
| `cart:{user_code}` | **Hash** | TV1 + TV3 | Field = `{item_code}_{size_code}`, Value = JSON `{ item_code, name, size_code, toppings, qty, unit_price }` (`HashSet`, `HashGetAll`, `HashDelete`). |

### 4.3. Quy ước Đặt tên Control WinForms & Nhánh Git
- **Tên Control trên Form**: Dùng tiền tố chuẩn WinForms (`btn`, `txt`, `cbo`, `dgv`, `lbl`, `flp`, `tmr`) để dễ đọc sự kiện.
- **Quy tắc bất di bất dịch**: Mỗi thành viên **chỉ mở Designer và sửa các file trong thư mục Form & Service của riêng mình** (`TV1_Booking`, `TV2_Admin`, `TV3_User`). Không tự ý sửa `FrmMain.cs`, `Program.cs` hay `Models/` sau Ngày 1.
- **Nhánh Git**: `main` (nhánh ghép chung), `feat/tv1-booking`, `feat/tv2-menu-admin`, `feat/tv3-auth-ui`.

---

## 5. Trình tự Khởi tạo & Cơ chế Làm việc Độc lập trên WinForms (Không ai phải chờ ai)

### 5.1. Giai đoạn 0 (Ngày 1): Việc TV1 (Nhóm trưởng) làm trước để mở đường cho cả nhóm
Để **TV2** và **TV3** mở Visual Studio lên là bấm `F5` chạy được ngay mà không bị conflict file `.csproj` hay `FrmMain.cs`, **TV1 làm trước 5 việc sau trong Ngày 1** rồi đẩy lên nhánh `main`:
1. **Xin GV duyệt đề tài** và thống nhất bộ quy ước ở Mục 4 với cả nhóm.
2. **Khởi tạo Solution `.NET 9.0 WinForms` & Git**: Tạo `RestaurantBooking.sln`, `.gitignore` (bỏ qua `bin/`, `obj/`, `.vs/`), cài sẵn 3 gói NuGet (`MongoDB.Driver`, `StackExchange.Redis`, `BCrypt.Net-Next`).
3. **Tạo bộ khung dùng chung cố định**: Viết sẵn `DbContext.cs`, `SessionContext.cs` (có sẵn tài khoản mặc định Dev Fallback), 5 class trong `Models/`, `FrmMain.cs` (gắn sẵn các nút mở `FrmBooking`, `FrmAdmin`, `FrmHome`, `FrmHistory`, `FrmLogin`, `FrmUserAdmin`), và tạo sẵn vỏ rỗng của 6 Form cùng 6 file Service trong các thư mục riêng. *(Riêng biểu đồ thống kê trên `FrmAdmin` thống nhất dùng `DataGridView` kết hợp biểu đồ cột tự vẽ bằng `Panel`/`Graphics` chuẩn của WinForms để TV2 không cần cài thêm NuGet gây sửa `.csproj`).*
4. **Viết & chạy thử `Services/DataSeeder.cs`**: Khi chạy ứng dụng lần đầu, nếu DB trống sẽ tự động nạp 5 collections (gồm **500 đơn hàng**) vào MongoDB và các key mẫu (`cart:USR001`, `ranking:*`) vào Memurai.
5. **Push nhánh `main` lên Git**: Sau bước này, TV2 & TV3 pull về, mở `RestaurantBooking.sln` bằng Visual Studio 2022, bấm `F5` là dữ liệu tự động được nạp đầy đủ và tách nhánh làm độc lập 100%.

> **Trong lúc chờ TV1 hoàn thành Giai đoạn 0**: TV2 và TV3 **không cần ngồi chờ** mà thực hiện ngay **Mục 2** (cài 6 bước môi trường trên máy cá nhân), đọc kỹ quy ước ở **Mục 4**, và chuẩn bị trước phần lý thuyết / danh sách truy vấn.

### 5.2. Giai đoạn 1 trở đi: 4 chốt chặn giúp 3 thành viên làm việc độc lập 100%
1. **Project `.NET 9` SDK-style + Tạo sẵn vỏ Form**: Không liệt kê cứng từng file `.cs` trong `.csproj`, và mỗi bạn đã có sẵn thư mục Form riêng (`TV1_Booking`, `TV2_Admin`, `TV3_User`) nên kéo thả giao diện thoải mái không bao giờ đụng file của nhau.
2. **Dữ liệu mẫu tự nạp từ đầu (`DataSeeder.cs`)**: TV2 mở máy là có ngay 500 đơn hàng để làm Index/Aggregation/Thống kê, TV1 có ngay 36 bàn để làm Sơ đồ bàn, TV3 có ngay Users/Menu để làm Trang chủ & Login.
3. **`SessionContext.cs` có sẵn chế độ Dev Fallback**: Khi TV3 chưa code xong `FrmLogin`, trên thanh tiêu đề của `FrmMain` có sẵn nút chuyển nhanh **"Đang test với quyền: Khách (USR001) / Admin (ADM001)"** để TV1 và TV2 vào thẳng `FrmBooking` hoặc `FrmAdmin` test toàn bộ chức năng.
4. **Giao tiếp Giỏ hàng ↔ Đặt bàn qua Key `cart:{user_code}` trên Memurai**: `DataSeeder.cs` nạp sẵn giỏ hàng mẫu cho `cart:USR001` trên Memurai, nên ở `FrmBooking`, TV1 đọc thẳng `cart:USR001` để test chốt đơn kèm món ăn mà không cần đợi TV3 làm xong `FrmHome`.

---

## 6. Luồng Hoạt động Tổng thể của Phần mềm WinForms

1. **Luồng Đăng nhập & Phiên (`TV3` — `FrmLogin`)**: Khách đăng nhập trên `FrmLogin` → `AuthService` đối chiếu `users` trong MongoDB bằng BCrypt → tạo `session:{token}` (`Hash`, TTL 24h) trong Memurai và lưu vào `SessionContext` → phân quyền ẩn/hiện nút Admin trên `FrmMain`.
2. **Luồng Xem Thực đơn Cache-Aside & Giỏ hàng (`TV2` + `TV3` — `FrmHome`)**: Khách mở `FrmHome` chọn chi nhánh → tăng lượt xem `INCR views:restaurant:RES01` (`TV2`) → tải thực đơn từ `cache:menu:RES01` trên Memurai (nếu MISS mới đọc MongoDB `menu_items` rồi nạp cache kèm Random TTL 300–360s — `TV2`, đồng thời hiển thị nhãn *"Nguồn: Memurai Cache (HIT)"* hoặc *"Nguồn: MongoDB (MISS)"* trên Form) → hiển thị Top món bán chạy từ `ranking:menu_items` (`ZSET` — `TV2`) → khách chọn món + Size + Topping lưu vào `cart:{user_code}` (`Hash` trên Memurai — `TV3`).
3. **Luồng Sơ đồ bàn, Giữ chỗ TTL 15 phút & Chốt đơn (`TV1` — `FrmBooking`)**: Khách mở `FrmBooking` chọn Chi nhánh + Ngày + Khung giờ → `BookingService` gộp dữ liệu từ `restaurants.tables` (MongoDB) + `bookings_orders` (MongoDB) + `lock:table:*` (Memurai) để sinh sơ đồ các nút `Button` trên `FlowLayoutPanel` với 3 màu: **Xanh lá (Trống) / Vàng cam (Đang giữ + `Timer` đếm ngược giây TTL thực tế từ lệnh `KeyTimeToLive`) / Đỏ (Đã đặt)** → khách bấm chọn bàn trống sẽ gọi `StringSet(..., When.NotExists, TimeSpan.FromSeconds(900))` khóa bàn 15 phút chống trùng → nếu hết bàn thì bấm đăng ký vào `waitlist:...` (`List`) → bấm **Chốt đơn** sẽ đọc món từ `cart:{user_code}`, lưu đơn vào `bookings_orders` (MongoDB), xóa khóa `lock`, xóa `cart`, và cộng điểm `SortedSetIncrement` (`ZINCRBY`) vào `ranking:menu_items` & `ranking:restaurants`.
4. **Luồng Lịch sử, Hủy đơn nhường bàn Waitlist & Đánh giá (`TV3` + `TV1` — `FrmHistory`)**: Khách xem đơn đã đặt ở `FrmHistory` (`TV3`) → nếu bấm Hủy đơn (`TV1`), hệ thống cập nhật trạng thái `"cancelled"` và gọi `ListRightPop` (`RPOP waitlist:...`) lấy khách chờ đầu tiên ra thông báo trên màn hình → khi đơn hoàn tất (`"completed"`), khách gửi đánh giá sao và nhận xét vào `reviews` (`TV3`).
5. **Luồng Quản trị Admin, Thống kê Aggregation & Backup/Restore (`TV1` + `TV2` + `TV3` — tách Form riêng chống conflict)**:
   - **Quản trị người dùng (`TV3` — `FrmUserAdmin`)**: Admin xem danh sách user, đổi quyền `admin`/`staff`/`customer`, khóa/mở tài khoản.
   - **Duyệt trạng thái đơn đặt (`TV1` — Tab Quản lý trong `FrmBooking`)**: Nhân viên/Admin lọc đơn theo ngày/chi nhánh và cập nhật trạng thái (`pending` → `confirmed` → `serving` → `completed` / `cancelled`).
   - **Quản trị Nhà hàng/Menu, Thống kê & Backup (`TV2` — `FrmAdmin`)**: Thêm/Sửa/Xóa Nhà hàng, Bàn & Món ăn kèm xóa cache tự động `KeyDelete("cache:menu:*")`, xem Tab Thống kê doanh thu/top món/giờ cao điểm/khách VIP từ MongoDB Aggregation (`DataGridView` + biểu đồ cột WinForms), và dùng Tab **Sao lưu / Phục hồi / Xuất-Nhập JSON**.

---

## 7. Phân công Chi tiết cho 3 Thành viên

### 👤 THÀNH VIÊN 1 (Nhóm trưởng): Khởi tạo Solution, Kiến trúc DB, Core Booking, Giữ chỗ TTL & Waitlist
- **Khởi tạo khung chung (Giai đoạn 0 - Ngày 1)**: Xin GV duyệt đề tài; tạo `RestaurantBooking.sln` (.NET 9 WinForms), cài NuGet, tạo `DbContext.cs`, `SessionContext.cs`, 5 class `Models/`, `FrmMain.cs` và vỏ rỗng các Form/Service rồi push lên nhánh `main` để TV2 & TV3 pull về làm song song.
- **Database & Dữ liệu (`Services/DataSeeder.cs`, `db/02_seed_data.js`)**: Thiết kế cấu trúc 5 collections + 8 mẫu key Memurai; viết code tự động sinh dữ liệu mẫu lớn (3 nhà hàng, 36 bàn, 30 món, 20 users, 500 đơn đặt, 100 đánh giá).
- **Truy vấn GUI**: Chạy & chụp ảnh các truy vấn kiểm tra bàn trống, đơn đặt theo khung giờ trên **MongoDB Compass** và kiểm tra `lock:table:*`, `TTL`, `waitlist:*` trên **Redis Insight**.
- **Code Nghiệp vụ & Giao diện (`Services/BookingService.cs`, `Forms/TV1_Booking/FrmBooking.cs`)**:
  - Hiển thị sơ đồ bàn động trên `FlowLayoutPanel` theo trạng thái thực tế (MongoDB + Memurai lock).
  - Giữ bàn tạm thời 15 phút (`SET ... NX EX 900`), dùng `System.Windows.Forms.Timer` (1 giây) cập nhật đồng hồ đếm ngược TTL ngay trên nút bàn & nhả bàn khi hủy chọn.
  - Đọc giỏ hàng từ Memurai, tính tiền, tạo đơn đặt bàn/mang về + cập nhật điểm xếp hạng `SortedSetIncrement`.
  - Đăng ký hàng đợi `waitlist` (`LPUSH`) khi hết bàn và tự động rút khách đầu hàng đợi (`RPOP`) khi có đơn hủy.
  - Chức năng chuyển trạng thái đơn (`pending` → `confirmed` → `serving` → `completed` / `cancelled`).
- **Báo cáo Word**: Viết **Chương 2** (Phân tích & Thiết kế DB, Dữ liệu mẫu), phần **Kết nối CSDL & Nghiệp vụ Booking** trong **Chương 4**, và **Chương 5 (Kết luận)**.

### 👤 THÀNH VIÊN 2: Truy vấn Nâng cao, Index, Cache-Aside, Thống kê & Backup/Restore
- **Index & Truy vấn nâng cao (`db/01_indexes.js`, `db/04_queries_advanced.js`, `db/05_redis_commands.txt`)**:
  - Tạo đủ 4 loại Index (Single, Compound, Unique, Sparse) và chụp kết quả `.explain("executionStats")` so sánh trước – sau khi đánh Index trên MongoDB Compass.
  - Viết 8–10 câu **Aggregation Pipeline** (doanh thu theo tháng/chi nhánh, `$unwind` tách mảng món tìm top doanh thu, khung giờ cao điểm, khách VIP `$addToSet`, `$lookup` nối đơn với đánh giá, `$project/$multiply`).
  - Thực hiện & chụp ảnh **Import/Export (`mongoexport`/`mongoimport`)** và **Backup/Restore** cho cả 2 hệ: MongoDB (`mongodump`/`mongorestore`) và Memurai (`BGSAVE` tạo `dump.rdb`, `appendonly.aof`, sao lưu/khôi phục file và restart service `Memurai` để kiểm chứng dữ liệu phục hồi trên Redis Insight).
- **Code Nghiệp vụ & Giao diện (`Services/MenuAdminService.cs`, `Services/BackupService.cs`, `Forms/TV2_Admin/FrmAdmin.cs`)**:
  - Hàm lấy danh sách nhà hàng (kèm `INCR views:restaurant:*`) và thực đơn áp dụng **Cache-Aside + Random TTL** (chống *Cache Avalanche* & *Cache Penetration*).
  - CRUD Nhà hàng, Bàn, Món ăn trên `DataGridView` (tự động xóa cache `DEL cache:menu:*` khi thêm/sửa/xóa).
  - Lấy Bảng xếp hạng Top món ăn / nhà hàng từ Memurai `Sorted Set` (`ZREVRANGE ... WITHSCORES`).
  - Hiển thị bảng `DataGridView` và biểu đồ cột WinForms (`Panel`/`Graphics` chuẩn, không cần cài thêm NuGet) cho các câu Thống kê Aggregation trên Tab Dashboard.
  - Các nút bấm **Backup (`mongodump` + Memurai `BGSAVE`) / Restore (`mongorestore`) / Export & Import JSON** ngay trên `FrmAdmin`.
- **Báo cáo Word & Slide**: Viết mục **Import/Export & Backup/Restore** (cuối Chương 2), toàn bộ **Chương 3 (Truy vấn cơ bản/nâng cao & Index Explain)**, phần **Thống kê, Cache & Backup** trong **Chương 4**, và làm **Slide PowerPoint**.

### 👤 THÀNH VIÊN 3: Lý thuyết NoSQL, Trang chủ Thực đơn, Quản trị Người dùng, Giỏ hàng & Đánh giá
- **Lý thuyết & Truy vấn cơ bản (`db/03_queries_basic.js`)**:
  - Viết 12 câu truy vấn cơ bản trên **MongoDB Compass** (CRUD, `$in`, `$gt/$lte`, regex, `$elemMatch`, projection, `sort/limit/skip`, `$set/$push/$addToSet/$pull`) + chụp ảnh thao tác `Hash` (`session:*`, `cart:*`) trên **Redis Insight**.
- **Code Nghiệp vụ & Giao diện (`Services/AuthService.cs`, `Services/CartReviewService.cs`, `Forms/TV3_User/FrmLogin.cs`, `FrmHome.cs`, `FrmHistory.cs`, `FrmUserAdmin.cs`)**:
  - `FrmLogin`: Đăng ký, Đăng nhập (`BCrypt` + lưu `session:{token}` trên Memurai `Hash` có TTL 24h), Đăng xuất (`KeyDelete`).
  - `FrmUserAdmin`: Quản trị danh sách người dùng (lọc/tìm kiếm, đổi role `admin`/`staff`/`customer`, khóa/mở tài khoản).
  - `FrmHome`: Hiển thị Chi nhánh, Thực đơn (kèm nhãn báo dữ liệu đọc từ *Memurai Cache* hay *MongoDB*), Top món bán chạy, chọn Size/Topping thêm vào Giỏ hàng Memurai `Hash` (`cart:{user_code}`), tăng/giảm số lượng, xóa món.
  - `FrmHistory`: Xem lịch sử đơn đặt của khách, gửi đánh giá sao (`reviews`), tính lại `avg_rating` cho món ăn và cho phép nhân viên phản hồi đánh giá.
- **Báo cáo Word**: Viết toàn bộ **Chương 1 (Cơ sở lý thuyết NoSQL, MongoDB, Redis/Memurai, Compass, Redis Insight)**, phần **Giao diện Trang chủ, Quản trị User (`FrmUserAdmin`), Giỏ hàng & Đánh giá** trong **Chương 4**, căn chỉnh định dạng toàn quyển **50–60 trang (Times New Roman 13, 1.5 line)** và lập **Bảng phân công công việc**.

---

## 8. Cấu trúc Quyển Báo cáo Word Chuẩn (50 – 60 trang) & Checklist Nộp bài

- **Phần mở đầu (4 – 5 trang)**: Trang bìa, Lời cảm ơn, Nhận xét của GV, Mục lục tự động, Danh mục hình ảnh/bảng biểu, **Bảng phân công công việc & tỷ lệ đóng góp của 3 thành viên**.
- **Chương 1: Cơ sở lý thuyết về NoSQL, MongoDB và Redis/Memurai (11 – 13 trang)** *(TV3 — phủ trọn Mục 1–4 thang điểm)*:
  - 1.1. Tổng quan về CSDL NoSQL (khái niệm, lịch sử, đặc trưng, ưu/nhược điểm, 4 phân loại, so sánh với RDBMS).
  - 1.2. Hệ quản trị CSDL MongoDB và công cụ MongoDB Compass (cài đặt, kiến trúc BSON/Document, chức năng chi tiết của Compass, so sánh điểm mạnh/yếu giữa **MongoDB Compass** và **Studio 3T**).
  - 1.3. Cơ sở dữ liệu In-Memory Redis (Memurai trên Windows) và công cụ Redis Insight (cài đặt, các kiểu dữ liệu String, Hash, List, Set, Sorted Set, TTL, cơ chế bền vững RDB & AOF, chức năng Redis Insight và so sánh với **Another Redis Desktop Manager**).
  - 1.4. Nêu bật điểm mạnh của MongoDB (Document) và Memurai/Redis (In-Memory Key-Value) đối với từng loại ứng dụng CSDL phổ biến.
  - 1.5. Lý do chọn kiến trúc đa mô hình MongoDB + Memurai cho bài toán Booking nhà hàng & đặt món ăn.
- **Chương 2: Phân tích bài toán & Thiết kế Cơ sở dữ liệu (13 – 15 trang)** *(TV1 + TV2 mục 2.6)*:
  - 2.1. Mô tả bài toán Đặt bàn nhà hàng & Đặt món ăn trực tuyến (các tác nhân & yêu cầu chức năng).
  - 2.2. Xác định tập yêu cầu truy vấn nghiệp vụ (`Q1..Qn`).
  - 2.3. Thiết kế 5 Collections trên MongoDB (chi tiết cấu trúc JSON, giải thích lý do chọn Embedded hay Reference cho từng bảng) và so sánh với mô hình quan hệ RDBMS.
  - 2.4. Thiết kế hệ thống 8 mẫu Key trên Memurai (Redis) phục vụ giữ chỗ thời gian thực, giỏ hàng, hàng đợi và xếp hạng.
  - 2.5. Xây dựng CSDL, thiết lập Index và nạp bộ dữ liệu mẫu (`DataSeeder.cs` / `02_seed_data.js`).
  - 2.6. Triển khai Import/Export (`mongoexport`/`mongoimport`) và Backup/Restore (`mongodump`/`mongorestore`, Memurai RDB/AOF).
- **Chương 3: Triển khai Truy vấn dữ liệu trên GUI Tool (10 – 12 trang)** *(TV2 + TV3)*:
  - 3.1. Nhóm truy vấn dữ liệu cơ bản trên MongoDB Compass (12 câu kèm giải thích cú pháp và ảnh chụp kết quả).
  - 3.2. Nhóm truy vấn dữ liệu nâng cao với Aggregation Pipeline trên MongoDB Compass (8–10 câu kèm ảnh chụp kết quả).
  - 3.3. Đánh giá và tối ưu hóa hiệu năng truy vấn bằng Index (`.explain("executionStats")` trước và sau khi tạo Index).
  - 3.4. Triển khai các lệnh thao tác và truy vấn cấu trúc dữ liệu trên Redis Insight (Memurai).
- **Chương 4: Xây dựng và Demo Ứng dụng C# WinForms (10 – 12 trang)** *(Cả 3 thành viên)*:
  - 4.1. Kiến trúc ứng dụng C# WinForms (.NET 9) kết nối đồng thời MongoDB (`MongoDB.Driver`) và Memurai (`StackExchange.Redis`).
  - 4.2. Phân hệ Đăng nhập/Session TTL (`FrmLogin`) và Quản trị người dùng (`FrmUserAdmin`) (`TV3`).
  - 4.3. Phân hệ Trang chủ, Thực đơn Cache-Aside, Bảng xếp hạng Sorted Set (`FrmHome`) và Lịch sử/Đánh giá (`FrmHistory`) (`TV2` & `TV3`).
  - 4.4. Phân hệ Sơ đồ bàn trực quan, Giữ chỗ tạm thời TTL 15 phút, Hàng đợi Waitlist, Chốt đơn và Duyệt đơn trên `FrmBooking` (`TV1`).
  - 4.5. Phân hệ Quản trị Nhà hàng/Menu, Thống kê Aggregation Dashboard và Sao lưu/Phục hồi trực tiếp trên `FrmAdmin` (`TV2`).
- **Chương 5: Kết luận và Tài liệu tham khảo (2 – 3 trang)** *(TV1)*:
  - 5.1. Kết quả đạt được (đối chiếu với 10 Chuẩn đầu ra `CLO1.1` – `CLO4.2`).
  - 5.2. Hạn chế và hướng phát triển.
  - 5.3. Danh mục tài liệu tham khảo.

### Checklist Hồ sơ Nộp Cuối kỳ (Đủ điều kiện chấm Mục 10)
- [ ] **File Báo cáo Word (`.docx`)**: 50–60 trang, Times New Roman size 13, giãn dòng 1.5 line, có đủ bảng phân công nhóm & ảnh chụp chạy truy vấn trên MongoDB Compass / Redis Insight / App WinForms.
- [ ] **File Trình chiếu PowerPoint (`.pptx`)**: Tóm tắt kiến trúc MongoDB + Memurai, demo kết quả truy vấn & tính năng chính.
- [ ] **Source code C# WinForms**: Toàn bộ thư mục dự án `restaurant-booking/` (mở và chạy được bằng `RestaurantBooking.sln`).
- [ ] **Bộ script CSDL & Backup mẫu (`db/`)**: Gồm `01_indexes.js`, `02_seed_data.js`, `03_queries_basic.js`, `04_queries_advanced.js`, `05_redis_commands.txt` và dữ liệu export/dump mẫu trong `db/backup/`.
