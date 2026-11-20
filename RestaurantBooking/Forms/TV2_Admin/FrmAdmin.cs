using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using RestaurantBooking.Config;
using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV2_Admin;

/// <summary>
/// [Độc quyền TV2 - Giai đoạn 1]
/// Quản trị Nhà hàng/Bàn/Thực đơn (xóa cache Memurai),
/// Thống kê Aggregation + Top Sorted Set (DataGridView & Biểu đồ cột Graphics),
/// và Backup/Restore/Export-Import (MongoDB Database Tools & Memurai RDB/AOF).
/// </summary>
public partial class FrmAdmin : Form
{
    private readonly MenuAdminService _menuService = new();
    private readonly BackupService _backupService = new();
    private readonly ToolTip _toolTip = new();

    private List<RestaurantModel> _restaurants = [];
    private List<MenuItemModel> _menuItems = [];
    private RestaurantModel? _selectedRestaurant;
    private bool _isBinding;

    private readonly List<ChartBarPoint> _chartPoints = [];
    private string _chartTitle = "Biểu đồ thống kê";
    private string _chartSubtitle = string.Empty;
    private Color _chartAccentColor = Color.FromArgb(25, 118, 210);

    public FrmAdmin()
    {
        InitializeComponent();

        typeof(Control)
            .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(pnlBarChart, true);

        WireEvents();
        InitializeStaticDropdowns();
        ConfigureFriendlyTooltips();
        ConfigureUnifiedNavigationAndTabs();
    }

    private void ConfigureUnifiedNavigationAndTabs()
    {
        lblInfo.BackColor = UiTheme.HeaderBg;
        lblInfo.UseMnemonic = false;
        lblInfo.Height = 46;
        lblInfo.Padding = new Padding(14, 12, 148, 8);

        var btnBack = new Button
        {
            Name = "btnAdminBack",
            Text = "← Quay lại",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Size = new Size(120, 32),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(lblInfo.Width - 134, 7)
        };
        UiTheme.StyleHeaderBackButton(btnBack);
        btnBack.Click += (_, _) => Close();
        lblInfo.Controls.Add(btnBack);

        UiTheme.StyleTabControl(tabAdmin);
        foreach (var grid in new[] { dgvRestaurants, dgvTables, dgvMenuItems, dgvStats })
        {
            UiTheme.StyleGrid(grid);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(2, 0, 2, 0);
        }
    }

