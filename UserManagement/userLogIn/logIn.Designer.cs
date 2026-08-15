namespace UserManagement.userLogIn
{
    partial class logIn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(logIn));
            this.user_LogIn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.user_Account = new System.Windows.Forms.TextBox();
            this.user_Password = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // user_LogIn
            // 
            resources.ApplyResources(this.user_LogIn, "user_LogIn");
            this.user_LogIn.Name = "user_LogIn";
            this.user_LogIn.UseVisualStyleBackColor = true;
            this.user_LogIn.Click += new System.EventHandler(this.user_LogIn_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // user_Account
            // 
            resources.ApplyResources(this.user_Account, "user_Account");
            this.user_Account.Name = "user_Account";
            // 
            // user_Password
            // 
            resources.ApplyResources(this.user_Password, "user_Password");
            this.user_Password.Name = "user_Password";
            // 
            // logIn
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.user_Password);
            this.Controls.Add(this.user_Account);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.user_LogIn);
            this.Name = "logIn";
            this.Load += new System.EventHandler(this.logIn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button user_LogIn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox user_Account;
        private System.Windows.Forms.TextBox user_Password;
    }
}