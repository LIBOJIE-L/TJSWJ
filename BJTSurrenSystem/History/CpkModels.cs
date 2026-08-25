using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BJTSurrenSystem.History
{
    internal enum CpkCalculationMode
    {
        IndividualMovingRange,
        Subgroup
    }

    internal sealed class CpkSpecification
    {
        public CpkSpecification()
        {
            Enabled = true;
            CalculationMode = CpkCalculationMode.IndividualMovingRange;
            SubgroupSize = 1;
            MinimumSampleSize = 30;
        }

        public string ParameterName { get; set; }
        public bool Enabled { get; set; }
        public string Unit { get; set; }
        public double? LowerLimit { get; set; }
        public double? Target { get; set; }
        public double? UpperLimit { get; set; }
        public CpkCalculationMode CalculationMode { get; set; }
        public int SubgroupSize { get; set; }
        public int MinimumSampleSize { get; set; }

        public CpkSpecification Clone()
        {
            return (CpkSpecification)MemberwiseClone();
        }
    }

    internal sealed class CpkSample
    {
        public int Sequence { get; set; }
        public DateTime CollectTime { get; set; }
        public string Barcode { get; set; }
        public string SourceType { get; set; }
        public string SourceResult { get; set; }
        public string SourceFile { get; set; }
        public double Value { get; set; }
        public bool? IsWithinSpecification { get; set; }
    }

    internal sealed class CpkCalculationResult
    {
        public CpkCalculationResult()
        {
            Samples = new List<CpkSample>();
        }

        public string ParameterName { get; set; }
        public CpkSpecification Specification { get; set; }
        public List<CpkSample> Samples { get; set; }
        public int SkippedValueCount { get; set; }
        public double Mean { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double? OverallStandardDeviation { get; set; }
        public double? WithinStandardDeviation { get; set; }
        public double? Cp { get; set; }
        public double? Cpu { get; set; }
        public double? Cpl { get; set; }
        public double? Cpk { get; set; }
        public double? Pp { get; set; }
        public double? Ppu { get; set; }
        public double? Ppl { get; set; }
        public double? Ppk { get; set; }
        public int OutOfSpecificationCount { get; set; }
        public bool MeetsMinimumSampleSize { get; set; }
        public string Conclusion { get; set; }

        public int SampleCount
        {
            get { return Samples == null ? 0 : Samples.Count; }
        }
    }

    internal static class CpkCalculator
    {
        private const double MovingRangeD2 = 1.128D;
        private const double MinimumPositiveSigma = 1E-12D;

        public static CpkCalculationResult Calculate(
            IEnumerable<HistoryRecord> records,
            string parameterName,
            CpkSpecification specification)
        {
            CpkCalculationResult result = new CpkCalculationResult
            {
                ParameterName = parameterName ?? string.Empty,
                Specification = specification == null ? null : specification.Clone()
            };

            if (records == null || string.IsNullOrWhiteSpace(parameterName))
            {
                result.Conclusion = "请选择数值参数";
                return result;
            }

            List<HistoryRecord> orderedRecords = records
                .OrderBy(item => item.CollectTime)
                .ThenBy(item => item.SourceFile, StringComparer.OrdinalIgnoreCase)
                .ToList();
            foreach (HistoryRecord record in orderedRecords)
            {
                double value;
                if (!TryGetScalarValue(record, parameterName, out value))
                {
                    result.SkippedValueCount++;
                    continue;
                }

                CpkSample sample = new CpkSample
                {
                    Sequence = result.Samples.Count + 1,
                    CollectTime = record.CollectTime,
                    Barcode = record.Barcode,
                    SourceType = record.SourceType,
                    SourceResult = record.Result,
                    SourceFile = record.SourceFile,
                    Value = value
                };
                if (specification != null &&
                    (specification.LowerLimit.HasValue || specification.UpperLimit.HasValue))
                {
                    sample.IsWithinSpecification =
                        (!specification.LowerLimit.HasValue || value >= specification.LowerLimit.Value) &&
                        (!specification.UpperLimit.HasValue || value <= specification.UpperLimit.Value);
                }
                result.Samples.Add(sample);
            }

            if (result.Samples.Count == 0)
            {
                result.Conclusion = "没有有效的单值数值样本";
                return result;
            }

            List<double> values = result.Samples.Select(item => item.Value).ToList();
            result.Mean = values.Average();
            result.Minimum = values.Min();
            result.Maximum = values.Max();
            result.OverallStandardDeviation = CalculateSampleStandardDeviation(values);
            result.WithinStandardDeviation = CalculateWithinStandardDeviation(values, specification);
            result.OutOfSpecificationCount = result.Samples.Count(item => item.IsWithinSpecification == false);

            int minimumSampleSize = specification == null
                ? 30
                : Math.Max(2, specification.MinimumSampleSize);
            result.MeetsMinimumSampleSize = values.Count >= minimumSampleSize;

            if (specification == null ||
                (!specification.LowerLimit.HasValue && !specification.UpperLimit.HasValue))
            {
                result.Conclusion = "未配置规格上下限";
                return result;
            }

            double? twoSided;
            double? upper;
            double? lower;
            double? centered;
            CalculateCapability(
                result.Mean,
                result.WithinStandardDeviation,
                specification,
                out twoSided,
                out upper,
                out lower,
                out centered);
            result.Cp = twoSided;
            result.Cpu = upper;
            result.Cpl = lower;
            result.Cpk = centered;
            CalculateCapability(
                result.Mean,
                result.OverallStandardDeviation,
                specification,
                out twoSided,
                out upper,
                out lower,
                out centered);
            result.Pp = twoSided;
            result.Ppu = upper;
            result.Ppl = lower;
            result.Ppk = centered;

            if (!result.MeetsMinimumSampleSize)
            {
                result.Conclusion = string.Format(
                    CultureInfo.CurrentCulture,
                    "样本不足（{0}/{1}）",
                    result.SampleCount,
                    minimumSampleSize);
            }
            else if (!result.Cpk.HasValue)
            {
                result.Conclusion = "波动为零或组内样本不足";
            }
            else if (result.Cpk.Value >= 1.67D)
            {
                result.Conclusion = "过程能力优秀";
            }
            else if (result.Cpk.Value >= 1.33D)
            {
                result.Conclusion = "过程能力合格";
            }
            else if (result.Cpk.Value >= 1D)
            {
                result.Conclusion = "过程能力需改善";
            }
            else
            {
                result.Conclusion = "过程能力不足";
            }

            return result;
        }

        private static double? CalculateWithinStandardDeviation(
            IList<double> values,
            CpkSpecification specification)
        {
            CpkCalculationMode mode = specification == null
                ? CpkCalculationMode.IndividualMovingRange
                : specification.CalculationMode;
            int subgroupSize = specification == null ? 1 : specification.SubgroupSize;
            if (mode == CpkCalculationMode.Subgroup && subgroupSize >= 2)
            {
                return CalculatePooledWithinStandardDeviation(values, subgroupSize);
            }

            if (values.Count < 2)
            {
                return null;
            }

            double movingRangeAverage = 0D;
            for (int index = 1; index < values.Count; index++)
            {
                movingRangeAverage += Math.Abs(values[index] - values[index - 1]);
            }
            movingRangeAverage /= values.Count - 1D;
            double sigma = movingRangeAverage / MovingRangeD2;
            return sigma > MinimumPositiveSigma ? (double?)sigma : null;
        }

        private static double? CalculatePooledWithinStandardDeviation(
            IList<double> values,
            int subgroupSize)
        {
            int completeGroupCount = values.Count / subgroupSize;
            if (completeGroupCount < 2)
            {
                return null;
            }

            double weightedVarianceSum = 0D;
            int freedom = 0;
            for (int groupIndex = 0; groupIndex < completeGroupCount; groupIndex++)
            {
                List<double> group = values
                    .Skip(groupIndex * subgroupSize)
                    .Take(subgroupSize)
                    .ToList();
                double? standardDeviation = CalculateSampleStandardDeviation(group);
                if (!standardDeviation.HasValue)
                {
                    continue;
                }
                weightedVarianceSum += (group.Count - 1D) * standardDeviation.Value * standardDeviation.Value;
                freedom += group.Count - 1;
            }

            if (freedom <= 0)
            {
                return null;
            }
            double sigma = Math.Sqrt(weightedVarianceSum / freedom);
            return sigma > MinimumPositiveSigma ? (double?)sigma : null;
        }

        private static double? CalculateSampleStandardDeviation(IList<double> values)
        {
            if (values == null || values.Count < 2)
            {
                return null;
            }

            double average = values.Average();
            double squaredDifferenceSum = values.Sum(value =>
                (value - average) * (value - average));
            double sigma = Math.Sqrt(squaredDifferenceSum / (values.Count - 1D));
            return sigma > MinimumPositiveSigma ? (double?)sigma : null;
        }

        private static void CalculateCapability(
            double mean,
            double? sigma,
            CpkSpecification specification,
            out double? twoSided,
            out double? upper,
            out double? lower,
            out double? centered)
        {
            twoSided = null;
            upper = null;
            lower = null;
            centered = null;
            if (!sigma.HasValue || sigma.Value <= MinimumPositiveSigma || specification == null)
            {
                return;
            }

            if (specification.UpperLimit.HasValue)
            {
                upper = (specification.UpperLimit.Value - mean) / (3D * sigma.Value);
            }
            if (specification.LowerLimit.HasValue)
            {
                lower = (mean - specification.LowerLimit.Value) / (3D * sigma.Value);
            }
            if (upper.HasValue && lower.HasValue)
            {
                twoSided = (specification.UpperLimit.Value - specification.LowerLimit.Value) /
                           (6D * sigma.Value);
                centered = Math.Min(upper.Value, lower.Value);
            }
            else
            {
                centered = upper ?? lower;
            }
        }

        private static bool TryGetScalarValue(
            HistoryRecord record,
            string parameterName,
            out double value)
        {
            value = 0D;
            if (record == null || string.IsNullOrWhiteSpace(parameterName))
            {
                return false;
            }

            string rawValue;
            if (!record.Fields.TryGetValue(parameterName, out rawValue))
            {
                string normalizedName = parameterName.Split('/')[0].Trim();
                KeyValuePair<string, string> match = record.Fields.FirstOrDefault(item =>
                    string.Equals(item.Key.Split('/')[0].Trim(), normalizedName, StringComparison.OrdinalIgnoreCase));
                rawValue = match.Value;
            }
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            string normalized = rawValue.Trim().Trim(';').Replace("\"", string.Empty);
            double parsedValue;
            bool parsed = double.TryParse(
                              normalized,
                              NumberStyles.Any,
                              CultureInfo.InvariantCulture,
                              out parsedValue) ||
                          double.TryParse(
                              normalized,
                              NumberStyles.Any,
                              CultureInfo.CurrentCulture,
                              out parsedValue);
            if (!parsed || double.IsNaN(parsedValue) || double.IsInfinity(parsedValue))
            {
                return false;
            }

            value = parsedValue;
            return true;
        }
    }
}
