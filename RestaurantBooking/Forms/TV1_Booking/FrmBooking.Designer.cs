namespace RestaurantBooking.Forms.TV1_Booking;

partial class FrmBooking
{
    private System.ComponentModel.IContainer components = null!;
    private System.Windows.Forms.Timer tmrCountdown = null!;
    private Label lblInfo = null!;
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
    private RadioButton rdoDineIn = null!;
    private RadioButton rdoTakeaway = null!;
    private Label lblGuestLabel = null!;
    private NumericUpDown nudGuestCount = null!;
    private Button btnReloadTables = null!;

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

        lblInfo = new Label();
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
        rdoDineIn = new RadioButton();
        rdoTakeaway = new RadioButton();
        lblGuestLabel = new Label();
        nudGuestCount = new NumericUpDown();
        btnReloadTables = new Button();

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

        // tmrCountdown (1 giây đếm ngược TTL)
        tmrCountdown.Interval = 1000;
        tmrCountdown.Tick += tmrCountdown_Tick;

        // lblInfo
        lblInfo.BackColor = Color.FromArgb(236, 253, 245);
        lblInfo.Dock = DockStyle.Top;
        lblInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblInfo.ForeColor = Color.FromArgb(20, 83, 45);
        lblInfo.Padding = new Padding(12, 8, 12, 6);
        lblInfo.Size = new Size(1124, 36);
        lblInfo.Text = "[TV1] Đặt bàn, Giữ chỗ TTL 15 phút, Waitlist & Duyệt đơn";

        // === TAB 1 TOP FILTER ===
        pnlTopFilter.BackColor = Color.FromArgb(248, 250, 252);
        pnlTopFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlTopFilter.Dock = DockStyle.Top;
        pnlTopFilter.Size = new Size(1116, 54);

        lblResLabel.AutoSize = true;
        lblResLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblResLabel.Location = new Point(10, 17);
        lblResLabel.Text = "Chi nhánh:";

        cboRestaurant.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRestaurant.Location = new Point(82, 13);
        cboRestaurant.Size = new Size(215, 25);
        cboRestaurant.SelectedIndexChanged += FilterInputs_Changed;

        lblDateLabel.AutoSize = true;
        lblDateLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDateLabel.Location = new Point(308, 17);
        lblDateLabel.Text = "Ngày:";

        dtpBookingDate.CustomFormat = "dd/MM/yyyy";
        dtpBookingDate.Format = DateTimePickerFormat.Custom;
        dtpBookingDate.Location = new Point(352, 13);
        dtpBookingDate.Size = new Size(112, 25);
        dtpBookingDate.ValueChanged += FilterInputs_Changed;

        lblSlotLabel.AutoSize = true;
        lblSlotLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSlotLabel.Location = new Point(474, 17);
        lblSlotLabel.Text = "Khung giờ:";

        cboTimeSlot.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTimeSlot.Location = new Point(546, 13);
        cboTimeSlot.Size = new Size(82, 25);
        cboTimeSlot.SelectedIndexChanged += FilterInputs_Changed;

        rdoDineIn.AutoSize = true;
        rdoDineIn.Checked = true;
        rdoDineIn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        rdoDineIn.Location = new Point(642, 15);
        rdoDineIn.Text = "Ăn tại bàn";
        rdoDineIn.CheckedChanged += OrderType_Changed;

        rdoTakeaway.AutoSize = true;
        rdoTakeaway.Font = new Font("Segoe UI", 9F);
        rdoTakeaway.Location = new Point(734, 15);
        rdoTakeaway.Text = "Mang về";
        rdoTakeaway.CheckedChanged += OrderType_Changed;

        lblGuestLabel.AutoSize = true;
        lblGuestLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblGuestLabel.Location = new Point(816, 17);
        lblGuestLabel.Text = "Số khách:";

        nudGuestCount.Location = new Point(884, 14);
        nudGuestCount.Maximum = 20;
        nudGuestCount.Minimum = 1;
        nudGuestCount.Size = new Size(56, 25);
        nudGuestCount.Value = 4;

