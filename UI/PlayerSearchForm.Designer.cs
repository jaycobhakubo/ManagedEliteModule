namespace GTI.Modules.Shared
{
    partial class PlayerSearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerSearchForm));
            this.m_searchCriteriaBox = new System.Windows.Forms.GroupBox();
            this.m_searchButton = new GTI.Controls.ImageButton();
            this.m_firstName = new System.Windows.Forms.TextBox();
            this.m_lastName = new System.Windows.Forms.TextBox();
            this.m_cardNumber = new System.Windows.Forms.TextBox();
            this.m_lastNameLabel = new System.Windows.Forms.Label();
            this.m_firstNameLabel = new System.Windows.Forms.Label();
            this.m_cardNumberLabel = new System.Windows.Forms.Label();
            this.m_nameSearchRadio = new System.Windows.Forms.RadioButton();
            this.m_cardSearchRadio = new System.Windows.Forms.RadioButton();
            this.m_searchResultsBox = new System.Windows.Forms.GroupBox();
            this.m_resultsList = new System.Windows.Forms.ListBox();
            this.m_okButton = new GTI.Controls.ImageButton();
            this.m_cancelButton = new GTI.Controls.ImageButton();
            this.m_errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.m_searchCriteriaBox.SuspendLayout();
            this.m_searchResultsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // m_searchCriteriaBox
            // 
            this.m_searchCriteriaBox.BackColor = System.Drawing.Color.Transparent;
            this.m_searchCriteriaBox.Controls.Add(this.m_searchButton);
            this.m_searchCriteriaBox.Controls.Add(this.m_firstName);
            this.m_searchCriteriaBox.Controls.Add(this.m_lastName);
            this.m_searchCriteriaBox.Controls.Add(this.m_cardNumber);
            this.m_searchCriteriaBox.Controls.Add(this.m_lastNameLabel);
            this.m_searchCriteriaBox.Controls.Add(this.m_firstNameLabel);
            this.m_searchCriteriaBox.Controls.Add(this.m_cardNumberLabel);
            this.m_searchCriteriaBox.Controls.Add(this.m_nameSearchRadio);
            this.m_searchCriteriaBox.Controls.Add(this.m_cardSearchRadio);
            resources.ApplyResources(this.m_searchCriteriaBox, "m_searchCriteriaBox");
            this.m_searchCriteriaBox.Name = "m_searchCriteriaBox";
            this.m_searchCriteriaBox.TabStop = false;
            // 
            // m_searchButton
            // 
            this.m_searchButton.FocusColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.m_searchButton, "m_searchButton");
            this.m_searchButton.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.BlueButtonUp;
            this.m_searchButton.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.BlueButtonDown;
            this.m_searchButton.Name = "m_searchButton";
            this.m_searchButton.SecondaryTextPadding = new System.Windows.Forms.Padding(5);
            this.m_searchButton.Click += new System.EventHandler(this.SearchClick);
            // 
            // m_firstName
            // 
            resources.ApplyResources(this.m_firstName, "m_firstName");
            this.m_firstName.Name = "m_firstName";
            this.m_firstName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EnterSearch);
            // 
            // m_lastName
            // 
            resources.ApplyResources(this.m_lastName, "m_lastName");
            this.m_lastName.Name = "m_lastName";
            this.m_lastName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EnterSearch);
            // 
            // m_cardNumber
            // 
            resources.ApplyResources(this.m_cardNumber, "m_cardNumber");
            this.m_cardNumber.Name = "m_cardNumber";
            this.m_cardNumber.Validating += new System.ComponentModel.CancelEventHandler(this.CardNumberValidate);
            // 
            // m_lastNameLabel
            // 
            resources.ApplyResources(this.m_lastNameLabel, "m_lastNameLabel");
            this.m_lastNameLabel.Name = "m_lastNameLabel";
            // 
            // m_firstNameLabel
            // 
            resources.ApplyResources(this.m_firstNameLabel, "m_firstNameLabel");
            this.m_firstNameLabel.Name = "m_firstNameLabel";
            // 
            // m_cardNumberLabel
            // 
            resources.ApplyResources(this.m_cardNumberLabel, "m_cardNumberLabel");
            this.m_cardNumberLabel.Name = "m_cardNumberLabel";
            // 
            // m_nameSearchRadio
            // 
            resources.ApplyResources(this.m_nameSearchRadio, "m_nameSearchRadio");
            this.m_nameSearchRadio.Name = "m_nameSearchRadio";
            this.m_nameSearchRadio.UseVisualStyleBackColor = true;
            // 
            // m_cardSearchRadio
            // 
            resources.ApplyResources(this.m_cardSearchRadio, "m_cardSearchRadio");
            this.m_cardSearchRadio.Checked = true;
            this.m_cardSearchRadio.Name = "m_cardSearchRadio";
            this.m_cardSearchRadio.TabStop = true;
            this.m_cardSearchRadio.UseVisualStyleBackColor = true;
            this.m_cardSearchRadio.CheckedChanged += new System.EventHandler(this.SearchModeChanged);
            // 
            // m_searchResultsBox
            // 
            this.m_searchResultsBox.BackColor = System.Drawing.Color.Transparent;
            this.m_searchResultsBox.Controls.Add(this.m_resultsList);
            resources.ApplyResources(this.m_searchResultsBox, "m_searchResultsBox");
            this.m_searchResultsBox.Name = "m_searchResultsBox";
            this.m_searchResultsBox.TabStop = false;
            // 
            // m_resultsList
            // 
            resources.ApplyResources(this.m_resultsList, "m_resultsList");
            this.m_resultsList.FormattingEnabled = true;
            this.m_resultsList.Name = "m_resultsList";
            this.m_resultsList.DoubleClick += new System.EventHandler(this.SelectPlayerClick);
            // 
            // m_okButton
            // 
            this.m_okButton.BackColor = System.Drawing.Color.Transparent;
            this.m_okButton.FocusColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.m_okButton, "m_okButton");
            this.m_okButton.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.BlueButtonUp;
            this.m_okButton.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.BlueButtonDown;
            this.m_okButton.Name = "m_okButton";
            this.m_okButton.SecondaryTextPadding = new System.Windows.Forms.Padding(5);
            this.m_okButton.UseVisualStyleBackColor = false;
            this.m_okButton.Click += new System.EventHandler(this.SelectPlayerClick);
            // 
            // m_cancelButton
            // 
            this.m_cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.m_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_cancelButton.FocusColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.m_cancelButton, "m_cancelButton");
            this.m_cancelButton.ImageNormal = global::GTI.Modules.Shared.Properties.Resources.BlueButtonUp;
            this.m_cancelButton.ImagePressed = global::GTI.Modules.Shared.Properties.Resources.BlueButtonDown;
            this.m_cancelButton.Name = "m_cancelButton";
            this.m_cancelButton.SecondaryTextPadding = new System.Windows.Forms.Padding(5);
            this.m_cancelButton.UseVisualStyleBackColor = false;
            // 
            // m_errorProvider
            // 
            this.m_errorProvider.ContainerControl = this;
            // 
            // PlayerSearchForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.m_cancelButton);
            this.Controls.Add(this.m_okButton);
            this.Controls.Add(this.m_searchResultsBox);
            this.Controls.Add(this.m_searchCriteriaBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlayerSearchForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClose);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressed);
            this.m_searchCriteriaBox.ResumeLayout(false);
            this.m_searchCriteriaBox.PerformLayout();
            this.m_searchResultsBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox m_searchCriteriaBox;
        private System.Windows.Forms.RadioButton m_nameSearchRadio;
        private System.Windows.Forms.RadioButton m_cardSearchRadio;
        private System.Windows.Forms.Label m_lastNameLabel;
        private System.Windows.Forms.Label m_firstNameLabel;
        private System.Windows.Forms.Label m_cardNumberLabel;
        private System.Windows.Forms.TextBox m_firstName;
        private System.Windows.Forms.TextBox m_lastName;
        private System.Windows.Forms.TextBox m_cardNumber;
        private System.Windows.Forms.GroupBox m_searchResultsBox;
        private GTI.Controls.ImageButton m_searchButton;
        private System.Windows.Forms.ListBox m_resultsList;
        private GTI.Controls.ImageButton m_okButton;
        private GTI.Controls.ImageButton m_cancelButton;
        private System.Windows.Forms.ErrorProvider m_errorProvider;
    }
}