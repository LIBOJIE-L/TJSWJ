using DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserManagement;
using UserManagement.userLogIn;

namespace HJMSurrenSystem.ShowUI
{
    public partial class User_UI : Form
    {
        public User_UI()
        {
            InitializeComponent();
        }

        private void User__UI_Load(object sender, EventArgs e)
        {
            DataGridViewClass.SetTitleWidth(dataGridView1);
            XmlHelper xmlHelper = new XmlHelper("user/userXML.xml");
            bool SystemBool = xmlHelper.Read(ref userResourceHandler.listUserParameters);
            if (!SystemBool)
            {
                MessageBox.Show("用户文件读取失败!\nSystemFileReadError!\nSystem beim Lesen der Sprachdatei!");
            }

            this.Invoke(new MethodInvoker(delegate {
                foreach (var item in userResourceHandler.listUserParameters)
                {
                    string[] user_Str = { item.userName, item.userPassword, item.userPermissions, item.cancellationTime };
                    DataGridViewClass.AddRows(dataGridView1, user_Str, Color.White);
                }
            }));
        }

        public void language()
        {
            switch (userResourceHandler.language)
            {
                case "中":
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("");
                    break;
                case "英":
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
                    break;
                case "德":
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("de");
                    break;
                default:
                    break;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (e.Value != null && e.Value.ToString().Length > 0)
                {
                e.Value = new string('*', e.Value.ToString().Length);
                }
            }
        }

        private void add_User_Click(object sender, EventArgs e)
        {
            add_User_UI add_User_UI = new add_User_UI();
            add_User_UI.language();
            if (add_User_UI.ShowDialog() == DialogResult.OK)
            {
                userInterfaceRefresh();
            }
        }

        private void amend_User_Click(object sender, EventArgs e)
        {
            int row = 0;
            try
            {
                row = dataGridView1.CurrentRow.Index; // 行
            }
            catch { 
                MessageBox.Show("所选地方为数据表以外！", "有误的操作"); 
                return; 
            }
            if (userResourceHandler.userParameters.userPermissions != null)
            {
                if (userResourceHandler.userParameters.userPermissions.Equals("管理员"))
                {
                    userResourceHandler.dparamParameters.userName = dataGridView1.Rows[row].Cells[0].Value.ToString();

                    amend_User_UI amend_User_UI = new amend_User_UI();
                    amend_User_UI.language();
                    amend_User_UI.ShowDialog();

                    userInterfaceRefresh();
                }
                else if (userResourceHandler.userParameters.userPermissions.Equals("OPN技师"))
                {
                    if (dataGridView1.Rows[row].Cells[2].Value.ToString().Equals("OPN技师") || dataGridView1.Rows[row].Cells[2].Value.ToString().Equals("OPN操作员"))
                    {
                        userResourceHandler.dparamParameters.userName = dataGridView1.Rows[row].Cells[0].Value.ToString();

                        amend_User_UI amend_User_UI = new amend_User_UI();
                        amend_User_UI.ShowDialog();

                        userInterfaceRefresh();
                    }
                    else
                    {
                        MessageBox.Show("修改权限不足");
                    }
                }
            }
            else {
                MessageBox.Show("未登录");
            }
            
        }

