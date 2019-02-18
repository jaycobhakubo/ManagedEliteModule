// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Drawing;
using System.Windows.Forms;
using GTI.Controls;
using GTI.Modules.Shared.Properties;
using System.Drawing.Drawing2D;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Enumerates the different types of message forms.
    /// </summary>
    public enum MessageFormTypes
    {
        OK,
        YesNo,
        YesCancel,
        YesNoCancel,
        YesNo_regular,
        Pause,
        YesCancelComp,
        YesNo_FlatDesign,
        OK_FlatDesign,
        RetryAbort,
        RetryAbortIgnore,
        RetryAbortForce,

        YesNo_DefNO,
        YesCancel_DefCancel,
        YesNoCancel_DefNo,
        YesNoCancel_DefCancel,
        YesNo_regular_DefNo,
        YesCancelComp_DefCancel,
        YesNo_FlatDesign_DefNo,
        RetryAbort_DefAbort,
        RetryAbortIgnore_DefAbort,
        RetryAbortIgnore_DefIgnore,
        RetryAbortForce_DefAbort,
        RetryAbortForce_DefForce,
        ContinueCancel,
        ContinueCancel_DefCancel,
        Custom1Button,
        Custom2Button,
        Custom3Button,

        ColorButtons = 1000
    }
    /// <summary>
    /// A form that displays messages.
    /// </summary>
    public partial class MessageForm : EliteGradientForm
    {
        #region Constants and Data Types
        protected readonly Size WinSize = new Size(320, 240);
        protected readonly Size WinYesNoCancelSize = new Size(365, 240);
        protected readonly Font WinFont = new Font("Trebuchet MS", 12F, FontStyle.Bold);
        protected readonly Font WinButtonFont = new Font("Trebuchet MS", 10F, FontStyle.Bold);
        protected readonly Size WinButtonSize = new Size(130, 30);

        protected readonly Size TouchYesNoCancelSize = new Size(365, 240);
        protected readonly Size TouchButtonSize = new Size(133, 50);
        #endregion

        #region Member Variables
        protected bool m_isTouchScreen;
        protected string m_button1Text = string.Empty;
        protected string m_button2Text = string.Empty;
        protected string m_button3Text = string.Empty;
        protected int m_defaultButton = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MessageForm class in window mode.
        /// </summary>
        /// <param name="text">The message to be displayed on the 
        /// form.</param>
        /// <param name="alignment">The string alignment of the text.</param>
        /// <param name="caption">The text to be displayed on the title 
        /// bar.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        protected MessageForm(string text, ContentAlignment alignment, string caption, MessageFormTypes type, int pause)
            : base(new NormalDisplayMode())
        {
            InitializeComponent();
            ArrangeForm(text, alignment, caption, type, pause);
        }

        /// <summary>
        /// Initializes a new instance of the MessageForm class in 
        /// touchscreen mode.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The message to be displayed on the 
        /// form.</param>
        /// <param name="alignment">The string alignment of the text.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        protected MessageForm(DisplayMode displayMode, string text, ContentAlignment alignment, MessageFormTypes type, int pause)
            : base(displayMode)
        {
            m_isTouchScreen = true;

            InitializeComponent();
            ApplyDisplayMode();
            ArrangeForm(text, alignment, null, type, pause);
        }

        protected MessageForm(DisplayMode displayMode, string text, string caption, ContentAlignment alignment, MessageFormTypes type, int pause)
            : base(displayMode)
        {
            m_isTouchScreen = true;

            InitializeComponent();
            ApplyDisplayMode();
            ArrangeForm(text, alignment, caption, type, pause);
        }

        protected MessageForm(DisplayMode displayMode, string text, string caption, MessageFormTypes type, int defaultButton, string button1Text, string button2Text, string button3Text, int pause)
            : base(displayMode)
        {
            m_isTouchScreen = true;
            DefaultButton = defaultButton;
            Button1Text = button1Text;
            Button2Text = button2Text;
            Button3Text = button3Text;

            InitializeComponent();
            ApplyDisplayMode();
            ArrangeForm(text, ContentAlignment.MiddleCenter, caption, type, pause);
        }

        protected MessageForm(string text, string caption, MessageFormTypes type, int defaultButton, string button1Text, string button2Text, string button3Text, int pause)
            : base(new NormalDisplayMode())
        {
            m_isTouchScreen = false;
            DefaultButton = defaultButton;
            Button1Text = button1Text;
            Button2Text = button2Text;
            Button3Text = button3Text;

            InitializeComponent();
            ApplyDisplayMode();
            ArrangeForm(text, ContentAlignment.MiddleCenter, caption, type, pause);
        }

        #endregion

        #region Member Methods

        /// <summary>
        /// Sets the settings of this form based on the current display mode.
        /// </summary>
        protected override void ApplyDisplayMode()
        {
            base.ApplyDisplayMode();

            // This is a dialog, so override the default size.
            Size = m_displayMode.DialogSize;
            StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// Arranges the controls on the form based on mode.
        /// </summary>
        /// <param name="text">The message to be displayed on the 
        /// form.</param>
        /// <param name="alignment">The string alignment of the text.</param>
        /// <param name="caption">The text to be displayed on the title 
        /// bar.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        protected void ArrangeForm(string text, ContentAlignment alignment, string caption, MessageFormTypes type, int pause = 0)
        {
            bool weAreAKiosk = false;
            ModuleComm modComm = null;

            // see if we are a kiosk
            try
            {
                modComm = new ModuleComm();

                int deviceType = modComm.GetDeviceId();

                weAreAKiosk = deviceType == Device.AdvancedPOSKiosk.Id || deviceType == Device.BuyAgainKiosk.Id || deviceType == Device.HybridKiosk.Id || deviceType == Device.SimplePOSKiosk.Id;
            }
            catch (Exception)
            {
            }

            DrawRounded = m_isTouchScreen;
            DrawAsGradient = true;
            DrawAsBurst = weAreAKiosk;

            bool UseColorButtons = type >= MessageFormTypes.ColorButtons;

            if (UseColorButtons)
                type -= MessageFormTypes.ColorButtons;

            ImageButton pressAfterPause = null;

            // Set the message.
            m_messageLabel.Text = text;
            m_messageLabel.TextAlign = alignment;

            // Change the form if this is not touchscreen.
            if (!m_isTouchScreen)
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                Size = WinSize;
                Font = WinFont;

                // Set the caption.
                if (!string.IsNullOrEmpty(caption))
                    Text = caption;
                else
                    Text = " ";
            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;
                Font = m_displayMode.MenuButtonFont;
                Text = "";
            }

            // Create the panel to hold the buttons.
            TableLayoutPanel innerPanel = new TableLayoutPanel();
            innerPanel.Dock = DockStyle.Fill;
            innerPanel.BackColor = Color.Transparent;

            // Create columns and buttons based on what which button mode.
            if(type == MessageFormTypes.YesNo || type == MessageFormTypes.YesNo_DefNO)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 5;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();

                yesButton.Text = Resources.MessageFormYes;
                yesButton.DialogResult = DialogResult.Yes;
                noButton.Text = Resources.MessageFormNo;
                noButton.DialogResult = DialogResult.No;

                if(!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.YesNo)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                    }
                    else //default to NO
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 1;
                    }
                }
                else
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.YesNo)
                    pressAfterPause = yesButton;
                else
                    pressAfterPause = noButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            else if (type == MessageFormTypes.ContinueCancel || type == MessageFormTypes.ContinueCancel_DefCancel)
            {
                //Returns OK for Continue and Cancel for Cancel

                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 5;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();

                yesButton.Text = Resources.MessageFormContinue;
                yesButton.DialogResult = DialogResult.OK;
                noButton.Text = Resources.MessageFormCancel;
                noButton.DialogResult = DialogResult.Cancel;

                if (!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;


                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.ContinueCancel)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                    }
                    else //default to Cancel
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 1;
                    }
                }
                else
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.YesNo)
                    pressAfterPause = yesButton;
                else
                    pressAfterPause = noButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            else if (type == MessageFormTypes.RetryAbort || type == MessageFormTypes.RetryAbort_DefAbort)
            {
//                m_messageLabel.Font = new Font(m_messageLabel.Font, FontStyle.Regular);

                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 5;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();

                yesButton.Text = Resources.MessageFormRetry;
                yesButton.DialogResult = DialogResult.Retry;
                noButton.Text = Resources.MessageFormAbort;
                noButton.DialogResult = DialogResult.Abort;

                if (!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.RetryAbort)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                    }
                    else //default to Abort
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 1;
                    }
                }
                else
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.RetryAbort)
                    pressAfterPause = yesButton;
                else
                    pressAfterPause = noButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            else if (type == MessageFormTypes.YesNo_FlatDesign || type == MessageFormTypes.YesNo_FlatDesign_DefNo)
            {
//               m_messageLabel.Font = new Font(m_messageLabel.Font, FontStyle.Regular);

                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 5;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();
     
                yesButton.Text = Resources.MessageFormYes;
                yesButton.DialogResult = DialogResult.Yes;
                noButton.Text = Resources.MessageFormNo;
                noButton.DialogResult = DialogResult.No;

                if (!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.YesNo_FlatDesign)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                    }
                    else //default to NO
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 1;
                    }
                }
                else
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.YesNo_FlatDesign)
                    pressAfterPause = yesButton;
                else
                    pressAfterPause = noButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                //Use flat background
                yesButton.ForeColor = System.Drawing.Color.Black;
                noButton.ForeColor = System.Drawing.Color.Black;         
                DrawAsGradient = false;
                System.Drawing.Color defaultBackground = System.Drawing.ColorTranslator.FromHtml("#44658D");
                this.BackColor = defaultBackground;
                this.ForeColor = System.Drawing.Color.White;
            }
            else if (type == MessageFormTypes.YesNo_regular || type == MessageFormTypes.YesNo_regular_DefNo)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 5;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();

                m_messageLabel.Font = new Font(m_messageLabel.Font, FontStyle.Regular);
                yesButton.Text = Resources.MessageFormYes;
                yesButton.DialogResult = DialogResult.Yes;
                noButton.Text = Resources.MessageFormNo;
                noButton.DialogResult = DialogResult.No;

                if(!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.YesNo_regular)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                    }
                    else //default to NO
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 1;
                    }
                }
                else
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 1;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.YesNo_regular)
                    pressAfterPause = yesButton;
                else
                    pressAfterPause = noButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                
                yesButton.Font = new Font(yesButton.Font, FontStyle.Regular);
                noButton.Font = new Font(noButton.Font, FontStyle.Regular);
                noButton.Focus();
                noButton.Select();

            }
            else if (type == MessageFormTypes.YesCancelComp || type == MessageFormTypes.YesCancelComp_DefCancel)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 5;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();

                m_messageLabel.Font = new Font(m_messageLabel.Font, FontStyle.Regular);
                yesButton.Text = "&Award";//Resources.MessageFormYes;

                yesButton.DialogResult = DialogResult.Yes;
                noButton.Text = Resources.MessageFormCancel;
                noButton.DialogResult = DialogResult.Cancel;

                //Use flat background 
                DrawAsGradient = false;
                System.Drawing.Color defaultBackground = System.Drawing.ColorTranslator.FromHtml("#44658D");
                this.BackColor = defaultBackground;
                this.ForeColor = System.Drawing.Color.White;
                yesButton.ForeColor = Color.Black;
                noButton.ForeColor = Color.Black;


                if (!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.YesCancelComp)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                    }
                    else //default to Cancel
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 1;
                    }
                }
                else
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.YesCancelComp)
                    pressAfterPause = yesButton;
                else
                    pressAfterPause = noButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                yesButton.Font = new Font(yesButton.Font, FontStyle.Regular);
                noButton.Font = new Font(noButton.Font, FontStyle.Regular);
            }
            else if (type == MessageFormTypes.YesCancel || type == MessageFormTypes.YesCancel_DefCancel)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 5;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();


                m_messageLabel.Font = new Font(m_messageLabel.Font, FontStyle.Regular);
                yesButton.Text = Resources.MessageFormYes;
              
                yesButton.DialogResult = DialogResult.Yes;
                noButton.Text = Resources.MessageFormCancel;
                noButton.DialogResult = DialogResult.Cancel;
       
                if (!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.YesCancel)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                    }
                    else //default to Cancel
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 1;
                    }
                }
                else
                {
                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.YesCancel)
                    pressAfterPause = yesButton;
                else
                    pressAfterPause = noButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                yesButton.Font = new Font(yesButton.Font, FontStyle.Regular);
                noButton.Font = new Font(noButton.Font, FontStyle.Regular);
            }
            else if(type == MessageFormTypes.YesNoCancel || type == MessageFormTypes.YesNoCancel_DefNo || type == MessageFormTypes.YesNoCancel_DefCancel)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 7;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();
                ImageButton cancelButton = new ImageButton();

                yesButton.Text = Resources.MessageFormYes;
                yesButton.DialogResult = DialogResult.Yes;
                noButton.Text = Resources.MessageFormNo;
                noButton.DialogResult = DialogResult.No;
                cancelButton.Text = Resources.MessageFormCancel;
                cancelButton.DialogResult = DialogResult.Cancel;

                if(!m_isTouchScreen)
                {
                    Size = WinYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        cancelButton.ImageNormal = Resources.YellowButtonUp;
                        cancelButton.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        cancelButton.ImageNormal = Resources.BlueButtonUp;
                        cancelButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    cancelButton.Size = WinButtonSize;
                    cancelButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.YesNoCancel)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                        cancelButton.TabIndex = 3;
                    }
                    else if (type == MessageFormTypes.YesNoCancel_DefNo)
                    {
                        yesButton.TabIndex = 3;
                        noButton.TabIndex = 1;
                        cancelButton.TabIndex = 2;
                    }
                    else //default to Cancel
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 3;
                        cancelButton.TabIndex = 1;
                    }
                }
                else
                {
                    Size = TouchYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        cancelButton.ImageNormal = Resources.YellowButtonUp;
                        cancelButton.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        cancelButton.ImageNormal = Resources.BigBlueButtonUp;
                        cancelButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    cancelButton.ShowFocus = false;
                    cancelButton.TabIndex = 0;
                    cancelButton.TabStop = false;
                    cancelButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                    cancelButton.Text = cancelButton.Text.Replace("&", string.Empty);
                    cancelButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.YesNoCancel)
                    pressAfterPause = yesButton;
                else if (type == MessageFormTypes.YesNoCancel_DefNo)
                    pressAfterPause = noButton;
                else //default to Cancel
                    pressAfterPause = cancelButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);
                innerPanel.Controls.Add(cancelButton, 5, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            else if (type == MessageFormTypes.RetryAbortIgnore || type == MessageFormTypes.RetryAbortIgnore_DefAbort || type == MessageFormTypes.RetryAbortIgnore_DefIgnore)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 7;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();
                ImageButton cancelButton = new ImageButton();

                yesButton.Text = Resources.MessageFormRetry;
                yesButton.DialogResult = DialogResult.Retry;
                noButton.Text = Resources.MessageFormAbort;
                noButton.DialogResult = DialogResult.Abort;
                cancelButton.Text = Resources.MessageFormIgnore;
                cancelButton.DialogResult = DialogResult.Ignore;

                if (!m_isTouchScreen)
                {
                    Size = WinYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        cancelButton.ImageNormal = Resources.YellowButtonUp;
                        cancelButton.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        cancelButton.ImageNormal = Resources.BlueButtonUp;
                        cancelButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    cancelButton.Size = WinButtonSize;
                    cancelButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.RetryAbortIgnore)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                        cancelButton.TabIndex = 3;
                    }
                    else if (type == MessageFormTypes.RetryAbortIgnore_DefAbort)
                    {
                        yesButton.TabIndex = 3;
                        noButton.TabIndex = 1;
                        cancelButton.TabIndex = 2;
                    }
                    else //default to Ignore
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 3;
                        cancelButton.TabIndex = 1;
                    }
                }
                else
                {
                    Size = TouchYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        cancelButton.ImageNormal = Resources.YellowButtonUp;
                        cancelButton.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        cancelButton.ImageNormal = Resources.BigBlueButtonUp;
                        cancelButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    cancelButton.ShowFocus = false;
                    cancelButton.TabIndex = 0;
                    cancelButton.TabStop = false;
                    cancelButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                    cancelButton.Text = cancelButton.Text.Replace("&", string.Empty);
                    cancelButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.RetryAbortIgnore)
                    pressAfterPause = yesButton;
                else if (type == MessageFormTypes.RetryAbortIgnore_DefAbort)
                    pressAfterPause = noButton;
                else //default to Ignore
                    pressAfterPause = cancelButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);
                innerPanel.Controls.Add(cancelButton, 5, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            else if (type == MessageFormTypes.RetryAbortForce || type == MessageFormTypes.RetryAbortForce_DefAbort || type == MessageFormTypes.RetryAbortForce_DefForce)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 7;

                ImageButton yesButton = new ImageButton();
                ImageButton noButton = new ImageButton();
                ImageButton cancelButton = new ImageButton();

                yesButton.Text = Resources.MessageFormRetry;
                yesButton.DialogResult = DialogResult.Retry;
                noButton.Text = Resources.MessageFormAbort;
                noButton.DialogResult = DialogResult.Abort;
                cancelButton.Text = Resources.MessageFormForce;
                cancelButton.DialogResult = DialogResult.Ignore;

                if (!m_isTouchScreen)
                {
                    Size = WinYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BlueButtonUp;
                        yesButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    yesButton.Size = WinButtonSize;
                    yesButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BlueButtonUp;
                        noButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    noButton.Size = WinButtonSize;
                    noButton.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        cancelButton.ImageNormal = Resources.YellowButtonUp;
                        cancelButton.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        cancelButton.ImageNormal = Resources.BlueButtonUp;
                        cancelButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    cancelButton.Size = WinButtonSize;
                    cancelButton.Font = WinButtonFont;

                    if (type == MessageFormTypes.RetryAbortForce)
                    {
                        yesButton.TabIndex = 1;
                        noButton.TabIndex = 2;
                        cancelButton.TabIndex = 3;
                    }
                    else if (type == MessageFormTypes.RetryAbortForce_DefAbort)
                    {
                        yesButton.TabIndex = 3;
                        noButton.TabIndex = 1;
                        cancelButton.TabIndex = 2;
                    }
                    else //default to Force
                    {
                        yesButton.TabIndex = 2;
                        noButton.TabIndex = 3;
                        cancelButton.TabIndex = 1;
                    }
                }
                else
                {
                    Size = TouchYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        yesButton.ImageNormal = Resources.GreenButtonUp;
                        yesButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        yesButton.ImageNormal = Resources.BigBlueButtonUp;
                        yesButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    yesButton.ShowFocus = false;
                    yesButton.TabIndex = 0;
                    yesButton.TabStop = false;
                    yesButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        noButton.ImageNormal = Resources.RedButtonUp;
                        noButton.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        noButton.ImageNormal = Resources.BigBlueButtonUp;
                        noButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    noButton.ShowFocus = false;
                    noButton.TabIndex = 0;
                    noButton.TabStop = false;
                    noButton.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        cancelButton.ImageNormal = Resources.YellowButtonUp;
                        cancelButton.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        cancelButton.ImageNormal = Resources.BigBlueButtonUp;
                        cancelButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    cancelButton.ShowFocus = false;
                    cancelButton.TabIndex = 0;
                    cancelButton.TabStop = false;
                    cancelButton.Size = TouchButtonSize;

                    // Remove Mnemonics
                    yesButton.Text = yesButton.Text.Replace("&", string.Empty);
                    yesButton.UseMnemonic = false;
                    noButton.Text = noButton.Text.Replace("&", string.Empty);
                    noButton.UseMnemonic = false;
                    cancelButton.Text = cancelButton.Text.Replace("&", string.Empty);
                    cancelButton.UseMnemonic = false;
                }

                if (type == MessageFormTypes.RetryAbortForce)
                    pressAfterPause = yesButton;
                else if (type == MessageFormTypes.RetryAbortForce_DefAbort)
                    pressAfterPause = noButton;
                else //default to Force
                    pressAfterPause = cancelButton;

                innerPanel.Controls.Add(yesButton, 1, 0);
                innerPanel.Controls.Add(noButton, 3, 0);
                innerPanel.Controls.Add(cancelButton, 5, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            else if (type == MessageFormTypes.Custom1Button || type == MessageFormTypes.Custom2Button || type == MessageFormTypes.Custom3Button)
            {
                innerPanel.RowCount = 1;

                if (type == MessageFormTypes.Custom1Button)
                    innerPanel.ColumnCount = 3;
                else if (type == MessageFormTypes.Custom2Button)
                    innerPanel.ColumnCount = 5;
                else
                    innerPanel.ColumnCount = 7;

                ImageButton button1 = new ImageButton();
                ImageButton button2 = new ImageButton();
                ImageButton button3 = new ImageButton();

                button1.Text = Button1Text;
                button1.DialogResult = (DialogResult)1;
                button2.Text = Button2Text;
                button2.DialogResult = (DialogResult)2;
                button3.Text = Button3Text;
                button3.DialogResult = (DialogResult)3;

                if (!m_isTouchScreen)
                {
                    if(type == MessageFormTypes.Custom3Button)
                        Size = WinYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        button1.ImageNormal = Resources.GreenButtonUp;
                        button1.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        button1.ImageNormal = Resources.BlueButtonUp;
                        button1.ImagePressed = Resources.BlueButtonDown;
                    }

                    button1.Size = WinButtonSize;
                    button1.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        button2.ImageNormal = Resources.RedButtonUp;
                        button2.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        button2.ImageNormal = Resources.BlueButtonUp;
                        button2.ImagePressed = Resources.BlueButtonDown;
                    }

                    button2.Size = WinButtonSize;
                    button2.Font = WinButtonFont;

                    if (UseColorButtons)
                    {
                        button3.ImageNormal = Resources.YellowButtonUp;
                        button3.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        button3.ImageNormal = Resources.BlueButtonUp;
                        button3.ImagePressed = Resources.BlueButtonDown;
                    }

                    button3.Size = WinButtonSize;
                    button3.Font = WinButtonFont;

                    if (DefaultButton == 0 || DefaultButton == 1)
                    {
                        button1.TabIndex = 1;
                        button2.TabIndex = 2;
                        button3.TabIndex = 3;
                    }
                    else if (DefaultButton == 2)
                    {
                        button1.TabIndex = type == MessageFormTypes.Custom2Button? 2 : 3;
                        button2.TabIndex = 1;
                        button3.TabIndex = type == MessageFormTypes.Custom2Button ? 2 : 0;
                    }
                    else //default 3
                    {
                        button1.TabIndex = 2;
                        button2.TabIndex = 3;
                        button3.TabIndex = 1;
                    }
                }
                else
                {
                    if(type == MessageFormTypes.Custom3Button)
                        Size = TouchYesNoCancelSize;

                    if (UseColorButtons)
                    {
                        button1.ImageNormal = Resources.GreenButtonUp;
                        button1.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        button1.ImageNormal = Resources.BigBlueButtonUp;
                        button1.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    button1.ShowFocus = false;
                    button1.TabIndex = 0;
                    button1.TabStop = false;
                    button1.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        button2.ImageNormal = Resources.RedButtonUp;
                        button2.ImagePressed = Resources.RedButtonDown;
                    }
                    else
                    {
                        button2.ImageNormal = Resources.BigBlueButtonUp;
                        button2.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    button2.ShowFocus = false;
                    button2.TabIndex = 0;
                    button2.TabStop = false;
                    button2.Size = TouchButtonSize;

                    if (UseColorButtons)
                    {
                        button3.ImageNormal = Resources.YellowButtonUp;
                        button3.ImagePressed = Resources.YellowButtonDown;
                    }
                    else
                    {
                        button3.ImageNormal = Resources.BigBlueButtonUp;
                        button3.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    button3.ShowFocus = false;
                    button3.TabIndex = 0;
                    button3.TabStop = false;
                    button3.Size = TouchButtonSize;

                    // Remove hotkeys
                    button1.Text = button1.Text.Replace("&", string.Empty);
                    button1.UseMnemonic = false;
                    button2.Text = button2.Text.Replace("&", string.Empty);
                    button2.UseMnemonic = false;
                    button3.Text = button3.Text.Replace("&", string.Empty);
                    button3.UseMnemonic = false;
                }

                if (DefaultButton == 0 || DefaultButton ==1)
                    pressAfterPause = button1;
                else if (DefaultButton == 2)
                    pressAfterPause = button2;
                else //default to 3
                    pressAfterPause = button3;

                if (type == MessageFormTypes.Custom1Button)
                {
                    innerPanel.Controls.Add(button1, 1, 0);

                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                }
                else if (type == MessageFormTypes.Custom2Button)
                {
                    innerPanel.Controls.Add(button1, 1, 0);
                    innerPanel.Controls.Add(button2, 3, 0);

                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                }
                else
                {
                    innerPanel.Controls.Add(button1, 1, 0);
                    innerPanel.Controls.Add(button2, 3, 0);
                    innerPanel.Controls.Add(button3, 5, 0);

                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                    innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                }
            }
            else if (type == MessageFormTypes.OK_FlatDesign)
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 3;

                ImageButton okButton = new ImageButton();

                okButton.Text = Resources.MessageFormOk;
                okButton.DialogResult = DialogResult.OK;

                //Use flat background 
                DrawAsGradient = false;
                System.Drawing.Color defaultBackground = System.Drawing.ColorTranslator.FromHtml("#44658D");
                this.BackColor = defaultBackground;
                this.ForeColor = System.Drawing.Color.White;
                okButton.ForeColor = Color.Black;
                //yesButton.ForeColor = Color.Black;
                //noButton.ForeColor = Color.Black;

                if (!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        okButton.ImageNormal = Resources.GreenButtonUp;
                        okButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        okButton.ImageNormal = Resources.BlueButtonUp;
                        okButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    okButton.TabIndex = 1;
                    okButton.Size = WinButtonSize;
                    okButton.Font = WinButtonFont;
                }
                else
                {
                    if (UseColorButtons)
                    {
                        okButton.ImageNormal = Resources.GreenButtonUp;
                        okButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        okButton.ImageNormal = Resources.BigBlueButtonUp;
                        okButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    okButton.ShowFocus = false;
                    okButton.TabIndex = 0;
                    okButton.TabStop = false;
                    okButton.Size = TouchButtonSize;

                    // Remove Mnemonic
                    okButton.Text = okButton.Text.Replace("&", string.Empty);
                    okButton.UseMnemonic = false;
                }

                pressAfterPause = okButton;

                if (type == MessageFormTypes.Pause)
                    okButton.Visible = false;

                innerPanel.Controls.Add(okButton, 1, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
            else
            {
                innerPanel.RowCount = 1;
                innerPanel.ColumnCount = 3;

                ImageButton okButton = new ImageButton();

                okButton.Text = Resources.MessageFormOk;
                okButton.DialogResult = DialogResult.OK;

                if (!m_isTouchScreen)
                {
                    if (UseColorButtons)
                    {
                        okButton.ImageNormal = Resources.GreenButtonUp;
                        okButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        okButton.ImageNormal = Resources.BlueButtonUp;
                        okButton.ImagePressed = Resources.BlueButtonDown;
                    }

                    okButton.TabIndex = 1;
                    okButton.Size = WinButtonSize;
                    okButton.Font = WinButtonFont;
                }
                else
                {
                    if (UseColorButtons)
                    {
                        okButton.ImageNormal = Resources.GreenButtonUp;
                        okButton.ImagePressed = Resources.GreenButtonDown;
                    }
                    else
                    {
                        okButton.ImageNormal = Resources.BigBlueButtonUp;
                        okButton.ImagePressed = Resources.BigBlueButtonDown;
                    }

                    okButton.ShowFocus = false;
                    okButton.TabIndex = 0;
                    okButton.TabStop = false;
                    okButton.Size = TouchButtonSize;

                    // Remove Mnemonic
                    okButton.Text = okButton.Text.Replace("&", string.Empty);
                    okButton.UseMnemonic = false;
                }

                pressAfterPause = okButton;

                if (type == MessageFormTypes.Pause)
                    okButton.Visible = false;

                innerPanel.Controls.Add(okButton, 1, 0);

                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 133F));
                innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }

            m_outerPanel.Controls.Add(innerPanel, 0, 1);
            innerPanel.Click += SomethingWasClicked;

            if(type == MessageFormTypes.Pause)
            {
                if (pause == 0)
                    pause = 5000;

                if(pause < 1000)
                    pause = 1000;
            }

            if(pause > 0)
            {
                m_pauseProgress.Minimum = 0;
                m_pauseProgress.Maximum = pause;
                m_pauseProgress.Value = 0;
                m_pauseProgress.Visible = true;

                m_pauseTimer.Interval = 100;
                m_pauseTimer.Tag = pressAfterPause;
                m_pauseTimer.Start();
            }
        }

        /// <summary>
        /// Handles the timer's Tick event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An EventArgs object that contains the event 
        /// data.</param>
        private void PauseTimerTick(object sender, EventArgs e)
        {
            m_pauseProgress.Increment(m_pauseTimer.Interval);

            if (m_pauseProgress.Value < m_pauseProgress.Maximum)
                return;

            m_pauseTimer.Stop();

            m_pauseProgress.Visible = false;

            if (!((ImageButton)m_pauseTimer.Tag).Visible) //need to see the button to click it
                ((ImageButton)m_pauseTimer.Tag).Visible = true;

            ((ImageButton)m_pauseTimer.Tag).PerformClick();
        }

        #endregion

        #region Static Methods
        /// <summary>
        /// Displays a windowed OK message form with specified text.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(string text)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, null, MessageFormTypes.OK, 0);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen OK message form with specified text.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>One of the DialogResult values.</returns>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        public static DialogResult Show(DisplayMode displayMode, string text)
        {
            MessageForm form = new MessageForm(displayMode, text, ContentAlignment.MiddleCenter, MessageFormTypes.OK, 0);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed OK message form in front of the specified 
        /// object with specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, string text)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, null, MessageFormTypes.OK, 0);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen OK message form in front of the specified 
        /// object with specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>One of the DialogResult values.</returns>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        public static DialogResult Show(IWin32Window owner, DisplayMode displayMode, string text)
        {
            MessageForm form = new MessageForm(displayMode, text, ContentAlignment.MiddleCenter, MessageFormTypes.OK, 0);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents(); //let the screen repaint smoothly

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed OK message form with specified text and 
        /// caption.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(string text, string caption)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, caption, MessageFormTypes.OK, 0);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed OK message form in front of the specified 
        /// object with specified text and caption.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, caption, MessageFormTypes.OK, 0);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed message form with specified text and caption.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <returns>One of the DialogResult values.</returns>
        /// <remarks>If a MessageFormType of Pause is specified, then the pause
        /// length will be five seconds.</remarks>
        public static DialogResult Show(string text, string caption, MessageFormTypes type)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, caption, type, 0);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen message form with specified text.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <returns>One of the DialogResult values.</returns>
        /// <remarks>If a MessageFormType of Pause is specified, then the pause
        /// length will be five seconds.</remarks>
        public static DialogResult Show(DisplayMode displayMode, string text, MessageFormTypes type)
        {
            MessageForm form = new MessageForm(displayMode, text, ContentAlignment.MiddleCenter, type, 0);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed message form in front of the specified object 
        /// with specified text and caption.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <returns>One of the DialogResult values.</returns>
        /// <remarks>If a MessageFormType of Pause is specified, then the pause
        /// length will be five seconds.</remarks>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageFormTypes type)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, caption, type, 0);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen message form in front of the specified object 
        /// with specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <returns>One of the DialogResult values.</returns>
        /// <remarks>If a MessageFormType of Pause is specified, then the pause
        /// length will be five seconds.</remarks>
        public static DialogResult Show(IWin32Window owner, DisplayMode displayMode, string text, MessageFormTypes type)
        {
            MessageForm form = new MessageForm(displayMode, text, ContentAlignment.MiddleCenter, type, 0);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed message form with specified text and caption.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(string text, string caption, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, caption, type, pause);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen message form with specified text.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(DisplayMode displayMode, string text, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(displayMode, text, ContentAlignment.MiddleCenter, type, pause);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed message form in front of the specified object 
        /// with specified text and caption.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(text, ContentAlignment.MiddleCenter, caption, type, pause);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen message form in front of the specified object 
        /// with specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, DisplayMode displayMode, string text, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(displayMode, text, ContentAlignment.MiddleCenter, type, pause);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        public static DialogResult Show(IWin32Window owner, DisplayMode displayMode, string text, string caption, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(displayMode, text, caption, ContentAlignment.MiddleCenter, type, pause);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed message form with specified text and caption.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(string text, ContentAlignment alignment, string caption, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(text, alignment, caption, type, pause);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen message form with specified text.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(DisplayMode displayMode, string text, ContentAlignment alignment, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(displayMode, text, alignment, type, pause);
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed message form in front of the specified object 
        /// with specified text and caption.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, string text, ContentAlignment alignment, string caption, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(text, alignment, caption, type, pause);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen message form in front of the specified object 
        /// with specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will 
        /// own the modal dialog box.</param>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(IWin32Window owner, DisplayMode displayMode, string text, ContentAlignment alignment, MessageFormTypes type, int pause)
        {
            MessageForm form = new MessageForm(displayMode, text, alignment, type, pause);
            DialogResult result = form.ShowDialog(owner);
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Shows a message form with no buttons for the given number of milliseconds.
        /// </summary>
        /// <param name="displayMode"></param>
        /// <param name="text"></param>
        /// <param name="pause"></param>
        /// <returns></returns>
        public static DialogResult TimedDisplay(DisplayMode displayMode, string text, int pause, Font font = null)
        {
            MessageForm form = new MessageForm(displayMode, text, ContentAlignment.MiddleCenter, MessageFormTypes.Pause, pause);
       
            if(font != null)
                form.Font = font;

            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a windowed message form with specified text and caption.
        /// </summary>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="caption">The text to display in the title bar of the 
        /// message box.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <param name="topMost">Whether this form is top most.</param>
        /// <param name="startPosition">The form's start position.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(string text, ContentAlignment alignment, string caption, MessageFormTypes type, int pause, bool topMost, FormStartPosition startPosition)
        {
            MessageForm form = new MessageForm(text, alignment, caption, type, pause);
            form.TopMost = topMost;
            form.StartPosition = startPosition;
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Displays a touchscreen message form with specified text.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="alignment">The alignment of the text.</param>
        /// <param name="type">The type of MessageForm to show.</param>
        /// <param name="pause">If the type is Pause, then this is how long to 
        /// show the dialog (in milliseconds).  It cannot be less 
        /// than one.</param>
        /// <param name="topMost">Whether this form is top most.</param>
        /// <param name="startPosition">The form's start position.</param>
        /// <exception cref="System.ArgumentException">type is 
        /// MessageFormTypes.Pause and pause is less than one.</exception>
        /// <returns>One of the DialogResult values.</returns>
        public static DialogResult Show(DisplayMode displayMode, string text, ContentAlignment alignment, MessageFormTypes type, int pause, bool topMost, FormStartPosition startPosition)
        {
            MessageForm form = new MessageForm(displayMode, text, alignment, type, pause);
            form.TopMost = topMost;
            form.StartPosition = startPosition;
            DialogResult result = form.ShowDialog();
            Application.DoEvents();

            form.Dispose();
            form = null;

            return result;
        }

        /// <summary>
        /// Shows a single button message dialog with user defined button text.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="displayMode">Pass null if not touch screen.</param>
        /// <param name="text">Message text.</param>
        /// <param name="title">Caption text (if there is a caption).</param>
        /// <param name="color">Should the button be green?</param>
        /// <param name="button1Text">Text for button.</param>
        /// <param name="pause">Milliseconds to show dialog before terminating.  Timeout will return defaultButton.</param>
        /// <returns>Button pressed = 1.</returns>
        public static int ShowCustomOneButton(IWin32Window owner, DisplayMode displayMode, string text, string title, bool color, string button1Text, int pause)
        {
            MessageFormTypes type = MessageFormTypes.Custom1Button;

            if (color)
                type = (MessageFormTypes)((int)type + (int)MessageFormTypes.ColorButtons);

            MessageForm form;
            
            if(displayMode == null)
                form = new MessageForm(text, title, type, 1, button1Text, "", "", pause);
            else
                form = new MessageForm(displayMode, text, title, type, 1, button1Text, "", "", pause);

            DialogResult result = form.ShowDialog(owner);

            Application.DoEvents();

            form.Dispose();
            form = null;

            return (int)result;
        }

        /// <summary>
        /// Shows a two button message dialog with user defined button text.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="displayMode">Pass null if not touch screen.</param>
        /// <param name="text">Message text.</param>
        /// <param name="title">Caption text (if there is a caption).</param>
        /// <param name="color">Should the buttons be green and red?</param>
        /// <param name="defaultButton">Which button is the default, 1 or 2?</param>
        /// <param name="button1Text">Text for button 1.</param>
        /// <param name="button2Text">Text for button 2.</param>
        /// <param name="pause">Milliseconds to show dialog before terminating.  Timeout will return defaultButton.</param>
        /// <returns>Button pressed = 1 or 2.</returns>
        public static int ShowCustomTwoButton(IWin32Window owner, DisplayMode displayMode, string text, string title, bool color, int defaultButton, string button1Text, string button2Text, int pause)
        {
            MessageFormTypes type = MessageFormTypes.Custom2Button;

            if (color)
                type = (MessageFormTypes)((int)type + (int)MessageFormTypes.ColorButtons);

            MessageForm form;

            if (displayMode == null)
                form = new MessageForm(text, title, type, defaultButton, button1Text, button2Text, "", pause);
            else
                form = new MessageForm(displayMode, text, title, type, defaultButton, button1Text, button2Text, "", pause);

            DialogResult result = form.ShowDialog(owner);

            Application.DoEvents();

            form.Dispose();
            form = null;

            return (int)result;
        }

        /// <summary>
        /// Shows a three button message dialog with user defined button text.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="displayMode">Pass null if not touch screen.</param>
        /// <param name="text">Message text.</param>
        /// <param name="title">Caption text (if there is a caption).</param>
        /// <param name="color">Should the buttons be green, red, and yellow?</param>
        /// <param name="defaultButton">Which button is the default, 1, 2, or 3?</param>
        /// <param name="button1Text">Text for button 1.</param>
        /// <param name="button2Text">Text for button 2.</param>
        /// <param name="button3Text">Text for button 3.</param>
        /// <param name="pause">Milliseconds to show dialog before terminating.  Timeout will return defaultButton.</param>
        /// <returns>Button pressed = 1, 2, or 3.</returns>
        public static int ShowCustomThreeButton(IWin32Window owner, DisplayMode displayMode, string text, string title, bool color, int defaultButton, string button1Text, string button2Text, string button3Text, int pause)
        {
            MessageFormTypes type = MessageFormTypes.Custom3Button;

            if (color)
                type = (MessageFormTypes)((int)type + (int)MessageFormTypes.ColorButtons);

            MessageForm form;

            if (displayMode == null)
                form = new MessageForm(text, title, type, defaultButton, button1Text, button2Text, button3Text, pause);
            else
                form = new MessageForm(displayMode, text, title, type, defaultButton, button1Text, button2Text, button3Text, pause);

            DialogResult result = form.ShowDialog(owner);

            Application.DoEvents();

            form.Dispose();
            form = null;

            return (int)result;
        }

        private void SomethingWasClicked(object sender, EventArgs e)
        {
            m_pauseProgress.Value = 0;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets the text for button 1 for a custom message form.
        /// </summary>
        public string Button1Text
        {
            get
            {
                return m_button1Text;
            }

            set
            {
                m_button1Text = value;
            }
        }

        /// <summary>
        /// Gets/sets the text for button 2 for a custom message form.
        /// </summary>
        public string Button2Text
        {
            get
            {
                return m_button2Text;
            }

            set
            {
                m_button2Text = value;
            }
        }

        /// <summary>
        /// Gets/sets the text for button 3 for a custom message form.
        /// </summary>
        public string Button3Text
        {
            get
            {
                return m_button3Text;
            }

            set
            {
                m_button3Text = value;
            }
        }

        /// <summary>
        /// Gets/sets the default button for a cutom message form.
        /// 0 = none
        /// </summary>
        public int DefaultButton
        {
            get
            {
                return m_defaultButton;
            }

            set
            {
                m_defaultButton = value;
            }
        }
        #endregion
    }
}