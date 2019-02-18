// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Drawing;
using System.Windows.Forms;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The form that displays while a touchscreen based module is loading.
    /// </summary>
    public partial class LoadingForm : EliteForm
    {
        #region Member Methods
        protected bool m_allowClose;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the LoadingForm class.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        public LoadingForm(DisplayMode displayMode)
            : base(displayMode)
        {
            InitializeComponent();
            ApplyDisplayMode();
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Sets the settings of this form based on the current display mode.
        /// </summary>
        protected override void ApplyDisplayMode()
        {
            base.ApplyDisplayMode();

            panel1.Size = m_displayMode.BaseFormSize;
            panel1.Location = new Point(m_displayMode.OffsetForFullScreenX, m_displayMode.OffsetForFullScreenY);

            if(m_displayMode is CompactDisplayMode)
            {
                panel1.BackgroundImage = Resources.LoadingScreen800;

                // US1999
                // Adjust controls for small screen.
                m_messageLabel.Location = new Point(50, 70);

                m_productNameLabel.Location = new Point(50, 290);

                m_versionLabel.Size = new Size(776, 17);
                m_versionLabel.Location = new Point(12, 574);
            }

            if (m_displayMode is WideDisplayMode)
            {
                m_messageLabel.Location = new Point(m_messageLabel.Location.X + m_displayMode.EdgeAdjustmentForNormalToWideX, m_messageLabel.Location.Y);
                m_productNameLabel.Location = new Point(m_productNameLabel.Location.X + m_displayMode.EdgeAdjustmentForNormalToWideX, m_productNameLabel.Location.Y);
                m_versionLabel.Location = new Point(m_versionLabel.Location.X + m_displayMode.EdgeAdjustmentForNormalToWideX, m_versionLabel.Location.Y);
            }
        }

        /// <summary>
        /// Call this method to close the form programmatically so it won't be 
        /// prevented by the FormClosing event.
        /// </summary>
        public void CloseForm()
        {
            m_allowClose = true;
            Close();
        }

        /// <summary>
        /// Handles the FormClosing event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An FormClosingEventArgs object that 
        /// contains the event data.</param>
        private void FormClose(object sender, FormClosingEventArgs e)
        {
            // Don't allow the closing of this form.
            if(e.CloseReason == CloseReason.UserClosing)
                e.Cancel = !m_allowClose;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the text to display on the form.
        /// </summary>
        public string Message
        {
            get
            {
                return m_messageLabel.Text;
            }

            set
            {
                m_messageLabel.Text = value;
                this.BringToFront();
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Gets or sets the name of the module to display on the form.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return m_productNameLabel.Text;
            }
            set
            {
                m_productNameLabel.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the text to display for the version.
        /// </summary>
        public string Version
        {
            get
            {
                return m_versionLabel.Text;
            }
            set
            {
                m_versionLabel.Text = value;
            }
        }
        #endregion

        private void LoadingForm_Shown(object sender, EventArgs e)
        {
            BringToFront();
        }
    }
}