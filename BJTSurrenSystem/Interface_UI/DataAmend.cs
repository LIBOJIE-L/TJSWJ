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

namespace BJTSurrenSystem.Interface_UI
{
    public partial class DataAmend : Form
    {
        public DataAmend()
        {
            InitializeComponent();

            ResourceHandler.dparamParameters.dataAmendParmeters = new List<string>();
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            索引(textBox1, textBox2, dataGridView1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt16(textBox1.Text) > 0)
                {
                    dataGridView1.Rows[Convert.ToInt32(textBox1.Text) - 1].Cells[0].Value = textBox2.Text;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("数据输入错误");
            }
        }

        private void 索引(TextBox label1, TextBox label2, DataGridView dataGriid)
        {
            int a;
            if (label1.Text == "")
            {
                label2.Text = "";
            }
            else
            if (int.TryParse(label1.Text, out a))
            {// 判断输入的是不是数字
                if ((Convert.ToInt32(label1.Text)-1) >= dataGriid.Rows.Count) { return; }// 判断输入的值有没有超过索引
                label2.Text = dataGriid.Rows[(Convert.ToInt32(label1.Text)-1)].Cells[0].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add("");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.dataAmendParmeters.Clear();
            ResourceHandler.dparamParameters.dataAmendParmeters = new List<string>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null)
                {
                    ResourceHandler.dparamParameters.dataAmendParmeters.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
                }
            }
            this.Hide();
        }
    }
}
