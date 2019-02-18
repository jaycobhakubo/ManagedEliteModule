// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The base class from which all managed Elite, touchscreen based forms 
    /// should derive.
    /// </summary>
    public partial class EliteForm : Form
    {
        #region Member Variables
        protected DisplayMode m_displayMode;
        #endregion

        #region Constructors
        /// <summary>
        /// Initalizes a new instance of the EliteForm class.
        /// Required method for Designer support.
        /// </summary>
        public EliteForm()
        {
        }

        /// <summary>
        /// Initalizes a new instance of the EliteForm class.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        public EliteForm(DisplayMode displayMode)
        {
            if(displayMode == null)
                throw new ArgumentNullException("displayMode");

            InitializeComponent();

            this.DoubleBuffered = true;
            m_displayMode = displayMode;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Sets the settings of this form based on the current display mode.
        /// </summary>
        protected virtual void ApplyDisplayMode()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ControlBox = false;
            MaximizeBox = false;

            if(m_displayMode != null)
                Size = m_displayMode.FormSize;
        }
        #endregion

        #region Properties

        public bool IsAtLeastWindows7
        {
            get
            {
                System.OperatingSystem OS = System.Environment.OSVersion;
                return (OS.Platform == System.PlatformID.Win32NT) && ((OS.Version.Major > 6) || (OS.Version.Major == 6 && OS.Version.Minor >= 1)); //Windows 7 is NT 6.1
            }
        }

        #endregion
    }
}