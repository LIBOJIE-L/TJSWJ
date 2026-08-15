namespace HJMSurrenSystem.Interface_UI
{
    partial class FormulaUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormulaUI));
            this.CreateProject = new System.Windows.Forms.Button();
            this.ProjectPath = new System.Windows.Forms.Button();
            this.NewName = new System.Windows.Forms.Button();
            this.DeteleProject = new System.Windows.Forms.Button();
            this.LoadProject = new System.Windows.Forms.Button();
            this.ProjectName = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label69 = new System.Windows.Forms.Label();
            this.UpdataProjectList = new System.Windows.Forms.Button();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.SuspendLayout();
            // 
            // CreateProject
            // 
            this.CreateProject.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.CreateProject, "CreateProject");
            this.CreateProject.Name = "CreateProject";
            this.CreateProject.UseVisualStyleBackColor = false;
            this.CreateProject.Click += new System.EventHandler(this.CreateProject_Click);
            // 
            // ProjectPath
            // 
            this.ProjectPath.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.ProjectPath, "ProjectPath");
            this.ProjectPath.Name = "ProjectPath";
            this.ProjectPath.UseVisualStyleBackColor = false;
            this.ProjectPath.Click += new System.EventHandler(this.ProjectPath_Click);
            // 
            // NewName
            // 
            this.NewName.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.NewName, "NewName");
            this.NewName.Name = "NewName";
            this.NewName.UseVisualStyleBackColor = false;
            this.NewName.Click += new System.EventHandler(this.NewName_Click);
            // 
            // DeteleProject
            // 
            this.DeteleProject.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.DeteleProject, "DeteleProject");
            this.DeteleProject.Name = "DeteleProject";
            this.DeteleProject.UseVisualStyleBackColor = false;
            this.DeteleProject.Click += new System.EventHandler(this.DeteleProject_Click);
            // 
            // LoadProject
            // 
            this.LoadProject.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.LoadProject, "LoadProject");
            this.LoadProject.Name = "LoadProject";
            this.LoadProject.UseVisualStyleBackColor = false;
            this.LoadProject.Click += new System.EventHandler(this.LoadProject_Click);
            // 
            // ProjectName
            // 
            resources.ApplyResources(this.ProjectName, "ProjectName");
            this.ProjectName.Name = "ProjectName";
            // 
            // label61
            // 
            resources.ApplyResources(this.label61, "label61");
            this.label61.Name = "label61";
            // 
            // label69
            // 
            resources.ApplyResources(this.label69, "label69");
            this.label69.Name = "label69";
            // 
            // UpdataProjectList
            // 
            this.UpdataProjectList.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.UpdataProjectList, "UpdataProjectList");
            this.UpdataProjectList.Name = "UpdataProjectList";
            this.UpdataProjectList.UseVisualStyleBackColor = false;
            this.UpdataProjectList.Click += new System.EventHandler(this.UpdataProjectList_Click);
            // 
            // dataGridView4
            // 
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.AllowUserToResizeColumns = false;
            this.dataGridView4.AllowUserToResizeRows = false;
            this.dataGridView4.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView4.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dataGridView4, "dataGridView4");
            this.dataGridView4.MultiSelect = false;
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.ReadOnly = true;
            this.dataGridView4.RowHeadersVisible = false;
            this.dataGridView4.RowTemplate.Height = 23;
            this.dataGridView4.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // FormulaUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CreateProject);
            this.Controls.Add(this.ProjectPath);
            this.Controls.Add(this.NewName);
            this.Controls.Add(this.ProjectName);
            this.Controls.Add(this.DeteleProject);
            this.Controls.Add(this.label61);
            this.Controls.Add(this.LoadProject);
            this.Controls.Add(this.label69);
            this.Controls.Add(this.UpdataProjectList);
            this.Controls.Add(this.dataGridView4);
            this.Name = "FormulaUI";
            this.Load += new System.EventHandler(this.FormulaUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CreateProject;
        private System.Windows.Forms.Button ProjectPath;
        private System.Windows.Forms.Button NewName;
        private System.Windows.Forms.Button DeteleProject;
        private System.Windows.Forms.Button LoadProject;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.Button UpdataProjectList;
        private System.Windows.Forms.DataGridView dataGridView4;
        public System.Windows.Forms.Label ProjectName;
    }
}