// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 GameTech
// International, Inc.


//US4436: Close a bank from the POS

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared.Data
{
    /// <summary>
    /// Represents a Get Staff Data server message.
    /// </summary>
    public class GetStaffTotalDueMessage : ServerMessage
    {
        #region Constants And Data Types
        protected const int MinResponseMessageLength = 6;
        
        #endregion

        #region Member Variables
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GetStaffDataMessage class
        /// with the specified staff and operator id.
        /// </summary>
        /// <param name="gamingDate"></param>
        /// <param name="session"></param>
        public GetStaffTotalDueMessage(DateTime gamingDate, int session)
        {
            m_id = 37035; // Get Staff Data
            GamingDate = gamingDate;
            Session = session;
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

            // Session Number
            requestWriter.Write(Session);

            //gaming date
            string tempDate = GamingDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            requestWriter.Write((ushort)tempDate.Length);
            requestWriter.Write(tempDate.ToCharArray());

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

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Staff Total Due Message");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                TotalDue = ReadDecimal(responseReader) ?? 0.0m;

                TotalPaperDue = ReadDecimal(responseReader) ?? 0.0m;
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Staff Total Due Message", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Staff Total Due Message", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        /// <summary>
        /// Gets the total due.
        /// </summary>
        /// <param name="gamingDate">The gaming date.</param>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        /// <exception cref="Exception">GetStaffTotalDueMessage:  + ex.Message</exception>
        public static GetStaffTotalDueMessage GetTotalDue(DateTime gamingDate, int session)
        {
            var message = new GetStaffTotalDueMessage(gamingDate, session);

            try
            {
                message.Send();
            }
            catch (Exception ex)
            {
                throw new Exception("GetStaffTotalDueMessage: " + ex.Message);
            }

            return message;
        }
        #endregion
        
        #region Member Properties

        /// <summary>
        /// Gets the gaming date.
        /// </summary>
        /// <value>
        /// The gaming date.
        /// </value>
        public DateTime GamingDate { get; private set; }

        /// <summary>
        /// Gets the session number.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public int Session { get; private set; }

        /// <summary>
        /// Gets the staffs total due.
        /// </summary>
        /// <value>
        /// The total due.
        /// </value>
        public decimal TotalDue { get; private set; }

        /// <summary>
        /// The total amount of paper sales due. Part of the TotalDue
        /// </summary>
        public decimal TotalPaperDue { get; private set; }
        #endregion
    }
}
