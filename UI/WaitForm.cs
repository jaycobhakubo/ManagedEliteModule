// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// A form that displays progress for a long running proccess.
    /// </summary>
    public partial class WaitForm : EliteGradientForm
    {
        #region Constants and Data Types
        protected readonly Size WinFullSize = new Size(320, 314);
        protected readonly Point WinFullTextLoc = new Point(37, 170);
        protected readonly Point WinFullButtonLoc = new Point(95, 248);

        protected readonly Size WinNoProgSize = new Size(320, 291);
        protected readonly Point WinNoProgTextLoc = new Point(37, 144);
        protected readonly Point WinNoProgButtonLoc = new Point(95, 225);

        protected readonly Size WinNoButtonSize = new Size(320, 277);
        protected readonly Point WinNoButtonTextLoc = new Point(37, 170);

        protected readonly Size WinNoProgAndButtonSize = new Size(320, 251);
        protected readonly Point WinProgAndButtonTextLoc = new Point(37, 144);

        protected readonly Point TouchTextLoc = new Point(37, 146);
        protected readonly Color TouchTextBack = Color.FromArgb(21, 70, 112);
        protected readonly Font TouchTextFont = new Font("Tahoma", 9F, FontStyle.Bold);
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the user clicks the Cancel button on the WaitForm.
        /// </summary>
        [Description("Occurs when the user clicks the Cancel button on the WaitForm.")]
        [Category("Action")]
        public event EventHandler CancelButtonClick;

        /// <summary>
        /// Occurs when the progress bar changes on the WaitForm.
        /// </summary>
        [Description("Occurs when the progress bar changes on the WaitForm.")]
        [Category("Action")]
        public event EventHandler ProgressBarChanged;
        #endregion

        #region Member Variables
        protected bool m_isTouchScreen;
        protected bool m_showProgressBar = true;
        protected bool m_showCancelButton = true;
        protected bool m_cancelClosesForm;
        protected bool m_allowClose;
        protected bool m_waitingToShow = false;
        private DateTime m_shownAt = DateTime.Now;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WaitForm class in window mode.
        /// </summary>
        public WaitForm()
            : base(new NormalDisplayMode())
        {
            InitializeComponent();
            RearrangeForm();
        }

        /// <summary>
        /// Initializes a new instance of the WaitForm class in touchscreen 
        /// mode.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        public WaitForm(DisplayMode displayMode)
            : base(displayMode)
        {
            m_isTouchScreen = true;
            InitializeComponent();
            ApplyDisplayMode();
            RearrangeForm();
        }

        #endregion

        #region Member Methods Private
        /// <summary>
        /// Will place the form's controls based on what is visible.
        /// </summary>
        protected void RearrangeForm()
        {
            if(!m_isTouchScreen)
            {
                // Which controls are showing?
                if(m_showProgressBar && m_showCancelButton)
                {
                    Size = WinFullSize;
                    m_messageLabel.Location = WinFullTextLoc;
                    m_cancelButton.Location = WinFullButtonLoc;
                }
                else if(!m_showProgressBar && m_showCancelButton)
                {
                    Size = WinNoProgSize;
                    m_messageLabel.Location = WinNoProgTextLoc;
                    m_cancelButton.Location = WinNoProgButtonLoc;
                }
                else if(m_showProgressBar && !m_showCancelButton)
                {
                    Size = WinNoButtonSize;
                    m_messageLabel.Location = WinNoButtonTextLoc;
                }
                else // No progress or button.
                {
                    Size = WinNoProgAndButtonSize;
                    m_messageLabel.Location = WinProgAndButtonTextLoc;
                }
            }
            else
            {
                Size = m_displayMode.DialogSize;
                DrawAsGradient = false;
                DrawRounded = true;
                DrawBorderOuterEdge = true;
                DrawBorderInnerEdge = true;
                DrawAColorBorder = true;
                BackgroundImage = Resources.WaitBack;
                StartPosition = FormStartPosition.CenterParent;

                m_showProgressBar = false;
                m_progressBar.Visible = false;
                m_showCancelButton = false;
                m_cancelButton.Visible = false;

                m_messageLabel.Location = TouchTextLoc;
                m_messageLabel.BackColor = TouchTextBack;
                m_messageLabel.ForeColor = Color.Yellow;
                m_messageLabel.Font = TouchTextFont;
                m_messageLabel.BorderStyle = BorderStyle.None;
            }
        }

        /// <summary>
        /// Call this method to close the form programmatically so it won't be 
        /// prevented by the FormClosing event.
        /// </summary>
        public void CloseForm()
        {
            m_waitingToShow = false;

            if (this.Visible)
            {
                m_allowClose = true;
                Close();
                Application.DoEvents();
            }
        }

        private void WaitForm_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            m_shownAt = DateTime.Now;
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

            // Reset the allow close.
            if(!e.Cancel)
                m_allowClose = false;

            if (!e.Cancel && this.Visible)
            {
                //if the dialog was displayed, make sure it is visible for at least 1/2 second
                while (((TimeSpan)(DateTime.Now - m_shownAt)).Milliseconds < 500)
                    System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Handles the cancel button's click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs object that contains 
        /// the event data.</param>
        private void CancelClick(object sender, EventArgs e)
        {
            // Close the form if specified.
            if(m_cancelClosesForm)
            {
                DialogResult = DialogResult.Cancel;
                m_allowClose = true;
                Close();
            }

            // Signal the event.
            EventHandler handler = CancelButtonClick;

            if(handler != null)
                handler(this, new EventArgs());
        }
        #endregion

        #region Member Methods Public

        public bool WaitToShow()
        {
            DateTime startedWaitingAt = DateTime.Now;

            m_waitingToShow = true;

            while (m_waitingToShow && ((TimeSpan)(DateTime.Now - startedWaitingAt)).Milliseconds < 1000)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }

            bool okToShow = m_waitingToShow;

            m_waitingToShow = false;

            return okToShow;
        }

        /// <summary>
        /// Will visually update the form to signify progress.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The ProgressChangedEventArgs object that 
        /// contains the event data.</param>
        public void ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            if(m_showProgressBar)
                m_progressBar.Value = e.ProgressPercentage;

            if (ProgressBarChanged != null) 
                ProgressBarChanged(this, new EventArgs());

            string newMessage = e.UserState as string;

            if(e.UserState != null)
                Message = newMessage;
        }

        /// <summary>
        /// Disposes of the wait form's wait image.
        /// </summary>
        private void RemoveWaitImage()
        {
            if(m_waitPicture.Image != null)
            {
                m_waitPicture.Image.Dispose();
                m_waitPicture.Image = null;
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the image to display while waiting.
        /// </summary>
        public Image WaitImage
        {
            get
            {
                return m_waitPicture.Image;
            }
            set
            {
                // TTP 50135
                // POS performance problems.

                // Clean up the old picture.
                RemoveWaitImage();

                if(value != null)
                    m_waitPicture.Image = (Image)value.Clone();
            }
        }

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
            }
        }

        /// <summary>
        /// Gets or sets whether to show the progress bar.  The progress bar is 
        /// only allowed in non-touchscreen mode.
        /// </summary>
        public bool ProgressBarVisible
        {
            get
            {
                return m_showProgressBar;
            }
            set
            {
                if(!m_isTouchScreen)
                {
                    m_showProgressBar = value;
                    m_progressBar.Visible = value;
                }
                else
                {
                    m_showProgressBar = false;
                    m_progressBar.Visible = false;
                }

                RearrangeForm();
            }
        }

        /// <summary>
        /// Gets the form's ProgressBar
        /// </summary>
        public ProgressBar WaitProgress
        {
            get
            {
                return m_progressBar;
            }
        }

        /// <summary>
        /// Gets or sets whether to show the cancel button. The cancel button 
        /// is only allowed in non-touchscreen mode.
        /// </summary>
        public bool CancelButtonVisible
        {
            get
            {
                return m_showCancelButton;
            }
            set
            {
                if(!m_isTouchScreen)
                {
                    m_showCancelButton = value;
                    m_cancelButton.Visible = value;
                }
                else
                {
                    m_showCancelButton = false;
                    m_cancelButton.Visible = false;
                }

                RearrangeForm();
            }
        }

        /// <summary>
        /// Gets or sets whether the cancel button closes the form when 
        /// clicked.
        /// </summary>
        public bool CancelButtonClosesForm
        {
            get
            {
                return m_cancelClosesForm;
            }
            set
            {
                m_cancelClosesForm = value;
            }
        }
        #endregion
    }
}