// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// A form that optionally has a linear gradient background.
    /// </summary>
    public partial class GradientForm : Form
    {
        #region Member Methods
        protected bool m_drawGradient = true;
        protected LinearGradientMode m_gradientMode = LinearGradientMode.Vertical;
        protected Color m_beginColor = Color.FromArgb(156, 179, 213);
        protected Color m_endColor = Color.FromArgb(184, 186, 192);
        #endregion

        #region Constructors
        /// <summary>
        /// Initalizes a new instance of the GradientForm class.
        /// </summary>
        public GradientForm()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            InitializeComponent();
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Handles the Form's paint background event.
        /// </summary>
        /// <param name="e">An PaintEventArgs object that contains the 
        /// event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if(m_drawGradient && ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, m_beginColor, m_endColor, m_gradientMode);
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets whether to draw the gradient background.
        /// </summary>
        [Description("Whether to draw the gradient background.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool DrawGradient
        {
            get
            {
                return m_drawGradient;
            }
            set
            {
                m_drawGradient = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the background gradient's mode.
        /// </summary>
        [Description("The background gradient's mode.")]
        [Category("Appearance")]
        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientMode
        {
            get
            {
                return m_gradientMode;
            }
            set
            {
                m_gradientMode = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the gradient's start color.
        /// </summary>
        [Description("The background gradient's start color.")]
        [Category("Appearance")]
        public Color GradientBeginColor
        {
            get
            {
                return m_beginColor;
            }
            set
            {
                m_beginColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the gradient's end color.
        /// </summary>
        [Description("The background gradient's end color.")]
        [Category("Appearance")]
        public Color GradientEndColor
        {
            get
            {
                return m_endColor;
            }
            set
            {
                m_endColor = value;
                Invalidate();
            }
        }
        #endregion
    }
}