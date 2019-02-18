namespace GTI.Modules.Shared
{
	partial class TextEntryForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEntryForm));
			this.lblDescription = new System.Windows.Forms.Label();
			this.btnClose = new GTI.Controls.ImageButton();
			this.btnOK = new GTI.Controls.ImageButton();
			this.txtResult = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// lblDescription
			// 
			this.lblDescription.AutoSize = true;
			this.lblDescription.BackColor = System.Drawing.Color.Transparent;
			this.lblDescription.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDescription.Location = new System.Drawing.Point(44, 12);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(233, 22);
			this.lblDescription.TabIndex = 9;
			this.lblDescription.Text = "Set Description property here";
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.Color.Transparent;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Font = new System.Drawing.Font("Trebuchet MS", 10F, System.Drawing.FontStyle.Bold);
			this.btnClose.ForeColor = System.Drawing.Color.Black;
			this.btnClose.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageNormal")));
			this.btnClose.ImagePressed = ((System.Drawing.Image)(resources.GetObject("btnClose.ImagePressed")));
			this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnClose.Location = new System.Drawing.Point(238, 87);
			this.btnClose.MinimumSize = new System.Drawing.Size(30, 30);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(101, 30);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "&Cancel";
			this.btnClose.UseVisualStyleBackColor = false;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.Transparent;
			this.btnOK.Font = new System.Drawing.Font("Trebuchet MS", 10F, System.Drawing.FontStyle.Bold);
			this.btnOK.ForeColor = System.Drawing.Color.Black;
			this.btnOK.ImageNormal = ((System.Drawing.Image)(resources.GetObject("btnOK.ImageNormal")));
			this.btnOK.ImagePressed = ((System.Drawing.Image)(resources.GetObject("btnOK.ImagePressed")));
			this.btnOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnOK.Location = new System.Drawing.Point(99, 87);
			this.btnOK.MinimumSize = new System.Drawing.Size(30, 30);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(101, 30);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// txtResult
			// 
			this.txtResult.Location = new System.Drawing.Point(48, 42);
			this.txtResult.MaxLength = 64;
			this.txtResult.Name = "txtResult";
			this.txtResult.Size = new System.Drawing.Size(340, 26);
			this.txtResult.TabIndex = 0;
			this.txtResult.Text = "TextResult property";
			// 
			// TextEntryForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(437, 130);
			this.ControlBox = false;
			this.Controls.Add(this.txtResult);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnClose);
			this.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "TextEntryForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Text property here";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private GTI.Controls.ImageButton btnClose;
		private GTI.Controls.ImageButton btnOK;
		private System.Windows.Forms.TextBox txtResult;
		private System.Windows.Forms.Label lblDescription;
	}
}