    private void WireEvents()
    {
        Load += FrmAdmin_Load;
        Shown += (_, _) => AdjustSplittersSafely();
        FormClosed += (_, _) => _toolTip.Dispose();
        Resize += (_, _) => pnlBarChart.Invalidate();
        pnlBarChart.Paint += PnlBarChart_Paint;
        pnlBarChart.Resize += (_, _) => pnlBarChart.Invalidate();

        // Tab 1: Restaurants & Tables
        dgvRestaurants.SelectionChanged += (_, _) => OnRestaurantSelected();
        dgvRestaurants.CellClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OnRestaurantSelected(e.RowIndex);
        };
        dgvRestaurants.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OnRestaurantSelected(e.RowIndex);
        };
        btnResNew.Click += async (_, _) => await PrepareNewRestaurantAsync();
        btnResAdd.Click += async (_, _) => await SaveRestaurantFromFormAsync(isNew: true);
        btnResUpdate.Click += async (_, _) => await SaveRestaurantFromFormAsync(isNew: false);
        btnResDelete.Click += async (_, _) => await DeleteSelectedRestaurantAsync();

        dgvTables.SelectionChanged += (_, _) => OnTableSelected();
        dgvTables.CellClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OnTableSelected(e.RowIndex);
        };
        dgvTables.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OnTableSelected(e.RowIndex);
        };
        btnTableNew.Click += (_, _) => PrepareNewTable();
        btnTableAdd.Click += async (_, _) => await SaveTableFromFormAsync(isNew: true);
        btnTableUpdate.Click += async (_, _) => await SaveTableFromFormAsync(isNew: false);
        btnTableDelete.Click += async (_, _) => await DeleteSelectedTableAsync();

        // Tab 1: Menu Items
        btnMenuFilter.Click += async (_, _) => await LoadMenuGridAsync();
        cboMenuFilterRes.SelectedIndexChanged += async (_, _) =>
        {
            if (!_isBinding) await LoadMenuGridAsync();
        };
        cboMenuFilterCat.SelectedIndexChanged += async (_, _) =>
        {
            if (!_isBinding) await LoadMenuGridAsync();
        };
        chkMenuOnlyAvail.CheckedChanged += async (_, _) =>
        {
            if (!_isBinding) await LoadMenuGridAsync();
        };
        txtMenuKeyword.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LoadMenuGridAsync();
            }
        };
        dgvMenuItems.SelectionChanged += (_, _) => OnMenuItemSelected();
        dgvMenuItems.CellClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OnMenuItemSelected(e.RowIndex);
        };
        dgvMenuItems.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) OnMenuItemSelected(e.RowIndex);
        };
        btnMenuNew.Click += async (_, _) => await PrepareNewMenuItemAsync();
        btnMenuAdd.Click += async (_, _) => await SaveMenuItemFromFormAsync(isNew: true);
        btnMenuUpdate.Click += async (_, _) => await SaveMenuItemFromFormAsync(isNew: false);
        btnMenuDelete.Click += async (_, _) => await DeleteSelectedMenuItemAsync();

        // Tab 2: Statistics
        btnRunStat.Click += async (_, _) => await RunSelectedStatisticAsync();
        cboStatQuery.SelectedIndexChanged += async (_, _) =>
        {
            if (!_isBinding) await RunSelectedStatisticAsync();
        };
        cboStatBranch.SelectedIndexChanged += async (_, _) =>
        {
            if (!_isBinding) await RunSelectedStatisticAsync();
        };
        tabAdmin.SelectedIndexChanged += async (_, _) =>
        {
            if (tabAdmin.SelectedTab == tabRestaurantMenu)
                await LoadTab1DataAsync(_selectedRestaurant?.RestaurantCode);
            else if (tabAdmin.SelectedTab == tabStatistics)
                await RunSelectedStatisticAsync();
            else if (tabAdmin.SelectedTab == tabBackup)
                await RefreshMemuraiPersistenceStatusAsync(logToConsole: false);
        };

        // Tab 3: Backup & Restore
        btnBrowseBackupFolder.Click += (_, _) => BrowseBackupFolder();
        btnOpenBackupFolder.Click += (_, _) => OpenBackupFolder();
        btnBrowseImportFile.Click += (_, _) => BrowseImportJsonFile();
        btnMongoDump.Click += async (_, _) => await ExecuteMongoDumpAsync();
        btnMongoRestore.Click += async (_, _) => await ExecuteMongoRestoreAsync();
        btnExportAllJson.Click += async (_, _) => await ExecuteExportAllJsonAsync();
        btnImportJson.Click += async (_, _) => await ExecuteImportJsonAsync();
        btnMemuraiBgSave.Click += async (_, _) => await ExecuteMemuraiBgSaveAsync();
        btnMemuraiBackupFiles.Click += async (_, _) => await ExecuteBackupMemuraiFilesAsync();
        btnMemuraiStatus.Click += async (_, _) => await RefreshMemuraiPersistenceStatusAsync(logToConsole: true);
    }

    private void InitializeStaticDropdowns()
    {
        var areaChoices = new List<ComboOption>
        {
            new("indoor", "Trong nhà"),
            new("outdoor", "Ngoài trời"),
            new("vip", "Phòng VIP")
        };
        BindCombo(cboTableArea, areaChoices, "indoor");

        cboMenuFilterCat.Items.Clear();
        cboMenuFilterCat.Items.AddRange(["(Tất cả nhóm)", "Khai vị", "Món chính", "Lẩu & Nướng", "Đồ uống", "Tráng miệng"]);
        cboMenuFilterCat.SelectedIndex = 0;

        cboStatQuery.Items.Clear();
        cboStatQuery.Items.AddRange([
            "1. Doanh thu theo tháng và chi nhánh",
            "2. Top 10 món ăn có doanh thu cao nhất",
            "3. Thống kê khung giờ cao điểm (tại chỗ và mang về)",
            "4. Top khách hàng thân thiết (VIP)",
            "5. Thống kê đánh giá và phản hồi theo chi nhánh",
            "6. Top món ăn được đặt nhiều nhất",
            "7. Xếp hạng chi nhánh theo số đơn và lượt xem"
        ]);
        cboStatQuery.SelectedIndex = 0;

        var tableChoices = new List<ComboOption>
        {
            new("bookings_orders", "Đơn đặt bàn và món (bookings_orders)"),
            new("menu_items", "Thực đơn món ăn (menu_items)"),
            new("restaurants", "Nhà hàng và bàn (restaurants)"),
            new("users", "Người dùng (users)"),
            new("reviews", "Đánh giá khách hàng (reviews)")
        };
        BindCombo(cboExportCollection, tableChoices, "bookings_orders");

        txtBackupFolder.Text = BackupService.GetDefaultBackupRoot();
    }

    private void ConfigureFriendlyTooltips()
    {
        _toolTip.SetToolTip(txtResPhone, "Nhập 9 đến 11 chữ số bắt đầu bằng 0 (ví dụ: 02838123401 hoặc 0901234567).");
        _toolTip.SetToolTip(txtResOpen, "Giờ mở cửa định dạng HH:mm (ví dụ: 09:00), phải trước giờ đóng cửa.");
        _toolTip.SetToolTip(txtResClose, "Giờ đóng cửa định dạng HH:mm (ví dụ: 22:30), phải sau giờ mở cửa.");
        _toolTip.SetToolTip(numTableCapacity, $"Số ghế của bàn phải lớn hơn 0 và nhỏ hơn hoặc bằng {MenuAdminService.MaxTableCapacity}.");
        _toolTip.SetToolTip(numItemBasePrice, $"Giá gốc hợp lệ từ {MenuAdminService.MinMenuBasePrice:N0} đ đến {MenuAdminService.MaxMenuBasePrice:N0} đ.");
        _toolTip.SetToolTip(numItemSizeLargeExtra, $"Giá cộng thêm size L hợp lệ từ 0 đ đến {MenuAdminService.MaxMenuExtraPrice:N0} đ.");
        _toolTip.SetToolTip(txtItemToppings, $"Tối đa {MenuAdminService.MaxToppingsPerItem} món kèm, định dạng 'Tên:Giá; Tên khác:Giá' (giá từ 0 đến {MenuAdminService.MaxMenuExtraPrice:N0} đ).");
        _toolTip.SetToolTip(btnMongoDump, "Chạy mongodump sao lưu 5 bảng dữ liệu chính sang định dạng nhị phân.");
        _toolTip.SetToolTip(btnMongoRestore, "Chạy mongorestore --drop để khôi phục dữ liệu chính từ thư mục đã sao lưu.");
        _toolTip.SetToolTip(btnExportAllJson, "Chạy mongoexport xuất 5 bảng dữ liệu ra 5 tập tin .json dễ đọc.");
        _toolTip.SetToolTip(btnImportJson, "Chạy mongoimport nhập dữ liệu từ tập tin .json vào bảng đang chọn.");
        _toolTip.SetToolTip(btnMemuraiBgSave, "Gửi lệnh BGSAVE để Memurai lưu nhanh dữ liệu bộ nhớ tạm xuống ổ cứng (dump.rdb).");
        _toolTip.SetToolTip(btnMemuraiBackupFiles, "Sao chép tập tin dump.rdb và thư mục appendonlydir ra thư mục dự phòng.");
    }

    private void AdjustSplittersSafely()
    {
        try
        {
            if (splitMain.Width >= 900)
            {
                int desiredLeft = Math.Max((int)(splitMain.Width * 0.44), btnTableDelete.Right + 20);
                splitMain.Panel1MinSize = 360;
                splitMain.Panel2MinSize = 420;
                splitMain.SplitterDistance = Math.Clamp(desiredLeft, 360, splitMain.Width - 420);
            }

            if (splitLeft.Height >= 380)
            {
                splitLeft.Panel1MinSize = 180;
                splitLeft.Panel2MinSize = 160;
                splitLeft.SplitterDistance = Math.Clamp((int)(splitLeft.Height * 0.54), 180, splitLeft.Height - 160);
            }

            if (splitStats.Width >= 680)
            {
                splitStats.Panel1MinSize = 320;
                splitStats.Panel2MinSize = 320;
                splitStats.SplitterDistance = Math.Clamp((int)(splitStats.Width * 0.52), 320, splitStats.Width - 320);
            }
        }
        catch
        {
            // Bỏ qua nếu cửa sổ đang thu nhỏ quá mức.
        }
    }

    private async void FrmAdmin_Load(object? sender, EventArgs e)
    {
        lblInfo.Text = $"Quản trị hệ thống nhà hàng — Quản trị viên: {SessionContext.FullName} ({SessionContext.UserCode})";
        await LoadTab1DataAsync();
        await RefreshKpiSummaryAsync();
        await RunSelectedStatisticAsync();
        await RefreshMemuraiPersistenceStatusAsync(logToConsole: false);
    }

    #region Tab 1: Restaurants, Tables & Menu Management

    private async Task LoadTab1DataAsync(string? selectRestaurantCode = null)
    {
        try
        {
            _isBinding = true;
            _restaurants = await _menuService.GetRestaurantsAsync();
            var viewsMap = await _menuService.GetRestaurantViewsMapAsync(_restaurants.Select(r => r.RestaurantCode));

            var resRows = _restaurants.Select(r => new
            {
                Mã = r.RestaurantCode,
                Tên_chi_nhánh = r.Name,
                Quận = r.District,
                Bàn = r.Tables.Count,
                Lượt_xem = viewsMap.GetValueOrDefault(r.RestaurantCode, 0),
                Hoạt_động = r.IsActive ? "Có" : "Ngưng"
            }).ToList();

            dgvRestaurants.DataSource = resRows;
            FormatRestaurantGridHeaders();

            BindRestaurantDropdowns();

            var targetCode = selectRestaurantCode
                ?? _selectedRestaurant?.RestaurantCode
                ?? _restaurants.FirstOrDefault()?.RestaurantCode;

            if (!string.IsNullOrEmpty(targetCode))
                SelectRestaurantRow(targetCode);
        }
        catch (Exception ex)
        {
            ShowError("Không tải được dữ liệu danh mục", ex);
        }
        finally
        {
            _isBinding = false;
        }

        OnRestaurantSelected();
        await LoadMenuGridAsync();
    }

    private void BindRestaurantDropdowns()
    {
        var prevFilter = cboMenuFilterRes.SelectedValue as string;
        var prevItemRes = cboItemRestaurant.SelectedValue as string;
        var prevStatRes = cboStatBranch.SelectedValue as string;

        var branchChoices = _restaurants
            .Select(r => new ComboOption(r.RestaurantCode, $"{r.RestaurantCode} - {r.Name}"))
            .ToList();

        var withAll = new List<ComboOption> { new("", "(Tất cả chi nhánh)") };
        withAll.AddRange(branchChoices);

        BindCombo(cboItemRestaurant, branchChoices, prevItemRes);
        BindCombo(cboMenuFilterRes, withAll, prevFilter);
        BindCombo(cboStatBranch, withAll, prevStatRes);
    }

    private static void BindCombo(ComboBox combo, List<ComboOption> items, string? previousValue)
    {
        combo.DisplayMember = nameof(ComboOption.Label);
        combo.ValueMember = nameof(ComboOption.Value);
        combo.DataSource = items.ToList();

        if (previousValue != null && items.Any(x => x.Value == previousValue))
            combo.SelectedValue = previousValue;
        else if (items.Count > 0)
            combo.SelectedIndex = 0;
    }

    private void SelectRestaurantRow(string restaurantCode)
    {
        foreach (DataGridViewRow row in dgvRestaurants.Rows)
        {
            if (string.Equals(row.Cells[0].Value?.ToString(), restaurantCode, StringComparison.OrdinalIgnoreCase))
            {
                row.Selected = true;
                dgvRestaurants.CurrentCell = row.Cells[0];
                break;
            }
        }
    }

    private void OnRestaurantSelected(int? rowIndex = null)
    {
        var idx = rowIndex ?? dgvRestaurants.CurrentRow?.Index ?? -1;
        if (idx < 0 || idx >= dgvRestaurants.Rows.Count)
        {
            _selectedRestaurant = null;
            dgvTables.DataSource = null;
            ResetTableForm();
            return;
        }

        var code = dgvRestaurants.Rows[idx].Cells[0].Value?.ToString();
        _selectedRestaurant = _restaurants.FirstOrDefault(r =>
            string.Equals(r.RestaurantCode, code, StringComparison.OrdinalIgnoreCase));

        if (_selectedRestaurant == null)
        {
            ResetTableForm();
            return;
        }

        txtResCode.Text = _selectedRestaurant.RestaurantCode;
        txtResName.Text = _selectedRestaurant.Name;
        txtResDistrict.Text = _selectedRestaurant.District;
        txtResAddress.Text = _selectedRestaurant.Address;
        txtResPhone.Text = _selectedRestaurant.Phone;
        txtResOpen.Text = _selectedRestaurant.OpenTime;
        txtResClose.Text = _selectedRestaurant.CloseTime;
        chkResActive.Checked = _selectedRestaurant.IsActive;

        var tableRows = _selectedRestaurant.Tables
            .OrderBy(t => t.TableId)
            .Select(t => new
            {
                Mã_bàn = t.TableId,
                Tên_bàn = t.TableName,
                Khu_vực = FormatAreaDisplay(t.Area),
                Số_ghế = t.Capacity,
                Sẵn_sàng = t.IsAvailable ? "Sẵn sàng" : "Đang khóa"
            }).ToList();

        dgvTables.DataSource = tableRows;
        FormatTableGridHeaders();
        if (dgvTables.Rows.Count > 0)
            OnTableSelected(0);
        else
            ResetTableForm();
    }

    private void OnTableSelected(int? rowIndex = null)
    {
        var idx = rowIndex ?? dgvTables.CurrentRow?.Index ?? -1;
        if (_selectedRestaurant == null || idx < 0 || idx >= dgvTables.Rows.Count)
        {
            ResetTableForm();
            return;
        }

        var tableId = dgvTables.Rows[idx].Cells[0].Value?.ToString();
        var table = _selectedRestaurant.Tables.FirstOrDefault(t =>
            string.Equals(t.TableId, tableId, StringComparison.OrdinalIgnoreCase));
        if (table == null) return;

        txtTableId.Text = table.TableId;
        txtTableName.Text = table.TableName;
        var areaCode = table.Area is "indoor" or "outdoor" or "vip" ? table.Area : "indoor";
        cboTableArea.SelectedValue = areaCode;
        numTableCapacity.Value = Math.Clamp(table.Capacity, (int)numTableCapacity.Minimum, (int)numTableCapacity.Maximum);
        chkTableAvailable.Checked = table.IsAvailable;
    }

    private void ResetTableForm()
    {
        txtTableId.Clear();
        txtTableName.Clear();
        cboTableArea.SelectedValue = "indoor";
        numTableCapacity.Value = 4;
        chkTableAvailable.Checked = true;
    }

    private async Task PrepareNewRestaurantAsync()
    {
        try
        {
            txtResCode.Text = await _menuService.GenerateNextRestaurantCodeAsync();
            txtResName.Clear();
            txtResDistrict.Clear();
            txtResAddress.Clear();
            txtResPhone.Clear();
            txtResOpen.Text = "09:00";
            txtResClose.Text = "22:30";
            chkResActive.Checked = true;
            txtResName.Focus();
        }
        catch (Exception ex)
        {
            ShowError("Không tạo được mã chi nhánh mới", ex);
        }
    }

    private async Task SaveRestaurantFromFormAsync(bool isNew)
    {
        try
        {
            var model = new RestaurantModel
            {
                RestaurantCode = txtResCode.Text,
                Name = txtResName.Text,
                District = txtResDistrict.Text,
                Address = txtResAddress.Text,
                Phone = txtResPhone.Text,
                OpenTime = txtResOpen.Text,
                CloseTime = txtResClose.Text,
                IsActive = chkResActive.Checked
            };

            await _menuService.SaveRestaurantAsync(model, isNew);
            await LoadTab1DataAsync(model.RestaurantCode.Trim().ToUpperInvariant());
            await RefreshKpiSummaryAsync();
            MessageBox.Show(
                isNew
                    ? $"Đã thêm chi nhánh {model.Name.Trim()} thành công."
                    : $"Đã cập nhật thông tin chi nhánh {model.Name.Trim()} thành công.",
                "Quản trị nhà hàng",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Không thể lưu chi nhánh", ex);
        }
    }

    private async Task DeleteSelectedRestaurantAsync()
    {
        var code = txtResCode.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(code)) return;

        var branchLabel = GetBranchDisplay(code);
        var confirm = MessageBox.Show(
            $"Bạn chắc chắn muốn xóa {branchLabel}?",
            "Xác nhận xóa chi nhánh",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            await _menuService.DeleteRestaurantAsync(code);
            await LoadTab1DataAsync();
            await RefreshKpiSummaryAsync();
            MessageBox.Show($"Đã xóa {branchLabel}.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Không thể xóa chi nhánh", ex);
        }
    }

    private void PrepareNewTable()
    {
        if (_selectedRestaurant == null)
        {
            MessageBox.Show("Hãy chọn một chi nhánh ở bảng trên trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var nextId = _menuService.GenerateNextTableId(_selectedRestaurant);
        txtTableId.Text = nextId;
        txtTableName.Text = $"Bàn {nextId.TrimStart('T')}";
        cboTableArea.SelectedValue = "indoor";
        numTableCapacity.Value = 4;
        chkTableAvailable.Checked = true;
        txtTableName.Focus();
    }

    private async Task SaveTableFromFormAsync(bool isNew)
    {
        if (_selectedRestaurant == null)
        {
            MessageBox.Show("Hãy chọn một chi nhánh trước khi thao tác bàn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            int capacity = ReadStrictNumericInt(
                numTableCapacity,
                "Số ghế của bàn",
                MenuAdminService.MinTableCapacity,
                MenuAdminService.MaxTableCapacity);

            var table = new TableItem
            {
                TableId = txtTableId.Text,
                TableName = txtTableName.Text,
                Area = cboTableArea.SelectedValue as string ?? "indoor",
                Capacity = capacity,
                IsAvailable = chkTableAvailable.Checked
            };

            var resCode = _selectedRestaurant.RestaurantCode;
            var branchLabel = GetBranchDisplay(resCode);
            await _menuService.UpsertTableAsync(resCode, table, isNew);
            await LoadTab1DataAsync(resCode);
            await RefreshKpiSummaryAsync();
            MessageBox.Show(
                isNew
                    ? $"Đã thêm {table.TableName.Trim()} vào {branchLabel}."
                    : $"Đã cập nhật {table.TableName.Trim()} tại {branchLabel}.",
                "Quản trị bàn",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Không thể lưu thông tin bàn", ex);
        }
    }

    private async Task DeleteSelectedTableAsync()
    {
        if (_selectedRestaurant == null) return;
        var tableId = txtTableId.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(tableId)) return;

        var tableObj = _selectedRestaurant.Tables.FirstOrDefault(t =>
            string.Equals(t.TableId, tableId, StringComparison.OrdinalIgnoreCase));
        var tableLabel = tableObj != null ? tableObj.TableName : tableId;
        var branchLabel = GetBranchDisplay(_selectedRestaurant.RestaurantCode);

        var confirm = MessageBox.Show(
            $"Xóa {tableLabel} khỏi {branchLabel}?",
            "Xác nhận xóa bàn",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            var resCode = _selectedRestaurant.RestaurantCode;
            await _menuService.DeleteTableAsync(resCode, tableId);
            await LoadTab1DataAsync(resCode);
            await RefreshKpiSummaryAsync();
            MessageBox.Show($"Đã xóa {tableLabel} khỏi {branchLabel}.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Không thể xóa bàn", ex);
        }
    }

    private async Task LoadMenuGridAsync(string? selectItemCode = null)
    {
        try
        {
            var resCode = cboMenuFilterRes.SelectedValue as string;
            var cat = cboMenuFilterCat.SelectedIndex > 0 ? cboMenuFilterCat.SelectedItem?.ToString() : null;
            var keyword = txtMenuKeyword.Text;
            var onlyAvail = chkMenuOnlyAvail.Checked;

            _menuItems = await _menuService.GetAllMenuItemsAsync(resCode, cat, keyword, onlyAvail);

            var rows = _menuItems.Select(m => new
            {
                Mã_món = m.ItemCode,
                Chi_nhánh = m.RestaurantCode,
                Tên_món = m.Name,
                Nhóm = m.Category,
                Giá_gốc = m.BasePrice,
                Đánh_giá = m.AvgRating,
                Toppings = m.Toppings.Count,
                Trạng_thái = m.IsAvailable ? "Còn bán" : "Tạm ngưng"
            }).ToList();

            dgvMenuItems.DataSource = rows;
            FormatMenuGridHeaders();

            if (!string.IsNullOrEmpty(selectItemCode))
            {
                foreach (DataGridViewRow row in dgvMenuItems.Rows)
                {
                    if (string.Equals(row.Cells[0].Value?.ToString(), selectItemCode, StringComparison.OrdinalIgnoreCase))
                    {
                        row.Selected = true;
                        dgvMenuItems.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Không tải được danh sách thực đơn", ex);
        }
    }

    private void OnMenuItemSelected(int? rowIndex = null)
    {
        var idx = rowIndex ?? dgvMenuItems.CurrentRow?.Index ?? -1;
        if (idx < 0 || idx >= dgvMenuItems.Rows.Count) return;

        var code = dgvMenuItems.Rows[idx].Cells[0].Value?.ToString();
        var item = _menuItems.FirstOrDefault(m =>
            string.Equals(m.ItemCode, code, StringComparison.OrdinalIgnoreCase));
        if (item == null) return;

        txtItemCode.Text = item.ItemCode;
        cboItemRestaurant.SelectedValue = item.RestaurantCode;
        txtItemName.Text = item.Name;
        cboItemCategory.SelectedItem = cboItemCategory.Items.Contains(item.Category) ? item.Category : "Món chính";
        numItemBasePrice.Value = Math.Clamp((decimal)item.BasePrice, numItemBasePrice.Minimum, numItemBasePrice.Maximum);

        var largeSize = item.Sizes.FirstOrDefault(s => string.Equals(s.SizeCode, "L", StringComparison.OrdinalIgnoreCase));
        numItemSizeLargeExtra.Value = Math.Clamp((decimal)(largeSize?.ExtraPrice ?? 0), numItemSizeLargeExtra.Minimum, numItemSizeLargeExtra.Maximum);

        txtItemImageUrl.Text = item.ImageUrl;
        txtItemToppings.Text = string.Join("; ", item.Toppings.Select(t => $"{t.Name}:{t.Price:0}"));
        chkItemAvailable.Checked = item.IsAvailable;
    }

    private async Task PrepareNewMenuItemAsync()
    {
        try
        {
            txtItemCode.Text = await _menuService.GenerateNextItemCodeAsync();
            txtItemName.Clear();
            numItemBasePrice.Value = 65000;
            numItemSizeLargeExtra.Value = 45000;
            txtItemImageUrl.Text = $"images/menu/{txtItemCode.Text.ToLowerInvariant()}.jpg";
            txtItemToppings.Text = "Thêm nước chấm đặc biệt:10000; Thêm rau sống:15000";
            chkItemAvailable.Checked = true;
            txtItemName.Focus();
        }
        catch (Exception ex)
        {
            ShowError("Không tạo được mã món mới", ex);
        }
    }

    private async Task SaveMenuItemFromFormAsync(bool isNew)
    {
        try
        {
            var resCode = cboItemRestaurant.SelectedValue as string ?? _selectedRestaurant?.RestaurantCode ?? "RES01";
            double basePrice = ReadStrictNumericDouble(
                numItemBasePrice,
                "Giá gốc của món",
                MenuAdminService.MinMenuBasePrice,
                MenuAdminService.MaxMenuBasePrice);
            double largeExtra = ReadStrictNumericDouble(
                numItemSizeLargeExtra,
                "Giá cộng thêm size L",
                0,
                MenuAdminService.MaxMenuExtraPrice);
            var toppings = ParseToppingsText(txtItemToppings.Text);
            var item = new MenuItemModel
            {
                ItemCode = txtItemCode.Text,
                RestaurantCode = resCode,
                Name = txtItemName.Text,
                Category = cboItemCategory.SelectedItem?.ToString() ?? "Món chính",
                BasePrice = basePrice,
                ImageUrl = txtItemImageUrl.Text,
                IsAvailable = chkItemAvailable.Checked,
                Sizes =
                [
                    new SizeOption { SizeCode = "S", Label = "Phần vừa", ExtraPrice = 0 },
                    new SizeOption { SizeCode = "L", Label = "Phần lớn", ExtraPrice = largeExtra }
                ],
                Toppings = toppings
            };

            await _menuService.SaveMenuItemAsync(item, isNew);
            await LoadMenuGridAsync(item.ItemCode.Trim().ToUpperInvariant());
            await RefreshKpiSummaryAsync();
            var branchLabel = GetBranchDisplay(resCode);

            MessageBox.Show(
                isNew
                    ? $"Đã thêm món '{item.Name.Trim()}' vào {branchLabel} thành công."
                    : $"Đã cập nhật món '{item.Name.Trim()}' tại {branchLabel} thành công.",
                "Quản trị thực đơn",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Không thể lưu món ăn", ex);
        }
    }

    private async Task DeleteSelectedMenuItemAsync()
    {
        var code = txtItemCode.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(code)) return;

        var existing = _menuItems.FirstOrDefault(m => string.Equals(m.ItemCode, code, StringComparison.OrdinalIgnoreCase));
        var resCode = existing?.RestaurantCode ?? (cboItemRestaurant.SelectedValue as string ?? string.Empty);
        var itemLabel = existing != null ? existing.Name : code;
        var branchLabel = GetBranchDisplay(resCode);

        var confirm = MessageBox.Show(
            $"Xóa món '{itemLabel}' khỏi thực đơn của {branchLabel}?",
            "Xác nhận xóa món",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            await _menuService.DeleteMenuItemAsync(code);
            await LoadMenuGridAsync();
            await RefreshKpiSummaryAsync();
            MessageBox.Show($"Đã xóa món '{itemLabel}' khỏi thực đơn của {branchLabel}.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Không thể xóa món ăn", ex);
        }
    }

    private string GetBranchDisplay(string? restaurantCode)
    {
        if (string.IsNullOrWhiteSpace(restaurantCode))
            return "chi nhánh";

        var found = _restaurants.FirstOrDefault(r =>
            string.Equals(r.RestaurantCode, restaurantCode.Trim(), StringComparison.OrdinalIgnoreCase));
        return found != null ? $"chi nhánh {found.Name}" : $"chi nhánh {restaurantCode.Trim()}";
    }

    private static string FormatAreaDisplay(string? area) =>
        area?.Trim().ToLowerInvariant() switch
        {
            "vip" => "Phòng VIP",
            "outdoor" => "Ngoài trời",
            _ => "Trong nhà"
        };

    private static int ReadStrictNumericInt(NumericUpDown control, string fieldName, int min, int max)
    {
        var raw = control.Text.Trim();
        if (string.IsNullOrEmpty(raw))
            throw new ArgumentException($"{fieldName} không được để trống (chỉ nhận số nguyên từ {min} đến {max}).");

        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            throw new ArgumentException($"{fieldName} phải là số nguyên hợp lệ (từ {min} đến {max}).");

        if (value < min || value > max)
            throw new ArgumentException($"{fieldName} phải lớn hơn 0 và nhỏ hơn hoặc bằng {max} (giá trị vừa nhập: {value}).");

        control.Value = value;
        return value;
    }

    private static double ReadStrictNumericDouble(NumericUpDown control, string fieldName, double min, double max)
    {
        var raw = control.Text.Trim();
        if (string.IsNullOrEmpty(raw))
            throw new ArgumentException($"{fieldName} không được để trống (hợp lệ từ {min:N0} đ đến {max:N0} đ).");

        var compact = Regex.IsMatch(raw, @"^-?\d+$")
            ? raw
            : Regex.IsMatch(raw, @"^-?\d{1,3}([.,]\d{3})+$")
                ? raw.Replace(",", "").Replace(".", "")
                : throw new ArgumentException($"{fieldName} chỉ nhận số nguyên hợp lệ (ví dụ: 50000 hoặc 50.000).");

        if (!double.TryParse(compact, NumberStyles.Integer, CultureInfo.InvariantCulture, out double value))
            throw new ArgumentException($"{fieldName} không hợp lệ.");

        if (value < min || value > max)
            throw new ArgumentException($"{fieldName} phải nằm trong khoảng từ {min:N0} đ đến {max:N0} đ.");

        control.Value = (decimal)value;
        return value;
    }

    private static List<ToppingOption> ParseToppingsText(string raw)
    {
        var list = new List<ToppingOption>();
        if (string.IsNullOrWhiteSpace(raw)) return list;

        var parts = raw.Split(';', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > MenuAdminService.MaxToppingsPerItem)
            throw new ArgumentException($"Mỗi món chỉ được cấu hình tối đa {MenuAdminService.MaxToppingsPerItem} món kèm.");

        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int idx = 1;
        foreach (var part in parts)
        {
            var segments = part.Split(':', StringSplitOptions.TrimEntries);
            if (segments.Length != 2 || string.IsNullOrWhiteSpace(segments[0]))
                throw new ArgumentException($"Định dạng món kèm không hợp lệ: '{part.Trim()}'. Hãy nhập dạng 'Tên món kèm:Giá; Tên khác:Giá'.");

            var name = segments[0];
            if (name.Length < 2 || name.Length > 80)
                throw new ArgumentException($"Tên món kèm '{name}' phải từ 2 đến 80 ký tự.");
            if (!seenNames.Add(name))
                throw new ArgumentException($"Món kèm '{name}' bị trùng lặp trong cùng một món.");

            var rawPrice = segments[1];
            string compactPrice;
            if (Regex.IsMatch(rawPrice, @"^\d+$"))
                compactPrice = rawPrice;
            else if (Regex.IsMatch(rawPrice, @"^\d{1,3}([.,]\d{3})+$"))
                compactPrice = rawPrice.Replace(",", "").Replace(".", "");
            else
                throw new ArgumentException($"Giá món kèm không hợp lệ ở '{part.Trim()}'. Hãy nhập số nguyên từ 0 đến {MenuAdminService.MaxMenuExtraPrice:N0} đ.");

            if (!double.TryParse(compactPrice, NumberStyles.Integer, CultureInfo.InvariantCulture, out var price) ||
                price < 0 || price > MenuAdminService.MaxMenuExtraPrice)
            {
                throw new ArgumentException($"Giá món kèm '{name}' phải từ 0 đ đến {MenuAdminService.MaxMenuExtraPrice:N0} đ.");
            }

            list.Add(new ToppingOption
            {
                ToppingCode = $"TOP{idx++:00}",
                Name = name,
                Price = price
            });
        }

        return list;
    }

    private void FormatRestaurantGridHeaders()
    {
        if (dgvRestaurants.Columns.Count < 6) return;
        dgvRestaurants.Columns[0].HeaderText = "Mã";
        dgvRestaurants.Columns[0].FillWeight = 55;
        dgvRestaurants.Columns[1].HeaderText = "Tên chi nhánh";
        dgvRestaurants.Columns[1].FillWeight = 135;
        dgvRestaurants.Columns[2].HeaderText = "Quận";
        dgvRestaurants.Columns[2].FillWeight = 75;
        dgvRestaurants.Columns[3].HeaderText = "Số bàn";
        dgvRestaurants.Columns[3].FillWeight = 55;
        dgvRestaurants.Columns[4].HeaderText = "Lượt xem";
        dgvRestaurants.Columns[4].FillWeight = 65;
        dgvRestaurants.Columns[5].HeaderText = "Trạng thái";
        dgvRestaurants.Columns[5].FillWeight = 65;
    }

    private void FormatTableGridHeaders()
    {
        if (dgvTables.Columns.Count < 5) return;
        dgvTables.Columns[0].HeaderText = "Mã bàn";
        dgvTables.Columns[1].HeaderText = "Tên bàn";
        dgvTables.Columns[2].HeaderText = "Khu vực";
        dgvTables.Columns[3].HeaderText = "Số ghế";
        dgvTables.Columns[4].HeaderText = "Trạng thái";
    }

    private void FormatMenuGridHeaders()
    {
        if (dgvMenuItems.Columns.Count < 8) return;
        dgvMenuItems.Columns[0].HeaderText = "Mã";
        dgvMenuItems.Columns[0].FillWeight = 60;
        dgvMenuItems.Columns[1].HeaderText = "CN";
        dgvMenuItems.Columns[1].FillWeight = 55;
        dgvMenuItems.Columns[2].HeaderText = "Tên món";
        dgvMenuItems.Columns[2].FillWeight = 175;
        dgvMenuItems.Columns[3].HeaderText = "Nhóm";
        dgvMenuItems.Columns[3].FillWeight = 90;
        dgvMenuItems.Columns[4].HeaderText = "Giá gốc";
        dgvMenuItems.Columns[4].DefaultCellStyle.Format = "N0";
        dgvMenuItems.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        dgvMenuItems.Columns[4].FillWeight = 80;
        dgvMenuItems.Columns[5].HeaderText = "Sao";
        dgvMenuItems.Columns[5].DefaultCellStyle.Format = "0.00";
        dgvMenuItems.Columns[5].FillWeight = 50;
        dgvMenuItems.Columns[6].HeaderText = "Topping";
        dgvMenuItems.Columns[6].FillWeight = 55;
        dgvMenuItems.Columns[7].HeaderText = "Trạng thái";
        dgvMenuItems.Columns[7].FillWeight = 75;
    }

    #endregion

    #region Tab 2: Aggregation Statistics & WinForms Bar Chart

    private async Task RefreshKpiSummaryAsync()
    {
        try
        {
            var s = await _menuService.GetAdminDashboardSummaryAsync();
            lblKpiBranches.Text = $"Chi nhánh / bàn\n{s.TotalRestaurants} chi nhánh • {s.TotalTables} bàn";
            lblKpiMenu.Text = $"Thực đơn\n{s.TotalMenuItems} món đang quản lý";
            lblKpiOrders.Text = $"Tổng đơn / hoàn tất\n{s.TotalOrders:N0} đơn ({s.CompletedOrders:N0} hoàn tất)";
            lblKpiRevenue.Text = $"Doanh thu hoàn tất\n{s.CompletedRevenue:N0} đ";
        }
        catch
        {
            // Bỏ qua nếu chưa kết nối.
        }
    }

    private async Task RunSelectedStatisticAsync()
    {
        try
        {
            await RefreshKpiSummaryAsync();
            var branchFilter = cboStatBranch.SelectedValue as string;
            int idx = cboStatQuery.SelectedIndex;

            _chartPoints.Clear();

            switch (idx)
            {
                case 0:
                {
                    var data = await _menuService.GetRevenueByMonthAndBranchAsync(branchFilter);
                    dgvStats.DataSource = data.Select(x => new
                    {
                        Tháng = x.YearMonth,
                        Mã_CN = x.RestaurantCode,
                        Tên_chi_nhánh = x.RestaurantName,
                        Số_đơn = x.OrderCount,
                        Doanh_thu_VNĐ = x.TotalRevenue,
                        TB_mỗi_đơn = x.AvgOrderValue
                    }).ToList();
                    FormatCurrencyColumns(dgvStats, [4, 5], [3]);

                    _chartTitle = "Doanh thu hoàn tất theo tháng và chi nhánh";
                    _chartSubtitle = "Tổng tiền thu về từ các đơn hàng đã hoàn tất theo từng tháng";
                    _chartAccentColor = Color.FromArgb(21, 101, 192);
                    foreach (var row in data.Take(10))
                    {
                        _chartPoints.Add(new ChartBarPoint(
                            $"{row.YearMonth} ({row.RestaurantCode})",
                            row.TotalRevenue,
                            FormatCompactVnd(row.TotalRevenue)));
                    }
                    break;
                }
                case 1:
                {
                    var data = await _menuService.GetTopItemsByRevenueAsync(10, branchFilter);
                    dgvStats.DataSource = data.Select(x => new
                    {
                        Mã_món = x.ItemCode,
                        Tên_món = x.Name,
                        Nhóm = x.Category,
                        Số_lượng = x.TotalQty,
                        Số_lượt_đơn = x.OrderAppearances,
                        Doanh_thu_VNĐ = x.TotalRevenue
                    }).ToList();
                    FormatCurrencyColumns(dgvStats, [5], [3, 4]);

                    _chartTitle = "Top 10 món ăn có doanh thu cao nhất";
                    _chartSubtitle = "Xếp hạng các món đóng góp doanh thu cao cho nhà hàng";
                    _chartAccentColor = Color.FromArgb(25, 135, 84);
                    foreach (var row in data)
                    {
                        _chartPoints.Add(new ChartBarPoint(
                            $"{row.ItemCode}\n{ShortenLabel(row.Name, 14)}",
                            row.TotalRevenue,
                            FormatCompactVnd(row.TotalRevenue)));
                    }
                    break;
                }
                case 2:
                {
                    var data = await _menuService.GetPeakTimeSlotsAsync(branchFilter);
                    dgvStats.DataSource = data.Select(x => new
                    {
                        Khung_giờ = x.TimeSlot,
                        Tổng_đơn = x.TotalOrders,
                        Ăn_tại_chỗ = x.DineInOrders,
                        Mang_về = x.TakeawayOrders,
                        Tổng_khách = x.TotalGuests,
                        Doanh_thu_VNĐ = x.TotalRevenue
                    }).ToList();
                    FormatCurrencyColumns(dgvStats, [5], [1, 2, 3, 4]);

                    _chartTitle = "Số lượng đơn theo khung giờ (giờ cao điểm)";
                    _chartSubtitle = "Phân tích lượng đơn ăn tại chỗ và mang về theo từng khung giờ";
                    _chartAccentColor = Color.FromArgb(230, 126, 34);
                    foreach (var row in data)
                    {
                        _chartPoints.Add(new ChartBarPoint(
                            row.TimeSlot,
                            row.TotalOrders,
                            $"{row.TotalOrders} đơn"));
                    }
                    break;
                }
                case 3:
                {
                    var data = await _menuService.GetVipCustomersAsync(12, branchFilter);
                    dgvStats.DataSource = data.Select(x => new
                    {
                        Mã_KH = x.UserCode,
                        Họ_tên = x.FullName,
                        SĐT = x.Phone,
                        Số_đơn = x.OrderCount,
                        Số_CN_ghé = x.BranchCount,
                        Các_chi_nhánh = x.VisitedRestaurants,
                        Tổng_chi_VNĐ = x.TotalSpent
                    }).ToList();
                    FormatCurrencyColumns(dgvStats, [6], [3, 4]);

                    _chartTitle = "Top khách hàng thân thiết (VIP)";
                    _chartSubtitle = "Những khách hàng có tổng chi tiêu cao và số chi nhánh đã ghé";
                    _chartAccentColor = Color.FromArgb(142, 68, 173);
                    foreach (var row in data.Take(10))
                    {
                        _chartPoints.Add(new ChartBarPoint(
                            $"{row.UserCode}\n{ShortenLabel(row.FullName, 12)}",
                            row.TotalSpent,
                            FormatCompactVnd(row.TotalSpent)));
                    }
                    break;
                }
                case 4:
                {
                    var data = await _menuService.GetRestaurantReviewStatsAsync(branchFilter);
                    dgvStats.DataSource = data.Select(x => new
                    {
                        Mã_CN = x.RestaurantCode,
                        Tên_chi_nhánh = x.RestaurantName,
                        Số_đánh_giá = x.ReviewCount,
                        Điểm_TB = x.AvgRating,
                        Đã_phản_hồi = x.RepliedCount,
                        Tỷ_lệ_phản_hồi_pct = x.ReplyRatePercent
                    }).ToList();
                    FormatCurrencyColumns(dgvStats, [], [2, 4]);
                    if (dgvStats.Columns.Count >= 6)
                        dgvStats.Columns[5].HeaderText = "Tỷ lệ phản hồi (%)";

                    _chartTitle = "Điểm đánh giá trung bình theo chi nhánh";
                    _chartSubtitle = "Tổng hợp số sao đánh giá và tỷ lệ nhân viên phản hồi khách hàng";
                    _chartAccentColor = Color.FromArgb(192, 57, 43);
                    foreach (var row in data)
                    {
                        _chartPoints.Add(new ChartBarPoint(
                            $"{row.RestaurantCode}\n({row.ReviewCount} đánh giá)",
                            row.AvgRating,
                            $"{row.AvgRating:0.00} ★"));
                    }
                    break;
                }
                case 5:
                {
                    var data = await _menuService.GetTopRankedItemsAsync(10);
                    dgvStats.DataSource = data.Select(x => new
                    {
                        Hạng = x.Rank,
                        Mã_món = x.ItemCode,
                        Tên_món = x.Name,
                        Chi_nhánh = x.RestaurantCode,
                        Nhóm = x.Category,
                        Giá_gốc = x.BasePrice,
                        Tổng_lượt_đặt = x.Score
                    }).ToList();
                    FormatCurrencyColumns(dgvStats, [5], [0, 6]);

                    _chartTitle = "Top 10 món ăn được đặt nhiều nhất";
                    _chartSubtitle = "Bảng xếp hạng theo tổng số phần món khách đã gọi";
                    _chartAccentColor = Color.FromArgb(0, 150, 136);
                    foreach (var row in data)
                    {
                        _chartPoints.Add(new ChartBarPoint(
                            $"{row.ItemCode}\n{ShortenLabel(row.Name, 14)}",
                            row.Score,
                            $"{row.Score:0}"));
                    }
                    break;
                }
                case 6:
                {
                    var data = await _menuService.GetTopRestaurantsAsync(10);
                    dgvStats.DataSource = data.Select(x => new
                    {
                        Hạng = x.Rank,
                        Mã_CN = x.RestaurantCode,
                        Tên_chi_nhánh = x.Name,
                        Quận = x.District,
                        Tổng_số_đơn = x.Score,
                        Lượt_xem = x.Views
                    }).ToList();
                    FormatCurrencyColumns(dgvStats, [], [0, 4, 5]);

                    _chartTitle = "Xếp hạng chi nhánh theo tổng đơn và lượt xem";
                    _chartSubtitle = "So sánh độ phổ biến của từng chi nhánh";
                    _chartAccentColor = Color.FromArgb(52, 73, 94);
                    foreach (var row in data)
                    {
                        _chartPoints.Add(new ChartBarPoint(
                            $"{row.RestaurantCode}\n({row.Views} lượt xem)",
                            row.Score,
                            $"{row.Score:0} đơn"));
                    }
                    break;
                }
            }

            grpChart.Text = $"Biểu đồ cột trực quan (cập nhật lúc {DateTime.Now:HH:mm:ss})";
            pnlBarChart.Invalidate();
        }
        catch (Exception ex)
        {
            ShowError("Lỗi khi chạy thống kê", ex);
        }
    }

    private static void FormatCurrencyColumns(DataGridView grid, int[] currencyIndices, int[] intIndices)
    {
        foreach (var colIdx in currencyIndices)
        {
            if (colIdx < grid.Columns.Count)
            {
                grid.Columns[colIdx].DefaultCellStyle.Format = "N0";
                grid.Columns[colIdx].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
        foreach (var colIdx in intIndices)
        {
            if (colIdx < grid.Columns.Count)
            {
                grid.Columns[colIdx].DefaultCellStyle.Format = "N0";
                grid.Columns[colIdx].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
    }

    private void PnlBarChart_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.White);

        var bounds = pnlBarChart.ClientRectangle;
        if (bounds.Width < 200 || bounds.Height < 180) return;

        using var titleFont = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        using var subFont = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        using var labelFont = new Font("Segoe UI", 8F);
        using var valueFont = new Font("Segoe UI", 8F, FontStyle.Bold);
        using var titleBrush = new SolidBrush(Color.FromArgb(30, 41, 59));
        using var subBrush = new SolidBrush(Color.FromArgb(100, 116, 139));
        using var axisPen = new Pen(Color.FromArgb(148, 163, 184), 1.2F);
        using var gridPen = new Pen(Color.FromArgb(226, 232, 240), 1F) { DashStyle = DashStyle.Dash };

        g.DrawString(_chartTitle, titleFont, titleBrush, 14, 10);
        if (!string.IsNullOrEmpty(_chartSubtitle))
            g.DrawString(_chartSubtitle, subFont, subBrush, 14, 32);

        if (_chartPoints.Count == 0)
        {
            g.DrawString("Chưa có dữ liệu biểu đồ cho bộ lọc hiện tại.", labelFont, subBrush, 20, 70);
            return;
        }

        int padLeft = 64;
        int padRight = 20;
        int padTop = 62;
        int padBottom = 64;

        var plot = new Rectangle(
            padLeft,
            padTop,
            Math.Max(60, bounds.Width - padLeft - padRight),
            Math.Max(60, bounds.Height - padTop - padBottom));

        double maxVal = _chartPoints.Max(p => p.Value);
        if (maxVal <= 0) maxVal = 1;

        // Vẽ 4 đường lưới ngang & nhãn trục Y
        const int gridSteps = 4;
        var yLabelFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        for (int i = 0; i <= gridSteps; i++)
        {
            float y = plot.Bottom - (plot.Height * i / (float)gridSteps);
            if (i > 0)
                g.DrawLine(gridPen, plot.Left, y, plot.Right, y);

            var tickVal = maxVal * i / gridSteps;
            g.DrawString(FormatAxisTick(tickVal), labelFont, subBrush, new RectangleF(4, y - 10, padLeft - 10, 20), yLabelFormat);
        }

        // Trục X & Y
        g.DrawLine(axisPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
        g.DrawLine(axisPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);

        int count = _chartPoints.Count;
        float slotWidth = plot.Width / (float)count;
        float barWidth = Math.Clamp(slotWidth * 0.56f, 16f, 56f);

        using var barBrush = new SolidBrush(_chartAccentColor);
        using var barBorder = new Pen(ControlPaint.Dark(_chartAccentColor, 0.15f), 1f);
        var centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };

        for (int i = 0; i < count; i++)
        {
            var pt = _chartPoints[i];
            float ratio = (float)(pt.Value / maxVal);
            float barHeight = Math.Max(2f, ratio * (plot.Height - 16));
            float x = plot.Left + i * slotWidth + (slotWidth - barWidth) / 2f;
            float y = plot.Bottom - barHeight;

            var rect = new RectangleF(x, y, barWidth, barHeight);
            g.FillRectangle(barBrush, rect);
            g.DrawRectangle(barBorder, rect.X, rect.Y, rect.Width, rect.Height);

            var valRect = new RectangleF(plot.Left + i * slotWidth, y - 18, slotWidth, 16);
            g.DrawString(pt.ValueText, valueFont, titleBrush, valRect, centerFormat);

            var lblRect = new RectangleF(plot.Left + i * slotWidth + 2, plot.Bottom + 5, slotWidth - 4, padBottom - 8);
            g.DrawString(pt.Label, labelFont, titleBrush, lblRect, centerFormat);
        }
    }

    private static string FormatCompactVnd(double value)
    {
        if (value >= 1_000_000) return $"{value / 1_000_000:0.##}M";
        if (value >= 1_000) return $"{value / 1_000:0.#}K";
        return $"{value:0}";
    }

    private static string FormatAxisTick(double value)
    {
        if (value >= 1_000_000) return $"{value / 1_000_000:0.#}M";
        if (value >= 1_000) return $"{value / 1_000:0.#}K";
        return value < 10 ? $"{value:0.#}" : $"{value:0}";
    }

    private static string ShortenLabel(string text, int maxLen) =>
        text.Length <= maxLen ? text : string.Concat(text.AsSpan(0, maxLen - 1), "…");

    #endregion

    #region Tab 3: Backup, Restore & Export/Import

    private void BrowseBackupFolder()
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "Chọn thư mục lưu tập tin sao lưu dữ liệu",
            SelectedPath = Directory.Exists(txtBackupFolder.Text) ? txtBackupFolder.Text : BackupService.GetDefaultBackupRoot()
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
            txtBackupFolder.Text = dlg.SelectedPath;
    }

    private void OpenBackupFolder()
    {
        var path = string.IsNullOrWhiteSpace(txtBackupFolder.Text)
            ? BackupService.GetDefaultBackupRoot()
            : txtBackupFolder.Text.Trim();
        Directory.CreateDirectory(path);
        Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
    }

    private void BrowseImportJsonFile()
    {
        using var dlg = new OpenFileDialog
        {
            Title = "Chọn tập tin JSON để nhập dữ liệu vào hệ thống",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = Directory.Exists(txtBackupFolder.Text) ? txtBackupFolder.Text : BackupService.GetDefaultBackupRoot()
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            txtImportJsonFile.Text = dlg.FileName;
            var fileNameNoExt = Path.GetFileNameWithoutExtension(dlg.FileName);
            if (BackupService.Collections.Contains(fileNameNoExt))
                cboExportCollection.SelectedValue = fileNameNoExt;
        }
    }

    private async Task ExecuteMongoDumpAsync()
    {
        SetBackupButtonsEnabled(false);
        try
        {
            var res = await _backupService.RunMongoDumpAsync(txtBackupFolder.Text);
            AppendBackupLog("Sao lưu dữ liệu chính (mongodump)", res);
            if (res.Success)
                MessageBox.Show($"Đã sao lưu dữ liệu chính thành công tại:\n{res.TargetPath}", "Sao lưu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Sao lưu thất bại. Xem chi tiết ở khung nhật ký bên dưới.", "Lỗi sao lưu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            ShowError("Lỗi khi sao lưu dữ liệu", ex);
        }
        finally
        {
            SetBackupButtonsEnabled(true);
        }
    }

    private async Task ExecuteMongoRestoreAsync()
    {
        using var dlg = new FolderBrowserDialog
        {
            Description = "Chọn thư mục chứa bản sao lưu trước đó (ví dụ: mongodump_yyyyMMdd_HHmmss)",
            SelectedPath = Directory.Exists(txtBackupFolder.Text) ? txtBackupFolder.Text : BackupService.GetDefaultBackupRoot()
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        var confirm = MessageBox.Show(
            $"Dữ liệu hiện tại của hệ thống sẽ được thay thế bằng bản sao lưu tại:\n{dlg.SelectedPath}\n\nBạn có chắc chắn muốn khôi phục?",
            "Xác nhận khôi phục dữ liệu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        SetBackupButtonsEnabled(false);
        try
        {
            var res = await _backupService.RunMongoRestoreAsync(dlg.SelectedPath, dropBeforeRestore: true);
            AppendBackupLog("Khôi phục dữ liệu chính (mongorestore)", res);
            if (res.Success)
            {
                await _menuService.InvalidateAllMenuCachesAsync();
                await LoadTab1DataAsync();
                await RunSelectedStatisticAsync();
                MessageBox.Show("Khôi phục dữ liệu thành công và đã làm mới bộ nhớ đệm thực đơn.", "Khôi phục dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Khôi phục dữ liệu thất bại. Xem chi tiết ở khung nhật ký bên dưới.", "Lỗi khôi phục", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            ShowError("Lỗi khi khôi phục dữ liệu", ex);
        }
        finally
        {
            SetBackupButtonsEnabled(true);
        }
    }

    private async Task ExecuteExportAllJsonAsync()
    {
        SetBackupButtonsEnabled(false);
        try
        {
            var res = await _backupService.ExportAllCollectionsToJsonAsync(txtBackupFolder.Text);
            AppendBackupLog("Xuất 5 bảng dữ liệu ra tập tin JSON (mongoexport)", res);
            if (res.Success)
                MessageBox.Show($"Đã xuất 5 bảng dữ liệu ra tập tin JSON tại:\n{res.TargetPath}", "Xuất tập tin JSON", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Xuất tập tin JSON gặp lỗi. Xem khung nhật ký bên dưới.", "Lỗi xuất JSON", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            ShowError("Lỗi khi xuất tập tin JSON", ex);
        }
        finally
        {
            SetBackupButtonsEnabled(true);
        }
    }

    private async Task ExecuteImportJsonAsync()
    {
        var collection = cboExportCollection.SelectedValue as string ?? string.Empty;
        var jsonPath = txtImportJsonFile.Text.Trim();

        if (string.IsNullOrEmpty(collection) || string.IsNullOrEmpty(jsonPath))
        {
            MessageBox.Show("Hãy chọn bảng dữ liệu và tập tin .json trước.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (chkImportDrop.Checked)
        {
            var confirm = MessageBox.Show(
                $"Bạn đang chọn xóa dữ liệu cũ: dữ liệu hiện tại của bảng '{collection}' sẽ được thay mới từ tập tin:\n{jsonPath}\n\nTiếp tục?",
                "Xác nhận nhập dữ liệu từ tập tin JSON",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;
        }

        SetBackupButtonsEnabled(false);
        try
        {
            var res = await _backupService.ImportCollectionFromJsonAsync(collection, jsonPath, chkImportDrop.Checked);
            AppendBackupLog($"Nhập dữ liệu từ tập tin JSON vào bảng '{collection}' (mongoimport)", res);
            if (res.Success)
            {
                await _menuService.InvalidateAllMenuCachesAsync();
                await LoadTab1DataAsync();
                await RunSelectedStatisticAsync();
                MessageBox.Show($"Nhập dữ liệu từ tập tin JSON vào bảng '{collection}' thành công và đã làm mới bộ nhớ đệm.", "Nhập JSON", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Nhập dữ liệu thất bại. Xem khung nhật ký bên dưới.", "Lỗi nhập JSON", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            ShowError("Lỗi khi nhập dữ liệu từ JSON", ex);
        }
        finally
        {
            SetBackupButtonsEnabled(true);
        }
    }

    private async Task ExecuteMemuraiBgSaveAsync()
    {
        SetBackupButtonsEnabled(false);
        try
        {
            var status = await _backupService.TriggerMemuraiBgSaveAsync();
            UpdateMemuraiPersistLabel(status);
            AppendTextLog(
                "Lưu trạng thái bộ nhớ tạm (Memurai BGSAVE)",
                $"Đã lưu trạng thái hiện tại (giỏ hàng, lượt xem, xếp hạng) xuống ổ cứng.\nThư mục dữ liệu: {status.DataDirectory}\nTập tin ảnh chụp (RDB): {status.DbFilename} (trạng thái={status.LastBgSaveStatus})\nNhật ký thao tác (AOF): {(status.AppendOnlyEnabled ? "đang bật" : "chưa bật")} ({status.AppendDirName}, {status.AofCurrentSize} bytes)");
        }
        catch (Exception ex)
        {
            ShowError("Lỗi khi lưu trạng thái bộ nhớ tạm", ex);
        }
        finally
        {
            SetBackupButtonsEnabled(true);
        }
    }

    private async Task ExecuteBackupMemuraiFilesAsync()
    {
        SetBackupButtonsEnabled(false);
        try
        {
            var res = await _backupService.BackupMemuraiPersistenceFilesAsync(txtBackupFolder.Text);
            AppendBackupLog("Chép tập tin dự phòng bộ nhớ tạm (RDB + AOF)", res);
            await RefreshMemuraiPersistenceStatusAsync(logToConsole: false);
            if (res.Success)
                MessageBox.Show($"Đã chép các tập tin dự phòng bộ nhớ tạm ra:\n{res.TargetPath}", "Sao lưu bộ nhớ tạm", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(res.ErrorLog, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            ShowError("Lỗi khi chép tập tin dự phòng bộ nhớ tạm", ex);
        }
        finally
        {
            SetBackupButtonsEnabled(true);
        }
    }

    private async Task RefreshMemuraiPersistenceStatusAsync(bool logToConsole)
    {
        try
        {
            var status = await _backupService.GetMemuraiPersistenceStatusAsync();
            UpdateMemuraiPersistLabel(status);
            if (logToConsole)
                AppendTextLog("Kiểm tra chế độ tự động lưu bộ nhớ tạm", status.RawInfo.Trim());
        }
        catch (Exception ex)
        {
            lblMemuraiPersist.ForeColor = Color.Firebrick;
            lblMemuraiPersist.Text = $"Chưa đọc được tình trạng bộ nhớ tạm: {ex.Message}";
        }
    }

    private void UpdateMemuraiPersistLabel(MemuraiPersistenceStatus status)
    {
        lblMemuraiPersist.ForeColor = status.AppendOnlyEnabled ? Color.FromArgb(20, 110, 45) : Color.FromArgb(180, 83, 9);
        lblMemuraiPersist.Text =
            $"Chế độ tự động lưu bộ nhớ tạm: {(status.AppendOnlyEnabled ? "đang bật (RDB + AOF)" : "chưa bật nhật ký AOF")} " +
            $"(ảnh chụp {status.DbFilename}: {status.LastBgSaveStatus} | nhật ký {status.AppendDirName}: {status.AofCurrentSize} bytes)";
    }

    private void AppendBackupLog(string title, BackupOperationResult res)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Command : {res.CommandText}");
        sb.AppendLine($"ExitCode: {res.ExitCode} ({(res.Success ? "SUCCESS" : "FAILED")})");
        if (!string.IsNullOrWhiteSpace(res.TargetPath))
            sb.AppendLine($"Target  : {res.TargetPath}");
        if (!string.IsNullOrWhiteSpace(res.OutputLog))
            sb.AppendLine($"Stdout  :\n{res.OutputLog}");
        if (!string.IsNullOrWhiteSpace(res.ErrorLog))
            sb.AppendLine($"Stderr  :\n{res.ErrorLog}");

        AppendTextLog(title, sb.ToString().TrimEnd());
    }

    private void AppendTextLog(string title, string body)
    {
        var block =
            $"==================== [{DateTime.Now:HH:mm:ss}] {title} ===================={Environment.NewLine}" +
            body.Replace("\n", Environment.NewLine) +
            Environment.NewLine + Environment.NewLine;

        txtBackupLog.AppendText(block);
    }

    private void SetBackupButtonsEnabled(bool enabled)
    {
        btnMongoDump.Enabled = enabled;
        btnMongoRestore.Enabled = enabled;
        btnExportAllJson.Enabled = enabled;
        btnImportJson.Enabled = enabled;
        btnMemuraiBgSave.Enabled = enabled;
        btnMemuraiBackupFiles.Enabled = enabled;
        btnMemuraiStatus.Enabled = enabled;
        Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
    }

    #endregion

    private static void ShowError(string title, Exception ex) =>
        MessageBox.Show($"{title}:\n{ex.Message}", title, MessageBoxButtons.OK, MessageBoxIcon.Error);

    private sealed record ComboOption(string Value, string Label);
    private sealed record ChartBarPoint(string Label, double Value, string ValueText);
}
