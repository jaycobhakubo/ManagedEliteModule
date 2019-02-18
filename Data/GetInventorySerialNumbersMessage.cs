// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Get inventory serial numbers
    /// </summary>
    public class GetInventorySerialNumbersMessage : ServerMessage
    {
        #region Member Variables

        private readonly bool m_getAdditionalDetails;

        #endregion

        #region Constructors
        /// <summary>
        /// inventory serial numbers
        /// </summary>
        public GetInventorySerialNumbersMessage(bool getAdditionalDetails = false)
        {
            m_id = 36034;
            m_getAdditionalDetails = getAdditionalDetails;
            InventoryItems = new List<Tuple<string, string>>();
        }

        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            if (m_getAdditionalDetails)
            {
                // Show Inactive Currencies
                requestWriter.Write(m_getAdditionalDetails);
            }
            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);
                
                //count 
                ushort numberOfItems = responseReader.ReadUInt16();

                for (int i = 0; i < numberOfItems; i++)
                {
                    var itemId = 0;
                    var productName = string.Empty;

                    //serial numbers
                    ushort stringLen = responseReader.ReadUInt16();
                    string serialNumber = new string(responseReader.ReadChars(stringLen));
                    
                    if (m_getAdditionalDetails)
                    {
                        //product name
                        stringLen = responseReader.ReadUInt16();
                        productName = new string(responseReader.ReadChars(stringLen));
                    }

                    //create inventory item
                    var item = new Tuple<string, string>(serialNumber, productName);

                    //add to list
                    InventoryItems.Add(item);
                }

            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Inventory Serial Numbers", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Inventory Serial Numbers", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion


        #region Member Properties

        public List<Tuple<string, string>> InventoryItems { get; private set; }

        #endregion
    }
}
