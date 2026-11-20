namespace RestaurantBooking.Forms.TV3_User;

partial class FrmHistory
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblInfo = null!;
    private Button btnBack = null!;
    private TabControl tabMain = null!;
    private TabPage tabMyOrders = null!;
    private TabPage tabReplyReviews = null!;

    // Tab 1: My Orders & Submit Review
    private Panel pnlOrderFilter = null!;
    private Label lblStatus = null!;
    private ComboBox cboStatusFilter = null!;
    private Button btnRefreshOrders = null!;
    private Button btnCancelOrder = null!;
    private DataGridView dgvOrders = null!;
    private GroupBox grpOrderItems = null!;
    private DataGridView dgvOrderItems = null!;
    private GroupBox grpReview = null!;
    private Label lblReviewStatus = null!;
    private Label lblRating = null!;
    private ComboBox cboRating = null!;
    private Label lblLiked = null!;
    private CheckedListBox clbLikedItems = null!;
    private Label lblComment = null!;
    private TextBox txtComment = null!;
    private Button btnSubmitReview = null!;

    // Tab 2: Staff/Admin Reply to Reviews
    private Panel pnlRevFilter = null!;
    private Label lblRevRes = null!;
    private ComboBox cboRevRestaurant = null!;
    private CheckBox chkOnlyUnreplied = null!;
    private Button btnLoadReviews = null!;
    private DataGridView dgvAllReviews = null!;
    private GroupBox grpReply = null!;
    private Label lblSelectedReview = null!;
    private TextBox txtReplyContent = null!;
    private Button btnSendReply = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblInfo = new Label();
        btnBack = new Button();
        tabMain = new TabControl();
        tabMyOrders = new TabPage();
        tabReplyReviews = new TabPage();

        pnlOrderFilter = new Panel();
        lblStatus = new Label();
        cboStatusFilter = new ComboBox();
        btnRefreshOrders = new Button();
        btnCancelOrder = new Button();
        dgvOrders = new DataGridView();
        grpOrderItems = new GroupBox();
        dgvOrderItems = new DataGridView();
        grpReview = new GroupBox();
        lblReviewStatus = new Label();
        lblRating = new Label();
        cboRating = new ComboBox();
        lblLiked = new Label();
        clbLikedItems = new CheckedListBox();
        lblComment = new Label();
        txtComment = new TextBox();
        btnSubmitReview = new Button();

        pnlRevFilter = new Panel();
        lblRevRes = new Label();
        cboRevRestaurant = new ComboBox();
        chkOnlyUnreplied = new CheckBox();
        btnLoadReviews = new Button();
        dgvAllReviews = new DataGridView();
        grpReply = new GroupBox();
        lblSelectedReview = new Label();
        txtReplyContent = new TextBox();
        btnSendReply = new Button();

        pnlHeader.SuspendLayout();
        tabMain.SuspendLayout();
        tabMyOrders.SuspendLayout();
        tabReplyReviews.SuspendLayout();
        pnlOrderFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvOrders).BeginInit();
        grpOrderItems.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvOrderItems).BeginInit();
        grpReview.SuspendLayout();
        pnlRevFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAllReviews).BeginInit();
        grpReply.SuspendLayout();
        SuspendLayout();

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
        lblInfo.Text = "Lịch sử đơn hàng & Đánh giá";
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

        // === TAB 1: ORDERS & REVIEW ===
        pnlOrderFilter.BackColor = Color.FromArgb(248, 250, 252);
        pnlOrderFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlOrderFilter.Dock = DockStyle.Top;
        pnlOrderFilter.Location = new Point(0, 0);
        pnlOrderFilter.Name = "pnlOrderFilter";
        pnlOrderFilter.Size = new Size(1116, 48);

        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.Location = new Point(12, 15);
        lblStatus.Name = "lblStatus";
        lblStatus.Text = "Lọc trạng thái:";

        cboStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatusFilter.Location = new Point(112, 11);
        cboStatusFilter.Name = "cboStatusFilter";
        cboStatusFilter.Size = new Size(175, 25);
        cboStatusFilter.SelectedIndexChanged += cboStatusFilter_SelectedIndexChanged;

        btnRefreshOrders.BackColor = Color.White;
        btnRefreshOrders.FlatStyle = FlatStyle.Flat;
        btnRefreshOrders.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRefreshOrders.Location = new Point(302, 9);
        btnRefreshOrders.Name = "btnRefreshOrders";
        btnRefreshOrders.Size = new Size(120, 30);
        btnRefreshOrders.Text = "Làm mới";
        btnRefreshOrders.Click += btnRefreshOrders_Click;

        btnCancelOrder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancelOrder.BackColor = Color.FromArgb(254, 242, 242);
        btnCancelOrder.FlatStyle = FlatStyle.Flat;
        btnCancelOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCancelOrder.ForeColor = Color.FromArgb(185, 28, 28);
        btnCancelOrder.Location = new Point(900, 9);
        btnCancelOrder.Name = "btnCancelOrder";
        btnCancelOrder.Size = new Size(202, 30);
        btnCancelOrder.Text = "Hủy đơn hàng đã chọn";
        btnCancelOrder.Click += btnCancelOrder_Click;

        pnlOrderFilter.Controls.Add(lblStatus);
        pnlOrderFilter.Controls.Add(cboStatusFilter);
        pnlOrderFilter.Controls.Add(btnRefreshOrders);
        pnlOrderFilter.Controls.Add(btnCancelOrder);

        dgvOrders.AllowUserToAddRows = false;
        dgvOrders.AllowUserToDeleteRows = false;
        dgvOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOrders.BackgroundColor = Color.White;
        dgvOrders.Location = new Point(10, 56);
        dgvOrders.MultiSelect = false;
        dgvOrders.Name = "dgvOrders";
        dgvOrders.ReadOnly = true;
        dgvOrders.RowHeadersVisible = false;
        dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrders.Size = new Size(1096, 256);
        dgvOrders.SelectionChanged += dgvOrders_SelectionChanged;

        grpOrderItems.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        grpOrderItems.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpOrderItems.Location = new Point(10, 320);
        grpOrderItems.Name = "grpOrderItems";
        grpOrderItems.Size = new Size(510, 294);
        grpOrderItems.Text = "Chi tiết món ăn trong đơn";

        dgvOrderItems.AllowUserToAddRows = false;
        dgvOrderItems.AllowUserToDeleteRows = false;
        dgvOrderItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOrderItems.BackgroundColor = Color.White;
        dgvOrderItems.Location = new Point(10, 24);
        dgvOrderItems.MultiSelect = false;
        dgvOrderItems.Name = "dgvOrderItems";
        dgvOrderItems.ReadOnly = true;
        dgvOrderItems.RowHeadersVisible = false;
        dgvOrderItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvOrderItems.Size = new Size(490, 258);

        grpOrderItems.Controls.Add(dgvOrderItems);

        grpReview.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpReview.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpReview.Location = new Point(530, 320);
        grpReview.Name = "grpReview";
        grpReview.Size = new Size(576, 294);
        grpReview.Text = "Đánh giá trải nghiệm (dành cho đơn đã hoàn tất)";

        lblReviewStatus.AutoSize = true;
        lblReviewStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblReviewStatus.ForeColor = Color.FromArgb(71, 85, 105);
        lblReviewStatus.Location = new Point(12, 24);
        lblReviewStatus.Name = "lblReviewStatus";
        lblReviewStatus.Text = "Chọn một đơn hàng ở bảng trên để xem hoặc gửi đánh giá.";
        lblReviewStatus.UseMnemonic = false;

        lblRating.AutoSize = true;
        lblRating.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblRating.Location = new Point(12, 56);
        lblRating.Name = "lblRating";
        lblRating.Text = "Số sao:";

        cboRating.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRating.Location = new Point(74, 52);
        cboRating.Name = "cboRating";
        cboRating.Size = new Size(134, 25);

        lblLiked.AutoSize = true;
        lblLiked.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblLiked.Location = new Point(222, 56);
        lblLiked.Name = "lblLiked";
        lblLiked.Text = "Món ăn bạn yêu thích:";

        clbLikedItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        clbLikedItems.CheckOnClick = true;
        clbLikedItems.Font = new Font("Segoe UI", 8.5F);
        clbLikedItems.Location = new Point(222, 78);
        clbLikedItems.Name = "clbLikedItems";
        clbLikedItems.Size = new Size(340, 66);

        lblComment.AutoSize = true;
        lblComment.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblComment.Location = new Point(12, 152);
        lblComment.Name = "lblComment";
        lblComment.Text = "Nhận xét của bạn:";

        txtComment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtComment.Font = new Font("Segoe UI", 9.5F);
        txtComment.Location = new Point(12, 174);
        txtComment.Multiline = true;
        txtComment.Name = "txtComment";
        txtComment.Size = new Size(550, 66);

        btnSubmitReview.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        btnSubmitReview.BackColor = Color.FromArgb(21, 128, 61);
        btnSubmitReview.FlatStyle = FlatStyle.Flat;
        btnSubmitReview.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSubmitReview.ForeColor = Color.White;
        btnSubmitReview.Location = new Point(12, 248);
        btnSubmitReview.Name = "btnSubmitReview";
        btnSubmitReview.Size = new Size(550, 36);
        btnSubmitReview.Text = "Gửi đánh giá";
        btnSubmitReview.Click += btnSubmitReview_Click;

        grpReview.Controls.Add(lblReviewStatus);
        grpReview.Controls.Add(lblRating);
        grpReview.Controls.Add(cboRating);
        grpReview.Controls.Add(lblLiked);
        grpReview.Controls.Add(clbLikedItems);
        grpReview.Controls.Add(lblComment);
        grpReview.Controls.Add(txtComment);
        grpReview.Controls.Add(btnSubmitReview);

        tabMyOrders.BackColor = Color.White;
        tabMyOrders.Location = new Point(4, 34);
        tabMyOrders.Name = "tabMyOrders";
        tabMyOrders.Size = new Size(1116, 624);
        tabMyOrders.Controls.Add(grpReview);
        tabMyOrders.Controls.Add(grpOrderItems);
        tabMyOrders.Controls.Add(dgvOrders);
        tabMyOrders.Controls.Add(pnlOrderFilter);
        tabMyOrders.Text = "Lịch sử đơn & Đánh giá";

        // === TAB 2: STAFF/ADMIN REPLY TO REVIEWS ===
        pnlRevFilter.BackColor = Color.FromArgb(248, 250, 252);
        pnlRevFilter.BorderStyle = BorderStyle.FixedSingle;
        pnlRevFilter.Dock = DockStyle.Top;
        pnlRevFilter.Location = new Point(0, 0);
        pnlRevFilter.Name = "pnlRevFilter";
        pnlRevFilter.Size = new Size(1116, 48);

        lblRevRes.AutoSize = true;
        lblRevRes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblRevRes.Location = new Point(12, 15);
        lblRevRes.Name = "lblRevRes";
        lblRevRes.Text = "Chi nhánh:";

        cboRevRestaurant.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRevRestaurant.Location = new Point(84, 11);
        cboRevRestaurant.Name = "cboRevRestaurant";
        cboRevRestaurant.Size = new Size(210, 25);

        chkOnlyUnreplied.AutoSize = true;
        chkOnlyUnreplied.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        chkOnlyUnreplied.Location = new Point(314, 14);
        chkOnlyUnreplied.Name = "chkOnlyUnreplied";
        chkOnlyUnreplied.Text = "Chỉ xem đánh giá chưa phản hồi";

        btnLoadReviews.BackColor = Color.White;
        btnLoadReviews.FlatStyle = FlatStyle.Flat;
        btnLoadReviews.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLoadReviews.Location = new Point(558, 9);
        btnLoadReviews.Name = "btnLoadReviews";
        btnLoadReviews.Size = new Size(134, 30);
        btnLoadReviews.Text = "Lọc đánh giá";
        btnLoadReviews.Click += btnLoadReviews_Click;

        pnlRevFilter.Controls.Add(lblRevRes);
        pnlRevFilter.Controls.Add(cboRevRestaurant);
        pnlRevFilter.Controls.Add(chkOnlyUnreplied);
        pnlRevFilter.Controls.Add(btnLoadReviews);

        dgvAllReviews.AllowUserToAddRows = false;
        dgvAllReviews.AllowUserToDeleteRows = false;
        dgvAllReviews.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvAllReviews.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAllReviews.BackgroundColor = Color.White;
        dgvAllReviews.Location = new Point(10, 56);
        dgvAllReviews.MultiSelect = false;
        dgvAllReviews.Name = "dgvAllReviews";
        dgvAllReviews.ReadOnly = true;
        dgvAllReviews.RowHeadersVisible = false;
        dgvAllReviews.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAllReviews.Size = new Size(1096, 380);
        dgvAllReviews.SelectionChanged += dgvAllReviews_SelectionChanged;

        grpReply.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpReply.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpReply.Location = new Point(10, 444);
        grpReply.Name = "grpReply";
        grpReply.Size = new Size(1096, 170);
        grpReply.Text = "Phản hồi đánh giá của khách hàng";

        lblSelectedReview.AutoSize = true;
        lblSelectedReview.Font = new Font("Segoe UI", 9F);
        lblSelectedReview.ForeColor = Color.FromArgb(15, 23, 42);
        lblSelectedReview.Location = new Point(12, 24);
        lblSelectedReview.Name = "lblSelectedReview";
        lblSelectedReview.Text = "Chọn một đánh giá ở bảng trên để nhập phản hồi.";
        lblSelectedReview.UseMnemonic = false;

        txtReplyContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        txtReplyContent.Font = new Font("Segoe UI", 9.5F);
        txtReplyContent.Location = new Point(12, 50);
        txtReplyContent.Multiline = true;
        txtReplyContent.Name = "txtReplyContent";
        txtReplyContent.Size = new Size(850, 102);

        btnSendReply.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSendReply.BackColor = Color.FromArgb(29, 78, 216);
        btnSendReply.FlatStyle = FlatStyle.Flat;
        btnSendReply.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSendReply.ForeColor = Color.White;
        btnSendReply.Location = new Point(876, 50);
        btnSendReply.Name = "btnSendReply";
        btnSendReply.Size = new Size(206, 48);
        btnSendReply.Text = "Gửi phản hồi";
        btnSendReply.Click += btnSendReply_Click;

        grpReply.Controls.Add(lblSelectedReview);
        grpReply.Controls.Add(txtReplyContent);
        grpReply.Controls.Add(btnSendReply);

        tabReplyReviews.BackColor = Color.White;
        tabReplyReviews.Location = new Point(4, 34);
        tabReplyReviews.Name = "tabReplyReviews";
        tabReplyReviews.Size = new Size(1116, 624);
        tabReplyReviews.Controls.Add(grpReply);
        tabReplyReviews.Controls.Add(dgvAllReviews);
        tabReplyReviews.Controls.Add(pnlRevFilter);
        tabReplyReviews.Text = "Quản lý & Phản hồi đánh giá";

        tabMain.Controls.Add(tabMyOrders);
        tabMain.Controls.Add(tabReplyReviews);
        tabMain.Dock = DockStyle.Fill;
        tabMain.Location = new Point(0, 52);
        tabMain.Name = "tabMain";
        tabMain.Size = new Size(1124, 658);
        tabMain.SelectedIndexChanged += tabMain_SelectedIndexChanged;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        BackColor = Color.White;
        ClientSize = new Size(1124, 710);
        Controls.Add(tabMain);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimumSize = new Size(960, 600);
        Name = "FrmHistory";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Đơn hàng & Đánh giá — Nhà hàng Hương Việt";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvOrders).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvOrderItems).EndInit();
        grpOrderItems.ResumeLayout(false);
        grpReview.ResumeLayout(false);
        grpReview.PerformLayout();
        pnlOrderFilter.ResumeLayout(false);
        pnlOrderFilter.PerformLayout();
        tabMyOrders.ResumeLayout(false);

        ((System.ComponentModel.ISupportInitialize)dgvAllReviews).EndInit();
        grpReply.ResumeLayout(false);
        grpReply.PerformLayout();
        pnlRevFilter.ResumeLayout(false);
        pnlRevFilter.PerformLayout();
        tabReplyReviews.ResumeLayout(false);

        tabMain.ResumeLayout(false);
        ResumeLayout(false);
    }
}
