namespace RestaurantBooking.Forms;

/// <summary>
/// Bộ phong cách giao diện tối giản, nhất quán cho toàn bộ ứng dụng Nhà hàng Hương Việt.
/// Tách khỏi file .Designer.cs để giữ 100% tương thích với Visual Studio WinForms Designer.
/// </summary>
public static class UiTheme
{
    public static readonly Color HeaderBg = Color.FromArgb(30, 41, 59);       // Slate-800
    public static readonly Color SurfaceBg = Color.White;
    public static readonly Color SubtleBg = Color.FromArgb(248, 250, 252);    // Slate-50
    public static readonly Color MutedBg = Color.FromArgb(241, 245, 249);     // Slate-100
    public static readonly Color BorderColor = Color.FromArgb(203, 213, 225); // Slate-300
    public static readonly Color GridLineColor = Color.FromArgb(226, 232, 240);
    public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);    // Slate-900
    public static readonly Color TextSecondary = Color.FromArgb(71, 85, 105); // Slate-600
    public static readonly Color PrimaryBlue = Color.FromArgb(29, 78, 216);   // Blue-700
    public static readonly Color PrimaryGreen = Color.FromArgb(21, 128, 61);  // Green-700
    public static readonly Color DangerRed = Color.FromArgb(185, 28, 28);     // Red-700
    public static readonly Color DangerSoftBg = Color.FromArgb(254, 242, 242);
    public static readonly Color SelectionBg = Color.FromArgb(219, 234, 254); // Blue-100

    /// <summary>
    /// Làm nổi bật thanh TabControl (to, rõ, có vạch chân xanh đậm ở tab đang chọn),
    /// đồng thời giữ nguyên font chuẩn cho các control bên trong mỗi TabPage.
    /// </summary>
    public static void StyleTabControl(TabControl tab)
    {
        var baseFont = tab.Parent?.Font ?? new Font("Segoe UI", 9F, FontStyle.Regular);
        tab.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        foreach (TabPage page in tab.TabPages)
            page.Font = baseFont;

        tab.Padding = new Point(18, 9);
        tab.SizeMode = TabSizeMode.Normal;
        tab.DrawMode = TabDrawMode.OwnerDrawFixed;
        tab.DrawItem -= Tab_DrawItem;
        tab.DrawItem += Tab_DrawItem;
    }

    private static void Tab_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (sender is not TabControl tc || e.Index < 0 || e.Index >= tc.TabPages.Count) return;

        var page = tc.TabPages[e.Index];
        var bounds = tc.GetTabRect(e.Index);
        bool isSelected = e.Index == tc.SelectedIndex;

        using var bgBrush = new SolidBrush(isSelected ? SurfaceBg : MutedBg);
        e.Graphics.FillRectangle(bgBrush, bounds);

        using var borderPen = new Pen(BorderColor, 1);
        e.Graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

        if (isSelected)
        {
            using var accentBrush = new SolidBrush(PrimaryBlue);
            e.Graphics.FillRectangle(accentBrush, bounds.X + 1, bounds.Bottom - 3, bounds.Width - 2, 3);
        }

        var displayText = page.Text.Replace("&&", "&");
        using var font = new Font("Segoe UI", 9.5F, isSelected ? FontStyle.Bold : FontStyle.Regular);
        var foreColor = isSelected ? HeaderBg : TextSecondary;

        TextRenderer.DrawText(
            e.Graphics,
            displayText,
            font,
            bounds,
            foreColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
    }

    /// <summary>
    /// Chuẩn hóa bảng DataGridView phẳng, thoáng, dễ đọc và dịu mắt.
    /// </summary>
    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = SurfaceBg;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        grid.EnableHeadersVisualStyles = false;
        grid.GridColor = GridLineColor;
        grid.RowHeadersVisible = false;
        grid.RowTemplate.Height = 30;

        grid.ColumnHeadersDefaultCellStyle.BackColor = MutedBg;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = MutedBg;
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextPrimary;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 4, 4, 4);
        grid.ColumnHeadersHeight = 34;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        grid.DefaultCellStyle.BackColor = SurfaceBg;
        grid.DefaultCellStyle.ForeColor = TextPrimary;
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
        grid.DefaultCellStyle.SelectionBackColor = SelectionBg;
        grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
        grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

        grid.AlternatingRowsDefaultCellStyle.BackColor = SubtleBg;
    }

    /// <summary>
    /// Áp dụng phong cách nút thẻ chức năng tối giản (trắng sạch, viền mảnh, hover nhẹ).
    /// </summary>
    public static void StyleCardButton(Button btn)
    {
        btn.BackColor = SurfaceBg;
        btn.ForeColor = TextPrimary;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = BorderColor;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.MouseOverBackColor = MutedBg;
        btn.FlatAppearance.MouseDownBackColor = SelectionBg;
        btn.Cursor = Cursors.Hand;
    }

    public static void StyleSecondaryButton(Button btn)
    {
        btn.BackColor = SurfaceBg;
        btn.ForeColor = TextPrimary;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = BorderColor;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.MouseOverBackColor = MutedBg;
        btn.Cursor = Cursors.Hand;
    }

    public static void StylePrimaryButton(Button btn, bool useGreen = false)
    {
        btn.BackColor = useGreen ? PrimaryGreen : PrimaryBlue;
        btn.ForeColor = Color.White;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.Cursor = Cursors.Hand;
    }

    public static void StyleDangerButton(Button btn)
    {
        btn.BackColor = DangerSoftBg;
        btn.ForeColor = DangerRed;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
        btn.FlatAppearance.BorderSize = 1;
        btn.Cursor = Cursors.Hand;
    }

    public static void StyleHeaderBackButton(Button btn)
    {
        btn.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
        btn.ForeColor = Color.White;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = Color.FromArgb(100, 116, 139);
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 85, 105);
        btn.Cursor = Cursors.Hand;
    }
}
