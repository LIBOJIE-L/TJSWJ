namespace BJTSurrenSystem.History
{
    partial class CpkSpecificationForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView specificationGrid;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button importMesButton;
        private System.Windows.Forms.Label hintLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.specificationGrid = new System.Windows.Forms.DataGridView();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.importMesButton = new System.Windows.Forms.Button();
            this.hintLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.specificationGrid)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // specificationGrid
            // 
            this.specificationGrid.AllowUserToAddRows = false;
            this.specificationGrid.AllowUserToDeleteRows = false;
            this.specificationGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.specificationGrid.BackgroundColor = System.Drawing.Color.White;
            this.specificationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.specificationGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specificationGrid.Location = new System.Drawing.Point(10, 42);
            this.specificationGrid.MultiSelect = false;
            this.specificationGrid.Name = "specificationGrid";
            this.specificationGrid.RowHeadersWidth = 36;
            this.specificationGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.specificationGrid.Size = new System.Drawing.Size(1060, 507);
            this.specificationGrid.TabIndex = 1;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.saveButton);
            this.buttonPanel.Controls.Add(this.cancelButton);
            this.buttonPanel.Controls.Add(this.deleteButton);
            this.buttonPanel.Controls.Add(this.addButton);
            this.buttonPanel.Controls.Add(this.importMesButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Location = new System.Drawing.Point(10, 549);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(4, 7, 4, 5);
            this.buttonPanel.Size = new System.Drawing.Size(1060, 52);
            this.buttonPanel.TabIndex = 2;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(948, 10);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(101, 32);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "保存并关闭";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(841, 10);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(101, 32);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(590, 10);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(101, 32);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "增加参数";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(734, 10);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(101, 32);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "删除参数";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // importMesButton
            // 
            this.importMesButton.Location = new System.Drawing.Point(453, 10);
            this.importMesButton.Name = "importMesButton";
            this.importMesButton.Size = new System.Drawing.Size(131, 32);
            this.importMesButton.TabIndex = 4;
            this.importMesButton.Text = "从MES配置导入";
            this.importMesButton.UseVisualStyleBackColor = true;
            this.importMesButton.Click += new System.EventHandler(this.ImportMesButton_Click);
            // 
            // hintLabel
            // 
            this.hintLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.hintLabel.Location = new System.Drawing.Point(10, 10);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Size = new System.Drawing.Size(1060, 32);
            this.hintLabel.TabIndex = 0;
            this.hintLabel.Text = "规格配置独立保存，不修改MES上传配置。至少填写一个规格限；双侧CPK需要同时填写LSL和USL。";
            this.hintLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CpkSpecificationForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1080, 611);
            this.Controls.Add(this.specificationGrid);
            this.Controls.Add(this.hintLabel);
            this.Controls.Add(this.buttonPanel);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "CpkSpecificationForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "涂胶CPK规格配置";
            ((System.ComponentModel.ISupportInitialize)(this.specificationGrid)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