        btnReloadTables.BackColor = Color.FromArgb(224, 242, 254);
        btnReloadTables.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnReloadTables.Location = new Point(956, 10);
        btnReloadTables.Size = new Size(146, 32);
        btnReloadTables.Text = "Làm mới sơ đồ";
        btnReloadTables.Click += async (_, _) => await ReloadTableMapAsync();

        pnlTopFilter.Controls.AddRange([
            lblResLabel, cboRestaurant, lblDateLabel, dtpBookingDate,
            lblSlotLabel, cboTimeSlot, rdoDineIn, rdoTakeaway,
            lblGuestLabel, nudGuestCount, btnReloadTables
        ]);

        // === TAB 1 LEFT: TABLE MAP ===
        grpTableMap.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpTableMap.Location = new Point(8, 62);
        grpTableMap.Size = new Size(666, 566);
        grpTableMap.Text = "Sơ đồ Bàn (Gộp MongoDB + Memurai TTL 15 phút)";

        lblLegend.AutoSize = true;
        lblLegend.Font = new Font("Segoe UI", 9F);
        lblLegend.Location = new Point(12, 24);
        lblLegend.Text = "Chú thích:  [Xanh lá] Trống   |   [Vàng cam] Đang giữ TTL (đếm ngược)   |   [Đỏ] Đã đặt   |   [Xám] Ngưng phục vụ";

        flpTables.AutoScroll = true;
        flpTables.BackColor = Color.White;
        flpTables.BorderStyle = BorderStyle.FixedSingle;
        flpTables.Location = new Point(12, 48);
        flpTables.Padding = new Padding(8);
        flpTables.Size = new Size(642, 456);

        pnlWaitlistBar.BackColor = Color.FromArgb(255, 251, 235);
        pnlWaitlistBar.BorderStyle = BorderStyle.FixedSingle;
        pnlWaitlistBar.Location = new Point(12, 512);
        pnlWaitlistBar.Size = new Size(642, 44);

        lblWaitlistCount.AutoSize = true;
        lblWaitlistCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblWaitlistCount.ForeColor = Color.FromArgb(146, 64, 14);
        lblWaitlistCount.Location = new Point(10, 13);
        lblWaitlistCount.Text = "Hàng đợi Waitlist khung giờ này: 0 khách";

        btnJoinWaitlist.BackColor = Color.FromArgb(254, 243, 199);
        btnJoinWaitlist.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnJoinWaitlist.Location = new Point(392, 6);
        btnJoinWaitlist.Size = new Size(240, 30);
        btnJoinWaitlist.Text = "Hết bàn? Đăng ký Waitlist (LPUSH)";
        btnJoinWaitlist.Click += btnJoinWaitlist_Click;

        pnlWaitlistBar.Controls.Add(lblWaitlistCount);
        pnlWaitlistBar.Controls.Add(btnJoinWaitlist);

        grpTableMap.Controls.Add(lblLegend);
        grpTableMap.Controls.Add(flpTables);
        grpTableMap.Controls.Add(pnlWaitlistBar);

        // === TAB 1 RIGHT: SUMMARY & CHECKOUT ===
        grpOrderSummary.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpOrderSummary.Location = new Point(682, 62);
        grpOrderSummary.Size = new Size(428, 566);
        grpOrderSummary.Text = "Giữ chỗ, Giỏ món (Memurai) && Chốt đơn";

        lblSelectedTableInfo.BackColor = Color.FromArgb(241, 245, 249);
        lblSelectedTableInfo.BorderStyle = BorderStyle.FixedSingle;
        lblSelectedTableInfo.Font = new Font("Segoe UI", 9F);
        lblSelectedTableInfo.Location = new Point(12, 24);
        lblSelectedTableInfo.Padding = new Padding(8, 6, 8, 6);
        lblSelectedTableInfo.Size = new Size(404, 52);
        lblSelectedTableInfo.Text = "Chưa chọn bàn. Bấm một bàn Trống (Xanh lá) bên trái.";

        btnLockTable.BackColor = Color.FromArgb(254, 243, 199);
        btnLockTable.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLockTable.Location = new Point(12, 82);
        btnLockTable.Size = new Size(196, 32);
        btnLockTable.Text = "Giữ bàn 15p (SET NX EX)";
        btnLockTable.Click += btnLockTable_Click;

