using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BJTSurrenSystem
{
    /// <summary>
    /// 主程序第一阶段界面主题。
    /// 只在运行时设置控件外观，不改控件名称、不改事件绑定，避免影响 PLC/MES 等业务逻辑。
    /// </summary>
    internal static class AppUiTheme
    {
        private static readonly HashSet<Form> ThemedForms = new HashSet<Form>();
        private static readonly HashSet<Control> ExcludedControls = new HashSet<Control>();
        private static bool applicationThemeEnabled;

        private static readonly Color PageBackColor = Color.FromArgb(245, 247, 251);
        private static readonly Color PanelBackColor = Color.FromArgb(250, 252, 255);
        private static readonly Color SurfaceColor = Color.White;
        private static readonly Color BorderColor = Color.FromArgb(210, 218, 230);
        private static readonly Color TextColor = Color.FromArgb(35, 45, 60);
        private static readonly Color MutedTextColor = Color.FromArgb(96, 108, 124);
        private static readonly Color PrimaryColor = Color.FromArgb(30, 113, 201);
        private static readonly Color PrimaryHoverColor = Color.FromArgb(24, 96, 176);
        private static readonly Color SuccessColor = Color.FromArgb(38, 149, 92);
        private static readonly Color DangerColor = Color.FromArgb(185, 28, 28);
        private static readonly Color WarningColor = Color.FromArgb(217, 119, 6);

        private static readonly Font MainFont = CreateFont(10.5F, FontStyle.Regular);
        private static readonly Font MenuFont = CreateFont(11F, FontStyle.Regular);
        private static readonly Font TitleFont = CreateFont(11F, FontStyle.Bold);
        private static readonly Font GridFont = CreateFont(10F, FontStyle.Regular);
        private static readonly Font GridHeaderFont = CreateFont(10.5F, FontStyle.Bold);

        /// <summary>
        /// 让后续打开的配置页、管理页和登录页自动使用统一主题。
        /// </summary>
        public static void EnableForApplication()
        {
            if (applicationThemeEnabled)
            {
                return;
            }

            applicationThemeEnabled = true;
            Application.Idle += Application_Idle;
        }

        /// <summary>
        /// 将指定控件及其所有子控件排除在统一主题之外。
        /// 用于保留历史数据页原有的专用报表样式。
        /// </summary>
        public static void Exclude(Control control)
        {
            if (control == null || control.IsDisposed || ExcludedControls.Contains(control))
            {
                return;
            }

            ExcludedControls.Add(control);
            control.Disposed += ExcludedControl_Disposed;
        }

        public static void Apply(Form form)
        {
            if (form == null || form.IsDisposed || IsExcluded(form))
            {
                return;
            }

            form.BackColor = PageBackColor;
            form.Font = MainFont;

            foreach (Control control in form.Controls)
            {
                ApplyControl(control);
            }
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            List<Form> openForms = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                openForms.Add(form);
            }

            foreach (Form form in openForms)
            {
                if (form == null || form.IsDisposed || ThemedForms.Contains(form))
                {
                    continue;
                }

                Apply(form);
                ThemedForms.Add(form);
                form.Disposed += ThemedForm_Disposed;
            }
        }

        private static void ThemedForm_Disposed(object sender, EventArgs e)
        {
            Form form = sender as Form;
            if (form != null)
            {
                ThemedForms.Remove(form);
            }
        }

        private static void ExcludedControl_Disposed(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                ExcludedControls.Remove(control);
            }
        }

        private static Font CreateFont(float size, FontStyle style)
        {
            try
            {
                return new Font("Microsoft YaHei UI", size, style);
            }
            catch
            {
                return new Font(SystemFonts.MessageBoxFont.FontFamily, size, style);
            }
        }

        private static void ApplyControl(Control control)
        {
            if (control == null || control.IsDisposed || IsExcluded(control))
            {
                return;
            }

            ToolStrip toolStrip = control as ToolStrip;
            if (toolStrip != null)
            {
                ApplyToolStrip(toolStrip);
            }
            else if (control is TabControl)
            {
                ApplyTabControl((TabControl)control);
            }
            else if (control is TabPage)
            {
                ApplyTabPage((TabPage)control);
            }
            else if (control is DataGridView)
            {
                ApplyGrid((DataGridView)control);
            }
            else if (control is Button)
            {
                ApplyButton((Button)control);
            }
            else if (control is GroupBox)
            {
                ApplyGroupBox((GroupBox)control);
            }
            else if (control is TextBoxBase)
            {
                ApplyTextBox((TextBoxBase)control);
            }
            else if (control is ComboBox)
            {
                ApplyComboBox((ComboBox)control);
            }
            else if (control is DateTimePicker)
            {
                ApplyDateTimePicker((DateTimePicker)control);
            }
            else if (control is Label)
            {
                ApplyLabel((Label)control);
            }
            else if (control is Chart)
            {
                ApplyChart((Chart)control);
            }
            else if (control is TableLayoutPanel || control is FlowLayoutPanel)
            {
                control.BackColor = PageBackColor;
            }
            else if (control is Panel)
            {
                ApplyPanel((Panel)control);
            }

            foreach (Control child in control.Controls)
            {
                ApplyControl(child);
            }
        }

        private static bool IsExcluded(Control control)
        {
            Control current = control;
            while (current != null)
            {
                if (ExcludedControls.Contains(current))
                {
                    return true;
                }

                current = current.Parent;
            }

            return false;
        }

        private static void ApplyToolStrip(ToolStrip toolStrip)
        {
            toolStrip.Font = MenuFont;
            toolStrip.BackColor = SurfaceColor;
            toolStrip.ForeColor = TextColor;
            toolStrip.RenderMode = ToolStripRenderMode.Professional;
            toolStrip.Renderer = new ThemeToolStripRenderer();
            toolStrip.Padding = new Padding(4, 3, 4, 3);
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;

            foreach (ToolStripItem item in toolStrip.Items)
            {
                ApplyToolStripItem(item);
            }
        }

        private static void ApplyToolStripItem(ToolStripItem item)
        {
            if (item == null)
            {
                return;
            }

            item.Font = MenuFont;
            item.Margin = new Padding(2, 0, 2, 0);

            if (item is ToolStripDropDownButton || item is ToolStripMenuItem)
            {
                item.ForeColor = TextColor;
            }

            ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
            if (dropDownItem != null)
            {
                dropDownItem.DropDown.BackColor = SurfaceColor;
                dropDownItem.DropDown.Padding = new Padding(2);
                dropDownItem.DropDown.Renderer = new ThemeToolStripRenderer();
                foreach (ToolStripItem child in dropDownItem.DropDownItems)
                {
                    ApplyToolStripItem(child);
                }
            }
        }

        private static void ApplyTabControl(TabControl tabControl)
        {
            tabControl.Font = MenuFont;
            tabControl.BackColor = PageBackColor;
        }

        private static void ApplyTabPage(TabPage tabPage)
        {
            tabPage.BackColor = PageBackColor;
            tabPage.ForeColor = TextColor;
            if (tabPage.Padding.Left < 6)
            {
                tabPage.Padding = new Padding(8);
            }
        }

        private static void ApplyPanel(Panel panel)
        {
            panel.BackColor = panel.BorderStyle == BorderStyle.None ? PageBackColor : SurfaceColor;
        }

        private static void ApplyGroupBox(GroupBox groupBox)
        {
            groupBox.BackColor = SurfaceColor;
            groupBox.ForeColor = TextColor;
            groupBox.Font = TitleFont;
            if (groupBox.Padding.Left < 6)
            {
                groupBox.Padding = new Padding(8);
            }
        }

        private static void ApplyLabel(Label label)
        {
            label.ForeColor = TextColor;
            label.Font = label.Font.Bold ? TitleFont : MainFont;
            label.BackColor = Color.Transparent;
        }

        private static void ApplyTextBox(TextBoxBase textBox)
        {
            textBox.Font = MainFont;
            textBox.BackColor = textBox.ReadOnly ? Color.FromArgb(243, 246, 250) : SurfaceColor;
            textBox.ForeColor = TextColor;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void ApplyComboBox(ComboBox comboBox)
        {
            comboBox.Font = MainFont;
            comboBox.BackColor = SurfaceColor;
            comboBox.ForeColor = TextColor;
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        private static void ApplyDateTimePicker(DateTimePicker picker)
        {
            picker.Font = MainFont;
            picker.CalendarTitleBackColor = PrimaryColor;
            picker.CalendarTitleForeColor = Color.White;
            picker.CalendarForeColor = TextColor;
            picker.CalendarMonthBackground = SurfaceColor;
        }

        private static void ApplyButton(Button button)
        {
            button.Font = TitleFont;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = BorderColor;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            string text = (button.Text ?? string.Empty).Trim();
            if (ContainsAny(text, "查询", "搜索", "确定", "保存", "登录"))
            {
                SetButtonColor(button, PrimaryColor, PrimaryHoverColor, Color.White);
            }
            else if (ContainsAny(text, "导出", "打开", "添加", "刷新", "上传"))
            {
                SetButtonColor(button, SuccessColor, Color.FromArgb(28, 125, 74), Color.White);
            }
            else if (ContainsAny(text, "删除", "清空", "取消"))
            {
                SetButtonColor(button, DangerColor, Color.FromArgb(153, 27, 27), Color.White);
            }
            else if (ContainsAny(text, "重置", "修改", "导入"))
            {
                SetButtonColor(button, SurfaceColor, Color.FromArgb(239, 244, 250), TextColor);
            }
            else
            {
                SetButtonColor(button, SurfaceColor, Color.FromArgb(239, 244, 250), TextColor);
            }
        }

        private static bool ContainsAny(string text, params string[] keywords)
        {
            foreach (string keyword in keywords)
            {
                if (text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetButtonColor(Button button, Color backColor, Color hoverColor, Color foreColor)
        {
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatAppearance.MouseOverBackColor = hoverColor;
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(hoverColor);
        }

        private static void ApplyGrid(DataGridView grid)
        {
            grid.BackgroundColor = SurfaceColor;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = BorderColor;
            grid.EnableHeadersVisualStyles = false;
            grid.Font = GridFont;
            grid.RowTemplate.Height = Math.Max(grid.RowTemplate.Height, 30);
            grid.ColumnHeadersHeight = Math.Max(grid.ColumnHeadersHeight, 34);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 243, 249);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            grid.ColumnHeadersDefaultCellStyle.Font = GridHeaderFont;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 243, 249);
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextColor;
            grid.DefaultCellStyle.BackColor = SurfaceColor;
            grid.DefaultCellStyle.ForeColor = TextColor;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 126, 213);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 251, 254);
            grid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 243, 249);
            grid.RowHeadersDefaultCellStyle.ForeColor = MutedTextColor;
            grid.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 126, 213);
            grid.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }

        private static void ApplyChart(Chart chart)
        {
            chart.BackColor = SurfaceColor;
            chart.BorderlineColor = BorderColor;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineWidth = 1;

            foreach (ChartArea area in chart.ChartAreas)
            {
                area.BackColor = SurfaceColor;
                area.AxisX.LabelStyle.ForeColor = TextColor;
                area.AxisY.LabelStyle.ForeColor = TextColor;
                area.AxisX.MajorGrid.LineColor = Color.FromArgb(225, 232, 241);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(225, 232, 241);
                area.AxisX.LineColor = BorderColor;
                area.AxisY.LineColor = BorderColor;
            }

            foreach (Legend legend in chart.Legends)
            {
                legend.BackColor = SurfaceColor;
                legend.ForeColor = TextColor;
                legend.Font = MainFont;
            }

            foreach (Title title in chart.Titles)
            {
                title.ForeColor = TextColor;
                title.Font = TitleFont;
            }
        }

        private sealed class ThemeToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ThemeToolStripRenderer()
                : base(new ThemeColorTable())
            {
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using (Pen pen = new Pen(BorderColor))
                {
                    e.Graphics.DrawLine(pen, 0, e.ToolStrip.Height - 1, e.ToolStrip.Width, e.ToolStrip.Height - 1);
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
                Color fill = e.Item.Selected || e.Item.Pressed
                    ? Color.FromArgb(232, 242, 253)
                    : SurfaceColor;
                using (SolidBrush brush = new SolidBrush(fill))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
        }

        private sealed class ThemeColorTable : ProfessionalColorTable
        {
            public override Color ToolStripGradientBegin { get { return SurfaceColor; } }
            public override Color ToolStripGradientMiddle { get { return SurfaceColor; } }
            public override Color ToolStripGradientEnd { get { return SurfaceColor; } }
            public override Color MenuStripGradientBegin { get { return SurfaceColor; } }
            public override Color MenuStripGradientEnd { get { return SurfaceColor; } }
            public override Color ToolStripDropDownBackground { get { return SurfaceColor; } }
            public override Color ImageMarginGradientBegin { get { return Color.FromArgb(246, 249, 253); } }
            public override Color ImageMarginGradientMiddle { get { return Color.FromArgb(246, 249, 253); } }
            public override Color ImageMarginGradientEnd { get { return Color.FromArgb(246, 249, 253); } }
            public override Color MenuItemSelected { get { return Color.FromArgb(232, 242, 253); } }
            public override Color MenuItemSelectedGradientBegin { get { return Color.FromArgb(232, 242, 253); } }
            public override Color MenuItemSelectedGradientEnd { get { return Color.FromArgb(232, 242, 253); } }
            public override Color MenuItemBorder { get { return Color.FromArgb(160, 198, 238); } }
            public override Color ButtonSelectedGradientBegin { get { return Color.FromArgb(232, 242, 253); } }
            public override Color ButtonSelectedGradientEnd { get { return Color.FromArgb(232, 242, 253); } }
            public override Color ButtonPressedGradientBegin { get { return Color.FromArgb(216, 234, 252); } }
            public override Color ButtonPressedGradientEnd { get { return Color.FromArgb(216, 234, 252); } }
            public override Color SeparatorDark { get { return BorderColor; } }
            public override Color SeparatorLight { get { return SurfaceColor; } }
        }
    }
}
