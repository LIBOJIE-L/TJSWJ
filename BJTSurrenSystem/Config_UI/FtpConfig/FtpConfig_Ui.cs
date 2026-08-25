using DataModel.CSV;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using UserManagement.userLogIn;

namespace BJTSurrenSystem.Config_UI.FtpConfig
{
    public partial class FtpConfig_Ui : Form
    {
        public FtpConfig_Ui()
        {
            InitializeComponent();
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

        private void Ftp1_IsEnabled_Cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;

            if (cb.Checked)
            {
                if ("Ftp1".Equals(cb.Tag.ToString()))
                {
                    Ftp1_GroupBox.Enabled = true;
                }
                else
                {
                    Ftp2_GroupBox.Enabled = true;
                }
            }
            else
            {
                if ("Ftp1".Equals(cb.Tag.ToString()))
                {
                    Ftp1_GroupBox.Enabled = false;
                }
                else
                {
                    Ftp2_GroupBox.Enabled = false;
                }
            }
            FtpParam ftpParam = new FtpParam();
            ftpParam.FtpIP = Ftp1Ip_Tx.Text;
            ftpParam.FtpPath = Ftp1Path_Tx.Text;
            ftpParam.FtpName = Ftp1Name_Tx.Text;
            ftpParam.FtpPwd = Ftp1Pwd_Tx.Text;
            ftpParam.IsEnabled = Ftp1_IsEnabled_Cb.Checked;
            ToolUtils.SetObjToIniData("FtpConfig1", ftpParam, "xml/FtpConfigs.ini");
            ftpParam.FtpIP = Ftp2Ip_Tx.Text;
            ftpParam.FtpPath = Ftp2Path_Tx.Text;
            ftpParam.FtpName = Ftp2Name_Tx.Text;
            ftpParam.FtpPwd = Ftp2Pwd_Tx.Text;
            ftpParam.IsEnabled = Ftp2_IsEnabled_Cb.Checked;
            ToolUtils.SetObjToIniData("FtpConfig2", ftpParam, "xml/FtpConfigs.ini");
        }

        private void SaveFtp_Btn_Click(object sender, EventArgs e)
        {
            FtpParam ftpParam = new FtpParam();

            Button button = (Button)sender;

            if ("SaveFtp1_Btn".Equals(button.Name))
            {
                ftpParam.FtpIP = Ftp1Ip_Tx.Text;
                ftpParam.FtpPath = Ftp1Path_Tx.Text;
                ftpParam.FtpName = Ftp1Name_Tx.Text;
                ftpParam.FtpPwd = Ftp1Pwd_Tx.Text;
                ftpParam.IsEnabled = Ftp1_IsEnabled_Cb.Checked;

                ToolUtils.SetObjToIniData("FtpConfig1", ftpParam, "xml/FtpConfigs.ini");
                return;
            }
            ftpParam.FtpIP = Ftp2Ip_Tx.Text;
            ftpParam.FtpPath = Ftp2Path_Tx.Text;
            ftpParam.FtpName = Ftp2Name_Tx.Text;
            ftpParam.FtpPwd = Ftp2Pwd_Tx.Text;
            ftpParam.IsEnabled = Ftp2_IsEnabled_Cb.Checked;

            ToolUtils.SetObjToIniData("FtpConfig2", ftpParam, "xml/FtpConfigs.ini");
        }

        private void FtpConfig_Ui_Load(object sender, EventArgs e)
        {
            FtpParam ftpParam1 = ToolUtils.GetObjToIniData<FtpParam>("FtpConfig1", typeof(FtpParam), "xml/FtpConfigs.ini");
            FtpParam ftpParam2 = ToolUtils.GetObjToIniData<FtpParam>("FtpConfig2", typeof(FtpParam), "xml/FtpConfigs.ini");

            //new .GetType().GetProperties(BindingFlags.Public);

            if (ftpParam1.IsEnabled)
            {
                Ftp1Ip_Tx.Text = ftpParam1?.FtpIP;
                Ftp1Path_Tx.Text = ftpParam1?.FtpPath;
                Ftp1Name_Tx.Text = ftpParam1?.FtpName;
                Ftp1Pwd_Tx.Text = ftpParam1?.FtpPwd;
                Ftp1_IsEnabled_Cb.Checked = ftpParam1.IsEnabled;
            }
            if(ftpParam2.IsEnabled)
            {
                Ftp2Ip_Tx.Text = ftpParam2?.FtpIP;
                Ftp2Path_Tx.Text = ftpParam2?.FtpPath;
                Ftp2Name_Tx.Text = ftpParam2?.FtpName;
                Ftp2Pwd_Tx.Text = ftpParam2?.FtpPwd;
                Ftp2_IsEnabled_Cb.Checked = ftpParam2.IsEnabled;

            }
        }
    }
}