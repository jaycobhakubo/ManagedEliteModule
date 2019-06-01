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
    /// A form that can either be an Elite touchscreen based form or a 
    /// gradient form.
    /// </summary>
    /// <remarks>This class is not intended to be used directly by any 
    /// modules.</remarks>
    public partial class EliteGradientForm : EliteForm
    {
        #region Member Variables
        protected bool m_drawGradient = false;
        protected bool m_drawRounded = false;
        protected bool m_drawInnerBorder = false;
        protected bool m_drawEdge = false;
        protected bool m_drawBurst = false;
        protected bool m_drawColorBorder = false;
        protected bool m_drawReverseGradientBorder = false;
        protected LinearGradientMode m_gradientMode = LinearGradientMode.Vertical;
        protected Color m_beginColor = Color.FromArgb(156, 179, 213);
        protected Color m_endColor = Color.FromArgb(184, 186, 192);
        protected Color m_edgeColor = Color.SlateGray;
        protected Color m_innerBorderColor = Color.SlateGray;
        protected Color m_borderColor = Color.Silver;
        protected int m_outerRadius = 30;
        protected int m_innerRadius = 20;
        protected int m_borderThickness = 6;
        private Point? m_locationBeforeHide = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initalizes a new instance of the EliteGradientForm class.
        /// Required method for Designer support.
        /// </summary>
        public EliteGradientForm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EliteGradientForm class.
        /// </summary>
        /// <param name="displayMode">The display mode used to show this 
        /// form.</param>
        /// <exception cref="System.ArgumentNullException">displayMode is a 
        /// null reference.</exception>
        public EliteGradientForm(DisplayMode displayMode)
            : base(displayMode)
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
            //if (DesignMode && DrawingRounded && (Region.GetBounds(e.Graphics).Width != Size.Width || Region.GetBounds(e.Graphics).Height != Size.Height)) //make a clipping region out of a rectangle with rounded corners
            //{
            //    using (GraphicsPath path = GetRoundRectangle(0, m_outerRadius))
            //        Region = new Region(path);

            //    Invalidate();

            //    return;
            //}

            base.OnPaintBackground(e);

            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                if (m_drawBurst)
                {
                    GraphicsPath path = new GraphicsPath();

                    path.AddEllipse(ClientRectangle);
                    
                    PathGradientBrush brush = new PathGradientBrush(path);
                    SolidBrush outerBrush = new SolidBrush(m_beginColor);

                    brush.CenterColor = m_endColor;
                    brush.SurroundColors = new Color[] { m_beginColor };

                    e.Graphics.FillRectangle(outerBrush, ClientRectangle);
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                    
                    brush.Dispose();
                    outerBrush.Dispose();
                }
                else if(m_drawGradient)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, m_beginColor, m_endColor, m_gradientMode);
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                    brush.Dispose();
                }

                GraphicsPath pathIn = null;
                GraphicsPath pathOut = null;
                bool drawingBorder = m_drawColorBorder || m_drawReverseGradientBorder;

                if (drawingBorder || m_drawEdge || m_drawInnerBorder)
                {
                    if (DrawingRounded)
                    {
                        if(drawingBorder || m_drawInnerBorder)
                            pathIn = GetRoundRectangle(m_borderThickness, m_innerRadius);

                        if (drawingBorder || m_drawEdge)
                            pathOut = GetRoundRectangle(0, m_outerRadius);
                    }
                    else
                    {
                        Rectangle rect = ClientRectangle;

                        if (drawingBorder || m_drawInnerBorder)
                        {
                            rect.Inflate(-m_borderThickness, -m_borderThickness);
                            pathIn = new GraphicsPath();
                            pathIn.AddRectangle(rect);
                        }

                        if (drawingBorder || m_drawEdge)
                        {
                            rect = ClientRectangle;
                            rect.Inflate(-1, -1);
                            pathOut = new GraphicsPath();
                            pathOut.AddRectangle(rect);
                        }
                    }

                    if (drawingBorder)
                    {
                        Region region = new Region(pathOut);

                        region.Exclude(pathIn);

                        if (m_drawReverseGradientBorder)
                            e.Graphics.FillRegion(new LinearGradientBrush(ClientRectangle, m_endColor, m_beginColor, m_gradientMode), region);
                        else
                            e.Graphics.FillRegion(new SolidBrush(m_borderColor), region);

                        region.Dispose();
                    }

                    if (m_drawInnerBorder)
                    {
                        using (Pen pen = new Pen(m_innerBorderColor, 1))
                        {
                            e.Graphics.DrawPath(pen, pathIn);
                        }
                    }

                    if (m_drawEdge)
                    {
                        using (Pen pen = new Pen(m_edgeColor, DrawingRounded? 4:2))
                        {
                            e.Graphics.DrawPath(pen, pathOut);
                        }
                    }

                    if(pathOut != null)
                        pathOut.Dispose();

                    if(pathIn != null)
                        pathIn.Dispose();
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Form_SizeChanged(null, e);
            Invalidate();
            Update();
        }

        private void Form_SizeChanged(object sender, EventArgs e)
        {
            if (DrawingRounded) //make a clipping region out of a rectangle with rounded corners
            {
                using (GraphicsPath path = GetRoundRectangle(0, m_outerRadius))
                    Region = new Region(path);
            }
            else //squared corners
            {
                Region = null; //no need for a clipping region if rectangular
            }
        }

        private GraphicsPath GetRoundRectangle(int deflateBy, float radius)
        {
            Rectangle bounds = new Rectangle(0, 0, Size.Width, Size.Height);

            if(deflateBy != 0)
                bounds.Inflate(-deflateBy, -deflateBy);

            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.X + bounds.Width - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.X + bounds.Width - radius, bounds.Y + bounds.Height - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Y + bounds.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        public bool MoveOffScreen(bool moveItOff = true)
        {
            bool success = false;

            if (moveItOff) //we should be hidden
            {
                if (!MovedOffScreen) //if we minimize or hide it will terminate ShowDialog so we will move the window off the screen.
                {
                    //visual cue that window is about to move (for debugging)
                    //for (int x = 0; x < 10; x++)
                    //{
                    //    this.DrawAColorBorder = !this.DrawAColorBorder;
                    //    Application.DoEvents();
                    //    System.Threading.Thread.Sleep(100);
                    //}

                    m_locationBeforeHide = Location; //save where we were
                    Location = new Point(0, Screen.PrimaryScreen.Bounds.Height + 100);
                    success = true;
                }
            }
            else //should be visible
            {
                if (MovedOffScreen) //we are off the screen
                {
                    Location = (Point)m_locationBeforeHide; //move the window back where it was
                    m_locationBeforeHide = null; //indicate we are not moved off the screen
                    success = true;
                }
            }

            return success;
        }

        #endregion

        #region Member Properties
        private bool DrawingRounded
        {
            get
            {
                return m_drawRounded && IsAtLeastWindows7;
            }
        }

        public bool MovedOffScreen
        {
            get
            {
                return m_locationBeforeHide != null;
            }
        }

        /// <summary>
        /// Gets or sets whether to draw the gradient background.
        /// </summary>
        [Description("Whether to draw the gradient background.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawAsGradient
        {
            get
            {
                return m_drawGradient;
            }

            set
            {
                m_drawGradient = value;
            
                if(m_drawGradient && m_drawBurst)
                    DrawAsBurst = false;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to draw the form with rounded corners.
        /// </summary>
        [Description("Whether to draw with rounded corners.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawRounded
        {
            get
            {
                return m_drawRounded;
            }

            set
            {
                m_drawRounded = value;

                if (DrawingRounded)
                    FormBorderStyle = FormBorderStyle.None;

                Form_SizeChanged(null, new EventArgs());
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to draw the background as a burst.
        /// </summary>
        [Description("Whether to draw the background as a burst.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawAsBurst
        {
            get
            {
                return m_drawBurst;
            }

            set
            {
                m_drawBurst = value;

                if (m_drawBurst && m_drawGradient)
                    DrawAsGradient = false;
                
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
        /// Gets or sets whether to draw an inner border.
        /// </summary>
        [Description("Whether to draw an inner border.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawBorderInnerEdge
        {
            get
            {
                return m_drawInnerBorder;
            }
            set
            {
                m_drawInnerBorder = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to draw the border edge.
        /// </summary>
        [Description("Whether to draw a border edge.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawBorderOuterEdge
        {
            get
            {
                return m_drawEdge;
            }
            set
            {
                m_drawEdge = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to draw a color border.
        /// </summary>
        [Description("Whether to draw a color border.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawAColorBorder
        {
            get
            {
                return m_drawColorBorder;
            }
            set
            {
                m_drawColorBorder = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to draw a gradient border.
        /// </summary>
        [Description("Whether to draw a gradient border.  Overrides a color border.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool DrawAGradientBorder
        {
            get
            {
                return m_drawReverseGradientBorder;
            }
            set
            {
                m_drawReverseGradientBorder = value;
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

        /// <summary>
        /// Gets or sets the gradient's edge color.
        /// </summary>
        [Description("The background gradient's edge color.")]
        [Category("Appearance")]
        public Color OuterBorderEdgeColor
        {
            get
            {
                return m_edgeColor;
            }
            set
            {
                m_edgeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the inner border color.
        /// </summary>
        [Description("The inner border color.")]
        [Category("Appearance")]
        public Color InnerBorderEdgeColor
        {
            get
            {
                return m_innerBorderColor;
            }
            set
            {
                m_innerBorderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        [Description("The border color between edge and inner border.")]
        [Category("Appearance")]
        public Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                m_borderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the outer radius.
        /// </summary>
        [Description("The radius that determines the outer rounded corner.")]
        [Category("Appearance")]
        [DefaultValue(30)]
        public int OuterRadius
        {
            get
            {
                return m_outerRadius;
            }
            set
            {
                m_outerRadius = value;
                Form_SizeChanged(null, new EventArgs());
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        [Description("The radius that determines the inner border edge's rounded corner.")]
        [Category("Appearance")]
        [DefaultValue(20)]
        public int InnerRadius
        {
            get
            {
                return m_innerRadius;
            }
            set
            {
                m_innerRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets thickness of the border.
        /// </summary>
        [Description("The thickness of the border.")]
        [Category("Appearance")]
        [DefaultValue(6)]
        public int BorderThickness
        {
            get
            {
                return m_borderThickness;
            }
            set
            {
                m_borderThickness = value;
                Invalidate();
            }
        }

        #endregion
    }
}