using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace BJTSurrenSystem.History
{
    internal partial class CpkSpecificationForm : Form
    {
        private readonly List<CpkSpecification> mesDefaults;

        public CpkSpecificationForm(
            IEnumerable<CpkSpecification> configuredSpecifications,
            IEnumerable<CpkSpecification> defaultSpecifications)
        {
            InitializeComponent();
            CreateColumns();
            mesDefaults = (defaultSpecifications ?? Enumerable.Empty<CpkSpecification>())
                .Select(item => item.Clone())
                .ToList();
            LoadRows(configuredSpecifications);
        }

        public List<CpkSpecification> Specifications { get; private set; }

        private void CreateColumns()
        {
            specificationGrid.Columns.Clear();
            specificationGrid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "enabledColumn",
                HeaderText = "启用",
                FillWeight = 42F
            });
            specificationGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "parameterColumn",
                HeaderText = "参数名称",
                FillWeight = 125F
            });
            specificationGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "unitColumn",
                HeaderText = "单位",
                FillWeight = 55F
            });
            specificationGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "lowerColumn",
                HeaderText = "下限 LSL",
                FillWeight = 72F
            });
            specificationGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "targetColumn",
                HeaderText = "目标值",
                FillWeight = 72F
            });
            specificationGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "upperColumn",
                HeaderText = "上限 USL",
                FillWeight = 72F
            });
            DataGridViewComboBoxColumn modeColumn = new DataGridViewComboBoxColumn
            {
                Name = "modeColumn",
                HeaderText = "计算模式",
                FillWeight = 105F,
                FlatStyle = FlatStyle.Flat
            };
            modeColumn.Items.AddRange("单值移动极差", "子组法");
            specificationGrid.Columns.Add(modeColumn);
            specificationGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "subgroupColumn",
                HeaderText = "子组大小",
                FillWeight = 66F
            });
            specificationGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "minimumColumn",
                HeaderText = "最小样本数",
                FillWeight = 75F
            });
        }

        private void LoadRows(IEnumerable<CpkSpecification> specifications)
        {
            specificationGrid.Rows.Clear();
            foreach (CpkSpecification specification in specifications ?? Enumerable.Empty<CpkSpecification>())
            {
                AddSpecificationRow(specification);
            }
            specificationGrid.ClearSelection();
        }

        private void AddSpecificationRow(CpkSpecification specification)
        {
            CpkSpecification value = specification ?? new CpkSpecification();
            specificationGrid.Rows.Add(
                value.Enabled,
                value.ParameterName ?? string.Empty,
                value.Unit ?? string.Empty,
                FormatNullable(value.LowerLimit),
                FormatNullable(value.Target),
                FormatNullable(value.UpperLimit),
                CpkSpecificationRepository.FormatMode(value.CalculationMode),
                Math.Max(1, value.SubgroupSize),
                Math.Max(2, value.MinimumSampleSize));
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddSpecificationRow(new CpkSpecification());
            int lastIndex = specificationGrid.Rows.Count - 1;
            if (lastIndex >= 0)
            {
                specificationGrid.CurrentCell = specificationGrid.Rows[lastIndex].Cells[1];
                specificationGrid.BeginEdit(true);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (specificationGrid.CurrentRow != null)
            {
                specificationGrid.Rows.Remove(specificationGrid.CurrentRow);
            }
        }

        private void ImportMesButton_Click(object sender, EventArgs e)
        {
            List<CpkSpecification> current;
            string error;
            if (!TryReadRows(false, out current, out error))
            {
                MessageBox.Show(error, "配置检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<CpkSpecification> merged = CpkSpecificationRepository.Merge(current, mesDefaults);
            LoadRows(merged);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            List<CpkSpecification> specifications;
            string error;
            if (!TryReadRows(true, out specifications, out error))
            {
                MessageBox.Show(error, "配置检查", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Specifications = specifications;
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool TryReadRows(
            bool requireSpecificationLimit,
            out List<CpkSpecification> specifications,
            out string error)
        {
            specifications = new List<CpkSpecification>();
            error = string.Empty;
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int rowIndex = 0; rowIndex < specificationGrid.Rows.Count; rowIndex++)
            {
                DataGridViewRow row = specificationGrid.Rows[rowIndex];
                string parameterName = CellText(row, 1).Trim();
                if (string.IsNullOrWhiteSpace(parameterName))
                {
                    error = string.Format("第 {0} 行参数名称不能为空。", rowIndex + 1);
                    return false;
                }
                if (!names.Add(parameterName))
                {
                    error = "参数名称重复：" + parameterName;
                    return false;
                }

                bool enabled = row.Cells[0].Value == null || Convert.ToBoolean(row.Cells[0].Value);

                double? lower;
                double? target;
                double? upper;
                if (!TryParseOptionalDouble(CellText(row, 3), out lower) ||
                    !TryParseOptionalDouble(CellText(row, 4), out target) ||
                    !TryParseOptionalDouble(CellText(row, 5), out upper))
                {
                    error = string.Format("第 {0} 行规格值格式不正确。", rowIndex + 1);
                    return false;
                }
                if (requireSpecificationLimit && enabled && !lower.HasValue && !upper.HasValue)
                {
                    error = string.Format("第 {0} 行至少需要填写LSL或USL。", rowIndex + 1);
                    return false;
                }
                if (lower.HasValue && upper.HasValue && lower.Value >= upper.Value)
                {
                    error = string.Format("第 {0} 行LSL必须小于USL。", rowIndex + 1);
                    return false;
                }
                if (target.HasValue &&
                    ((lower.HasValue && target.Value < lower.Value) ||
                     (upper.HasValue && target.Value > upper.Value)))
                {
                    error = string.Format("第 {0} 行目标值必须位于规格范围内。", rowIndex + 1);
                    return false;
                }

                int subgroupSize;
                int minimumSampleSize;
                if (!int.TryParse(CellText(row, 7), out subgroupSize) || subgroupSize < 1 ||
                    !int.TryParse(CellText(row, 8), out minimumSampleSize) || minimumSampleSize < 2)
                {
                    error = string.Format("第 {0} 行子组大小或最小样本数格式不正确。", rowIndex + 1);
                    return false;
                }
                CpkCalculationMode mode = CpkSpecificationRepository.ParseMode(CellText(row, 6));
                if (mode == CpkCalculationMode.Subgroup && subgroupSize < 2)
                {
                    error = string.Format("第 {0} 行使用子组法时，子组大小至少为2。", rowIndex + 1);
                    return false;
                }

                specifications.Add(new CpkSpecification
                {
                    Enabled = enabled,
                    ParameterName = parameterName,
                    Unit = CellText(row, 2).Trim(),
                    LowerLimit = lower,
                    Target = target,
                    UpperLimit = upper,
                    CalculationMode = mode,
                    SubgroupSize = subgroupSize,
                    MinimumSampleSize = minimumSampleSize
                });
            }
            return true;
        }

        private static bool TryParseOptionalDouble(string text, out double? value)
        {
            value = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }
            double parsed;
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed) ||
                double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out parsed))
            {
                value = parsed;
                return true;
            }
            return false;
        }

        private static string CellText(DataGridViewRow row, int index)
        {
            return row.Cells[index].Value == null ? string.Empty : row.Cells[index].Value.ToString();
        }

        private static string FormatNullable(double? value)
        {
            return value.HasValue
                ? value.Value.ToString("0.###############", CultureInfo.InvariantCulture)
                : string.Empty;
        }
    }
}
