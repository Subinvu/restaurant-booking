namespace RestaurantBooking.Forms.TV3_User;

partial class FrmLogin
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
        lblPlaceholder.Text = "[TV3 - Giai đoạn 1] Giao diện Đăng nhập / Đăng ký (BCrypt + Memurai session:{token} TTL 24h).";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(560, 320);
        Controls.Add(lblPlaceholder);
        Name = "FrmLogin";
        StartPosition = FormStartPosition.CenterParent;
        Text = "[TV3] Đăng nhập / Đăng ký";
        ResumeLayout(false);
        PerformLayout();
    }
}
