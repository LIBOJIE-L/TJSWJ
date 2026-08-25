namespace BJTSurrenSystem.History
{
    partial class CpkAnalysisControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel filterLayout;
        private System.Windows.Forms.ComboBox periodComboBox;
        private System.Windows.Forms.DateTimePicker startDatePicker;
        private System.Windows.Forms.DateTimePicker endDatePicker;
        private System.Windows.Forms.ComboBox sourceComboBox;
        private System.Windows.Forms.ComboBox parameterComboBox;
        private System.Windows.Forms.TextBox barcodeTextBox;
        private System.Windows.Forms.Button queryButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button specificationButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Label specificationInfoLabel;
        private System.Windows.Forms.TableLayoutPanel summaryLayout;
        private System.Windows.Forms.Label sampleCountValueLabel;
        private System.Windows.Forms.Label meanValueLabel;
        private System.Windows.Forms.Label withinSigmaValueLabel;
        private System.Windows.Forms.Label cpValueLabel;
        private System.Windows.Forms.Label cpkValueLabel;
        private System.Windows.Forms.Label ppkValueLabel;
        private System.Windows.Forms.Label outOfSpecValueLabel;
        private System.Windows.Forms.Label conclusionValueLabel;
        private System.Windows.Forms.TableLayoutPanel chartLayout;
        private System.Windows.Forms.DataVisualization.Charting.Chart trendChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart histogramChart;
        private System.Windows.Forms.DataGridView sampleGrid;
        private System.Windows.Forms.TableLayoutPanel statusLayout;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar progressBar;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (queryCancellation != null)
                {
                    queryCancellation.Cancel();
                    queryCancellation.Dispose();
                    queryCancellation = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea trendArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend trendLegend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title trendTitle = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea histogramArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend histogramLegend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title histogramTitle = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.filterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.periodComboBox = new System.Windows.Forms.ComboBox();
            this.startDatePicker = new System.Windows.Forms.DateTimePicker();
            this.endDatePicker = new System.Windows.Forms.DateTimePicker();
            this.sourceComboBox = new System.Windows.Forms.ComboBox();
            this.parameterComboBox = new System.Windows.Forms.ComboBox();
            this.barcodeTextBox = new System.Windows.Forms.TextBox();
            this.queryButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.specificationButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.specificationInfoLabel = new System.Windows.Forms.Label();
            this.summaryLayout = new System.Windows.Forms.TableLayoutPanel();
            this.sampleCountValueLabel = new System.Windows.Forms.Label();
            this.meanValueLabel = new System.Windows.Forms.Label();
            this.withinSigmaValueLabel = new System.Windows.Forms.Label();
            this.cpValueLabel = new System.Windows.Forms.Label();
            this.cpkValueLabel = new System.Windows.Forms.Label();
            this.ppkValueLabel = new System.Windows.Forms.Label();
            this.outOfSpecValueLabel = new System.Windows.Forms.Label();
            this.conclusionValueLabel = new System.Windows.Forms.Label();
            this.chartLayout = new System.Windows.Forms.TableLayoutPanel();
            this.trendChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.histogramChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.sampleGrid = new System.Windows.Forms.DataGridView();
            this.statusLayout = new System.Windows.Forms.TableLayoutPanel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.rootLayout.SuspendLayout();
            this.filterLayout.SuspendLayout();
            this.summaryLayout.SuspendLayout();
            this.chartLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trendChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.histogramChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleGrid)).BeginInit();
            this.statusLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.filterLayout, 0, 0);
            this.rootLayout.Controls.Add(this.specificationInfoLabel, 0, 1);
            this.rootLayout.Controls.Add(this.summaryLayout, 0, 2);
            this.rootLayout.Controls.Add(this.chartLayout, 0, 3);
            this.rootLayout.Controls.Add(this.sampleGrid, 0, 4);
            this.rootLayout.Controls.Add(this.statusLayout, 0, 5);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(8);
            this.rootLayout.RowCount = 6;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.rootLayout.Size = new System.Drawing.Size(1420, 850);
            this.rootLayout.TabIndex = 0;
            // 
            // filterLayout
            // 
            this.filterLayout.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.filterLayout.ColumnCount = 10;
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.filterLayout.Controls.Add(CreateHeaderLabel("周期"), 0, 0);
            this.filterLayout.Controls.Add(CreateHeaderLabel("开始日期"), 1, 0);
            this.filterLayout.Controls.Add(CreateHeaderLabel("结束日期"), 2, 0);
            this.filterLayout.Controls.Add(CreateHeaderLabel("数据类型"), 3, 0);
            this.filterLayout.Controls.Add(CreateHeaderLabel("涂胶数值参数"), 4, 0);
            this.filterLayout.Controls.Add(CreateHeaderLabel("条码/模组码"), 5, 0);
            this.filterLayout.Controls.Add(this.periodComboBox, 0, 1);
            this.filterLayout.Controls.Add(this.startDatePicker, 1, 1);
            this.filterLayout.Controls.Add(this.endDatePicker, 2, 1);
            this.filterLayout.Controls.Add(this.sourceComboBox, 3, 1);
            this.filterLayout.Controls.Add(this.parameterComboBox, 4, 1);
            this.filterLayout.Controls.Add(this.barcodeTextBox, 5, 1);
            this.filterLayout.Controls.Add(this.queryButton, 6, 1);
            this.filterLayout.Controls.Add(this.resetButton, 7, 1);
            this.filterLayout.Controls.Add(this.specificationButton, 8, 1);
            this.filterLayout.Controls.Add(this.exportButton, 9, 1);
            this.filterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterLayout.Location = new System.Drawing.Point(11, 11);
            this.filterLayout.Name = "filterLayout";
            this.filterLayout.Padding = new System.Windows.Forms.Padding(8, 4, 8, 5);
            this.filterLayout.RowCount = 2;
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.Size = new System.Drawing.Size(1398, 66);
            this.filterLayout.TabIndex = 0;
            // 
            // periodComboBox
            // 
            this.periodComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.periodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.periodComboBox.FormattingEnabled = true;
            this.periodComboBox.Location = new System.Drawing.Point(11, 30);
            this.periodComboBox.Name = "periodComboBox";
            this.periodComboBox.Size = new System.Drawing.Size(99, 25);
            this.periodComboBox.TabIndex = 0;
            this.periodComboBox.SelectedIndexChanged += new System.EventHandler(this.PeriodComboBox_SelectedIndexChanged);
            // 
            // startDatePicker
            // 
            this.startDatePicker.CustomFormat = "yyyy-MM-dd";
            this.startDatePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDatePicker.Location = new System.Drawing.Point(116, 30);
            this.startDatePicker.Name = "startDatePicker";
            this.startDatePicker.Size = new System.Drawing.Size(120, 23);
            this.startDatePicker.TabIndex = 1;
            // 
            // endDatePicker
            // 
            this.endDatePicker.CustomFormat = "yyyy-MM-dd";
            this.endDatePicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.endDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDatePicker.Location = new System.Drawing.Point(242, 30);
            this.endDatePicker.Name = "endDatePicker";
            this.endDatePicker.Size = new System.Drawing.Size(120, 23);
            this.endDatePicker.TabIndex = 2;
            // 
            // sourceComboBox
            // 
            this.sourceComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceComboBox.FormattingEnabled = true;
            this.sourceComboBox.Location = new System.Drawing.Point(368, 30);
            this.sourceComboBox.Name = "sourceComboBox";
            this.sourceComboBox.Size = new System.Drawing.Size(139, 25);
            this.sourceComboBox.TabIndex = 3;
            // 
            // parameterComboBox
            // 
            this.parameterComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parameterComboBox.FormattingEnabled = true;
            this.parameterComboBox.Location = new System.Drawing.Point(513, 30);
            this.parameterComboBox.Name = "parameterComboBox";
            this.parameterComboBox.Size = new System.Drawing.Size(245, 25);
            this.parameterComboBox.TabIndex = 4;
            this.parameterComboBox.SelectedIndexChanged += new System.EventHandler(this.ParameterComboBox_SelectedIndexChanged);
            // 
            // barcodeTextBox
            // 
            this.barcodeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.barcodeTextBox.Location = new System.Drawing.Point(764, 30);
            this.barcodeTextBox.Name = "barcodeTextBox";
            this.barcodeTextBox.Size = new System.Drawing.Size(162, 23);
            this.barcodeTextBox.TabIndex = 5;
            this.barcodeTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BarcodeTextBox_KeyDown);
            // 
            // queryButton
            // 
            this.queryButton.BackColor = System.Drawing.Color.FromArgb(30, 112, 191);
            this.queryButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.queryButton.ForeColor = System.Drawing.Color.White;
            this.queryButton.Location = new System.Drawing.Point(932, 30);
            this.queryButton.Name = "queryButton";
            this.queryButton.Size = new System.Drawing.Size(78, 28);
            this.queryButton.TabIndex = 6;
            this.queryButton.Text = "查询";
            this.queryButton.UseVisualStyleBackColor = false;
            this.queryButton.Click += new System.EventHandler(this.QueryButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resetButton.Location = new System.Drawing.Point(1016, 30);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(78, 28);
            this.resetButton.TabIndex = 7;
            this.resetButton.Text = "重置";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // specificationButton
            // 
            this.specificationButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specificationButton.Location = new System.Drawing.Point(1100, 30);
            this.specificationButton.Name = "specificationButton";
            this.specificationButton.Size = new System.Drawing.Size(96, 28);
            this.specificationButton.TabIndex = 8;
            this.specificationButton.Text = "规格配置";
            this.specificationButton.UseVisualStyleBackColor = true;
            this.specificationButton.Click += new System.EventHandler(this.SpecificationButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.BackColor = System.Drawing.Color.FromArgb(31, 132, 79);
            this.exportButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportButton.Enabled = false;
            this.exportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exportButton.ForeColor = System.Drawing.Color.White;
            this.exportButton.Location = new System.Drawing.Point(1202, 30);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(185, 28);
            this.exportButton.TabIndex = 9;
            this.exportButton.Text = "导出Excel";
            this.exportButton.UseVisualStyleBackColor = false;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // specificationInfoLabel
            // 
            this.specificationInfoLabel.BackColor = System.Drawing.Color.FromArgb(233, 242, 250);
            this.specificationInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specificationInfoLabel.ForeColor = System.Drawing.Color.FromArgb(35, 70, 100);
            this.specificationInfoLabel.Location = new System.Drawing.Point(11, 80);
            this.specificationInfoLabel.Name = "specificationInfoLabel";
            this.specificationInfoLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.specificationInfoLabel.Size = new System.Drawing.Size(1398, 35);
            this.specificationInfoLabel.TabIndex = 1;
            this.specificationInfoLabel.Text = "请选择涂胶数值参数；规格未配置时可从MES上下限读取。";
            this.specificationInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // summaryLayout
            // 
            this.summaryLayout.ColumnCount = 8;
            for (int index = 0; index < 8; index++)
            {
                this.summaryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            }
            this.summaryLayout.Controls.Add(CreateSummaryCard("样本数", this.sampleCountValueLabel, System.Drawing.Color.FromArgb(30, 112, 191)), 0, 0);
            this.summaryLayout.Controls.Add(CreateSummaryCard("平均值", this.meanValueLabel, System.Drawing.Color.FromArgb(205, 112, 23)), 1, 0);
            this.summaryLayout.Controls.Add(CreateSummaryCard("组内σ", this.withinSigmaValueLabel, System.Drawing.Color.FromArgb(75, 112, 165)), 2, 0);
            this.summaryLayout.Controls.Add(CreateSummaryCard("Cp", this.cpValueLabel, System.Drawing.Color.FromArgb(79, 129, 189)), 3, 0);
            this.summaryLayout.Controls.Add(CreateSummaryCard("Cpk", this.cpkValueLabel, System.Drawing.Color.FromArgb(124, 78, 153)), 4, 0);
            this.summaryLayout.Controls.Add(CreateSummaryCard("Ppk", this.ppkValueLabel, System.Drawing.Color.FromArgb(84, 130, 53)), 5, 0);
            this.summaryLayout.Controls.Add(CreateSummaryCard("超限数", this.outOfSpecValueLabel, System.Drawing.Color.FromArgb(190, 36, 40)), 6, 0);
            this.summaryLayout.Controls.Add(CreateSummaryCard("能力判定", this.conclusionValueLabel, System.Drawing.Color.FromArgb(100, 100, 100)), 7, 0);
            this.summaryLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryLayout.Location = new System.Drawing.Point(11, 118);
            this.summaryLayout.Name = "summaryLayout";
            this.summaryLayout.Padding = new System.Windows.Forms.Padding(0, 5, 0, 7);
            this.summaryLayout.RowCount = 1;
            this.summaryLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.summaryLayout.Size = new System.Drawing.Size(1398, 96);
            this.summaryLayout.TabIndex = 2;
            // 
            // chartLayout
            // 
            this.chartLayout.ColumnCount = 2;
            this.chartLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.chartLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.chartLayout.Controls.Add(this.trendChart, 0, 0);
            this.chartLayout.Controls.Add(this.histogramChart, 1, 0);
            this.chartLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartLayout.Location = new System.Drawing.Point(11, 220);
            this.chartLayout.Name = "chartLayout";
            this.chartLayout.RowCount = 1;
            this.chartLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.chartLayout.Size = new System.Drawing.Size(1398, 280);
            this.chartLayout.TabIndex = 3;
            // 
            // trendChart
            // 
            trendArea.AxisX.MajorGrid.Enabled = false;
            trendArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            trendArea.Name = "TrendArea";
            this.trendChart.ChartAreas.Add(trendArea);
            this.trendChart.Dock = System.Windows.Forms.DockStyle.Fill;
            trendLegend.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            trendLegend.Name = "TrendLegend";
            this.trendChart.Legends.Add(trendLegend);
            this.trendChart.Location = new System.Drawing.Point(3, 3);
            this.trendChart.Name = "trendChart";
            this.trendChart.Size = new System.Drawing.Size(693, 274);
            this.trendChart.TabIndex = 0;
            trendTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            trendTitle.Name = "TrendTitle";
            trendTitle.Text = "涂胶参数趋势";
            this.trendChart.Titles.Add(trendTitle);
            // 
            // histogramChart
            // 
            histogramArea.AxisX.MajorGrid.Enabled = false;
            histogramArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            histogramArea.Name = "HistogramArea";
            this.histogramChart.ChartAreas.Add(histogramArea);
            this.histogramChart.Dock = System.Windows.Forms.DockStyle.Fill;
            histogramLegend.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            histogramLegend.Name = "HistogramLegend";
            this.histogramChart.Legends.Add(histogramLegend);
            this.histogramChart.Location = new System.Drawing.Point(702, 3);
            this.histogramChart.Name = "histogramChart";
            this.histogramChart.Size = new System.Drawing.Size(693, 274);
            this.histogramChart.TabIndex = 1;
            histogramTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            histogramTitle.Name = "HistogramTitle";
            histogramTitle.Text = "涂胶参数分布";
            this.histogramChart.Titles.Add(histogramTitle);
            // 
            // sampleGrid
            // 
            this.sampleGrid.AllowUserToAddRows = false;
            this.sampleGrid.AllowUserToDeleteRows = false;
            this.sampleGrid.AllowUserToResizeRows = false;
            this.sampleGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.sampleGrid.BackgroundColor = System.Drawing.Color.White;
            this.sampleGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sampleGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sampleGrid.Location = new System.Drawing.Point(11, 506);
            this.sampleGrid.MultiSelect = false;
            this.sampleGrid.Name = "sampleGrid";
            this.sampleGrid.ReadOnly = true;
            this.sampleGrid.RowHeadersVisible = false;
            this.sampleGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sampleGrid.Size = new System.Drawing.Size(1398, 276);
            this.sampleGrid.TabIndex = 4;
            this.sampleGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SampleGrid_CellDoubleClick);
            // 
            // statusLayout
            // 
            this.statusLayout.ColumnCount = 2;
            this.statusLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.statusLayout.Controls.Add(this.statusLabel, 0, 0);
            this.statusLayout.Controls.Add(this.progressBar, 1, 0);
            this.statusLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLayout.Location = new System.Drawing.Point(11, 788);
            this.statusLayout.Name = "statusLayout";
            this.statusLayout.RowCount = 1;
            this.statusLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusLayout.Size = new System.Drawing.Size(1398, 24);
            this.statusLayout.TabIndex = 5;
            // 
            // statusLabel
            // 
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(205, 112, 23);
            this.statusLabel.Location = new System.Drawing.Point(3, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(1172, 24);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "准备就绪";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(1181, 3);
            this.progressBar.MarqueeAnimationSpeed = 25;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(214, 18);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 1;
            this.progressBar.Visible = false;
            // 
            // CpkAnalysisControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.Name = "CpkAnalysisControl";
            this.Size = new System.Drawing.Size(1420, 850);
            this.rootLayout.ResumeLayout(false);
            this.filterLayout.ResumeLayout(false);
            this.filterLayout.PerformLayout();
            this.summaryLayout.ResumeLayout(false);
            this.chartLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trendChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.histogramChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sampleGrid)).EndInit();
            this.statusLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static System.Windows.Forms.Label CreateHeaderLabel(string text)
        {
            return new System.Windows.Forms.Label
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ForeColor = System.Drawing.Color.FromArgb(70, 70, 70),
                Text = text,
                TextAlign = System.Drawing.ContentAlignment.BottomLeft
            };
        }

        private static System.Windows.Forms.Panel CreateSummaryCard(
            string title,
            System.Windows.Forms.Label valueLabel,
            System.Drawing.Color accentColor)
        {
            System.Windows.Forms.Panel card = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = new System.Windows.Forms.Padding(4, 4, 4, 6),
                BackColor = System.Drawing.Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            };
            System.Windows.Forms.TableLayoutPanel cardLayout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = System.Windows.Forms.Padding.Empty,
                Padding = System.Windows.Forms.Padding.Empty
            };
            cardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            cardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            cardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            System.Windows.Forms.Panel accent = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Margin = System.Windows.Forms.Padding.Empty,
                BackColor = accentColor
            };
            System.Windows.Forms.TableLayoutPanel contentLayout = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = System.Windows.Forms.Padding.Empty,
                Padding = new System.Windows.Forms.Padding(8, 4, 6, 8)
            };
            contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            System.Windows.Forms.Label titleLabel = new System.Windows.Forms.Label
            {
                Text = title,
                Dock = System.Windows.Forms.DockStyle.Fill,
                AutoEllipsis = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                ForeColor = System.Drawing.Color.FromArgb(95, 95, 95),
            };
            valueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            valueLabel.Font = new System.Drawing.Font(
                System.Drawing.SystemFonts.MessageBoxFont.FontFamily,
                13F,
                System.Drawing.FontStyle.Bold);
            valueLabel.ForeColor = accentColor;
            valueLabel.Text = "--";
            valueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            contentLayout.Controls.Add(titleLabel, 0, 0);
            contentLayout.Controls.Add(valueLabel, 0, 1);
            cardLayout.Controls.Add(accent, 0, 0);
            cardLayout.Controls.Add(contentLayout, 1, 0);
            card.Controls.Add(cardLayout);
            return card;
        }
    }
}