        btnReleaseLock.BackColor = Color.FromArgb(241, 245, 249);
        btnReleaseLock.Font = new Font("Segoe UI", 9F);
        btnReleaseLock.Location = new Point(220, 82);
        btnReleaseLock.Size = new Size(196, 32);
        btnReleaseLock.Text = "Hủy giữ bàn (DEL)";
        btnReleaseLock.Click += btnReleaseLock_Click;

        lblCartSource.AutoSize = true;
        lblCartSource.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblCartSource.ForeColor = Color.FromArgb(30, 64, 175);
        lblCartSource.Location = new Point(12, 126);
        lblCartSource.Text = "Giỏ món đặt trước từ Memurai Hash (cart:USR001):";

        btnReloadCart.Font = new Font("Segoe UI", 8.5F);
        btnReloadCart.Location = new Point(324, 121);
        btnReloadCart.Size = new Size(92, 26);
        btnReloadCart.Text = "Tải lại giỏ";
        btnReloadCart.Click += async (_, _) => await LoadCartAsync();

        dgvCart.AllowUserToAddRows = false;
        dgvCart.AllowUserToDeleteRows = false;
        dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCart.BackgroundColor = Color.White;
        dgvCart.Location = new Point(12, 152);
        dgvCart.ReadOnly = true;
        dgvCart.RowHeadersVisible = false;
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCart.Size = new Size(404, 182);

        lblSubtotal.AutoSize = true;
        lblSubtotal.Font = new Font("Segoe UI", 9.5F);
        lblSubtotal.Location = new Point(12, 346);
        lblSubtotal.Text = "Tạm tính món ăn: 0 đ";

        lblVoucherLabel.AutoSize = true;
        lblVoucherLabel.Font = new Font("Segoe UI", 9F);
        lblVoucherLabel.Location = new Point(12, 376);
        lblVoucherLabel.Text = "Mã giảm giá (thử HUIT10):";

        txtVoucher.Font = new Font("Segoe UI", 9.5F);
        txtVoucher.Location = new Point(182, 372);
        txtVoucher.Size = new Size(234, 25);
        txtVoucher.TextChanged += (_, _) => UpdateTotalsPreview();

        lblDiscount.AutoSize = true;
        lblDiscount.Font = new Font("Segoe UI", 9F);
        lblDiscount.ForeColor = Color.DarkGreen;
        lblDiscount.Location = new Point(12, 408);
        lblDiscount.Text = "Giảm giá: 0 đ";

        lblTotalAmount.AutoSize = true;
        lblTotalAmount.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        lblTotalAmount.ForeColor = Color.Firebrick;
        lblTotalAmount.Location = new Point(12, 434);
        lblTotalAmount.Text = "Tổng thanh toán: 0 đ";

        lblPaymentLabel.AutoSize = true;
        lblPaymentLabel.Font = new Font("Segoe UI", 9F);
        lblPaymentLabel.Location = new Point(12, 470);
        lblPaymentLabel.Text = "Phương thức thanh toán:";

        cboPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPaymentMethod.Location = new Point(182, 466);
        cboPaymentMethod.Size = new Size(234, 25);

        btnConfirmBooking.BackColor = Color.FromArgb(22, 163, 74);
        btnConfirmBooking.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnConfirmBooking.ForeColor = Color.White;
        btnConfirmBooking.Location = new Point(12, 504);
        btnConfirmBooking.Size = new Size(404, 48);
        btnConfirmBooking.Text = "CHỐT ĐƠN ĐẶT BÀN && CẬP NHẬT XẾP HẠNG";
        btnConfirmBooking.Click += btnConfirmBooking_Click;

        grpOrderSummary.Controls.AddRange([
            lblSelectedTableInfo, btnLockTable, btnReleaseLock,
            lblCartSource, btnReloadCart, dgvCart,
            lblSubtotal, lblVoucherLabel, txtVoucher,
            lblDiscount, lblTotalAmount, lblPaymentLabel,
            cboPaymentMethod, btnConfirmBooking
        ]);

