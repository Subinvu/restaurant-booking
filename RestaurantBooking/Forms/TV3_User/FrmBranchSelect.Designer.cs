namespace RestaurantBooking.Forms.TV3_User;

partial class FrmBranchSelect
{
    private System.ComponentModel.IContainer components = null!;
    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private Button btnBackMain = null!;
    private Panel pnlSubheader = null!;
    private Label lblPrompt = null!;
    private FlowLayoutPanel flpBranches = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        btnBackMain = new Button();
        pnlSubheader = new Panel();
        lblPrompt = new Label();
        flpBranches = new FlowLayoutPanel();

        pnlHeader.SuspendLayout();
        pnlSubheader.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 41, 59);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(860, 56);

        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(18, 16);
        lblTitle.Name = "lblTitle";
        lblTitle.Text = "Chọn chi nhánh Nhà hàng Hương Việt";
        lblTitle.UseMnemonic = false;

        btnBackMain.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBackMain.BackColor = Color.FromArgb(51, 65, 85);
        btnBackMain.FlatStyle = FlatStyle.Flat;
        btnBackMain.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnBackMain.ForeColor = Color.White;
        btnBackMain.Location = new Point(692, 11);
        btnBackMain.Name = "btnBackMain";
        btnBackMain.Size = new Size(150, 34);
        btnBackMain.Text = "← Trang chủ";
        btnBackMain.Click += btnBackMain_Click;

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(btnBackMain);

        // pnlSubheader
        pnlSubheader.BackColor = Color.White;
        pnlSubheader.BorderStyle = BorderStyle.FixedSingle;
        pnlSubheader.Dock = DockStyle.Top;
        pnlSubheader.Location = new Point(0, 56);
        pnlSubheader.Name = "pnlSubheader";
        pnlSubheader.Size = new Size(860, 44);

        lblPrompt.AutoSize = true;
        lblPrompt.Font = new Font("Segoe UI", 9.5F);
        lblPrompt.ForeColor = Color.FromArgb(71, 85, 105);
        lblPrompt.Location = new Point(18, 12);
        lblPrompt.Name = "lblPrompt";
        lblPrompt.Text = "Chọn một chi nhánh bên dưới để xem thực đơn và đặt món:";

        pnlSubheader.Controls.Add(lblPrompt);

        // flpBranches
        flpBranches.AutoScroll = true;
        flpBranches.BackColor = Color.FromArgb(248, 250, 252);
        flpBranches.Dock = DockStyle.Fill;
        flpBranches.FlowDirection = FlowDirection.TopDown;
        flpBranches.Location = new Point(0, 100);
        flpBranches.Name = "flpBranches";
        flpBranches.Padding = new Padding(22, 16, 22, 16);
        flpBranches.Size = new Size(860, 440);
        flpBranches.WrapContents = false;

        // FrmBranchSelect
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(860, 540);
        Controls.Add(flpBranches);
        Controls.Add(pnlSubheader);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimumSize = new Size(760, 460);
        Name = "FrmBranchSelect";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Chọn chi nhánh — Nhà hàng Hương Việt";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlSubheader.ResumeLayout(false);
        pnlSubheader.PerformLayout();
        ResumeLayout(false);
    }
}
