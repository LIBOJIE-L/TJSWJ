using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BJTSurrenSystem.History
{
    internal sealed class CpkSpecificationRepository
    {
        private static readonly string[] Headers =
        {
            "参数名称", "启用", "单位", "下限LSL", "目标值", "上限USL",
            "计算模式", "子组大小", "最小样本数"
        };

        private readonly string filePath;

        public CpkSpecificationRepository(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("CPK配置文件路径不能为空。", "filePath");
            }
            this.filePath = Path.GetFullPath(filePath);
        }

        public string FilePath
        {
            get { return filePath; }
        }

        public List<CpkSpecification> Load()
        {
            List<CpkSpecification> specifications = new List<CpkSpecification>();
            if (!File.Exists(filePath))
            {
                return specifications;
            }

            using (FileStream stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                string line;
                bool firstLine = true;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    List<string> fields = ParseCsvLine(line);
                    if (firstLine)
                    {
                        firstLine = false;
                        if (fields.Count > 0 && string.Equals(fields[0].Trim(), Headers[0], StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    CpkSpecification specification;
                    if (TryParseSpecification(fields, out specification))
                    {
                        specifications.Add(specification);
                    }
                }
            }

            return specifications
                .GroupBy(item => item.ParameterName, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.Last())
                .OrderBy(item => item.ParameterName, StringComparer.CurrentCulture)
                .ToList();
        }

        public void Save(IEnumerable<CpkSpecification> specifications)
        {
            string directory = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directory);
            string temporaryPath = filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (FileStream stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(true)))
                {
                    writer.WriteLine(string.Join(",", Headers.Select(EscapeCsvField)));
                    foreach (CpkSpecification specification in (specifications ?? Enumerable.Empty<CpkSpecification>())
                        .Where(item => item != null && !string.IsNullOrWhiteSpace(item.ParameterName))
                        .OrderBy(item => item.ParameterName, StringComparer.CurrentCulture))
                    {
                        string[] fields =
                        {
                            specification.ParameterName.Trim(),
                            specification.Enabled ? "true" : "false",
                            specification.Unit ?? string.Empty,
                            FormatNullable(specification.LowerLimit),
                            FormatNullable(specification.Target),
                            FormatNullable(specification.UpperLimit),
                            FormatMode(specification.CalculationMode),
                            Math.Max(1, specification.SubgroupSize).ToString(CultureInfo.InvariantCulture),
                            Math.Max(2, specification.MinimumSampleSize).ToString(CultureInfo.InvariantCulture)
                        };
                        writer.WriteLine(string.Join(",", fields.Select(EscapeCsvField)));
                    }
                }

                File.Copy(temporaryPath, filePath, true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        public static List<CpkSpecification> Merge(
            IEnumerable<CpkSpecification> configured,
            IEnumerable<CpkSpecification> fallback)
        {
            Dictionary<string, CpkSpecification> merged = new Dictionary<string, CpkSpecification>(StringComparer.OrdinalIgnoreCase);
            foreach (CpkSpecification item in fallback ?? Enumerable.Empty<CpkSpecification>())
            {
                if (item != null && !string.IsNullOrWhiteSpace(item.ParameterName))
                {
                    merged[item.ParameterName.Trim()] = item.Clone();
                }
            }
            foreach (CpkSpecification item in configured ?? Enumerable.Empty<CpkSpecification>())
            {
                if (item != null && !string.IsNullOrWhiteSpace(item.ParameterName))
                {
                    merged[item.ParameterName.Trim()] = item.Clone();
                }
            }
            return merged.Values
                .OrderBy(item => item.ParameterName, StringComparer.CurrentCulture)
                .ToList();
        }

        public static string FormatMode(CpkCalculationMode mode)
        {
            return mode == CpkCalculationMode.Subgroup ? "子组法" : "单值移动极差";
        }

        public static CpkCalculationMode ParseMode(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.IndexOf("子组", StringComparison.OrdinalIgnoreCase) >= 0
                ? CpkCalculationMode.Subgroup
                : CpkCalculationMode.IndividualMovingRange;
        }

        private static bool TryParseSpecification(
            IList<string> fields,
            out CpkSpecification specification)
        {
            specification = null;
            if (fields == null || fields.Count == 0 || string.IsNullOrWhiteSpace(fields[0]))
            {
                return false;
            }

            CpkSpecification parsed = new CpkSpecification
            {
                ParameterName = fields[0].Trim(),
                Enabled = fields.Count < 2 || ParseBoolean(fields[1], true),
                Unit = fields.Count > 2 ? fields[2].Trim() : string.Empty,
                LowerLimit = fields.Count > 3 ? ParseNullableDouble(fields[3]) : null,
                Target = fields.Count > 4 ? ParseNullableDouble(fields[4]) : null,
                UpperLimit = fields.Count > 5 ? ParseNullableDouble(fields[5]) : null,
                CalculationMode = fields.Count > 6 ? ParseMode(fields[6]) : CpkCalculationMode.IndividualMovingRange,
                SubgroupSize = fields.Count > 7 ? ParseInteger(fields[7], 1, 1) : 1,
                MinimumSampleSize = fields.Count > 8 ? ParseInteger(fields[8], 30, 2) : 30
            };
            if (parsed.CalculationMode == CpkCalculationMode.Subgroup && parsed.SubgroupSize < 2)
            {
                parsed.SubgroupSize = 5;
            }
            specification = parsed;
            return true;
        }

        private static bool ParseBoolean(string value, bool defaultValue)
        {
            bool parsed;
            if (bool.TryParse((value ?? string.Empty).Trim(), out parsed))
            {
                return parsed;
            }
            string normalized = (value ?? string.Empty).Trim();
            if (normalized == "1" || string.Equals(normalized, "是", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (normalized == "0" || string.Equals(normalized, "否", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return defaultValue;
        }

        private static double? ParseNullableDouble(string value)
        {
            double parsed;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsed) ||
                double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out parsed))
            {
                return parsed;
            }
            return null;
        }

        private static int ParseInteger(string value, int defaultValue, int minimum)
        {
            int parsed;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed)
                ? Math.Max(minimum, parsed)
                : defaultValue;
        }

        private static string FormatNullable(double? value)
        {
            return value.HasValue
                ? value.Value.ToString("0.###############", CultureInfo.InvariantCulture)
                : string.Empty;
        }

        private static string EscapeCsvField(string value)
        {
            string text = value ?? string.Empty;
            if (text.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0)
            {
                return text;
            }
            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }

        private static List<string> ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder current = new StringBuilder();
            bool insideQuotes = false;
            for (int index = 0; index < (line ?? string.Empty).Length; index++)
            {
                char character = line[index];
                if (character == '"')
                {
                    if (insideQuotes && index + 1 < line.Length && line[index + 1] == '"')
                    {
                        current.Append('"');
                        index++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (character == ',' && !insideQuotes)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(character);
                }
            }
            fields.Add(current.ToString());
            return fields;
        }
    }
}
