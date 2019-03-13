// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013 FortuNet, Inc.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace GTI.Modules.Shared
{

    public enum BarcodeType
    {
        UnknownBarcodeType = 0,
        PaperScanCode = 1,
        ProductScanCode = 2
    }

    /// <summary>
    /// Represents the method that handles the BarcodeScan event
    /// of the BarcodeReader class.
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The string that contains the event data.</param>
    public delegate void BarcodeScanHandler(object sender, string e);
    
    /// <summary>
    /// This class parses and signals the presence of barcode data from 
    /// the keyboard and, potentially, other sources.
    /// </summary>
    public class BarcodeReader
    {
        #region Member Variables
        protected object readingSync = new object();
        protected StringBuilder codeData = new StringBuilder();
        protected ISynchronizeInvoke m_syncObject;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a barcode is scanned
        /// </summary>
        public event BarcodeScanHandler BarcodeScanned;
        #endregion

        #region Constructor
        public BarcodeReader()
        {
        }
        #endregion

        #region Member Methods
        public void ProcessCharacter(char character)
        {
            if (character == '\r')
                ProcessCmdKey(Keys.Enter);
            else
                codeData.Append(character);
        }

        public void ProcessCmdKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (codeData.Length > 0)
                {
                    string barcode = codeData.ToString();
                    Reset();
                    OnBarcodeScanned(this, barcode);
                }
            }
        }

        public void Reset()
        {
            codeData.Clear();
        }

        /// <summary>
        /// Raises the BarcodeScan event
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="data">The data read from the barcode scanner</param>
        protected virtual void OnBarcodeScanned(object sender, string data)
        {
            BarcodeScanHandler handler = BarcodeScanned;
            if (handler != null && data.Length > 0)
            {
                if (m_syncObject == null || !m_syncObject.InvokeRequired)
                    handler(sender, new string(data.ToCharArray()));
                else
                    m_syncObject.Invoke(handler, new object[] { sender, new string(data.ToCharArray()) });
            }
        }

        #endregion

        #region Member Properties
        public string Barcode
        {
            get
            {
                return codeData.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that 
        /// are issued when a barcode has been scanned.
        /// </summary>
        /// <remarks>
        /// When SynchronizingObject is null, the method that handles the 
        /// BarcodeScanned event might be called on a background thread. When the 
        /// BarcodeScanned event is handled by a visual Windows Forms component, 
        /// such as a button, accessing the component on another thread might 
        /// result in an exception or just might not work. Avoid this effect by 
        /// setting SynchronizingObject to a Windows Forms component, which 
        /// causes the method that handles the BarcodeScanned event to be called on 
        /// the same thread that the component was created on.
        /// </remarks>
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return m_syncObject;
            }

            set
            {
                m_syncObject = value;

            }
        }

        #endregion
    }
}
