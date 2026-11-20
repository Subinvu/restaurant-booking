namespace RestaurantBooking.Forms.TV1_Booking;

partial class FrmBooking
{
    private System.ComponentModel.IContainer components = null!;
    private System.Windows.Forms.Timer tmrCountdown = null!;
    private Panel pnlHeader = null!;
    private Label lblInfo = null!;
    private Button btnBack = null!;
    private TabControl tabMain = null!;
    private TabPage tabBooking = null!;
    private TabPage tabManageOrders = null!;

    // Tab 1 controls
    private Panel pnlTopFilter = null!;
    private Label lblResLabel = null!;
    private ComboBox cboRestaurant = null!;
    private Label lblDateLabel = null!;
    private DateTimePicker dtpBookingDate = null!;
    private Label lblSlotLabel = null!;
    private ComboBox cboTimeSlot = null!;
    private Label lblGuestLabel = null!;
    private NumericUpDown nudGuestCount = null!;
    private Button btnReloadTables = null!;
    private Button btnSwitchTakeaway = null!;

    private GroupBox grpTableMap = null!;
    private Label lblLegend = null!;
    private FlowLayoutPanel flpTables = null!;
    private Panel pnlWaitlistBar = null!;
    private Label lblWaitlistCount = null!;
    private Button btnJoinWaitlist = null!;

    private GroupBox grpOrderSummary = null!;
    private Label lblSelectedTableInfo = null!;
    private Button btnLockTable = null!;
    private Button btnReleaseLock = null!;
    private Label lblCartSource = null!;
    private Button btnReloadCart = null!;
    private DataGridView dgvCart = null!;
    private Label lblSubtotal = null!;
    private Label lblVoucherLabel = null!;
    private TextBox txtVoucher = null!;
    private Label lblDiscount = null!;
    private Label lblTotalAmount = null!;
    private Label lblPaymentLabel = null!;
    private ComboBox cboPaymentMethod = null!;
    private Button btnConfirmBooking = null!;

    // Tab 2 controls
    private Panel pnlManageFilter = null!;
    private Label lblMngRes = null!;
    private ComboBox cboManageRestaurant = null!;
    private CheckBox chkFilterDate = null!;
    private DateTimePicker dtpManageDate = null!;
    private Label lblMngStatus = null!;
    private ComboBox cboManageStatus = null!;
    private Button btnFilterOrders = null!;
    private DataGridView dgvOrders = null!;
    private Panel pnlManageActions = null!;
    private Button btnConfirmOrder = null!;
    private Button btnServingOrder = null!;
    private Button btnCompleteOrder = null!;
    private Button btnCancelOrder = null!;
    private GroupBox grpManageWaitlist = null!;
    private Label lblManageWaitlistSlot = null!;
    private DataGridView dgvManageWaitlist = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        tmrCountdown = new System.Windows.Forms.Timer(components);

        pnlHeader = new Panel();
        lblInfo = new Label();
        btnBack = new Button();
        tabMain = new TabControl();
        tabBooking = new TabPage();
        tabManageOrders = new TabPage();

        pnlTopFilter = new Panel();
        lblResLabel = new Label();
        cboRestaurant = new ComboBox();
        lblDateLabel = new Label();
        dtpBookingDate = new DateTimePicker();
        lblSlotLabel = new Label();
        cboTimeSlot = new ComboBox();
        lblGuestLabel = new Label();
        nudGuestCount = new NumericUpDown();
        btnReloadTables = new Button();
        btnSwitchTakeaway = new Button();

        grpTableMap = new GroupBox();
        lblLegend = new Label();
        flpTables = new FlowLayoutPanel();
        pnlWaitlistBar = new Panel();
        lblWaitlistCount = new Label();
        btnJoinWaitlist = new Button();

