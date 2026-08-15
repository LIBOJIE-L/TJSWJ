using DataModel;
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

namespace UserManagement.userLogIn
{
    public partial class slotCardLogIn : Form
    {
        public slotCardLogIn()
        {
            InitializeComponent();
        }

        private void slotCardLogIn_Load(object sender, EventArgs e)
        {
            XmlHelper xmlHelper = new XmlHelper("user/userXML.xml");
            bool SystemBool = xmlHelper.Read(ref userResourceHandler.listUserParameters);
            if (!SystemBool)
            {
                MessageBox.Show("用户文件读取失败!\nSystemFileReadError!\nSystem beim Lesen der Sprachdatei!");
            }
        }

        public void langage() {
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (user_NameAndPassword.Text.Length > 7)
            {
                foreach (var item in userResourceHandler.listUserParameters)
                {
                    if ( user_NameAndPassword.Text.Equals(item.userPassword))
                    {
                        userResourceHandler.userParameters.userName = item.userName;
                        userResourceHandler.userParameters.cancellationTime = item.cancellationTime;
                        userResourceHandler.userParameters.userPermissions = item.userPermissions;
                        this.Close();
                        return;
                    }
                }
            }
        }

       
    }
}
