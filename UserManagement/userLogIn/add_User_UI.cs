using BJTSurrenSystem.ShowUI;
using language;
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
using UserManagement.userLogIn;

namespace UserManagement
{
    public partial class add_User_UI : Form
    {
        public add_User_UI()
        {
            InitializeComponent();
        }

        private void add_User_UI_Load(object sender, EventArgs e)
        {
            if (userResourceHandler.userParameters.userPermissions == "管理员")
            {
                checkedListBox2.Items.Insert(0, "OPN操作员");
                checkedListBox2.Items.Insert(1, "OPN技师");
                checkedListBox2.Items.Insert(0, "ME");
                checkedListBox2.Items.Insert(1, "PE");
                checkedListBox2.Items.Insert(0, "管理员");
            }
            else if (userResourceHandler.userParameters.userPermissions == "ME" || userResourceHandler.userParameters.userPermissions == "PE")
            {
                checkedListBox2.Items.Insert(0, "OPN操作员");
                checkedListBox2.Items.Insert(1, "OPN技师");
                checkedListBox2.Items.Insert(0, "ME");
                checkedListBox2.Items.Insert(1, "PE");
            }
            else
            {
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

        private void button1_Click(object sender, EventArgs e)
        {
            string user_Name = textBox1.Text;
            string user_Paw = textBox2.Text;
            string userPermissions = checkedListBox_Set(checkedListBox2);
            string cancellationTime = checkedListBox_Set(checkedListBox1);

            if (user_Name != "" && user_Paw != "" && userPermissions != "" && cancellationTime != "")
            {
                foreach (var item in userResourceHandler.listUserParameters)
                {
                    if (item.userName.Equals(user_Name))
                    {
                        MessageBox.Show("用户名已存在！");
                        return;
                    }
                }

                UserParameters UserParameters = new UserParameters();
                UserParameters.userName = user_Name;
                UserParameters.userPassword = user_Paw;
                UserParameters.userPermissions = userPermissions;
                UserParameters.cancellationTime = cancellationTime;
                userResourceHandler.listUserParameters.Add(UserParameters);

                XmlHelper xmlHelper = new XmlHelper("user/userXML.xml");
                bool saveSucceeded = xmlHelper.Write(userResourceHandler.listUserParameters);
                if (!saveSucceeded)
                {
                    userResourceHandler.listUserParameters.Remove(UserParameters);
                    MessageBox.Show("用户文件保存失败，请检查文件后重试！");
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            else {
                MessageBox.Show("账号、密码、权限、注销时间不能为空！");
            }
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                if (i != e.Index)//除去当前选中项其余都处于未选中状态
                {
                    checkedListBox2.SetItemCheckState(i, System.Windows.Forms.CheckState.Unchecked);
                }
            }
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

        private string checkedListBox_Set(CheckedListBox checkedListBox) {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                if (checkedListBox.GetItemChecked(i))
                {
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("OPN操作员"))
                        return "OPN操作员";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("OPN技师"))
                        return "OPN技师";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("ME"))
                        return "ME";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("PE"))
                        return "PE";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("管理员"))
                        return "管理员";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("3分钟"))
                        return "3";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("5分钟"))
                        return "5";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("10分钟"))
                        return "10";
                    if (checkedListBox.GetItemText(checkedListBox.Items[i]).Equals("15分钟"))
                        return "15";
                }
            }
            return "";
        }
    }
}
