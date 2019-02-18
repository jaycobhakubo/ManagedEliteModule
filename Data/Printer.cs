// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a printer attached to the GameTech Elite system.
    /// </summary>
    /// <remarks>This class is only capable of printing text.</remarks>
    public class Printer
    {
        #region Constants and Data Types
        protected const int PaperRoll58mmWidth = 228;

        /// <summary>
        /// Represents one line on a piece of paper.
        /// </summary>
        protected class LineEntry
        {
            public string Text;
            public StringAlignment Alignment;
            public Font Font;
            public float Height;
            public bool IsPageBreak;
        }
        #endregion

        #region Member Variables
        protected string m_printerName;
        protected bool m_is58mm;
        protected PrintDocument m_printDoc;
        protected Font m_defaultFont;
        protected ArrayList m_lines;
        protected int m_currentLine;
        protected int m_maxLine;
        protected byte[] m_openDrawerCode;
        #endregion

        #region Construtors
        /// <summary>
        /// Initializes a new instance of the Printer class.
        /// </summary>
        public Printer()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Printer class with
        /// the specified printer.
        /// </summary>
        /// <param name="printerName">The name of the printer to use.
        /// If the string is empty, the default printer is used.</param>
        public Printer(string printerName)
        {
            // If a name is specified, use that, else use the default printer.
            if(!string.IsNullOrEmpty(printerName))
            {
                PrinterSettings printSettings = new PrinterSettings();
                printSettings.PrinterName = printerName;

                if(!printSettings.IsValid)
                    throw new InvalidPrinterException(printSettings);

                m_printerName = printerName;
                m_printDoc = new PrintDocument();
                m_printDoc.PrinterSettings = printSettings;
            }
            else
            {
                m_printDoc = new PrintDocument();

                if(!m_printDoc.PrinterSettings.IsValid)
                    throw new InvalidPrinterException(m_printDoc.PrinterSettings);

                m_printerName = m_printDoc.PrinterSettings.PrinterName;
            }

            // Change the document name & suppress the status dialog controller.
            m_printDoc.DocumentName = "GameTech Elite Document";
            m_printDoc.PrintController = new StandardPrintController();
            m_printDoc.BeginPrint += new PrintEventHandler(OnBeginPrint);
            m_printDoc.PrintPage += new PrintPageEventHandler(OnPrintPage);

            // Create the ArrayList to hold the print lines.
            m_lines = new ArrayList();

            // Create a default font.
            m_defaultFont = new Font("Microsoft Sans Serif", 8F);

            // Check to see if we are using 58mm width paper.
            // PDTS 584 - Portable POS Support
            m_is58mm = (m_printDoc.DefaultPageSettings.PaperSize.Width <= PaperRoll58mmWidth);
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Adds a line to be printed to the printer.  Lines are printed
        /// in the order that they are added.
        /// </summary>
        /// <param name="line">The string of text to be printed.</param>
        /// <param name="align">The alignment of the text to be 
        /// printed.</param>
        /// <param name="font">The font to use for printing this line.</param>
        /// <returns>The line number of the text.</returns>
        public int AddLine(string line, StringAlignment align, Font font)
        {
            LineEntry entry = new LineEntry();
            entry.Text = line;
            entry.Alignment = align;

            if(font != null)
                entry.Font = font;
            else
                entry.Font = m_defaultFont;
           

            entry.Height = 0F;
            entry.IsPageBreak = false;

            return m_lines.Add(entry);
        }

        /// <summary>
        /// Adds a page break to the lines to be printed.
        /// </summary>
        /// <returns>The line number of the page break.</returns>
        public int AddPageBreak()
        {
            LineEntry entry = new LineEntry();

            entry.IsPageBreak = true;

            return m_lines.Add(entry);
        }

        /// <summary>
        /// Removes a line of text to be printed.
        /// </summary>
        /// <param name="index">The line number to remove.</param>
        public void RemoveLine(int index)
        {
            m_lines.RemoveAt(index);
        }

        /// <summary>
        /// Removes all lines that are to be printed.
        /// </summary>
        public void ClearLines()
        {
            m_lines.Clear();
        }

        /// <summary>
        /// Returns the size of a string if it were printed with the specified 
        /// font and alignment.
        /// </summary>
        /// <param name="testString">The string to measure.</param>
        /// <param name="font">The font the string will be printed in.</param>
        /// <param name="alignment">The alignment the string will be printed 
        /// with.</param>
        /// <returns>A SizeF structure that represents the size of the string.
        /// </returns>
        public SizeF MeasureString(string testString, Font font, StringAlignment alignment)
        {
            // Create the graphics object.
            Graphics g = m_printDoc.PrinterSettings.CreateMeasurementGraphics();

            // Setup the StringFormat.
            StringFormat format = new StringFormat();
            format.Trimming = StringTrimming.EllipsisCharacter;
            format.FormatFlags |= StringFormatFlags.NoWrap;
            format.Alignment = alignment;

            // Get the size of the string.
            return g.MeasureString(testString, font, 0, format);
        }

        /// <summary>
        /// Returns the line spacing, in the current unit of the printer, of 
        /// the passed in font.
        /// </summary>
        /// <param name="font">The font to measure.</param>
        /// <returns>The line spacing, in the current unit of the printer.
        /// </returns>
        public float GetFontHeight(Font font)
        {
            // Create the graphics object.
            Graphics g = m_printDoc.PrinterSettings.CreateMeasurementGraphics();
            float height = font.GetHeight(g);
            g.Dispose();

            return height;
        }

        /// <summary>
        /// Estimates the number of pages that are required to print the 
        /// current document.
        /// </summary>
        /// <returns>The number of pages.</returns>
        public int CalculatePages()
        {
            int pages = 0;

            if(m_lines.Count > 0)
            {
                // Create the graphics object.
                Graphics g = m_printDoc.PrinterSettings.CreateMeasurementGraphics();

                // Setup sizes.
                pages = 1;
                int currLine = 0;
                float pageHeight = m_printDoc.DefaultPageSettings.PaperSize.Height - (m_printDoc.DefaultPageSettings.Margins.Top + m_printDoc.DefaultPageSettings.Margins.Bottom);
                float currHeight = 0;

                // Loop through every line we have so far.
                while(currLine < m_lines.Count)
                {
                    LineEntry entry = (LineEntry)m_lines[currLine];

                    if(!entry.IsPageBreak)
                    {
                        // How tall is this line?
                        float lineHeight = entry.Font.GetHeight(g);
                        currHeight += lineHeight;

                        if(currHeight > pageHeight) // We've gone over a page.
                        {
                            pages++;
                            currHeight = 0;
                        }
                        else if(lineHeight > pageHeight)
                            break; // We can't fit this line on page, error.
                        else
                            currLine++; // Move on to the next page.
                    }
                    else // It's a page break, so skip to the next page.
                    {
                        pages++;
                        currLine++;
                        currHeight = 0;
                    }
                }

                g.Dispose();
            }

            return pages;
        }

        /// <summary>
        /// Sends the lines of text to the printer.
        /// </summary>
        public void Print()
        {
            if(m_printDoc != null)
                m_printDoc.Print();
        }

        /// <summary>
        /// Sets the printer's document to the specified PrintPreviewControl.
        /// </summary>
        /// <param name="previewControl">The control the will be previewing the
        /// printer's document.</param>
        public void SetPrintPreview(PrintPreviewControl previewControl)
        {
            previewControl.Document = m_printDoc;
        }

        /// <summary>
        /// Handles the printer's BeginPrint event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An PrintEventArgs object that contains 
        /// the event data.</param>
        void OnBeginPrint(object sender, PrintEventArgs e)
        {
            // Reset which line we are printing and the max line
            // we can print on this page.
            m_currentLine = 0;
            m_maxLine = 0;
        }

        /// <summary>
        /// Handles the printer's PrintPage event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An PrintPageEventArgs object that contains 
        /// the event data.</param>
        void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            // First check to see if there are any pages to print.
            if(m_lines.Count > 0)
            {
                // Calculate the printing rectangle.
                RectangleF fullRect;

                if(e.Graphics.VisibleClipBounds.X < 0) // Print Preview
                    fullRect = e.MarginBounds;
                else
                {
                    fullRect = new RectangleF(
                        e.MarginBounds.Left - (e.PageBounds.Width - e.Graphics.VisibleClipBounds.Width) / 2,
                        e.MarginBounds.Top - (e.PageBounds.Height - e.Graphics.VisibleClipBounds.Height) / 2,
                        e.MarginBounds.Width, e.MarginBounds.Height);
                }

                // Calculate how many lines we can fit on this page.
                float totalHeight = 0.0F;

                for(int x = m_currentLine; x < m_lines.Count; x++)
                {                   
                    LineEntry entry = ((LineEntry)m_lines[x]);

                    if(!entry.IsPageBreak)
                    {
                        entry.Height = entry.Font.GetHeight(e.Graphics);
                        totalHeight += entry.Height;

                        if(totalHeight < fullRect.Height)
                            m_maxLine = x;
                        else // We've gone over the allowed height.
                            break;
                    }
                    else // We've hit a page break.
                    {
                        m_maxLine = x;
                        break;
                    }
                }

                // Set up the string formatting.
                StringFormat format = new StringFormat();
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags |= StringFormatFlags.NoWrap;

                SolidBrush blackBrush = new SolidBrush(Color.Black);
                totalHeight = fullRect.Top;

                while(m_currentLine <= m_maxLine)
                {
                    LineEntry entry = (LineEntry)m_lines[m_currentLine];

                    if(!entry.IsPageBreak)
                    {
                        // Draw the string to the printer.
                        format.Alignment = entry.Alignment;
                        RectangleF drawingRect = new RectangleF(fullRect.Left, totalHeight, fullRect.Width, entry.Height);
                        e.Graphics.DrawString(entry.Text, entry.Font, blackBrush, drawingRect, format);
                        totalHeight += entry.Height;
                    }

                    // Move to the next line.
                    m_currentLine++;
                }

                // Check to see if we need to print on the next page.
                if(m_currentLine < m_lines.Count)
                    e.HasMorePages = true;
            }
        }

        /// <summary>
        /// Sends the drawer code bytes to the printer.
        /// </summary>C
        public void OpenDrawer()
        {
            if(m_openDrawerCode != null && m_openDrawerCode.Length != 0)
            {
                // First alloc some unmanaged bytes so they don't get moved 
                // around on the RawPrinterHelper.
                IntPtr pUnmanagedBytes = new IntPtr(0);

                pUnmanagedBytes = Marshal.AllocCoTaskMem(m_openDrawerCode.Length);
                Marshal.Copy(m_openDrawerCode, 0, pUnmanagedBytes, m_openDrawerCode.Length);

                // Send the code.
                RawPrinterHelper.SendBytesToPrinter(m_printDoc.PrinterSettings.PrinterName, pUnmanagedBytes, m_openDrawerCode.Length);

                // Free the unmanaged memory.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets whether the default paper size for the printer is a 
        /// 58mm paper roll.
        /// </summary>
        public bool Using58mmPaper
        {
            get
            {
                return m_is58mm;
            }
        }

        /// <summary>
        /// Gets the x-coordinate, in hundredths of an inch, of the hard
        /// margin at the left of the page.
        /// </summary>
        public float HardMarginX
        {
            get
            {
                return m_printDoc.DefaultPageSettings.HardMarginX;
            }
        }

        /// <summary>
        /// Gets the y-coordinate, in hundredths of an inch, of the hard
        /// margin at the top of the page.
        /// </summary>
        public float HardMarginY
        {
            get
            {
                return m_printDoc.DefaultPageSettings.HardMarginY;
            }
        }

        /// <summary>
        /// Gets the bounds of the printable area of the page for the printer.
        /// This property will correctly account for landscape mode.
        /// </summary>
        public RectangleF PrintableArea
        {
            get
            {
                if(!m_printDoc.DefaultPageSettings.Landscape)
                    return m_printDoc.DefaultPageSettings.PrintableArea;
                else
                {
                    // We have to adjust for landscape since .NET won't do it 
                    // for us.
                    RectangleF returnVal = new RectangleF();

                    returnVal.X = m_printDoc.DefaultPageSettings.PrintableArea.Y;
                    returnVal.Y = m_printDoc.DefaultPageSettings.PrintableArea.X;
                    returnVal.Width = m_printDoc.DefaultPageSettings.PrintableArea.Height;
                    returnVal.Height = m_printDoc.DefaultPageSettings.PrintableArea.Width;

                    return returnVal;
                }
            }
        }

        /// <summary>
        /// Gets or sets the default margins for this printer.
        /// </summary>
        public Margins Margins
        {
            get
            {
                return m_printDoc.DefaultPageSettings.Margins;
            }
            set
            {
                m_printDoc.DefaultPageSettings.Margins = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of copies to print.
        /// </summary>
        public short Copies
        {
            get
            {
                return m_printDoc.PrinterSettings.Copies;
            }
            set
            {
                m_printDoc.PrinterSettings.Copies = value;
            }
        }

        /// <summary>
        /// Gets or sets the default font for this printer.
        /// </summary>
        public Font DefaultFont
        {
            get
            {
                return m_defaultFont;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("DefaultFont");
                else
                    m_defaultFont = value;
            }
        }

        /// <summary>
        /// The sequence of bytes used to send a cash drawer kick signal.
        /// </summary>
        public byte[] OpenDrawerCode
        {
            get
            {
                return m_openDrawerCode;
            }
            set
            {
                m_openDrawerCode = value;
            }
        }
        #endregion
    }
}
