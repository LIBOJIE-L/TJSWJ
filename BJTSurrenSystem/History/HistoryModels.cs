using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BJTSurrenSystem.History
{
    internal enum HistoryPeriodKind
    {
        LastSevenDays,
        LastThreeMonths,
        Quarter,
        Year,
        Custom
    }

    internal sealed class HistoryFilter
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SourceType { get; set; }
        public string Barcode { get; set; }
        public HistoryPeriodKind PeriodKind { get; set; }
    }

    internal sealed class HistoryRecord
    {
        public HistoryRecord()
        {
            Fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string SourceType { get; set; }
        public string SourceFile { get; set; }
        public DateTime CollectTime { get; set; }
        public Dictionary<string, string> Fields { get; private set; }

        public string Barcode
        {
            get { return FindValue("模组码", "条码", "SFC", "sfc"); }
        }

        public string Result
        {
            get
            {
                string result = FindValue("结果", "result");
                if (string.IsNullOrWhiteSpace(result))
                {
                    // 兼容旧流程中MES失败时把NG误写到“班次”列的历史文件。
                    string shift = FindValue("班次");
                    if (string.Equals(shift, "NG", StringComparison.OrdinalIgnoreCase))
                    {
                        return "NG";
                    }
                }

                if (result.IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    result.IndexOf("失败", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "NG";
                }

                if (result.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    result.IndexOf("成功", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "OK";
                }

                return string.IsNullOrWhiteSpace(result) ? "未知" : result.Trim();
            }
        }

        public string Device
        {
            get { return FindValue("设备标识", "设备标识号", "Resource"); }
        }

        public string Shift
        {
            get { return FindValue("班次"); }
        }

        public string FindValue(params string[] aliases)
        {
            foreach (string alias in aliases)
            {
                string exactValue;
                if (Fields.TryGetValue(alias, out exactValue))
                {
                    return exactValue ?? string.Empty;
                }
            }

            foreach (KeyValuePair<string, string> field in Fields)
            {
                foreach (string alias in aliases)
                {
                    if (field.Key.StartsWith(alias + "/", StringComparison.OrdinalIgnoreCase) ||
                        field.Key.StartsWith(alias + " ", StringComparison.OrdinalIgnoreCase))
                    {
                        return field.Value ?? string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        public bool TryGetNumericValue(string fieldName, out double value)
        {
            value = 0D;
            string rawValue;
            if (string.IsNullOrWhiteSpace(fieldName) || !Fields.TryGetValue(fieldName, out rawValue))
            {
                return false;
            }

            return HistoryNumberParser.TryParse(rawValue, out value);
        }
    }

    internal static class HistoryNumberParser
    {
        public static bool TryParse(string rawValue, out double value)
        {
            value = 0D;
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            string normalized = rawValue.Trim().Trim(';').Replace("\"", string.Empty);
            double directValue;
            if (double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out directValue) ||
                double.TryParse(normalized, NumberStyles.Any, CultureInfo.CurrentCulture, out directValue))
            {
                value = directValue;
                return !double.IsNaN(value) && !double.IsInfinity(value);
            }

            string[] parts = normalized.Split(new[] { '，', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<double> values = new List<double>();
            foreach (string part in parts)
            {
                double partValue;
                string cleanPart = part.Trim();
                if (double.TryParse(cleanPart, NumberStyles.Any, CultureInfo.InvariantCulture, out partValue) ||
                    double.TryParse(cleanPart, NumberStyles.Any, CultureInfo.CurrentCulture, out partValue))
                {
                    values.Add(partValue);
                }
            }

            if (values.Count == 0)
            {
                return false;
            }

            value = values.Average();
            return true;
        }
    }

    internal sealed class HistoryParameterLimit
    {
        public bool HasLowerLimit { get; set; }
        public double LowerLimit { get; set; }
        public bool HasUpperLimit { get; set; }
        public double UpperLimit { get; set; }
    }

    internal sealed class HistoryBucket
    {
        public string Label { get; set; }
        public int TotalCount { get; set; }
        public int OkCount { get; set; }
        public int NgCount { get; set; }
        public bool HasParameterValue { get; set; }
        public double ParameterAverage { get; set; }
        public double ParameterMinimum { get; set; }
        public double ParameterMaximum { get; set; }
    }

    internal sealed class HistoryParameterStatistics
    {
        public string ParameterName { get; set; }
        public int ValueCount { get; set; }
        public double Average { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public int OutOfLimitCount { get; set; }
    }

    internal sealed class HistoryQueryResult
    {
        public HistoryQueryResult()
        {
            Records = new List<HistoryRecord>();
            NumericParameters = new List<string>();
        }

        public HistoryFilter Filter { get; set; }
        public List<HistoryRecord> Records { get; set; }
        public List<string> NumericParameters { get; set; }

        public int TotalCount { get { return Records.Count; } }
        public int OkCount { get { return Records.Count(item => item.Result == "OK"); } }
        public int NgCount { get { return Records.Count(item => item.Result == "NG"); } }
        public double YieldRate { get { return TotalCount == 0 ? 0D : OkCount * 100D / TotalCount; } }
    }

    internal static class HistoryAggregator
    {
        public static List<HistoryBucket> CreateBuckets(
            HistoryQueryResult result,
            string parameterName)
        {
            List<HistoryBucket> buckets = new List<HistoryBucket>();
            if (result == null || result.Filter == null)
            {
                return buckets;
            }

            DateTime cursor = GetBucketStart(result.Filter.StartTime, result.Filter.PeriodKind);
            DateTime end = result.Filter.EndTime;
            while (cursor <= end)
            {
                DateTime next = GetNextBucket(cursor, result.Filter.PeriodKind);
                DateTime bucketEnd = next.AddTicks(-1);
                List<HistoryRecord> records = result.Records
                    .Where(item => item.CollectTime >= cursor && item.CollectTime < next)
                    .ToList();
                List<double> parameterValues = new List<double>();
                if (!string.IsNullOrWhiteSpace(parameterName))
                {
                    foreach (HistoryRecord record in records)
                    {
                        double numericValue;
                        if (record.TryGetNumericValue(parameterName, out numericValue))
                        {
                            parameterValues.Add(numericValue);
                        }
                    }
                }

                HistoryBucket bucket = new HistoryBucket
                {
                    Label = GetBucketLabel(cursor, bucketEnd, result.Filter.PeriodKind),
                    TotalCount = records.Count,
                    OkCount = records.Count(item => item.Result == "OK"),
                    NgCount = records.Count(item => item.Result == "NG"),
                    HasParameterValue = parameterValues.Count > 0
                };
                if (parameterValues.Count > 0)
                {
                    bucket.ParameterAverage = parameterValues.Average();
                    bucket.ParameterMinimum = parameterValues.Min();
                    bucket.ParameterMaximum = parameterValues.Max();
                }

                buckets.Add(bucket);
                cursor = next;
            }

            return buckets;
        }

        public static HistoryParameterStatistics CreateParameterStatistics(
            HistoryQueryResult result,
            string parameterName,
            HistoryParameterLimit limit)
        {
            HistoryParameterStatistics statistics = new HistoryParameterStatistics
            {
                ParameterName = parameterName ?? string.Empty
            };
            if (result == null || string.IsNullOrWhiteSpace(parameterName))
            {
                return statistics;
            }

            List<double> values = new List<double>();
            foreach (HistoryRecord record in result.Records)
            {
                double numericValue;
                if (record.TryGetNumericValue(parameterName, out numericValue))
                {
                    values.Add(numericValue);
                }
            }

            statistics.ValueCount = values.Count;
            if (values.Count == 0)
            {
                return statistics;
            }

            statistics.Average = values.Average();
            statistics.Minimum = values.Min();
            statistics.Maximum = values.Max();
            if (limit != null)
            {
                statistics.OutOfLimitCount = values.Count(value =>
                    (limit.HasLowerLimit && value < limit.LowerLimit) ||
                    (limit.HasUpperLimit && value > limit.UpperLimit));
            }

            return statistics;
        }

        private static DateTime GetBucketStart(DateTime start, HistoryPeriodKind periodKind)
        {
            DateTime date = start.Date;
            if (periodKind == HistoryPeriodKind.Year)
            {
                return new DateTime(date.Year, date.Month, 1);
            }

            if (periodKind == HistoryPeriodKind.LastThreeMonths || periodKind == HistoryPeriodKind.Quarter)
            {
                int daysFromMonday = ((int)date.DayOfWeek + 6) % 7;
                return date.AddDays(-daysFromMonday);
            }

            return date;
        }

        private static DateTime GetNextBucket(DateTime bucketStart, HistoryPeriodKind periodKind)
        {
            if (periodKind == HistoryPeriodKind.Year)
            {
                return bucketStart.AddMonths(1);
            }

            if (periodKind == HistoryPeriodKind.LastThreeMonths || periodKind == HistoryPeriodKind.Quarter)
            {
                return bucketStart.AddDays(7);
            }

            return bucketStart.AddDays(1);
        }

        private static string GetBucketLabel(DateTime start, DateTime end, HistoryPeriodKind periodKind)
        {
            if (periodKind == HistoryPeriodKind.Year)
            {
                return start.ToString("yyyy-MM");
            }

            if (periodKind == HistoryPeriodKind.LastThreeMonths || periodKind == HistoryPeriodKind.Quarter)
            {
                return start.ToString("MM-dd") + "~" + end.ToString("MM-dd");
            }

            return start.ToString("MM-dd");
        }
    }
}
