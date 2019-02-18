// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2018 Fortunet

using System;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get CBB Info From Transaction server message.
    /// </summary>
    public class GetCBBInfoFromTransactionMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_transNum;
        protected int m_receiptID = 0;
        protected bool m_hasPaper = false;
        protected bool m_hasElectronic = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetCBBInfoFromTransactionMessage class 
        /// with the specified parameters.
        /// </summary>
        /// <param name="gamingDate">The gaming date criterion for the receipts
        /// to find.</param>
        public GetCBBInfoFromTransactionMessage(int transactionNumber)
        {
            m_id = 18259; // Find Receipt Data
            m_transNum = transactionNumber;
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

            // Transaction No
            requestWriter.Write(m_transNum);
            
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
            // Clear the return values.
            m_receiptID = 0;
            m_hasPaper = false;
            m_hasElectronic = false;

            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get CBB Info From Transaction");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                m_receiptID = responseReader.ReadInt32();
                m_hasPaper = responseReader.ReadBoolean();
                m_hasElectronic = responseReader.ReadBoolean();
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get CBB Info From Transaction", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get CBB Info From Transaction", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties

        /// <summary>
        /// Gets the found receipt ID.
        /// The ID will only be found if the transaction number is for the current gaming date
        /// and has at least one crystal ball item.
        /// </summary>
        public int ReceiptID
        {
            get
            {
                return m_receiptID;
            }
        }

        public bool HasPaper
        {
            get
            {
                return m_hasPaper;
            }
        }

        public bool HasElectronic
        {
            get
            {
                return m_hasElectronic;
            }
        }

        #endregion
    }
}
