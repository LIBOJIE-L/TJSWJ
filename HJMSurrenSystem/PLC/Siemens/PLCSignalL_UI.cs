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

namespace HJMSurrenSystem_Siemens
{
    public partial class PLCSignalL_UI : Form
    {
        public PLCSignalL_UI()
        {
            InitializeComponent();
        }

        private void PLCSignalL_UI_Load(object sender, EventArgs e)
        {

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
            this.Controls.Clear();
            InitializeComponent();
        }

        
    }
}