namespace RestaurantBooking.Forms.TV2_Admin;

partial class FrmAdmin
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblInfo = null!;
    private TabControl tabAdmin = null!;

    // Tab 1: Nhà hàng, bàn & thực đơn (quản lý thuần nghiệp vụ)
    private TabPage tabRestaurantMenu = null!;

    private SplitContainer splitMain = null!;
    private SplitContainer splitLeft = null!;

    private GroupBox grpRestaurants = null!;
    private DataGridView dgvRestaurants = null!;
    private Panel pnlResForm = null!;
    private TextBox txtResCode = null!;
    private TextBox txtResName = null!;
    private TextBox txtResDistrict = null!;
    private TextBox txtResPhone = null!;
    private TextBox txtResOpen = null!;
    private TextBox txtResClose = null!;
    private TextBox txtResAddress = null!;
    private CheckBox chkResActive = null!;
    private Button btnResNew = null!;
    private Button btnResAdd = null!;
    private Button btnResUpdate = null!;
    private Button btnResDelete = null!;

    private GroupBox grpTables = null!;
    private DataGridView dgvTables = null!;
    private Panel pnlTableForm = null!;
    private TextBox txtTableId = null!;
    private TextBox txtTableName = null!;
    private ComboBox cboTableArea = null!;
    private NumericUpDown numTableCapacity = null!;
    private CheckBox chkTableAvailable = null!;
    private Button btnTableNew = null!;
    private Button btnTableAdd = null!;
    private Button btnTableUpdate = null!;
    private Button btnTableDelete = null!;

    private GroupBox grpMenuItems = null!;
    private Panel pnlMenuFilter = null!;
    private ComboBox cboMenuFilterRes = null!;
    private ComboBox cboMenuFilterCat = null!;
    private TextBox txtMenuKeyword = null!;
    private CheckBox chkMenuOnlyAvail = null!;
    private Button btnMenuFilter = null!;
    private DataGridView dgvMenuItems = null!;
    private Panel pnlMenuForm = null!;
    private TextBox txtItemCode = null!;
    private ComboBox cboItemRestaurant = null!;
    private TextBox txtItemName = null!;
    private ComboBox cboItemCategory = null!;
    private NumericUpDown numItemBasePrice = null!;
    private NumericUpDown numItemSizeLargeExtra = null!;
    private TextBox txtItemToppings = null!;
    private TextBox txtItemImageUrl = null!;
    private CheckBox chkItemAvailable = null!;
    private Button btnMenuNew = null!;
    private Button btnMenuAdd = null!;
    private Button btnMenuUpdate = null!;
    private Button btnMenuDelete = null!;

    // Tab 2: Thống kê Aggregation & Top ZSET
    private TabPage tabStatistics = null!;
    private TableLayoutPanel tlpKpi = null!;
    private Label lblKpiBranches = null!;
    private Label lblKpiMenu = null!;
    private Label lblKpiOrders = null!;
    private Label lblKpiRevenue = null!;
    private Panel pnlStatFilter = null!;
    private ComboBox cboStatQuery = null!;
    private ComboBox cboStatBranch = null!;
    private Button btnRunStat = null!;
    private SplitContainer splitStats = null!;
    private DataGridView dgvStats = null!;
    private GroupBox grpChart = null!;
    private Panel pnlBarChart = null!;

    // Tab 3: Backup, Restore & Export/Import
    private TabPage tabBackup = null!;
    private GroupBox grpBackupPaths = null!;
    private TextBox txtBackupFolder = null!;
    private Button btnBrowseBackupFolder = null!;
    private Button btnOpenBackupFolder = null!;
    private ComboBox cboExportCollection = null!;
    private TextBox txtImportJsonFile = null!;
    private Button btnBrowseImportFile = null!;
    private CheckBox chkImportDrop = null!;
    private GroupBox grpBackupActions = null!;
    private Button btnMongoDump = null!;
    private Button btnMongoRestore = null!;
    private Button btnExportAllJson = null!;
    private Button btnImportJson = null!;
    private Button btnMemuraiBgSave = null!;
    private Button btnMemuraiBackupFiles = null!;
    private Button btnMemuraiStatus = null!;
    private Label lblMemuraiPersist = null!;
    private GroupBox grpBackupLog = null!;
    private TextBox txtBackupLog = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblInfo = new Label();
        tabAdmin = new TabControl();

        // Tab 1 controls
        tabRestaurantMenu = new TabPage();

        splitMain = new SplitContainer();
        splitLeft = new SplitContainer();

        grpRestaurants = new GroupBox();
        dgvRestaurants = new DataGridView();
        pnlResForm = new Panel();
        txtResCode = new TextBox();
        txtResName = new TextBox();
        txtResDistrict = new TextBox();
        txtResPhone = new TextBox();
        txtResOpen = new TextBox();
        txtResClose = new TextBox();
        txtResAddress = new TextBox();
        chkResActive = new CheckBox();
        btnResNew = new Button();
        btnResAdd = new Button();
        btnResUpdate = new Button();
        btnResDelete = new Button();

        grpTables = new GroupBox();
        dgvTables = new DataGridView();
        pnlTableForm = new Panel();
        txtTableId = new TextBox();
        txtTableName = new TextBox();
        cboTableArea = new ComboBox();
        numTableCapacity = new NumericUpDown();
        chkTableAvailable = new CheckBox();
        btnTableNew = new Button();
        btnTableAdd = new Button();
        btnTableUpdate = new Button();
        btnTableDelete = new Button();

        grpMenuItems = new GroupBox();
        pnlMenuFilter = new Panel();
        cboMenuFilterRes = new ComboBox();
        cboMenuFilterCat = new ComboBox();
        txtMenuKeyword = new TextBox();
        chkMenuOnlyAvail = new CheckBox();
        btnMenuFilter = new Button();
        dgvMenuItems = new DataGridView();
        pnlMenuForm = new Panel();
        txtItemCode = new TextBox();
        cboItemRestaurant = new ComboBox();
        txtItemName = new TextBox();
        cboItemCategory = new ComboBox();
        numItemBasePrice = new NumericUpDown();
        numItemSizeLargeExtra = new NumericUpDown();
        txtItemToppings = new TextBox();
        txtItemImageUrl = new TextBox();
        chkItemAvailable = new CheckBox();
        btnMenuNew = new Button();
        btnMenuAdd = new Button();
        btnMenuUpdate = new Button();
        btnMenuDelete = new Button();

        // Tab 2 controls
        tabStatistics = new TabPage();
        tlpKpi = new TableLayoutPanel();
        lblKpiBranches = new Label();
        lblKpiMenu = new Label();
        lblKpiOrders = new Label();
        lblKpiRevenue = new Label();
        pnlStatFilter = new Panel();
        cboStatQuery = new ComboBox();
        cboStatBranch = new ComboBox();
        btnRunStat = new Button();
        splitStats = new SplitContainer();
        dgvStats = new DataGridView();
        grpChart = new GroupBox();
        pnlBarChart = new Panel();

        // Tab 3 controls
        tabBackup = new TabPage();
        grpBackupPaths = new GroupBox();
        txtBackupFolder = new TextBox();
        btnBrowseBackupFolder = new Button();
        btnOpenBackupFolder = new Button();
        cboExportCollection = new ComboBox();
        txtImportJsonFile = new TextBox();
        btnBrowseImportFile = new Button();
        chkImportDrop = new CheckBox();
        grpBackupActions = new GroupBox();
        btnMongoDump = new Button();
        btnMongoRestore = new Button();
        btnExportAllJson = new Button();
        btnImportJson = new Button();
        btnMemuraiBgSave = new Button();
        btnMemuraiBackupFiles = new Button();
        btnMemuraiStatus = new Button();
        lblMemuraiPersist = new Label();
        grpBackupLog = new GroupBox();
        txtBackupLog = new TextBox();

        tabAdmin.SuspendLayout();
        tabRestaurantMenu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
        splitMain.Panel1.SuspendLayout();
        splitMain.Panel2.SuspendLayout();
        splitMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitLeft).BeginInit();
        splitLeft.Panel1.SuspendLayout();
        splitLeft.Panel2.SuspendLayout();
        splitLeft.SuspendLayout();
        grpRestaurants.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvRestaurants).BeginInit();
        pnlResForm.SuspendLayout();
        grpTables.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTables).BeginInit();
        pnlTableForm.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numTableCapacity).BeginInit();
        grpMenuItems.SuspendLayout();
        pnlMenuFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvMenuItems).BeginInit();
        pnlMenuForm.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numItemBasePrice).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numItemSizeLargeExtra).BeginInit();

        tabStatistics.SuspendLayout();
        tlpKpi.SuspendLayout();
        pnlStatFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitStats).BeginInit();
        splitStats.Panel1.SuspendLayout();
        splitStats.Panel2.SuspendLayout();
        splitStats.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvStats).BeginInit();
        grpChart.SuspendLayout();

        tabBackup.SuspendLayout();
        grpBackupPaths.SuspendLayout();
        grpBackupActions.SuspendLayout();
        grpBackupLog.SuspendLayout();
        SuspendLayout();

        // Top banner
        lblInfo.BackColor = Color.FromArgb(24, 58, 90);
        lblInfo.Dock = DockStyle.Top;
        lblInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblInfo.ForeColor = Color.White;
        lblInfo.Padding = new Padding(12, 8, 12, 6);
        lblInfo.Size = new Size(1240, 38);
        lblInfo.Text = "Quản trị hệ thống nhà hàng — Quản trị viên";

        // ====================== TAB 1 ======================
        tabRestaurantMenu.Padding = new Padding(6);
        tabRestaurantMenu.Text = "Nhà hàng, bàn && thực đơn";

        splitMain.Dock = DockStyle.Fill;
        splitMain.Orientation = Orientation.Vertical;

        splitLeft.Dock = DockStyle.Fill;
        splitLeft.Orientation = Orientation.Horizontal;

        // Group 1: Restaurants
        grpRestaurants.Dock = DockStyle.Fill;
        grpRestaurants.Text = "1. Danh sách chi nhánh nhà hàng";
        ConfigureGrid(dgvRestaurants);
        dgvRestaurants.Dock = DockStyle.Fill;

        pnlResForm.Dock = DockStyle.Bottom;
        pnlResForm.Size = new Size(520, 148);

        var lblResCode = MakeLabel("Mã:", 8, 10);
        txtResCode.Location = new Point(52, 7);
        txtResCode.MaxLength = 15;
        txtResCode.Size = new Size(76, 23);

        var lblResName = MakeLabel("Tên:", 136, 10);
        txtResName.Location = new Point(170, 7);
        txtResName.MaxLength = 100;
        txtResName.Size = new Size(210, 23);

        chkResActive.AutoSize = true;
        chkResActive.Checked = true;
        chkResActive.Location = new Point(392, 9);
        chkResActive.Text = "Đang hoạt động";

        var lblResDistrict = MakeLabel("Quận:", 8, 40);
        txtResDistrict.Location = new Point(52, 37);
        txtResDistrict.MaxLength = 60;
        txtResDistrict.Size = new Size(110, 23);

        var lblResPhone = MakeLabel("SĐT:", 170, 40);
        txtResPhone.Location = new Point(208, 37);
        txtResPhone.MaxLength = 15;
        txtResPhone.Size = new Size(110, 23);

        var lblResHours = MakeLabel("Giờ:", 326, 40);
        txtResOpen.Location = new Point(358, 37);
        txtResOpen.MaxLength = 5;
        txtResOpen.Size = new Size(65, 23);
        txtResOpen.Text = "09:00";
        txtResClose.Location = new Point(430, 37);
        txtResClose.MaxLength = 5;
        txtResClose.Size = new Size(65, 23);
        txtResClose.Text = "22:30";

        var lblResAddr = MakeLabel("Đ/c:", 8, 70);
        txtResAddress.Location = new Point(52, 67);
        txtResAddress.MaxLength = 200;
        txtResAddress.Size = new Size(443, 23);

        btnResNew.Location = new Point(8, 104);
        btnResNew.Size = new Size(92, 30);
        btnResNew.Text = "Mã mới";

        btnResAdd.BackColor = Color.FromArgb(25, 135, 84);
        btnResAdd.ForeColor = Color.White;
        btnResAdd.FlatStyle = FlatStyle.Flat;
        btnResAdd.Location = new Point(108, 104);
        btnResAdd.Size = new Size(118, 30);
        btnResAdd.Text = "Thêm mới";

        btnResUpdate.BackColor = Color.FromArgb(13, 110, 253);
        btnResUpdate.ForeColor = Color.White;
        btnResUpdate.FlatStyle = FlatStyle.Flat;
        btnResUpdate.Location = new Point(234, 104);
        btnResUpdate.Size = new Size(125, 30);
        btnResUpdate.Text = "Cập nhật";

        btnResDelete.BackColor = Color.FromArgb(220, 53, 69);
        btnResDelete.ForeColor = Color.White;
        btnResDelete.FlatStyle = FlatStyle.Flat;
        btnResDelete.Location = new Point(367, 104);
        btnResDelete.Size = new Size(125, 30);
        btnResDelete.Text = "Xóa";

        pnlResForm.Controls.AddRange([
            lblResCode, txtResCode, lblResName, txtResName, chkResActive,
            lblResDistrict, txtResDistrict, lblResPhone, txtResPhone, lblResHours, txtResOpen, txtResClose,
            lblResAddr, txtResAddress, btnResNew, btnResAdd, btnResUpdate, btnResDelete
        ]);
        grpRestaurants.Controls.Add(dgvRestaurants);
        grpRestaurants.Controls.Add(pnlResForm);

        // Group 2: Embedded Tables
        grpTables.Dock = DockStyle.Fill;
        grpTables.Text = "2. Danh sách bàn ăn của chi nhánh đang chọn";
        ConfigureGrid(dgvTables);
        dgvTables.Dock = DockStyle.Fill;

        pnlTableForm.Dock = DockStyle.Bottom;
        pnlTableForm.Size = new Size(520, 96);

        var lblTblId = MakeLabel("Mã bàn:", 8, 12);
        txtTableId.Location = new Point(64, 9);
        txtTableId.MaxLength = 15;
        txtTableId.Size = new Size(58, 23);

        var lblTblName = MakeLabel("Tên:", 128, 12);
        txtTableName.Location = new Point(160, 9);
        txtTableName.MaxLength = 50;
        txtTableName.Size = new Size(105, 23);

        var lblTblArea = MakeLabel("Khu:", 272, 12);
        cboTableArea.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTableArea.Location = new Point(306, 9);
        cboTableArea.Size = new Size(106, 23);

        var lblTblCap = MakeLabel("Ghế:", 418, 12);
        numTableCapacity.Location = new Point(450, 9);
        numTableCapacity.Maximum = 10;
        numTableCapacity.Minimum = 1;
        numTableCapacity.Size = new Size(50, 23);
        numTableCapacity.Value = 4;

        chkTableAvailable.AutoSize = true;
        chkTableAvailable.Checked = true;
        chkTableAvailable.Location = new Point(8, 42);
        chkTableAvailable.Text = "Bàn sẵn sàng";

        btnTableNew.Location = new Point(170, 56);
        btnTableNew.Size = new Size(76, 28);
        btnTableNew.Text = "Mã mới";

        btnTableAdd.BackColor = Color.FromArgb(25, 135, 84);
        btnTableAdd.ForeColor = Color.White;
        btnTableAdd.FlatStyle = FlatStyle.Flat;
        btnTableAdd.Location = new Point(252, 56);
        btnTableAdd.Size = new Size(82, 28);
        btnTableAdd.Text = "Thêm bàn";

        btnTableUpdate.BackColor = Color.FromArgb(13, 110, 253);
        btnTableUpdate.ForeColor = Color.White;
        btnTableUpdate.FlatStyle = FlatStyle.Flat;
        btnTableUpdate.Location = new Point(340, 56);
        btnTableUpdate.Size = new Size(82, 28);
        btnTableUpdate.Text = "Sửa bàn";

        btnTableDelete.BackColor = Color.FromArgb(220, 53, 69);
        btnTableDelete.ForeColor = Color.White;
        btnTableDelete.FlatStyle = FlatStyle.Flat;
        btnTableDelete.Location = new Point(428, 56);
        btnTableDelete.Size = new Size(78, 28);
        btnTableDelete.Text = "Xóa bàn";

        pnlTableForm.Controls.AddRange([
            lblTblId, txtTableId, lblTblName, txtTableName, lblTblArea, cboTableArea,
            lblTblCap, numTableCapacity, chkTableAvailable,
            btnTableNew, btnTableAdd, btnTableUpdate, btnTableDelete
        ]);
        grpTables.Controls.Add(dgvTables);
        grpTables.Controls.Add(pnlTableForm);

        splitLeft.Panel1.Controls.Add(grpRestaurants);
        splitLeft.Panel2.Controls.Add(grpTables);
        splitMain.Panel1.Controls.Add(splitLeft);

        // Group 3: Menu Items
        grpMenuItems.Dock = DockStyle.Fill;
        grpMenuItems.Text = "3. Danh sách thực đơn theo chi nhánh";

        pnlMenuFilter.Dock = DockStyle.Top;
        pnlMenuFilter.Size = new Size(650, 38);

        cboMenuFilterRes.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMenuFilterRes.Location = new Point(8, 7);
        cboMenuFilterRes.Size = new Size(185, 23);

        cboMenuFilterCat.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMenuFilterCat.Location = new Point(199, 7);
        cboMenuFilterCat.Size = new Size(120, 23);

        txtMenuKeyword.Location = new Point(325, 7);
        txtMenuKeyword.MaxLength = 100;
        txtMenuKeyword.PlaceholderText = "Tìm tên / mã món...";
        txtMenuKeyword.Size = new Size(140, 23);

        chkMenuOnlyAvail.AutoSize = true;
        chkMenuOnlyAvail.Location = new Point(472, 10);
        chkMenuOnlyAvail.Text = "Còn bán";

        btnMenuFilter.Location = new Point(546, 5);
        btnMenuFilter.Size = new Size(88, 27);
        btnMenuFilter.Text = "Lọc món";

        pnlMenuFilter.Controls.AddRange([cboMenuFilterRes, cboMenuFilterCat, txtMenuKeyword, chkMenuOnlyAvail, btnMenuFilter]);

        ConfigureGrid(dgvMenuItems);
        dgvMenuItems.Dock = DockStyle.Fill;

        pnlMenuForm.Dock = DockStyle.Bottom;
        pnlMenuForm.Size = new Size(650, 198);

        var lblItemCode = MakeLabel("Mã món:", 8, 12);
        txtItemCode.Location = new Point(74, 9);
        txtItemCode.MaxLength = 15;
        txtItemCode.Size = new Size(80, 23);

        var lblItemRes = MakeLabel("Chi nhánh:", 162, 12);
        cboItemRestaurant.DropDownStyle = ComboBoxStyle.DropDownList;
        cboItemRestaurant.Location = new Point(236, 9);
        cboItemRestaurant.Size = new Size(215, 23);

        chkItemAvailable.AutoSize = true;
        chkItemAvailable.Checked = true;
        chkItemAvailable.Location = new Point(462, 11);
        chkItemAvailable.Text = "Đang phục vụ";

        var lblItemName = MakeLabel("Tên món:", 8, 44);
        txtItemName.Location = new Point(74, 41);
        txtItemName.MaxLength = 100;
        txtItemName.Size = new Size(245, 23);

        var lblItemCat = MakeLabel("Nhóm:", 328, 44);
        cboItemCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cboItemCategory.Items.AddRange(["Khai vị", "Món chính", "Lẩu & Nướng", "Đồ uống", "Tráng miệng"]);
        cboItemCategory.SelectedIndex = 1;
        cboItemCategory.Location = new Point(378, 41);
        cboItemCategory.Size = new Size(185, 23);

        var lblItemPrice = MakeLabel("Giá gốc:", 8, 76);
        numItemBasePrice.Increment = 5000;
        numItemBasePrice.Location = new Point(74, 73);
        numItemBasePrice.Maximum = 5000000;
        numItemBasePrice.Minimum = 1000;
        numItemBasePrice.Size = new Size(125, 23);
        numItemBasePrice.ThousandsSeparator = true;
        numItemBasePrice.Value = 50000;

        var lblSizeL = MakeLabel("Cộng size L:", 210, 76);
        numItemSizeLargeExtra.Increment = 5000;
        numItemSizeLargeExtra.Location = new Point(296, 73);
        numItemSizeLargeExtra.Maximum = 2000000;
        numItemSizeLargeExtra.Minimum = 0;
        numItemSizeLargeExtra.Size = new Size(110, 23);
        numItemSizeLargeExtra.ThousandsSeparator = true;
        numItemSizeLargeExtra.Value = 45000;

        var lblImg = MakeLabel("Ảnh:", 416, 76);
        txtItemImageUrl.Location = new Point(452, 73);
        txtItemImageUrl.MaxLength = 255;
        txtItemImageUrl.PlaceholderText = "images/menu/mn031.jpg";
        txtItemImageUrl.Size = new Size(180, 23);

        var lblToppings = MakeLabel("Món kèm:", 8, 108);
        txtItemToppings.Location = new Point(74, 105);
        txtItemToppings.MaxLength = 500;
        txtItemToppings.PlaceholderText = "Ví dụ: Thêm nấm:35000; Ba chỉ bò:65000";
        txtItemToppings.Size = new Size(558, 23);

        btnMenuNew.Location = new Point(8, 146);
        btnMenuNew.Size = new Size(105, 32);
        btnMenuNew.Text = "Tạo mã mới";

        btnMenuAdd.BackColor = Color.FromArgb(25, 135, 84);
        btnMenuAdd.ForeColor = Color.White;
        btnMenuAdd.FlatStyle = FlatStyle.Flat;
        btnMenuAdd.Location = new Point(121, 146);
        btnMenuAdd.Size = new Size(155, 32);
        btnMenuAdd.Text = "Thêm món mới";

        btnMenuUpdate.BackColor = Color.FromArgb(13, 110, 253);
        btnMenuUpdate.ForeColor = Color.White;
        btnMenuUpdate.FlatStyle = FlatStyle.Flat;
        btnMenuUpdate.Location = new Point(284, 146);
        btnMenuUpdate.Size = new Size(165, 32);
        btnMenuUpdate.Text = "Cập nhật món";

        btnMenuDelete.BackColor = Color.FromArgb(220, 53, 69);
        btnMenuDelete.ForeColor = Color.White;
        btnMenuDelete.FlatStyle = FlatStyle.Flat;
        btnMenuDelete.Location = new Point(457, 146);
        btnMenuDelete.Size = new Size(155, 32);
        btnMenuDelete.Text = "Xóa món";

        pnlMenuForm.Controls.AddRange([
            lblItemCode, txtItemCode, lblItemRes, cboItemRestaurant, chkItemAvailable,
            lblItemName, txtItemName, lblItemCat, cboItemCategory,
            lblItemPrice, numItemBasePrice, lblSizeL, numItemSizeLargeExtra, lblImg, txtItemImageUrl,
            lblToppings, txtItemToppings,
            btnMenuNew, btnMenuAdd, btnMenuUpdate, btnMenuDelete
        ]);

        grpMenuItems.Controls.Add(dgvMenuItems);
        grpMenuItems.Controls.Add(pnlMenuFilter);
        grpMenuItems.Controls.Add(pnlMenuForm);
        splitMain.Panel2.Controls.Add(grpMenuItems);

        tabRestaurantMenu.Controls.Add(splitMain);

        // ====================== TAB 2 ======================
        tabStatistics.Padding = new Padding(6);
        tabStatistics.Text = "Báo cáo && thống kê kinh doanh";

        tlpKpi.ColumnCount = 4;
        tlpKpi.Dock = DockStyle.Top;
        tlpKpi.RowCount = 1;
        tlpKpi.Size = new Size(1220, 62);
        for (int i = 0; i < 4; i++)
            tlpKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpKpi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        ConfigureKpiLabel(lblKpiBranches, Color.FromArgb(232, 244, 253), Color.FromArgb(13, 71, 161), "Chi nhánh / bàn\n-");
        ConfigureKpiLabel(lblKpiMenu, Color.FromArgb(237, 247, 237), Color.FromArgb(27, 94, 32), "Thực đơn\n-");
        ConfigureKpiLabel(lblKpiOrders, Color.FromArgb(255, 248, 225), Color.FromArgb(170, 95, 0), "Đơn hàng / hoàn tất\n-");
        ConfigureKpiLabel(lblKpiRevenue, Color.FromArgb(252, 236, 239), Color.FromArgb(155, 28, 49), "Doanh thu hoàn tất\n-");

        tlpKpi.Controls.Add(lblKpiBranches, 0, 0);
        tlpKpi.Controls.Add(lblKpiMenu, 1, 0);
        tlpKpi.Controls.Add(lblKpiOrders, 2, 0);
        tlpKpi.Controls.Add(lblKpiRevenue, 3, 0);

        pnlStatFilter.Dock = DockStyle.Top;
        pnlStatFilter.Size = new Size(1220, 44);

        var lblStatPick = MakeLabel("Báo cáo:", 8, 12);
        cboStatQuery.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatQuery.Location = new Point(74, 9);
        cboStatQuery.Size = new Size(510, 23);

        var lblStatRes = MakeLabel("Chi nhánh:", 596, 12);
        cboStatBranch.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatBranch.Location = new Point(670, 9);
        cboStatBranch.Size = new Size(195, 23);

        btnRunStat.BackColor = Color.FromArgb(21, 101, 192);
        btnRunStat.ForeColor = Color.White;
        btnRunStat.FlatStyle = FlatStyle.Flat;
        btnRunStat.Location = new Point(876, 7);
        btnRunStat.Size = new Size(150, 28);
        btnRunStat.Text = "Làm mới số liệu";

        pnlStatFilter.Controls.AddRange([lblStatPick, cboStatQuery, lblStatRes, cboStatBranch, btnRunStat]);

        splitStats.Dock = DockStyle.Fill;
        splitStats.Orientation = Orientation.Vertical;

        ConfigureGrid(dgvStats);
        dgvStats.Dock = DockStyle.Fill;
        splitStats.Panel1.Controls.Add(dgvStats);

        grpChart.Dock = DockStyle.Fill;
        grpChart.Text = "Biểu đồ cột trực quan";
        pnlBarChart.BackColor = Color.White;
        pnlBarChart.Dock = DockStyle.Fill;
        grpChart.Controls.Add(pnlBarChart);
        splitStats.Panel2.Controls.Add(grpChart);

        tabStatistics.Controls.Add(splitStats);
        tabStatistics.Controls.Add(pnlStatFilter);
        tabStatistics.Controls.Add(tlpKpi);

        // ====================== TAB 3 ======================
        tabBackup.Padding = new Padding(8);
        tabBackup.Text = "Sao lưu && phục hồi dữ liệu";

        grpBackupPaths.Dock = DockStyle.Top;
        grpBackupPaths.Size = new Size(1210, 102);
        grpBackupPaths.Text = "1. Chọn thư mục lưu trữ và tập tin dữ liệu";

        var lblBkFolder = MakeLabel("Thư mục lưu:", 14, 28);
        txtBackupFolder.Location = new Point(112, 25);
        txtBackupFolder.Size = new Size(760, 23);

        btnBrowseBackupFolder.Location = new Point(880, 23);
        btnBrowseBackupFolder.Size = new Size(135, 27);
        btnBrowseBackupFolder.Text = "Chọn thư mục...";

        btnOpenBackupFolder.Location = new Point(1023, 23);
        btnOpenBackupFolder.Size = new Size(135, 27);
        btnOpenBackupFolder.Text = "Mở thư mục";

        var lblCol = MakeLabel("Bảng dữ liệu:", 14, 64);
        cboExportCollection.DropDownStyle = ComboBoxStyle.DropDownList;
        cboExportCollection.Location = new Point(112, 61);
        cboExportCollection.Size = new Size(215, 23);

        var lblJsonFile = MakeLabel("Tập tin JSON:", 338, 64);
        txtImportJsonFile.Location = new Point(424, 61);
        txtImportJsonFile.PlaceholderText = "Chọn tập tin .json để nhập vào bảng...";
        txtImportJsonFile.Size = new Size(448, 23);

        btnBrowseImportFile.Location = new Point(880, 59);
        btnBrowseImportFile.Size = new Size(135, 27);
        btnBrowseImportFile.Text = "Chọn tập tin .json...";

        chkImportDrop.AutoSize = true;
        chkImportDrop.Location = new Point(1023, 63);
        chkImportDrop.Text = "Xóa dữ liệu cũ trước khi nhập";

        grpBackupPaths.Controls.AddRange([
            lblBkFolder, txtBackupFolder, btnBrowseBackupFolder, btnOpenBackupFolder,
            lblCol, cboExportCollection, lblJsonFile, txtImportJsonFile, btnBrowseImportFile, chkImportDrop
        ]);

        grpBackupActions.Dock = DockStyle.Top;
        grpBackupActions.Size = new Size(1210, 146);
        grpBackupActions.Text = "2. Thao tác sao lưu và khôi phục dữ liệu";

        btnMongoDump.BackColor = Color.FromArgb(25, 135, 84);
        btnMongoDump.ForeColor = Color.White;
        btnMongoDump.FlatStyle = FlatStyle.Flat;
        btnMongoDump.Location = new Point(14, 24);
        btnMongoDump.Size = new Size(275, 34);
        btnMongoDump.Text = "Sao lưu dữ liệu chính";

        btnMongoRestore.BackColor = Color.FromArgb(176, 42, 55);
        btnMongoRestore.ForeColor = Color.White;
        btnMongoRestore.FlatStyle = FlatStyle.Flat;
        btnMongoRestore.Location = new Point(298, 24);
        btnMongoRestore.Size = new Size(285, 34);
        btnMongoRestore.Text = "Khôi phục dữ liệu từ bản sao lưu";

        btnExportAllJson.BackColor = Color.FromArgb(13, 110, 253);
        btnExportAllJson.ForeColor = Color.White;
        btnExportAllJson.FlatStyle = FlatStyle.Flat;
        btnExportAllJson.Location = new Point(592, 24);
        btnExportAllJson.Size = new Size(285, 34);
        btnExportAllJson.Text = "Xuất 5 bảng ra tập tin (JSON)";

        btnImportJson.BackColor = Color.FromArgb(198, 120, 0);
        btnImportJson.ForeColor = Color.White;
        btnImportJson.FlatStyle = FlatStyle.Flat;
        btnImportJson.Location = new Point(886, 24);
        btnImportJson.Size = new Size(285, 34);
        btnImportJson.Text = "Nhập dữ liệu vào bảng từ tập tin (JSON)";

        btnMemuraiBgSave.BackColor = Color.FromArgb(111, 66, 193);
        btnMemuraiBgSave.ForeColor = Color.White;
        btnMemuraiBgSave.FlatStyle = FlatStyle.Flat;
        btnMemuraiBgSave.Location = new Point(14, 68);
        btnMemuraiBgSave.Size = new Size(275, 34);
        btnMemuraiBgSave.Text = "Lưu trạng thái bộ nhớ tạm";

        btnMemuraiBackupFiles.BackColor = Color.FromArgb(32, 120, 136);
        btnMemuraiBackupFiles.ForeColor = Color.White;
        btnMemuraiBackupFiles.FlatStyle = FlatStyle.Flat;
        btnMemuraiBackupFiles.Location = new Point(298, 68);
        btnMemuraiBackupFiles.Size = new Size(285, 34);
        btnMemuraiBackupFiles.Text = "Chép tập tin dự phòng bộ nhớ tạm";

        btnMemuraiStatus.Location = new Point(592, 68);
        btnMemuraiStatus.Size = new Size(285, 34);
        btnMemuraiStatus.Text = "Kiểm tra chế độ tự động lưu";

        lblMemuraiPersist.AutoSize = true;
        lblMemuraiPersist.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMemuraiPersist.ForeColor = Color.FromArgb(30, 64, 175);
        lblMemuraiPersist.Location = new Point(14, 112);
        lblMemuraiPersist.Text = "Tình trạng tự động lưu bộ nhớ tạm: đang kiểm tra...";

        grpBackupActions.Controls.AddRange([
            btnMongoDump, btnMongoRestore, btnExportAllJson, btnImportJson,
            btnMemuraiBgSave, btnMemuraiBackupFiles, btnMemuraiStatus, lblMemuraiPersist
        ]);

        grpBackupLog.Dock = DockStyle.Fill;
        grpBackupLog.Text = "3. Nhật ký chi tiết quá trình thực hiện";

        txtBackupLog.BackColor = Color.FromArgb(24, 24, 27);
        txtBackupLog.Dock = DockStyle.Fill;
        txtBackupLog.Font = new Font("Consolas", 9.5F);
        txtBackupLog.ForeColor = Color.FromArgb(220, 245, 220);
        txtBackupLog.Multiline = true;
        txtBackupLog.ReadOnly = true;
        txtBackupLog.ScrollBars = ScrollBars.Both;
        txtBackupLog.WordWrap = false;
        grpBackupLog.Controls.Add(txtBackupLog);

        tabBackup.Controls.Add(grpBackupLog);
        tabBackup.Controls.Add(grpBackupActions);
        tabBackup.Controls.Add(grpBackupPaths);

        // ====================== MAIN FORM ======================
        tabAdmin.Controls.Add(tabRestaurantMenu);
        tabAdmin.Controls.Add(tabStatistics);
        tabAdmin.Controls.Add(tabBackup);
        tabAdmin.Dock = DockStyle.Fill;
        tabAdmin.Location = new Point(0, 38);
        tabAdmin.Size = new Size(1240, 722);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1240, 760);
        MinimumSize = new Size(1060, 680);
        MaximizeBox = true;
        Controls.Add(tabAdmin);
        Controls.Add(lblInfo);
        Name = "FrmAdmin";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Quản trị nhà hàng, thống kê kinh doanh & sao lưu dữ liệu";

        tabAdmin.ResumeLayout(false);
        tabRestaurantMenu.ResumeLayout(false);
        splitMain.Panel1.ResumeLayout(false);
        splitMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
        splitMain.ResumeLayout(false);
        splitLeft.Panel1.ResumeLayout(false);
        splitLeft.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitLeft).EndInit();
        splitLeft.ResumeLayout(false);
        grpRestaurants.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvRestaurants).EndInit();
        pnlResForm.ResumeLayout(false);
        pnlResForm.PerformLayout();
        grpTables.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTables).EndInit();
        pnlTableForm.ResumeLayout(false);
        pnlTableForm.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numTableCapacity).EndInit();
        grpMenuItems.ResumeLayout(false);
        pnlMenuFilter.ResumeLayout(false);
        pnlMenuFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvMenuItems).EndInit();
        pnlMenuForm.ResumeLayout(false);
        pnlMenuForm.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numItemBasePrice).EndInit();
        ((System.ComponentModel.ISupportInitialize)numItemSizeLargeExtra).EndInit();

        tabStatistics.ResumeLayout(false);
        tlpKpi.ResumeLayout(false);
        pnlStatFilter.ResumeLayout(false);
        pnlStatFilter.PerformLayout();
        splitStats.Panel1.ResumeLayout(false);
        splitStats.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitStats).EndInit();
        splitStats.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvStats).EndInit();
        grpChart.ResumeLayout(false);

        tabBackup.ResumeLayout(false);
        grpBackupPaths.ResumeLayout(false);
        grpBackupPaths.PerformLayout();
        grpBackupActions.ResumeLayout(false);
        grpBackupActions.PerformLayout();
        grpBackupLog.ResumeLayout(false);
        grpBackupLog.PerformLayout();
        ResumeLayout(false);
    }

    private static Label MakeLabel(string text, int x, int y) =>
        new()
        {
            AutoSize = true,
            Location = new Point(x, y),
            Text = text
        };

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.MultiSelect = false;
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    private static void ConfigureKpiLabel(Label label, Color backColor, Color foreColor, string text)
    {
        label.BackColor = backColor;
        label.BorderStyle = BorderStyle.FixedSingle;
        label.Dock = DockStyle.Fill;
        label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        label.ForeColor = foreColor;
        label.Margin = new Padding(4);
        label.Text = text;
        label.TextAlign = ContentAlignment.MiddleCenter;
    }
}
