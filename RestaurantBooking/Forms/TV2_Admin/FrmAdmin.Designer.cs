namespace RestaurantBooking.Forms.TV2_Admin;

partial class FrmAdmin
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblInfo = null!;
    private TabControl tabAdmin = null!;
    private TabPage tabRestaurantMenu = null!;
    private Label lblMenuPlaceholder = null!;
    private TabPage tabStatistics = null!;
    private Label lblStatsPlaceholder = null!;
    private TabPage tabBackup = null!;
    private Label lblBackupPlaceholder = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblInfo = new Label();
        tabAdmin = new TabControl();
        tabRestaurantMenu = new TabPage();
        lblMenuPlaceholder = new Label();
        tabStatistics = new TabPage();
        lblStatsPlaceholder = new Label();
        tabBackup = new TabPage();
        lblBackupPlaceholder = new Label();

        tabAdmin.SuspendLayout();
        tabRestaurantMenu.SuspendLayout();
        tabStatistics.SuspendLayout();
        tabBackup.SuspendLayout();
        SuspendLayout();

        lblInfo.Dock = DockStyle.Top;
        lblInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblInfo.Padding = new Padding(12, 10, 12, 6);
        lblInfo.Size = new Size(940, 38);
        lblInfo.Text = "[TV2] Quản trị Nhà hàng/Menu, Thống kê Aggregation & Backup/Restore";

        tabRestaurantMenu.Controls.Add(lblMenuPlaceholder);
        tabRestaurantMenu.Text = "Nhà hàng, Bàn && Thực đơn";
        lblMenuPlaceholder.AutoSize = true;
        lblMenuPlaceholder.Location = new Point(20, 24);
        lblMenuPlaceholder.Text = "Khu vực CRUD Nhà hàng/Bàn/Món ăn trên DataGridView kèm DEL cache:menu:* (TV2 - Giai đoạn 1).";

        tabStatistics.Controls.Add(lblStatsPlaceholder);
        tabStatistics.Text = "Thống kê Aggregation & Top ZSET";
        lblStatsPlaceholder.AutoSize = true;
        lblStatsPlaceholder.Location = new Point(20, 24);
        lblStatsPlaceholder.Text = "Khu vực DataGridView + biểu đồ cột WinForms (Panel/Graphics) từ Aggregation Pipeline (TV2 - Giai đoạn 1).";

        tabBackup.Controls.Add(lblBackupPlaceholder);
        tabBackup.Text = "Backup, Restore && Export/Import";
        lblBackupPlaceholder.AutoSize = true;
        lblBackupPlaceholder.Location = new Point(20, 24);
        lblBackupPlaceholder.Text = "Khu vực nút bấm mongodump, mongorestore, mongoexport, mongoimport & Memurai BGSAVE (TV2 - Giai đoạn 1).";

        tabAdmin.Controls.Add(tabRestaurantMenu);
        tabAdmin.Controls.Add(tabStatistics);
        tabAdmin.Controls.Add(tabBackup);
        tabAdmin.Dock = DockStyle.Fill;
        tabAdmin.Location = new Point(0, 38);
        tabAdmin.Size = new Size(940, 542);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(940, 580);
        Controls.Add(tabAdmin);
        Controls.Add(lblInfo);
        Name = "FrmAdmin";
        StartPosition = FormStartPosition.CenterParent;
        Text = "[TV2] Quản trị, Thống kê & Backup";

        tabAdmin.ResumeLayout(false);
        tabRestaurantMenu.ResumeLayout(false);
        tabRestaurantMenu.PerformLayout();
        tabStatistics.ResumeLayout(false);
        tabStatistics.PerformLayout();
        tabBackup.ResumeLayout(false);
        tabBackup.PerformLayout();
        ResumeLayout(false);
    }
}
