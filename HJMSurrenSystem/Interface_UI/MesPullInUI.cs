using DataModel;
using HJMSurrenSystem.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserManagement.userLogIn;

namespace HJMSurrenSystem.Interface_UI
{
    public partial class MesPullInUI : Form
    {
        Main main;
        public MesPullInUI(Main main)
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

        private void button2_Click(object sender, EventArgs e)
        {
            // 弹出文件选择对话框
            try
            {
                OpenFileDialog openLabelFileDialog = new OpenFileDialog();
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                openLabelFileDialog.Filter = "CSV文件(*.CSV)|*.CSV|所有文件(*.*)|*.*";
                //openLabelFileDialog.InitialDirectory = @"C:\Users\admin\Desktop";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    // 清空当前数据表里面的数据
                    DataGridViewClass.RemoveAllRow(dataGridView1);
                    // 读取外部CSV文件表里面的内容
                    List<string> strTemp = DataGridViewClass.Read_CSV(openLabelFileDialog.FileName);
                    for (int i = 0; i < strTemp.Count; i++)
                    {
                        // 将List转换成String[]
                        DataGridViewClass.AddRows(dataGridView1, strTemp[i].Split(','), Color.White);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件异常：" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 弹出选择文件路径保存对话框
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                saveFileDialog1.Filter = "CSV文件(*.CSV)|*.CSV";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 判断这个路径下是否存在旧的CSV表头文件
                    if (File.Exists(dialog.SelectedPath + "/MES进站参数配置表.CSV"))
                    {
                        File.Delete(dialog.SelectedPath + "/MES进站参数配置表.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/MES进站参数配置表.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
                    // 向文件里面写入数据
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        string[] temp = DataGridViewClass.GetRowsData(dataGridView1, i);

                        string str = "";
                        // 加入 , 号
                        for (int k = 0; k < temp.Length; k++)
                        {
                            if (k == temp.Length - 1)
                            {
                                str = str + temp[k];
                                break;
                            }
                            str = str + temp[k] + ",";
                        }

                        // 写入数据
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
                dialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataGridViewClass.AddRows(dataGridView1, new string[] { }, Color.White);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveIndexRow(dataGridView1, dataGridView1.CurrentRow.Index);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 移除所有行
            DataGridViewClass.RemoveAllRow(dataGridView1);
            // 重新导入表头
            XmlHelper xmlHelper = new XmlHelper("xml/" + main.workstationName.Text + "/MesPullIn.xml");
            bool headerBool = xmlHelper.Read(ref ResourceHandler.listMesPullInParameters);
            if (!headerBool)
            {
                MessageBox.Show("MES进站参数配置文件读取失败!\nPLCInteractiveAddressFileReadError!\nPLCInteractiveAddress beim Lesen der Sprachdatei!");
            }

            foreach (var item in ResourceHandler.listMesPullInParameters)
            {
                string[] type_Str = { item.ParametersName, item.ParametersExplain, item.ParametersPrice };
                DataGridViewClass.AddRows(dataGridView1, type_Str, Color.White);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ResourceHandler.listMesPullInParameters.Clear();

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null ||dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(""))
                    ResourceHandler.mesPullInParameters.ParametersName = "";
                else
                    ResourceHandler.mesPullInParameters.ParametersName = dataGridView1.Rows[i].Cells[0].Value.ToString();

                if (dataGridView1.Rows[i].Cells[1].Value == null || dataGridView1.Rows[i].Cells[1].Value.ToString().Equals(""))
                    ResourceHandler.mesPullInParameters.ParametersExplain = "";
                else
                    ResourceHandler.mesPullInParameters.ParametersExplain = dataGridView1.Rows[i].Cells[1].Value.ToString();

                if (dataGridView1.Rows[i].Cells[2].Value == null || dataGridView1.Rows[i].Cells[2].Value.ToString().Equals(""))
                    ResourceHandler.mesPullInParameters.ParametersPrice = "";
                else
                    ResourceHandler.mesPullInParameters.ParametersPrice = dataGridView1.Rows[i].Cells[2].Value.ToString();

                ResourceHandler.listMesPullInParameters.Add(ResourceHandler.mesPullInParameters);
            }

            XmlHelper xmlHelper = new XmlHelper("xml/" + main.workstationName.Text + "/MesPullIn.xml");
            bool headerBool = xmlHelper.Write(ResourceHandler.listMesPullInParameters);
            if (!headerBool)
            {
                MessageBox.Show("MES进站参数配置文件保存失败!\nPLCInteractiveAddressFileReadError!\nPLCInteractiveAddress beim Lesen der Sprachdatei!");
            }
        }

        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView DataGridView = (DataGridView)sender;
            string updateData = DataGridView.CurrentCell.EditedFormattedValue.ToString();
            string preData = DataGridView.CurrentCell.FormattedValue.ToString();

            string name = DataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();

            main.outDiary($"登陆用户：{main.user_Name.Text}  参数名称：{name}   原始{DataGridView.Columns[e.ColumnIndex].HeaderCell.FormattedValue}：{preData}  修改{DataGridView.Columns[e.ColumnIndex].HeaderCell.FormattedValue}：{updateData}", "信息");
        }
    }
}
