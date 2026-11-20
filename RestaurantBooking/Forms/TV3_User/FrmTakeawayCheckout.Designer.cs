namespace RestaurantBooking.Forms.TV3_User;

partial class FrmTakeawayCheckout
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private Button btnBack = null!;

    // Left: Order items & Branch summary
    private GroupBox grpOrderItems = null!;
    private Label lblBranchSummary = null!;
    private DataGridView dgvItems = null!;

    // Right: Delivery / Pickup & Payment
    private GroupBox grpCheckout = null!;
    private Label lblMethodLabel = null!;
    private RadioButton rdoDelivery = null!;
    private RadioButton rdoPickup = null!;
    private Label lblName = null!;
    private TextBox txtReceiverName = null!;
    private Label lblPhone = null!;
    private TextBox txtReceiverPhone = null!;
    private Label lblAddress = null!;
    private TextBox txtDeliveryAddress = null!;
    private Label lblNote = null!;
    private TextBox txtNote = null!;
    private Label lblDate = null!;
    private DateTimePicker dtpReceiveDate = null!;
    private Label lblSlot = null!;
    private ComboBox cboTimeSlot = null!;
    private Label lblVoucher = null!;
    private TextBox txtVoucher = null!;
    private Label lblPayment = null!;
    private ComboBox cboPaymentMethod = null!;
    private Panel pnlTotals = null!;
    private Label lblSubtotal = null!;
    private Label lblDiscount = null!;
    private Label lblTotalAmount = null!;
    private Button btnConfirmOrder = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        btnBack = new Button();

        grpOrderItems = new GroupBox();
        lblBranchSummary = new Label();
        dgvItems = new DataGridView();

        grpCheckout = new GroupBox();
        lblMethodLabel = new Label();
        rdoDelivery = new RadioButton();
        rdoPickup = new RadioButton();
        lblName = new Label();
        txtReceiverName = new TextBox();
        lblPhone = new Label();
        txtReceiverPhone = new TextBox();
        lblAddress = new Label();
        txtDeliveryAddress = new TextBox();
        lblNote = new Label();
        txtNote = new TextBox();
        lblDate = new Label();
        dtpReceiveDate = new DateTimePicker();
        lblSlot = new Label();
        cboTimeSlot = new ComboBox();
        lblVoucher = new Label();
        txtVoucher = new TextBox();
        lblPayment = new Label();
        cboPaymentMethod = new ComboBox();
        pnlTotals = new Panel();
        lblSubtotal = new Label();
        lblDiscount = new Label();
        lblTotalAmount = new Label();
        btnConfirmOrder = new Button();

        pnlHeader.SuspendLayout();
        grpOrderItems.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
        grpCheckout.SuspendLayout();
        pnlTotals.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1060, 54);

        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(16, 15);
        lblTitle.Name = "lblTitle";
        lblTitle.Text = "Thanh toán đơn mang về / giao hàng";
        lblTitle.UseMnemonic = false;

        btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBack.BackColor = Color.FromArgb(51, 65, 85);
        btnBack.FlatStyle = FlatStyle.Flat;
        btnBack.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnBack.ForeColor = Color.White;
        btnBack.Location = new Point(874, 10);
        btnBack.Name = "btnBack";
        btnBack.Size = new Size(170, 34);
        btnBack.Text = "← Quay lại thực đơn";
        btnBack.Click += btnBack_Click;

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(btnBack);

        // grpOrderItems (Left)
        grpOrderItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpOrderItems.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpOrderItems.Location = new Point(14, 66);
        grpOrderItems.Name = "grpOrderItems";
        grpOrderItems.Size = new Size(540, 566);
        grpOrderItems.Text = "Thông tin món ăn trong đơn";

        lblBranchSummary.BackColor = Color.FromArgb(248, 250, 252);
        lblBranchSummary.BorderStyle = BorderStyle.FixedSingle;
        lblBranchSummary.Font = new Font("Segoe UI", 9F);
        lblBranchSummary.ForeColor = Color.FromArgb(15, 23, 42);
        lblBranchSummary.Location = new Point(12, 26);
        lblBranchSummary.Name = "lblBranchSummary";
        lblBranchSummary.Padding = new Padding(8, 6, 8, 6);
        lblBranchSummary.Size = new Size(516, 52);
        lblBranchSummary.Text = "Chi nhánh phục vụ: ...";
        lblBranchSummary.UseMnemonic = false;

        dgvItems.AllowUserToAddRows = false;
        dgvItems.AllowUserToDeleteRows = false;
        dgvItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvItems.BackgroundColor = Color.White;
        dgvItems.Location = new Point(12, 86);
        dgvItems.MultiSelect = false;
        dgvItems.Name = "dgvItems";
        dgvItems.ReadOnly = true;
        dgvItems.RowHeadersVisible = false;
        dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvItems.Size = new Size(516, 466);

        grpOrderItems.Controls.Add(lblBranchSummary);
        grpOrderItems.Controls.Add(dgvItems);

        // grpCheckout (Right)
        grpCheckout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        grpCheckout.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpCheckout.Location = new Point(566, 66);
        grpCheckout.Name = "grpCheckout";
        grpCheckout.Size = new Size(480, 566);
        grpCheckout.Text = "Thông tin nhận hàng && Thanh toán";

        lblMethodLabel.AutoSize = true;
        lblMethodLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMethodLabel.Location = new Point(14, 28);
        lblMethodLabel.Name = "lblMethodLabel";
        lblMethodLabel.Text = "Hình thức nhận món:";

        rdoDelivery.AutoSize = true;
        rdoDelivery.Checked = true;
        rdoDelivery.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        rdoDelivery.Location = new Point(156, 26);
        rdoDelivery.Name = "rdoDelivery";
        rdoDelivery.Text = "Giao tận nơi";
        rdoDelivery.CheckedChanged += ReceiveMode_Changed;

        rdoPickup.AutoSize = true;
        rdoPickup.Font = new Font("Segoe UI", 9F);
        rdoPickup.Location = new Point(282, 26);
        rdoPickup.Name = "rdoPickup";
        rdoPickup.Text = "Tự đến lấy tại quán";
        rdoPickup.CheckedChanged += ReceiveMode_Changed;

        lblName.AutoSize = true;
        lblName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblName.Location = new Point(14, 62);
        lblName.Name = "lblName";
        lblName.Text = "Người nhận (*):";

        txtReceiverName.Font = new Font("Segoe UI", 9.5F);
        txtReceiverName.Location = new Point(156, 58);
        txtReceiverName.Name = "txtReceiverName";
        txtReceiverName.Size = new Size(308, 25);

        lblPhone.AutoSize = true;
        lblPhone.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPhone.Location = new Point(14, 96);
        lblPhone.Name = "lblPhone";
        lblPhone.Text = "Số điện thoại (*):";

        txtReceiverPhone.Font = new Font("Segoe UI", 9.5F);
        txtReceiverPhone.Location = new Point(156, 92);
        txtReceiverPhone.Name = "txtReceiverPhone";
        txtReceiverPhone.Size = new Size(308, 25);

        lblAddress.AutoSize = true;
        lblAddress.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAddress.Location = new Point(14, 130);
        lblAddress.Name = "lblAddress";
        lblAddress.Text = "Địa chỉ giao hàng (*):";

        txtDeliveryAddress.Font = new Font("Segoe UI", 9.5F);
        txtDeliveryAddress.Location = new Point(156, 126);
        txtDeliveryAddress.Multiline = true;
        txtDeliveryAddress.Name = "txtDeliveryAddress";
        txtDeliveryAddress.PlaceholderText = "Số nhà, tên đường, phường, quận...";
        txtDeliveryAddress.Size = new Size(308, 52);

        lblNote.AutoSize = true;
        lblNote.Font = new Font("Segoe UI", 9F);
        lblNote.Location = new Point(14, 190);
        lblNote.Name = "lblNote";
        lblNote.Text = "Ghi chú giao hàng:";

        txtNote.Font = new Font("Segoe UI", 9.5F);
        txtNote.Location = new Point(156, 186);
        txtNote.Name = "txtNote";
        txtNote.PlaceholderText = "Ví dụ: Gọi trước khi giao, ít cay...";
        txtNote.Size = new Size(308, 25);

        lblDate.AutoSize = true;
        lblDate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDate.Location = new Point(14, 226);
        lblDate.Name = "lblDate";
        lblDate.Text = "Ngày nhận:";

        dtpReceiveDate.CustomFormat = "dd/MM/yyyy";
        dtpReceiveDate.Format = DateTimePickerFormat.Custom;
        dtpReceiveDate.Location = new Point(156, 222);
        dtpReceiveDate.Name = "dtpReceiveDate";
        dtpReceiveDate.Size = new Size(124, 25);

        lblSlot.AutoSize = true;
        lblSlot.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSlot.Location = new Point(292, 226);
        lblSlot.Name = "lblSlot";
        lblSlot.Text = "Khung giờ:";

        cboTimeSlot.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTimeSlot.Location = new Point(374, 222);
        cboTimeSlot.Name = "cboTimeSlot";
        cboTimeSlot.Size = new Size(90, 25);

        lblVoucher.AutoSize = true;
        lblVoucher.Font = new Font("Segoe UI", 9F);
        lblVoucher.Location = new Point(14, 262);
        lblVoucher.Name = "lblVoucher";
        lblVoucher.Text = "Mã giảm giá:";

        txtVoucher.Font = new Font("Segoe UI", 9.5F);
        txtVoucher.Location = new Point(156, 258);
        txtVoucher.Name = "txtVoucher";
        txtVoucher.PlaceholderText = "Nhập mã ưu đãi (ví dụ: HUIT10)";
        txtVoucher.Size = new Size(308, 25);
        txtVoucher.TextChanged += txtVoucher_TextChanged;

        lblPayment.AutoSize = true;
        lblPayment.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPayment.Location = new Point(14, 298);
        lblPayment.Name = "lblPayment";
        lblPayment.Text = "Thanh toán:";

        cboPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPaymentMethod.Location = new Point(156, 294);
        cboPaymentMethod.Name = "cboPaymentMethod";
        cboPaymentMethod.Size = new Size(308, 25);

        pnlTotals.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pnlTotals.BackColor = Color.FromArgb(248, 250, 252);
        pnlTotals.BorderStyle = BorderStyle.FixedSingle;
        pnlTotals.Location = new Point(14, 344);
        pnlTotals.Name = "pnlTotals";
        pnlTotals.Size = new Size(450, 144);

        lblSubtotal.AutoSize = true;
        lblSubtotal.Font = new Font("Segoe UI", 9.5F);
        lblSubtotal.Location = new Point(14, 16);
        lblSubtotal.Name = "lblSubtotal";
        lblSubtotal.Text = "Tạm tính: 0 đ";

        lblDiscount.AutoSize = true;
        lblDiscount.Font = new Font("Segoe UI", 9.5F);
        lblDiscount.ForeColor = Color.FromArgb(21, 128, 61);
        lblDiscount.Location = new Point(14, 48);
        lblDiscount.Name = "lblDiscount";
        lblDiscount.Text = "Giảm giá: 0 đ";

        lblTotalAmount.AutoSize = true;
        lblTotalAmount.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTotalAmount.ForeColor = Color.FromArgb(15, 23, 42);
        lblTotalAmount.Location = new Point(14, 88);
        lblTotalAmount.Name = "lblTotalAmount";
        lblTotalAmount.Text = "Tổng thanh toán: 0 đ";

        pnlTotals.Controls.Add(lblSubtotal);
        pnlTotals.Controls.Add(lblDiscount);
        pnlTotals.Controls.Add(lblTotalAmount);

        btnConfirmOrder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        btnConfirmOrder.BackColor = Color.FromArgb(21, 128, 61);
        btnConfirmOrder.FlatStyle = FlatStyle.Flat;
        btnConfirmOrder.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnConfirmOrder.ForeColor = Color.White;
        btnConfirmOrder.Location = new Point(14, 502);
        btnConfirmOrder.Name = "btnConfirmOrder";
        btnConfirmOrder.Size = new Size(450, 48);
        btnConfirmOrder.Text = "Xác nhận đặt hàng";
        btnConfirmOrder.Click += btnConfirmOrder_Click;

        grpCheckout.Controls.Add(lblMethodLabel);
        grpCheckout.Controls.Add(rdoDelivery);
        grpCheckout.Controls.Add(rdoPickup);
        grpCheckout.Controls.Add(lblName);
        grpCheckout.Controls.Add(txtReceiverName);
        grpCheckout.Controls.Add(lblPhone);
        grpCheckout.Controls.Add(txtReceiverPhone);
        grpCheckout.Controls.Add(lblAddress);
        grpCheckout.Controls.Add(txtDeliveryAddress);
        grpCheckout.Controls.Add(lblNote);
        grpCheckout.Controls.Add(txtNote);
        grpCheckout.Controls.Add(lblDate);
        grpCheckout.Controls.Add(dtpReceiveDate);
        grpCheckout.Controls.Add(lblSlot);
        grpCheckout.Controls.Add(cboTimeSlot);
        grpCheckout.Controls.Add(lblVoucher);
        grpCheckout.Controls.Add(txtVoucher);
        grpCheckout.Controls.Add(lblPayment);
        grpCheckout.Controls.Add(cboPaymentMethod);
        grpCheckout.Controls.Add(pnlTotals);
        grpCheckout.Controls.Add(btnConfirmOrder);

        // FrmTakeawayCheckout
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        BackColor = Color.White;
        ClientSize = new Size(1060, 646);
        Controls.Add(grpCheckout);
        Controls.Add(grpOrderItems);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimumSize = new Size(940, 600);
        Name = "FrmTakeawayCheckout";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Thanh toán đơn mang về / giao hàng — Nhà hàng Hương Việt";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
        grpOrderItems.ResumeLayout(false);
        pnlTotals.ResumeLayout(false);
        pnlTotals.PerformLayout();
        grpCheckout.ResumeLayout(false);
        grpCheckout.PerformLayout();
        ResumeLayout(false);
    }
}
