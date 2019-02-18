namespace GTI.Modules.Shared
{
    partial class LoadingForm
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            this.m_messageLabel = new System.Windows.Forms.Label();
            this.m_versionLabel = new System.Windows.Forms.Label();
            this.m_productNameLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_messageLabel
            // 
            this.m_messageLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_messageLabel, "m_messageLabel");
            this.m_messageLabel.ForeColor = System.Drawing.Color.White;
            this.m_messageLabel.Name = "m_messageLabel";
            // 
            // m_versionLabel
            // 
            this.m_versionLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_versionLabel, "m_versionLabel");
            this.m_versionLabel.ForeColor = System.Drawing.Color.Yellow;
            this.m_versionLabel.Name = "m_versionLabel";
            // 
            // m_productNameLabel
            // 
            this.m_productNameLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_productNameLabel, "m_productNameLabel");
            this.m_productNameLabel.ForeColor = System.Drawing.Color.Yellow;
            this.m_productNameLabel.Name = "m_productNameLabel";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::GTI.Modules.Shared.Properties.Resources.LoadingScreen1024;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.m_productNameLabel);
            this.panel1.Controls.Add(this.m_versionLabel);
            this.panel1.Controls.Add(this.m_messageLabel);
            this.panel1.Name = "panel1";
            // 
            // LoadingForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadingForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            this.Shown += new System.EventHandler(this.LoadingForm_Shown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_messageLabel;
        private System.Windows.Forms.Label m_versionLabel;
        private System.Windows.Forms.Label m_productNameLabel;
        private System.Windows.Forms.Panel panel1;
    }
}