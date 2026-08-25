using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;

namespace BJTSurrenSystem.History
{
    internal static class HistoryExcelExporter
    {
        private const int ExcelMaximumRows = 1048576;

        public static void Export(
            string filePath,
            HistoryQueryResult result,
            string parameterName,
            HistoryParameterLimit parameterLimit)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            if (result.Records.Count + 1 > ExcelMaximumRows)
            {
                throw new InvalidOperationException("明细数据超过Excel单表最大行数，请缩小查询时间后再导出。");
            }

            string directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
            Directory.CreateDirectory(directory);
            string temporaryPath = filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            List<HistoryBucket> buckets = HistoryAggregator.CreateBuckets(result, parameterName);
            HistoryParameterStatistics selectedStatistics = HistoryAggregator.CreateParameterStatistics(
                result,
                parameterName,
                parameterLimit);
            bool hasParameterChart = !string.IsNullOrWhiteSpace(parameterName) &&
                                     buckets.Any(item => item.HasParameterValue);

            try
            {
                using (FileStream stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, false, Encoding.UTF8))
                {
                    WriteEntry(archive, "[Content_Types].xml", CreateContentTypes(hasParameterChart));
                    WriteEntry(archive, "_rels/.rels", CreateRootRelationships());
                    WriteEntry(archive, "docProps/core.xml", CreateCoreProperties());
                    WriteEntry(archive, "docProps/app.xml", CreateApplicationProperties());
                    WriteEntry(archive, "xl/workbook.xml", CreateWorkbook());
                    WriteEntry(archive, "xl/_rels/workbook.xml.rels", CreateWorkbookRelationships());
                    WriteEntry(archive, "xl/styles.xml", CreateStyles());
                    WriteEntry(archive, "xl/worksheets/sheet1.xml", CreateSummarySheet(result, buckets, selectedStatistics, parameterName));
                    WriteEntry(archive, "xl/worksheets/_rels/sheet1.xml.rels", CreateSummarySheetRelationships());
                    WriteEntry(archive, "xl/worksheets/sheet2.xml", CreateParameterStatisticsSheet(result));
                    WriteEntry(archive, "xl/worksheets/sheet3.xml", CreateDetailsSheet(result));
                    WriteEntry(archive, "xl/worksheets/sheet4.xml", CreateFilterSheet(result, parameterName));
                    WriteEntry(archive, "xl/drawings/drawing1.xml", CreateDrawing(hasParameterChart));
                    WriteEntry(archive, "xl/drawings/_rels/drawing1.xml.rels", CreateDrawingRelationships(hasParameterChart));
                    WriteEntry(archive, "xl/charts/chart1.xml", CreateProductionChart(buckets));
                    if (hasParameterChart)
                    {
                        WriteEntry(archive, "xl/charts/chart2.xml", CreateParameterChart(buckets, parameterName));
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

        private static string CreateContentTypes(bool hasParameterChart)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(XmlHeader)
                .Append("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">")
                .Append("<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>")
                .Append("<Default Extension=\"xml\" ContentType=\"application/xml\"/>")
                .Append("<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>")
                .Append("<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>")
                .Append("<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>")
                .Append("<Override PartName=\"/xl/worksheets/sheet2.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>")
                .Append("<Override PartName=\"/xl/worksheets/sheet3.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>")
                .Append("<Override PartName=\"/xl/worksheets/sheet4.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>")
                .Append("<Override PartName=\"/xl/drawings/drawing1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.drawing+xml\"/>")
                .Append("<Override PartName=\"/xl/charts/chart1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.drawingml.chart+xml\"/>");
            if (hasParameterChart)
            {
                xml.Append("<Override PartName=\"/xl/charts/chart2.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.drawingml.chart+xml\"/>");
            }
            xml.Append("<Override PartName=\"/docProps/core.xml\" ContentType=\"application/vnd.openxmlformats-package.core-properties+xml\"/>")
                .Append("<Override PartName=\"/docProps/app.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.extended-properties+xml\"/>")
                .Append("</Types>");
            return xml.ToString();
        }

        private static string CreateRootRelationships()
        {
            return XmlHeader +
                   "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                   "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                   "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties\" Target=\"docProps/core.xml\"/>" +
                   "<Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties\" Target=\"docProps/app.xml\"/>" +
                   "</Relationships>";
        }

        private static string CreateCoreProperties()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            return XmlHeader +
                   "<cp:coreProperties xmlns:cp=\"http://schemas.openxmlformats.org/package/2006/metadata/core-properties\" " +
                   "xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:dcterms=\"http://purl.org/dc/terms/\" " +
                   "xmlns:dcmitype=\"http://purl.org/dc/dcmitype/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                   "<dc:title>PLC采集历史统计报表</dc:title><dc:creator>追溯软件</dc:creator>" +
                   "<cp:lastModifiedBy>追溯软件</cp:lastModifiedBy>" +
                   "<dcterms:created xsi:type=\"dcterms:W3CDTF\">" + timestamp + "</dcterms:created>" +
                   "<dcterms:modified xsi:type=\"dcterms:W3CDTF\">" + timestamp + "</dcterms:modified>" +
                   "</cp:coreProperties>";
        }

        private static string CreateApplicationProperties()
        {
            return XmlHeader +
                   "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\" " +
                   "xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">" +
                   "<Application>追溯软件</Application><AppVersion>1.1</AppVersion>" +
                   "</Properties>";
        }

        private static string CreateWorkbook()
        {
            return XmlHeader +
                   "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
                   "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   "<bookViews><workbookView/></bookViews><sheets>" +
                   "<sheet name=\"统计概览\" sheetId=\"1\" r:id=\"rId1\"/>" +
                   "<sheet name=\"参数统计\" sheetId=\"2\" r:id=\"rId2\"/>" +
                   "<sheet name=\"明细数据\" sheetId=\"3\" r:id=\"rId3\"/>" +
                   "<sheet name=\"查询条件\" sheetId=\"4\" r:id=\"rId4\"/>" +
                   "</sheets><calcPr calcId=\"191029\" fullCalcOnLoad=\"1\" forceFullCalc=\"1\"/>" +
                   "</workbook>";
        }

        private static string CreateWorkbookRelationships()
        {
            return XmlHeader +
                   "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                   "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                   "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet2.xml\"/>" +
                   "<Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet3.xml\"/>" +
                   "<Relationship Id=\"rId4\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet4.xml\"/>" +
                   "<Relationship Id=\"rId5\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>" +
                   "</Relationships>";
        }

        private static string CreateStyles()
        {
            return XmlHeader +
                   "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                   "<fonts count=\"4\">" +
                   "<font><sz val=\"11\"/><name val=\"Microsoft YaHei UI\"/></font>" +
                   "<font><b/><sz val=\"16\"/><color rgb=\"FF1E5A8A\"/><name val=\"Microsoft YaHei UI\"/></font>" +
                   "<font><b/><sz val=\"11\"/><color rgb=\"FFFFFFFF\"/><name val=\"Microsoft YaHei UI\"/></font>" +
                   "<font><b/><sz val=\"11\"/><name val=\"Microsoft YaHei UI\"/></font>" +
                   "</fonts>" +
                   "<fills count=\"5\">" +
                   "<fill><patternFill patternType=\"none\"/></fill>" +
                   "<fill><patternFill patternType=\"gray125\"/></fill>" +
                   "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FF1E70BF\"/><bgColor indexed=\"64\"/></patternFill></fill>" +
                   "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFE0F7E6\"/><bgColor indexed=\"64\"/></patternFill></fill>" +
                   "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFFFE0E0\"/><bgColor indexed=\"64\"/></patternFill></fill>" +
                   "</fills>" +
                   "<borders count=\"2\"><border/><border><left style=\"thin\"><color rgb=\"FFD0D5DA\"/></left>" +
                   "<right style=\"thin\"><color rgb=\"FFD0D5DA\"/></right><top style=\"thin\"><color rgb=\"FFD0D5DA\"/></top>" +
                   "<bottom style=\"thin\"><color rgb=\"FFD0D5DA\"/></bottom><diagonal/></border></borders>" +
                   "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                   "<cellXfs count=\"8\">" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyAlignment=\"1\"><alignment horizontal=\"center\"/></xf>" +
                   "<xf numFmtId=\"0\" fontId=\"2\" fillId=\"2\" borderId=\"1\" xfId=\"0\" applyAlignment=\"1\"><alignment horizontal=\"center\" vertical=\"center\" wrapText=\"1\"/></xf>" +
                   "<xf numFmtId=\"10\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyNumberFormat=\"1\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"3\" borderId=\"1\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"4\" borderId=\"1\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"3\" fillId=\"0\" borderId=\"1\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyAlignment=\"1\"><alignment vertical=\"center\" wrapText=\"1\"/></xf>" +
                   "</cellXfs>" +
                   "<cellStyles count=\"1\"><cellStyle name=\"常规\" xfId=\"0\" builtinId=\"0\"/></cellStyles>" +
                   "</styleSheet>";
        }

        private static string CreateSummarySheet(
            HistoryQueryResult result,
            List<HistoryBucket> buckets,
            HistoryParameterStatistics selectedStatistics,
            string parameterName)
        {
            List<List<ExcelCell>> rows = new List<List<ExcelCell>>();
            rows.Add(Row(TextCell("PLC采集历史统计报表", 1)));
            rows.Add(Row(TextCell("生成时间", 6), TextCell(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 7)));
            rows.Add(Row(TextCell("统计周期", 6), TextCell(result.Filter.StartTime.ToString("yyyy-MM-dd") + " 至 " + result.Filter.EndTime.ToString("yyyy-MM-dd"), 7)));
            rows.Add(Row(TextCell("数据类型", 6), TextCell(result.Filter.SourceType, 7)));
            rows.Add(Row(TextCell("条码条件", 6), TextCell(string.IsNullOrWhiteSpace(result.Filter.Barcode) ? "全部" : result.Filter.Barcode, 7)));
            rows.Add(Row(TextCell("所选PLC参数", 6), TextCell(string.IsNullOrWhiteSpace(parameterName) ? "未选择" : parameterName, 7)));
            rows.Add(new List<ExcelCell>());
            rows.Add(Row(TextCell("统计指标", 2), TextCell("统计值", 2)));
            rows.Add(Row(TextCell("采集总数", 6), NumberCell(result.TotalCount, 7)));
            rows.Add(Row(TextCell("OK数量", 6), NumberCell(result.OkCount, 4)));
            rows.Add(Row(TextCell("NG数量", 6), NumberCell(result.NgCount, 5)));
            rows.Add(Row(TextCell("合格率", 6), NumberCell(result.YieldRate / 100D, 3)));
            rows.Add(Row(TextCell("参数有效值数", 6), NumberCell(selectedStatistics.ValueCount, 7)));
            rows.Add(Row(TextCell("参数平均值", 6), selectedStatistics.ValueCount > 0 ? NumberCell(selectedStatistics.Average, 7) : TextCell("--", 7)));
            rows.Add(Row(TextCell("参数最小值", 6), selectedStatistics.ValueCount > 0 ? NumberCell(selectedStatistics.Minimum, 7) : TextCell("--", 7)));
            rows.Add(Row(TextCell("参数最大值", 6), selectedStatistics.ValueCount > 0 ? NumberCell(selectedStatistics.Maximum, 7) : TextCell("--", 7)));
            rows.Add(Row(TextCell("参数超限数", 6), NumberCell(selectedStatistics.OutOfLimitCount, 7)));
            rows.Add(new List<ExcelCell>());
            rows.Add(Row(
                TextCell("周期", 2), TextCell("总数", 2), TextCell("OK", 2), TextCell("NG", 2),
                TextCell("合格率", 2), TextCell("参数平均值", 2), TextCell("参数最小值", 2), TextCell("参数最大值", 2)));
            foreach (HistoryBucket bucket in buckets)
            {
                rows.Add(Row(
                    TextCell(bucket.Label, 7),
                    NumberCell(bucket.TotalCount, 7),
                    NumberCell(bucket.OkCount, 4),
                    NumberCell(bucket.NgCount, 5),
                    NumberCell(bucket.TotalCount == 0 ? 0D : bucket.OkCount / (double)bucket.TotalCount, 3),
                    bucket.HasParameterValue ? NumberCell(bucket.ParameterAverage, 7) : TextCell(string.Empty, 7),
                    bucket.HasParameterValue ? NumberCell(bucket.ParameterMinimum, 7) : TextCell(string.Empty, 7),
                    bucket.HasParameterValue ? NumberCell(bucket.ParameterMaximum, 7) : TextCell(string.Empty, 7)));
            }

            return CreateWorksheet(
                rows,
                new[] { 22D, 18D, 14D, 14D, 14D, 18D, 18D, 18D },
                "A1:H1",
                true,
                19);
        }

        private static string CreateParameterStatisticsSheet(HistoryQueryResult result)
        {
            List<List<ExcelCell>> rows = new List<List<ExcelCell>>
            {
                Row(TextCell("PLC参数统计", 1)),
                Row(TextCell("参数名称", 2), TextCell("有效值数", 2), TextCell("平均值", 2), TextCell("最小值", 2), TextCell("最大值", 2))
            };
            foreach (string parameter in result.NumericParameters)
            {
                HistoryParameterStatistics statistics = HistoryAggregator.CreateParameterStatistics(result, parameter, null);
                rows.Add(Row(
                    TextCell(parameter, 7), NumberCell(statistics.ValueCount, 7), NumberCell(statistics.Average, 7),
                    NumberCell(statistics.Minimum, 7), NumberCell(statistics.Maximum, 7)));
            }
            return CreateWorksheet(rows, new[] { 42D, 14D, 18D, 18D, 18D }, "A1:E1", false, 2);
        }

        private static string CreateDetailsSheet(HistoryQueryResult result)
        {
            List<string> fieldNames = result.Records
                .SelectMany(item => item.Fields.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item, StringComparer.CurrentCulture)
                .ToList();
            List<List<ExcelCell>> rows = new List<List<ExcelCell>>();
            List<ExcelCell> header = new List<ExcelCell>
            {
                TextCell("序号", 2), TextCell("采集时间", 2), TextCell("数据类型", 2),
                TextCell("条码/模组码", 2), TextCell("结果", 2), TextCell("原始CSV", 2)
            };
            header.AddRange(fieldNames.Select(item => TextCell(item, 2)));
            rows.Add(header);

            for (int index = 0; index < result.Records.Count; index++)
            {
                HistoryRecord record = result.Records[index];
                int rowStyle = record.Result == "NG" ? 5 : record.Result == "OK" ? 4 : 7;
                List<ExcelCell> row = new List<ExcelCell>
                {
                    NumberCell(index + 1, rowStyle),
                    TextCell(record.CollectTime.ToString("yyyy-MM-dd HH:mm:ss"), rowStyle),
                    TextCell(record.SourceType, rowStyle),
                    TextCell(record.Barcode, rowStyle),
                    TextCell(record.Result, rowStyle),
                    TextCell(record.SourceFile, rowStyle)
                };
                foreach (string fieldName in fieldNames)
                {
                    string value;
                    record.Fields.TryGetValue(fieldName, out value);
                    // 明细全部按文本写入，确保条码和长数字不被Excel改为科学计数法。
                    row.Add(TextCell(value ?? string.Empty, rowStyle));
                }
                rows.Add(row);
            }

            double[] widths = Enumerable.Repeat(16D, 6 + fieldNames.Count).ToArray();
            widths[0] = 10D;
            widths[1] = 21D;
            widths[2] = 16D;
            widths[3] = 28D;
            widths[4] = 10D;
            widths[5] = 55D;
            return CreateWorksheet(rows, widths, null, false, 1, true);
        }

        private static string CreateFilterSheet(HistoryQueryResult result, string parameterName)
        {
            List<List<ExcelCell>> rows = new List<List<ExcelCell>>
            {
                Row(TextCell("报表查询条件", 1)),
                Row(TextCell("开始时间", 6), TextCell(result.Filter.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), 7)),
                Row(TextCell("结束时间", 6), TextCell(result.Filter.EndTime.ToString("yyyy-MM-dd HH:mm:ss"), 7)),
                Row(TextCell("数据类型", 6), TextCell(result.Filter.SourceType, 7)),
                Row(TextCell("条码条件", 6), TextCell(string.IsNullOrWhiteSpace(result.Filter.Barcode) ? "全部" : result.Filter.Barcode, 7)),
                Row(TextCell("PLC参数", 6), TextCell(string.IsNullOrWhiteSpace(parameterName) ? "未选择" : parameterName, 7)),
                Row(TextCell("记录总数", 6), NumberCell(result.TotalCount, 7)),
                Row(TextCell("说明", 6), TextCell("原始CSV保持不变；本报表由历史数据页按当前查询条件生成。", 7))
            };
            return CreateWorksheet(rows, new[] { 22D, 78D }, "A1:B1", false, 2);
        }