        private void del_User_Click(object sender, EventArgs e)
        {
            int row = 0;
            try
            {
                row = dataGridView1.CurrentRow.Index; // 行
            }
            catch
            {
                MessageBox.Show("所选地方为数据表以外！,有误的操作");
                return;
            }

            if (userResourceHandler.userParameters.userPermissions.Equals("管理员"))
            {
                userResourceHandler.listUserParameters.RemoveAt(row);

                this.Invoke(new MethodInvoker(delegate {
                    DataGridViewClass.RemoveIndexRow(dataGridView1, row);
                }));

                userInterfaceRefresh();
            }
            else if (userResourceHandler.userParameters.userPermissions.Equals("OPN技师"))
            {
                if (dataGridView1.Rows[row].Cells[2].Value.ToString().Equals("OPN技师") || dataGridView1.Rows[row].Cells[2].Value.ToString().Equals("OPN操作员"))
                {
                    userResourceHandler.listUserParameters.RemoveAt(row);

                    this.Invoke(new MethodInvoker(delegate {
                        DataGridViewClass.RemoveIndexRow(dataGridView1, row);
                    }));

                    userInterfaceRefresh();
                }
                else
                {
                    MessageBox.Show("修改权限不足");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            XmlHelper xmlHelper = new XmlHelper("user/userXML.xml");
            bool SystemBool = xmlHelper.Write(userResourceHandler.listUserParameters);
            if (SystemBool)
            {
                MessageBox.Show("用户文件保存成功!\nSystemFileReadError!\nSystem beim Lesen der Sprachdatei!");
            }
        }

        private void exportCsvButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV文件 (*.csv)|*.csv";
                saveFileDialog.DefaultExt = "csv";
                saveFileDialog.AddExtension = true;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = "用户数据_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("用户名,密码,权限,注销时间(分钟)");
                        foreach (UserParameters user in userResourceHandler.listUserParameters)
                        {
                            writer.WriteLine(string.Join(",", new string[]
                            {
                                EscapeCsvField(user.userName),
                                EscapeCsvField(user.userPassword),
                                EscapeCsvField(user.userPermissions),
                                EscapeCsvField(user.cancellationTime)
                            }));
                        }
                    }

                    MessageBox.Show("用户数据导出成功！\n文件中包含用户密码，请妥善保管。", "导出完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("用户数据导出失败：" + ex.Message, "导出失败");
                }
            }
        }

