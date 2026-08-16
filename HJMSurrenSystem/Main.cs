using DataModel;
using DataModel.CSV;
using HJMSurrenSystem.Config_UI.FtpConfig;
using HJMSurrenSystem.Interface_UI;
using HJMSurrenSystem.MES;
using HJMSurrenSystem.Parameters;
using HJMSurrenSystem.RunProcess;
using HJMSurrenSystem.ShowUI;
using HJMSurrenSystem.Siemens;
using HJMSurrenSystem_Siemens;
using language;
using MachineIntegrationServiceService;
using NationalInstruments.Restricted;
using NxLog;
using Siemens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using UserManagement.userLogIn;

namespace HJMSurrenSystem
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            InitializeBarcodeLengthOptions();
            using (System.IO.Stream iconStream = typeof(Main).Assembly.GetManifestResourceStream("HJMSurrenSystem.Assets.AppLogo.ico"))
            {
                if (iconStream != null)
                {
                    using (Icon applicationIcon = new Icon(iconStream))
                    {
                        Icon = (Icon)applicationIcon.Clone();
                    }
                }
            }
            //EncryptionDog softDog = new EncryptionDog();
            //string readReg_str = softDog.readReg();
            //if (string.IsNullOrEmpty(readReg_str))
            //{
            //    DialogResult result = softDog.ShowDialog();
            //    result.CompareTo(DialogResult.Cancel);
            //}
            systemParameter();
        }
        public List<Button> buttons = new List<Button>();

        public void Form1_Load(object sender, EventArgs e)
        {
            languageParameter();
            languageSwitching();
            // languageSwitching 会重新执行 InitializeComponent，动态页面必须在它之后创建。
            InitializeHistoryDataPage();
            systemParameter();
            headerParameter();
            PLCInteractiveAddressParameter();
            MesPullInParameter();
            MesBomInventoryParameter();
            MesAssembleMaterialParameter();
            MesPullOutParameter();
            MesInitialWorkpieceParameter();
            MesPullOutUploadingParameter();
            EnsureManualUploadInterfaceTypes();

            operation_ban();
            Ini_Data();
            Ini();
            DataGridViewClass.DataGridCreateParams(this.dataGridView1);
            DataGridViewClass.DataGridCreateParams(this.dataGridView2);
            DataGridViewClass.DataGridCreateParams(this.dataGridView3);

            InitializeHistoryDataAfterLoad();

            BeginInvoke(new Action(() =>
            {
                if (scanBarcodeTextBox.CanFocus)
                {
                    scanBarcodeTextBox.Focus();
                }
            }));
        }

        private void scanBarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;

            string barcode = scanBarcodeTextBox.Text.Trim();
            if (string.IsNullOrEmpty(barcode))
            {
                scanBarcodeTextBox.Clear();
                return;
            }

            int barcodeLength = GetConfiguredBarcodeLength();
            bool isOk = barcode.Length == barcodeLength;
            string result = isOk ? "OK" : "NG";
            int sequence = scanResultGrid.Rows.Count + 1;
            DateTime scanTime = DateTime.Now;
            int rowIndex = scanResultGrid.Rows.Add(
                sequence,
                barcode,
                result,
                scanTime.ToString("yyyy-MM-dd HH:mm:ss"));

            Color rowColor = isOk ? Color.LightGreen : Color.Firebrick;
            Color textColor = isOk ? Color.Black : Color.White;
            DataGridViewRow row = scanResultGrid.Rows[rowIndex];
            row.DefaultCellStyle.BackColor = rowColor;
            row.DefaultCellStyle.ForeColor = textColor;
            row.DefaultCellStyle.SelectionBackColor = rowColor;
            row.DefaultCellStyle.SelectionForeColor = textColor;

            if (isOk)
            {
                WriteValidBarcodeToPlc(barcode);
            }

            SaveScanRecord(sequence, barcode, result, scanTime);

            scanResultGrid.ClearSelection();
            scanResultGrid.FirstDisplayedScrollingRowIndex = rowIndex;
            scanBarcodeTextBox.Clear();
            scanBarcodeTextBox.Focus();
        }

        private void WriteValidBarcodeToPlc(string barcode)
        {
            PLCInteractiveAddress barcodeSetting = ResourceHandler.listPLCInteractiveAddress
                .FirstOrDefault(item => string.Equals(
                    item.AddressThat == null ? string.Empty : item.AddressThat.Trim(),
                    "条码",
                    StringComparison.Ordinal));

            if (string.IsNullOrWhiteSpace(barcodeSetting.AddressThat) ||
                string.IsNullOrWhiteSpace(barcodeSetting.Address))
            {
                outDiary("扫码条码写入PLC失败：PLC交互配置表中未找到有效的“条码”地址。", "错误");
                return;
            }

            if (!string.Equals(
                barcodeSetting.AddressType == null ? string.Empty : barcodeSetting.AddressType.Trim(),
                "string",
                StringComparison.OrdinalIgnoreCase))
            {
                outDiary(
                    "扫码条码写入PLC失败：PLC交互配置表中“条码”的读取类型必须为string，当前类型：" +
                    barcodeSetting.AddressType,
                    "错误");
                return;
            }

            if (ResourceHandler.dparamParameters.siemensS7Net == null)
            {
                outDiary("扫码条码写入PLC失败：PLC通讯对象尚未初始化。", "错误");
                return;
            }

            bool writeSucceeded = ResourceHandler.dparamParameters.siemensS7Net.PLC_TryWrite_string(
                barcodeSetting.Address,
                barcode);
            if (writeSucceeded)
            {
                outDiary(
                    "扫码条码已写入PLC，地址：" + barcodeSetting.Address + "，条码：" + barcode,
                    "信息");
            }
            else
            {
                outDiary(
                    "扫码条码写入PLC失败，地址：" + barcodeSetting.Address + "，条码：" + barcode,
                    "错误");
            }
        }

        private void SaveScanRecord(int sequence, string barcode, string result, DateTime scanTime)
        {
            try
            {
                string programLogPath = ResourceHandler.listSystemParameters[0].ProgramLogPath;
                if (string.IsNullOrWhiteSpace(programLogPath))
                {
                    programLogPath = Application.StartupPath;
                }

                string saveDirectory = Path.Combine(
                    programLogPath,
                    "扫码数据",
                    scanTime.ToString("yyyy年MM月dd日"));
                Directory.CreateDirectory(saveDirectory);

                string filePath = Path.Combine(saveDirectory, "扫码记录.CSV");
                bool needsHeader = !File.Exists(filePath) || new FileInfo(filePath).Length == 0;

                using (FileStream fileStream = new FileStream(
                    filePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.ReadWrite))
                using (StreamWriter writer = new StreamWriter(fileStream, new UTF8Encoding(true)))
                {
                    if (needsHeader)
                    {
                        writer.WriteLine("序号,条码,结果,时间");
                    }

                    string excelBarcode = "=\"" + barcode.Replace("\"", "\"\"") + "\"";
                    writer.WriteLine(string.Join(",", new[]
                    {
                        EscapeScanCsvField(sequence.ToString()),
                        EscapeScanCsvField(excelBarcode),
                        EscapeScanCsvField(result),
                        EscapeScanCsvField(scanTime.ToString("yyyy-MM-dd HH:mm:ss"))
                    }));
                    writer.Flush();
                }
            }
            catch (Exception exception)
            {
                outDiary("扫码记录保存失败：" + exception.Message, "错误");
                MessageBox.Show(
                    "扫码记录保存失败，当前记录未写入本地Excel表格！\r\n" + exception.Message,
                    "扫码记录保存失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static string EscapeScanCsvField(string value)
        {
            string fieldValue = value ?? string.Empty;
            if (fieldValue.Contains(",") || fieldValue.Contains("\"") || fieldValue.Contains("\r") || fieldValue.Contains("\n"))
            {
                return "\"" + fieldValue.Replace("\"", "\"\"") + "\"";
            }

            return fieldValue;
        }

        private void InitializeBarcodeLengthOptions()
        {
            if (scanBarcodeLengthComboBox.Items.Count > 0)
            {
                return;
            }

            for (int barcodeLength = 1; barcodeLength <= 100; barcodeLength++)
            {
                scanBarcodeLengthComboBox.Items.Add(barcodeLength);
            }
        }

        private void LoadBarcodeLengthSetting()
        {
            InitializeBarcodeLengthOptions();

            int barcodeLength;
            bool hasValidSetting = int.TryParse(
                ResourceHandler.listSystemParameters[0].BarcodeLength,
                out barcodeLength)
                && barcodeLength >= 1
                && barcodeLength <= 100;

            if (!hasValidSetting)
            {
                barcodeLength = 9;
                ResourceHandler.listSystemParameters[0].BarcodeLength = barcodeLength.ToString();
                SaveBarcodeLengthSetting(false);
            }

            scanBarcodeLengthComboBox.SelectedItem = barcodeLength;
        }

        private int GetConfiguredBarcodeLength()
        {
            if (scanBarcodeLengthComboBox.SelectedItem is int barcodeLength)
            {
                return barcodeLength;
            }

            return 9;
        }

        private void scanBarcodeLengthComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].BarcodeLength = GetConfiguredBarcodeLength().ToString();
            SaveBarcodeLengthSetting(true);
            scanBarcodeTextBox.Focus();
        }

        private void SaveBarcodeLengthSetting(bool showFailureMessage)
        {
            XmlHelper xmlHelper = new XmlHelper("xml/systemParameters.xml");
            bool saveSucceeded = xmlHelper.Write(ResourceHandler.listSystemParameters);
            if (!saveSucceeded && showFailureMessage)
            {
                MessageBox.Show("条码长度保存失败！\nFailed to save barcode length!\nBarcodelänge konnte nicht gespeichert werden!");
            }
        }
        //public void operation_ban()
        //{
        //    groupBox9.Enabled = false;


        //    toolStripDropDownButton1.Enabled = false;
        //    toolStripDropDownButton2.Enabled = false;
        //}
        public void operation_ban()
        {
            //groupBox9.Enabled = false;//手动上传功能组
            dateTimePicker1.Enabled = false;//选择日期
            textBox1.Enabled = false;//输入条码
            cmbInterfaceType.Enabled = false;//选择功能
            btnSerach.Enabled = false;//查询按钮
            btnUP.Enabled = false;//上传按钮
            toolStripDropDownButton3.Enabled = false;//查看按钮
            toolStripDropDownButton1.Enabled = false;//设置按钮
            toolStripDropDownButton2.Enabled = false;//管理按钮
        }

        static string nasduabwduadawdb(string miawdiawduasdhasd)
        {
            StringBuilder stringBuilder = new StringBuilder();
            MD5 mD = MD5.Create();
            byte[] array = mD.ComputeHash(Encoding.Unicode.GetBytes(miawdiawduasdhasd));
            mD.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append((255 - array[i]).ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "程序即将关闭,是否退出？"), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                try
                {
                    // 程序退出前去掉单例窗口设置
                    Program.mutex.ReleaseMutex();
                    Program.mutex.Close();
                    Program.mutex = null;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        #region 配置参数加载

        /// <summary>
        /// 加载语言配置文件
        /// </summary>
        public void languageParameter()
        {
            XmlHelper xmlHelper = new XmlHelper("language/languages.xml");
            bool languageBool = xmlHelper.Read(ref Method.languages);
            if (!languageBool)
            {
                MessageBox.Show("语言文件读取失败!\nLanguageFileReadError!\nFehler beim Lesen der Sprachdatei!");
            }
        }

        /// <summary>
        /// 加载系统配置文件
        /// </summary>
        public void systemParameter()
        {
            XmlHelper xmlHelper = new XmlHelper("xml/systemParameters.xml");
            bool SystemBool = xmlHelper.Read(ref ResourceHandler.listSystemParameters);
            if (!SystemBool)
            {
                MessageBox.Show("系统文件读取失败!\nSystemFileReadError!\nSystem beim Lesen der Sprachdatei!");
            }

            LoadBarcodeLengthSetting();
            workstationName.Text = ResourceHandler.listSystemParameters[0].currentLocation;
            equipmentIdentification.Text = ResourceHandler.listSystemParameters[0].deviceIdentification;
            ResourceHandler.dparamParameters.formulaUI = new FormulaUI(this);
            ResourceHandler.dparamParameters.formulaUI.ProjectName.Text = ResourceHandler.listSystemParameters[0].FormulaName;

            ResourceHandler.dparamParameters.offLineUI = new OffLineUI(this);
        }

        /// <summary>
        /// 表头参数加载
        /// </summary>
        public void headerParameter()
        {
            XmlHelper xmlHelper = new XmlHelper("xml/" + workstationName.Text + "/header.xml");
            bool headerBool = xmlHelper.Read(ref ResourceHandler.listHeader);
            if (!headerBool)
            {
                MessageBox.Show("表头文件读取失败!\nheaderFileReadError!\nDie haltung beim Lesen der Sprachdatei!");
            }
        }

        /// <summary>
        /// PLC交互地址参数加载
        /// </summary>
        public void PLCInteractiveAddressParameter()
        {
            XmlHelper xmlPLCInteractiveAddress = new XmlHelper($"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/PLCInteractiveAddress.xml");
            bool PLCInteractiveAddressBool = xmlPLCInteractiveAddress.Read(ref ResourceHandler.listPLCInteractiveAddress);
            if (!PLCInteractiveAddressBool)
            {
                MessageBox.Show("Plc地址文件读取失败!\nPlcAddressFileReadError!\nPlc adresse beim Lesen der Sprachdatei!");
            }

            ResourceHandler.dparamParameters.pLCInteractionUI = new PLCInteractionUI(this);

            foreach (var item in ResourceHandler.listPLCInteractiveAddress)
            {
                string[] signalAddress_Ary = { item.AddressThat, item.Address, item.AddressType };
                DataGridViewClass.AddRows(ResourceHandler.dparamParameters.pLCInteractionUI.dataGridView1, signalAddress_Ary, Color.White);
            }
        }

        /// <summary>
        /// MES进站参数加载
        /// </summary>
        public void MesPullInParameter()
        {
            XmlHelper xmlMesPullIn = new XmlHelper("xml/" + workstationName.Text + "/MesPullIn.xml");
            bool MesPullInBool = xmlMesPullIn.Read(ref ResourceHandler.listMesPullInParameters);
            if (!MesPullInBool)
            {
                MessageBox.Show("MES进站参数读取失败!\nPlcAddressFileReadError!\nPlc adresse beim Lesen der Sprachdatei!");
            }
            ResourceHandler.dparamParameters.mesPullInUI = new MesPullInUI(this);
        }

        private static void AddInterfaceDefault(
            List<MesPullInParameters> parameters,
            string name,
            string explanation,
            string value)
        {
            if (parameters.Any(item => string.Equals(item.ParametersName, name, StringComparison.Ordinal)))
            {
                return;
            }

            parameters.Add(new MesPullInParameters
            {
                ParametersName = name,
                ParametersExplain = explanation,
                ParametersPrice = value
            });
        }

        private static string ReadLegacyParameter(string name)
        {
            MesPullInParameters parameter = ResourceHandler.listMesPullInParameters.FirstOrDefault(item =>
                string.Equals(item.ParametersName, name, StringComparison.OrdinalIgnoreCase));
            return parameter.ParametersPrice ?? "";
        }

        private static bool SetMigratedParameter(
            List<MesPullInParameters> parameters,
            string targetName,
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            int index = parameters.FindIndex(item => string.Equals(item.ParametersName, targetName, StringComparison.Ordinal));
            if (index < 0)
            {
                return false;
            }

            MesPullInParameters parameter = parameters[index];
            parameter.ParametersPrice = value.Trim();
            parameters[index] = parameter;
            return true;
        }

        private static string ToWsdlUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || url.IndexOf("?wsdl", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return url;
            }

            return url.TrimEnd('/') + "?wsdl";
        }

        private bool MigrateBomInventoryParameters()
        {
            bool migrated = false;
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "enabled", ReadLegacyParameter("bomInventoryEnabled"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "MES WSDL", ToWsdlUrl(ReadLegacyParameter("bomInventoryUrl")));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "TimeOut(ms)", ReadLegacyParameter("bomInventoryTimeout"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "User", ReadLegacyParameter("bomInventoryUserName"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "Password", ReadLegacyParameter("bomInventoryPassword"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "site", ReadLegacyParameter("bomInventorySite"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "user", ReadLegacyParameter("bomInventoryUser"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "operation", ReadLegacyParameter("bomInventoryOperation"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "operationRevision", ReadLegacyParameter("bomInventoryOperationRevision"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "activity", ReadLegacyParameter("bomInventoryActivity"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "Resource", ReadLegacyParameter("bomInventoryResource"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "modeCheckOperation", ReadLegacyParameter("bomInventoryModeCheckOperation"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "modeProcessSfc", ReadLegacyParameter("bomInventoryModeProcessSfc"));

            string usage = string.Join(";", new[] { ReadLegacyParameter("bomInventoryUsage1"), ReadLegacyParameter("bomInventoryUsage2") }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
            string category = string.Join(";", new[] { ReadLegacyParameter("bomInventoryCategory1"), ReadLegacyParameter("bomInventoryCategory2") }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
            string dataField = string.Join(";", new[] { ReadLegacyParameter("bomInventoryDataField1"), ReadLegacyParameter("bomInventoryDataField2") }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "usage", usage);
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "category", category);
            migrated |= SetMigratedParameter(ResourceHandler.listMesBomInventoryParameters, "dataField", dataField);
            return migrated;
        }

        private bool MigrateAssembleMaterialParameters()
        {
            bool migrated = false;
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "enabled", ReadLegacyParameter("assembleMaterialEnabled"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "MES WSDL", ToWsdlUrl(ReadLegacyParameter("assembleMaterialUrl")));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "TimeOut(ms)", ReadLegacyParameter("assembleMaterialTimeout"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "User", ReadLegacyParameter("assembleMaterialUserName"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "Password", ReadLegacyParameter("assembleMaterialPassword"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "site", ReadLegacyParameter("assembleMaterialSite"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "user", ReadLegacyParameter("assembleMaterialUser"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "operation", ReadLegacyParameter("assembleMaterialOperation"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "operationRevision", ReadLegacyParameter("assembleMaterialOperationRevision"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "activityId", ReadLegacyParameter("assembleMaterialActivityId"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "Resource", ReadLegacyParameter("assembleMaterialResource"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "dcGroup", ReadLegacyParameter("assembleMaterialDcGroup"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "dcGroupRevision", ReadLegacyParameter("assembleMaterialDcGroupRevision"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "modeProcessSfc", ReadLegacyParameter("assembleMaterialModeProcessSfc"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "partialAssembly", ReadLegacyParameter("assembleMaterialPartialAssembly"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "inventoryArray[]", ReadLegacyParameter("assembleMaterialInventoryArray"));
            migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "parameterArray[]", ReadLegacyParameter("assembleMaterialParameterArray"));

            string legacyNcArray = ReadLegacyParameter("assembleMaterialNcCodeArray");
            if (!string.IsNullOrWhiteSpace(legacyNcArray))
            {
                List<string> ncCodes = new List<string>();
                List<string> hasNcValues = new List<string>();
                foreach (string item in legacyNcArray.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = item.Split(new[] { '|' }, 2);
                    if (parts.Length == 2)
                    {
                        ncCodes.Add(parts[0].Trim());
                        hasNcValues.Add(parts[1].Trim());
                    }
                }
                migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "ncCode", string.Join(";", ncCodes));
                migrated |= SetMigratedParameter(ResourceHandler.listMesAssembleMaterialParameters, "hasNc", string.Join(";", hasNcValues));
            }
            return migrated;
        }

        private bool RemoveLegacyParameters(string prefix)
        {
            int removed = ResourceHandler.listMesPullInParameters.RemoveAll(item =>
                !string.IsNullOrWhiteSpace(item.ParametersName) &&
                item.ParametersName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            return removed > 0;
        }

        public void MesBomInventoryParameter()
        {
            string path = "xml/" + workstationName.Text + "/MesCheckBOMInventory.xml";
            bool fileExists = File.Exists(path);
            XmlHelper xmlHelper = new XmlHelper(path);
            bool loaded = xmlHelper.Read(ref ResourceHandler.listMesBomInventoryParameters);
            if (!loaded)
            {
                ResourceHandler.listMesBomInventoryParameters.Clear();
                if (fileExists)
                {
                    MessageBox.Show("贴纸PN及库存校验参数读取失败！");
                }
            }

            bool hadIndependentParameters = ResourceHandler.listMesBomInventoryParameters.Count > 0;
            EnsureBomInventoryParameters();
            bool migrated = !hadIndependentParameters && MigrateBomInventoryParameters();
            bool removedLegacy = RemoveLegacyParameters("bomInventory");
            if ((!fileExists || (loaded && migrated)) && !xmlHelper.Write(ResourceHandler.listMesBomInventoryParameters))
            {
                MessageBox.Show("贴纸PN及库存校验参数保存失败！");
            }
            if (removedLegacy)
            {
                new XmlHelper("xml/" + workstationName.Text + "/MesPullIn.xml").Write(ResourceHandler.listMesPullInParameters);
                RefreshMesPullInParameterGrid();
            }

            ResourceHandler.dparamParameters.mesBomInventoryUI = new MesPullInUI(
                this,
                ResourceHandler.listMesBomInventoryParameters,
                "MesCheckBOMInventory.xml",
                "贴纸PN及库存校验参数配置表.CSV",
                "贴纸PN及库存校验配置",
                EnsureBomInventoryParameters);
        }

        public void MesAssembleMaterialParameter()
        {
            string path = "xml/" + workstationName.Text + "/MesAssembleAndCollectDataForSfc.xml";
            bool fileExists = File.Exists(path);
            XmlHelper xmlHelper = new XmlHelper(path);
            bool loaded = xmlHelper.Read(ref ResourceHandler.listMesAssembleMaterialParameters);
            if (!loaded)
            {
                ResourceHandler.listMesAssembleMaterialParameters.Clear();
                if (fileExists)
                {
                    MessageBox.Show("组装物料参数读取失败！");
                }
            }

            bool hadIndependentParameters = ResourceHandler.listMesAssembleMaterialParameters.Count > 0;
            EnsureAssembleMaterialParameters();
            bool migrated = !hadIndependentParameters && MigrateAssembleMaterialParameters();
            bool removedLegacy = RemoveLegacyParameters("assembleMaterial");
            if ((!fileExists || (loaded && migrated)) && !xmlHelper.Write(ResourceHandler.listMesAssembleMaterialParameters))
            {
                MessageBox.Show("组装物料参数保存失败！");
            }
            if (removedLegacy)
            {
                new XmlHelper("xml/" + workstationName.Text + "/MesPullIn.xml").Write(ResourceHandler.listMesPullInParameters);
                RefreshMesPullInParameterGrid();
            }

            ResourceHandler.dparamParameters.mesAssembleMaterialUI = new MesPullInUI(
                this,
                ResourceHandler.listMesAssembleMaterialParameters,
                "MesAssembleAndCollectDataForSfc.xml",
                "组装物料参数配置表.CSV",
                "组装物料配置",
                EnsureAssembleMaterialParameters);
        }

        public void EnsureBomInventoryParameters()
        {
            List<MesPullInParameters> parameters = ResourceHandler.listMesBomInventoryParameters;
            AddInterfaceDefault(parameters, "enabled", "程序控制项：启用贴纸PN及库存校验(true/false)", "true");
            AddInterfaceDefault(parameters, "MES WSDL", "WS服务器WSDL", "http://172.26.11.3:50200/atlmeswebservice/MiCheckBOMInventoryServiceService?wsdl");
            AddInterfaceDefault(parameters, "TimeOut(ms)", "WS服务器连接超时设置，毫秒", "10000");
            AddInterfaceDefault(parameters, "User", "连接服务器用户名", "");
            AddInterfaceDefault(parameters, "Password", "连接服务器用户密码", "");
            AddInterfaceDefault(parameters, "site", "设备所在的站点", "M002");
            AddInterfaceDefault(parameters, "user", "操作用户", "");
            AddInterfaceDefault(parameters, "operation", "工位", "");
            AddInterfaceDefault(parameters, "operationRevision", "工位版本", "#");
            AddInterfaceDefault(parameters, "activity", "活动", "EAP_WS");
            AddInterfaceDefault(parameters, "Resource", "设备资源号", "");
            AddInterfaceDefault(parameters, "modeCheckOperation", "工位检查模式", "");
            AddInterfaceDefault(parameters, "modeProcessSfc", "过站模式", "MODE_COMPLETE_SFC_POST_DC");
            AddInterfaceDefault(parameters, "usage", "parameterArray[]中的usage；多项用分号分隔", "RESOURCE;BOM");
            AddInterfaceDefault(parameters, "category", "parameterArray[]中的category；多项用分号分隔", "RESOURCE;RESOURCE");
            AddInterfaceDefault(parameters, "dataField", "parameterArray[]中的dataField；多项用分号分隔", "Z_FMA_RES;Z_FMA_BOM");
            AddInterfaceDefault(parameters, "sfc", "SFC：模组号；{SFC}表示使用设备条码", "{SFC}");
            AddInterfaceDefault(parameters, "parameterArray[]", "DC参数数组：usage|category|dataField；多项用分号分隔", "");
        }

        public void EnsureAssembleMaterialParameters()
        {
            List<MesPullInParameters> parameters = ResourceHandler.listMesAssembleMaterialParameters;
            AddInterfaceDefault(parameters, "enabled", "程序控制项：启用组装物料(true/false)", "true");
            AddInterfaceDefault(parameters, "MES WSDL", "WS服务器WSDL", "http://172.26.11.3:50200/atlmeswebservice/MiAssembleAndCollectDataForSfcServiceService?wsdl");
            AddInterfaceDefault(parameters, "TimeOut(ms)", "WS服务器连接超时设置，毫秒", "10000");
            AddInterfaceDefault(parameters, "User", "连接服务器用户名", "");
            AddInterfaceDefault(parameters, "Password", "连接服务器用户密码", "");
            AddInterfaceDefault(parameters, "site", "设备所在的站点", "M002");
            AddInterfaceDefault(parameters, "user", "操作用户", "");
            AddInterfaceDefault(parameters, "operation", "工位", "");
            AddInterfaceDefault(parameters, "operationRevision", "工位版本", "#");
            AddInterfaceDefault(parameters, "activityId", "活动", "EAP_WS");
            AddInterfaceDefault(parameters, "Resource", "设备资源号", "");
            AddInterfaceDefault(parameters, "dcGroup", "数据收集组", "*");
            AddInterfaceDefault(parameters, "dcGroupRevision", "数据收集组版本", "#");
            AddInterfaceDefault(parameters, "modeProcessSfc", "过站模式", "MODE_NONE");
            AddInterfaceDefault(parameters, "partialAssembly", "是否部分组装(true/false)", "true");
            AddInterfaceDefault(parameters, "sfc", "SFC：模组号；{SFC}表示使用设备条码", "{SFC}");
            AddInterfaceDefault(parameters, "ncCode", "NC代码名称；多项用分号分隔", "");
            AddInterfaceDefault(parameters, "hasNc", "是否NC；与ncCode逐项对应，多项用分号分隔", "false");
            AddInterfaceDefault(parameters, "inventoryArray[]", "库存数组：库存号|数量|属性=值&属性=值；多项用分号分隔", "");
            AddInterfaceDefault(parameters, "parameterArray[]", "DC参数数组：名称|类型|值；类型NUMBER/TEXT/FORMULA/BOOLEAN", "");
        }

        public void RefreshMesPullInParameterGrid()
        {
            MesPullInUI pullInUi = ResourceHandler.dparamParameters.mesPullInUI;
            if (pullInUi == null || pullInUi.IsDisposed)
            {
                return;
            }

            pullInUi.ReloadGrid();
        }

        private void EnsureManualUploadInterfaceTypes()
        {
            const string bomInventoryManualType = "手动贴纸PN/库存校验";
            const string assembleMaterialManualType = "手动组装物料";
            if (!cmbInterfaceType.Items.Contains(bomInventoryManualType))
            {
                cmbInterfaceType.Items.Add(bomInventoryManualType);
            }
            if (!cmbInterfaceType.Items.Contains(assembleMaterialManualType))
            {
                cmbInterfaceType.Items.Add(assembleMaterialManualType);
            }
        }

        /// <summary>
        /// MES出站参数加载
        /// </summary>
        public void MesPullOutParameter()
        {
            XmlHelper xmlMesPullOut = new XmlHelper("xml/" + workstationName.Text + "/MesPullOut.xml");
            bool MesPullOutBool = xmlMesPullOut.Read(ref ResourceHandler.listMesPullOutParameters);
            if (!MesPullOutBool)
            {
                MessageBox.Show("MES出站参数读取失败!\nPlcAddressFileReadError!\nPlc adresse beim Lesen der Sprachdatei!");
            }
            ResourceHandler.dparamParameters.mesPullOutUI = new MesPullOutUI(this);
            foreach (var item in ResourceHandler.listMesPullOutParameters)
            {
                string[] mesPullOutUI_Ary = { item.ParametersName, item.ParametersExplain, item.ParametersPrice };
                DataGridViewClass.AddRows(ResourceHandler.dparamParameters.mesPullOutUI.dataGridView1, mesPullOutUI_Ary, Color.White);
            }
        }

        /// <summary>
        /// MES首件参数加载
        /// </summary>
        public void MesInitialWorkpieceParameter()
        {
            XmlHelper xmlMesInitialWorkpiece = new XmlHelper("xml/" + workstationName.Text + "/MesInitialWorkpiece.xml");
            bool MesInitialWorkpieceBool = xmlMesInitialWorkpiece.Read(ref ResourceHandler.listMesInitialWorkpieceParameters);
            if (!MesInitialWorkpieceBool)
            {
                MessageBox.Show("MES首件参数读取失败!\nPlcAddressFileReadError!\nPlc adresse beim Lesen der Sprachdatei!");
            }
            ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI = new MesInitialWorkpieceParametersUI(this);
            foreach (var item in ResourceHandler.listMesInitialWorkpieceParameters)
            {
                string[] mesInitialWorkpieceParametersUI_Ary = { item.ParametersName, item.ParametersExplain, item.ParametersPrice };
                DataGridViewClass.AddRows(ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.dataGridView1, mesInitialWorkpieceParametersUI_Ary, Color.White);
            }
        }

        /// <summary>
        /// MES电芯校验参数加载
        /// </summary>
        public void MesDX_verifyParameter()
        {
            电芯校验配置ToolStripMenuItem.Visible = true;
            XmlHelper xmlMesDX_verify = new XmlHelper("xml/" + workstationName.Text + "/MesDX_verifyUI.xml");
            bool MesDX_verifyBool = xmlMesDX_verify.Read(ref ResourceHandler.listMesDX_verifyParameters);
            if (!MesDX_verifyBool)
            {
                MessageBox.Show("MES电芯校验参数!\nPlcAddressFileReadError!\nPlc adresse beim Lesen der Sprachdatei!");
            }
            ResourceHandler.dparamParameters.mesDX_verifyUI = new MesDX_verifyUI(this);
            foreach (var item in ResourceHandler.listMesDX_verifyParameters)
            {
                string[] mesDX_verify_Ary = { item.ParametersName, item.ParametersExplain, item.ParametersPrice };
                DataGridViewClass.AddRows(ResourceHandler.dparamParameters.mesDX_verifyUI.dataGridView1, mesDX_verify_Ary, Color.White);
            }
        }

        /// <summary>
        /// MES上传参数加载
        /// </summary>
        public void MesPullOutUploadingParameter()
        {
            XmlHelper xmlMESPullOut = new XmlHelper($"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/MesPullOutUploading.xml");
            bool MESPullOutBool = xmlMESPullOut.Read(ref ResourceHandler.listMesPullOutUploadingParameters);
            if (!MESPullOutBool)
            {
                MessageBox.Show("MES出站上传参数读取失败!\nPlcAddressFileReadError!\nPlc adresse beim Lesen der Sprachdatei!");
            }
            ResourceHandler.dparamParameters.mesPullOutUploadingUI = new MesPullOutUploadingUI(this);
            foreach (var item in ResourceHandler.listMesPullOutUploadingParameters)
            {
                string[] mesPullOutUploading_Ary = { item.Header, item.ParametersMESName, item.ParametersMESType, item.PLCAddres, item.ParametersPLCType, item.ParametersUpperLimit.ToString(), item.ParametersLowerLimit.ToString(), item.WhetherUploading.ToString(), item.WhetherInitial.ToString(), item.RepetitionVerify.ToString(), item.DataQuantityPath, item.DataQuantityType, item.DataIncrementValue };
                DataGridViewClass.AddRows(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1, mesPullOutUploading_Ary, Color.White);
            }
        }

        /// <summary>
        /// MES侧板上传参数加载
        /// </summary>
        public void MesPullOutSide_plateParameter()
        {
            侧板出站上传参数ToolStripMenuItem.Visible = true; //显示控件
            XmlHelper xmlMESPullOut = new XmlHelper($"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/MesPullOutSide_plate.xml");
            bool MESPullOutBool = xmlMESPullOut.Read(ref ResourceHandler.listMesPullOutSide_plateParameters);
            if (!MESPullOutBool)
            {
                MessageBox.Show("MES侧板上传参数读取失败!\nPlcAddressFileReadError!\nPlc adresse beim Lesen der Sprachdatei!");
            }
            ResourceHandler.dparamParameters.mesPullOutSide_plateUI = new MesPullOutSide_plateUI(this);
            foreach (var item in ResourceHandler.listMesPullOutSide_plateParameters)
            {
                string[] mesPullOutUploading_Ary = { item.Header, item.ParametersMESName, item.ParametersMESType, item.PLCAddres, item.ParametersPLCType, item.ParametersUpperLimit.ToString(), item.ParametersLowerLimit.ToString(), item.WhetherUploading.ToString(), item.WhetherInitial.ToString(), item.RepetitionVerify.ToString() };
                DataGridViewClass.AddRows(ResourceHandler.dparamParameters.mesPullOutSide_plateUI.dataGridView1, mesPullOutUploading_Ary, Color.White);
            }
        }

        /// <summary>
        /// 语言切换
        /// </summary>
        public void languageSwitching()
        {
            if (ResourceHandler.listSystemParameters[0].Language.Equals("英"))
            {
                ResourceHandler.listSystemParameters[0].Language = "英";

                ResourceHandler.dparamParameters.Language = LanguageType.英语;
                switch (ResourceHandler.dparamParameters.Language)
                {
                    case LanguageType.简体中文:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("");
                        break;
                    case LanguageType.英语:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
                        break;
                    case LanguageType.德语:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");
                        break;
                    default:
                        break;
                }
                ResourceHandler.listSystemParameters[0].Language = "英";
                userResourceHandler.language = "英";

                保存ToolStripMenuItem_Click();

                this.Controls.Clear();
                FormClosing -= new FormClosingEventHandler(this.Form1_FormClosing);
                InitializeComponent();
            }
            else if (ResourceHandler.listSystemParameters[0].Language.Equals("德"))
            {
                ResourceHandler.listSystemParameters[0].Language = "德";

                ResourceHandler.dparamParameters.Language = LanguageType.德语;
                switch (ResourceHandler.dparamParameters.Language)
                {
                    case LanguageType.简体中文:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("");
                        break;
                    case LanguageType.英语:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
                        break;
                    case LanguageType.德语:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");
                        break;
                    default:
                        break;
                }
                ResourceHandler.listSystemParameters[0].Language = "德";
                userResourceHandler.language = "德";

                保存ToolStripMenuItem_Click();

                this.Controls.Clear();
                FormClosing -= new FormClosingEventHandler(this.Form1_FormClosing);
                InitializeComponent();
            }
            else
            {
                ResourceHandler.listSystemParameters[0].Language = "中";

                ResourceHandler.dparamParameters.Language = LanguageType.简体中文;
                switch (ResourceHandler.dparamParameters.Language)
                {
                    case LanguageType.简体中文:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("");
                        break;
                    case LanguageType.英语:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
                        break;
                    case LanguageType.德语:
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");
                        break;
                    default:
                        break;
                }
                ResourceHandler.listSystemParameters[0].Language = "中";
                userResourceHandler.language = "中";

                保存ToolStripMenuItem_Click();

                this.Controls.Clear();
                FormClosing -= new FormClosingEventHandler(this.Form1_FormClosing);
                InitializeComponent();
            }

            LoggingServiceText.setPath(ResourceHandler.listSystemParameters[0].ProgramLogPath);

            outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + user_Name.Text + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "系统文件读取成功"), "调试");
        }

        /// <summary>
        /// 表格初始化
        /// </summary>
        public void Ini_Data()
        {
            // 删除所有数据表的列
            DataGridViewClass.RemoveAllColumns(dataGridView1);
            DataGridViewClass.RemoveAllColumns(dataGridView2);
            DataGridViewClass.RemoveAllColumns(dataGridView3);

            DataGridViewClass.AddColumns(dataGridView1, ResourceHandler.listHeader[0].InOutStandingHeader.Split('~'));

            string[] header = new string[ResourceHandler.listMesPullOutUploadingParameters.Count + 6];
            header[0] = "模组码";
            for (int i = 0; i < ResourceHandler.listMesPullOutUploadingParameters.Count; i++)
            {
                header[i + 1] = ResourceHandler.listMesPullOutUploadingParameters[i].Header;
            }
            header[ResourceHandler.listMesPullOutUploadingParameters.Count + 1] = "结果";
            header[ResourceHandler.listMesPullOutUploadingParameters.Count + 2] = "时间";
            header[ResourceHandler.listMesPullOutUploadingParameters.Count + 3] = "班次";
            header[ResourceHandler.listMesPullOutUploadingParameters.Count + 4] = "设备标识";
            header[ResourceHandler.listMesPullOutUploadingParameters.Count + 5] = "是否首件";

            DataGridViewClass.AddColumns(dataGridView2, header);
            DataGridViewClass.AddColumns(dataGridView3, header);

            // 关闭掉数据表的上下排序
            DataGridViewClass.BanSort(dataGridView1);
            DataGridViewClass.SetTitleWidth(dataGridView1);

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, Lab_Red_Btn.Width, Lab_Red_Btn.Height);
            Lab_Red_Btn.Region = new Region(gp);
            Lab_Green_Btn.Region = new Region(gp);
            Lab_Yellow_Btn.Region = new Region(gp);
            Lab_Red_Btn.FlatAppearance.BorderSize = 0;
            Lab_Red_Btn.FlatStyle = FlatStyle.Flat;
            Lab_Green_Btn.FlatAppearance.BorderSize = 0;
            Lab_Green_Btn.FlatStyle = FlatStyle.Flat;
            Lab_Yellow_Btn.FlatAppearance.BorderSize = 0;
            Lab_Yellow_Btn.FlatStyle = FlatStyle.Flat;


        }
        private FTPHelper ftpClient1;
        private FtpParam ftpParam1;
        /// <summary>
        ///程序流程加载
        /// </summary>
        public void Ini()
        {
            JPIO_OPC.heartBeatCallBack = (JPIO_OPC.ThreadEvent)Delegate.Combine(JPIO_OPC.heartBeatCallBack, new JPIO_OPC.ThreadEvent(PLC_heartBeatEvent));
            switch (workstationName.Text)
            {
                case "涂胶检查":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new BSB焊接(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "极柱拍照":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 极柱拍照(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "CCS上料":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new CCS上料(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "侧缝焊接":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 侧缝焊接(this);
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "镍片焊接":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 镍片焊接(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "激光刻码":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 激光刻码(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "水冷板焊接":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 水冷板焊接(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "焊中检测":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 焊中检测(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabPage1.Parent = null;//隐藏进出站选项卡
                    tabPage6.Parent = null;//隐藏数据重传选项卡
                    ftpParam1 = ToolUtils.GetObjToIniData<FtpParam>("FtpConfig1", typeof(FtpParam), "xml/FtpConfigs.ini");
                    if (ftpParam1 != null && ftpParam1.IsEnabled)
                        ftpClient1 = new FTPHelper(ftpParam1.FtpIP, @ftpParam1.FtpPath, ftpParam1.FtpName, ftpParam1.FtpPwd);
                    break;
                case "焊后铣削":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 焊后铣削(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
                case "焊后除尘":
                    ResourceHandler.dparamParameters.flowClass = null;
                    ResourceHandler.dparamParameters.flowClass = new 焊后除尘(this);
                    tabPage3.Parent = null;//隐藏侧板出站选项卡
                    tabControl1.TabPages.Remove(Wdd_Tabpage);// 隐藏Wdd选项卡
                    break;
            }
            if (!tabControl1.TabPages.Contains(Wdd_Tabpage))
                return;

            //初始化WDD
            string poleRow = ToolUtils.Get_ini_data("WddSetting", "poleRow", $"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/WddSetting.ini");
            string poleCol = ToolUtils.Get_ini_data("WddSetting", "poleCol", $"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/WddSetting.ini");
            BSBDataName_Tx.Text = ToolUtils.Get_ini_data("WddSetting", "BsbDataName", $"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/WddSetting.ini");
            if (string.IsNullOrEmpty(poleRow) || string.IsNullOrEmpty(poleCol))
                return;
            PoleRow_Tx.Text = poleRow;
            PoleCol_Tx.Text = poleCol;
            GenerateModule(int.Parse(poleRow), int.Parse(poleCol));
        }

        private void PLC_heartBeatEvent(bool status)
        {
            try
            {
                base.Invoke(new MethodInvoker(delegate
                {
                    this.plcConnectState.BackColor = status ? Color.Green : Color.GreenYellow;
                }));
            }
            catch (Exception exception)
            {
                outDiary("心跳显示异常:" + exception.Message, "错误");
            }
        }

        #endregion

        #region 系统语言
        CultureInfo CultureInfo;
        private void 中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].Language = "中";

            ResourceHandler.dparamParameters.Language = LanguageType.简体中文;
            switch (ResourceHandler.dparamParameters.Language)
            {
                case LanguageType.简体中文:
                    CultureInfo = new CultureInfo("");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //CultureInfo = new CultureInfo("");
                    break;
                case LanguageType.英语:
                    CultureInfo = new CultureInfo("en");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");

                    break;
                case LanguageType.德语:
                    CultureInfo = new CultureInfo("de");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");
                    break;
                default:
                    break;
            }
            ResourceHandler.listSystemParameters[0].Language = "中";
            userResourceHandler.language = "中";

            保存ToolStripMenuItem_Click();
            diary_Log.Text = Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, diary_Log.Text);
            Form_language_loading();
            Semaphore_language_loading();
            //Application.ExitThread();
            //Thread thtmp = new Thread(new ParameterizedThreadStart(run));
            //object appName = Application.ExecutablePath;
            //Thread.Sleep(1);
            //thtmp.Start(appName);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].Language = "英";

            ResourceHandler.dparamParameters.Language = LanguageType.英语;
            switch (ResourceHandler.dparamParameters.Language)
            {
                case LanguageType.简体中文:
                    CultureInfo = new CultureInfo("");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //CultureInfo = new CultureInfo("");
                    break;
                case LanguageType.英语:
                    CultureInfo = new CultureInfo("en");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");

                    break;
                case LanguageType.德语:
                    CultureInfo = new CultureInfo("de");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");
                    break;
                default:
                    break;
            }
            ResourceHandler.listSystemParameters[0].Language = "英";
            userResourceHandler.language = "英";

            保存ToolStripMenuItem_Click();
            diary_Log.Text = Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, diary_Log.Text);
            Form_language_loading();
            Semaphore_language_loading();
            //Application.ExitThread();
            //Thread thtmp = new Thread(new ParameterizedThreadStart(run));
            //object appName = Application.ExecutablePath;
            //Thread.Sleep(1);
            //thtmp.Start(appName);
        }

        private void germanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].Language = "德";

            ResourceHandler.dparamParameters.Language = LanguageType.德语;
            switch (ResourceHandler.dparamParameters.Language)
            {
                case LanguageType.简体中文:
                    CultureInfo = new CultureInfo("");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //CultureInfo = new CultureInfo("");
                    break;
                case LanguageType.英语:
                    CultureInfo = new CultureInfo("en");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");

                    break;
                case LanguageType.德语:
                    CultureInfo = new CultureInfo("de");
                    Thread.CurrentThread.CurrentUICulture = CultureInfo;
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");
                    break;
                default:
                    break;
            }
            ResourceHandler.listSystemParameters[0].Language = "德";
            userResourceHandler.language = "德";

            保存ToolStripMenuItem_Click();
            diary_Log.Text = Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, diary_Log.Text);
            Form_language_loading();
            Semaphore_language_loading();
            //Application.ExitThread();
            //Thread thtmp = new Thread(new ParameterizedThreadStart(run));
            //object appName = Application.ExecutablePath;
            //Thread.Sleep(1);
            //thtmp.Start(appName);
        }

        private void run(Object obj)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.Start();
        }

        #endregion

        #region 用户登录

        private void 用户登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logIn logIn = new logIn();
            logIn.language();
            logIn.ShowDialog();



            if (userResourceHandler.userParameters.userName != null)
            {
                user_LogIn();
                userResourceHandler.dparamParameters.loginTime = DateTime.Now;

                ResourceHandler.dparamParameters.loginThread = new Thread(quitLoginThread);
                ResourceHandler.dparamParameters.loginThread.Start();
            }
        }

        private void 刷卡登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            slotCardLogIn slotCardLogIn = new slotCardLogIn();
            slotCardLogIn.langage();
            slotCardLogIn.ShowDialog();

            if (userResourceHandler.userParameters.userName != null)
            {
                user_LogIn();
                userResourceHandler.dparamParameters.loginTime = DateTime.Now;

                ResourceHandler.dparamParameters.loginThread = new Thread(quitLoginThread);
                ResourceHandler.dparamParameters.loginThread.Start();
            }
        }

        private void 注销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate {
                user_Name.Text = " ";
                userResourceHandler.userParameters.userName = null;
                用户登录ToolStripMenuItem.Enabled = true;
                刷卡登录ToolStripMenuItem.Enabled = true;
                注销ToolStripMenuItem.Enabled = false;
                operation_ban();
            }));
            ResourceHandler.dparamParameters.loginThread.Abort();
            ResourceHandler.dparamParameters.loginThread = null;

            operation_ban();
        }

        /*private void user_LogIn()
        {
            user_Name.Text = userResourceHandler.userParameters.userName;
            用户登录ToolStripMenuItem.Enabled = false;
            刷卡登录ToolStripMenuItem.Enabled = false;
            注销ToolStripMenuItem.Enabled = true;

            if (userResourceHandler.userParameters.userPermissions.Contains("OPN操作员"))
            {
                groupBox9.Enabled = true;

            }
            else if (userResourceHandler.userParameters.userPermissions.Contains("OPN技师"))
            {
                groupBox9.Enabled = true;
                toolStripDropDownButton3.Enabled = true;
            }
            else if (userResourceHandler.userParameters.userPermissions == "PE" || userResourceHandler.userParameters.userPermissions.Contains("ME"))
            {

                groupBox9.Enabled = true;
                toolStripDropDownButton3.Enabled = true;
                toolStripDropDownButton1.Enabled = true;
                toolStripDropDownButton2.Enabled = true;
                btnUP.Enabled = true;
            }
            else if (userResourceHandler.userParameters.userPermissions == "管理员")
            {
                groupBox9.Enabled = true;
                toolStripDropDownButton3.Enabled = true;
                toolStripDropDownButton1.Enabled = true;
                toolStripDropDownButton2.Enabled = true;
                btnUP.Enabled = true;
            }

            outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + user_Name.Text + "   " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "权限") + ":" + userResourceHandler.userParameters.userPermissions, "信息");
        }*/
        private void user_LogIn()
        {
            user_Name.Text = userResourceHandler.userParameters.userName;
            用户登录ToolStripMenuItem.Enabled = false;
            刷卡登录ToolStripMenuItem.Enabled = false;
            注销ToolStripMenuItem.Enabled = true;

            if (userResourceHandler.userParameters.userPermissions.Contains("OPN操作员"))
            {
                //groupBox9.Enabled = false;//手动上传功能组
                dateTimePicker1.Enabled = false;//选择日期
                textBox1.Enabled = false;//输入条码
                cmbInterfaceType.Enabled = false;//选择功能
                btnSerach.Enabled = false;//查询按钮
                btnUP.Enabled = false;//上传按钮
                toolStripDropDownButton3.Enabled = true;//查看按钮
                toolStripDropDownButton1.Enabled = false;//设置按钮
                toolStripDropDownButton2.Enabled = false;//管理按钮
                用户管理ToolStripMenuItem.Enabled = false;//用户管理,仅管理员可用
            }
            else if (userResourceHandler.userParameters.userPermissions.Contains("OPN技师"))
            {
                //groupBox9.Enabled = false;//手动上传功能组
                dateTimePicker1.Enabled = true;//选择日期
                textBox1.Enabled = true;//输入条码
                cmbInterfaceType.Enabled = false;//选择功能
                btnSerach.Enabled = false;//查询按钮
                btnUP.Enabled = false;//上传按钮
                toolStripDropDownButton3.Enabled = true;//查看按钮
                toolStripDropDownButton1.Enabled = false;//设置按钮
                toolStripDropDownButton2.Enabled = false;//管理按钮
                用户管理ToolStripMenuItem.Enabled = false;//用户管理,仅管理员可用
            }
            else if (/*userResourceHandler.userParameters.userPermissions == "PE"||*/  userResourceHandler.userParameters.userPermissions.Contains("ME"))
            {

                //groupBox9.Enabled = true;//手动上传功能组
                dateTimePicker1.Enabled = true;//选择日期
                textBox1.Enabled = true;//输入条码
                cmbInterfaceType.Enabled = true;//选择功能
                btnSerach.Enabled = true;//查询按钮
                btnUP.Enabled = true;//上传按钮
                toolStripDropDownButton3.Enabled = true;//查看按钮
                toolStripDropDownButton1.Enabled = true;//设置按钮
                toolStripDropDownButton2.Enabled = true;//管理按钮
                用户管理ToolStripMenuItem.Enabled = false;//用户管理,仅管理员可用
            }
            else if (userResourceHandler.userParameters.userPermissions == "管理员")
            {
                //groupBox9.Enabled = true;//手动上传功能组
                dateTimePicker1.Enabled = true;//选择日期
                textBox1.Enabled = true;//输入条码
                cmbInterfaceType.Enabled = true;//选择功能
                btnSerach.Enabled = true;//查询按钮
                btnUP.Enabled = true;//上传按钮
                toolStripDropDownButton3.Enabled = true;//查看按钮
                toolStripDropDownButton1.Enabled = true;//设置按钮
                toolStripDropDownButton2.Enabled = true;//管理按钮
                用户管理ToolStripMenuItem.Enabled = true;//用户管理,仅管理员可用
            }

            outDiary(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + user_Name.Text + "   " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "权限") + ":" + userResourceHandler.userParameters.userPermissions, "信息");
        }

        private void quitLoginThread()
        {
            while (true)
            {
                Thread.Sleep(180000);
                //Thread.Sleep(10000);

                System.TimeSpan time = userResourceHandler.dparamParameters.loginTime - DateTime.Now;

                double getMinute = time.TotalMinutes;

                if (Math.Abs(getMinute) > Convert.ToDouble(userResourceHandler.userParameters.cancellationTime))
                {
                    注销ToolStripMenuItem_Click(null, null);
                }
                用户管理ToolStripMenuItem.Enabled = false;
                工位切换ToolStripMenuItem.Enabled = false;
                配方管理ToolStripMenuItem.Enabled = false;
                pLC参数设置ToolStripMenuItem.Enabled = false;
                MES配置ToolStripMenuItem.Enabled = false;
                pLC交互表配置ToolStripMenuItem1.Enabled = false;
                离线参数配置ToolStripMenuItem.Enabled = false;
                文件路径设置ToolStripMenuItem.Enabled = false;
                toolStripDropDownButton4.Enabled = false;
                btnUP.Enabled = false;
                工位切换ToolStripMenuItem.Enabled = false;
                pLC参数设置ToolStripMenuItem.Enabled = false;
            }
        }

        #endregion

        #region 日记保存

        public void outDiary(string diary_Str, string type)
        {
            diary_Str = Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, diary_Str);

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    AppendProgramLog(diary_Str, type);
                }));
            }
            else
            {
                AppendProgramLog(diary_Str, type);
            }


            if (type.Equals("致命错误"))
                NxLog.LoggingServiceText.CriticalError(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("调试"))
                NxLog.LoggingServiceText.Debug(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("错误"))
                NxLog.LoggingServiceText.Error(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("信息"))
                NxLog.LoggingServiceText.Info(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("警告"))
                NxLog.LoggingServiceText.Warn(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
        }

        private void AppendProgramLog(string diary_Str, string type)
        {
            if (diary_Log.TextLength > 500000)
            {
                diary_Log.Clear();
            }

            diary_Log.SelectionStart = diary_Log.TextLength;
            diary_Log.SelectionLength = 0;
            diary_Log.SelectionColor = type.Equals("错误") || type.Equals("致命错误")
                ? Color.Red
                : Color.Black;
            diary_Log.AppendText(DateTime.Now.ToString() + ":" + diary_Str + "\r\n");
            diary_Log.SelectionColor = Color.Black;
            diary_Log.ScrollToCaret();
        }

        public void MESoutDiary(string diary_Str, string type)
        {
            diary_Str = Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, diary_Str);
            richTextBox1.AppendText(DateTime.Now.ToString() + ":" + diary_Str + "\r\n");

            if (type.Equals("致命错误"))
                NxLog.LoggingServiceText.CriticalError(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("调试"))
                NxLog.LoggingServiceText.Debug(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("错误"))
                NxLog.LoggingServiceText.Error(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("信息"))
                NxLog.LoggingServiceText.Info(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
            else if (type.Equals("警告"))
                NxLog.LoggingServiceText.Warn(diary_Str, Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, type));
        }

        public void 保存ToolStripMenuItem_Click()
        {
            XmlHelper xmlHelper = new XmlHelper("xml/systemParameters.xml");
            bool SystemBool = xmlHelper.Write(ResourceHandler.listSystemParameters);
            if (!SystemBool)
            {
                MessageBox.Show("系统文件保存失败!\nSystem file saved successfully!\nSystemdatei erfolgreich gespeichert!");
            }
        }

        #endregion

        #region 显示

        private void pLC信号灯ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.pLCSignalL_UI.ShowDialog();
        }

        private void 程序日记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Path = ResourceHandler.listSystemParameters[0].ProgramLogPath;
            System.Diagnostics.Process.Start("explorer.exe", Path);
        }

        private void MES交互信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Path = ResourceHandler.listSystemParameters[0].MESLogPath;
            System.Diagnostics.Process.Start("explorer.exe", Path);
        }

        #endregion

        #region 管理

        #region 用户管理

        private void 用户管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            User_UI user_UI = new User_UI();
            user_UI.language();
            user_UI.ShowDialog();
        }

        private void 配方管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.formulaUI.language();
            ResourceHandler.dparamParameters.formulaUI.ShowDialog();
        }

        #endregion

        #region 工位切换

        private void 侧缝焊接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "侧缝焊接";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }

        private void 激光刻码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "激光刻码";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }

        private void 极柱拍照ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "极柱拍照";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }

        private void cCS上料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "CCS上料";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }

        private void bSB焊接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "涂胶检查";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }

        private void 镍片焊接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "镍片焊接";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }
        private void 水冷板焊接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "水冷板焊接";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }

        private void 焊后除尘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "焊后除尘";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }
        private void 焊中检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "焊中检测";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }
        private void 焊后铣削ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].currentLocation = "焊后铣削";

            保存ToolStripMenuItem_Click();

            Process process = Process.GetCurrentProcess();

            process.Close();

            Application.Restart();
        }
        #endregion

        #endregion

        #region 参数设置

        private void pLC参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == ResourceHandler.dparamParameters.set_PlcUI)
            {
                ResourceHandler.dparamParameters.set_PlcUI = new set_PLC(this);
            }
            ResourceHandler.dparamParameters.set_PlcUI.language();
            ResourceHandler.dparamParameters.set_PlcUI.ShowDialog();
        }

        private void 文件路径设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.filePathUI = new FilePathUI();
            ResourceHandler.dparamParameters.filePathUI.language();
            ResourceHandler.dparamParameters.filePathUI.ShowDialog();
        }

        private void 离线参数配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.offLineUI.language();
            ResourceHandler.dparamParameters.offLineUI.ShowDialog();
        }

        private void pLC交互表配置ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.pLCInteractionUI.language();
            ResourceHandler.dparamParameters.pLCInteractionUI.ShowDialog();
        }

        private void 进站配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesPullInUI.language();
            ResourceHandler.dparamParameters.mesPullInUI.ShowDialog();
        }

        private void 贴纸PN库存校验配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesBomInventoryUI.language();
            ResourceHandler.dparamParameters.mesBomInventoryUI.ShowDialog(this);
        }

        private void 组装物料配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesAssembleMaterialUI.language();
            ResourceHandler.dparamParameters.mesAssembleMaterialUI.ShowDialog(this);
        }

        private void 出站配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesPullOutUI.language();
            ResourceHandler.dparamParameters.mesPullOutUI.ShowDialog();
        }

        private void 首件配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.language();
            ResourceHandler.dparamParameters.mesInitialWorkpieceParametersUI.ShowDialog();
        }

        private void 出站参数表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesPullOutUploadingUI.language();
            ResourceHandler.dparamParameters.mesPullOutUploadingUI.ShowDialog();

            //Ini_Data();
        }
        private void fTP参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.FtpConfigUI = new FtpConfig_Ui();
            ResourceHandler.dparamParameters.FtpConfigUI.language();
            ResourceHandler.dparamParameters.FtpConfigUI.ShowDialog();
        }
        #endregion

        #region 右击事件

        private void 修改数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 得到鼠标右击的行
            try
            {
                if (dataGridView3.CurrentCell.Value != null)
                {
                    string selectedColumnCount = dataGridView3.CurrentCell.Value.ToString();
                    string header = dataGridView3.Columns[dataGridView3.CurrentCell.ColumnIndex].HeaderText;

                    for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
                    {
                        if (header.Equals(ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i].Cells[0].Value.ToString()))
                        {
                            DataAmend dataAmend = new DataAmend();

                            set_DataGridRowNum(dataAmend.dataGridView1, selectedColumnCount);
                            dataAmend.ShowDialog();
                            string dataAmend_Str = "";
                            foreach (var item in ResourceHandler.dparamParameters.dataAmendParmeters)
                            {
                                dataAmend_Str += item.ToString();
                            }
                            dataGridView3.CurrentCell.Value = dataAmend_Str;
                            return;
                        }
                    }
                }
                MessageBox.Show("数据选择错误，当前数据不支持修改");
            }
            catch (Exception)
            {
                MessageBox.Show("请选择正常数据");
            }

        }

        private void set_DataGridRowNum(DataGridView dataGridView, string selectedColumnCount)
        {
            string[] dataGirdTitleText = Analysis(selectedColumnCount);
            // 提取设置界面的参数数据表里面的表行数据
            for (int i = 0; i < dataGirdTitleText.Length; i++)
            {
                string v = dataGirdTitleText[i].Trim();
                DataGridViewClass.AddRows(dataGridView, new string[] { v }, Color.White);
            }
        }

        private string[] Analysis(string tempStr)
        {
            if (!tempStr.Contains("["))
            {
                tempStr = "[" + tempStr + "]";
            }
            tempStr = tempStr.Replace("[", "");
            string[] str = tempStr.Split(']');
            return str.Take(str.Count() - 1).ToArray();
        }

        #endregion

        #region 数据重传

        private void btnSerach_Click(object sender, EventArgs e)
        {
            switch ("'" + textBox1.Text)
            {
                case "'":// 如果是空的就什么都不查询
                    MessageBox.Show("请在输入框输入想要查询的数据");
                    break;
                default:// 按照搜索内容进行查找
                    // 清理掉原有的表行
                    DataGridViewClass.RemoveAllRow(dataGridView3);

                    // 读取路径下所有的CSV文件名
                    string fileName = ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + Convert.ToDateTime(dateTimePicker1.Value).ToString("yyyy年MM月dd日") + "\\" + textBox1.Text + ".CSV";

                    // 读取CSV表里面的所有数据读取出来
                    List<string> list2 = DataGridViewClass.Read_CSV(fileName);


                    if (list2 != null && list2.Count > 1)
                    {
                        //int i = DataGridViewClass.GetColumnsIndex(list2, textBox1.Text);
                        for (int i = 1; i < list2.Count; i++)
                        {
                            string[] data = list2[index:i].Split(separator:',');
                            data[i] = data[i].Replace("\"", "");
                            //String[] dataswap = data.Where(s => s[0] == '"').Select(s => s.Substring(1, s.Length - 2)).ToArray();
                            //data = dataswap.Length == 0 ? data : dataswap;
                            DataGridViewClass.AddRows(dataGridView:dataGridView3,content:data,BackColor:Color.White);
                        }

                        MessageBox.Show("数据查询完成！");
                        break;
                    }
                    else
                    {
                        MessageBox.Show("没有匹配的数据");
                        break;
                    }
            }

            //dataGridView2.SortOrder = SortOrder.Ascending;
            foreach (DataGridViewColumn c in dataGridView3.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private void btnUP_Click(object sender, EventArgs e)
        {
            try
            {
                string moduleCode = textBox1.Text.Trim();
                int row = dataGridView3.CurrentRow == null ? -1 : dataGridView3.CurrentRow.Index;
                if (string.IsNullOrEmpty(moduleCode))
                {
                    if (row < 0)
                    {
                        MessageBox.Show("请输入条码，或先查询并选择一条数据");
                        return;
                    }
                    moduleCode = DataGridViewClass.GetRowsData(dataGridView3, row)[DataGridViewClass.GetColumnsIndex(dataGridView3, "模组码")];
                }

                if (string.IsNullOrWhiteSpace(moduleCode))
                {
                    MessageBox.Show("条码不能为空");
                    return;
                }

                if (cmbInterfaceType.Text.Equals("手动进站"))
                {
                    ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.PullIn(moduleCode);
                    if (responseData.code != 0)
                    {
                        MESoutDiary($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");
                        return;
                    }
                    MESoutDiary("手动进站成功", "信息");
                }
                else if (cmbInterfaceType.Text.Equals("手动贴纸PN/库存校验"))
                {
                    ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.CheckStickerPnAndInventory(moduleCode, true);
                    if (responseData.code != 0)
                    {
                        MESoutDiary($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\n贴纸PN及库存校验失败", "警告");
                        return;
                    }
                    MESoutDiary("手动贴纸PN及库存校验成功", "信息");
                }
                else if (cmbInterfaceType.Text.Equals("手动组装物料"))
                {
                    ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.AssembleMaterial(moduleCode, true);
                    if (responseData.code != 0)
                    {
                        MESoutDiary($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\n组装物料失败", "警告");
                        return;
                    }
                    MESoutDiary("手动组装物料成功", "信息");
                }
                else if (cmbInterfaceType.Text.Equals("手动出站"))
                {
                    MachineIntegrationServiceService.dataCollectForSfcEx dataCollectForSfcEx = new MachineIntegrationServiceService.dataCollectForSfcEx();
                    dataCollectForSfcEx.SfcDcExRequest = new MachineIntegrationServiceService.sfcDcExRequest();
                    List<MachineIntegrationServiceService.machineIntegrationParametricData> parametricDataList = new List<MachineIntegrationServiceService.machineIntegrationParametricData>();
                    bool state = true;
                    string[] temp = new string[dataGridView2.Columns.Count];

                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "模组码")] = moduleCode;
                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "班次")] = ClassesJudge();
                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "是否首件")] = "否";

                    this.Invoke(new MethodInvoker(delegate
                    {
                        ResourceHandler.dparamParameters.flowClass.ManuallyUploadDataCollection2(temp, row, ref state, parametricDataList);
                        dataCollectForSfcEx.SfcDcExRequest.parametricDataArray = parametricDataList.ToArray();
                        ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.PullOut(moduleCode, dataCollectForSfcEx);
                        DataGridViewClass.AddRows(dataGridView2, temp, Color.White);
                        if (responseData.code != 0)
                        {
                            temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "结果")] = "NG";
                            DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", dataGridView2, moduleCode);
                            MESoutDiary($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");
                            return;
                        }
                        temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "结果")] = "OK";
                        DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", dataGridView2, moduleCode);
                        MESoutDiary("手动出站成功", "信息");
                    }));
                }
                else if (cmbInterfaceType.Text.Equals("手动首件"))
                {
                    dataCollectForResourceFAI dataCollectForResourceFAI = new dataCollectForResourceFAI();
                    dataCollectForResourceFAI.resourceRequest = new dataCollectForResourceFAIRequest();
                    dataCollectForResourceFAI.resourceRequest.parametricDataArray = new machineIntegrationParametricData[2048];
                    string[] temp = new string[dataGridView2.Columns.Count];

                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "模组码")] = moduleCode;
                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "班次")] = ResourceHandler.dparamParameters.flowClass.ClassesJudge();
                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
                    temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "是否首件")] = "否";

                    this.Invoke(new MethodInvoker(delegate
                    {
                        bool state = true;
                        ResourceHandler.dparamParameters.flowClass.ManuallyUploadDataCollection(temp, row, ref state, ref dataCollectForResourceFAI);
                        ResponseData responseData = ResourceHandler.dparamParameters.MesInteraction.InitialWorkpiece(moduleCode, dataCollectForResourceFAI);
                        DataGridViewClass.AddRows(dataGridView2, temp, Color.White);
                        if (responseData.code != 0)
                        {
                            temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "结果")] = "NG";
                            DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", dataGridView2, moduleCode);
                            MESoutDiary($"Code:{responseData.code} \r\nMessage:{responseData.message}\r\n原因:{responseData.Message}\r\n解决办法:{responseData.way}\r\n处理人员:{responseData.personinCharge}\r\nMES审核失败", "警告");
                            return;
                        }
                        temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "结果")] = "OK";
                        DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "\\", dataGridView2, moduleCode);
                        MESoutDiary("手动出站成功", "信息");
                    }));
                }
                else
                {
                    MessageBox.Show("请选择上传模式");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("请选择上传模式");
            }
        }

        #endregion

        private void 电芯校验配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesDX_verifyUI.language();
            ResourceHandler.dparamParameters.mesDX_verifyUI.ShowDialog();
        }

        private void 侧板出站上传参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.mesPullOutSide_plateUI.language();
            ResourceHandler.dparamParameters.mesPullOutSide_plateUI.ShowDialog();
        }

        private void Semaphore_language_loading()
        {
            ResourceManager rs1 = new ResourceManager("HJMSurrenSystem_Siemens.PLCSignalL_UI", typeof(PLCSignalL_UI).Assembly);
            ResourceHandler.dparamParameters.pLCSignalL_UI.label1.Text = rs1.GetString("label1.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label2.Text = rs1.GetString("label2.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label3.Text = rs1.GetString("label3.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label4.Text = rs1.GetString("label4.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label5.Text = rs1.GetString("label5.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label6.Text = rs1.GetString("label6.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label7.Text = rs1.GetString("label7.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label8.Text = rs1.GetString("label8.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label9.Text = rs1.GetString("label9.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label10.Text = rs1.GetString("label10.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label11.Text = rs1.GetString("label11.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label12.Text = rs1.GetString("label12.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label13.Text = rs1.GetString("label13.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label14.Text = rs1.GetString("label14.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label15.Text = rs1.GetString("label15.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label16.Text = rs1.GetString("label16.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label17.Text = rs1.GetString("label17.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label18.Text = rs1.GetString("label18.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label19.Text = rs1.GetString("label19.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label20.Text = rs1.GetString("label20.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label21.Text = rs1.GetString("label21.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label22.Text = rs1.GetString("label22.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label23.Text = rs1.GetString("label23.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label24.Text = rs1.GetString("label24.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label25.Text = rs1.GetString("label25.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label26.Text = rs1.GetString("label26.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label27.Text = rs1.GetString("label27.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label28.Text = rs1.GetString("label28.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label29.Text = rs1.GetString("label29.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label30.Text = rs1.GetString("label30.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label31.Text = rs1.GetString("label31.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label32.Text = rs1.GetString("label32.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label33.Text = rs1.GetString("label33.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label34.Text = rs1.GetString("label34.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label35.Text = rs1.GetString("label35.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label36.Text = rs1.GetString("label36.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label37.Text = rs1.GetString("label37.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label38.Text = rs1.GetString("label38.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label39.Text = rs1.GetString("label39.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label40.Text = rs1.GetString("label40.Text");
            ResourceHandler.dparamParameters.pLCSignalL_UI.label41.Text = rs1.GetString("label41.Text");
        }

        private void Form_language_loading()
        {
            ResourceManager rs = new ResourceManager("HJMSurrenSystem.Main", typeof(Main).Assembly);
            bSB焊接ToolStripMenuItem.Text = rs.GetString("bSB焊接ToolStripMenuItem.Text");
            btnSerach.Text = rs.GetString("btnSerach.Text");
            btnUP.Text = rs.GetString("btnUP.Text");
            cCS上料ToolStripMenuItem.Text = rs.GetString("cCS上料ToolStripMenuItem.Text");
            groupBox1.Text = rs.GetString("groupBox1.Text");
            groupBox10.Text = rs.GetString("groupBox10.Text");
            groupBox2.Text = rs.GetString("groupBox2.Text");
            groupBox9.Text = rs.GetString("groupBox9.Text");
            scanBarcodeLabel.Text = rs.GetString("scanBarcodeLabel.Text") ?? "条码：";
            scanBarcodeLengthLabel.Text = rs.GetString("scanBarcodeLengthLabel.Text") ?? "条码长度：";
            scanSequenceColumn.HeaderText = rs.GetString("scanSequenceColumn.HeaderText") ?? "序号";
            scanBarcodeColumn.HeaderText = rs.GetString("scanBarcodeColumn.HeaderText") ?? "条码";
            scanResultColumn.HeaderText = rs.GetString("scanResultColumn.HeaderText") ?? "结果";
            scanTimeColumn.HeaderText = rs.GetString("scanTimeColumn.HeaderText") ?? "时间";
            MES交互信息ToolStripMenuItem.Text = rs.GetString("MES交互信息ToolStripMenuItem.Text");
            MES配置ToolStripMenuItem.Text = rs.GetString("MES配置ToolStripMenuItem.Text");
            pLC参数设置ToolStripMenuItem.Text = rs.GetString("pLC参数设置ToolStripMenuItem.Text");
            pLC交互表配置ToolStripMenuItem1.Text = rs.GetString("pLC交互表配置ToolStripMenuItem1.Text");
            pLC信号灯ToolStripMenuItem1.Text = rs.GetString("pLC信号灯ToolStripMenuItem1.Text");
            tabPage1.Text = rs.GetString("tabPage1.Text");
            tabPage2.Text = rs.GetString("tabPage2.Text");
            tabPage4.Text = rs.GetString("tabPage4.Text");
            tabPage6.Text = rs.GetString("tabPage6.Text");
            toolStripButton1.Text = rs.GetString("toolStripButton1.Text");
            toolStripDropDownButton1.Text = rs.GetString("toolStripDropDownButton1.Text");
            toolStripDropDownButton2.Text = rs.GetString("toolStripDropDownButton2.Text");
            toolStripDropDownButton3.Text = rs.GetString("toolStripDropDownButton3.Text");
            toolStripDropDownButton4.Text = rs.GetString("toolStripDropDownButton4.Text");
            toolStripLabel10.Text = rs.GetString("toolStripLabel10.Text");
            toolStripLabel13.Text = rs.GetString("toolStripLabel13.Text");
            toolStripLabel16.Text = rs.GetString("toolStripLabel16.Text");
            toolStripLabel4.Text = rs.GetString("toolStripLabel4.Text");
            toolStripLabel6.Text = rs.GetString("toolStripLabel6.Text");
            侧缝焊接ToolStripMenuItem.Text = rs.GetString("侧缝焊接ToolStripMenuItem.Text");
            程序日记ToolStripMenuItem.Text = rs.GetString("程序日记ToolStripMenuItem.Text");
            出站参数表ToolStripMenuItem.Text = rs.GetString("出站参数表ToolStripMenuItem.Text");
            出站配置ToolStripMenuItem.Text = rs.GetString("出站配置ToolStripMenuItem.Text");
            工位切换ToolStripMenuItem.Text = rs.GetString("工位切换ToolStripMenuItem.Text");
            激光刻码ToolStripMenuItem.Text = rs.GetString("激光刻码ToolStripMenuItem.Text");
            极柱拍照ToolStripMenuItem.Text = rs.GetString("极柱拍照ToolStripMenuItem.Text");
            进站配置ToolStripMenuItem.Text = rs.GetString("进站配置ToolStripMenuItem.Text");
            离线参数配置ToolStripMenuItem.Text = rs.GetString("离线参数配置ToolStripMenuItem.Text");
            镍片焊接ToolStripMenuItem.Text = rs.GetString("镍片焊接ToolStripMenuItem.Text");
            配方管理ToolStripMenuItem.Text = rs.GetString("配方管理ToolStripMenuItem.Text");
            首件配置ToolStripMenuItem.Text = rs.GetString("首件配置ToolStripMenuItem.Text");
            刷卡登录ToolStripMenuItem.Text = rs.GetString("刷卡登录ToolStripMenuItem.Text");
            文件路径设置ToolStripMenuItem.Text = rs.GetString("文件路径设置ToolStripMenuItem.Text");
            用户登录ToolStripMenuItem.Text = rs.GetString("用户登录ToolStripMenuItem.Text");
            用户管理ToolStripMenuItem.Text = rs.GetString("用户管理ToolStripMenuItem.Text");
            注销ToolStripMenuItem.Text = rs.GetString("注销ToolStripMenuItem.Text");
            电芯校验配置ToolStripMenuItem.Text = rs.GetString("电芯校验配置ToolStripMenuItem.Text");
            tabPage3.Text = rs.GetString("tabPage3.Text");
            侧板出站上传参数ToolStripMenuItem.Text = rs.GetString("侧板出站上传参数ToolStripMenuItem.Text");
            this.Text = rs.GetString("$this.Text");
            焊后除尘ToolStripMenuItem.Text = rs.GetString("焊后除尘ToolStripMenuItem.Text");
            焊中检测ToolStripMenuItem.Text = rs.GetString("焊中检测ToolStripMenuItem.Text");
            fTP参数设置ToolStripMenuItem.Text = rs.GetString("fTP参数设置ToolStripMenuItem.Text");
            焊后铣削ToolStripMenuItem.Text = rs.GetString("焊后铣削ToolStripMenuItem.Text");
            ApplyHistoryPageLanguage();
        }

        private void Pole_Tx_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }

        private void SetitingPole_Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PoleRow_Tx.Text) || string.IsNullOrEmpty(PoleCol_Tx.Text))
            {
                MessageBox.Show("请输入正确的行列数");
                return;
            }
            int poleRow = int.Parse(PoleRow_Tx.Text);
            int poleCol = int.Parse(PoleCol_Tx.Text);
            if (poleRow < 1 || poleCol < 1)
            {
                MessageBox.Show("请输入正确的行列数");
                return;
            }
            ToolUtils.Set_ini_data("WddSetting", "BsbDataName", BSBDataName_Tx.Text + "", $"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/WddSetting.ini");
            ToolUtils.Set_ini_data("WddSetting", "poleRow", poleRow + "", $"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/WddSetting.ini");
            ToolUtils.Set_ini_data("WddSetting", "poleCol", poleCol + "", $"xml/{workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/WddSetting.ini");
            var panels = tableLayoutPanel1.Controls.OfType<Panel>().ToArray();

            for (int i = 0; i < panels.Length; i++)
            {
                tableLayoutPanel1.Controls.Remove(panels[i]);
            }
            GenerateModule(poleRow, poleCol);
        }

        /// <summary>
        /// 生成极柱图
        /// </summary>
        private void GenerateModule(int row, int col)
        {
            //Panel Pole_Show_Panel = new Panel();
            //Pole_Show_Panel.AutoScroll = true;
            TableLayoutPanel Pole_Show_Panel = new TableLayoutPanel();
            Pole_Show_Panel.Size = new Size((int)(col * 54), row * 55);
            Pole_Show_Panel.Margin = new Padding(0, 0, 0, 0);
            Pole_Show_Panel.AutoScroll = true;
            Pole_Show_Panel.Controls.Clear();
            Pole_Show_Panel.RowCount = row;
            Pole_Show_Panel.ColumnCount = col;
            //Pole_Show_Panel.RowCount=

            for (int k = 0; k < row; k++)
            {
                for (int i = 0; i < col; i++)
                {

                    Panel pan = new Panel();
                    Button lab = new Button();
                    pan.Controls.Add(lab);
                    pan.Margin = new Padding(1, 1, 1, 1);
                    pan.Size = new Size(60, 60);
                    lab.FlatAppearance.BorderSize = 0;
                    lab.FlatStyle = FlatStyle.Flat;
                    lab.FlatAppearance.BorderSize = 0;
                    lab.Size = new Size(60, 60);
                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(0, 0, lab.Width, lab.Height);
                    lab.Region = new Region(gp);
                    //lab.Text = (buttons.Count + 1) + "_OK";
                    lab.Text = BSBDataName_Tx.Text + (buttons.Count + 1) + "_OK ";
                    lab.Tag = true;
                    lab.Font = new Font("宋体", 9, lab.Font.Style | FontStyle.Regular);
                    lab.BackColor = Color.Green;
                    lab.Click += new EventHandler((sender, e) =>
                    {
                        Button btn = (Button)sender;
                        if ((bool)btn.Tag)
                        {
                            btn.Text = btn.Text.Replace("OK", "NG");
                            btn.BackColor = Color.Red;
                            btn.Tag = false;
                        }
                        else
                        {
                            btn.Text = btn.Text.Replace("NG", "OK");
                            btn.BackColor = Color.Green;
                            btn.Tag = true;
                        }
                        GraphicsPath btnGp = new GraphicsPath();
                        btnGp.AddEllipse(0, 0, btn.Width, btn.Height);
                        btn.Region = new Region(btnGp);
                    });
                    buttons.Add(lab);

                    Pole_Show_Panel.Controls.Add(pan); // 修改这里

                }
            }
            tableLayoutPanel1.Controls.Add(Pole_Show_Panel);
        }

        #region 查询极柱信息
        bool isProcessing = false;
        bool isDownload = false;
        private void Wdd_Select_Btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (string.IsNullOrEmpty(WddCode_Box.Text))
            {
                MessageBox.Show("请输入模组码！！");
                Cursor.Current = Cursors.Default;
                return;
            }
            if (isProcessing)
            {
                // 如果正在处理就返回
                MessageBox.Show("处理中，请稍等！！");
                Cursor.Current = Cursors.Default;
                return;
            }
            isProcessing = true;
            string dateTime = Wdd_DateTimePicker.Value.ToString("yyyy年MM月dd日");
            List<string> bsbData = DataGridViewClass.Read_CSV($"{ResourceHandler.listSystemParameters[0].ProgramLogPath}\\BSB\\{dateTime}\\{WddCode_Box.Text}.csv");
            bool uploadFlag = true;
            if (bsbData == null || bsbData.Count == 0)
            {
                // 先去读有没有对应模组的数据，没有就去获取BSB焊接数据
                try
                {
                    if (ftpClient1 != null)
                        ftpClient1.Download($"{ResourceHandler.listSystemParameters[0].ProgramLogPath}\\BSB\\{dateTime}\\{WddCode_Box.Text}.csv", $"出站数据\\{dateTime}\\{WddCode_Box.Text}.csv", null);
                    else
                        uploadFlag = false;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("file not found"))
                    {
                        outDiary($"未查到该模组码信息，模组码{WddCode_Box.Text}，ftp状态：{uploadFlag}：", "错误");
                        MessageBox.Show("未查到该模组码信息！！");
                        isProcessing = false;
                        Cursor.Current = Cursors.Default;
                        return;
                    }
                    outDiary("焊接数据下载失败：" + ex.Message, "错误");
                    MessageBox.Show("焊接数据下载失败：" + ex.Message);
                    isProcessing = false;
                    Cursor.Current = Cursors.Default;
                    return;
                }
            }
            if (!uploadFlag)
            {
                outDiary($"未查到该模组码信息，模组码{WddCode_Box.Text}，ftp状态：{uploadFlag}：", "错误");
                MessageBox.Show("未查到该模组码信息！！");
                isProcessing = false;
                Cursor.Current = Cursors.Default;
                return;
            }
            // 这里是因为有可能本地已经有了这个模组数据文件，与下面的是针对不一样的情况，一个针对已经下载过的，一个是刚下载的
            if (null == bsbData || bsbData.IsEmpty())
            {
                // 这里是下载后重新读取数据
                bsbData = DataGridViewClass.Read_CSV($"{ResourceHandler.listSystemParameters[0].ProgramLogPath}\\BSB\\{dateTime}\\{WddCode_Box.Text}.csv");
                // 如果数据为空，给出提示
                if (null == bsbData || bsbData.IsEmpty())
                {
                    outDiary($"未查到该模组码结果信息，模组码{WddCode_Box.Text}，ftp状态：{uploadFlag}：", "错误");
                    MessageBox.Show("未查到该模组码结果信息！！");
                    isProcessing = false;
                    Cursor.Current = Cursors.Default;
                    return;
                }
            }
            bool isOk = true;
            WddResultShow(bsbData, WddCode_Box.Text, ref isOk);
            string[] temp = new string[dataGridView2.Columns.Count];
            temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "模组码")] = WddCode_Box.Text;
            temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "班次")] = ClassesJudge();
            temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "设备标识")] = ResourceHandler.listSystemParameters[0].deviceIdentification;
            temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "是否首件")] = "否";
            for (int i = 0; i < ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow dataGridViewRow = ResourceHandler.dparamParameters.mesPullOutUploadingUI.dataGridView1.Rows[i];
                temp[DataGridViewClass.GetColumnsIndex(dataGridView2, dataGridViewRow.Cells[0].Value.ToString())] = getResultsave(bsbData, dataGridViewRow.Cells[0].Value.ToString());
            }
            temp[DataGridViewClass.GetColumnsIndex(dataGridView2, "结果")] = (isOk ? "OK" : "NG");
            DataGridViewClass.AddRows(dataGridView2, temp, Color.White);
            DataGridViewClass.Write_CSV(temp, ResourceHandler.listSystemParameters[0].ProgramLogPath + "\\出站数据\\" + dateTime + "\\", dataGridView2, WddCode_Box.Text);

            isProcessing = false;
            Cursor.Current = Cursors.Default;
        }
        public string ClassesJudge()
        {
            int time = DateTime.Now.Hour;
            if (time < Convert.ToInt16(ResourceHandler.listSystemParameters[0].nightShift) && time >= Convert.ToInt16(ResourceHandler.listSystemParameters[0].dayShift))
                return ResourceHandler.dparamParameters.offLineUI.label3.Text;

            else
                return ResourceHandler.dparamParameters.offLineUI.label8.Text;
        }
        public void WddResultShow(List<string> bsbData, string mouduleCode)
        {
            string[] titles = bsbData[0].Split(',');
            if (2 > bsbData.Count || string.IsNullOrEmpty(bsbData[1]))
            {
                outDiary($"未查到该模组码结果数据信息", "错误");
                MessageBox.Show("未查到该模组码结果数据信息！！");
                return;
            }
            string[] bsbDatas = bsbData[1].Split(',');
            List<string> results = new List<string>();
            for (int i = 0; i < titles.Length; i++)
            {
                if (titles[i].Contains(BSBDataName_Tx.Text))
                {
                    if (string.IsNullOrEmpty(bsbDatas[i]))
                    {
                        continue;
                    }
                    // 一列内容可能有多个数据
                    string[] dataArray = bsbDatas[i].Split('，');
                    if (null != dataArray && 0 < dataArray.Length)
                    {
                        foreach (var item in dataArray)
                        {
                            results.Add(item.Trim());
                        }
                    }
                }
            }
            if (results.Count != buttons.Count)
            {
                outDiary($"检测结果和设置数量不一致  检测结果数量：{results.Count}  设置数量：{buttons.Count}", "信息");
                MessageBox.Show($"检测结果和设置焊量不一致  结果数量：{results.Count}  设置数量：{buttons.Count}");
            }
            Lab_Green_Btn.BackColor = Color.Brown;
            Lab_Green_Btn.BackColor = Color.Green;

            WddCode_Box.Text = mouduleCode;
            bool flag = true;
            for (int i = 0; i < buttons.Count; i++)
            {
                string result = "";
                if (i < results.Count)
                {
                    result = results[i];
                }
                if (result.Equals("0") || result.Equals("OK"))
                {
                    buttons[i].Text = (BSBDataName_Tx.Text + (i + 1) + "_OK");
                    buttons[i].BackColor = Color.Green;
                    buttons[i].Tag = true;
                }
                else if (result.Equals("1") || result.Equals("NG"))
                {
                    buttons[i].Text = (BSBDataName_Tx.Text + (i + 1) + "_NG");
                    buttons[i].BackColor = Color.Red;
                    buttons[i].Tag = false;
                    flag = false;
                }
                else
                {
                    buttons[i].Text = (BSBDataName_Tx.Text + (i + 1) + "_ERR");
                    buttons[i].BackColor = Color.Yellow;
                    buttons[i].Tag = false;
                    flag = false;
                }
            }
            if (flag)
                Lab_Green_Btn.BackColor = Color.Lime;
            else
                Lab_Red_Btn.BackColor = Color.Red;
        }

        /// <summary>
        /// 根据焊接寄过渲染极柱图
        /// </summary>
        /// <param name="bsbData"></param>
        /// <param name="mouduleCode"></param>
        /// <param name="isOk">// 如果焊接结果是空，那么本地记录的结果就是NG；不为空的话，只有全部OK才结果OK；</param>
        public void WddResultShow(List<string> bsbData, string mouduleCode, ref bool isOk)
        {
            string[] titles = bsbData[0].Split(',');
            if (2 > bsbData.Count || string.IsNullOrEmpty(bsbData[1]))
            {
                isOk = false;
                outDiary($"未查到该模组码结果数据信息", "错误");
                MessageBox.Show("未查到该模组码结果数据信息！！");
                return;
            }
            string[] bsbDatas = bsbData[1].Split(',');
            List<string> results = new List<string>();
            for (int i = 0; i < titles.Length; i++)
            {
                if (titles[i].Contains(BSBDataName_Tx.Text))
                {
                    if (string.IsNullOrEmpty(bsbDatas[i]))
                    {
                        continue;
                    }
                    // 一列内容可能有多个数据
                    string[] dataArray = bsbDatas[i].Split('，');
                    if (null != dataArray && 0 < dataArray.Length)
                    {
                        foreach (var item in dataArray)
                        {
                            results.Add(item.Trim());
                        }
                    }
                }
            }
            if (results.Count != buttons.Count)
            {
                if (results.Count == 0)
                {
                    isOk = false;
                }
                outDiary($"检测结果和设置数量不一致  检测结果数量：{results.Count}  设置数量：{buttons.Count}", "信息");
                MessageBox.Show($"检测结果和设置焊量不一致  结果数量：{results.Count}  设置数量：{buttons.Count}");
            }
            Lab_Green_Btn.BackColor = Color.Brown;
            Lab_Green_Btn.BackColor = Color.Green;

            WddCode_Box.Text = mouduleCode;
            bool flag = true;
            for (int i = 0; i < buttons.Count; i++)
            {
                string result = "";
                if (i < results.Count)
                {
                    result = results[i];
                }
                if (result.Equals("0") || result.Equals("OK"))
                {
                    buttons[i].Text = (BSBDataName_Tx.Text + (i + 1) + "_OK");
                    buttons[i].BackColor = Color.Green;
                    buttons[i].Tag = true;
                }
                else if (result.Equals("1") || result.Equals("NG"))
                {
                    buttons[i].Text = (BSBDataName_Tx.Text + (i + 1) + "_NG");
                    buttons[i].BackColor = Color.Red;
                    buttons[i].Tag = false;
                    flag = false;
                    isOk = false;
                }
                else
                {
                    buttons[i].Text = (BSBDataName_Tx.Text + (i + 1) + "_ERR");
                    buttons[i].BackColor = Color.Yellow;
                    buttons[i].Tag = false;
                    flag = false;
                    isOk = false;
                }
            }
            if (flag)
                Lab_Green_Btn.BackColor = Color.Lime;
            else
                Lab_Red_Btn.BackColor = Color.Red;
        }

        public string getResultsave(List<string> bsbData, string name)
        {
            string[] titles = bsbData[0].Split(',');
            if (2 > bsbData.Count || string.IsNullOrEmpty(bsbData[1]))
            {
                return "";
            }
            string[] bsbDatas = bsbData[1].Split(',');
            List<string> results = new List<string>();
            for (int i = 0; i < titles.Length; i++)
            {
                if (name.Equals(titles[i]))
                {
                    return bsbDatas[i];
                }
            }
            return "";
        }

        public string GetWddResultStr(string wddResult)
        {
            switch (wddResult)
            {
                case "0":
                    return "当前件OK";
                case "1":
                    return "当前件NG";
                    /*case "1":
                        return "当前件OK，无连2报警，无累3报警";
                    case "2":
                        return "当前件NG, 无连2报警，无累3报警";
                    case "3":
                        return "当前件NG，有连2报警，无累3报警";
                    case "4":
                        return "当前件NG，无连2报警，有累3报警";
                    case "5":
                        return "当前件NG，有连2报警，且有累3报警";*/
            }
            return "";
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            ((焊后除尘)ResourceHandler.dparamParameters.flowClass).PullInTrigger("");
        }

        private void cmbInterfaceType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
