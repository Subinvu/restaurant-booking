namespace RestaurantBooking.Forms.TV3_User;

partial class FrmHistory
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
        lblPlaceholder.Text = "[TV3 - Giai đoạn 1] Lịch sử đơn hàng, Hủy đơn (RPOP Waitlist) & Gửi đánh giá sao (reviews).";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(880, 520);
        Controls.Add(lblPlaceholder);
        Name = "FrmHistory";
        StartPosition = FormStartPosition.CenterParent;
        Text = "[TV3] Lịch sử đơn & Đánh giá";
        ResumeLayout(false);
        PerformLayout();
    }
}
