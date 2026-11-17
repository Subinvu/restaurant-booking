namespace RestaurantBooking.Forms.TV1_Booking;

partial class FrmBooking
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblInfo = null!;
    private TabControl tabMain = null!;
    private TabPage tabBooking = null!;
    private Label lblBookingPlaceholder = null!;
    private TabPage tabManageOrders = null!;
    private Label lblManagePlaceholder = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblInfo = new Label();
        tabMain = new TabControl();
        tabBooking = new TabPage();
        lblBookingPlaceholder = new Label();
        tabManageOrders = new TabPage();
        lblManagePlaceholder = new Label();

        tabMain.SuspendLayout();
        tabBooking.SuspendLayout();
        tabManageOrders.SuspendLayout();
        SuspendLayout();

        lblInfo.Dock = DockStyle.Top;
        lblInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblInfo.Padding = new Padding(12, 10, 12, 6);
        lblInfo.Size = new Size(920, 38);
        lblInfo.Text = "[TV1] Đặt bàn, Giữ chỗ TTL 15 phút, Waitlist & Duyệt đơn";

        tabBooking.Controls.Add(lblBookingPlaceholder);
        tabBooking.Text = "Đặt bàn & Sơ đồ TTL";

        lblBookingPlaceholder.AutoSize = true;
        lblBookingPlaceholder.Location = new Point(20, 24);
        lblBookingPlaceholder.Text = "Khu vực thiết kế Sơ đồ bàn (FlowLayoutPanel + Button Xanh/Vàng/Đỏ), Timer 15:00, Waitlist và Chốt đơn (TV1 - Giai đoạn 1).";

        tabManageOrders.Controls.Add(lblManagePlaceholder);
        tabManageOrders.Text = "Quản lý & Duyệt đơn (Staff/Admin)";

        lblManagePlaceholder.AutoSize = true;
        lblManagePlaceholder.Location = new Point(20, 24);
        lblManagePlaceholder.Text = "Khu vực lọc & cập nhật trạng thái đơn + RPOP Waitlist khi hủy đơn (TV1 - Giai đoạn 1).";

        tabMain.Controls.Add(tabBooking);
        tabMain.Controls.Add(tabManageOrders);
        tabMain.Dock = DockStyle.Fill;
        tabMain.Location = new Point(0, 38);
        tabMain.Size = new Size(920, 522);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(920, 560);
        Controls.Add(tabMain);
        Controls.Add(lblInfo);
        Name = "FrmBooking";
        StartPosition = FormStartPosition.CenterParent;
        Text = "[TV1] Đặt bàn & Duyệt đơn";

        tabMain.ResumeLayout(false);
        tabBooking.ResumeLayout(false);
        tabBooking.PerformLayout();
        tabManageOrders.ResumeLayout(false);
        tabManageOrders.PerformLayout();
        ResumeLayout(false);
    }
}
