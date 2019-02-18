namespace GTI.Modules.Shared
{
    partial class SplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.m_applicationName = new System.Windows.Forms.Label();
            this.m_version = new System.Windows.Forms.Label();
            this.m_copyright1 = new System.Windows.Forms.Label();
            this.m_status = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_applicationName
            // 
            this.m_applicationName.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_applicationName, "m_applicationName");
            this.m_applicationName.ForeColor = System.Drawing.Color.White;
            this.m_applicationName.Name = "m_applicationName";
            // 
            // m_version
            // 
            this.m_version.BackColor = System.Drawing.Color.Transparent;
            this.m_version.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.m_version, "m_version");
            this.m_version.Name = "m_version";
            // 
            // m_copyright1
            // 
            this.m_copyright1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_copyright1, "m_copyright1");
            this.m_copyright1.ForeColor = System.Drawing.Color.Yellow;
            this.m_copyright1.Name = "m_copyright1";
            // 
            // m_status
            // 
            this.m_status.AutoEllipsis = true;
            this.m_status.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_status, "m_status");
            this.m_status.ForeColor = System.Drawing.Color.Yellow;
            this.m_status.Name = "m_status";
            // 
            // SplashScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::GTI.Modules.Shared.Properties.Resources.SplashscreenBG;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.m_status);
            this.Controls.Add(this.m_copyright1);
            this.Controls.Add(this.m_version);
            this.Controls.Add(this.m_applicationName);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_applicationName;
        private System.Windows.Forms.Label m_version;
        private System.Windows.Forms.Label m_copyright1;
        private System.Windows.Forms.Label m_status;
    }
}