using HJMSurrenSystem.Parameters;
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

namespace HJMSurrenSystem.Interface_UI
{
    public partial class OffLineUI : Form
    {
        Main main;

        public OffLineUI(Main main)
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

        private void OffLineUI_Load(object sender, EventArgs e)
        {
            textBox1.Text = ResourceHandler.listSystemParameters[0].deviceIdentification;
            textBox2.Text = ResourceHandler.listSystemParameters[0].dayShift;
            textBox3.Text = ResourceHandler.listSystemParameters[0].nightShift;
            textBox4.Text = ResourceHandler.listSystemParameters[0].ModuleCode;
            textBox5.Text = ResourceHandler.listSystemParameters[0].PNCode;
            textBox6.Text = ResourceHandler.listSystemParameters[0].ModuleType;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResourceHandler.listSystemParameters[0].deviceIdentification = textBox1.Text;
            ResourceHandler.listSystemParameters[0].dayShift = textBox2.Text;
            ResourceHandler.listSystemParameters[0].nightShift = textBox3.Text;
            ResourceHandler.listSystemParameters[0].ModuleCode = textBox4.Text;
            ResourceHandler.listSystemParameters[0].PNCode = textBox5.Text;
            ResourceHandler.listSystemParameters[0].ModuleType = textBox6.Text;

            main.保存ToolStripMenuItem_Click();
        }
    }
}
