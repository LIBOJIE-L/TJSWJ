using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HJMSurrenSystem.History
{
    internal sealed class HistoryDataRepository
    {
        private const string CacheMagic = "HJMS_HISTORY_INDEX_V2";
        private static readonly string[] SourceDirectories =
        {
            "出站数据",
            "停机出站数据",
            "侧板出站数据",
            "进站数据"
        };

        private static readonly Regex DateDirectoryRegex = new Regex(
            @"(?<year>\d{4})年(?<month>\d{1,2})月(?<day>\d{1,2})日",
            RegexOptions.Compiled);

        private readonly object cacheSync = new object();
        private readonly string dataRoot;
        private readonly string cacheDirectory;

        public HistoryDataRepository(string programLogPath)
        {
            dataRoot = string.IsNullOrWhiteSpace(programLogPath)
                ? AppDomain.CurrentDomain.BaseDirectory
                : Path.GetFullPath(programLogPath);
            cacheDirectory = Path.Combine(dataRoot, "历史统计", "索引");
        }

        public string DataRoot { get { return dataRoot; } }
        public string ReportDirectory { get { return Path.Combine(dataRoot, "历史报表"); } }

        public HistoryQueryResult Query(
            HistoryFilter filter,
            IProgress<string> progress,
            CancellationToken cancellationToken)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            DateTime startTime = filter.StartTime;
            DateTime endTime = filter.EndTime;
            if (endTime < startTime)
            {
                throw new ArgumentException("结束时间不能早于开始时间。");
            }

            List<HistoryRecord> records = new List<HistoryRecord>();
            lock (cacheSync)
            {
                List<DateTime> months = GetMonths(startTime, endTime);
                DateTime firstMonth = new DateTime(startTime.Year, startTime.Month, 1);
                DateTime monthAfterLast = new DateTime(endTime.Year, endTime.Month, 1).AddMonths(1);
                List<SourceFileInfo> allSourceFiles = FindSourceFiles(firstMonth, monthAfterLast, cancellationToken);
                for (int index = 0; index < months.Count; index++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    DateTime month = months[index];
                    if (progress != null)
                    {
                        progress.Report(string.Format(
                            "正在加载 {0:yyyy年MM月}（{1}/{2}）...",
                            month,
                            index + 1,
                            months.Count));
                    }

                    List<SourceFileInfo> sourceFiles = allSourceFiles.Where(item =>
                        item.FolderDate.Year == month.Year && item.FolderDate.Month == month.Month).ToList();
                    records.AddRange(LoadOrRebuildMonthCache(month, sourceFiles, cancellationToken));
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            IEnumerable<HistoryRecord> query = records.Where(item =>
                item.CollectTime >= startTime && item.CollectTime <= endTime);
            if (!string.IsNullOrWhiteSpace(filter.SourceType) && filter.SourceType != "全部采集数据")
            {
                query = query.Where(item => string.Equals(
                    item.SourceType,
                    filter.SourceType,
                    StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Barcode))
            {
                string barcode = filter.Barcode.Trim();
                query = query.Where(item => item.Barcode.IndexOf(barcode, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            List<HistoryRecord> filteredRecords = query
                .OrderBy(item => item.CollectTime)
                .ThenBy(item => item.SourceFile, StringComparer.OrdinalIgnoreCase)
                .ToList();

            HistoryQueryResult result = new HistoryQueryResult
            {
                Filter = filter,
                Records = filteredRecords,
                NumericParameters = FindNumericParameters(filteredRecords)
            };
            if (progress != null)
            {
                progress.Report(string.Format("查询完成，共 {0} 条记录。", result.TotalCount));
            }

            return result;
        }

        private List<SourceFileInfo> FindSourceFiles(
            DateTime periodStart,
            DateTime periodEnd,
            CancellationToken cancellationToken)
        {
            List<SourceFileInfo> files = new List<SourceFileInfo>();

            foreach (string sourceDirectoryName in SourceDirectories)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string sourceDirectory = Path.Combine(dataRoot, sourceDirectoryName);
                if (!Directory.Exists(sourceDirectory))
                {
                    continue;
                }

                IEnumerable<string> candidateFiles;
                try
                {
                    candidateFiles = Directory.EnumerateFiles(sourceDirectory, "*.CSV", SearchOption.AllDirectories);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string filePath in candidateFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    DateTime fileDate;
                    if (!TryGetDateFromPath(filePath, out fileDate))
                    {
                        fileDate = File.GetLastWriteTime(filePath);
                    }

                    if (fileDate < periodStart || fileDate >= periodEnd)
                    {
                        continue;
                    }

                    try
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        files.Add(new SourceFileInfo
                        {
                            SourceType = sourceDirectoryName,
                            FullPath = fileInfo.FullName,
                            FolderDate = fileDate.Date,
                            Length = fileInfo.Length,
                            LastWriteTicks = fileInfo.LastWriteTimeUtc.Ticks
                        });
                    }
                    catch (FileNotFoundException)
                    {
                        // 采集线程刚好正在生成或替换文件，下次查询会自动补入。
                    }
                }
            }

            return files.OrderBy(item => item.FullPath, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private List<HistoryRecord> LoadOrRebuildMonthCache(
            DateTime month,
            List<SourceFileInfo> sourceFiles,
            CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(cacheDirectory);
            string cacheBaseName = month.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            string cachePath = Path.Combine(cacheDirectory, cacheBaseName + ".hidx");
            string signaturePath = Path.Combine(cacheDirectory, cacheBaseName + ".sig");
            string currentSignature = CreateSignature(sourceFiles);

            if (File.Exists(cachePath) && File.Exists(signaturePath))
            {
                try
                {
                    string savedSignature = File.ReadAllText(signaturePath, Encoding.ASCII);
                    if (string.Equals(savedSignature, currentSignature, StringComparison.Ordinal))
                    {
                        return ReadCache(cachePath, cancellationToken);
                    }
                }
                catch (IOException)
                {
                    // 缓存读取失败时从原始CSV重建，不影响原始数据。
                }
                catch (InvalidDataException)
                {
                    // 缓存格式无效时自动重建。
                }
            }

            List<HistoryRecord> rebuiltRecords = new List<HistoryRecord>();
            bool allFilesReadSuccessfully = true;
            foreach (SourceFileInfo sourceFile in sourceFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                bool readSucceeded;
                rebuiltRecords.AddRange(ReadSourceCsv(sourceFile, out readSucceeded));
                allFilesReadSuccessfully &= readSucceeded;
            }

            rebuiltRecords = rebuiltRecords
                .OrderBy(item => item.CollectTime)
                .ThenBy(item => item.SourceFile, StringComparer.OrdinalIgnoreCase)
                .ToList();
            WriteCache(cachePath, rebuiltRecords);
            if (allFilesReadSuccessfully)
            {
                File.WriteAllText(signaturePath, currentSignature, Encoding.ASCII);
            }
            else if (File.Exists(signaturePath))
            {
                File.Delete(signaturePath);
            }
            return rebuiltRecords;
        }

        private static IEnumerable<HistoryRecord> ReadSourceCsv(SourceFileInfo sourceFile, out bool readSucceeded)
        {
            List<HistoryRecord> records = new List<HistoryRecord>();
            readSucceeded = false;
            try
            {
                using (FileStream stream = new FileStream(
                    sourceFile.FullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete))
                using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
                {
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                    {
                        return records;
                    }

                    List<string> headers = ParseCsvLine(headerLine.Replace("\t", string.Empty));
                    string dataLine;
                    while ((dataLine = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(dataLine))
                        {
                            continue;
                        }

                        List<string> values = ParseCsvLine(dataLine.Replace("\t", string.Empty));
                        HistoryRecord record = new HistoryRecord
                        {
                            SourceType = sourceFile.SourceType,
                            SourceFile = sourceFile.FullPath,
                            CollectTime = sourceFile.FolderDate
                        };

                        for (int index = 0; index < headers.Count; index++)
                        {
                            string header = (headers[index] ?? string.Empty).Trim();
                            if (string.IsNullOrWhiteSpace(header))
                            {
                                header = "未命名字段" + (index + 1).ToString(CultureInfo.InvariantCulture);
                            }

                            string value = index < values.Count ? values[index] : string.Empty;
                            record.Fields[header] = value == null ? string.Empty : value.Trim();
                        }

                        DateTime collectTime;
                        if (TryFindCollectTime(record, out collectTime))
                        {
                            record.CollectTime = collectTime;
                        }
                        else
                        {
                            record.CollectTime = sourceFile.FolderDate.Date.Add(sourceFile.LastWriteTime.TimeOfDay);
                        }

                        records.Add(record);
                    }
                }
                readSucceeded = true;
            }
            catch (IOException)
            {
                // 文件正在由采集线程写入时，本次跳过；文件签名变化后下次会自动重建。
            }
            catch (UnauthorizedAccessException)
            {
                // 单个不可访问文件不阻断其他历史数据查询。
            }

            return records;
        }

        private static bool TryFindCollectTime(HistoryRecord record, out DateTime collectTime)
        {
            collectTime = DateTime.MinValue;
            string rawTime = record.FindValue("时间", "time", "采集时间");
            if (string.IsNullOrWhiteSpace(rawTime))
            {
                return false;
            }

            string[] formats =
            {
                "yyyy年MM月dd日 HH:mm:ss",
                "yyyy年M月d日 H:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy/M/d H:mm:ss",
                "yyyy-MM-ddTHH:mm:ss"
            };
            return DateTime.TryParseExact(
                       rawTime.Trim(),
                       formats,
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.AllowWhiteSpaces,
                       out collectTime) ||
                   DateTime.TryParse(rawTime.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out collectTime);
        }

        private static List<string> FindNumericParameters(List<HistoryRecord> records)
        {
            HashSet<string> excludedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "模组码", "条码", "SFC", "结果", "班次", "设备标识", "设备标识号", "是否首件", "时间"
            };
            Dictionary<string, int> numericCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (HistoryRecord record in records)
            {
                foreach (KeyValuePair<string, string> field in record.Fields)
                {
                    string normalizedHeader = field.Key.Split('/')[0].Trim();
                    if (excludedFields.Contains(normalizedHeader))
                    {
                        continue;
                    }

                    double numericValue;
                    if (!HistoryNumberParser.TryParse(field.Value, out numericValue))
                    {
                        continue;
                    }

                    int count;
                    numericCounts.TryGetValue(field.Key, out count);
                    numericCounts[field.Key] = count + 1;
                }
            }

            return numericCounts
                .Where(item => item.Value > 0)
                .OrderByDescending(item => item.Value)
                .ThenBy(item => item.Key, StringComparer.CurrentCulture)
                .Select(item => item.Key)
                .ToList();
        }

        private static void WriteCache(string cachePath, List<HistoryRecord> records)
        {
            string temporaryPath = cachePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (FileStream stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    writer.Write(CacheMagic);
                    writer.Write(records.Count);
                    foreach (HistoryRecord record in records)
                    {
                        writer.Write(record.SourceType ?? string.Empty);
                        writer.Write(record.SourceFile ?? string.Empty);
                        writer.Write(record.CollectTime.Ticks);
                        writer.Write(record.Fields.Count);
                        foreach (KeyValuePair<string, string> field in record.Fields)
                        {
                            writer.Write(field.Key ?? string.Empty);
                            writer.Write(field.Value ?? string.Empty);
                        }
                    }
                }

                File.Copy(temporaryPath, cachePath, true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static List<HistoryRecord> ReadCache(string cachePath, CancellationToken cancellationToken)
        {
            List<HistoryRecord> records = new List<HistoryRecord>();
            using (FileStream stream = new FileStream(cachePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
            {
                string magic = reader.ReadString();
                if (!string.Equals(magic, CacheMagic, StringComparison.Ordinal))
                {
                    throw new InvalidDataException("历史数据缓存版本不匹配。");
                }

                int recordCount = reader.ReadInt32();
                if (recordCount < 0 || recordCount > 10000000)
                {
                    throw new InvalidDataException("历史数据缓存记录数无效。");
                }

                for (int recordIndex = 0; recordIndex < recordCount; recordIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    HistoryRecord record = new HistoryRecord
                    {
                        SourceType = reader.ReadString(),
                        SourceFile = reader.ReadString(),
                        CollectTime = new DateTime(reader.ReadInt64())
                    };
                    int fieldCount = reader.ReadInt32();
                    if (fieldCount < 0 || fieldCount > 10000)
                    {
                        throw new InvalidDataException("历史数据缓存字段数无效。");
                    }

                    for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
                    {
                        record.Fields[reader.ReadString()] = reader.ReadString();
                    }

                    records.Add(record);
                }
            }

            return records;
        }

        private static string CreateSignature(List<SourceFileInfo> sourceFiles)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                StringBuilder signatureInput = new StringBuilder(CacheMagic);
                foreach (SourceFileInfo sourceFile in sourceFiles)
                {
                    signatureInput
                        .Append('|')
                        .Append(sourceFile.SourceType)
                        .Append('|')
                        .Append(sourceFile.FullPath.ToUpperInvariant())
                        .Append('|')
                        .Append(sourceFile.Length.ToString(CultureInfo.InvariantCulture))
                        .Append('|')
                        .Append(sourceFile.LastWriteTicks.ToString(CultureInfo.InvariantCulture));
                }

                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(signatureInput.ToString()));
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        private static List<DateTime> GetMonths(DateTime startTime, DateTime endTime)
        {
            List<DateTime> months = new List<DateTime>();
            DateTime month = new DateTime(startTime.Year, startTime.Month, 1);
            DateTime lastMonth = new DateTime(endTime.Year, endTime.Month, 1);
            while (month <= lastMonth)
            {
                months.Add(month);
                month = month.AddMonths(1);
            }

            return months;
        }

        private static bool TryGetDateFromPath(string filePath, out DateTime date)
        {
            date = DateTime.MinValue;
            Match match = DateDirectoryRegex.Match(filePath);
            int year;
            int month;
            int day;
            if (!match.Success ||
                !int.TryParse(match.Groups["year"].Value, out year) ||
                !int.TryParse(match.Groups["month"].Value, out month) ||
                !int.TryParse(match.Groups["day"].Value, out day))
            {
                return false;
            }

            try
            {
                date = new DateTime(year, month, day);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        private static List<string> ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder field = new StringBuilder();
            bool inQuotes = false;
            for (int index = 0; index < line.Length; index++)
            {
                char character = line[index];
                if (character == '"')
                {
                    if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (character == ',' && !inQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(character);
                }
            }

            fields.Add(field.ToString());
            return fields;
        }

        private sealed class SourceFileInfo
        {
            public string SourceType { get; set; }
            public string FullPath { get; set; }
            public DateTime FolderDate { get; set; }
            public long Length { get; set; }
            public long LastWriteTicks { get; set; }
            public DateTime LastWriteTime { get { return new DateTime(LastWriteTicks, DateTimeKind.Utc).ToLocalTime(); } }
        }
    }
}
