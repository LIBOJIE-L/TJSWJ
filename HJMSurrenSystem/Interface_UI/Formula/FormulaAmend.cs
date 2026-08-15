using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HJMSurrenSystem.Interface_UI.Formula
{
    public partial class FormulaAmend : Form
    {
        public string New_Name = "";

        public FormulaAmend()
        {
            InitializeComponent();
        }

        private void FormulaAmend_Load(object sender, EventArgs e)
        {
            New_Name = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                DialogResult dlgResult = MessageBox.Show(this, "确定创建产品型号为：" + textBox1.Text, "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dlgResult == DialogResult.OK)
                {
                    New_Name = textBox1.Text;
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("产品型号不能为空！");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            New_Name = "";
            this.Close();
        }

    }
}
