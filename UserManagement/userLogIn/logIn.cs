using DataModel;
using language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserManagement.userLogIn
{
    public partial class logIn : Form
    {
        public logIn()
        {
            InitializeComponent();
        }

        private void logIn_Load(object sender, EventArgs e)
        {
            XmlHelper xmlHelper = new XmlHelper("user/userXML.xml");
            bool SystemBool = xmlHelper.Read(ref userResourceHandler.listUserParameters);
            if (!SystemBool)
            {
                MessageBox.Show("用户文件读取失败!\nSystemFileReadError!\nSystem beim Lesen der Sprachdatei!");
            }
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

        private void user_LogIn_Click(object sender, EventArgs e)
        {
            foreach (var item in userResourceHandler.listUserParameters)
            {
                if (user_Account.Text.Equals(item.userName) && user_Password.Text.Equals(item.userPassword))
                {
                    userResourceHandler.userParameters.userName = item.userName;
                    userResourceHandler.userParameters.cancellationTime = item.cancellationTime;
                    userResourceHandler.userParameters.userPermissions = item.userPermissions;
                    this.Close();
                    return;
                }
            }
            MessageBox.Show("账号或密码错误，请重试！");
        }

    }
}
