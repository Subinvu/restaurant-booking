namespace RestaurantBooking.Forms.TV3_User;

partial class FrmHome
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblInfo = null!;
    private Label lblBranchAddress = null!;
    private Button btnChangeBranch = null!;
    private Button btnBackMain = null!;
    private Panel pnlTop = null!;
    private Label lblCat = null!;
    private ComboBox cboCategory = null!;
    private Label lblSearch = null!;
    private TextBox txtSearch = null!;
    private Button btnLoadMenu = null!;

    // Left: Menu + Top Hot
    private GroupBox grpMenu = null!;
    private DataGridView dgvMenu = null!;
    private GroupBox grpTopHot = null!;
    private DataGridView dgvTopHot = null!;

    // Right: Customize Dish + Cart
    private GroupBox grpCustomize = null!;
    private Label lblSelectedDish = null!;
    private Label lblSize = null!;
    private ComboBox cboSize = null!;
    private Label lblQty = null!;
    private NumericUpDown nudQty = null!;
    private Label lblToppings = null!;
    private CheckedListBox clbToppings = null!;
    private Label lblUnitPreview = null!;
    private Button btnAddToCart = null!;

    private GroupBox grpCart = null!;
    private DataGridView dgvCart = null!;
    private Button btnQtyPlus = null!;
    private Button btnQtyMinus = null!;
    private Button btnRemoveItem = null!;
    private Button btnClearCart = null!;
    private Label lblCartTotal = null!;
    private Button btnBookTable = null!;
    private Button btnTakeawayCheckout = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblInfo = new Label();
        lblBranchAddress = new Label();
        btnChangeBranch = new Button();
        btnBackMain = new Button();
        pnlTop = new Panel();
        lblCat = new Label();
        cboCategory = new ComboBox();
        lblSearch = new Label();
        txtSearch = new TextBox();
        btnLoadMenu = new Button();

        grpMenu = new GroupBox();
        dgvMenu = new DataGridView();
        grpTopHot = new GroupBox();
        dgvTopHot = new DataGridView();

        grpCustomize = new GroupBox();
        lblSelectedDish = new Label();
        lblSize = new Label();
        cboSize = new ComboBox();
        lblQty = new Label();
        nudQty = new NumericUpDown();
        lblToppings = new Label();
        clbToppings = new CheckedListBox();
        lblUnitPreview = new Label();
        btnAddToCart = new Button();

        grpCart = new GroupBox();
        dgvCart = new DataGridView();
        btnQtyPlus = new Button();
        btnQtyMinus = new Button();
        btnRemoveItem = new Button();
        btnClearCart = new Button();
        lblCartTotal = new Label();
        btnBookTable = new Button();
        btnTakeawayCheckout = new Button();

        pnlHeader.SuspendLayout();
        pnlTop.SuspendLayout();
        grpMenu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvMenu).BeginInit();
        grpTopHot.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTopHot).BeginInit();
        grpCustomize.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)nudQty).BeginInit();
        grpCart.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCart).BeginInit();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1168, 60);

        lblInfo.AutoSize = true;
        lblInfo.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        lblInfo.ForeColor = Color.White;
        lblInfo.Location = new Point(16, 8);
        lblInfo.Name = "lblInfo";
        lblInfo.Text = "Thực đơn — Nhà hàng Hương Việt";
        lblInfo.UseMnemonic = false;

        lblBranchAddress.AutoSize = true;
        lblBranchAddress.Font = new Font("Segoe UI", 8.5F);
        lblBranchAddress.ForeColor = Color.FromArgb(203, 213, 225);
        lblBranchAddress.Location = new Point(16, 35);
        lblBranchAddress.Name = "lblBranchAddress";
        lblBranchAddress.Text = "Chọn món yêu thích để đặt bàn tại quán hoặc đặt mang về / giao hàng";
        lblBranchAddress.UseMnemonic = false;

        btnChangeBranch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnChangeBranch.BackColor = Color.FromArgb(51, 65, 85);
        btnChangeBranch.FlatStyle = FlatStyle.Flat;
        btnChangeBranch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnChangeBranch.ForeColor = Color.White;
        btnChangeBranch.Location = new Point(854, 13);
        btnChangeBranch.Name = "btnChangeBranch";
        btnChangeBranch.Size = new Size(156, 34);
        btnChangeBranch.Text = "← Đổi chi nhánh";
        btnChangeBranch.Click += btnChangeBranch_Click;

        btnBackMain.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBackMain.BackColor = Color.FromArgb(51, 65, 85);
        btnBackMain.FlatStyle = FlatStyle.Flat;
        btnBackMain.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnBackMain.ForeColor = Color.White;
        btnBackMain.Location = new Point(1020, 13);
        btnBackMain.Name = "btnBackMain";
        btnBackMain.Size = new Size(134, 34);
        btnBackMain.Text = "← Trang chủ";
        btnBackMain.Click += btnBackMain_Click;

        pnlHeader.Controls.Add(lblInfo);
        pnlHeader.Controls.Add(lblBranchAddress);
        pnlHeader.Controls.Add(btnChangeBranch);
        pnlHeader.Controls.Add(btnBackMain);

        // pnlTop
        pnlTop.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlTop.BackColor = Color.FromArgb(248, 250, 252);
        pnlTop.BorderStyle = BorderStyle.FixedSingle;
        pnlTop.Location = new Point(12, 68);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(1144, 50);

        lblCat.AutoSize = true;
        lblCat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCat.Location = new Point(12, 16);
        lblCat.Name = "lblCat";
        lblCat.Text = "Nhóm món:";

        cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        cboCategory.Location = new Point(92, 12);
        cboCategory.Name = "cboCategory";
        cboCategory.Size = new Size(180, 25);
        cboCategory.SelectedIndexChanged += cboCategory_SelectedIndexChanged;

        lblSearch.AutoSize = true;
        lblSearch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSearch.Location = new Point(290, 16);
        lblSearch.Name = "lblSearch";
        lblSearch.Text = "Tìm kiếm:";

        txtSearch.Location = new Point(362, 12);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Nhập tên món ăn...";
        txtSearch.Size = new Size(250, 25);
        txtSearch.TextChanged += txtSearch_TextChanged;

        btnLoadMenu.BackColor = Color.White;
        btnLoadMenu.FlatStyle = FlatStyle.Flat;
        btnLoadMenu.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLoadMenu.Location = new Point(628, 10);
        btnLoadMenu.Name = "btnLoadMenu";
        btnLoadMenu.Size = new Size(106, 30);
        btnLoadMenu.Text = "Tải lại";
        btnLoadMenu.Click += btnLoadMenu_Click;

        pnlTop.Controls.Add(lblCat);
        pnlTop.Controls.Add(cboCategory);
        pnlTop.Controls.Add(lblSearch);
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(btnLoadMenu);

        // grpMenu (Left top)
        grpMenu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpMenu.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpMenu.Location = new Point(12, 126);
        grpMenu.Name = "grpMenu";
        grpMenu.Size = new Size(688, 368);
        grpMenu.Text = "Thực đơn chi nhánh";

        dgvMenu.AllowUserToAddRows = false;
        dgvMenu.AllowUserToDeleteRows = false;
        dgvMenu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvMenu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvMenu.BackgroundColor = Color.White;
        dgvMenu.Location = new Point(10, 24);
        dgvMenu.MultiSelect = false;
        dgvMenu.Name = "dgvMenu";
        dgvMenu.ReadOnly = true;
        dgvMenu.RowHeadersVisible = false;
        dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvMenu.Size = new Size(668, 334);
        dgvMenu.SelectionChanged += dgvMenu_SelectionChanged;

        grpMenu.Controls.Add(dgvMenu);

        // grpTopHot (Left bottom)
        grpTopHot.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpTopHot.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpTopHot.Location = new Point(12, 502);
        grpTopHot.Name = "grpTopHot";
        grpTopHot.Size = new Size(688, 198);
        grpTopHot.Text = "Món được yêu thích nhất";

        dgvTopHot.AllowUserToAddRows = false;
        dgvTopHot.AllowUserToDeleteRows = false;
        dgvTopHot.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvTopHot.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTopHot.BackgroundColor = Color.White;
        dgvTopHot.Location = new Point(10, 24);
        dgvTopHot.MultiSelect = false;
        dgvTopHot.Name = "dgvTopHot";
        dgvTopHot.ReadOnly = true;
        dgvTopHot.RowHeadersVisible = false;
        dgvTopHot.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTopHot.Size = new Size(668, 164);

        grpTopHot.Controls.Add(dgvTopHot);

        // grpCustomize (Right top)
        grpCustomize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        grpCustomize.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpCustomize.Location = new Point(712, 126);
        grpCustomize.Name = "grpCustomize";
        grpCustomize.Size = new Size(444, 248);
        grpCustomize.Text = "Tùy chọn kích cỡ && topping";

        lblSelectedDish.AutoSize = true;
        lblSelectedDish.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblSelectedDish.ForeColor = Color.FromArgb(15, 23, 42);
        lblSelectedDish.Location = new Point(12, 24);
        lblSelectedDish.Name = "lblSelectedDish";
        lblSelectedDish.Text = "Chọn một món ăn trong thực đơn bên trái.";
        lblSelectedDish.UseMnemonic = false;

        lblSize.AutoSize = true;
        lblSize.Font = new Font("Segoe UI", 9F);
        lblSize.Location = new Point(12, 54);
        lblSize.Name = "lblSize";
        lblSize.Text = "Kích cỡ:";

        cboSize.DropDownStyle = ComboBoxStyle.DropDownList;
        cboSize.Location = new Point(74, 50);
        cboSize.Name = "cboSize";
        cboSize.Size = new Size(198, 25);
        cboSize.SelectedIndexChanged += cboSize_SelectedIndexChanged;

        lblQty.AutoSize = true;
        lblQty.Font = new Font("Segoe UI", 9F);
        lblQty.Location = new Point(288, 54);
        lblQty.Name = "lblQty";
        lblQty.Text = "SL:";

        nudQty.Location = new Point(318, 51);
        nudQty.Maximum = 20;
        nudQty.Minimum = 1;
        nudQty.Name = "nudQty";
        nudQty.Size = new Size(64, 25);
        nudQty.Value = 1;
        nudQty.ValueChanged += nudQty_ValueChanged;

        lblToppings.AutoSize = true;
        lblToppings.Font = new Font("Segoe UI", 9F);
        lblToppings.Location = new Point(12, 84);
        lblToppings.Name = "lblToppings";
        lblToppings.Text = "Topping dùng kèm (tùy chọn):";

        clbToppings.CheckOnClick = true;
        clbToppings.Font = new Font("Segoe UI", 9F);
        clbToppings.Location = new Point(12, 106);
        clbToppings.Name = "clbToppings";
        clbToppings.Size = new Size(420, 60);
        clbToppings.ItemCheck += clbToppings_ItemCheck;

        lblUnitPreview.AutoSize = true;
        lblUnitPreview.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblUnitPreview.ForeColor = Color.FromArgb(15, 23, 42);
        lblUnitPreview.Location = new Point(12, 172);
        lblUnitPreview.Name = "lblUnitPreview";
        lblUnitPreview.Text = "Tạm tính: 0 đ";

        btnAddToCart.BackColor = Color.FromArgb(29, 78, 216);
        btnAddToCart.FlatStyle = FlatStyle.Flat;
        btnAddToCart.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnAddToCart.ForeColor = Color.White;
        btnAddToCart.Location = new Point(12, 200);
        btnAddToCart.Name = "btnAddToCart";
        btnAddToCart.Size = new Size(420, 38);
        btnAddToCart.Text = "Thêm vào giỏ hàng";
        btnAddToCart.Click += btnAddToCart_Click;

        grpCustomize.Controls.Add(lblSelectedDish);
        grpCustomize.Controls.Add(lblSize);
        grpCustomize.Controls.Add(cboSize);
        grpCustomize.Controls.Add(lblQty);
        grpCustomize.Controls.Add(nudQty);
        grpCustomize.Controls.Add(lblToppings);
        grpCustomize.Controls.Add(clbToppings);
        grpCustomize.Controls.Add(lblUnitPreview);
        grpCustomize.Controls.Add(btnAddToCart);

        // grpCart (Right bottom)
        grpCart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        grpCart.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpCart.Location = new Point(712, 382);
        grpCart.Name = "grpCart";
        grpCart.Size = new Size(444, 318);
        grpCart.Text = "Giỏ hàng của bạn";

        dgvCart.AllowUserToAddRows = false;
        dgvCart.AllowUserToDeleteRows = false;
        dgvCart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCart.BackgroundColor = Color.White;
        dgvCart.Location = new Point(12, 24);
        dgvCart.MultiSelect = false;
        dgvCart.Name = "dgvCart";
        dgvCart.ReadOnly = true;
        dgvCart.RowHeadersVisible = false;
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCart.Size = new Size(420, 158);

        btnQtyPlus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnQtyPlus.FlatStyle = FlatStyle.Flat;
        btnQtyPlus.Location = new Point(12, 190);
        btnQtyPlus.Name = "btnQtyPlus";
        btnQtyPlus.Size = new Size(88, 30);
        btnQtyPlus.Text = "+1 SL";
        btnQtyPlus.Click += btnQtyPlus_Click;

        btnQtyMinus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnQtyMinus.FlatStyle = FlatStyle.Flat;
        btnQtyMinus.Location = new Point(106, 190);
        btnQtyMinus.Name = "btnQtyMinus";
        btnQtyMinus.Size = new Size(88, 30);
        btnQtyMinus.Text = "-1 SL";
        btnQtyMinus.Click += btnQtyMinus_Click;

        btnRemoveItem.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnRemoveItem.BackColor = Color.FromArgb(254, 242, 242);
        btnRemoveItem.FlatStyle = FlatStyle.Flat;
        btnRemoveItem.ForeColor = Color.FromArgb(185, 28, 28);
        btnRemoveItem.Location = new Point(200, 190);
        btnRemoveItem.Name = "btnRemoveItem";
        btnRemoveItem.Size = new Size(106, 30);
        btnRemoveItem.Text = "Xóa món";
        btnRemoveItem.Click += btnRemoveItem_Click;

        btnClearCart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnClearCart.FlatStyle = FlatStyle.Flat;
        btnClearCart.Location = new Point(312, 190);
        btnClearCart.Name = "btnClearCart";
        btnClearCart.Size = new Size(120, 30);
        btnClearCart.Text = "Xóa hết giỏ";
        btnClearCart.Click += btnClearCart_Click;

        lblCartTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblCartTotal.AutoSize = true;
        lblCartTotal.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblCartTotal.ForeColor = Color.FromArgb(15, 23, 42);
        lblCartTotal.Location = new Point(12, 230);
        lblCartTotal.Name = "lblCartTotal";
        lblCartTotal.Text = "Tổng giỏ hàng: 0 đ";

        btnBookTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnBookTable.BackColor = Color.FromArgb(21, 128, 61);
        btnBookTable.FlatStyle = FlatStyle.Flat;
        btnBookTable.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnBookTable.ForeColor = Color.White;
        btnBookTable.Location = new Point(12, 260);
        btnBookTable.Name = "btnBookTable";
        btnBookTable.Size = new Size(204, 44);
        btnBookTable.Text = "Đặt bàn tại quán";
        btnBookTable.Click += btnBookTable_Click;

        btnTakeawayCheckout.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnTakeawayCheckout.BackColor = Color.FromArgb(29, 78, 216);
        btnTakeawayCheckout.FlatStyle = FlatStyle.Flat;
        btnTakeawayCheckout.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnTakeawayCheckout.ForeColor = Color.White;
        btnTakeawayCheckout.Location = new Point(226, 260);
        btnTakeawayCheckout.Name = "btnTakeawayCheckout";
        btnTakeawayCheckout.Size = new Size(206, 44);
        btnTakeawayCheckout.Text = "Đặt mang về / Giao hàng";
        btnTakeawayCheckout.Click += btnTakeawayCheckout_Click;

        grpCart.Controls.Add(dgvCart);
        grpCart.Controls.Add(btnQtyPlus);
        grpCart.Controls.Add(btnQtyMinus);
        grpCart.Controls.Add(btnRemoveItem);
        grpCart.Controls.Add(btnClearCart);
        grpCart.Controls.Add(lblCartTotal);
        grpCart.Controls.Add(btnBookTable);
        grpCart.Controls.Add(btnTakeawayCheckout);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        BackColor = Color.White;
        ClientSize = new Size(1168, 712);
        Controls.Add(grpCart);
        Controls.Add(grpCustomize);
        Controls.Add(grpTopHot);
        Controls.Add(grpMenu);
        Controls.Add(pnlTop);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimumSize = new Size(1020, 640);
        Name = "FrmHome";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Thực đơn & Giỏ hàng — Nhà hàng Hương Việt";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvMenu).EndInit();
        grpMenu.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTopHot).EndInit();
        grpTopHot.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)nudQty).EndInit();
        grpCustomize.ResumeLayout(false);
        grpCustomize.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCart).EndInit();
        grpCart.ResumeLayout(false);
        grpCart.PerformLayout();
        ResumeLayout(false);
    }
}
