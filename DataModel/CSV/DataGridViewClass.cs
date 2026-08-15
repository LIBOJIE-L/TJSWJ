using CsvHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

// 这里写的都是一些针对dataGridView控件的工具类

namespace DataModel
{
    /// <summary> 这里写的都是一些针对dataGridView控件的公用函数
    /// </summary>
    public static class DataGridViewClass
    {
        #region 增操作

        /// <summary> 增加列
        /// 表对象 - 内容
        /// </summary>
        public static void AddColumns(DataGridView dataGridView, string content)
        {
            DataGridViewTextBoxColumn acCode = new DataGridViewTextBoxColumn();
            acCode.Name = content;
            acCode.DataPropertyName = content;
            acCode.HeaderText = content;
            dataGridView.Columns.Add(acCode);
        }
        /// <summary> 增加列
        /// 表对象 - 内容(数组)
        /// </summary>
        public static void AddColumns(DataGridView dataGridView, string[] content)
        {
            for (int i = 0; i < content.Length; i++)
            {
                DataGridViewTextBoxColumn acCode = new DataGridViewTextBoxColumn();
                acCode.Name = content[i];
                acCode.DataPropertyName = content[i];
                acCode.HeaderText = content[i];
                dataGridView.Columns.Add(acCode);
            }
        }

        /// <summary> 添加行
        /// 表对象 - 内容(数组)
        /// </summary>
        public static void AddRows(DataGridView dataGridView, string[] content, Color BackColor, DataGridView TargetDataGridView = null)
        {
            // 添加一空行
            try {
                dataGridView.Rows.Add("");
            }
            catch (System.InvalidOperationException) {// 当使用的目标表的列为空的时候就会触发这里的异常，作用是将第三个参数的Data表的表头添加进来
                if (TargetDataGridView != null)
                {
                    for (int i = 0; i < TargetDataGridView.Columns.Count; i++)
                    {
                        AddColumns(dataGridView, TargetDataGridView.Columns[i].HeaderText);
                    }

                    dataGridView.Rows.Add("");
                }
            }

            try {
                // 寻找无数据的行
                for (int k = 0; k < dataGridView.Rows.Count; k++)
                {
                    // 判断当前行是否为空可以拿来使用
                    if (dataGridView.Rows[k].Cells[0].Value == null || dataGridView.Rows[k].Cells[0].Value == "")
                    {
                        // 将传来的数组数据导入到表里面
                        for (int i = 0; i < dataGridView.Columns.Count; i++)
                        {
                            dataGridView.Rows[k].Cells[i].Value = content[i];
                        }
                        dataGridView.Rows[k].DefaultCellStyle.BackColor = BackColor;
                        return;// 数据全部写完了后就可以不需要进行最外层的循环，直接跳出即可
                    }
                }
            }
            catch (System.IndexOutOfRangeException) { }
            
            dataGridView.CurrentCell = dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[0];
        }
        #endregion

        #region 删操作
        /// <summary> 移除所有列
        /// 需要传入表对象
        /// </summary>
        public static void RemoveAllColumns(DataGridView dataGridView) {
            while (dataGridView.Columns.Count > 0)
            {
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    dataGridView.Columns.RemoveAt(i);
                }
            }
        }

        /// <summary> 移除所有行
        /// 需要传入表对象
        /// </summary>
        public static void RemoveAllRow(DataGridView dataGridView)
        {
            try {
                while (dataGridView.Rows.Count > 0)
                {
                    for (int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        Thread.Sleep(10);
                        DataGridViewRow row2 = dataGridView.Rows[i];
                        dataGridView.Rows.Remove(row2);// 移除选中的行
                    }
                }
            } catch (System.InvalidOperationException ex) { MessageBox.Show("操作出了问题，请重试！"); }
            
        }

