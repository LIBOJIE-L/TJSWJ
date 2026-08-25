using DataModel;
using BJTSurrenSystem.Parameters;
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

namespace BJTSurrenSystem.Siemens
{
    public partial class set_PLC : Form
    {
        Main main;
        public set_PLC(Main main)
        {
            InitializeComponent();
            this.main = main;
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
            ResourceHandler.listPlcParameters[0].IP = ResourceHandler.dparamParameters.set_PlcUI.PLC_IP.Text;

            ResourceHandler.listPlcParameters[0].PLCmodel = ResourceHandler.dparamParameters.set_PlcUI.PLC_model.Text;

            XmlHelper xmlHelper = new XmlHelper($"xml/{main.workstationName}/PLCParameterSetting.xml");
            bool SystemBool = xmlHelper.Write(ResourceHandler.listPlcParameters);
            if (!SystemBool)
            {
                MessageBox.Show("系统文件保存失败!\nSystem file saved successfully!\nSystemdatei erfolgreich gespeichert!");
            }
        }

        private void set_PLC_Load(object sender, EventArgs e)
        {

        }
    }
}
