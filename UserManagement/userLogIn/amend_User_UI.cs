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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UserManagement.userLogIn
{
    public partial class amend_User_UI : Form
    {
        public amend_User_UI()
        {
            InitializeComponent();
        }

        private void amend_User_UI_Load(object sender, EventArgs e)
        {
            if (userResourceHandler.userParameters.userPermissions == "OPN技师")
            {
                checkedListBox2.Items.Insert(0, "OPN操作员");
                checkedListBox2.Items.Insert(1, "OPN技师");
            }
            else if (userResourceHandler.userParameters.userPermissions == "管理员")
            {
                checkedListBox2.Items.Insert(0, "OPN操作员");
                checkedListBox2.Items.Insert(1, "OPN技师");
                checkedListBox2.Items.Insert(0, "ME");
                checkedListBox2.Items.Insert(1, "PE");
                checkedListBox2.Items.Insert(0, "管理员");
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
            string user_Name = userResourceHandler.dparamParameters.userName;
            string userPermissions = checkedListBox_Set(checkedListBox2);
            string cancellationTime = checkedListBox_Set(checkedListBox1);
            if (user_Name != "" && userPermissions != "" && cancellationTime != "")
            {
                foreach (var item in userResourceHandler.listUserParameters)
                {
                    if (item.userName.Equals(user_Name))
                    {
                        item.userPermissions = userPermissions;
                        item.cancellationTime = cancellationTime;
                        MessageBox.Show("修改成功！");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("权限、注销时间不能为空！");
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

        private string checkedListBox_Set(CheckedListBox checkedListBox)
        {
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
