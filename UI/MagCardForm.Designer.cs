namespace GTI.Modules.Shared
{
    partial class MagCardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MagCardForm));
            this.m_clearCardButton = new GTI.Controls.ImageButton();
            this.m_cancelButton = new GTI.Controls.ImageButton();
            this.m_messageLabel = new System.Windows.Forms.Label();
            this.lblAnalyze = new System.Windows.Forms.Label();
            this.lblPleaseWait = new System.Windows.Forms.Label();
            this.m_btnNoThanks = new GTI.Controls.ImageButton();
            this.m_lblPatronMessage = new System.Windows.Forms.Label();
            this.m_timeoutProgress = new System.Windows.Forms.ProgressBar();
            this.m_kioskTimer = new System.Windows.Forms.Timer(this.components);
            this.m_txtbxCardNumber = new GTI.Controls.TextBoxNumeric();
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
            this.m_messageLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.m_messageLabel, "m_messageLabel");
            this.m_messageLabel.Name = "m_messageLabel";
            // 
            // lblAnalyze
            // 
            resources.ApplyResources(this.lblAnalyze, "lblAnalyze");
            this.lblAnalyze.BackColor = System.Drawing.Color.Transparent;
            this.lblAnalyze.Name = "lblAnalyze";
            // 
            // lblPleaseWait
            // 
            this.lblPleaseWait.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblPleaseWait, "lblPleaseWait");
            this.lblPleaseWait.Name = "lblPleaseWait";
            // 
            // m_btnNoThanks
            // 
            this.m_btnNoThanks.BackColor = System.Drawing.Color.Transparent;
            this.m_btnNoThanks.FocusColor = System.Drawing.Color.Black;
            this.m_btnNoThanks.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.RedButtonUp;
            this.m_btnNoThanks.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.RedButtonDown;
            resources.ApplyResources(this.m_btnNoThanks, "m_btnNoThanks");
            this.m_btnNoThanks.Name = "m_btnNoThanks";
            this.m_btnNoThanks.SecondaryTextPadding = new System.Windows.Forms.Padding(5);
            this.m_btnNoThanks.ShowFocus = false;
            this.m_btnNoThanks.UseVisualStyleBackColor = false;
            this.m_btnNoThanks.Click += new System.EventHandler(this.CancelClick);
            // 
            // m_lblPatronMessage
            // 
            this.m_lblPatronMessage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.m_lblPatronMessage, "m_lblPatronMessage");
            this.m_lblPatronMessage.ForeColor = System.Drawing.Color.Blue;
            this.m_lblPatronMessage.Name = "m_lblPatronMessage";
            this.m_lblPatronMessage.Click += new System.EventHandler(this.SomethingWasClicked);
            // 
            // m_timeoutProgress
            // 
            this.m_timeoutProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(186)))), ((int)(((byte)(192)))));
            this.m_timeoutProgress.ForeColor = System.Drawing.Color.Gold;
            resources.ApplyResources(this.m_timeoutProgress, "m_timeoutProgress");
            this.m_timeoutProgress.Name = "m_timeoutProgress";
            this.m_timeoutProgress.Click += new System.EventHandler(this.SomethingWasClicked);
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
            // MagCardForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::GTI.Modules.Shared.Properties.Resources.MagCardBack;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.m_txtbxCardNumber);
            this.Controls.Add(this.m_timeoutProgress);
            this.Controls.Add(this.m_lblPatronMessage);
            this.Controls.Add(this.m_btnNoThanks);
            this.Controls.Add(this.lblAnalyze);
            this.Controls.Add(this.m_messageLabel);
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_clearCardButton);
            this.Controls.Add(this.lblPleaseWait);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MagCardForm";
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
        private System.Windows.Forms.Label lblAnalyze;
        private System.Windows.Forms.Label lblPleaseWait;
        private Controls.ImageButton m_btnNoThanks;
        private System.Windows.Forms.Label m_lblPatronMessage;
        private System.Windows.Forms.ProgressBar m_timeoutProgress;
        private System.Windows.Forms.Timer m_kioskTimer;
        private Controls.TextBoxNumeric m_txtbxCardNumber;
    }
}