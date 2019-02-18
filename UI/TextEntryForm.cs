using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using GTI.Modules.Shared.Properties;



namespace GTI.Modules.Shared
{
	public partial class TextEntryForm : GradientForm
	{
		bool m_bIsNumeric = false;
		bool m_bIsKiosk = false;
		int m_nOriginalHeight;

		public TextEntryForm()
		{
			InitializeComponent();
			this.TextResult = "";
			m_nOriginalHeight = this.Height;
		}



		public bool IsKiosk
		{
			set 
			{ 
				m_bIsKiosk = value;
				btnClose.Height = m_bIsKiosk ? 50 : 30;
				btnOK.Height = m_bIsKiosk ? 50 : 30;
				this.Height = m_bIsKiosk ? m_nOriginalHeight + 20 : m_nOriginalHeight;

				// Set fonts
				Font font1 = m_bIsKiosk ? new Font("Tahoma", 12F, FontStyle.Bold) : new Font("Trebuchet MS", 12F, FontStyle.Bold);
				this.Font = font1;
				txtResult.Font = font1;
				lblDescription.Font = font1;
				// Button fonts
				Font font2 = m_bIsKiosk ? new Font("Tahoma", 10F, FontStyle.Bold) : new Font("Trebuchet MS", 10F, FontStyle.Bold);
				btnOK.Font = font2;
				btnClose.Font = font2;
				if (m_bIsKiosk)
				{
					btnOK.ImageNormal = Resources.BigBlueButtonUp;
					btnOK.ImagePressed = Resources.BigBlueButtonDown;
					btnClose.ImageNormal = Resources.BigBlueButtonUp;
					btnClose.ImagePressed = Resources.BigBlueButtonDown;
				}
			}
		}

		public string TextResult
		{
			get { return txtResult.Text; }
			set { txtResult.Text = value; }
		}

		public string Description
		{
			set { lblDescription.Text = value; }
		}

		// Set to true if you want numeric input
		public bool IsNumeric
		{
			set { m_bIsNumeric = value; }
		}



		private void btnOK_Click(object sender, EventArgs e)
		{
			// Make sure they entered something
			if (txtResult.Text.Length == 0)
			{
				MessageForm.Show(this, Resources.NoTextEntered);
				txtResult.Select();
				return;
			}

			// If this is a numeric box, validate
			if ( m_bIsNumeric && ! Information.IsNumeric(txtResult.Text) )
			{
				MessageForm.Show(this, Resources.EnterNumber);
				txtResult.Text = "";
				txtResult.Select();
				return;
			}

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}