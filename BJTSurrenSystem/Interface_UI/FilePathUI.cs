using DataModel;
using BJTSurrenSystem.Parameters;
using language;
using NxLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserManagement.userLogIn;

namespace BJTSurrenSystem.Interface_UI
{
    public partial class FilePathUI : Form
    {
        public FilePathUI()
        {
            InitializeComponent();
            if (ResourceHandler.listSystemParameters[0].LogClearTime.Equals("90"))
                checkedListBox1.SetItemCheckState(1, CheckState.Checked);
            else if (ResourceHandler.listSystemParameters[0].LogClearTime.Equals("180"))
                checkedListBox1.SetItemCheckState(2, CheckState.Checked);
            else if (ResourceHandler.listSystemParameters[0].LogClearTime.Equals("360"))
                checkedListBox1.SetItemCheckState(3, CheckState.Checked);
            else
                checkedListBox1.SetItemCheckState(0, CheckState.Checked);
        }

        private void FilePathUI_Load(object sender, EventArgs e)
        {
            label3.Text = ResourceHandler.listSystemParameters[0].ProgramLogPath;
            label4.Text = ResourceHandler.listSystemParameters[0].MESLogPath;
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

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                label3.Text = folderBrowserDialog.SelectedPath;
                ResourceHandler.listSystemParameters[0].ProgramLogPath = folderBrowserDialog.SelectedPath;
                MessageBox.Show("程序日记路径修改成功！");
            }

            this.Invoke(new MethodInvoker(delegate
            {
                NxLog.LoggingServiceText.Dispose();

                LoggingServiceText.setPath(ResourceHandler.listSystemParameters[0].ProgramLogPath);
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                label4.Text = folderBrowserDialog.SelectedPath;
                ResourceHandler.listSystemParameters[0].MESLogPath = folderBrowserDialog.SelectedPath;
                MessageBox.Show("MES数据保存路径修改成功！");
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string logClearTime = checkedListBox_Set(checkedListBox1);
            ResourceHandler.listSystemParameters[0].LogClearTime = logClearTime;
            NxLog.LoggingServiceText.CriticalError(Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "登录用户") + ":" + userResourceHandler.userParameters.userName + "     " + Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "用户修改日记清除时间"+":"+logClearTime+"day"), Method.StringToLanguage(ResourceHandler.listSystemParameters[0].Language, "信息"));
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (i != e.Index)//除去当前选中项其余都处于未选中状态
                {
                    checkedListBox1.SetItemCheckState(i, System.Windows.Forms.CheckState.Unchecked);
                }
            }
        }

        private string checkedListBox_Set(CheckedListBox checkedListBox)
        {
            if (null == checkedListBox || null == checkedListBox.SelectedItem)
            {
                return "30";
            }
            if (checkedListBox.SelectedItem.ToString().Equals("1month"))
                return "30";
            if (checkedListBox.SelectedItem.ToString().Equals("3month"))
                return "90";
            if (checkedListBox.SelectedItem.ToString().Equals("6month"))
                return "180";
            if (checkedListBox.SelectedItem.ToString().Equals("12month"))
                return "360";
            return "30";
        }

        private void FilePathUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            XmlHelper xmlHelper = new XmlHelper("xml/systemParameters.xml");
            bool SystemBool = xmlHelper.Write(ResourceHandler.listSystemParameters);
            if (!SystemBool)
            {
                MessageBox.Show("系统文件保存失败!\nSystem file saved successfully!\nSystemdatei erfolgreich gespeichert!");
            }
        }
    }
}
