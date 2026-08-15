namespace HJMSurrenSystem.Siemens
{
    partial class set_PLC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(set_PLC));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.PLC_IP = new System.Windows.Forms.TextBox();
            this.PLC_model = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // PLC_IP
            // 
            resources.ApplyResources(this.PLC_IP, "PLC_IP");
            this.PLC_IP.Name = "PLC_IP";
            // 
            // PLC_model
            // 
            resources.ApplyResources(this.PLC_model, "PLC_model");
            this.PLC_model.FormattingEnabled = true;
            this.PLC_model.Items.AddRange(new object[] {
            resources.GetString("PLC_model.Items"),
            resources.GetString("PLC_model.Items1"),
            resources.GetString("PLC_model.Items2"),
            resources.GetString("PLC_model.Items3"),
            resources.GetString("PLC_model.Items4"),
            resources.GetString("PLC_model.Items5")});
            this.PLC_model.Name = "PLC_model";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // set_PLC
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PLC_model);
            this.Controls.Add(this.PLC_IP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "set_PLC";
            this.Load += new System.EventHandler(this.set_PLC_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox PLC_IP;
        public System.Windows.Forms.ComboBox PLC_model;
    }
}