        tabBooking.BackColor = Color.White;
        tabBooking.Controls.Add(grpOrderSummary);
        tabBooking.Controls.Add(grpTableMap);
        tabBooking.Controls.Add(pnlTopFilter);
        tabBooking.Text = "Đặt bàn & Sơ đồ TTL";

        // === TAB 2 MANAGE ORDERS ===
        pnlManageFilter.BackColor = Color.FromArgb(248, 250, 252);
        pnlManageFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlManageFilter.Dock = DockStyle.Top;
        pnlManageFilter.Size = new Size(1116, 52);

        lblMngRes.AutoSize = true;
        lblMngRes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMngRes.Location = new Point(12, 16);
        lblMngRes.Text = "Chi nhánh:";

        cboManageRestaurant.DropDownStyle = ComboBoxStyle.DropDownList;
        cboManageRestaurant.Location = new Point(84, 12);
        cboManageRestaurant.Size = new Size(210, 25);

        chkFilterDate.AutoSize = true;
        chkFilterDate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        chkFilterDate.Location = new Point(312, 15);
        chkFilterDate.Text = "Lọc theo ngày:";
        chkFilterDate.CheckedChanged += (_, _) => dtpManageDate.Enabled = chkFilterDate.Checked;

        dtpManageDate.CustomFormat = "dd/MM/yyyy";
        dtpManageDate.Enabled = false;
        dtpManageDate.Format = DateTimePickerFormat.Custom;
        dtpManageDate.Location = new Point(422, 12);
        dtpManageDate.Size = new Size(118, 25);

        lblMngStatus.AutoSize = true;
        lblMngStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMngStatus.Location = new Point(558, 16);
        lblMngStatus.Text = "Trạng thái:";

        cboManageStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboManageStatus.Location = new Point(632, 12);
        cboManageStatus.Size = new Size(150, 25);

        btnFilterOrders.BackColor = Color.FromArgb(224, 242, 254);
        btnFilterOrders.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnFilterOrders.Location = new Point(800, 9);
        btnFilterOrders.Size = new Size(140, 32);
        btnFilterOrders.Text = "Lọc đơn đặt";
        btnFilterOrders.Click += async (_, _) => await LoadManageOrdersAsync();

        pnlManageFilter.Controls.AddRange([
            lblMngRes, cboManageRestaurant, chkFilterDate, dtpManageDate,
            lblMngStatus, cboManageStatus, btnFilterOrders
        ]);

        dgvOrders.AllowUserToAddRows = false;
        dgvOrders.AllowUserToDeleteRows = false;
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOrders.BackgroundColor = Color.White;
        dgvOrders.Location = new Point(10, 62);
        dgvOrders.MultiSelect = false;
        dgvOrders.ReadOnly = true;
        dgvOrders.RowHeadersVisible = false;
        dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrders.Size = new Size(1096, 314);
        dgvOrders.SelectionChanged += dgvOrders_SelectionChanged;

        pnlManageActions.Location = new Point(10, 384);
        pnlManageActions.Size = new Size(1096, 44);

        btnConfirmOrder.BackColor = Color.FromArgb(219, 234, 254);
        btnConfirmOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnConfirmOrder.Location = new Point(0, 4);
        btnConfirmOrder.Size = new Size(200, 36);
        btnConfirmOrder.Text = "1. Xác nhận (confirmed)";
        btnConfirmOrder.Click += async (_, _) => await ChangeSelectedOrderStatusAsync("confirmed");

        btnServingOrder.BackColor = Color.FromArgb(254, 243, 199);
        btnServingOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnServingOrder.Location = new Point(214, 4);
        btnServingOrder.Size = new Size(210, 36);
        btnServingOrder.Text = "2. Đang phục vụ (serving)";
        btnServingOrder.Click += async (_, _) => await ChangeSelectedOrderStatusAsync("serving");

        btnCompleteOrder.BackColor = Color.FromArgb(220, 252, 231);
        btnCompleteOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCompleteOrder.Location = new Point(438, 4);
        btnCompleteOrder.Size = new Size(200, 36);
        btnCompleteOrder.Text = "3. Hoàn tất (completed)";
        btnCompleteOrder.Click += async (_, _) => await ChangeSelectedOrderStatusAsync("completed");

