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
    internal static class CpkExcelExporter
    {
        private const int ExcelMaximumRows = 1048576;
        private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";

        public static void Export(
            string filePath,
            HistoryFilter filter,
            CpkCalculationResult calculation)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
            if (calculation.SampleCount + 1 > ExcelMaximumRows)
            {
                throw new InvalidOperationException("样本数量超过Excel单表最大行数，请缩小查询时间后再导出。");
            }

            string directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
            Directory.CreateDirectory(directory);
            string temporaryPath = filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            HistogramData histogram = CreateHistogram(calculation.Samples);
            int trendHeaderRow;
            string overviewSheet = CreateOverviewSheet(filter, calculation, out trendHeaderRow);
            try
            {
                using (FileStream stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, false, Encoding.UTF8))
                {
                    WriteEntry(archive, "[Content_Types].xml", CreateContentTypes());
                    WriteEntry(archive, "_rels/.rels", CreateRootRelationships());
                    WriteEntry(archive, "docProps/core.xml", CreateCoreProperties());
                    WriteEntry(archive, "docProps/app.xml", CreateApplicationProperties());
                    WriteEntry(archive, "xl/workbook.xml", CreateWorkbook());
                    WriteEntry(archive, "xl/_rels/workbook.xml.rels", CreateWorkbookRelationships());
                    WriteEntry(archive, "xl/styles.xml", CreateStyles());
                    WriteEntry(archive, "xl/worksheets/sheet1.xml", overviewSheet);
                    WriteEntry(archive, "xl/worksheets/_rels/sheet1.xml.rels", CreateOverviewRelationships());
                    WriteEntry(archive, "xl/worksheets/sheet2.xml", CreateSamplesSheet(calculation));
                    WriteEntry(archive, "xl/worksheets/sheet3.xml", CreateHistogramSheet(histogram));
                    WriteEntry(archive, "xl/drawings/drawing1.xml", CreateDrawing());
                    WriteEntry(archive, "xl/drawings/_rels/drawing1.xml.rels", CreateDrawingRelationships());
                    WriteEntry(archive, "xl/charts/chart1.xml", CreateTrendChart(calculation, trendHeaderRow));
                    WriteEntry(archive, "xl/charts/chart2.xml", CreateHistogramChart(histogram));
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

        private static string CreateContentTypes()
        {
            return XmlHeader +
                   "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                   "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                   "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                   "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                   "<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>" +
                   "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                   "<Override PartName=\"/xl/worksheets/sheet2.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                   "<Override PartName=\"/xl/worksheets/sheet3.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                   "<Override PartName=\"/xl/drawings/drawing1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.drawing+xml\"/>" +
                   "<Override PartName=\"/xl/charts/chart1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.drawingml.chart+xml\"/>" +
                   "<Override PartName=\"/xl/charts/chart2.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.drawingml.chart+xml\"/>" +
                   "<Override PartName=\"/docProps/core.xml\" ContentType=\"application/vnd.openxmlformats-package.core-properties+xml\"/>" +
                   "<Override PartName=\"/docProps/app.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.extended-properties+xml\"/>" +
                   "</Types>";
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
                   "<dc:title>涂胶CPK分析报表</dc:title><dc:creator>上位机软件</dc:creator>" +
                   "<cp:lastModifiedBy>上位机软件</cp:lastModifiedBy>" +
                   "<dcterms:created xsi:type=\"dcterms:W3CDTF\">" + timestamp + "</dcterms:created>" +
                   "<dcterms:modified xsi:type=\"dcterms:W3CDTF\">" + timestamp + "</dcterms:modified>" +
                   "</cp:coreProperties>";
        }

        private static string CreateApplicationProperties()
        {
            return XmlHeader +
                   "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\" " +
                   "xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">" +
                   "<Application>上位机软件</Application><AppVersion>1.1</AppVersion></Properties>";
        }

        private static string CreateWorkbook()
        {
            return XmlHeader +
                   "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
                   "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   "<bookViews><workbookView/></bookViews><sheets>" +
                   "<sheet name=\"CPK概览\" sheetId=\"1\" r:id=\"rId1\"/>" +
                   "<sheet name=\"样本明细\" sheetId=\"2\" r:id=\"rId2\"/>" +
                   "<sheet name=\"直方图数据\" sheetId=\"3\" r:id=\"rId3\"/>" +
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
                   "<Relationship Id=\"rId4\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>" +
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
                   "</fonts><fills count=\"5\">" +
                   "<fill><patternFill patternType=\"none\"/></fill><fill><patternFill patternType=\"gray125\"/></fill>" +
                   "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FF1E70BF\"/><bgColor indexed=\"64\"/></patternFill></fill>" +
                   "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFE0F7E6\"/><bgColor indexed=\"64\"/></patternFill></fill>" +
                   "<fill><patternFill patternType=\"solid\"><fgColor rgb=\"FFFFDADA\"/><bgColor indexed=\"64\"/></patternFill></fill>" +
                   "</fills><borders count=\"2\"><border/><border><left style=\"thin\"/><right style=\"thin\"/><top style=\"thin\"/><bottom style=\"thin\"/><diagonal/></border></borders>" +
                   "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                   "<cellXfs count=\"8\">" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyAlignment=\"1\"><alignment horizontal=\"center\"/></xf>" +
                   "<xf numFmtId=\"0\" fontId=\"2\" fillId=\"2\" borderId=\"1\" xfId=\"0\" applyAlignment=\"1\"><alignment horizontal=\"center\" vertical=\"center\" wrapText=\"1\"/></xf>" +
                   "<xf numFmtId=\"0\" fontId=\"3\" fillId=\"0\" borderId=\"1\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"3\" borderId=\"1\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"4\" borderId=\"1\" xfId=\"0\"/>" +
                   "<xf numFmtId=\"10\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyNumberFormat=\"1\"/>" +
                   "</cellXfs><cellStyles count=\"1\"><cellStyle name=\"常规\" xfId=\"0\" builtinId=\"0\"/></cellStyles>" +
                   "</styleSheet>";
        }

        private static string CreateOverviewSheet(
            HistoryFilter filter,
            CpkCalculationResult calculation,
            out int trendHeaderRow)
        {
            CpkSpecification specification = calculation.Specification;
            List<List<ExcelCell>> rows = new List<List<ExcelCell>>
            {
                Row(TextCell("涂胶CPK分析报表", 1)),
                Row(TextCell("生成时间", 3), TextCell(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 4)),
                Row(TextCell("查询周期", 3), TextCell(filter.StartTime.ToString("yyyy-MM-dd") + " 至 " + filter.EndTime.ToString("yyyy-MM-dd"), 4)),
                Row(TextCell("数据类型", 3), TextCell(filter.SourceType, 4)),
                Row(TextCell("条码条件", 3), TextCell(string.IsNullOrWhiteSpace(filter.Barcode) ? "全部" : filter.Barcode, 4)),
                Row(TextCell("参数名称", 3), TextCell(calculation.ParameterName, 4)),
                Row(TextCell("单位", 3), TextCell(specification == null ? string.Empty : specification.Unit, 4)),
                Row(TextCell("计算模式", 3), TextCell(specification == null ? "未配置" : CpkSpecificationRepository.FormatMode(specification.CalculationMode), 4)),
                Row(TextCell("规格", 3), TextCell(CreateSpecificationText(specification), 4)),
                new List<ExcelCell>(),
                Row(TextCell("样本数", 3), NumberCell(calculation.SampleCount, 4), TextCell("平均值", 3), NumberCell(calculation.Mean, 4), TextCell("超限数", 3), NumberCell(calculation.OutOfSpecificationCount, 4)),
                Row(TextCell("最小值", 3), NumberCell(calculation.Minimum, 4), TextCell("最大值", 3), NumberCell(calculation.Maximum, 4), TextCell("跳过值数", 3), NumberCell(calculation.SkippedValueCount, 4)),
                Row(TextCell("组内标准差", 3), NullableNumberCell(calculation.WithinStandardDeviation, 4), TextCell("总体标准差", 3), NullableNumberCell(calculation.OverallStandardDeviation, 4), TextCell("能力判定", 3), TextCell(calculation.Conclusion, 4)),
                Row(TextCell("Cp", 3), NullableNumberCell(calculation.Cp, 4), TextCell("Cpu", 3), NullableNumberCell(calculation.Cpu, 4), TextCell("Cpl", 3), NullableNumberCell(calculation.Cpl, 4)),
                Row(TextCell("Cpk", 3), NullableNumberCell(calculation.Cpk, 4), TextCell("Pp", 3), NullableNumberCell(calculation.Pp, 4), TextCell("Ppk", 3), NullableNumberCell(calculation.Ppk, 4)),
                Row(TextCell("说明", 3), TextCell("Cpk使用组内波动；Ppk使用全部样本总体波动。原始CSV未被修改。", 4)),
                new List<ExcelCell>()
            };

            trendHeaderRow = rows.Count + 1;
            rows.Add(Row(
                TextCell("样本序号", 2), TextCell("采集时间", 2), TextCell("测量值", 2),
                TextCell("LSL", 2), TextCell("目标值", 2), TextCell("USL", 2)));
            foreach (CpkSample sample in calculation.Samples)
            {
                rows.Add(Row(
                    NumberCell(sample.Sequence, 4),
                    TextCell(sample.CollectTime.ToString("yyyy-MM-dd HH:mm:ss"), 4),
                    NumberCell(sample.Value, sample.IsWithinSpecification == false ? 6 : 5),
                    specification != null ? NullableNumberCell(specification.LowerLimit, 4) : TextCell(string.Empty, 4),
                    specification != null ? NullableNumberCell(specification.Target, 4) : TextCell(string.Empty, 4),
                    specification != null ? NullableNumberCell(specification.UpperLimit, 4) : TextCell(string.Empty, 4)));
            }
            return CreateWorksheet(rows, new[] { 15D, 22D, 18D, 18D, 18D, 20D }, "A1:F1", true, trendHeaderRow);
        }

        private static string CreateSamplesSheet(CpkCalculationResult calculation)
        {
            List<List<ExcelCell>> rows = new List<List<ExcelCell>>
            {
                Row(
                    TextCell("序号", 2), TextCell("采集时间", 2), TextCell("条码/模组码", 2),
                    TextCell("数据类型", 2), TextCell("参数名称", 2), TextCell("测量值", 2),
                    TextCell("规格判定", 2), TextCell("原始结果", 2), TextCell("原始CSV", 2))
            };
            foreach (CpkSample sample in calculation.Samples)
            {
                int style = sample.IsWithinSpecification == false ? 6 : sample.IsWithinSpecification == true ? 5 : 4;
                rows.Add(Row(
                    NumberCell(sample.Sequence, style),
                    TextCell(sample.CollectTime.ToString("yyyy-MM-dd HH:mm:ss"), style),
                    TextCell(sample.Barcode, style),
                    TextCell(sample.SourceType, style),
                    TextCell(calculation.ParameterName, style),
                    NumberCell(sample.Value, style),
                    TextCell(sample.IsWithinSpecification.HasValue ? sample.IsWithinSpecification.Value ? "OK" : "NG" : "--", style),
                    TextCell(sample.SourceResult, style),
                    TextCell(sample.SourceFile, style)));
            }
            return CreateWorksheet(rows, new[] { 10D, 22D, 30D, 18D, 30D, 16D, 14D, 14D, 60D }, null, false, 1, true);
        }

        private static string CreateHistogramSheet(HistogramData histogram)
        {
            List<List<ExcelCell>> rows = new List<List<ExcelCell>>
            {
                Row(TextCell("区间中心", 2), TextCell("区间下限", 2), TextCell("区间上限", 2), TextCell("频数", 2))
            };
            for (int index = 0; index < histogram.Counts.Count; index++)
            {
                rows.Add(Row(
                    NumberCell(histogram.Centers[index], 4),
                    NumberCell(histogram.LowerBounds[index], 4),
                    NumberCell(histogram.UpperBounds[index], 4),
                    NumberCell(histogram.Counts[index], 4)));
            }
            return CreateWorksheet(rows, new[] { 18D, 18D, 18D, 14D }, null, false, 1, true);
        }

        private static string CreateWorksheet(
            IList<List<ExcelCell>> rows,
            double[] widths,
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
                .Append("<dimension ref=\"A1:").Append(lastCell).Append("\"/><sheetViews><sheetView workbookViewId=\"0\">");
            if (frozenRows > 0)
            {
                xml.Append("<pane ySplit=\"").Append(frozenRows).Append("\" topLeftCell=\"A")
                    .Append(frozenRows + 1).Append("\" activePane=\"bottomLeft\" state=\"frozen\"/>");
            }
            xml.Append("</sheetView></sheetViews><sheetFormatPr defaultRowHeight=\"18\"/><cols>");
            for (int index = 0; index < widths.Length; index++)
            {
                xml.Append("<col min=\"").Append(index + 1).Append("\" max=\"").Append(index + 1)
                    .Append("\" width=\"").Append(widths[index].ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("\" customWidth=\"1\"/>");
            }
            xml.Append("</cols><sheetData>");
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                int excelRow = rowIndex + 1;
                xml.Append("<row r=\"").Append(excelRow).Append("\">");
                for (int columnIndex = 0; columnIndex < rows[rowIndex].Count; columnIndex++)
                {
                    AppendCell(xml, GetColumnName(columnIndex + 1) + excelRow, rows[rowIndex][columnIndex]);
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

        private static string CreateOverviewRelationships()
        {
            return XmlHeader +
                   "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                   "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing\" Target=\"../drawings/drawing1.xml\"/>" +
                   "</Relationships>";
        }

        private static string CreateDrawing()
        {
            return XmlHeader +
                   "<xdr:wsDr xmlns:xdr=\"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing\" " +
                   "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\" " +
                   "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   CreateChartAnchor(7, 1, 16, 17, 2, "涂胶参数趋势图", "rId1") +
                   CreateChartAnchor(17, 1, 26, 17, 3, "涂胶参数直方图", "rId2") +
                   "</xdr:wsDr>";
        }

        private static string CreateChartAnchor(int fromColumn, int fromRow, int toColumn, int toRow, int id, string name, string relationshipId)
        {
            return "<xdr:twoCellAnchor><xdr:from><xdr:col>" + fromColumn + "</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>" + fromRow + "</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:from>" +
                   "<xdr:to><xdr:col>" + toColumn + "</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>" + toRow + "</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:to>" +
                   "<xdr:graphicFrame macro=\"\"><xdr:nvGraphicFramePr><xdr:cNvPr id=\"" + id + "\" name=\"" + Escape(name) + "\"/><xdr:cNvGraphicFramePr/></xdr:nvGraphicFramePr>" +
                   "<xdr:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"0\" cy=\"0\"/></xdr:xfrm><a:graphic><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                   "<c:chart r:id=\"" + relationshipId + "\"/></a:graphicData></a:graphic></xdr:graphicFrame><xdr:clientData/></xdr:twoCellAnchor>";
        }

        private static string CreateDrawingRelationships()
        {
            return XmlHeader +
                   "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                   "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart\" Target=\"../charts/chart1.xml\"/>" +
                   "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart\" Target=\"../charts/chart2.xml\"/>" +
                   "</Relationships>";
        }

        private static string CreateTrendChart(CpkCalculationResult calculation, int headerRow)
        {
            int firstRow = headerRow + 1;
            int lastRow = Math.Max(firstRow, headerRow + calculation.SampleCount);
            string categories = "'CPK概览'!$B$" + firstRow + ":$B$" + lastRow;
            string[] names = { "测量值", "LSL", "目标值", "USL" };
            string[] columns = { "C", "D", "E", "F" };
            StringBuilder series = new StringBuilder();
            for (int index = 0; index < names.Length; index++)
            {
                series.Append(CreateChartSeries(index, names[index], categories,
                    "'CPK概览'!$" + columns[index] + "$" + firstRow + ":$" + columns[index] + "$" + lastRow));
            }
            return CreateChartSpace(calculation.ParameterName + " 趋势", "lineChart", series.ToString(), false);
        }

        private static string CreateHistogramChart(HistogramData histogram)
        {
            int firstRow = 2;
            int lastRow = Math.Max(firstRow, histogram.Counts.Count + 1);
            string series = CreateChartSeries(
                0,
                "频数",
                "'直方图数据'!$A$" + firstRow + ":$A$" + lastRow,
                "'直方图数据'!$D$" + firstRow + ":$D$" + lastRow);
            return CreateChartSpace("涂胶参数分布", "barChart", series, true);
        }

        private static string CreateChartSpace(string title, string chartElement, string seriesXml, bool barChart)
        {
            string chartProperties = barChart
                ? "<c:barDir val=\"col\"/><c:grouping val=\"clustered\"/><c:varyColors val=\"0\"/>"
                : "<c:grouping val=\"standard\"/><c:varyColors val=\"0\"/>";
            return XmlHeader +
                   "<c:chartSpace xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                   "<c:chart><c:title><c:tx><c:rich><a:bodyPr/><a:lstStyle/><a:p><a:r><a:rPr lang=\"zh-CN\"/><a:t>" + Escape(title) + "</a:t></a:r></a:p></c:rich></c:tx><c:layout/><c:overlay val=\"0\"/></c:title>" +
                   "<c:plotArea><c:layout/><c:" + chartElement + ">" + chartProperties + seriesXml +
                   "<c:axId val=\"48650112\"/><c:axId val=\"48672768\"/></c:" + chartElement + ">" +
                   "<c:catAx><c:axId val=\"48650112\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"b\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"48672768\"/><c:crosses val=\"autoZero\"/><c:auto val=\"1\"/><c:lblAlgn val=\"ctr\"/><c:lblOffset val=\"100\"/></c:catAx>" +
                   "<c:valAx><c:axId val=\"48672768\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"l\"/><c:majorGridlines/><c:numFmt formatCode=\"General\" sourceLinked=\"1\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"48650112\"/><c:crosses val=\"autoZero\"/><c:crossBetween val=\"between\"/></c:valAx>" +
                   "</c:plotArea><c:legend><c:legendPos val=\"b\"/><c:layout/><c:overlay val=\"0\"/></c:legend><c:plotVisOnly val=\"1\"/><c:dispBlanksAs val=\"gap\"/></c:chart></c:chartSpace>";
        }

        private static string CreateChartSeries(int index, string name, string categoryFormula, string valueFormula)
        {
            return "<c:ser><c:idx val=\"" + index + "\"/><c:order val=\"" + index + "\"/><c:tx><c:v>" + Escape(name) + "</c:v></c:tx>" +
                   "<c:cat><c:strRef><c:f>" + Escape(categoryFormula) + "</c:f></c:strRef></c:cat>" +
                   "<c:val><c:numRef><c:f>" + Escape(valueFormula) + "</c:f></c:numRef></c:val></c:ser>";
        }

        private static HistogramData CreateHistogram(IList<CpkSample> samples)
        {
            HistogramData histogram = new HistogramData();
            if (samples == null || samples.Count == 0)
            {
                return histogram;
            }
            int binCount = Math.Max(5, Math.Min(30, (int)Math.Ceiling(Math.Sqrt(samples.Count))));
            double minimum = samples.Min(item => item.Value);
            double maximum = samples.Max(item => item.Value);
            if (maximum - minimum <= 1E-12D)
            {
                double range = Math.Max(Math.Abs(minimum) * 0.02D, 1D);
                minimum -= range / 2D;
                maximum += range / 2D;
            }
            double width = (maximum - minimum) / binCount;
            for (int index = 0; index < binCount; index++)
            {
                histogram.LowerBounds.Add(minimum + index * width);
                histogram.UpperBounds.Add(minimum + (index + 1D) * width);
                histogram.Centers.Add(minimum + (index + 0.5D) * width);
                histogram.Counts.Add(0);
            }
            foreach (CpkSample sample in samples)
            {
                int index = (int)((sample.Value - minimum) / width);
                index = Math.Max(0, Math.Min(binCount - 1, index));
                histogram.Counts[index]++;
            }
            return histogram;
        }

        private static string CreateSpecificationText(CpkSpecification specification)
        {
            if (specification == null)
            {
                return "未配置";
            }
            return "LSL=" + FormatNullable(specification.LowerLimit) +
                   ", Target=" + FormatNullable(specification.Target) +
                   ", USL=" + FormatNullable(specification.UpperLimit);
        }

        private static string FormatNullable(double? value)
        {
            return value.HasValue ? value.Value.ToString("0.#####", CultureInfo.InvariantCulture) : "--";
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
                    .Append(Escape(TrimExcelText(cell.Value))).Append("</t></is></c>");
            }
        }

        private static ExcelCell TextCell(string value, int style)
        {
            return new ExcelCell { Value = value ?? string.Empty, StyleIndex = style };
        }

        private static ExcelCell NumberCell(double value, int style)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return TextCell(string.Empty, style);
            }
            return new ExcelCell
            {
                Value = value.ToString("0.###############", CultureInfo.InvariantCulture),
                StyleIndex = style,
                IsNumber = true
            };
        }

        private static ExcelCell NullableNumberCell(double? value, int style)
        {
            return value.HasValue ? NumberCell(value.Value, style) : TextCell(string.Empty, style);
        }

        private static List<ExcelCell> Row(params ExcelCell[] cells)
        {
            return cells.ToList();
        }

        private static string GetColumnName(int columnNumber)
        {
            StringBuilder name = new StringBuilder();
            int number = columnNumber;
            while (number > 0)
            {
                number--;
                name.Insert(0, (char)('A' + number % 26));
                number /= 26;
            }
            return name.ToString();
        }

        private static string Escape(string value)
        {
            return SecurityElement.Escape(SanitizeXmlText(value ?? string.Empty)) ?? string.Empty;
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
            return string.IsNullOrEmpty(value) || value.Length <= 32767
                ? value ?? string.Empty
                : value.Substring(0, 32760) + "...(截断)";
        }

        private static void WriteEntry(ZipArchive archive, string name, string content)
        {
            ZipArchiveEntry entry = archive.CreateEntry(name, CompressionLevel.Optimal);
            using (Stream stream = entry.Open())
            using (StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                writer.Write(content);
            }
        }

        private sealed class ExcelCell
        {
            public string Value { get; set; }
            public int StyleIndex { get; set; }
            public bool IsNumber { get; set; }
        }

        private sealed class HistogramData
        {
            public HistogramData()
            {
                Centers = new List<double>();
                LowerBounds = new List<double>();
                UpperBounds = new List<double>();
                Counts = new List<int>();
            }

            public List<double> Centers { get; private set; }
            public List<double> LowerBounds { get; private set; }
            public List<double> UpperBounds { get; private set; }
            public List<int> Counts { get; private set; }
        }
    }
}
