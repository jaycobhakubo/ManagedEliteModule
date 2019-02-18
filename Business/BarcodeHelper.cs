// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using IDAutomation.NetAssembly;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// This helper class facilitates the use of barcodes.
    /// </summary>
    public class BarcodeHelper
    {
        #region Constants and Data Types
        protected const string IDAutomationLargeFont = "IDAutomationC128L";
        protected const string IDAutomationMediumFont = "IDAutomationC128M";
        protected const string IDAutomationSmallFont = "IDAutomationC128S";
        protected const string IDAutomationXLargeFont = "IDAutomationC128XL";
        protected const string IDAutomationXSmallFont = "IDAutomationC128XS";
        protected const string IDAutomationXXLargeFont = "IDAutomationC128XXL";
        #endregion

        #region Member Variables
        protected FontEncoder m_fontEncoder = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BarcodeHelper class.
        /// </summary>
        public BarcodeHelper()
        {
            m_fontEncoder = new FontEncoder();
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Encodes a string to be printed in a code 128 barcode format.
        /// </summary>
        /// <param name="data">The string to encode.</param>
        /// <returns>The encoding string.</returns>
        public string EncodeToCode128(string data)
        {
            try
            {
                return m_fontEncoder.Code128(data, 0, false);
            }
            catch(Exception e)
            {
                throw new ModuleException("Error encoding Code 128 barcode.", e);
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets the name of the medium-sized barcode font.
        /// </summary>
        public static string MediumFontName
        {
            get
            {
                return IDAutomationMediumFont;
            }
        }

        /// <summary>
        /// Gets the name of the small-sized barcode font.
        /// </summary>
        public static string SmallFontName
        {
            get
            {
                return IDAutomationSmallFont;
            }
        }

        /// <summary>
        /// Gets the name of the large-sized barcode font.
        /// </summary>
        public static string LargeFontName
        {
            get
            {
                return IDAutomationLargeFont;
            }
        }

        /// <summary>
        /// Gets the name of the extra large-sized barcode font.
        /// </summary>
        public static string XLargeFontName
        {
            get
            {
                return IDAutomationXLargeFont;
            }
        }

        /// <summary>
        /// Gets the name of the extra small-sized barcode font.
        /// </summary>
        public static string XSmallFontName
        {
            get
            {
                return IDAutomationXSmallFont;
            }
        }

        /// <summary>
        /// Gets the name of the extra, extra large-sized barcode font.
        /// </summary>
        public static string XXLargeFontName
        {
            get
            {
                return IDAutomationXXLargeFont;
            }
        }
        #endregion
    }
}