        btnCancelOrder.BackColor = Color.FromArgb(254, 226, 226);
        btnCancelOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCancelOrder.ForeColor = Color.Firebrick;
        btnCancelOrder.Location = new Point(796, 4);
        btnCancelOrder.Size = new Size(300, 36);
        btnCancelOrder.Text = "Hủy đơn && Rút khách Waitlist (RPOP)";
        btnCancelOrder.Click += btnCancelOrder_Click;

        pnlManageActions.Controls.AddRange([
            btnConfirmOrder, btnServingOrder, btnCompleteOrder, btnCancelOrder
        ]);

        grpManageWaitlist.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpManageWaitlist.Location = new Point(10, 434);
        grpManageWaitlist.Size = new Size(1096, 194);
        grpManageWaitlist.Text = "Hàng đợi Waitlist trên Memurai (Theo đơn đang chọn)";

        lblManageWaitlistSlot.AutoSize = true;
        lblManageWaitlistSlot.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblManageWaitlistSlot.ForeColor = Color.FromArgb(146, 64, 14);
        lblManageWaitlistSlot.Location = new Point(12, 24);
        lblManageWaitlistSlot.Text = "Chọn một đơn 'ăn tại bàn' ở bảng trên để xem danh sách khách đang chờ Waitlist cùng khung giờ.";

        dgvManageWaitlist.AllowUserToAddRows = false;
        dgvManageWaitlist.AllowUserToDeleteRows = false;
        dgvManageWaitlist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvManageWaitlist.BackgroundColor = Color.White;
        dgvManageWaitlist.Location = new Point(12, 48);
        dgvManageWaitlist.ReadOnly = true;
        dgvManageWaitlist.RowHeadersVisible = false;
        dgvManageWaitlist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvManageWaitlist.Size = new Size(1072, 134);

        grpManageWaitlist.Controls.Add(lblManageWaitlistSlot);
        grpManageWaitlist.Controls.Add(dgvManageWaitlist);

        tabManageOrders.BackColor = Color.White;
        tabManageOrders.Controls.Add(grpManageWaitlist);
        tabManageOrders.Controls.Add(pnlManageActions);
        tabManageOrders.Controls.Add(dgvOrders);
        tabManageOrders.Controls.Add(pnlManageFilter);
        tabManageOrders.Text = "Quản lý & Duyệt đơn (Staff/Admin)";

        tabMain.Controls.Add(tabBooking);
        tabMain.Controls.Add(tabManageOrders);
        tabMain.Dock = DockStyle.Fill;
        tabMain.Location = new Point(0, 36);
        tabMain.Size = new Size(1124, 664);
        tabMain.SelectedIndexChanged += async (_, _) =>
        {
            if (tabMain.SelectedTab == tabManageOrders)
                await LoadManageOrdersAsync();
        };

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1124, 700);
        Controls.Add(tabMain);
        Controls.Add(lblInfo);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "FrmBooking";
        StartPosition = FormStartPosition.CenterParent;
        Text = "[TV1] Đặt bàn, Sơ đồ TTL 15p & Duyệt đơn";

        tabMain.ResumeLayout(false);
        tabBooking.ResumeLayout(false);
        tabManageOrders.ResumeLayout(false);
        pnlTopFilter.ResumeLayout(false);
        pnlTopFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudGuestCount).EndInit();
        grpTableMap.ResumeLayout(false);
        grpTableMap.PerformLayout();
        pnlWaitlistBar.ResumeLayout(false);
        pnlWaitlistBar.PerformLayout();
        grpOrderSummary.ResumeLayout(false);
        grpOrderSummary.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCart).EndInit();
        pnlManageFilter.ResumeLayout(false);
        pnlManageFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvOrders).EndInit();
        pnlManageActions.ResumeLayout(false);
        grpManageWaitlist.ResumeLayout(false);
        grpManageWaitlist.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvManageWaitlist).EndInit();
        ResumeLayout(false);
    }
}