        /// <summary> 移除指定的烈的表头
        /// 需要传入表对象和指定列里的内容
        /// </summary>
        public static void RemoveAllRow(DataGridView dataGridView, String TitleName)
        {
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                if (dataGridView.Columns[i].HeaderText == TitleName) {
                    dataGridView.Columns.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary> 根据传进来的索引位置进行删除指定的行
        /// </summary>
        public static void RemoveIndexRow(DataGridView dataGirdView, int index) {
            dataGirdView.Rows.RemoveAt(index);
        }
        #endregion

        #region 查操作
        /// <summary> 获取到数据表里面某一行里面的数据内容
        /// </summary>
        /// <returns>string[] 字符串数组</returns>
        public static string[] GetRowsData(DataGridView dataGridView,int index)
        {
            string[] temp = new string[dataGridView.Columns.Count];// 创建一个字符串素组

            try {
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    temp[i] = (dataGridView.Rows[index].Cells[i].Value == null ? "" : dataGridView.Rows[index].Cells[i].Value.ToString());
                }
                return temp;
            }
            catch (System.ArgumentOutOfRangeException)
            {// 索引超出范围
                MessageBox.Show("当前搜索的行在列表中并不存在！");
            }
            
            return new string[]{"",""};
        }
        public static string[] GetRowsData(DataGridView dataGridView)
        {
            string[] temp = new string[dataGridView.Columns.Count];// 创建一个字符串素组

            try
            {
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    temp[i] = (dataGridView.Rows[0].Cells[i].Value == null ? "" : dataGridView.Rows[0].Cells[i].Value.ToString());
                }
                return temp;
            }
            catch (System.ArgumentOutOfRangeException)
            {// 索引超出范围
                MessageBox.Show("当前搜索的行在列表中并不存在！");
            }

            return new string[] { "", "" };
        }
        /// <summary> 获取某行的索引位置
        /// 数据表对象 - 查询的数据内容
        /// </summary>
        /// <returns>返回int值</returns>
        public static int GetRowsIndex(DataGridView dataGridView, string content)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    if (dataGridView.Rows[i].Cells[k].Value != null && dataGridView.Rows[i].Cells[k].Value.ToString() == content)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary> 获取指定行的数据
        /// 数据表对象 - 查询的数据内容
        /// </summary>
        /// <returns>返回string值</returns>
        public static int GetRowsText(DataGridView dataGridView, string content)
        {
            //content是传入的模组码
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    if (dataGridView.Rows[i].Cells[k].Value != null && dataGridView.Rows[i].Cells[k].Value.ToString() == content)
                    {
                        return i;
                        //return dataGridView.Rows[i].Cells[k].Value.ToString();
                    }
                }
            }
            return -1;
        }
        /// <summary> 获取某列的索引位置
        /// </summary>
        /// <returns>返回int值</returns>
        public static int GetColumnsIndex(DataGridView dataGridView, string content)
        {
            
                for (int k = 0; k < dataGridView.Columns.Count; k++)// 循环列
                {
                    if (dataGridView.Columns[k].HeaderText == content)
                    {
                        return k;// 返回列索引
                    }
                }
            
            return 0;
        }
        /// <summary> 获取某行的索引位置
        /// </summary>
        /// <returns>返回int值</returns>
        public static int GetColumnsIndex2(DataGridView dataGridView)
        {
           
            return dataGridView.RowCount;
        }
        /// <summary> 获取指定的表的所有表标题
        /// </summary>
        /// <param name="dataGridView">指定的表对象</param>
        /// <returns>字符串数组</returns>
        public static string[] GetTitle(DataGridView dataGridView) {
            string[] tempStr = new string[dataGridView.Columns.Count];
            for (int i = 0; i < dataGridView.Columns.Count; i++ )
            {
                tempStr[i] = dataGridView.Columns[i].HeaderText;
            }
            return tempStr;
        }
        #endregion

        #region 改操作
        /// <summary> 修改指定的索引行内的内容
        /// </summary>
        /// <param name="dataGridView">数据表对象</param>
        /// <param name="contont">修改的内容</param>
        /// <param name="index">行索引</param>
        public static void RevampRows(DataGridView dataGridView, string[] contont, int index) {
            for (int i = 0; i < dataGridView.Rows.Count; i++) {
                dataGridView.Rows[index].Cells[i].Value = contont[i];
            }
        }
        #endregion

        /// <summary> 禁止点击了表标题后对内容进行排序
        /// </summary>
        /// <param name="dataGridView">表对象</param>
        public static void BanSort(DataGridView dataGridView)
        {
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary> 将表头平均占据整个表的宽度
        /// </summary>
        /// <param name="dataGridView"></param>
        public static void SetTitleWidth(DataGridView dataGridView)
        {
            if (dataGridView.Columns.Count <= 1) { return; }
            int width = dataGridView.Size.Width / dataGridView.Columns.Count;

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].Width = width;
            }
        }

        /// <summary> 开启表控件的双缓冲器
        /// </summary>
        /// <param name="dataGridView">表对象</param>
        public static void DataGridCreateParams(DataGridView dataGridView)
        {
            Type dgvType = dataGridView.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView, true, null);
        }

        /// <summary> 读取CSV表里面全部的内容
        /// </summary>
        public static List<string> Read_CSV(string FliePath)
        {
            string str2 = FliePath;
            //if (!Directory.Exists(str2))// 如果CSV文件不存在了，就需要提示
            if (!File.Exists(str2))
            {
                return null;
            }
            StreamReader reader = new StreamReader(str2, System.Text.Encoding.Default);
            string line = "";
            List<string> listStrArr = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                listStrArr.Add(line.Replace("\t", ""));//将文件内容分割成数组
            }
            reader.Close();
            return listStrArr;
        }

        /// <summary> 获取某列的索引位置
        /// </summary>
        /// <returns>返回int值</returns>
        public static int GetColumnsIndex(List<string> list, string content)
        {
            for (int k = 1; k < list.Count; k++)// 循环列
            {
                if (list[k].Contains(content))
                {
                    return k;// 返回列索引
                }
            }

            return 0;
        }

        public static string FindIniPath(DataGridView dataGridView,string name)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].Cells[0].Value != null && dataGridView.Rows[i].Cells[0].Value.ToString().Equals(name))
                {
                    return dataGridView.Rows[i].Cells[1].Value.ToString();  
                }
            }
            return "";
        }

        public static string MesFindDataGridViewRead(DataGridView dataGridView, string name) {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows[i].Cells[0].Value != null && dataGridView.Rows[i].Cells[0].Value.ToString().Equals(name))
                {
                    return dataGridView.Rows[i].Cells[2].Value.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// 数据写入CSV
        /// </summary>
        /// <param name="content">写入CSV数组</param>
        /// <param name="filesPath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        public static void Write_MESLOG_CSV(object[] content, string filesPath, string fileName)
        {
            try
            {
                string str2 = filesPath + "\\"+fileName + ".CSV";
                if (!File.Exists(filesPath))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Directory.CreateDirectory(filesPath);
                }
                StreamWriter writer = new StreamWriter(new FileStream(str2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex) { MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！"); }
            catch (System.IO.IOException ex) { MessageBox.Show("MES数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly); }
            catch (Exception ex) { }
        }

        /// <summary> 向指定的CSV文件路径写入数据
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <param name="content">要写入的内容，数组</param>
        public static void Write_CSV(object[] content, string filePath, DataGridView dataGridView, string fileName = "")// CSV表写入操作
        {
            StreamWriter writer = null;
            try
            {
                string filePathCSV = filePath + (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";

                if (System.IO.Directory.Exists(filePath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(filePath);
                }

                if (!File.Exists(filePathCSV))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(filePathCSV, DataGridViewClass.GetTitle(dataGridView));
                }
                writer = new StreamWriter(new FileStream(filePathCSV, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);
                // 向文件里面写入数据
                try
                {
                    writer.WriteLine(string.Join("\t,", content));
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
                finally { writer.Close(); }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            /*try
            {
                if (System.IO.Directory.Exists(filePath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(filePath);
                }
                string filePathCSV = filePath + (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";
                if (!File.Exists(filePathCSV))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(filePathCSV, DataGridViewClass.GetTitle(dataGridView));
                }
                using (var csvWriter = new StreamWriter(new FileStream(filePathCSV, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default))
                using (var csv = new CsvWriter(csvWriter, CultureInfo.InvariantCulture))
                {
                    // 向文件里面写入数据
                    try
                    {
                        csv.WriteRecords(content);
                    }
                    catch (WriterException exception)
                    {
                        throw new Exception("导出错误" + exception.Message);
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }*/
        }

        private static void Write_CSV_Title(string fileName, object[] content) // 写入表头数据
        {
            StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);
            // 向文件里面写入数据
            try
            {
                writer.WriteLine(string.Join(",", content));
                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("导出错误" + exception.Message);
            }
            finally { writer.Close(); }
        }


    }
}