        private static string CreateWorksheet(
            List<List<ExcelCell>> rows,
            double[] columnWidths,
            string mergedRange,
            bool hasDrawing,
            int frozenRows,
            bool autoFilter = false)
        {
            int maximumColumns = rows.Count == 0 ? 1 : Math.Max(1, rows.Max(item => item.Count));
            string lastCell = GetColumnName(maximumColumns) + Math.Max(1, rows.Count).ToString(CultureInfo.InvariantCulture);
            StringBuilder xml = new StringBuilder();
            xml.Append(XmlHeader)
                .Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">")
                .Append("<dimension ref=\"A1:").Append(lastCell).Append("\"/>")
                .Append("<sheetViews><sheetView workbookViewId=\"0\">");
            if (frozenRows > 0)
            {
                xml.Append("<pane ySplit=\"").Append(frozenRows.ToString(CultureInfo.InvariantCulture))
                    .Append("\" topLeftCell=\"A").Append((frozenRows + 1).ToString(CultureInfo.InvariantCulture))
                    .Append("\" activePane=\"bottomLeft\" state=\"frozen\"/>");
            }
            xml.Append("</sheetView></sheetViews><sheetFormatPr defaultRowHeight=\"18\"/>")
                .Append("<cols>");
            for (int index = 0; index < columnWidths.Length; index++)
            {
                xml.Append("<col min=\"").Append(index + 1).Append("\" max=\"").Append(index + 1)
                    .Append("\" width=\"").Append(columnWidths[index].ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("\" customWidth=\"1\"/>");
            }
            xml.Append("</cols><sheetData>");
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                int excelRow = rowIndex + 1;
                xml.Append("<row r=\"").Append(excelRow).Append("\">");
                for (int columnIndex = 0; columnIndex < rows[rowIndex].Count; columnIndex++)
                {
                    AppendCell(xml, GetColumnName(columnIndex + 1) + excelRow.ToString(CultureInfo.InvariantCulture), rows[rowIndex][columnIndex]);
                }
                xml.Append("</row>");
            }
            xml.Append("</sheetData>");
            if (!string.IsNullOrWhiteSpace(mergedRange))
            {
                xml.Append("<mergeCells count=\"1\"><mergeCell ref=\"").Append(mergedRange).Append("\"/></mergeCells>");
            }
            if (autoFilter && rows.Count > 1)
            {
                xml.Append("<autoFilter ref=\"A1:").Append(GetColumnName(maximumColumns)).Append(rows.Count).Append("\"/>");
            }
            if (hasDrawing)
            {
                xml.Append("<drawing r:id=\"rId1\"/>");
            }
            xml.Append("</worksheet>");
            return xml.ToString();
        }

