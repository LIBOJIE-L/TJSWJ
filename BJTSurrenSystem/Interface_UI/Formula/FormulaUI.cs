using BJTSurrenSystem.Interface_UI.Formula;
using BJTSurrenSystem.Parameters;
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

namespace BJTSurrenSystem.Interface_UI
{
    public partial class FormulaUI : Form
    {
        public Main main;
        public string LoadN;

        List<string> columnHeader_Project = new List<string>() { "工程列表", "创建时间" };

        public FormulaUI(Main main)
        {
            InitializeComponent();
            this.main = main;
        }

        public void FormulaUI_Load(object sender, EventArgs e)
        {
            ResourceHandler.dparamParameters.dataTable = new DataTable ();
            ResourceHandler.dparamParameters.mFormulaFilePath = Application.StartupPath + "\\xml\\" + main.workstationName.Text;

            foreach (string s in columnHeader_Project)//添加序号名称
            {
                ResourceHandler.dparamParameters.dataTable.Columns.Add(s, Type.GetType("System.String"));
            }
            dataGridView4.DataSource = ResourceHandler.dparamParameters.dataTable;

            UpdataProjectList_Click(null, null);
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

        public void UpdataProjectList_Click(object sender, EventArgs e)
        {
            while (ResourceHandler.dparamParameters.dataTable.Rows.Count > 0)
            {
                ResourceHandler.dparamParameters.dataTable.Rows.RemoveAt(0);
            }
            if (Directory.Exists(ResourceHandler.dparamParameters.mFormulaFilePath))
            {
                DirectoryInfo fileName = new DirectoryInfo(ResourceHandler.dparamParameters.mFormulaFilePath);
                
                foreach (FileSystemInfo NextFile in fileName.GetDirectories()/* fileName.GetFileSystemInfos()*/)
                {
                    DataRow dr = ResourceHandler.dparamParameters.dataTable.NewRow();
                    dr[0] = NextFile.Name;
                    dr[1] = NextFile.CreationTime;
                    ResourceHandler.dparamParameters.dataTable.Rows.Add(dr);
                    dataGridView4.DataSource = ResourceHandler.dparamParameters.dataTable;
                }
            }
        }

        private void CreateProject_Click(object sender, EventArgs e)
        {
            FormulaAddUI formulaAddUI = new FormulaAddUI();
            formulaAddUI.ShowDialog();
            string CreateName = formulaAddUI.CreateName;
            if (CreateName == "")
            {
                return;
            }
            string CreateFilePath = ResourceHandler.dparamParameters.mFormulaFilePath + "/" + CreateName;
            if (!Directory.Exists(CreateFilePath))
            {
                Directory.CreateDirectory(CreateFilePath);
                DirectoryInfo TheFiles = new DirectoryInfo(Application.StartupPath + "\\BackUp");
                // DirectoryInfo TheFilesContent = new DirectoryInfo(TheFiles.GetFileSystemInfos()[0].FullName);
                foreach (FileInfo NextFile in TheFiles.GetFiles())
                {
                    File.Copy(NextFile.FullName, CreateFilePath + "\\" + NextFile.Name, true);
                }
                MessageBox.Show("创建成功！");
            }
            else
            {
                MessageBox.Show("文件已经存在！");
            }
            UpdataProjectList_Click(null, null);
        }

        private void DeteleProject_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView4.SelectedRows.Count != 0)
                {
                    DialogResult dlgResult = MessageBox.Show(this, "确定要删除工程：" + dataGridView4.SelectedRows[dataGridView4.SelectedRows.Count - 1].Cells[0].Value.ToString(), "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dlgResult == DialogResult.OK)
                    {
                        string DeleteName = dataGridView4.SelectedRows[dataGridView4.SelectedRows.Count - 1].Cells[0].Value.ToString();
                        Directory.Delete(ResourceHandler.dparamParameters.mFormulaFilePath + "/" + DeleteName, true);
                        ResourceHandler.dparamParameters.dataTable.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
                        dataGridView4.DataSource = ResourceHandler.dparamParameters.dataTable;
                    }
                }
                UpdataProjectList_Click(null, null);
            }
            catch
            {


            }
        }

        private void NewName_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView4.SelectedRows.Count != 0)
                {
                    FormulaAmend formulaAmend = new FormulaAmend();
                    formulaAmend.ShowDialog();
                    string New_Name = formulaAmend.New_Name;
                    if (New_Name == "")
                    {
                        return;
                    }
                    string CreateFilePath = ResourceHandler.dparamParameters.mFormulaFilePath + "/" + New_Name;
                    string SelcetName = dataGridView4.SelectedRows[dataGridView4.SelectedRows.Count - 1].Cells[0].Value.ToString();
                    string SourcePath = ResourceHandler.dparamParameters.mFormulaFilePath + "/" + SelcetName;
                    if (Directory.Exists(SourcePath) && !Directory.Exists(CreateFilePath))
                    {
                        Directory.Move(SourcePath, CreateFilePath);
                        if (dataGridView4.SelectedRows[dataGridView4.SelectedRows.Count - 1].Cells[0].Value.ToString() == ProjectName.Text)
                        {
                            //所选命名的工程是当前的工程的话要保存进ini
                            ResourceHandler.listSystemParameters[0].FormulaName = New_Name.ToString();
                            ProjectName.Text = New_Name;

                            main.保存ToolStripMenuItem_Click();

                            main.MesPullOutUploadingParameter();
                            main.Ini_Data();
                        }
                    }
                    else
                    {
                        MessageBox.Show("命名有误！");
                    }

                }
                UpdataProjectList_Click(null, null);
            }
            catch
            {


            }
        }

        private void LoadProject_Click(object sender, EventArgs e)
        {
            MessageBox.Show("加载其他产品型号前，请确认此型号参数已经保存！");
            if (dataGridView4.SelectedRows[dataGridView4.SelectedRows.Count - 1].Cells[0].Value.ToString() == ProjectName.Text)
            {
                MessageBox.Show("当前工程即为所选工程");
                return;
            }
            if (dataGridView4.SelectedRows.Count != 0)
            {
                DialogResult dlgResult = MessageBox.Show(this, "确定要加载产品型号：" + dataGridView4.SelectedRows[dataGridView4.SelectedRows.Count - 1].Cells[0].Value.ToString(), "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dlgResult == DialogResult.OK)
                {
                    LoadN = dataGridView4.SelectedRows[dataGridView4.SelectedRows.Count - 1].Cells[0].Value.ToString();

                    ResourceHandler.listSystemParameters[0].FormulaName = LoadN.ToString();

                    main.保存ToolStripMenuItem_Click();

                    main.MesPullOutUploadingParameter();
                    main.PLCInteractiveAddressParameter();
                    main.Ini_Data();

                    ProjectName.Text = LoadN.ToString();

                }
            }
        }

        private void ProjectPath_Click(object sender, EventArgs e)
        {
            try
            { System.Diagnostics.Process.Start("explorer.exe", ResourceHandler.dparamParameters.mFormulaFilePath); }
            catch
            { }
        }
    }
}
