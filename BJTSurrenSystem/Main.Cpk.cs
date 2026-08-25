using BJTSurrenSystem.History;
using BJTSurrenSystem.Parameters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BJTSurrenSystem
{
    public partial class Main
    {
        private TabPage cpkTabPage;
        private CpkAnalysisControl cpkAnalysisControl;

        private void InitializeCpkDataPage()
        {
            if (tabControl2 == null || tabControl2.IsDisposed)
            {
                return;
            }
            if (cpkTabPage == null || cpkTabPage.IsDisposed)
            {
                cpkTabPage = new TabPage
                {
                    Name = "cpkTabPage",
                    Padding = new Padding(3),
                    UseVisualStyleBackColor = true
                };
            }
            // CPK 页面包含自己的指标卡、图表和状态配色，避免全局主题覆盖这些语义颜色。
            AppUiTheme.Exclude(cpkTabPage);
            if (cpkAnalysisControl == null || cpkAnalysisControl.IsDisposed)
            {
                cpkAnalysisControl = new CpkAnalysisControl
                {
                    Dock = DockStyle.Fill,
                    Name = "cpkAnalysisControl"
                };
            }
            if (!cpkTabPage.Controls.Contains(cpkAnalysisControl))
            {
                cpkTabPage.Controls.Clear();
                cpkTabPage.Controls.Add(cpkAnalysisControl);
            }
            if (!tabControl2.TabPages.Contains(cpkTabPage))
            {
                tabControl2.TabPages.Add(cpkTabPage);
            }
            ApplyCpkPageLanguage();
        }

        private void InitializeCpkDataAfterLoad()
        {
            if (cpkAnalysisControl == null || cpkAnalysisControl.IsDisposed ||
                ResourceHandler.listSystemParameters.Count == 0)
            {
                return;
            }
            string workstation = string.IsNullOrWhiteSpace(workstationName.Text)
                ? "涂胶检查"
                : workstationName.Text.Trim();
            string formulaName = string.IsNullOrWhiteSpace(ResourceHandler.listSystemParameters[0].FormulaName)
                ? "默认配方"
                : ResourceHandler.listSystemParameters[0].FormulaName.Trim();
            string specificationPath = Path.Combine(
                Application.StartupPath,
                "xml",
                workstation,
                formulaName,
                "涂胶CPK参数配置表.CSV");
            cpkAnalysisControl.Configure(
                ResourceHandler.listSystemParameters[0].ProgramLogPath,
                specificationPath,
                ReadMesCpkDefaultSpecifications());
        }

        private static List<CpkSpecification> ReadMesCpkDefaultSpecifications()
        {
            List<CpkSpecification> specifications = new List<CpkSpecification>();
            foreach (MesPullOutUploadingParameters parameter in ResourceHandler.listMesPullOutUploadingParameters)
            {
                if (string.IsNullOrWhiteSpace(parameter.Header))
                {
                    continue;
                }
                double lower;
                double upper;
                bool hasLower = TryParseCpkLimit(parameter.ParametersLowerLimit, out lower);
                bool hasUpper = TryParseCpkLimit(parameter.ParametersUpperLimit, out upper);
                if (!hasLower && !hasUpper)
                {
                    continue;
                }
                CpkSpecification specification = new CpkSpecification
                {
                    ParameterName = parameter.Header.Trim(),
                    Enabled = true,
                    LowerLimit = hasLower ? (double?)lower : null,
                    UpperLimit = hasUpper ? (double?)upper : null,
                    CalculationMode = CpkCalculationMode.IndividualMovingRange,
                    SubgroupSize = 1,
                    MinimumSampleSize = 30
                };
                if (specification.LowerLimit.HasValue && specification.UpperLimit.HasValue)
                {
                    specification.Target = (specification.LowerLimit.Value + specification.UpperLimit.Value) / 2D;
                }
                specifications.Add(specification);
            }
            return specifications
                .GroupBy(item => item.ParameterName, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.Last())
                .ToList();
        }

        private static bool TryParseCpkLimit(string text, out double value)
        {
            value = 0D;
            return !string.IsNullOrWhiteSpace(text) &&
                   (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ||
                    double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value));
        }

        private void ApplyCpkPageLanguage()
        {
            if (cpkTabPage == null || cpkTabPage.IsDisposed)
            {
                return;
            }
            string language = ResourceHandler.listSystemParameters.Count > 0
                ? ResourceHandler.listSystemParameters[0].Language
                : "中";
            cpkTabPage.Text = language == "英"
                ? "Glue CPK"
                : language == "德" ? "Klebstoff-CPK" : "涂胶CPK";
        }
    }
}