        private static string CreateSummarySheetRelationships()
        {
            return XmlHeader +
                   "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                   "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing\" Target=\"../drawings/drawing1.xml\"/>" +
                   "</Relationships>";
        }

        private static string CreateDrawing(bool hasParameterChart)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append(XmlHeader)
                .Append("<xdr:wsDr xmlns:xdr=\"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing\" ")
                .Append("xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" ")
                .Append("xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\" ")
                .Append("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">")
                .Append(CreateChartAnchor(9, 1, 18, 17, 2, "采集数量趋势图", "rId1"));
            if (hasParameterChart)
            {
                xml.Append(CreateChartAnchor(9, 18, 18, 34, 3, "PLC参数趋势图", "rId2"));
            }
            xml.Append("</xdr:wsDr>");
            return xml.ToString();
        }

        private static string CreateChartAnchor(
            int fromColumn,
            int fromRow,
            int toColumn,
            int toRow,
            int id,
            string name,
            string relationshipId)
        {
            return "<xdr:twoCellAnchor>" +
                   "<xdr:from><xdr:col>" + fromColumn + "</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>" + fromRow + "</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:from>" +
                   "<xdr:to><xdr:col>" + toColumn + "</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>" + toRow + "</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:to>" +
                   "<xdr:graphicFrame macro=\"\"><xdr:nvGraphicFramePr><xdr:cNvPr id=\"" + id + "\" name=\"" + Escape(name) + "\"/><xdr:cNvGraphicFramePr/></xdr:nvGraphicFramePr>" +
                   "<xdr:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"0\" cy=\"0\"/></xdr:xfrm>" +
                   "<a:graphic><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/chart\"><c:chart r:id=\"" + relationshipId + "\"/></a:graphicData></a:graphic>" +
                   "</xdr:graphicFrame><xdr:clientData/></xdr:twoCellAnchor>";
        }

        private static string CreateDrawingRelationships(bool hasParameterChart)
        {
            string relationships = XmlHeader +
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart\" Target=\"../charts/chart1.xml\"/>";
            if (hasParameterChart)
            {
                relationships += "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart\" Target=\"../charts/chart2.xml\"/>";
            }
            return relationships + "</Relationships>";
        }

        private static string CreateProductionChart(List<HistoryBucket> buckets)
        {
            int firstDataRow = 20;
            int lastDataRow = Math.Max(firstDataRow, firstDataRow + buckets.Count - 1);
            string[] names = { "总数", "OK", "NG" };
            string[] columns = { "B", "C", "D" };
            StringBuilder series = new StringBuilder();
            for (int index = 0; index < names.Length; index++)
            {
                series.Append(CreateChartSeries(
                    index,
                    names[index],
                    "'统计概览'!$A$" + firstDataRow + ":$A$" + lastDataRow,
                    "'统计概览'!$" + columns[index] + "$" + firstDataRow + ":$" + columns[index] + "$" + lastDataRow));
            }
            return CreateChartSpace("采集数量及OK/NG趋势", "barChart", series.ToString(), true);
        }

        private static string CreateParameterChart(List<HistoryBucket> buckets, string parameterName)
        {
            int firstDataRow = 20;
            int lastDataRow = Math.Max(firstDataRow, firstDataRow + buckets.Count - 1);
            string[] names = { "平均值", "最小值", "最大值" };
            string[] columns = { "F", "G", "H" };
            StringBuilder series = new StringBuilder();
            for (int index = 0; index < names.Length; index++)
            {
                series.Append(CreateChartSeries(
                    index,
                    names[index],
                    "'统计概览'!$A$" + firstDataRow + ":$A$" + lastDataRow,
                    "'统计概览'!$" + columns[index] + "$" + firstDataRow + ":$" + columns[index] + "$" + lastDataRow));
            }
            return CreateChartSpace(parameterName + " 趋势", "lineChart", series.ToString(), false);
        }

        private static string CreateChartSpace(string title, string chartElement, string seriesXml, bool isBarChart)
        {
            string chartProperties = isBarChart
                ? "<c:barDir val=\"col\"/><c:grouping val=\"clustered\"/><c:varyColors val=\"0\"/>"
                : "<c:grouping val=\"standard\"/><c:varyColors val=\"0\"/>";
            return XmlHeader +
                   "<c:chartSpace xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   "<c:chart><c:title><c:tx><c:rich><a:bodyPr/><a:lstStyle/><a:p><a:r><a:rPr lang=\"zh-CN\"/><a:t>" + Escape(title) + "</a:t></a:r></a:p></c:rich></c:tx><c:layout/><c:overlay val=\"0\"/></c:title>" +
                   "<c:autoTitleDeleted val=\"0\"/><c:plotArea><c:layout/><c:" + chartElement + ">" +
                   chartProperties + seriesXml + "<c:axId val=\"48650112\"/><c:axId val=\"48672768\"/></c:" + chartElement + ">" +
                   "<c:catAx><c:axId val=\"48650112\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"b\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"48672768\"/><c:crosses val=\"autoZero\"/><c:auto val=\"1\"/><c:lblAlgn val=\"ctr\"/><c:lblOffset val=\"100\"/></c:catAx>" +
                   "<c:valAx><c:axId val=\"48672768\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"l\"/><c:majorGridlines/><c:numFmt formatCode=\"General\" sourceLinked=\"1\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"48650112\"/><c:crosses val=\"autoZero\"/><c:crossBetween val=\"between\"/></c:valAx>" +
                   "</c:plotArea><c:legend><c:legendPos val=\"b\"/><c:layout/><c:overlay val=\"0\"/></c:legend><c:plotVisOnly val=\"1\"/><c:dispBlanksAs val=\"gap\"/></c:chart>" +
                   "<c:printSettings><c:headerFooter/><c:pageMargins b=\"0.75\" l=\"0.7\" r=\"0.7\" t=\"0.75\" header=\"0.3\" footer=\"0.3\"/><c:pageSetup/></c:printSettings></c:chartSpace>";
        }

        private static string CreateChartSeries(int index, string name, string categoryFormula, string valueFormula)
        {
            return "<c:ser><c:idx val=\"" + index + "\"/><c:order val=\"" + index + "\"/>" +
                   "<c:tx><c:v>" + Escape(name) + "</c:v></c:tx>" +
                   "<c:cat><c:strRef><c:f>" + Escape(categoryFormula) + "</c:f></c:strRef></c:cat>" +
                   "<c:val><c:numRef><c:f>" + Escape(valueFormula) + "</c:f></c:numRef></c:val>" +
                   "</c:ser>";
        }

        private static void AppendCell(StringBuilder xml, string reference, ExcelCell cell)
        {
            xml.Append("<c r=\"").Append(reference).Append("\" s=\"").Append(cell.StyleIndex).Append("\"");
            if (cell.IsNumber)
            {
                xml.Append("><v>").Append(cell.Value).Append("</v></c>");
            }
            else
            {
                xml.Append(" t=\"inlineStr\"><is><t xml:space=\"preserve\">")
                    .Append(Escape(TrimExcelText(cell.Value)))
                    .Append("</t></is></c>");
            }
        }

        private static ExcelCell TextCell(string value, int styleIndex)
        {
            return new ExcelCell { Value = value ?? string.Empty, StyleIndex = styleIndex, IsNumber = false };
        }

        private static ExcelCell NumberCell(double value, int styleIndex)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return TextCell(string.Empty, styleIndex);
            }
            return new ExcelCell
            {
                Value = value.ToString("0.###############", CultureInfo.InvariantCulture),
                StyleIndex = styleIndex,
                IsNumber = true
            };
        }

        private static List<ExcelCell> Row(params ExcelCell[] cells)
        {
            return cells.ToList();
        }

        private static string GetColumnName(int columnNumber)
        {
            StringBuilder columnName = new StringBuilder();
            int number = columnNumber;
            while (number > 0)
            {
                number--;
                columnName.Insert(0, (char)('A' + number % 26));
                number /= 26;
            }
            return columnName.ToString();
        }

        private static string Escape(string value)
        {
            string sanitized = SanitizeXmlText(value ?? string.Empty);
            return SecurityElement.Escape(sanitized) ?? string.Empty;
        }

        private static string SanitizeXmlText(string value)
        {
            StringBuilder builder = new StringBuilder(value.Length);
            foreach (char character in value)
            {
                if (character == '\t' || character == '\n' || character == '\r' || character >= 0x20)
                {
                    builder.Append(character);
                }
            }
            return builder.ToString();
        }

        private static string TrimExcelText(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= 32767)
            {
                return value ?? string.Empty;
            }
            return value.Substring(0, 32760) + "...(截断)";
        }

        private static void WriteEntry(ZipArchive archive, string entryName, string content)
        {
            ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            using (Stream stream = entry.Open())
            using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                writer.Write(content);
            }
        }

        private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";

        private sealed class ExcelCell
        {
            public string Value { get; set; }
            public int StyleIndex { get; set; }
            public bool IsNumber { get; set; }
        }
    }
}
