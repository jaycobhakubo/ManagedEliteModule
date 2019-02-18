// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Windows.Forms;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The common SplashScreen that should be used for all Windows Elite 
    /// modules.
    /// </summary>
    public partial class SplashScreen : Form
    {
        #region Member Methods
        protected bool m_allowClose;
        #endregion

        #region Constructors
        /// <summary>
        /// Initalizes a new instance of the SplashScreen class.
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();
        }
        #endregion

        #region Member Methods
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
        /// Gets or sets the name of the application to display.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return m_applicationName.Text;
            }
            set
            {
                m_applicationName.Text = value;
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Gets or sets the version to display.
        /// </summary>
        public string Version
        {
            get
            {
                return m_version.Text;
            }
            set
            {
                m_version.Text = string.Format(Resources.Version, value); 
                Application.DoEvents(); 
            }
        }

        /// <summary>
        /// Gets or sets the text to display in the status box.
        /// </summary>
        public string Status
        {
            get
            {
                return m_status.Text;
            }
            set
            {
                m_status.Text = value;
                Application.DoEvents();
            }
        }
        #endregion
    }
    
}