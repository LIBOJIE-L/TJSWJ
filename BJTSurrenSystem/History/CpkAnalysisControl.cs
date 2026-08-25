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

namespace BJTSurrenSystem.History
{
    internal partial class CpkAnalysisControl : UserControl
    {
        private const string NoParameterText = "（请先查询并选择参数）";
        private const int MaximumDisplayedRows = 5000;
        private const int MaximumChartPoints = 500;

        private HistoryDataRepository historyRepository;
        private CpkSpecificationRepository specificationRepository;
        private List<CpkSpecification> configuredSpecifications = new List<CpkSpecification>();
        private List<CpkSpecification> defaultSpecifications = new List<CpkSpecification>();
        private HistoryQueryResult lastQueryResult;
        private CpkCalculationResult lastCalculation;
        private CancellationTokenSource queryCancellation;
        private string reportDirectory;
        private bool configured;
        private bool startupQueryStarted;
        private bool changingParameterItems;

        public CpkAnalysisControl()
        {
            InitializeComponent();
            InitializeLookups();
            InitializeSampleGrid();
            ClearResultDisplay();
        }

        public void Configure(
            string programLogPath,
            string specificationFilePath,
            IEnumerable<CpkSpecification> mesDefaults)
        {
            string dataRoot = string.IsNullOrWhiteSpace(programLogPath)
                ? AppDomain.CurrentDomain.BaseDirectory
                : programLogPath;
            historyRepository = new HistoryDataRepository(dataRoot);
            specificationRepository = new CpkSpecificationRepository(specificationFilePath);
            reportDirectory = Path.Combine(Path.GetFullPath(dataRoot), "CPK报表");
            defaultSpecifications = (mesDefaults ?? Enumerable.Empty<CpkSpecification>())
                .Where(item => item != null && !string.IsNullOrWhiteSpace(item.ParameterName))
                .Select(item => item.Clone())
                .ToList();
            try
            {
                configuredSpecifications = specificationRepository.Load();
                statusLabel.Text = configuredSpecifications.Count > 0
                    ? "已加载涂胶CPK规格配置：" + specificationRepository.FilePath
                    : "尚未建立独立CPK规格配置，查询时将优先使用MES参数上下限。";
            }
            catch (Exception exception)
            {
                configuredSpecifications = new List<CpkSpecification>();
                statusLabel.Text = "CPK规格配置读取失败：" + exception.Message;
            }
            configured = true;
            PopulateKnownParameters();
            if (Visible && !startupQueryStarted)
            {
                startupQueryStarted = true;
                BeginQuery();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible && configured && !startupQueryStarted && !DesignMode)
            {
                startupQueryStarted = true;
                BeginQuery();
            }
        }

        private void InitializeLookups()
        {
            periodComboBox.Items.AddRange(new object[]
            {
                "近7天", "近3个月", "本季度", "本年度", "自定义"
            });
            periodComboBox.SelectedIndex = 0;
            sourceComboBox.Items.AddRange(new object[]
            {
                "全部采集数据", "出站数据", "停机出站数据", "侧板出站数据", "进站数据"
            });
            sourceComboBox.SelectedIndex = 0;
            parameterComboBox.Items.Add(NoParameterText);
            parameterComboBox.SelectedIndex = 0;
            ApplyPeriodSelection();
        }

        private void InitializeSampleGrid()
        {
            sampleGrid.Columns.Clear();
            sampleGrid.Columns.Add("sequenceColumn", "序号");
            sampleGrid.Columns.Add("timeColumn", "采集时间");
            sampleGrid.Columns.Add("barcodeColumn", "条码/模组码");
            sampleGrid.Columns.Add("sourceColumn", "数据类型");
            sampleGrid.Columns.Add("valueColumn", "测量值");
            sampleGrid.Columns.Add("specificationResultColumn", "规格判定");
            sampleGrid.Columns.Add("sourceResultColumn", "原始结果");
            sampleGrid.Columns[0].FillWeight = 35F;
            sampleGrid.Columns[1].FillWeight = 95F;
            sampleGrid.Columns[2].FillWeight = 145F;
            sampleGrid.Columns[3].FillWeight = 75F;
            sampleGrid.Columns[4].FillWeight = 70F;
            sampleGrid.Columns[5].FillWeight = 60F;
            sampleGrid.Columns[6].FillWeight = 60F;
        }

