// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Current Gaming Date server message.
    /// </summary>
    public class GetGamingDateMessage : ServerMessage
    {
        #region Member Variables
        protected int m_operatorId;
        protected DateTime m_gamingDate;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetGamingDateMessage class.
        /// </summary>
        /// <param name="operatorId">The id of the operator to get the gaming 
        /// date for.</param>
        public GetGamingDateMessage(int operatorId)
        {
            m_id = 18017; // Get Current Gaming Date
            m_operatorId = operatorId;
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

            // Operator Id
            requestWriter.Write(m_operatorId);

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

                // Length of the date.
                ushort stringLen = responseReader.ReadUInt16();

                // Gaming Date
                string tempDate = new string(responseReader.ReadChars(stringLen));
                if (!string.IsNullOrEmpty(tempDate))
                {
                    m_gamingDate = DateTime.Parse(tempDate, CultureInfo.InvariantCulture);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Current Gaming Date", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Current Gaming Date", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the operator to get the gaming date for.
        /// </summary>
        public int OperatorId
        {
            get
            {
                return m_operatorId;
            }
            set
            {
                m_operatorId = value;
            }
        }

        /// <summary>
        /// Gets the gaming date received from the server.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
        }
        #endregion
    }
}
