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
    /// An abstract base class for specifying how to display interface 
    /// elements to the user.
    /// </summary>
    public abstract class DisplayMode
    {
        #region Member Variables
        protected Size m_formSize;
        protected Size m_baseFormSize;
        protected Size m_dialogSize;
        protected Size m_largeDialogSize;
        protected Point m_centerFormLocation;
        protected Size m_menuButtonSize;
        protected int m_menuButtonXSpacing; // Pixels
        protected int m_menuButtonYSpacing; // Pixels
        protected byte m_menuButtonsPerPage;
        protected int m_menuButtonsPerColumn;
        protected Font m_menuButtonFont;
        protected int m_systemMenuButtonsPerPage;
        protected int m_menuPagesPerPOSPage = 1;
        protected int m_XoffsetForFullScreen = 0;
        protected int m_YoffsetForFullScreen = 0;
        protected bool m_fullScreen = false;
        protected bool m_wideScreen = false;
        #endregion

        #region Member Methods

        /// <summary>
        /// Calls this display mode's basic constructor.
        /// </summary>
        /// <returns>Normal display mode.</returns>
        public DisplayMode BasicCopy()
        {
            if (this is NormalDisplayMode)
                return new NormalDisplayMode();
            else if (this is WideDisplayMode)
                return new WideDisplayMode();
            else if (this is CompactDisplayMode)
                return new CompactDisplayMode();

            return null;
        }

        #endregion

        #region Member Properties
        public Size NormalFormSize
        {
            get
            {
                return new Size(1024, 768);
            }
        }

        public Size WideFormSize
        {
            get
            {
                return new Size(1366, 768);
            }
        }

        public Size CompactFormSize
        {
            get
            {
                return new Size(800, 600);
            }
        }
        
        /// <summary>
        /// Gets the size of normal forms.
        /// </summary>
        public Size FormSize
        {
            get
            {
                return m_formSize;
            }
        }

        /// <summary>
        /// Gets the size of the form if not full screen.
        /// </summary>
        public Size BaseFormSize
        {
            get
            {
                return m_baseFormSize;
            }
        }

        /// <summary>
        /// Gets the location of the top left corner of the form centered on the screen.
        /// </summary>
        public Point CenterFormLocation
        {
            get
            {
                return m_centerFormLocation;
            }
        }

        /// <summary>
        /// Offset from left edge of form to centered window.
        /// </summary>
        public int OffsetForFullScreenX
        {
            get
            {
                return m_XoffsetForFullScreen;
            }

            protected set
            {
                m_XoffsetForFullScreen = value;
            }
        }

        /// <summary>
        /// Offset from top edge of form to centered window.
        /// </summary>
        public int OffsetForFullScreenY
        {
            get
            {
                return m_YoffsetForFullScreen;
            }

            protected set
            {
                m_YoffsetForFullScreen = value;
            }
        }

        /// <summary>
        /// Increase in width from Normal to Wide.
        /// </summary>
        public int WidthIncreaseFromNormal
        {
            get
            {
                return m_wideScreen? WideFormSize.Width - NormalFormSize.Width : 0;
            }
        }

        /// <summary>
        /// Increase in height from Normal to Wide.
        /// </summary>
        public int HeightIncreaseFromNormal
        {
            get
            {
                return m_wideScreen ? WideFormSize.Height - NormalFormSize.Height : 0;
            }
        }

        /// <summary>
        /// X adjustment for centering controls on wide with normal background.
        /// </summary>
        public int EdgeAdjustmentForNormalToWideX
        {
            get
            {
                return WidthIncreaseFromNormal / 2;
            }
        }

        /// <summary>
        /// Y adjustment for centering controls on wide with normal background.
        /// </summary>
        public int EdgeAdjustmentForNormalToWideY
        {
            get
            {
                return HeightIncreaseFromNormal / 2;
            }
        }

        /// <summary>
        /// Gets the size of dialog forms.
        /// </summary>
        public Size DialogSize
        {
            get
            {
                return m_dialogSize;
            }
        }

        /// <summary>
        /// Gets the size of large dialog forms.
        /// </summary>
        public Size LargeDialogSize
        {
            get
            {
                return m_largeDialogSize;
            }
        }

        /// <summary>
        /// Gets the size of menu buttons.
        /// </summary>
        public Size MenuButtonSize
        {
            get
            {
                return m_menuButtonSize;
            }
        }

        /// <summary>
        /// Gets the horizontal pixel spacing of menu buttons.
        /// </summary>
        public int MenuButtonXSpacing
        {
            get
            {
                return m_menuButtonXSpacing;
            }
        }

        /// <summary>
        /// Gets the vertical pixel spacing of menu buttons.
        /// </summary>
        public int MenuButtonYSpacing
        {
            get
            {
                return m_menuButtonYSpacing;
            }
        }

        /// <summary>
        /// Gets the number of menu buttons per page.
        /// </summary>
        public byte MenuButtonsPerPage
        {
            get
            {
                return m_menuButtonsPerPage;
            }
        }

        public int MenuPagesPerPOSPage
        {
            get
            {
                return m_menuPagesPerPOSPage;
            }

            set
            {
                if (m_menuPagesPerPOSPage != 1)
                    throw new Exception("POS menu pages can not be adjusted twice.");

                m_menuPagesPerPOSPage = value;
                m_menuButtonSize.Width /= m_menuPagesPerPOSPage;
                m_menuButtonXSpacing = 0;
                m_menuButtonsPerPage *= (byte)m_menuPagesPerPOSPage;
            }
        }

        /// <summary>
        /// Gets the number of menu buttons per column.
        /// </summary>
        public int MenuButtonsPerColumn
        {
            get
            {
                return m_menuButtonsPerColumn;
            }
        }

        /// <summary>
        /// Gets the font to use on menu buttons.
        /// </summary>
        public Font MenuButtonFont
        {
            get
            {
                return m_menuButtonFont;
            }
        }

        /// <summary>
        /// Gets the number of buttons per page on system menu.
        /// </summary>
        public int SystemMenuButtonsPerPage
        {
            get
            {
                return m_systemMenuButtonsPerPage;
            }
        }

        public bool IsWidescreen
        {
            get
            {
                return m_wideScreen;
            }

            protected set
            {
                m_wideScreen = value;
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return m_fullScreen;
            }

            protected set
            {
                m_fullScreen = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Specifies how to display interface elements to the user on normal 
    /// screens.
    /// </summary>
    public class NormalDisplayMode : DisplayMode
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the NormalDisplayMode class.
        /// </summary>
        public NormalDisplayMode(bool fullScreen = false)
        {
//            Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Rectangle rect = System.Windows.Forms.Screen.FromPoint(Cursor.Position).Bounds;

            if (fullScreen)
            {
                IsFullScreen = true;
                m_formSize = new Size(rect.Width, rect.Height);
            }
            else
            {
                m_formSize = NormalFormSize;
            }

            OffsetForFullScreenX = (m_formSize.Width - NormalFormSize.Width) / 2;
            OffsetForFullScreenY = (m_formSize.Height - NormalFormSize.Height) / 2;

            m_centerFormLocation = new Point((rect.Width - m_formSize.Width) / 2, (rect.Height - m_formSize.Height) / 2);

            m_baseFormSize = NormalFormSize;
            m_dialogSize = new Size(320, 240);
            m_largeDialogSize = new Size(800, 600);
            m_menuButtonSize = new Size(138, 80);
            m_menuButtonXSpacing = 5;
            m_menuButtonYSpacing = 4;
            m_menuButtonsPerPage = 20;
            m_menuButtonsPerColumn = 5;
            m_menuButtonFont = new Font("Tahoma", 11, System.Drawing.FontStyle.Bold);
            m_systemMenuButtonsPerPage = 7;
        }
        #endregion
    }

    /// <summary>
    /// Specifies how to display interface elements to the user on normal 
    /// screens.
    /// </summary>
    public class WideDisplayMode : DisplayMode
    {
        protected bool m_twoPagesPerPage = false;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WideDisplayMode class.
        /// </summary>
        public WideDisplayMode(bool twoMenuPagesOnEachOfOurPages = false, bool fullScreen = false)
        {
//            Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Rectangle rect = System.Windows.Forms.Screen.FromPoint(Cursor.Position).Bounds;

            IsWidescreen = true;

            if (fullScreen)
            {
                IsFullScreen = true;
                m_formSize = new Size(rect.Width, rect.Height);
            }
            else
            {
                m_formSize = WideFormSize;
            }

            m_baseFormSize = WideFormSize;

            OffsetForFullScreenX = (m_formSize.Width - WideFormSize.Width) / 2;
            OffsetForFullScreenY = (m_formSize.Height - WideFormSize.Height) / 2;

            m_centerFormLocation = new Point((rect.Width - m_formSize.Width) / 2, (rect.Height - m_formSize.Height) / 2);

            m_dialogSize = new Size(320, 240);
            m_largeDialogSize = new Size(800, 600);
            m_systemMenuButtonsPerPage = 12;
            m_menuButtonFont = new Font("Tahoma", 11, System.Drawing.FontStyle.Bold);

            if (twoMenuPagesOnEachOfOurPages)
            {
                m_twoPagesPerPage = true;
                m_menuButtonSize = new Size(112, 82);
                m_menuButtonXSpacing = 2;
                m_menuButtonYSpacing = 2;
                m_menuButtonsPerPage = 40;
                m_menuButtonsPerColumn = 5;
            }
            else
            {
                m_twoPagesPerPage = false;
                m_menuButtonSize = new Size(215, 80);
                m_menuButtonXSpacing = 17;
                m_menuButtonYSpacing = 4;
                m_menuButtonsPerPage = 20;
                m_menuButtonsPerColumn = 5;
            }
        }
        #endregion

        #region Properties

        public bool TwoPagesPerPage
        {
            get
            {
                return m_twoPagesPerPage;
            }
        }

        #endregion
    }

    /// <summary>
    /// Specifies how to display interface elements to the user on compact 
    /// screens.
    /// </summary>
    public class CompactDisplayMode : DisplayMode
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CompactDisplayMode class.
        /// </summary>
        public CompactDisplayMode()
        {
//            Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Rectangle rect = System.Windows.Forms.Screen.FromPoint(Cursor.Position).Bounds;

            m_formSize = CompactFormSize;
            m_baseFormSize = CompactFormSize;
            m_dialogSize = new Size(320, 240);
            m_largeDialogSize = new Size(800, 600);
            m_menuButtonSize = new Size(108, 80);
            m_menuButtonXSpacing = 5;
            m_menuButtonYSpacing = 4;
            m_menuButtonsPerPage = 12;
            m_menuButtonsPerColumn = 3;
            m_menuButtonFont = new Font("Tahoma", 9, FontStyle.Bold);
            m_systemMenuButtonsPerPage = 4;
            m_centerFormLocation = new Point((rect.Width - m_formSize.Width) / 2, (rect.Height - m_formSize.Height) / 2);
        }
        #endregion
    }
}