        private void PopulateKnownParameters()
        {
            string selected = GetSelectedParameter();
            List<string> names = CpkSpecificationRepository.Merge(
                    configuredSpecifications,
                    defaultSpecifications)
                .Where(item => item.Enabled)
                .Select(item => item.ParameterName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(item => item.IndexOf("胶", StringComparison.OrdinalIgnoreCase) >= 0)
                .ThenBy(item => item, StringComparer.CurrentCulture)
                .ToList();
            SetParameterItems(names, selected);
        }

        private void SetParameterItems(IEnumerable<string> names, string preferredParameter)
        {
            changingParameterItems = true;
            try
            {
                parameterComboBox.BeginUpdate();
                parameterComboBox.Items.Clear();
                parameterComboBox.Items.Add(NoParameterText);
                foreach (string name in names ?? Enumerable.Empty<string>())
                {
                    if (!string.IsNullOrWhiteSpace(name) && !parameterComboBox.Items.Contains(name))
                    {
                        parameterComboBox.Items.Add(name);
                    }
                }

                int preferredIndex = -1;
                if (!string.IsNullOrWhiteSpace(preferredParameter))
                {
                    preferredIndex = FindParameterItemIndex(preferredParameter);
                }
                if (preferredIndex < 0)
                {
                    preferredIndex = FindFirstConfiguredParameterIndex();
                }
                parameterComboBox.SelectedIndex = preferredIndex >= 0
                    ? preferredIndex
                    : parameterComboBox.Items.Count > 1 ? 1 : 0;
            }
            finally
            {
                parameterComboBox.EndUpdate();
                changingParameterItems = false;
            }
        }

        private int FindParameterItemIndex(string parameterName)
        {
            for (int index = 1; index < parameterComboBox.Items.Count; index++)
            {
                if (string.Equals(
                    parameterComboBox.Items[index].ToString(),
                    parameterName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }
            return -1;
        }

        private int FindFirstConfiguredParameterIndex()
        {
            List<CpkSpecification> effective = CpkSpecificationRepository.Merge(
                configuredSpecifications,
                defaultSpecifications);
            foreach (CpkSpecification specification in effective.Where(item => item.Enabled))
            {
                int index = FindParameterItemIndex(specification.ParameterName);
                if (index >= 0)
                {
                    return index;
                }
            }
            return -1;
        }

        private async void BeginQuery()
        {
            if (!configured || historyRepository == null || IsDisposed)
            {
                return;
            }

            if (queryCancellation != null)
            {
                queryCancellation.Cancel();
                queryCancellation.Dispose();
            }
            queryCancellation = new CancellationTokenSource();
            CancellationToken cancellationToken = queryCancellation.Token;
            HistoryFilter filter = new HistoryFilter
            {
                StartTime = startDatePicker.Value.Date,
                EndTime = endDatePicker.Value.Date.AddDays(1).AddTicks(-1),
                SourceType = sourceComboBox.SelectedItem == null
                    ? "全部采集数据"
                    : sourceComboBox.SelectedItem.ToString(),
                Barcode = barcodeTextBox.Text.Trim(),
                PeriodKind = GetPeriodKind()
            };
            if (filter.EndTime < filter.StartTime)
            {
                MessageBox.Show("结束日期不能早于开始日期。", "CPK查询", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetBusy(true, "正在读取历史索引并提取涂胶参数...");
            string selectedParameter = GetSelectedParameter();
            Progress<string> progress = new Progress<string>(message =>
            {
                if (!IsDisposed)
                {
                    statusLabel.Text = message;
                }
            });
            try
            {
                HistoryQueryResult queryResult = await Task.Run(
                    () => historyRepository.Query(filter, progress, cancellationToken),
                    cancellationToken);
                if (IsDisposed || cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                lastQueryResult = queryResult;
                List<string> parameters = queryResult.NumericParameters
                    .OrderByDescending(item => item.IndexOf("胶", StringComparison.OrdinalIgnoreCase) >= 0)
                    .ThenBy(item => item, StringComparer.CurrentCulture)
                    .ToList();
                SetParameterItems(parameters, selectedParameter);
                RenderCalculation();
            }
            catch (OperationCanceledException)
            {
                statusLabel.Text = "查询已取消。";
            }
            catch (Exception exception)
            {
                lastQueryResult = null;
                lastCalculation = null;
                ClearResultDisplay();
                MessageBox.Show("CPK数据查询失败：" + exception.Message, "CPK查询", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "CPK数据查询失败：" + exception.Message;
            }
            finally
            {
                if (!IsDisposed)
                {
                    SetBusy(false, statusLabel.Text);
                }
            }
        }

        private void RenderCalculation()
        {
            if (lastQueryResult == null)
            {
                ClearResultDisplay();
                return;
            }
            string parameterName = GetSelectedParameter();
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                lastCalculation = null;
                ClearResultDisplay();
                statusLabel.Text = lastQueryResult.TotalCount == 0
                    ? "当前查询条件没有历史记录。"
                    : "当前记录中没有可用于CPK计算的单值数值参数。";
                return;
            }

            CpkSpecification specification = FindEffectiveSpecification(parameterName);
            lastCalculation = CpkCalculator.Calculate(
                lastQueryResult.Records,
                parameterName,
                specification);
            RenderSpecificationInfo(parameterName, specification);
            RenderSummary(lastCalculation);
            RenderTrendChart(lastCalculation);
            RenderHistogram(lastCalculation);
            RenderSampleGrid(lastCalculation, specification);
            exportButton.Enabled = lastCalculation.SampleCount > 0;

            string skippedText = lastCalculation.SkippedValueCount > 0
                ? string.Format("，另有 {0} 个空值、数组值或非数值被跳过", lastCalculation.SkippedValueCount)
                : string.Empty;
            string truncatedText = lastCalculation.SampleCount > MaximumDisplayedRows
                ? string.Format("；明细仅显示最新 {0} 条", MaximumDisplayedRows)
                : string.Empty;
            statusLabel.Text = string.Format(
                "计算完成：参数 {0}，有效样本 {1} 个{2}{3}。",
                parameterName,
                lastCalculation.SampleCount,
                skippedText,
                truncatedText);
        }

        private CpkSpecification FindEffectiveSpecification(string parameterName)
        {
            CpkSpecification configuredMatch = FindSpecification(configuredSpecifications, parameterName);
            if (configuredMatch != null)
            {
                return configuredMatch.Enabled ? configuredMatch.Clone() : null;
            }
            CpkSpecification defaultMatch = FindSpecification(defaultSpecifications, parameterName);
            return defaultMatch != null && defaultMatch.Enabled ? defaultMatch.Clone() : null;
        }

        private static CpkSpecification FindSpecification(
            IEnumerable<CpkSpecification> specifications,
            string parameterName)
        {
            string normalizedName = NormalizeParameterName(parameterName);
            return (specifications ?? Enumerable.Empty<CpkSpecification>()).FirstOrDefault(item =>
                item != null &&
                (string.Equals(item.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(NormalizeParameterName(item.ParameterName), normalizedName, StringComparison.OrdinalIgnoreCase)));
        }

        private void RenderSpecificationInfo(string parameterName, CpkSpecification specification)
        {
            if (specification == null)
            {
                specificationInfoLabel.Text = parameterName + "：未配置规格，平均值和分布仍可查看，但不能计算能力指数。";
                return;
            }
            string source = FindSpecification(configuredSpecifications, parameterName) != null
                ? "独立CPK配置"
                : "MES上下限默认值";
            specificationInfoLabel.Text = string.Format(
                "{0}  |  LSL：{1}  目标值：{2}  USL：{3}  单位：{4}  |  {5}，{6}，最小样本数 {7}",
                parameterName,
                FormatNullable(specification.LowerLimit),
                FormatNullable(specification.Target),
                FormatNullable(specification.UpperLimit),
                string.IsNullOrWhiteSpace(specification.Unit) ? "--" : specification.Unit,
                source,
                CpkSpecificationRepository.FormatMode(specification.CalculationMode),
                specification.MinimumSampleSize);
        }

        private void RenderSummary(CpkCalculationResult result)
        {
            sampleCountValueLabel.Text = result.SampleCount.ToString("N0");
            meanValueLabel.Text = result.SampleCount > 0 ? result.Mean.ToString("0.#####") : "--";
            withinSigmaValueLabel.Text = FormatNullable(result.WithinStandardDeviation);
            cpValueLabel.Text = FormatNullable(result.Cp);
            cpkValueLabel.Text = FormatNullable(result.Cpk);
            ppkValueLabel.Text = FormatNullable(result.Ppk);
            outOfSpecValueLabel.Text = result.OutOfSpecificationCount.ToString("N0");
            conclusionValueLabel.Text = result.Conclusion ?? "--";

            Color conclusionColor = Color.FromArgb(100, 100, 100);
            if (result.Cpk.HasValue && result.MeetsMinimumSampleSize)
            {
                conclusionColor = result.Cpk.Value >= 1.33D
                    ? Color.FromArgb(28, 135, 84)
                    : result.Cpk.Value >= 1D
                        ? Color.FromArgb(205, 112, 23)
                        : Color.FromArgb(190, 36, 40);
            }
            conclusionValueLabel.ForeColor = conclusionColor;
            cpkValueLabel.ForeColor = conclusionColor;
        }

        private void RenderTrendChart(CpkCalculationResult result)
        {
            trendChart.Series.Clear();
            trendChart.Titles[0].Text = string.IsNullOrWhiteSpace(result.ParameterName)
                ? "涂胶参数趋势"
                : result.ParameterName + " 趋势";
            if (result.SampleCount == 0)
            {
                return;
            }

            Series valueSeries = new Series("测量值")
            {
                ChartType = result.SampleCount > 250 ? SeriesChartType.FastLine : SeriesChartType.Line,
                Color = Color.FromArgb(48, 116, 181),
                BorderWidth = 2,
                XValueType = ChartValueType.Int32,
                YValueType = ChartValueType.Double
            };
            if (result.SampleCount <= 250)
            {
                valueSeries.MarkerStyle = MarkerStyle.Circle;
                valueSeries.MarkerSize = 4;
            }
            foreach (CpkSample sample in SelectChartSamples(result.Samples))
            {
                int pointIndex = valueSeries.Points.AddXY(sample.Sequence, sample.Value);
                valueSeries.Points[pointIndex].ToolTip = string.Format(
                    "#{0}  {1:yyyy-MM-dd HH:mm:ss}\n{2}: {3:0.#####}",
                    sample.Sequence,
                    sample.CollectTime,
                    result.ParameterName,
                    sample.Value);
            }
            trendChart.Series.Add(valueSeries);
            AddHorizontalSeries(trendChart, "平均值", result.Mean, result.SampleCount, Color.FromArgb(90, 90, 90), ChartDashStyle.Dash);
            if (result.Specification != null)
            {
                if (result.Specification.LowerLimit.HasValue)
                {
                    AddHorizontalSeries(trendChart, "LSL", result.Specification.LowerLimit.Value, result.SampleCount, Color.DarkOrange, ChartDashStyle.Dash);
                }
                if (result.Specification.Target.HasValue)
                {
                    AddHorizontalSeries(trendChart, "目标", result.Specification.Target.Value, result.SampleCount, Color.SeaGreen, ChartDashStyle.Dot);
                }
                if (result.Specification.UpperLimit.HasValue)
                {
                    AddHorizontalSeries(trendChart, "USL", result.Specification.UpperLimit.Value, result.SampleCount, Color.Firebrick, ChartDashStyle.Dash);
                }
            }
            trendChart.ChartAreas[0].AxisX.Title = "样本序号";
            trendChart.ChartAreas[0].AxisY.Title = string.IsNullOrWhiteSpace(result.Specification == null ? null : result.Specification.Unit)
                ? "测量值"
                : result.Specification.Unit;
            trendChart.ChartAreas[0].RecalculateAxesScale();
        }

        private void RenderHistogram(CpkCalculationResult result)
        {
            histogramChart.Series.Clear();
            ChartArea area = histogramChart.ChartAreas[0];
            area.AxisX.StripLines.Clear();
            area.AxisX.Minimum = double.NaN;
            area.AxisX.Maximum = double.NaN;
            area.AxisX.LabelStyle.Format = "0.###";
            area.AxisX.IsLabelAutoFit = true;
            area.AxisX.LabelAutoFitMinFontSize = 7;
            area.AxisX.LabelAutoFitMaxFontSize = 9;
            histogramChart.Titles[0].Text = string.IsNullOrWhiteSpace(result.ParameterName)
                ? "涂胶参数分布"
                : result.ParameterName + " 分布";
            if (result.SampleCount == 0)
            {
                return;
            }

            int binCount = Math.Max(5, Math.Min(30, (int)Math.Ceiling(Math.Sqrt(result.SampleCount))));
            double minimum = result.Minimum;
            double maximum = result.Maximum;
            double range = maximum - minimum;
            if (range <= 1E-12D)
            {
                range = Math.Max(Math.Abs(minimum) * 0.02D, 1D);
                minimum -= range / 2D;
                maximum += range / 2D;
            }
            double binWidth = (maximum - minimum) / binCount;
            int[] counts = new int[binCount];
            foreach (CpkSample sample in result.Samples)
            {
                int binIndex = (int)((sample.Value - minimum) / binWidth);
                binIndex = Math.Max(0, Math.Min(binCount - 1, binIndex));
                counts[binIndex]++;
            }

            Series histogramSeries = new Series("频数")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(79, 129, 189),
                XValueType = ChartValueType.Double,
                YValueType = ChartValueType.Int32
            };
            histogramSeries["PointWidth"] = "0.9";
            for (int index = 0; index < binCount; index++)
            {
                double center = minimum + (index + 0.5D) * binWidth;
                int pointIndex = histogramSeries.Points.AddXY(center, counts[index]);
                histogramSeries.Points[pointIndex].ToolTip = string.Format(
                    "{0:0.###} ~ {1:0.###}: {2}",
                    minimum + index * binWidth,
                    minimum + (index + 1D) * binWidth,
                    counts[index]);
            }
            histogramChart.Series.Add(histogramSeries);

            double axisMinimum = minimum;
            double axisMaximum = maximum;
            if (result.Specification != null)
            {
                IncludeAxisValue(result.Specification.LowerLimit, ref axisMinimum, ref axisMaximum);
                IncludeAxisValue(result.Specification.Target, ref axisMinimum, ref axisMaximum);
                IncludeAxisValue(result.Specification.UpperLimit, ref axisMinimum, ref axisMaximum);
                AddSpecificationStripLine(histogramChart, area, result.Specification.LowerLimit, "LSL", Color.DarkOrange);
                AddSpecificationStripLine(histogramChart, area, result.Specification.Target, "目标", Color.SeaGreen);
                AddSpecificationStripLine(histogramChart, area, result.Specification.UpperLimit, "USL", Color.Firebrick);
            }

            double axisRange = Math.Max(axisMaximum - axisMinimum, binWidth);
            double axisPadding = Math.Max(axisRange * 0.04D, binWidth * 0.5D);
            area.AxisX.Minimum = axisMinimum - axisPadding;
            area.AxisX.Maximum = axisMaximum + axisPadding;
            area.AxisX.Title = string.IsNullOrWhiteSpace(result.Specification == null ? null : result.Specification.Unit)
                ? "测量值"
                : result.Specification.Unit;
            area.AxisY.Title = "频数";
            area.RecalculateAxesScale();
        }

        private void RenderSampleGrid(CpkCalculationResult result, CpkSpecification specification)
        {
            sampleGrid.SuspendLayout();
            sampleGrid.Rows.Clear();
            List<CpkSample> displaySamples = result.Samples
                .OrderByDescending(item => item.CollectTime)
                .ThenByDescending(item => item.Sequence)
                .Take(MaximumDisplayedRows)
                .ToList();
            string unit = specification == null ? string.Empty : specification.Unit;
            foreach (CpkSample sample in displaySamples)
            {
                string specificationResult = sample.IsWithinSpecification.HasValue
                    ? sample.IsWithinSpecification.Value ? "OK" : "NG"
                    : "--";
                int rowIndex = sampleGrid.Rows.Add(
                    sample.Sequence,
                    sample.CollectTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    sample.Barcode,
                    sample.SourceType,
                    sample.Value.ToString("0.#####") + (string.IsNullOrWhiteSpace(unit) ? string.Empty : " " + unit),
                    specificationResult,
                    sample.SourceResult);
                DataGridViewRow row = sampleGrid.Rows[rowIndex];
                row.Tag = sample.SourceFile;
                if (sample.IsWithinSpecification == false)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 218, 218);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(145, 12, 18);
                }
                else if (sample.IsWithinSpecification == true)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(224, 247, 230);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(20, 100, 53);
                }
            }
            sampleGrid.ClearSelection();
            sampleGrid.ResumeLayout();
        }

        private void ClearResultDisplay()
        {
            sampleCountValueLabel.Text = "--";
            meanValueLabel.Text = "--";
            withinSigmaValueLabel.Text = "--";
            cpValueLabel.Text = "--";
            cpkValueLabel.Text = "--";
            ppkValueLabel.Text = "--";
            outOfSpecValueLabel.Text = "--";
            conclusionValueLabel.Text = "--";
            trendChart.Series.Clear();
            histogramChart.Series.Clear();
            sampleGrid.Rows.Clear();
            exportButton.Enabled = false;
        }

        private void SetBusy(bool busy, string message)
        {
            progressBar.Visible = busy;
            queryButton.Enabled = !busy;
            resetButton.Enabled = !busy;
            specificationButton.Enabled = !busy;
            exportButton.Enabled = !busy && lastCalculation != null && lastCalculation.SampleCount > 0;
            statusLabel.Text = message ?? string.Empty;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private string GetSelectedParameter()
        {
            if (parameterComboBox.SelectedItem == null ||
                string.Equals(parameterComboBox.SelectedItem.ToString(), NoParameterText, StringComparison.Ordinal))
            {
                return string.Empty;
            }
            return parameterComboBox.SelectedItem.ToString();
        }

        private HistoryPeriodKind GetPeriodKind()
        {
            switch (periodComboBox.SelectedIndex)
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

        private void ApplyPeriodSelection()
        {
            DateTime today = DateTime.Today;
            bool custom = periodComboBox.SelectedIndex == 4;
            startDatePicker.Enabled = custom;
            endDatePicker.Enabled = custom;
            switch (periodComboBox.SelectedIndex)
            {
                case 1:
                    startDatePicker.Value = today.AddMonths(-3).AddDays(1);
                    endDatePicker.Value = today;
                    break;
                case 2:
                    int quarterStartMonth = ((today.Month - 1) / 3) * 3 + 1;
                    startDatePicker.Value = new DateTime(today.Year, quarterStartMonth, 1);
                    endDatePicker.Value = today;
                    break;
                case 3:
                    startDatePicker.Value = new DateTime(today.Year, 1, 1);
                    endDatePicker.Value = today;
                    break;
                case 4:
                    if (startDatePicker.Value.Date > endDatePicker.Value.Date)
                    {
                        startDatePicker.Value = today.AddDays(-6);
                        endDatePicker.Value = today;
                    }
                    break;
                default:
                    startDatePicker.Value = today.AddDays(-6);
                    endDatePicker.Value = today;
                    break;
            }
        }

        private void QueryButton_Click(object sender, EventArgs e)
        {
            BeginQuery();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            periodComboBox.SelectedIndex = 0;
            sourceComboBox.SelectedIndex = 0;
            barcodeTextBox.Clear();
            ApplyPeriodSelection();
            BeginQuery();
        }

        private void PeriodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyPeriodSelection();
        }

        private void ParameterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!changingParameterItems && lastQueryResult != null)
            {
                RenderCalculation();
            }
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                BeginQuery();
            }
        }

        private void SpecificationButton_Click(object sender, EventArgs e)
        {
            if (specificationRepository == null)
            {
                MessageBox.Show("CPK页面尚未完成初始化。", "规格配置", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<CpkSpecification> displaySpecifications = CpkSpecificationRepository.Merge(
                configuredSpecifications,
                defaultSpecifications);
            using (CpkSpecificationForm form = new CpkSpecificationForm(displaySpecifications, defaultSpecifications))
            {
                if (form.ShowDialog(FindForm()) != DialogResult.OK)
                {
                    return;
                }
                try
                {
                    specificationRepository.Save(form.Specifications);
                    configuredSpecifications = form.Specifications.Select(item => item.Clone()).ToList();
                    PopulateKnownParameters();
                    RenderCalculation();
                    statusLabel.Text = "CPK规格配置已保存：" + specificationRepository.FilePath;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("CPK规格配置保存失败：" + exception.Message, "规格配置", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (lastQueryResult == null || lastCalculation == null || lastCalculation.SampleCount == 0)
            {
                return;
            }
            Directory.CreateDirectory(reportDirectory);
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "导出涂胶CPK报表";
                dialog.Filter = "Excel工作簿 (*.xlsx)|*.xlsx";
                dialog.InitialDirectory = reportDirectory;
                dialog.FileName = string.Format(
                    "涂胶CPK_{0}_{1:yyyyMMdd}_{2:yyyyMMdd}.xlsx",
                    SanitizeFileName(lastCalculation.ParameterName),
                    lastQueryResult.Filter.StartTime,
                    lastQueryResult.Filter.EndTime);
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                {
                    return;
                }
                try
                {
                    CpkExcelExporter.Export(dialog.FileName, lastQueryResult.Filter, lastCalculation);
                    statusLabel.Text = "CPK报表已导出：" + dialog.FileName;
                    if (MessageBox.Show("CPK报表导出成功，是否立即打开？", "导出Excel", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("CPK报表导出失败：" + exception.Message, "导出Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SampleGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            string filePath = sampleGrid.Rows[e.RowIndex].Tag as string;
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", "/select,\"" + filePath + "\"")
                {
                    UseShellExecute = true
                });
            }
        }

        private static IEnumerable<CpkSample> SelectChartSamples(IList<CpkSample> samples)
        {
            if (samples.Count <= MaximumChartPoints)
            {
                return samples;
            }
            int step = (int)Math.Ceiling(samples.Count / (double)MaximumChartPoints);
            List<CpkSample> selected = new List<CpkSample>();
            for (int index = 0; index < samples.Count; index += step)
            {
                selected.Add(samples[index]);
            }
            if (selected.Count == 0 || selected[selected.Count - 1].Sequence != samples[samples.Count - 1].Sequence)
            {
                selected.Add(samples[samples.Count - 1]);
            }
            return selected;
        }

        private static void AddHorizontalSeries(
            Chart chart,
            string name,
            double value,
            int sampleCount,
            Color color,
            ChartDashStyle dashStyle)
        {
            Series series = new Series(name)
            {
                ChartType = SeriesChartType.Line,
                Color = color,
                BorderWidth = 2,
                BorderDashStyle = dashStyle,
                IsVisibleInLegend = true
            };
            series.Points.AddXY(1, value);
            series.Points.AddXY(Math.Max(1, sampleCount), value);
            chart.Series.Add(series);
        }

        private static void AddSpecificationStripLine(
            Chart chart,
            ChartArea area,
            double? value,
            string title,
            Color color)
        {
            if (!value.HasValue)
            {
                return;
            }
            area.AxisX.StripLines.Add(new StripLine
            {
                IntervalOffset = value.Value,
                StripWidth = 0D,
                BorderColor = color,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash,
                ToolTip = string.Format("{0}: {1:0.###}", title, value.Value)
            });

            Series legendSeries = new Series("Specification_" + title)
            {
                ChartType = SeriesChartType.Line,
                Color = color,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash,
                IsVisibleInLegend = true,
                LegendText = string.Format("{0} {1:0.###}", title, value.Value)
            };
            chart.Series.Add(legendSeries);
        }

        private static void IncludeAxisValue(double? value, ref double minimum, ref double maximum)
        {
            if (!value.HasValue || double.IsNaN(value.Value) || double.IsInfinity(value.Value))
            {
                return;
            }
            minimum = Math.Min(minimum, value.Value);
            maximum = Math.Max(maximum, value.Value);
        }

        private static string NormalizeParameterName(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Split('/')[0].Trim();
        }

        private static string FormatNullable(double? value)
        {
            return value.HasValue && !double.IsNaN(value.Value) && !double.IsInfinity(value.Value)
                ? value.Value.ToString("0.#####", CultureInfo.CurrentCulture)
                : "--";
        }

        private static string SanitizeFileName(string value)
        {
            string name = string.IsNullOrWhiteSpace(value) ? "未命名参数" : value;
            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidCharacter, '_');
            }
            return name;
        }
    }
}