        grpOrderSummary = new GroupBox();
        lblSelectedTableInfo = new Label();
        btnLockTable = new Button();
        btnReleaseLock = new Button();
        lblCartSource = new Label();
        btnReloadCart = new Button();
        dgvCart = new DataGridView();
        lblSubtotal = new Label();
        lblVoucherLabel = new Label();
        txtVoucher = new TextBox();
        lblDiscount = new Label();
        lblTotalAmount = new Label();
        lblPaymentLabel = new Label();
        cboPaymentMethod = new ComboBox();
        btnConfirmBooking = new Button();

        pnlManageFilter = new Panel();
        lblMngRes = new Label();
        cboManageRestaurant = new ComboBox();
        chkFilterDate = new CheckBox();
        dtpManageDate = new DateTimePicker();
        lblMngStatus = new Label();
        cboManageStatus = new ComboBox();
        btnFilterOrders = new Button();
        dgvOrders = new DataGridView();
        pnlManageActions = new Panel();
        btnConfirmOrder = new Button();
        btnServingOrder = new Button();
        btnCompleteOrder = new Button();
        btnCancelOrder = new Button();
        grpManageWaitlist = new GroupBox();
        lblManageWaitlistSlot = new Label();
        dgvManageWaitlist = new DataGridView();

        pnlHeader.SuspendLayout();
        tabMain.SuspendLayout();
        tabBooking.SuspendLayout();
        tabManageOrders.SuspendLayout();
        pnlTopFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudGuestCount).BeginInit();
        grpTableMap.SuspendLayout();
        pnlWaitlistBar.SuspendLayout();
        grpOrderSummary.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCart).BeginInit();
        pnlManageFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvOrders).BeginInit();
        pnlManageActions.SuspendLayout();
        grpManageWaitlist.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvManageWaitlist).BeginInit();
        SuspendLayout();

        // tmrCountdown
        tmrCountdown.Interval = 1000;
        tmrCountdown.Tick += tmrCountdown_Tick;

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1124, 52);

        lblInfo.AutoSize = true;
        lblInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblInfo.ForeColor = Color.White;
        lblInfo.Location = new Point(16, 14);
        lblInfo.Name = "lblInfo";
        lblInfo.Text = "Đặt bàn trực tuyến — Nhà hàng Hương Việt";
        lblInfo.UseMnemonic = false;

        btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBack.BackColor = Color.FromArgb(51, 65, 85);
        btnBack.FlatStyle = FlatStyle.Flat;
        btnBack.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnBack.ForeColor = Color.White;
        btnBack.Location = new Point(988, 9);
        btnBack.Name = "btnBack";
        btnBack.Size = new Size(120, 34);
        btnBack.Text = "← Quay lại";
        btnBack.Click += btnBack_Click;

        pnlHeader.Controls.Add(lblInfo);
        pnlHeader.Controls.Add(btnBack);

        // === TAB 1 TOP FILTER ===
        pnlTopFilter.BackColor = Color.FromArgb(248, 250, 252);
        pnlTopFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlTopFilter.Dock = DockStyle.Top;
        pnlTopFilter.Location = new Point(0, 0);
        pnlTopFilter.Name = "pnlTopFilter";
        pnlTopFilter.Size = new Size(1116, 52);

        lblResLabel.AutoSize = true;
        lblResLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblResLabel.Location = new Point(12, 16);
        lblResLabel.Name = "lblResLabel";
        lblResLabel.Text = "Chi nhánh:";

        cboRestaurant.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRestaurant.Location = new Point(84, 12);
        cboRestaurant.Name = "cboRestaurant";
        cboRestaurant.Size = new Size(216, 25);
        cboRestaurant.SelectedIndexChanged += FilterInputs_Changed;

        lblDateLabel.AutoSize = true;
        lblDateLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDateLabel.Location = new Point(312, 16);
        lblDateLabel.Name = "lblDateLabel";
        lblDateLabel.Text = "Ngày:";

        dtpBookingDate.CustomFormat = "dd/MM/yyyy";
        dtpBookingDate.Format = DateTimePickerFormat.Custom;
        dtpBookingDate.Location = new Point(358, 12);
        dtpBookingDate.Name = "dtpBookingDate";
        dtpBookingDate.Size = new Size(106, 25);
        dtpBookingDate.ValueChanged += FilterInputs_Changed;

        lblSlotLabel.AutoSize = true;
        lblSlotLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSlotLabel.Location = new Point(478, 16);
        lblSlotLabel.Name = "lblSlotLabel";
        lblSlotLabel.Text = "Khung giờ:";

        cboTimeSlot.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTimeSlot.Location = new Point(560, 12);
        cboTimeSlot.Name = "cboTimeSlot";
        cboTimeSlot.Size = new Size(80, 25);
        cboTimeSlot.SelectedIndexChanged += FilterInputs_Changed;

        lblGuestLabel.AutoSize = true;
        lblGuestLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblGuestLabel.Location = new Point(656, 16);
        lblGuestLabel.Name = "lblGuestLabel";
        lblGuestLabel.Text = "Số khách:";

        nudGuestCount.Location = new Point(728, 13);
        nudGuestCount.Maximum = 20;
        nudGuestCount.Minimum = 1;
        nudGuestCount.Name = "nudGuestCount";
        nudGuestCount.Size = new Size(56, 25);
        nudGuestCount.Value = 4;

        btnReloadTables.BackColor = Color.White;
        btnReloadTables.FlatStyle = FlatStyle.Flat;
        btnReloadTables.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnReloadTables.Location = new Point(800, 9);
        btnReloadTables.Name = "btnReloadTables";
        btnReloadTables.Size = new Size(126, 32);
        btnReloadTables.Text = "Làm mới sơ đồ";
        btnReloadTables.Click += btnReloadTables_Click;

        btnSwitchTakeaway.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSwitchTakeaway.BackColor = Color.White;
        btnSwitchTakeaway.FlatStyle = FlatStyle.Flat;
        btnSwitchTakeaway.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSwitchTakeaway.Location = new Point(936, 9);
        btnSwitchTakeaway.Name = "btnSwitchTakeaway";
        btnSwitchTakeaway.Size = new Size(168, 32);
        btnSwitchTakeaway.Text = "Đặt mang về / Ship →";
        btnSwitchTakeaway.Click += btnSwitchTakeaway_Click;

        pnlTopFilter.Controls.Add(lblResLabel);
        pnlTopFilter.Controls.Add(cboRestaurant);
        pnlTopFilter.Controls.Add(lblDateLabel);
        pnlTopFilter.Controls.Add(dtpBookingDate);
        pnlTopFilter.Controls.Add(lblSlotLabel);
        pnlTopFilter.Controls.Add(cboTimeSlot);
        pnlTopFilter.Controls.Add(lblGuestLabel);
        pnlTopFilter.Controls.Add(nudGuestCount);
        pnlTopFilter.Controls.Add(btnReloadTables);
        pnlTopFilter.Controls.Add(btnSwitchTakeaway);

        // === TAB 1 LEFT: TABLE MAP ===
        grpTableMap.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpTableMap.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpTableMap.Location = new Point(8, 60);
        grpTableMap.Name = "grpTableMap";
        grpTableMap.Size = new Size(666, 560);
        grpTableMap.Text = "Sơ đồ bàn theo khung giờ";

        lblLegend.AutoSize = true;
        lblLegend.Font = new Font("Segoe UI", 8.5F);
        lblLegend.ForeColor = Color.FromArgb(71, 85, 105);
        lblLegend.Location = new Point(12, 24);
        lblLegend.Name = "lblLegend";
        lblLegend.Text = "Trạng thái:  [Xanh lá] Trống   •   [Cam] Đang giữ chỗ (15 phút)   •   [Đỏ] Đã đặt   •   [Xám] Ngưng phục vụ";

        flpTables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        flpTables.AutoScroll = true;
        flpTables.BackColor = Color.White;
        flpTables.BorderStyle = BorderStyle.FixedSingle;
        flpTables.Location = new Point(12, 46);
        flpTables.Name = "flpTables";
        flpTables.Padding = new Padding(8);
        flpTables.Size = new Size(642, 452);

        pnlWaitlistBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlWaitlistBar.BackColor = Color.FromArgb(248, 250, 252);
        pnlWaitlistBar.BorderStyle = BorderStyle.FixedSingle;
        pnlWaitlistBar.Location = new Point(12, 506);
        pnlWaitlistBar.Name = "pnlWaitlistBar";
        pnlWaitlistBar.Size = new Size(642, 44);

        lblWaitlistCount.AutoSize = true;
        lblWaitlistCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblWaitlistCount.ForeColor = Color.FromArgb(51, 65, 85);
        lblWaitlistCount.Location = new Point(10, 13);
        lblWaitlistCount.Name = "lblWaitlistCount";
        lblWaitlistCount.Text = "Danh sách chờ khung giờ này: 0 khách";
        lblWaitlistCount.UseMnemonic = false;

        btnJoinWaitlist.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnJoinWaitlist.BackColor = Color.White;
        btnJoinWaitlist.FlatStyle = FlatStyle.Flat;
        btnJoinWaitlist.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnJoinWaitlist.Location = new Point(416, 6);
        btnJoinWaitlist.Name = "btnJoinWaitlist";
        btnJoinWaitlist.Size = new Size(216, 30);
        btnJoinWaitlist.Text = "Đăng ký danh sách chờ";
        btnJoinWaitlist.Click += btnJoinWaitlist_Click;

        pnlWaitlistBar.Controls.Add(lblWaitlistCount);
        pnlWaitlistBar.Controls.Add(btnJoinWaitlist);

        grpTableMap.Controls.Add(lblLegend);
        grpTableMap.Controls.Add(flpTables);
        grpTableMap.Controls.Add(pnlWaitlistBar);

        // === TAB 1 RIGHT: SUMMARY & CHECKOUT ===
        grpOrderSummary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        grpOrderSummary.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpOrderSummary.Location = new Point(682, 60);
        grpOrderSummary.Name = "grpOrderSummary";
        grpOrderSummary.Size = new Size(426, 560);
        grpOrderSummary.Text = "Thông tin giữ bàn && Xác nhận đặt bàn";

        lblSelectedTableInfo.BackColor = Color.FromArgb(248, 250, 252);
        lblSelectedTableInfo.BorderStyle = BorderStyle.FixedSingle;
        lblSelectedTableInfo.Font = new Font("Segoe UI", 9F);
        lblSelectedTableInfo.Location = new Point(12, 24);
        lblSelectedTableInfo.Name = "lblSelectedTableInfo";
        lblSelectedTableInfo.Padding = new Padding(8, 6, 8, 6);
        lblSelectedTableInfo.Size = new Size(402, 52);
        lblSelectedTableInfo.Text = "Chưa chọn bàn. Bấm vào một bàn Trống ở sơ đồ bên trái.";
        lblSelectedTableInfo.UseMnemonic = false;

        btnLockTable.BackColor = Color.White;
        btnLockTable.FlatStyle = FlatStyle.Flat;
        btnLockTable.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLockTable.Location = new Point(12, 82);
        btnLockTable.Name = "btnLockTable";
        btnLockTable.Size = new Size(196, 32);
        btnLockTable.Text = "Giữ bàn (15 phút)";
        btnLockTable.Click += btnLockTable_Click;

        btnReleaseLock.BackColor = Color.White;
        btnReleaseLock.FlatStyle = FlatStyle.Flat;
        btnReleaseLock.Font = new Font("Segoe UI", 9F);
        btnReleaseLock.Location = new Point(218, 82);
        btnReleaseLock.Name = "btnReleaseLock";
        btnReleaseLock.Size = new Size(196, 32);
        btnReleaseLock.Text = "Bỏ giữ bàn";
        btnReleaseLock.Click += btnReleaseLock_Click;

        lblCartSource.AutoSize = true;
        lblCartSource.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCartSource.ForeColor = Color.FromArgb(51, 65, 85);
        lblCartSource.Location = new Point(12, 126);
        lblCartSource.Name = "lblCartSource";
        lblCartSource.Text = "Món ăn đặt trước (tùy chọn):";

        btnReloadCart.FlatStyle = FlatStyle.Flat;
        btnReloadCart.Font = new Font("Segoe UI", 8.5F);
        btnReloadCart.Location = new Point(322, 121);
        btnReloadCart.Name = "btnReloadCart";
        btnReloadCart.Size = new Size(92, 26);
        btnReloadCart.Text = "Tải lại giỏ";
        btnReloadCart.Click += btnReloadCart_Click;

        dgvCart.AllowUserToAddRows = false;
        dgvCart.AllowUserToDeleteRows = false;
        dgvCart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCart.BackgroundColor = Color.White;
        dgvCart.Location = new Point(12, 152);
        dgvCart.Name = "dgvCart";
        dgvCart.ReadOnly = true;
        dgvCart.RowHeadersVisible = false;
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCart.Size = new Size(402, 176);

        lblSubtotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblSubtotal.AutoSize = true;
        lblSubtotal.Font = new Font("Segoe UI", 9.5F);
        lblSubtotal.Location = new Point(12, 340);
        lblSubtotal.Name = "lblSubtotal";
        lblSubtotal.Text = "Tạm tính món ăn: 0 đ";

        lblVoucherLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblVoucherLabel.AutoSize = true;
        lblVoucherLabel.Font = new Font("Segoe UI", 9F);
        lblVoucherLabel.Location = new Point(12, 370);
        lblVoucherLabel.Name = "lblVoucherLabel";
        lblVoucherLabel.Text = "Mã giảm giá:";

        txtVoucher.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtVoucher.Font = new Font("Segoe UI", 9.5F);
        txtVoucher.Location = new Point(180, 366);
        txtVoucher.Name = "txtVoucher";
        txtVoucher.PlaceholderText = "Ví dụ: HUIT10";
        txtVoucher.Size = new Size(234, 25);
        txtVoucher.TextChanged += txtVoucher_TextChanged;

        lblDiscount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblDiscount.AutoSize = true;
        lblDiscount.Font = new Font("Segoe UI", 9F);
        lblDiscount.ForeColor = Color.FromArgb(21, 128, 61);
        lblDiscount.Location = new Point(12, 402);
        lblDiscount.Name = "lblDiscount";
        lblDiscount.Text = "Giảm giá: 0 đ";

        lblTotalAmount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblTotalAmount.AutoSize = true;
        lblTotalAmount.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        lblTotalAmount.ForeColor = Color.FromArgb(15, 23, 42);
        lblTotalAmount.Location = new Point(12, 428);
        lblTotalAmount.Name = "lblTotalAmount";
        lblTotalAmount.Text = "Tổng thanh toán: 0 đ";

        lblPaymentLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblPaymentLabel.AutoSize = true;
        lblPaymentLabel.Font = new Font("Segoe UI", 9F);
        lblPaymentLabel.Location = new Point(12, 464);
        lblPaymentLabel.Name = "lblPaymentLabel";
        lblPaymentLabel.Text = "Phương thức thanh toán:";

        cboPaymentMethod.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        cboPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPaymentMethod.Location = new Point(180, 460);
        cboPaymentMethod.Name = "cboPaymentMethod";
        cboPaymentMethod.Size = new Size(234, 25);

        btnConfirmBooking.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        btnConfirmBooking.BackColor = Color.FromArgb(21, 128, 61);
        btnConfirmBooking.FlatStyle = FlatStyle.Flat;
        btnConfirmBooking.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnConfirmBooking.ForeColor = Color.White;
        btnConfirmBooking.Location = new Point(12, 498);
        btnConfirmBooking.Name = "btnConfirmBooking";
        btnConfirmBooking.Size = new Size(402, 48);
        btnConfirmBooking.Text = "Xác nhận đặt bàn";
        btnConfirmBooking.Click += btnConfirmBooking_Click;

        grpOrderSummary.Controls.Add(lblSelectedTableInfo);
        grpOrderSummary.Controls.Add(btnLockTable);
        grpOrderSummary.Controls.Add(btnReleaseLock);
        grpOrderSummary.Controls.Add(lblCartSource);
        grpOrderSummary.Controls.Add(btnReloadCart);
        grpOrderSummary.Controls.Add(dgvCart);
        grpOrderSummary.Controls.Add(lblSubtotal);
        grpOrderSummary.Controls.Add(lblVoucherLabel);
        grpOrderSummary.Controls.Add(txtVoucher);
        grpOrderSummary.Controls.Add(lblDiscount);
        grpOrderSummary.Controls.Add(lblTotalAmount);
        grpOrderSummary.Controls.Add(lblPaymentLabel);
        grpOrderSummary.Controls.Add(cboPaymentMethod);
        grpOrderSummary.Controls.Add(btnConfirmBooking);

        tabBooking.AutoScroll = true;
        tabBooking.BackColor = Color.White;
        tabBooking.Location = new Point(4, 34);
        tabBooking.Name = "tabBooking";
        tabBooking.Size = new Size(1116, 628);
        tabBooking.Controls.Add(grpOrderSummary);
        tabBooking.Controls.Add(grpTableMap);
        tabBooking.Controls.Add(pnlTopFilter);
        tabBooking.Text = "Đặt bàn & Sơ đồ bàn";

        // === TAB 2 MANAGE ORDERS ===
        pnlManageFilter.BackColor = Color.FromArgb(248, 250, 252);
        pnlManageFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlManageFilter.Dock = DockStyle.Top;
        pnlManageFilter.Location = new Point(0, 0);
        pnlManageFilter.Name = "pnlManageFilter";
        pnlManageFilter.Size = new Size(1116, 52);

        lblMngRes.AutoSize = true;
        lblMngRes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMngRes.Location = new Point(12, 16);
        lblMngRes.Name = "lblMngRes";
        lblMngRes.Text = "Chi nhánh:";

        cboManageRestaurant.DropDownStyle = ComboBoxStyle.DropDownList;
        cboManageRestaurant.Location = new Point(84, 12);
        cboManageRestaurant.Name = "cboManageRestaurant";
        cboManageRestaurant.Size = new Size(210, 25);

        chkFilterDate.AutoSize = true;
        chkFilterDate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        chkFilterDate.Location = new Point(308, 15);
        chkFilterDate.Name = "chkFilterDate";
        chkFilterDate.Text = "Lọc theo ngày:";
        chkFilterDate.CheckedChanged += chkFilterDate_CheckedChanged;

        dtpManageDate.CustomFormat = "dd/MM/yyyy";
        dtpManageDate.Enabled = false;
        dtpManageDate.Format = DateTimePickerFormat.Custom;
        dtpManageDate.Location = new Point(438, 12);
        dtpManageDate.Name = "dtpManageDate";
        dtpManageDate.Size = new Size(118, 25);

        lblMngStatus.AutoSize = true;
        lblMngStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMngStatus.Location = new Point(572, 16);
        lblMngStatus.Name = "lblMngStatus";
        lblMngStatus.Text = "Trạng thái:";

        cboManageStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboManageStatus.Location = new Point(650, 12);
        cboManageStatus.Name = "cboManageStatus";
        cboManageStatus.Size = new Size(155, 25);

        btnFilterOrders.BackColor = Color.White;
        btnFilterOrders.FlatStyle = FlatStyle.Flat;
        btnFilterOrders.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnFilterOrders.Location = new Point(822, 9);
        btnFilterOrders.Name = "btnFilterOrders";
        btnFilterOrders.Size = new Size(140, 32);
        btnFilterOrders.Text = "Lọc đơn đặt";
        btnFilterOrders.Click += btnFilterOrders_Click;

        pnlManageFilter.Controls.Add(lblMngRes);
        pnlManageFilter.Controls.Add(cboManageRestaurant);
        pnlManageFilter.Controls.Add(chkFilterDate);
        pnlManageFilter.Controls.Add(dtpManageDate);
        pnlManageFilter.Controls.Add(lblMngStatus);
        pnlManageFilter.Controls.Add(cboManageStatus);
        pnlManageFilter.Controls.Add(btnFilterOrders);

        dgvOrders.AllowUserToAddRows = false;
        dgvOrders.AllowUserToDeleteRows = false;
        dgvOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOrders.BackgroundColor = Color.White;
        dgvOrders.Location = new Point(10, 62);
        dgvOrders.MultiSelect = false;
        dgvOrders.Name = "dgvOrders";
        dgvOrders.ReadOnly = true;
        dgvOrders.RowHeadersVisible = false;
        dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrders.Size = new Size(1096, 308);
        dgvOrders.SelectionChanged += dgvOrders_SelectionChanged;

        pnlManageActions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlManageActions.Location = new Point(10, 378);
        pnlManageActions.Name = "pnlManageActions";
        pnlManageActions.Size = new Size(1096, 44);

        btnConfirmOrder.BackColor = Color.White;
        btnConfirmOrder.FlatStyle = FlatStyle.Flat;
        btnConfirmOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnConfirmOrder.Location = new Point(0, 4);
        btnConfirmOrder.Name = "btnConfirmOrder";
        btnConfirmOrder.Size = new Size(190, 36);
        btnConfirmOrder.Text = "1. Xác nhận đơn";
        btnConfirmOrder.Click += btnConfirmOrder_Click;

        btnServingOrder.BackColor = Color.White;
        btnServingOrder.FlatStyle = FlatStyle.Flat;
        btnServingOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnServingOrder.Location = new Point(202, 4);
        btnServingOrder.Name = "btnServingOrder";
        btnServingOrder.Size = new Size(200, 36);
        btnServingOrder.Text = "2. Bắt đầu phục vụ";
        btnServingOrder.Click += btnServingOrder_Click;

        btnCompleteOrder.BackColor = Color.FromArgb(21, 128, 61);
        btnCompleteOrder.FlatStyle = FlatStyle.Flat;
        btnCompleteOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCompleteOrder.ForeColor = Color.White;
        btnCompleteOrder.Location = new Point(414, 4);
        btnCompleteOrder.Name = "btnCompleteOrder";
        btnCompleteOrder.Size = new Size(190, 36);
        btnCompleteOrder.Text = "3. Hoàn tất đơn";
        btnCompleteOrder.Click += btnCompleteOrder_Click;

        btnCancelOrder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancelOrder.BackColor = Color.FromArgb(254, 242, 242);
        btnCancelOrder.FlatStyle = FlatStyle.Flat;
        btnCancelOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCancelOrder.ForeColor = Color.FromArgb(185, 28, 28);
        btnCancelOrder.Location = new Point(806, 4);
        btnCancelOrder.Name = "btnCancelOrder";
        btnCancelOrder.Size = new Size(290, 36);
        btnCancelOrder.Text = "Hủy đơn && Gọi khách chờ kế tiếp";
        btnCancelOrder.Click += btnCancelOrder_Click;

        pnlManageActions.Controls.Add(btnConfirmOrder);
        pnlManageActions.Controls.Add(btnServingOrder);
        pnlManageActions.Controls.Add(btnCompleteOrder);
        pnlManageActions.Controls.Add(btnCancelOrder);

        grpManageWaitlist.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpManageWaitlist.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpManageWaitlist.Location = new Point(10, 428);
        grpManageWaitlist.Name = "grpManageWaitlist";
        grpManageWaitlist.Size = new Size(1096, 192);
        grpManageWaitlist.Text = "Danh sách khách chờ bàn theo khung giờ";

        lblManageWaitlistSlot.AutoSize = true;
        lblManageWaitlistSlot.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblManageWaitlistSlot.ForeColor = Color.FromArgb(71, 85, 105);
        lblManageWaitlistSlot.Location = new Point(12, 24);
        lblManageWaitlistSlot.Name = "lblManageWaitlistSlot";
        lblManageWaitlistSlot.Text = "Chọn một đơn ăn tại bàn ở trên để xem danh sách khách đang chờ cùng khung giờ.";
        lblManageWaitlistSlot.UseMnemonic = false;

        dgvManageWaitlist.AllowUserToAddRows = false;
        dgvManageWaitlist.AllowUserToDeleteRows = false;
        dgvManageWaitlist.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvManageWaitlist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvManageWaitlist.BackgroundColor = Color.White;
        dgvManageWaitlist.Location = new Point(12, 48);
        dgvManageWaitlist.Name = "dgvManageWaitlist";
        dgvManageWaitlist.ReadOnly = true;
        dgvManageWaitlist.RowHeadersVisible = false;
        dgvManageWaitlist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvManageWaitlist.Size = new Size(1072, 132);

        grpManageWaitlist.Controls.Add(lblManageWaitlistSlot);
        grpManageWaitlist.Controls.Add(dgvManageWaitlist);

        tabManageOrders.AutoScroll = true;
        tabManageOrders.BackColor = Color.White;
        tabManageOrders.Location = new Point(4, 34);
        tabManageOrders.Name = "tabManageOrders";
        tabManageOrders.Size = new Size(1116, 628);
        tabManageOrders.Controls.Add(grpManageWaitlist);
        tabManageOrders.Controls.Add(pnlManageActions);
        tabManageOrders.Controls.Add(dgvOrders);
        tabManageOrders.Controls.Add(pnlManageFilter);
        tabManageOrders.Text = "Quản lý & Duyệt đơn";

        tabMain.Controls.Add(tabBooking);
        tabMain.Controls.Add(tabManageOrders);
        tabMain.Dock = DockStyle.Fill;
        tabMain.Location = new Point(0, 52);
        tabMain.Name = "tabMain";
        tabMain.Size = new Size(1124, 658);
        tabMain.SelectedIndexChanged += tabMain_SelectedIndexChanged;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(1124, 710);
        Controls.Add(tabMain);
        Controls.Add(pnlHeader);
        MinimumSize = new Size(920, 580);
        Name = "FrmBooking";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Đặt bàn trực tuyến — Nhà hàng Hương Việt";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudGuestCount).EndInit();
        pnlTopFilter.ResumeLayout(false);
        pnlTopFilter.PerformLayout();
        pnlWaitlistBar.ResumeLayout(false);
        pnlWaitlistBar.PerformLayout();
        grpTableMap.ResumeLayout(false);
        grpTableMap.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCart).EndInit();
        grpOrderSummary.ResumeLayout(false);
        grpOrderSummary.PerformLayout();
        tabBooking.ResumeLayout(false);

        pnlManageFilter.ResumeLayout(false);
        pnlManageFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvOrders).EndInit();
        pnlManageActions.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvManageWaitlist).EndInit();
        grpManageWaitlist.ResumeLayout(false);
        grpManageWaitlist.PerformLayout();
        tabManageOrders.ResumeLayout(false);

        tabMain.ResumeLayout(false);
        ResumeLayout(false);
    }
}