        private void importCsvButton_Click(object sender, EventArgs e)
        {
            string currentPermission = userResourceHandler.userParameters.userPermissions;
            if (currentPermission != "管理员" && currentPermission != "ME" && currentPermission != "PE")
            {
                MessageBox.Show("当前用户没有批量导入用户的权限！", "权限不足");
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV文件 (*.csv)|*.csv|所有文件 (*.*)|*.*";
                openFileDialog.Multiselect = false;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    List<string[]> records = ReadCsvRecords(openFileDialog.FileName);
                    int startIndex = records.Count > 0 && IsUserCsvHeader(records[0]) ? 1 : 0;
                    int invalidCount = 0;
                    int duplicateCount = 0;
                    int permissionDeniedCount = 0;
                    List<UserParameters> importedUsers = new List<UserParameters>();
                    HashSet<string> userNames = new HashSet<string>(
                        userResourceHandler.listUserParameters.Select(user => user.userName),
                        StringComparer.Ordinal);

                    for (int index = startIndex; index < records.Count; index++)
                    {
                        string[] record = records[index];
                        if (IsEmptyCsvRecord(record))
                        {
                            continue;
                        }

                        if (record.Length != 4)
                        {
                            invalidCount++;
                            continue;
                        }

                        string userName = record[0].Trim();
                        string password = record[1];
                        string permission = record[2].Trim();
                        string cancellationTime = NormalizeCancellationTime(record[3]);

                        if (string.IsNullOrWhiteSpace(userName)
                            || string.IsNullOrWhiteSpace(password)
                            || !IsKnownPermission(permission)
                            || !IsKnownCancellationTime(cancellationTime))
                        {
                            invalidCount++;
                            continue;
                        }

                        if (userNames.Contains(userName))
                        {
                            duplicateCount++;
                            continue;
                        }

                        if (!CanImportPermission(currentPermission, permission))
                        {
                            permissionDeniedCount++;
                            continue;
                        }

                        importedUsers.Add(new UserParameters
                        {
                            userName = userName,
                            userPassword = password,
                            userPermissions = permission,
                            cancellationTime = cancellationTime
                        });
                        userNames.Add(userName);
                    }

                    if (importedUsers.Count == 0)
                    {
                        ShowImportResult(0, duplicateCount, invalidCount, permissionDeniedCount);
                        return;
                    }

                    int originalUserCount = userResourceHandler.listUserParameters.Count;
                    userResourceHandler.listUserParameters.AddRange(importedUsers);

                    XmlHelper xmlHelper = new XmlHelper("user/userXML.xml");
                    if (!xmlHelper.Write(userResourceHandler.listUserParameters))
                    {
                        userResourceHandler.listUserParameters.RemoveRange(originalUserCount, importedUsers.Count);
                        MessageBox.Show("批量导入的数据保存失败，未修改原用户数据！", "导入失败");
                        return;
                    }

                    userInterfaceRefresh();
                    ShowImportResult(importedUsers.Count, duplicateCount, invalidCount, permissionDeniedCount);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("用户数据导入失败：" + ex.Message, "导入失败");
                }
            }
        }

        private static string EscapeCsvField(string value)
        {
            string field = value ?? string.Empty;
            if (field.IndexOfAny(new char[] { ',', '"', '\r', '\n' }) >= 0)
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }

        private static List<string[]> ReadCsvRecords(string fileName)
        {
            string csvText;
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8, true))
            {
                csvText = reader.ReadToEnd();
            }

            List<string[]> records = new List<string[]>();
            List<string> fields = new List<string>();
            StringBuilder field = new StringBuilder();
            bool insideQuotes = false;

            for (int index = 0; index < csvText.Length; index++)
            {
                char current = csvText[index];
                if (current == '"')
                {
                    if (insideQuotes && index + 1 < csvText.Length && csvText[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (current == ',' && !insideQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else if ((current == '\r' || current == '\n') && !insideQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                    records.Add(fields.ToArray());
                    fields.Clear();

                    if (current == '\r' && index + 1 < csvText.Length && csvText[index + 1] == '\n')
                    {
                        index++;
                    }
                }
                else
                {
                    field.Append(current);
                }
            }

            if (insideQuotes)
            {
                throw new FormatException("CSV文件中存在未闭合的双引号。");
            }

            if (field.Length > 0 || fields.Count > 0)
            {
                fields.Add(field.ToString());
                records.Add(fields.ToArray());
            }

            return records;
        }

        private static bool IsUserCsvHeader(string[] record)
        {
            if (record == null || record.Length != 4)
            {
                return false;
            }

            string firstColumn = record[0].Trim();
            return firstColumn.Equals("用户名", StringComparison.OrdinalIgnoreCase)
                || firstColumn.Equals("username", StringComparison.OrdinalIgnoreCase)
                || firstColumn.Equals("user name", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsEmptyCsvRecord(string[] record)
        {
            return record == null || record.All(value => string.IsNullOrWhiteSpace(value));
        }

        private static string NormalizeCancellationTime(string value)
        {
            string cancellationTime = (value ?? string.Empty).Trim();
            if (cancellationTime.EndsWith("分钟", StringComparison.Ordinal))
            {
                cancellationTime = cancellationTime.Substring(0, cancellationTime.Length - 2).Trim();
            }

            return cancellationTime;
        }

        private static bool IsKnownPermission(string permission)
        {
            return permission == "管理员"
                || permission == "ME"
                || permission == "PE"
                || permission == "OPN操作员"
                || permission == "OPN技师";
        }

        private static bool IsKnownCancellationTime(string cancellationTime)
        {
            return cancellationTime == "3"
                || cancellationTime == "5"
                || cancellationTime == "10"
                || cancellationTime == "15";
        }

        private static bool CanImportPermission(string currentPermission, string importedPermission)
        {
            if (currentPermission == "管理员")
            {
                return true;
            }

            return (currentPermission == "ME" || currentPermission == "PE")
                && importedPermission != "管理员";
        }

        private static void ShowImportResult(int importedCount, int duplicateCount, int invalidCount, int permissionDeniedCount)
        {
            MessageBox.Show(
                "批量导入完成！\n"
                + "成功导入：" + importedCount + " 条\n"
                + "重复用户名：" + duplicateCount + " 条\n"
                + "无效数据：" + invalidCount + " 条\n"
                + "权限不足：" + permissionDeniedCount + " 条",
                "导入结果");
        }

        private void userInterfaceRefresh() {
            this.Invoke(new MethodInvoker(delegate {
                DataGridViewClass.RemoveAllRow(dataGridView1);
            }));

            this.Invoke(new MethodInvoker(delegate {
                foreach (var item in userResourceHandler.listUserParameters)
                {
                    string[] user_Str = { item.userName, item.userPassword, item.userPermissions, item.cancellationTime };
                    DataGridViewClass.AddRows(dataGridView1, user_Str, Color.White);
                }
            }));
        }

    }
}
