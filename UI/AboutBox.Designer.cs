namespace GTI.Modules.Shared
{
    public partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.m_description = new System.Windows.Forms.TextBox();
            this.m_copyright2 = new System.Windows.Forms.Label();
            this.m_copyright1 = new System.Windows.Forms.Label();
            this.m_version = new System.Windows.Forms.Label();
            this.m_productName = new System.Windows.Forms.Label();
            this.m_tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.m_okButton = new GTI.Controls.ImageButton();
            this.m_pictureBox = new System.Windows.Forms.PictureBox();
            this.m_tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // m_description
            // 
            this.m_description.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.m_description, "m_description");
            this.m_description.Name = "m_description";
            this.m_description.ReadOnly = true;
            this.m_description.TabStop = false;
            // 
            // m_copyright2
            // 
            resources.ApplyResources(this.m_copyright2, "m_copyright2");
            this.m_copyright2.MaximumSize = new System.Drawing.Size(0, 29);
            this.m_copyright2.Name = "m_copyright2";
            // 
            // m_copyright1
            // 
            resources.ApplyResources(this.m_copyright1, "m_copyright1");
            this.m_copyright1.MaximumSize = new System.Drawing.Size(0, 29);
            this.m_copyright1.Name = "m_copyright1";
            // 
            // m_version
            // 
            resources.ApplyResources(this.m_version, "m_version");
            this.m_version.MaximumSize = new System.Drawing.Size(0, 29);
            this.m_version.Name = "m_version";
            // 
            // m_productName
            // 
            resources.ApplyResources(this.m_productName, "m_productName");
            this.m_productName.MaximumSize = new System.Drawing.Size(0, 29);
            this.m_productName.Name = "m_productName";
            // 
            // m_tableLayoutPanel
            // 
            this.m_tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_tableLayoutPanel, "m_tableLayoutPanel");
            this.m_tableLayoutPanel.Controls.Add(this.m_productName, 1, 0);
            this.m_tableLayoutPanel.Controls.Add(this.m_version, 1, 1);
            this.m_tableLayoutPanel.Controls.Add(this.m_copyright1, 1, 2);
            this.m_tableLayoutPanel.Controls.Add(this.m_copyright2, 1, 3);
            this.m_tableLayoutPanel.Controls.Add(this.m_description, 1, 4);
            this.m_tableLayoutPanel.Controls.Add(this.m_okButton, 1, 5);
            this.m_tableLayoutPanel.Controls.Add(this.m_pictureBox, 0, 0);
            this.m_tableLayoutPanel.Name = "m_tableLayoutPanel";
            // 
            // m_okButton
            // 
            resources.ApplyResources(this.m_okButton, "m_okButton");
            this.m_okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_okButton.FocusColor = System.Drawing.Color.Black;
            this.m_okButton.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.BlueButtonUp;
            this.m_okButton.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.BlueButtonDown;
            this.m_okButton.MinimumSize = new System.Drawing.Size(30, 30);
            this.m_okButton.Name = "m_okButton";
            // 
            // m_pictureBox
            // 
            resources.ApplyResources(this.m_pictureBox, "m_pictureBox");
            this.m_pictureBox.Image = global::GTI.Modules.Shared.Properties.Resources.AboutBack;
            this.m_pictureBox.Name = "m_pictureBox";
            this.m_tableLayoutPanel.SetRowSpan(this.m_pictureBox, 6);
            this.m_pictureBox.TabStop = false;
            // 
            // AboutBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.m_tableLayoutPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.m_tableLayoutPanel.ResumeLayout(false);
            this.m_tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox m_description;
        private System.Windows.Forms.Label m_copyright2;
        private System.Windows.Forms.Label m_copyright1;
        private System.Windows.Forms.Label m_version;
        private System.Windows.Forms.Label m_productName;
        private System.Windows.Forms.TableLayoutPanel m_tableLayoutPanel;
        private GTI.Controls.ImageButton m_okButton;
        private System.Windows.Forms.PictureBox m_pictureBox;

    }
}
