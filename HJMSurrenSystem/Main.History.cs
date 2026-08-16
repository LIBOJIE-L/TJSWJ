using HJMSurrenSystem.History;
using HJMSurrenSystem.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HJMSurrenSystem
{
    public partial class Main
    {
        private const string HistoryNoParameterText = "（请选择PLC参数）";
        private ComboBox historyPeriodComboBox;
        private ComboBox historyYearComboBox;
        private ComboBox historyQuarterComboBox;
        private DateTimePicker historyStartDatePicker;
        private DateTimePicker historyEndDatePicker;
        private ComboBox historySourceComboBox;
        private ComboBox historyParameterComboBox;
        private TextBox historyBarcodeTextBox;
        private Button historyQueryButton;
        private Button historyResetButton;
        private Button historyExportButton;
        private Button historyOpenFolderButton;
        private Label historyTotalValueLabel;
        private Label historyOkValueLabel;
        private Label historyNgValueLabel;
        private Label historyYieldValueLabel;
        private Label historyParameterAverageValueLabel;
        private Label historyOutOfLimitValueLabel;
        private Chart historyProductionChart;
        private Chart historyParameterChart;
        private DataGridView historyDataGridView;
        private Label historyStatusLabel;
        private ProgressBar historyProgressBar;
        private HistoryDataRepository historyRepository;
        private HistoryQueryResult lastHistoryQueryResult;
        private Dictionary<string, HistoryParameterLimit> historyParameterLimits;
        private CancellationTokenSource historyQueryCancellation;
        private bool historyStartupLoadStarted;

        private void InitializeHistoryDataPage()
        {
            if (tabControl2 == null || tabControl2.IsDisposed)
            {
                return;
            }

            // 主界面会在启动语言初始化时重新创建全部设计器控件。若本方法被重复
            // 调用，只保留当前 tabControl2 中的一份历史页面和一套事件订阅。
            if (historyTabPage != null &&
                !historyTabPage.IsDisposed &&
                tabControl2.TabPages.Contains(historyTabPage) &&
                historyDataGridView != null &&
                !historyDataGridView.IsDisposed &&
                historyTabPage.Contains(historyDataGridView))
            {
                ApplyHistoryPageLanguage();
                return;
            }

            if (historyTabPage == null || historyTabPage.IsDisposed)
            {
                historyTabPage = new TabPage { Name = "historyTabPage" };
            }
            historyTabPage.Controls.Clear();
            historyTabPage.Padding = new Padding(8);
            historyTabPage.UseVisualStyleBackColor = true;
            historyParameterLimits = new Dictionary<string, HistoryParameterLimit>(StringComparer.OrdinalIgnoreCase);

            TableLayoutPanel pageLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(2)
            };
            pageLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 53F));
            pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 47F));
            pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

            pageLayout.Controls.Add(CreateHistoryFilterPanel(), 0, 0);
            pageLayout.Controls.Add(CreateHistorySummaryPanel(), 0, 1);
            pageLayout.Controls.Add(CreateHistoryChartPanel(), 0, 2);
            pageLayout.Controls.Add(CreateHistoryDetailGrid(), 0, 3);
            pageLayout.Controls.Add(CreateHistoryStatusPanel(), 0, 4);
            historyTabPage.Controls.Add(pageLayout);

            if (!tabControl2.TabPages.Contains(historyTabPage))
            {
                tabControl2.TabPages.Add(historyTabPage);
            }
            tabControl2.Selected -= HistoryTabControl_Selected;
            tabControl2.Selected += HistoryTabControl_Selected;
            ApplyHistoryPageLanguage();
            ApplyHistoryPeriodSelection();
        }

        private void ApplyHistoryPageLanguage()
        {
            if (historyTabPage == null || historyTabPage.IsDisposed)
            {
                return;
            }

            string language = ResourceHandler.listSystemParameters.Count > 0
                ? ResourceHandler.listSystemParameters[0].Language
                : "中";
            historyTabPage.Text = language == "英"
                ? "History data"
                : language == "德" ? "Verlaufsdaten" : "历史数据";
        }

        private Control CreateHistoryFilterPanel()
        {
            FlowLayoutPanel filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                Padding = new Padding(8, 7, 8, 5),
                BackColor = Color.FromArgb(244, 247, 250)
            };

            historyPeriodComboBox = CreateHistoryComboBox(116);
            historyPeriodComboBox.Items.AddRange(new object[]
            {
                "近7天", "近3个月", "季度", "年度", "自定义"
            });
            historyPeriodComboBox.SelectedIndex = 0;
            historyPeriodComboBox.SelectedIndexChanged += HistoryPeriodComboBox_SelectedIndexChanged;

            historyYearComboBox = CreateHistoryComboBox(86);
            int currentYear = DateTime.Today.Year;
            for (int year = currentYear - 10; year <= currentYear + 1; year++)
            {
                historyYearComboBox.Items.Add(year);
            }
            historyYearComboBox.SelectedItem = currentYear;
            historyYearComboBox.SelectedIndexChanged += HistoryYearOrQuarter_SelectedIndexChanged;

            historyQuarterComboBox = CreateHistoryComboBox(72);
            historyQuarterComboBox.Items.AddRange(new object[] { "Q1", "Q2", "Q3", "Q4" });
            historyQuarterComboBox.SelectedIndex = (DateTime.Today.Month - 1) / 3;
            historyQuarterComboBox.SelectedIndexChanged += HistoryYearOrQuarter_SelectedIndexChanged;

            historyStartDatePicker = new DateTimePicker
            {
                Width = 122,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd"
            };
            historyEndDatePicker = new DateTimePicker
            {
                Width = 122,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd"
            };

            historySourceComboBox = CreateHistoryComboBox(132);
            historySourceComboBox.Items.AddRange(new object[]
            {
                "全部采集数据", "出站数据", "停机出站数据", "侧板出站数据", "进站数据"
            });
            historySourceComboBox.SelectedIndex = 0;

            historyParameterComboBox = CreateHistoryComboBox(210);
            historyParameterComboBox.Items.Add(HistoryNoParameterText);
            historyParameterComboBox.SelectedIndex = 0;
            historyParameterComboBox.SelectedIndexChanged += HistoryParameterComboBox_SelectedIndexChanged;

            historyBarcodeTextBox = new TextBox { Width = 145 };
            historyBarcodeTextBox.KeyDown += HistoryBarcodeTextBox_KeyDown;

            historyQueryButton = CreateHistoryButton("查询", Color.FromArgb(30, 112, 191), Color.White);
            historyQueryButton.Click += delegate { BeginHistoryQuery(); };
            historyResetButton = CreateHistoryButton("重置", Color.White, Color.FromArgb(50, 50, 50));
            historyResetButton.Click += HistoryResetButton_Click;
            historyExportButton = CreateHistoryButton("导出Excel", Color.FromArgb(31, 132, 79), Color.White);
            historyExportButton.Enabled = false;
            historyExportButton.Click += HistoryExportButton_Click;
            historyOpenFolderButton = CreateHistoryButton("打开报表目录", Color.White, Color.FromArgb(50, 50, 50));
            historyOpenFolderButton.Width = 112;
            historyOpenFolderButton.Click += HistoryOpenFolderButton_Click;

            AddHistoryFilter(filterPanel, "周期", historyPeriodComboBox);
            AddHistoryFilter(filterPanel, "年份", historyYearComboBox);
            AddHistoryFilter(filterPanel, "季度", historyQuarterComboBox);
            AddHistoryFilter(filterPanel, "开始日期", historyStartDatePicker);
            AddHistoryFilter(filterPanel, "结束日期", historyEndDatePicker);
            AddHistoryFilter(filterPanel, "数据类型", historySourceComboBox);
            AddHistoryFilter(filterPanel, "PLC参数", historyParameterComboBox);
            AddHistoryFilter(filterPanel, "条码", historyBarcodeTextBox);
            filterPanel.Controls.Add(historyQueryButton);
            filterPanel.Controls.Add(historyResetButton);
            filterPanel.Controls.Add(historyExportButton);
            filterPanel.Controls.Add(historyOpenFolderButton);
            return filterPanel;
        }

        private Control CreateHistorySummaryPanel()
        {
            TableLayoutPanel summaryPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                Padding = new Padding(5, 4, 5, 12)
            };
            for (int column = 0; column < 6; column++)
            {
                summaryPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6667F));
            }

            historyTotalValueLabel = CreateHistorySummaryCard(summaryPanel, 0, "采集总数", Color.FromArgb(30, 112, 191));
            historyOkValueLabel = CreateHistorySummaryCard(summaryPanel, 1, "OK数量", Color.FromArgb(28, 135, 84));
            historyNgValueLabel = CreateHistorySummaryCard(summaryPanel, 2, "NG数量", Color.FromArgb(190, 36, 40));
            historyYieldValueLabel = CreateHistorySummaryCard(summaryPanel, 3, "合格率", Color.FromArgb(124, 78, 153));
            historyParameterAverageValueLabel = CreateHistorySummaryCard(summaryPanel, 4, "参数平均值", Color.FromArgb(205, 112, 23));
            historyOutOfLimitValueLabel = CreateHistorySummaryCard(summaryPanel, 5, "参数超限数", Color.FromArgb(115, 72, 48));
            return summaryPanel;
        }

        private Control CreateHistoryChartPanel()
        {
            TableLayoutPanel chartLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(5, 12, 5, 5),
                Margin = Padding.Empty,
                AutoScroll = false
            };
            chartLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            chartLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            historyProductionChart = CreateHistoryChart("采集数量及OK/NG趋势");
            historyParameterChart = CreateHistoryChart("PLC参数趋势（请选择参数）");
            chartLayout.Controls.Add(historyProductionChart, 0, 0);
            chartLayout.Controls.Add(historyParameterChart, 1, 0);
            return chartLayout;
        }

        private Control CreateHistoryDetailGrid()
        {
            historyDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                RowHeadersVisible = false,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
            };
            historyDataGridView.Columns.Add("historySequenceColumn", "序号");
            historyDataGridView.Columns.Add("historyTimeColumn", "采集时间");
            historyDataGridView.Columns.Add("historySourceColumn", "数据类型");
            historyDataGridView.Columns.Add("historyBarcodeColumn", "条码/模组码");
            historyDataGridView.Columns.Add("historyResultColumn", "结果");
            historyDataGridView.Columns.Add("historyDeviceColumn", "设备标识");
            historyDataGridView.Columns.Add("historyParameterColumn", "所选参数值");
            historyDataGridView.Columns[0].FillWeight = 35F;
            historyDataGridView.Columns[1].FillWeight = 95F;
            historyDataGridView.Columns[2].FillWeight = 75F;
            historyDataGridView.Columns[3].FillWeight = 150F;
            historyDataGridView.Columns[4].FillWeight = 45F;
            historyDataGridView.Columns[5].FillWeight = 90F;
            historyDataGridView.Columns[6].FillWeight = 120F;
            historyDataGridView.CellDoubleClick += HistoryDataGridView_CellDoubleClick;
            return historyDataGridView;
        }

        private Control CreateHistoryStatusPanel()
        {
            TableLayoutPanel statusPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(6, 3, 6, 2)
            };
            statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190F));
            historyStatusLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "准备就绪"
            };
            historyProgressBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25,
                Visible = false
            };
            statusPanel.Controls.Add(historyStatusLabel, 0, 0);
            statusPanel.Controls.Add(historyProgressBar, 1, 0);
            return statusPanel;
        }

        private static ComboBox CreateHistoryComboBox(int width)
        {
            return new ComboBox
            {
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                IntegralHeight = false,
                MaxDropDownItems = 16
            };
        }

        private static Button CreateHistoryButton(string text, Color backColor, Color foreColor)
        {
            return new Button
            {
                Text = text,
                Width = 82,
                Height = 30,
                Margin = new Padding(5, 13, 2, 2),
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat
            };
        }

        private static void AddHistoryFilter(FlowLayoutPanel panel, string labelText, Control inputControl)
        {
            Panel itemPanel = new Panel
            {
                Width = inputControl.Width + 8,
                Height = 49,
                Margin = new Padding(3, 0, 3, 0)
            };
            Label label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 20,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = Color.FromArgb(70, 70, 70)
            };
            inputControl.Dock = DockStyle.Bottom;
            inputControl.Height = 25;
            itemPanel.Controls.Add(inputControl);
            itemPanel.Controls.Add(label);
            panel.Controls.Add(itemPanel);
        }

        private static Label CreateHistorySummaryCard(
            TableLayoutPanel summaryPanel,
            int column,
            string title,
            Color accentColor)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 4, 4, 6),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            TableLayoutPanel cardLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel accent = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = accentColor
            };
            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = new Padding(8, 4, 6, 8)
            };
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Label titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(95, 95, 95)
            };
            Label valueLabel = new Label
            {
                Text = "--",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 13F, FontStyle.Bold),
                ForeColor = accentColor
            };
            contentLayout.Controls.Add(titleLabel, 0, 0);
            contentLayout.Controls.Add(valueLabel, 0, 1);
            cardLayout.Controls.Add(accent, 0, 0);
            cardLayout.Controls.Add(contentLayout, 1, 0);
            card.Controls.Add(cardLayout);
            summaryPanel.Controls.Add(card, column, 0);
            return valueLabel;
        }

        private static Chart CreateHistoryChart(string title)
        {
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderlineColor = Color.FromArgb(210, 215, 220),
                BorderlineDashStyle = ChartDashStyle.Solid,
                BorderlineWidth = 1,
                Palette = ChartColorPalette.None
            };
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.BackColor = Color.White;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisX.ScrollBar.Enabled = false;
            chartArea.AxisX.ScaleView.Zoomable = false;
            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartArea.AxisX.LabelStyle.Angle = -35;
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 233, 236);
            chartArea.AxisY.ScrollBar.Enabled = false;
            chartArea.AxisY.ScaleView.Zoomable = false;
            chartArea.AxisY.IsStartedFromZero = true;
            chart.ChartAreas.Add(chartArea);
            chart.Legends.Add(new Legend("MainLegend")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            });
            chart.Titles.Add(new Title(title)
            {
                Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 75)
            });
            return chart;
        }

        private void InitializeHistoryDataAfterLoad()
        {
            // 防止后续界面初始化流程替换 tabControl2 后历史页面没有重新挂载。
            if (historyTabPage == null ||
                historyTabPage.IsDisposed ||
                tabControl2 == null ||
                !tabControl2.TabPages.Contains(historyTabPage))
            {
                InitializeHistoryDataPage();
            }
            ApplyHistoryPageLanguage();

            if (historyStartupLoadStarted)
            {
                return;
            }

            historyStartupLoadStarted = true;
            string dataRoot = ResourceHandler.listSystemParameters.Count > 0
                ? ResourceHandler.listSystemParameters[0].ProgramLogPath
                : string.Empty;
            historyRepository = new HistoryDataRepository(dataRoot);
            historyParameterLimits = ReadCurrentHistoryParameterLimits();
            BeginHistoryQuery();
        }

        private async void BeginHistoryQuery()
        {
            if (historyRepository == null)
            {
                string dataRoot = ResourceHandler.listSystemParameters.Count > 0
                    ? ResourceHandler.listSystemParameters[0].ProgramLogPath
                    : string.Empty;
                historyRepository = new HistoryDataRepository(dataRoot);
            }

            HistoryFilter filter = CreateHistoryFilter();
            if (filter.EndTime < filter.StartTime)
            {
                MessageBox.Show("结束日期不能早于开始日期！", "历史数据", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (historyQueryCancellation != null)
            {
                historyQueryCancellation.Cancel();
                historyQueryCancellation.Dispose();
            }
            historyQueryCancellation = new CancellationTokenSource();
            CancellationToken cancellationToken = historyQueryCancellation.Token;
            SetHistoryBusyState(true, "正在检查历史数据索引...");
            Progress<string> progress = new Progress<string>(message => historyStatusLabel.Text = message);

            try
            {
                HistoryQueryResult queryResult = await Task.Run(
                    () => historyRepository.Query(filter, progress, cancellationToken),
                    cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                lastHistoryQueryResult = queryResult;
                PopulateHistoryParameterOptions(GetAvailableHistoryParameterNames(queryResult.NumericParameters));
                RenderHistoryQueryResult();
                historyExportButton.Enabled = queryResult.TotalCount > 0;
                historyStatusLabel.Text = string.Format(
                    "查询完成：{0:yyyy-MM-dd} 至 {1:yyyy-MM-dd}，共 {2} 条；双击明细可定位原始CSV。",
                    filter.StartTime,
                    filter.EndTime,
                    queryResult.TotalCount);
            }
            catch (OperationCanceledException)
            {
                historyStatusLabel.Text = "历史数据查询已取消。";
            }
            catch (Exception exception)
            {
                historyStatusLabel.Text = "历史数据查询失败：" + exception.Message;
                outDiary("历史数据查询失败：" + exception, "错误");
                MessageBox.Show(
                    "历史数据查询失败：\r\n" + exception.Message,
                    "历史数据",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    SetHistoryBusyState(false, historyStatusLabel.Text);
                }
            }
        }

        private HistoryFilter CreateHistoryFilter()
        {
            return new HistoryFilter
            {
                StartTime = historyStartDatePicker.Value.Date,
                EndTime = historyEndDatePicker.Value.Date.AddDays(1).AddTicks(-1),
                SourceType = historySourceComboBox.SelectedItem == null
                    ? "全部采集数据"
                    : historySourceComboBox.SelectedItem.ToString(),
                Barcode = historyBarcodeTextBox.Text.Trim(),
                PeriodKind = GetSelectedHistoryPeriodKind()
            };
        }

        private HistoryPeriodKind GetSelectedHistoryPeriodKind()
        {
            switch (historyPeriodComboBox.SelectedIndex)
            {
                case 1:
                    return HistoryPeriodKind.LastThreeMonths;
                case 2:
                    return HistoryPeriodKind.Quarter;
                case 3:
                    return HistoryPeriodKind.Year;
                case 4:
                    return HistoryPeriodKind.Custom;
                default:
                    return HistoryPeriodKind.LastSevenDays;
            }
        }

        private void ApplyHistoryPeriodSelection()
        {
            if (historyPeriodComboBox == null)
            {
                return;
            }

            DateTime today = DateTime.Today;
            HistoryPeriodKind periodKind = GetSelectedHistoryPeriodKind();
            int selectedYear = historyYearComboBox.SelectedItem is int
                ? (int)historyYearComboBox.SelectedItem
                : today.Year;
            int selectedQuarter = historyQuarterComboBox.SelectedIndex < 0
                ? (today.Month - 1) / 3
                : historyQuarterComboBox.SelectedIndex;

            historyYearComboBox.Enabled = periodKind == HistoryPeriodKind.Quarter || periodKind == HistoryPeriodKind.Year;
            historyQuarterComboBox.Enabled = periodKind == HistoryPeriodKind.Quarter;
            historyStartDatePicker.Enabled = periodKind == HistoryPeriodKind.Custom;
            historyEndDatePicker.Enabled = periodKind == HistoryPeriodKind.Custom;

            switch (periodKind)
            {
                case HistoryPeriodKind.LastThreeMonths:
                    historyStartDatePicker.Value = today.AddMonths(-3).AddDays(1);
                    historyEndDatePicker.Value = today;
                    break;
                case HistoryPeriodKind.Quarter:
                    DateTime quarterStart = new DateTime(selectedYear, selectedQuarter * 3 + 1, 1);
                    historyStartDatePicker.Value = quarterStart;
                    historyEndDatePicker.Value = quarterStart.AddMonths(3).AddDays(-1);
                    break;
                case HistoryPeriodKind.Year:
                    historyStartDatePicker.Value = new DateTime(selectedYear, 1, 1);
                    historyEndDatePicker.Value = new DateTime(selectedYear, 12, 31);
                    break;
                case HistoryPeriodKind.Custom:
                    break;
                default:
                    historyStartDatePicker.Value = today.AddDays(-6);
                    historyEndDatePicker.Value = today;
                    break;
            }
        }

        private void PopulateHistoryParameterOptions(IEnumerable<string> parameters)
        {
            string currentSelection = GetSelectedHistoryParameter();
            historyParameterComboBox.BeginUpdate();
            historyParameterComboBox.Items.Clear();
            historyParameterComboBox.Items.Add(HistoryNoParameterText);
            foreach (string parameter in parameters)
            {
                historyParameterComboBox.Items.Add(parameter);
            }

            int selectedIndex = string.IsNullOrWhiteSpace(currentSelection)
                ? -1
                : historyParameterComboBox.FindStringExact(currentSelection);
            historyParameterComboBox.SelectedIndex = selectedIndex >= 0
                ? selectedIndex
                : (historyParameterComboBox.Items.Count > 1 ? 1 : 0);
            historyParameterComboBox.EndUpdate();
        }

        private IEnumerable<string> GetAvailableHistoryParameterNames(IEnumerable<string> historyParameters)
        {
            IEnumerable<string> configuredParameters = ResourceHandler.listMesPullOutUploadingParameters
                .Where(IsNumericHistoryParameter)
                .Select(item => item.Header == null ? string.Empty : item.Header.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item));

            return configuredParameters
                .Concat(historyParameters ?? Enumerable.Empty<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.CurrentCulture)
                .ToList();
        }

        private static bool IsNumericHistoryParameter(MesPullOutUploadingParameters parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter.Header))
            {
                return false;
            }

            string mesType = (parameter.ParametersMESType ?? string.Empty).Trim();
            if (mesType.Equals("NUMBER", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string plcType = (parameter.ParametersPLCType ?? string.Empty).Trim().ToLowerInvariant();
            switch (plcType)
            {
                case "byte":
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                case "float":
                case "double":
                case "decimal":
                    return true;
                default:
                    return false;
            }
        }

        private string GetSelectedHistoryParameter()
        {
            if (historyParameterComboBox.SelectedItem == null ||
                string.Equals(historyParameterComboBox.SelectedItem.ToString(), HistoryNoParameterText, StringComparison.Ordinal))
            {
                return string.Empty;
            }

            return historyParameterComboBox.SelectedItem.ToString();
        }

        private void RenderHistoryQueryResult()
        {
            if (lastHistoryQueryResult == null)
            {
                return;
            }

            string parameterName = GetSelectedHistoryParameter();
            HistoryParameterLimit parameterLimit = FindHistoryParameterLimit(parameterName);
            HistoryParameterStatistics parameterStatistics = HistoryAggregator.CreateParameterStatistics(
                lastHistoryQueryResult,
                parameterName,
                parameterLimit);
            List<HistoryBucket> buckets = HistoryAggregator.CreateBuckets(lastHistoryQueryResult, parameterName);

            historyTotalValueLabel.Text = lastHistoryQueryResult.TotalCount.ToString("N0");
            historyOkValueLabel.Text = lastHistoryQueryResult.OkCount.ToString("N0");
            historyNgValueLabel.Text = lastHistoryQueryResult.NgCount.ToString("N0");
            historyYieldValueLabel.Text = lastHistoryQueryResult.YieldRate.ToString("0.00") + "%";
            historyParameterAverageValueLabel.Text = parameterStatistics.ValueCount > 0
                ? parameterStatistics.Average.ToString("0.###")
                : "--";
            historyOutOfLimitValueLabel.Text = parameterStatistics.ValueCount > 0
                ? parameterStatistics.OutOfLimitCount.ToString("N0")
                : "--";

            RenderHistoryProductionChart(buckets);
            RenderHistoryParameterChart(buckets, parameterName, parameterLimit);
            RenderHistoryDetailGrid(parameterName);
        }

        private void RenderHistoryProductionChart(List<HistoryBucket> buckets)
        {
            historyProductionChart.Series.Clear();
            Series totalSeries = CreateHistorySeries("总数", SeriesChartType.Column, Color.FromArgb(54, 118, 181));
            Series okSeries = CreateHistorySeries("OK", SeriesChartType.Column, Color.FromArgb(45, 151, 91));
            Series ngSeries = CreateHistorySeries("NG", SeriesChartType.Column, Color.FromArgb(198, 57, 58));
            foreach (HistoryBucket bucket in buckets)
            {
                totalSeries.Points.AddXY(bucket.Label, bucket.TotalCount);
                okSeries.Points.AddXY(bucket.Label, bucket.OkCount);
                ngSeries.Points.AddXY(bucket.Label, bucket.NgCount);
            }
            historyProductionChart.Series.Add(totalSeries);
            historyProductionChart.Series.Add(okSeries);
            historyProductionChart.Series.Add(ngSeries);
            historyProductionChart.ChartAreas[0].RecalculateAxesScale();
        }

        private void RenderHistoryParameterChart(
            List<HistoryBucket> buckets,
            string parameterName,
            HistoryParameterLimit parameterLimit)
        {
            historyParameterChart.Series.Clear();
            historyParameterChart.Titles[0].Text = string.IsNullOrWhiteSpace(parameterName)
                ? "PLC参数趋势（请选择参数）"
                : parameterName + " 趋势";
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return;
            }

            Series averageSeries = CreateHistorySeries("平均值", SeriesChartType.Line, Color.FromArgb(54, 118, 181));
            Series minimumSeries = CreateHistorySeries("最小值", SeriesChartType.Line, Color.FromArgb(45, 151, 91));
            Series maximumSeries = CreateHistorySeries("最大值", SeriesChartType.Line, Color.FromArgb(222, 135, 45));
            averageSeries.BorderWidth = 3;
            minimumSeries.BorderWidth = 2;
            maximumSeries.BorderWidth = 2;
            averageSeries.MarkerStyle = MarkerStyle.Circle;

            foreach (HistoryBucket bucket in buckets)
            {
                if (!bucket.HasParameterValue)
                {
                    AddEmptyHistoryPoint(averageSeries, bucket.Label);
                    AddEmptyHistoryPoint(minimumSeries, bucket.Label);
                    AddEmptyHistoryPoint(maximumSeries, bucket.Label);
                    continue;
                }
                averageSeries.Points.AddXY(bucket.Label, bucket.ParameterAverage);
                minimumSeries.Points.AddXY(bucket.Label, bucket.ParameterMinimum);
                maximumSeries.Points.AddXY(bucket.Label, bucket.ParameterMaximum);
            }
            historyParameterChart.Series.Add(averageSeries);
            historyParameterChart.Series.Add(minimumSeries);
            historyParameterChart.Series.Add(maximumSeries);

            if (parameterLimit != null && parameterLimit.HasUpperLimit)
            {
                AddHistoryLimitSeries(historyParameterChart, buckets, "上限", parameterLimit.UpperLimit, Color.Firebrick);
            }
            if (parameterLimit != null && parameterLimit.HasLowerLimit)
            {
                AddHistoryLimitSeries(historyParameterChart, buckets, "下限", parameterLimit.LowerLimit, Color.DarkOrange);
            }
            historyParameterChart.ChartAreas[0].RecalculateAxesScale();
        }

        private static void AddEmptyHistoryPoint(Series series, string label)
        {
            int pointIndex = series.Points.AddXY(label, 0D);
            series.Points[pointIndex].IsEmpty = true;
        }

        private static Series CreateHistorySeries(string name, SeriesChartType chartType, Color color)
        {
            return new Series(name)
            {
                ChartType = chartType,
                Color = color,
                IsValueShownAsLabel = false,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsVisibleInLegend = true
            };
        }

        private static void AddHistoryLimitSeries(
            Chart chart,
            List<HistoryBucket> buckets,
            string name,
            double limitValue,
            Color color)
        {
            Series series = CreateHistorySeries(name, SeriesChartType.Line, color);
            series.BorderWidth = 2;
            series.BorderDashStyle = ChartDashStyle.Dash;
            foreach (HistoryBucket bucket in buckets)
            {
                series.Points.AddXY(bucket.Label, limitValue);
            }
            chart.Series.Add(series);
        }

        private void RenderHistoryDetailGrid(string parameterName)
        {
            historyDataGridView.SuspendLayout();
            historyDataGridView.Rows.Clear();
            List<HistoryRecord> displayRecords = lastHistoryQueryResult.Records
                .OrderByDescending(item => item.CollectTime)
                .Take(5000)
                .ToList();
            for (int index = 0; index < displayRecords.Count; index++)
            {
                HistoryRecord record = displayRecords[index];
                string parameterValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(parameterName))
                {
                    record.Fields.TryGetValue(parameterName, out parameterValue);
                }
                int rowIndex = historyDataGridView.Rows.Add(
                    index + 1,
                    record.CollectTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    record.SourceType,
                    record.Barcode,
                    record.Result,
                    record.Device,
                    parameterValue ?? string.Empty);
                DataGridViewRow row = historyDataGridView.Rows[rowIndex];
                row.Tag = record.SourceFile;
                if (record.Result == "NG")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 224);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(135, 16, 20);
                }
                else if (record.Result == "OK")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(224, 247, 230);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(20, 100, 53);
                }
            }
            historyDataGridView.ClearSelection();
            historyDataGridView.ResumeLayout();
        }

        private Dictionary<string, HistoryParameterLimit> ReadCurrentHistoryParameterLimits()
        {
            Dictionary<string, HistoryParameterLimit> limits = new Dictionary<string, HistoryParameterLimit>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (ResourceHandler.dparamParameters.mesPullOutUploadingUI == null ||
                    ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1 == null)
                {
                    return limits;
                }

                foreach (DataGridViewRow row in ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows)
                {
                    if (row.IsNewRow || row.Cells.Count < 7 || row.Cells[0].Value == null)
                    {
                        continue;
                    }
                    string parameterName = row.Cells[0].Value.ToString().Trim();
                    if (string.IsNullOrWhiteSpace(parameterName))
                    {
                        continue;
                    }

                    HistoryParameterLimit limit = new HistoryParameterLimit();
                    double lowerLimit;
                    double upperLimit;
                    string lowerText = row.Cells[6].Value == null ? string.Empty : row.Cells[6].Value.ToString();
                    string upperText = row.Cells[5].Value == null ? string.Empty : row.Cells[5].Value.ToString();
                    limit.HasLowerLimit = HistoryNumberParser.TryParse(lowerText, out lowerLimit);
                    limit.LowerLimit = lowerLimit;
                    limit.HasUpperLimit = HistoryNumberParser.TryParse(upperText, out upperLimit);
                    limit.UpperLimit = upperLimit;
                    if (limit.HasLowerLimit || limit.HasUpperLimit)
                    {
                        limits[parameterName] = limit;
                    }
                }
            }
            catch (Exception exception)
            {
                outDiary("历史数据读取PLC参数上下限失败：" + exception.Message, "警告");
            }
            return limits;
        }

        private HistoryParameterLimit FindHistoryParameterLimit(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return null;
            }
            HistoryParameterLimit limit;
            if (historyParameterLimits.TryGetValue(parameterName, out limit))
            {
                return limit;
            }
            string normalizedName = parameterName.Split('/')[0].Trim();
            historyParameterLimits.TryGetValue(normalizedName, out limit);
            return limit;
        }

        private void SetHistoryBusyState(bool busy, string status)
        {
            historyProgressBar.Visible = busy;
            historyQueryButton.Enabled = !busy;
            historyResetButton.Enabled = !busy;
            historyExportButton.Enabled = !busy && lastHistoryQueryResult != null && lastHistoryQueryResult.TotalCount > 0;
            historyStatusLabel.Text = status;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void HistoryPeriodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyHistoryPeriodSelection();
        }

        private void HistoryYearOrQuarter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GetSelectedHistoryPeriodKind() == HistoryPeriodKind.Quarter ||
                GetSelectedHistoryPeriodKind() == HistoryPeriodKind.Year)
            {
                ApplyHistoryPeriodSelection();
            }
        }

        private void HistoryParameterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lastHistoryQueryResult != null)
            {
                RenderHistoryQueryResult();
            }
        }

        private void HistoryBarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                BeginHistoryQuery();
            }
        }

        private void HistoryResetButton_Click(object sender, EventArgs e)
        {
            historyPeriodComboBox.SelectedIndex = 0;
            historySourceComboBox.SelectedIndex = 0;
            historyBarcodeTextBox.Clear();
            ApplyHistoryPeriodSelection();
            BeginHistoryQuery();
        }

        private void HistoryTabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == historyTabPage && !historyStartupLoadStarted)
            {
                InitializeHistoryDataAfterLoad();
            }
        }

        private void HistoryDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            string sourceFile = historyDataGridView.Rows[e.RowIndex].Tag as string;
            if (string.IsNullOrWhiteSpace(sourceFile) || !File.Exists(sourceFile))
            {
                return;
            }
            try
            {
                Process.Start("explorer.exe", "/select,\"" + sourceFile + "\"");
            }
            catch (Exception exception)
            {
                MessageBox.Show("无法定位原始CSV：\r\n" + exception.Message, "历史数据");
            }
        }

        private void HistoryOpenFolderButton_Click(object sender, EventArgs e)
        {
            if (historyRepository == null)
            {
                return;
            }
            try
            {
                Directory.CreateDirectory(historyRepository.ReportDirectory);
                Process.Start("explorer.exe", historyRepository.ReportDirectory);
            }
            catch (Exception exception)
            {
                MessageBox.Show("无法打开报表目录：\r\n" + exception.Message, "历史数据");
            }
        }

        private async void HistoryExportButton_Click(object sender, EventArgs e)
        {
            if (lastHistoryQueryResult == null || lastHistoryQueryResult.TotalCount == 0)
            {
                MessageBox.Show("当前没有可导出的历史数据。", "历史数据");
                return;
            }

            Directory.CreateDirectory(historyRepository.ReportDirectory);
            string defaultFileName = string.Format(
                "PLC采集统计_{0:yyyyMMdd}_{1:yyyyMMdd}_{2:HHmmss}.xlsx",
                lastHistoryQueryResult.Filter.StartTime,
                lastHistoryQueryResult.Filter.EndTime,
                DateTime.Now);
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "导出历史数据Excel报表";
                dialog.Filter = "Excel工作簿 (*.xlsx)|*.xlsx";
                dialog.InitialDirectory = historyRepository.ReportDirectory;
                dialog.FileName = defaultFileName;
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    string parameterName = GetSelectedHistoryParameter();
                    HistoryParameterLimit limit = FindHistoryParameterLimit(parameterName);
                    string exportFileName = dialog.FileName;
                    HistoryQueryResult exportResult = lastHistoryQueryResult;
                    SetHistoryBusyState(true, "正在生成Excel报表，请稍候...");
                    await Task.Run(() => HistoryExcelExporter.Export(
                        exportFileName,
                        exportResult,
                        parameterName,
                        limit));
                    historyStatusLabel.Text = "Excel报表已生成：" + dialog.FileName;
                    DialogResult openResult = MessageBox.Show(
                        "Excel报表生成成功！\r\n\r\n是否立即打开？",
                        "历史数据",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);
                    if (openResult == DialogResult.Yes)
                    {
                        Process.Start(dialog.FileName);
                    }
                }
                catch (Exception exception)
                {
                    outDiary("历史数据Excel导出失败：" + exception, "错误");
                    MessageBox.Show(
                        "Excel报表生成失败：\r\n" + exception.Message,
                        "历史数据",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    SetHistoryBusyState(false, historyStatusLabel.Text);
                }
            }
        }
    }
}
