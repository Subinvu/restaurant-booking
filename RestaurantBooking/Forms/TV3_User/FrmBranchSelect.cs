using RestaurantBooking.Models;
using RestaurantBooking.Services;

namespace RestaurantBooking.Forms.TV3_User;

/// <summary>
/// Màn hình chọn Chi nhánh trước khi vào Thực đơn & Đặt món.
/// Chỉ hiển thị thông tin thân thiện cho khách hàng (không hiện mã chi nhánh nội bộ).
/// </summary>
public partial class FrmBranchSelect : Form
{
    private readonly MenuAdminService _menuService = new();
    private List<RestaurantModel> _restaurants = [];

    public FrmBranchSelect()
    {
        InitializeComponent();
        UiTheme.StyleHeaderBackButton(btnBackMain);
        Load += FrmBranchSelect_Load;
        Resize += (_, _) => AdjustCardWidths();
    }

    private async void FrmBranchSelect_Load(object? sender, EventArgs e)
    {
        try
        {
            _restaurants = await _menuService.GetRestaurantsAsync(activeOnly: true);
            if (IsDisposed) return;

            RenderBranchCards();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không tải được danh sách chi nhánh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RenderBranchCards()
    {
        flpBranches.SuspendLayout();
        flpBranches.Controls.Clear();

        if (_restaurants.Count == 0)
        {
            var empty = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = UiTheme.TextSecondary,
                Text = "Hiện chưa có chi nhánh nào đang mở phục vụ."
            };
            flpBranches.Controls.Add(empty);
            flpBranches.ResumeLayout();
            return;
        }

        int cardWidth = ComputeBranchCardLayout();

        foreach (var branch in _restaurants)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 14),
                Size = new Size(cardWidth, 114),
                Tag = branch
            };

            var accent = new Panel
            {
                BackColor = UiTheme.HeaderBg,
                Dock = DockStyle.Left,
                Width = 5
            };

            var lblName = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(20, 14),
                Text = branch.Name,
                UseMnemonic = false
            };

            var lblAddress = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(20, 44),
                Text = $"Địa chỉ: {branch.Address}",
                UseMnemonic = false
            };

            int activeTables = branch.Tables.Count(t => t.IsAvailable);
            var lblMeta = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(20, 72),
                Text = $"Giờ mở cửa: {branch.OpenTime} – {branch.CloseTime}   •   Điện thoại: {branch.Phone}   •   Quy mô: {activeTables} bàn",
                UseMnemonic = false
            };

            var btnSelect = new Button
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(cardWidth - 178, 36),
                Size = new Size(156, 40),
                Text = "Xem thực đơn →"
            };
            UiTheme.StylePrimaryButton(btnSelect);

            EventHandler openHandler = (_, _) => OpenBranchMenu(branch);
            card.Click += openHandler;
            lblName.Click += openHandler;
            lblAddress.Click += openHandler;
            lblMeta.Click += openHandler;
            btnSelect.Click += openHandler;

            card.Controls.Add(btnSelect);
            card.Controls.Add(lblMeta);
            card.Controls.Add(lblAddress);
            card.Controls.Add(lblName);
            card.Controls.Add(accent);

            flpBranches.Controls.Add(card);
        }

        flpBranches.ResumeLayout();
    }

    private int ComputeBranchCardLayout()
    {
        int availW = flpBranches.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 16;
        int maxCardW = (int)(1100 * (DeviceDpi / 96f));
        int minCardW = (int)(620 * (DeviceDpi / 96f));
        int cardWidth = Math.Clamp(availW - 48, minCardW, Math.Max(minCardW, maxCardW));
        int padX = Math.Max(24, (flpBranches.ClientSize.Width - cardWidth - 8) / 2);
        int totalCardsH = _restaurants.Count * (int)(128 * (DeviceDpi / 96f));
        int padY = _restaurants.Count > 0 && flpBranches.ClientSize.Height > totalCardsH + 120
            ? Math.Clamp((flpBranches.ClientSize.Height - totalCardsH) / 3, 24, 120)
            : 20;
        flpBranches.Padding = new Padding(padX, padY, padX, 24);
        return cardWidth;
    }

    private void AdjustCardWidths()
    {
        if (flpBranches.Controls.Count == 0) return;
        int cardWidth = ComputeBranchCardLayout();
        foreach (Control ctrl in flpBranches.Controls)
        {
            if (ctrl is Panel p)
                p.Width = cardWidth;
        }
    }

    private void OpenBranchMenu(RestaurantModel branch)
    {
        Hide();
        using var homeForm = new FrmHome(branch);
        homeForm.ShowDialog(this);

        if (homeForm.ReturnToMainRequested)
        {
            Close();
            return;
        }

        if (!IsDisposed)
        {
            Show();
            Activate();
        }
    }

    private void btnBackMain_Click(object? sender, EventArgs e) => Close();
}
