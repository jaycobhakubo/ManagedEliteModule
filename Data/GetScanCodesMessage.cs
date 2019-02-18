// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2017 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Scan Codes Message
    /// </summary>
    public class GetScanCodesMessage : ServerMessage
    {
        protected const int MinResponseMessageLength = 6;
        protected List<string> m_scanCodes = new List<string>();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetScanCodesMessage class.
        /// </summary>
        public GetScanCodesMessage()
            : this(true, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetScanCodesMessage class.
        /// </summary>
        /// <param name="isPackage"></param>
        /// <param name="itemId"></param>
        public GetScanCodesMessage(bool isPackage, int itemId)
        {
            m_id = 18248; // Get Scan Codes
            ItemIsPackage = isPackage;
            ItemID = itemId;

            m_scanCodes.Clear();
        }
        #endregion

        #region Member Methods
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write(ItemIsPackage ? 1 : 0);
            requestWriter.Write(ItemID);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            var responseStream = new MemoryStream(m_responsePayload);
            var responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Scan Codes");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                int codes = responseReader.ReadUInt16();
                UInt16 stringLength = 0;

                for(int x = 0; x < codes; x++)
                {
                    stringLength = responseReader.ReadUInt16();

                    if(stringLength > 0)
                        m_scanCodes.Add(new string(responseReader.ReadChars(stringLength)));
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Scan Codes", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Scan Codes", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        
        /// <summary>
        /// Gets or sets if the item is a package.
        /// </summary>
        public bool ItemIsPackage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if the item is a product.
        /// </summary>
        public bool ItemIsProduct
        {
            get
            {
                return !ItemIsPackage;
            }

            set
            {
                ItemIsPackage = !value;
            }
        }

        /// <summary>
        /// Gets or Sets the Item ID.
        /// </summary>
        public int ItemID
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the list of scan codes.
        /// </summary>
        public List<string> ScanCodes
        {
            get
            {
                return m_scanCodes;
            }
        }

        #endregion
    }
}
