namespace RestaurantBooking.Forms.TV3_User;

partial class FrmUserAdmin
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblPlaceholder = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblPlaceholder = new Label();
        SuspendLayout();

        lblPlaceholder.AutoSize = true;
        lblPlaceholder.Font = new Font("Segoe UI", 10F);
        lblPlaceholder.Location = new Point(24, 28);
        lblPlaceholder.Text = "[TV3 - Giai đoạn 1] Quản trị User: lọc/tìm kiếm, phân quyền admin/staff/customer, khóa/mở tài khoản.";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(860, 500);
        Controls.Add(lblPlaceholder);
        Name = "FrmUserAdmin";
        StartPosition = FormStartPosition.CenterParent;
        Text = "[TV3] Quản trị Người dùng";
        ResumeLayout(false);
        PerformLayout();
    }
}
