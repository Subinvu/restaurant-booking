namespace RestaurantBooking.Forms.TV3_User;

partial class FrmHome
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
        lblPlaceholder.Text = "[TV3 - Giai đoạn 1] Trang chủ Thực đơn (Cache-Aside HIT/MISS), Top món ZSET & Giỏ hàng Size/Topping.";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 540);
        Controls.Add(lblPlaceholder);
        Name = "FrmHome";
        StartPosition = FormStartPosition.CenterParent;
        Text = "[TV3] Trang chủ & Thực đơn";
        ResumeLayout(false);
        PerformLayout();
    }
}
