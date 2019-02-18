namespace GTI.Modules.Shared
{
    partial class WaitForm
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

            WaitImage = null; // TTP 50135

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WaitForm));
            this.m_progressBar = new System.Windows.Forms.ProgressBar();
            this.m_cancelButton = new GTI.Controls.ImageButton();
            this.m_waitPicture = new System.Windows.Forms.PictureBox();
            this.m_messageLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_waitPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // m_progressBar
            // 
            this.m_progressBar.ForeColor = System.Drawing.Color.ForestGreen;
            resources.ApplyResources(this.m_progressBar, "m_progressBar");
            this.m_progressBar.Name = "m_progressBar";
            // 
            // m_cancelButton
            // 
            this.m_cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.m_cancelButton.FocusColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.m_cancelButton, "m_cancelButton");
            this.m_cancelButton.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.BlueButtonUp;
            this.m_cancelButton.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.BlueButtonDown;
            this.m_cancelButton.Name = "m_cancelButton";
            this.m_cancelButton.RepeatRate = 150;
            this.m_cancelButton.RepeatWhenHeldFor = 750;
            this.m_cancelButton.SecondaryTextPadding = new System.Windows.Forms.Padding(5);
            this.m_cancelButton.UseVisualStyleBackColor = false;
            this.m_cancelButton.Click += new System.EventHandler(this.CancelClick);
            // 
            // m_waitPicture
            // 
            this.m_waitPicture.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_waitPicture, "m_waitPicture");
            this.m_waitPicture.Name = "m_waitPicture";
            this.m_waitPicture.TabStop = false;
            // 
            // m_messageLabel
            // 
            this.m_messageLabel.BackColor = System.Drawing.Color.Transparent;
            this.m_messageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.m_messageLabel, "m_messageLabel");
            this.m_messageLabel.Name = "m_messageLabel";
            // 
            // WaitForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.m_progressBar);
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_waitPicture);
            this.Controls.Add(this.m_messageLabel);
            this.DoubleBuffered = true;
            this.DrawAsGradient = true;
            this.DrawBorderOuterEdge = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.InnerBorderEdgeColor = System.Drawing.Color.Black;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitForm";
            this.OuterBorderEdgeColor = System.Drawing.Color.Black;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            this.Shown += new System.EventHandler(this.WaitForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.m_waitPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label m_messageLabel;
        private System.Windows.Forms.PictureBox m_waitPicture;
        private GTI.Controls.ImageButton m_cancelButton;
        private System.Windows.Forms.ProgressBar m_progressBar;
    }
}