#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2008 GameTech
// International, Inc.
#endregion

using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The form that reads magnetic card swipes from the keyboard.
    /// </summary>
    public partial class MagCardForm : EliteGradientForm
    {
        #region Constants and Data Types
        protected readonly Size WinSize = new Size(320, 240);
        protected readonly Point WinMessageLoc = new Point(8, 75);
        protected readonly Font WinMessageFont = new Font("Trebuchet MS", 12F, FontStyle.Bold);
        protected readonly Size WinButtonSize = new Size(130, 30);
        protected readonly Point WinButton1Loc = new Point(13, 159);
        protected readonly Point WinButton2Loc = new Point(172, 159);
        protected readonly Font WinButtonFont = new Font("Trebuchet MS", 10F, FontStyle.Bold);
        #endregion

        #region Member Variables
        protected bool m_isTouchScreen;
        protected MagneticCardReader m_magCardReader; // PDTS 1064
        protected bool m_detectedSwipe; // PDTS 1064
        protected bool m_readWholeCard;
        protected StringBuilder m_cardData = new StringBuilder(); // PDTS 1064
        protected System.Windows.Forms.Timer waitingForCardByte = null;
        protected bool m_AnalyzingCards = false;
        protected bool m_atLeastOneCardToAnalyze = false;
        protected bool m_cardReady = false;
        protected bool m_TestingCard = false;
        private bool m_EatCharacters = false;
        protected bool m_BadStartingCharacter = false;
        protected short m_MatchedFilter;
        protected bool m_PatronFacing = false;
        protected int m_PatronFacingIdleMax = 20000;
        Timer TestTimer = null;
        private DateTime m_idleSince = DateTime.Now;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MagCardForm class in window 
        /// mode.  The form will also return all key presses captured.
        /// </summary>
        public MagCardForm()
            : base(new NormalDisplayMode())
        {
            m_readWholeCard = true;
            InitializeComponent();
            m_cancelButton.Text = "Cancel";
            ApplyDisplayMode();
        }

        /// <summary>
        /// Initializes a new instance of the MagCardForm class in window 
        /// mode.
        /// </summary>
        /// <param name="magCardReader">The mag. card reader used to dectect 
        /// swipes.</param>
        /// <exception cref="System.ArgumentNullException">magCardReader is a 
        /// null reference.</exception>
        public MagCardForm(MagneticCardReader magCardReader)
            : base(new NormalDisplayMode())
        {
            // PDTS 1064
            if(magCardReader == null)
                throw new ArgumentNullException("magCardReader");

            // Start listening for swipes.
            m_magCardReader = magCardReader;
            m_magCardReader.CardSwiped += new MagneticCardSwipedHandler(CardSwiped);

            InitializeComponent();
            m_cancelButton.Text = "Cancel";
            ApplyDisplayMode();
        }

        /// <summary>
        /// Initializes a new instance of the MagCardForm class in touchscreen 
        /// mode.  The form will also return all key presses captured.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        public MagCardForm(DisplayMode displayMode)
            : base(displayMode)
        {
            m_isTouchScreen = true;
            m_readWholeCard = true;
            InitializeComponent();
            m_cancelButton.Text = "Cancel";
            ApplyDisplayMode();
        }

        /// <summary>
        /// Initializes a new instance of the MagCardForm class in touchscreen 
        /// mode.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="magCardReader">The mag. card reader used to dectect 
        /// swipes.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <exception cref="System.ArgumentNullException">magCardReader is a 
        /// null reference.</exception>
        public MagCardForm(DisplayMode displayMode, MagneticCardReader magCardReader)
            : base(displayMode)
        {
            // PDTS 1064
            if(magCardReader == null)
                throw new ArgumentNullException("magCardReader");

            m_isTouchScreen = true;

            // Start listening for swipes.
            m_magCardReader = magCardReader;
            m_magCardReader.CardSwiped += new MagneticCardSwipedHandler(CardSwiped);

            InitializeComponent();
            m_cancelButton.Text = "Cancel";
            ApplyDisplayMode();
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Sets the settings of this form based on the current display mode.
        /// </summary>
        protected override void ApplyDisplayMode()
        {
            if(m_isTouchScreen)
            {
                base.ApplyDisplayMode();
                StartPosition = FormStartPosition.CenterParent;

                // This is a dialog, so override the default size.
                Size = m_displayMode.DialogSize;

                BackgroundImage = null;
                DrawAsGradient = true;
                DrawRounded = true;
                
                // Remove the mnemonics.
                m_clearCardButton.Text = m_clearCardButton.Text.Replace("&", string.Empty);
                m_clearCardButton.UseMnemonic = false;

                m_cancelButton.Text = m_cancelButton.Text.Replace("&", string.Empty);
                m_cancelButton.UseMnemonic = false;
            }
            else // Window mode.
            {
                // Change the look and locations.
                FormBorderStyle = FormBorderStyle.FixedSingle;
                BackgroundImage = null;
                DrawAsGradient = true;
                Size = WinSize;

                m_messageLabel.Location = WinMessageLoc;           
                //m_messageLabel.Font = WinMessageFont;

                m_clearCardButton.Location = WinButton1Loc;             
                m_clearCardButton.Size = WinButtonSize;
                m_clearCardButton.Font = WinButtonFont;
                m_clearCardButton.ImageNormal = Resources.BlueButtonUp;
                m_clearCardButton.ImagePressed = Resources.BlueButtonDown;
                m_clearCardButton.ShowFocus = true;
                m_clearCardButton.TabStop = true;

                m_cancelButton.Location = WinButton2Loc;                   
                m_cancelButton.Size = WinButtonSize;
                m_cancelButton.Font = WinButtonFont;
                m_cancelButton.ImageNormal = Resources.BlueButtonUp;
                m_cancelButton.ImagePressed = Resources.BlueButtonDown;
                m_cancelButton.ShowFocus = true;
                m_cancelButton.TabStop = true;
            }
        }

        public void RedesignUI()
        {
            m_cancelButton.Location = new Point(172, 170);
            m_clearCardButton.Location = new Point(13, 170);
            m_messageLabel.Location = new Point(14, 16);
            m_messageLabel.Size = new System.Drawing.Size(286, 80);
            m_messageLabel.Text = "Please swipe or enter your player card.";
            m_txtbxCardNumber.Visible = true;
           

            //Use clear button as ok button
            m_clearCardButton.Text = "Ok";
            //removed the current event
            m_clearCardButton.Click -= new EventHandler(ClearCardClick);
            //add the new current event
            m_clearCardButton.Click += new EventHandler(FindPlayerOkClick);
            
            m_txtbxCardNumber.TabIndex = 0;
            m_txtbxCardNumber.Focus();
        }


        /// <summary>
        /// Processes a dialog box key.
        /// </summary>
        /// <param name="keyData">One of the Keys values that 
        /// represents the key to process.</param>
        /// <returns>true if the keystroke was processed and consumed by the 
        /// control; otherwise, false to allow further processing.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            NotIdle();

            return false;

            /*            if((keyData & Keys.Enter) == Keys.Enter)
                        {
                            // PDTS 1064
                            // If the key is a enter and we've read a mag. card,
                            // close the form and don't allow the control to process it.
                            if((m_readWholeCard && m_LastSwipeCharacter != '\x00') || (!m_readWholeCard && m_detectedSwipe))
                            {
                                m_detectedSwipe = false;
                                DialogResult = DialogResult.OK;
                                Close();
                                return true;
                            }
                            else
                                return base.ProcessDialogKey(keyData);
                        }
                        else 
                            return base.ProcessDialogKey(keyData);
            */             
        }
        	
        public void CenterToScreen()
        {
            Screen screen = Screen.FromControl(this);

            Rectangle workingArea = screen.WorkingArea;
          
            this.Location = new Point() 
                                {
                                    X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - this.Width) / 2),
                                    Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - this.Height) / 2)
                                };
        }

        private void MagCardForm_KeyDown(object sender, KeyEventArgs e)
        {
            NotIdle();

            e.SuppressKeyPress = false;

            if ((m_readWholeCard && m_cardData.Length > 0) || (!m_readWholeCard && (m_magCardReader.MSRInputInProgress || m_EatCharacters)))
                e.Handled = true;
        }

        /// <summary>
        /// Handles the form's KeyPress event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An KeyPressEventArgs object that contains the 
        /// event data.</param>
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            NotIdle();

            // PDTS 1064
            if(m_readWholeCard)
            {
                if (waitingForCardByte == null) //setup a timer to detect when the card is read
                {
                    waitingForCardByte = new Timer();
                    waitingForCardByte.Tick += new EventHandler(waitingForCardByte_Tick);
                    waitingForCardByte.Interval = 500;
                    waitingForCardByte.Start();

                    if(AnalyzingCards)
                    {
                        m_messageLabel.Visible = false;
                        lblAnalyze.Visible = false;
                        lblPleaseWait.Visible = true;
                    }
                }
                else
                {
                    waitingForCardByte.Stop();
                    waitingForCardByte.Interval = 500;
                    waitingForCardByte.Start();
                }

                m_cardData.Append(e.KeyChar);

                e.Handled = true; // Don't send to the active control.
            }
            else // Normal reading.
            {
                if (m_EatCharacters)
                {
                    TestTimer.Stop();
                    TestTimer.Interval = 500;
                    TestTimer.Start();

                    e.Handled = true;
                    return;
                }

                if (TestingCards && !m_magCardReader.MSRInputInProgress && !m_magCardReader.MSRStartCharacters.Contains(e.KeyChar.ToString()))
                {
                    if(e.KeyChar != '\x0D')
                        m_EatCharacters = true;

                    TestTimer = new Timer();
                    TestTimer.Interval = 500;
                    TestTimer.Tick += new EventHandler(TestTimer_Tick);
                    TestTimer.Start();
                }

                if(m_magCardReader.ProcessCharacter(e.KeyChar))
                    e.Handled = true; // Don't send to the active control.
            }
        }

        void TestTimer_Tick(object sender, EventArgs e)
        {
            TestTimer.Stop();
            TestTimer.Dispose();

            m_BadStartingCharacter = true;
            m_EatCharacters = false;
            m_detectedSwipe = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        void waitingForCardByte_Tick(object sender, EventArgs e)
        {
            waitingForCardByte.Stop();
            m_detectedSwipe = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the clear card button click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An EventArgs object that contains 
        /// the event data.</param>
        private void ClearCardClick(object sender, EventArgs e)
        {
            // PDTS 1064
            m_cardData.Length = 0;

            if (!m_readWholeCard)
                m_magCardReader.Reset();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void FindPlayerOkClick(object sender, EventArgs e)
        {
            m_cardData.Append(m_txtbxCardNumber.Text);
            // What is this code do? = m_MatchedFilter = e.MatchFilter;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the cancel button click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An EventArgs object that contains 
        /// the event data.</param>
        private void CancelClick(object sender, EventArgs e)
        {
            // PDTS 1064
            m_cardData.Length = 0;

            if (!m_readWholeCard)
                m_magCardReader.Reset();

            DialogResult = DialogResult.Cancel;
            Close();
        }

        // PDTS 1064
        /// <summary>
        /// Handles the MagneticCardReader's CardSwiped event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">A MagneticCardSwipeArgs object that contains the
        /// event data.</param>
        void CardSwiped(object sender, MagneticCardSwipeArgs e)
        {
            if(ContainsFocus)
            {
                // Store the card data.
                m_detectedSwipe = true;
                m_cardData.Length = 0;
                m_cardData.Append(e.CardData);
                m_MatchedFilter = e.MatchFilter;

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void MagCardForm_Shown(object sender, EventArgs e)
        {
            if (IsPatronFacing)
            {
                NotIdle();
                m_timeoutProgress.Maximum = m_PatronFacingIdleMax;
                m_kioskTimer.Start();
            }
        }

        private void NotIdle()
        {
            m_idleSince = DateTime.Now;
            m_timeoutProgress.Value = 0;
            m_timeoutProgress.Hide();
        }

        // PDTS 1064
        /// <summary>
        /// Handles the FormClosing event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An FormClosingEventArgs object that contains the 
        /// event data.</param>
        private void FormClose(object sender, FormClosingEventArgs e)
        {
            if (IsPatronFacing)
            {
                NotIdle();
                m_kioskTimer.Stop();
            }
                
            // Don't listen to the CardSwiped event anymore since we 
            // are closing.
            if(!m_readWholeCard && m_magCardReader != null)
                m_magCardReader.CardSwiped -= CardSwiped;

            if (waitingForCardByte != null)
            {
                waitingForCardByte.Stop();
                waitingForCardByte.Dispose();
                waitingForCardByte = null;
            }

            m_detectedSwipe = false;

            if (AnalyzingCards && DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;

                m_cardReady = true;

                m_messageLabel.Visible = false;
                lblAnalyze.Visible = false;
                lblPleaseWait.Visible = true;
            }
            else
            {
                AnalyzingCards = false;
            }
        }

        private void m_kioskTimer_Tick(object sender, EventArgs e)
        {
            if (!m_PatronFacing)
                return;

            TimeSpan idleFor = DateTime.Now - m_idleSince;

            if (idleFor > TimeSpan.FromMilliseconds(m_PatronFacingIdleMax / 3))
            {
                if (!m_timeoutProgress.Visible)
                    m_timeoutProgress.Show();

                m_timeoutProgress.Increment(m_kioskTimer.Interval);

                if (m_timeoutProgress.Value >= m_timeoutProgress.Maximum)
                    CancelClick(null, new EventArgs());
            }
        }

        
        private void SomethingWasClicked(object sender, EventArgs e)
        {
            NotIdle();
        }
        
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets whether the clear card button is shown.
        /// </summary>
        public bool ClearCardButtonVisible
        {
            get
            {
                return m_clearCardButton.Visible;
            }
            set
            {
                m_clearCardButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets the mag. card data read in.
        /// </summary>
        public string MagCardNumber
        {
            get
            {
                string cardData = m_cardData.ToString();

                m_cardReady = false;

                if (AnalyzingCards)
                {
                    m_cardData.Length = 0;
                    lblPleaseWait.Visible = false;
                    m_messageLabel.Visible = true;
                    lblAnalyze.Visible = true;
                }

                // PDTS 1064
                return cardData;
            }
        }

        /// <summary>
        /// Gets or sets if analyzing cards.
        /// </summary>
        public bool AnalyzingCards
        {
            get
            {
                return m_AnalyzingCards;
            }

            set
            {
                if (m_readWholeCard)
                {
                    m_AnalyzingCards = value;

                    lblAnalyze.Visible = m_AnalyzingCards;

                    if(m_AnalyzingCards)
                        m_cancelButton.Text = "Analyze";
                }
            }
        }

        /// <summary>
        /// Gets or sets if analyzing cards.
        /// </summary>
        public bool AtLeastOneCardToAnalyze
        {
            get
            {
                return m_atLeastOneCardToAnalyze;
            }

            set
            {
                m_atLeastOneCardToAnalyze = value;
                m_cancelButton.Visible = m_atLeastOneCardToAnalyze;
            }
        }

        /// <summary>
        /// Gets or sets if analyzing cards.
        /// </summary>
        public bool TestingCards
        {
            get
            {
                return m_TestingCard;
            }

            set
            {
                if (!m_readWholeCard)
                {
                    m_TestingCard = value;
                }
            }
        }


                /// <summary>
        /// Gets if the card swipe was being tested and had a bad starting character and resets the status
        /// </summary>
        public bool BadStartingCharacter
        {
            get
            {
                bool tmp = m_BadStartingCharacter;

                m_BadStartingCharacter = false;

                return tmp;
            }
        }

        /// <summary>
        /// Gets or sets if analyzing cards.
        /// </summary>
        public bool CardReady
        {
            get
            {
                return m_cardReady;
            }
        }

        public short MatchedFilter
        {
            get
            {
                return m_MatchedFilter;
            }
        }
        
        /// <summary>
        /// Gets or sets whether the dialog is facing the casino's customer
        /// like on a Kiosk.
        /// </summary>
        public bool IsPatronFacing
        {
            get
            {
                return m_PatronFacing;
            }

            set
            {
                m_PatronFacing = value;

                if (m_PatronFacing)
                {
                    m_cancelButton.Hide();
                    m_clearCardButton.Hide();
                    m_messageLabel.Hide();

                    m_btnNoThanks.Show();
                    m_lblPatronMessage.Show();
                }
                else
                {
                    m_cancelButton.Show();
                    m_clearCardButton.Show();
                    m_messageLabel.Show();

                    m_btnNoThanks.Hide();
                    m_lblPatronMessage.Hide();
                }
            }
        }

        public int PatronFacingCancelDelayInMilliseconds
        {
            get
            {
                return m_PatronFacingIdleMax;
            }

            set
            {
                m_PatronFacingIdleMax = value;
            }
        }

#endregion
    }
}