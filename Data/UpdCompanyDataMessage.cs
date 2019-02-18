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
    public class UpdCompanyDataMessage : ServerMessage
    {
        #region "Member Properties"

        public Company Properties = new Company();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class.
        /// </summary>
        public UpdCompanyDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class
        /// with the specified operator id.
        /// </summary>
        /// <param name="operatorId">The id of the operator to get 
        /// data for.</param>
        public UpdCompanyDataMessage(int companyID)
        {
            m_id = 18049; // Update Company Data
            Properties.CompanyID = companyID;
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
            requestWriter.Write(Properties.SubCompanyID);
            requestWriter.Write(Properties.AddressID);
            requestWriter.Write((ushort)Properties.Name.Length);
            requestWriter.Write(Properties.Name.ToCharArray());
            requestWriter.Write((ushort)Properties.Phone.Length);
            requestWriter.Write(Properties.Phone.ToCharArray());
            requestWriter.Write((ushort)Properties.Owner.Length);
            requestWriter.Write(Properties.Owner.ToCharArray());
            requestWriter.Write((byte)(Properties.Active? 1:0));

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
                // Seek past return code
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Company ID
                Properties.CompanyID = responseReader.ReadInt16();

            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Upd Company Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Upd Company Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }
    
}
