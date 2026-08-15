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
    public partial class MesPullOutSide_plateUI : Form
    {
        Main main;
        public MesPullOutSide_plateUI(Main main)
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
                    if (File.Exists(dialog.SelectedPath + "/MES出站上传参数配置表.CSV"))
                    {
                        File.Delete(dialog.SelectedPath + "/MES出站上传参数配置表.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/MES侧板上传参数配置表.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
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
            XmlHelper xmlHelper = new XmlHelper("xml/" + main.workstationName.Text + "/MesPullOutSide_plate.xml");
            bool headerBool = xmlHelper.Read(ref ResourceHandler.listMesPullOutSide_plateParameters);
            if (!headerBool)
            {
                MessageBox.Show("MES出站上传参数表配置文件读取失败!\nPLCInteractiveAddressFileReadError!\nPLCInteractiveAddress beim Lesen der Sprachdatei!");
            }

            foreach (var item in ResourceHandler.listMesPullOutSide_plateParameters)
            {
                string[] type_Str = { item.Header, item.ParametersMESName, item.ParametersMESType, item.PLCAddres, item.ParametersPLCType, item.ParametersUpperLimit.ToString(), item.ParametersLowerLimit.ToString(), item.WhetherUploading.ToString(), item.WhetherInitial.ToString(), item.RepetitionVerify.ToString() };
                DataGridViewClass.AddRows(dataGridView1, type_Str, Color.White);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ResourceHandler.listMesPullOutSide_plateParameters.Clear();

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null || dataGridView1.Rows[i].Cells[0].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.Header = "";
                else
                    ResourceHandler.mesPullOutSide_plateParameters.Header = dataGridView1.Rows[i].Cells[0].Value.ToString();

                if (dataGridView1.Rows[i].Cells[1].Value == null ||dataGridView1.Rows[i].Cells[1].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersMESName = "";
                else
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersMESName = dataGridView1.Rows[i].Cells[1].Value.ToString();

                if (dataGridView1.Rows[i].Cells[2].Value == null || dataGridView1.Rows[i].Cells[2].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersMESType = "TEXT";
                else
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersMESType = dataGridView1.Rows[i].Cells[2].Value.ToString();

                if (dataGridView1.Rows[i].Cells[3].Value == null || dataGridView1.Rows[i].Cells[3].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.PLCAddres = "";
                else
                    ResourceHandler.mesPullOutSide_plateParameters.PLCAddres = dataGridView1.Rows[i].Cells[3].Value.ToString();
                
                if (dataGridView1.Rows[i].Cells[4].Value == null || dataGridView1.Rows[i].Cells[4].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersPLCType = "STRING";
                else
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersPLCType = dataGridView1.Rows[i].Cells[4].Value.ToString();

                try
                {
                    if (dataGridView1.Rows[i].Cells[5].Value == null || dataGridView1.Rows[i].Cells[5].Value.ToString().Equals("") || dataGridView1.Rows[i].Cells[5].Value.ToString().Equals("-"))
                        ResourceHandler.mesPullOutSide_plateParameters.ParametersUpperLimit = "-";
                    else
                        ResourceHandler.mesPullOutSide_plateParameters.ParametersUpperLimit = Convert.ToDouble(dataGridView1.Rows[i].Cells[5].Value).ToString();
                }
                catch (Exception)
                {
                    dataGridView1.Rows[i].Cells[5].Value = "-";
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersUpperLimit = "-";
                    MessageBox.Show("参数上限输入错误！");
                }

                try
                {
                    if (dataGridView1.Rows[i].Cells[6].Value == null || dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("") || dataGridView1.Rows[i].Cells[6].Value.ToString().Equals("-"))
                        ResourceHandler.mesPullOutSide_plateParameters.ParametersLowerLimit = "-";
                    else
                        ResourceHandler.mesPullOutSide_plateParameters.ParametersLowerLimit = Convert.ToDouble(dataGridView1.Rows[i].Cells[6].Value).ToString();
                }
                catch (Exception)
                {
                    dataGridView1.Rows[i].Cells[6].Value = "-";
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersLowerLimit = "-";
                    MessageBox.Show("参数下限输入错误！");
                }

                try
                {
                    if (Convert.ToDouble(ResourceHandler.mesPullOutSide_plateParameters.ParametersUpperLimit)  < Convert.ToDouble(ResourceHandler.mesPullOutSide_plateParameters.ParametersLowerLimit))
                    {
                        dataGridView1.Rows[i].Cells[6].Value = "0";
                        ResourceHandler.mesPullOutSide_plateParameters.ParametersLowerLimit = "0";
                        MessageBox.Show("参数下限不能大于参数上限");
                    }
                }
                catch (Exception)
                {
                    dataGridView1.Rows[i].Cells[5].Value = "-";
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersUpperLimit = "-";
                    dataGridView1.Rows[i].Cells[6].Value = "-";
                    ResourceHandler.mesPullOutSide_plateParameters.ParametersLowerLimit = "-";
                }
                

                if (dataGridView1.Rows[i].Cells[7].Value == null || dataGridView1.Rows[i].Cells[7].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.WhetherUploading = false;
                else
                    ResourceHandler.mesPullOutSide_plateParameters.WhetherUploading = Convert.ToBoolean(dataGridView1.Rows[i].Cells[7].Value);

                if (dataGridView1.Rows[i].Cells[8].Value == null || dataGridView1.Rows[i].Cells[8].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.WhetherInitial = false;
                else
                    ResourceHandler.mesPullOutSide_plateParameters.WhetherInitial = Convert.ToBoolean(dataGridView1.Rows[i].Cells[8].Value);

                if (dataGridView1.Rows[i].Cells[9].Value == null || dataGridView1.Rows[i].Cells[9].Value.ToString().Equals(""))
                    ResourceHandler.mesPullOutSide_plateParameters.RepetitionVerify = false;
                else
                    ResourceHandler.mesPullOutSide_plateParameters.RepetitionVerify = Convert.ToBoolean(dataGridView1.Rows[i].Cells[9].Value);

                ResourceHandler.listMesPullOutSide_plateParameters.Add(ResourceHandler.mesPullOutSide_plateParameters);
            }

            XmlHelper xmlHelper = new XmlHelper($"xml/{main.workstationName.Text}/{ResourceHandler.listSystemParameters[0].FormulaName}/MesPullOutSide_plate.xml");
            bool headerBool = xmlHelper.Write(ResourceHandler.listMesPullOutSide_plateParameters);
            if (!headerBool)
            {
                MessageBox.Show("MES侧板上传参数配置文件保存失败!\nPLCInteractiveAddressFileReadError!\nPLCInteractiveAddress beim Lesen der Sprachdatei!");
            }
        }

        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView DataGridView = (DataGridView)sender;
            string updateData = DataGridView.CurrentCell.EditedFormattedValue.ToString();
            string preData = DataGridView.CurrentCell.FormattedValue.ToString();

            string name = DataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();

            main.outDiary($"登陆用户：{main.user_Name.Text}  参数表头：{name}   原始{DataGridView.Columns[e.ColumnIndex].HeaderCell.FormattedValue}：{preData}  修改{DataGridView.Columns[e.ColumnIndex].HeaderCell.FormattedValue}：{updateData}", "信息");
        }
    }
}
