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
    /// Represents a Get Company Data server message.
    /// </summary>
    public class GetCompanyDataMessage : ServerMessage
    {
        #region Member Variables
        public Company Properties = new Company();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetCompanyDataMessage class.
        /// </summary>
        public GetCompanyDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetCompanyDataMessage class
        /// with the specified company id.
        /// </summary>
        /// <param name="companyId">The id of the company to get 
        /// data for.</param>
        public GetCompanyDataMessage(int companyId)
        {
            m_id = 18047; // Get Company Data
            Properties.CompanyID = companyId;
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

            // Company Id
            requestWriter.Write(Properties.CompanyID);

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

                // Sub Company ID
                Properties.SubCompanyID = responseReader.ReadInt32();

                // AddressID
                Properties.AddressID = responseReader.ReadInt32();

                // Company Name
                ushort stringLen = responseReader.ReadUInt16();
                Properties.Name = new string(responseReader.ReadChars(stringLen));

                // Phone
                stringLen = responseReader.ReadUInt16();
                Properties.Phone = new string(responseReader.ReadChars(stringLen));

                // Owner
                stringLen = responseReader.ReadUInt16();
                Properties.Owner = new string(responseReader.ReadChars(stringLen));

                // Active
                Properties.Active = responseReader.ReadBoolean();

            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Company Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Company Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion
    }
}
