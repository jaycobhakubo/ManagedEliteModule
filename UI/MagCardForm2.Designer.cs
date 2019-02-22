namespace GTI.Modules.Shared
{
    partial class MagCardForm2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MagCardForm2));
            this.m_clearCardButton = new GTI.Controls.ImageButton();
            this.m_cancelButton = new GTI.Controls.ImageButton();
            this.m_messageLabel = new System.Windows.Forms.Label();
            this.m_kioskTimer = new System.Windows.Forms.Timer(this.components);
            this.m_txtbxCardNumber = new GTI.Controls.TextBoxNumeric();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // m_clearCardButton
            // 
            this.m_clearCardButton.BackColor = System.Drawing.Color.Transparent;
            this.m_clearCardButton.FocusColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.m_clearCardButton, "m_clearCardButton");
            this.m_clearCardButton.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.BigBlueButtonUp;
            this.m_clearCardButton.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.BigBlueButtonDown;
            this.m_clearCardButton.Name = "m_clearCardButton";
            this.m_clearCardButton.SecondaryTextPadding = new System.Windows.Forms.Padding(5);
            this.m_clearCardButton.ShowFocus = false;
            this.m_clearCardButton.TabStop = false;
            this.m_clearCardButton.UseVisualStyleBackColor = false;
            this.m_clearCardButton.Click += new System.EventHandler(this.ClearCardClick);
            // 
            // m_cancelButton
            // 
            this.m_cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.m_cancelButton.FocusColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.m_cancelButton, "m_cancelButton");
            this.m_cancelButton.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.BigBlueButtonUp;
            this.m_cancelButton.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.BigBlueButtonDown;
            this.m_cancelButton.Name = "m_cancelButton";
            this.m_cancelButton.SecondaryTextPadding = new System.Windows.Forms.Padding(5);
            this.m_cancelButton.ShowFocus = false;
            this.m_cancelButton.TabStop = false;
            this.m_cancelButton.UseVisualStyleBackColor = false;
            this.m_cancelButton.Click += new System.EventHandler(this.CancelClick);
            // 
            // m_messageLabel
            // 
            this.m_messageLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_messageLabel, "m_messageLabel");
            this.m_messageLabel.Name = "m_messageLabel";
            // 
            // m_kioskTimer
            // 
            this.m_kioskTimer.Enabled = true;
            this.m_kioskTimer.Interval = 500;
            this.m_kioskTimer.Tick += new System.EventHandler(this.m_kioskTimer_Tick);
            // 
            // m_txtbxCardNumber
            // 
            resources.ApplyResources(this.m_txtbxCardNumber, "m_txtbxCardNumber");
            this.m_txtbxCardNumber.Mask = GTI.Controls.TextBoxNumeric.TextBoxType.Integer;
            this.m_txtbxCardNumber.Name = "m_txtbxCardNumber";
            this.m_txtbxCardNumber.Precision = 2;
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Checked = true;
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.TabStop = true;
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            resources.ApplyResources(this.radioButton2, "radioButton2");
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // MagCardForm2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::GTI.Modules.Shared.Properties.Resources.MagCardBack;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.m_txtbxCardNumber);
            this.Controls.Add(this.m_messageLabel);
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_clearCardButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MagCardForm2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            this.Shown += new System.EventHandler(this.MagCardForm_Shown);
            this.Click += new System.EventHandler(this.SomethingWasClicked);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MagCardForm_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GTI.Controls.ImageButton m_clearCardButton;
        private GTI.Controls.ImageButton m_cancelButton;
        private System.Windows.Forms.Label m_messageLabel;
        private System.Windows.Forms.Timer m_kioskTimer;
        private Controls.TextBoxNumeric m_txtbxCardNumber;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}