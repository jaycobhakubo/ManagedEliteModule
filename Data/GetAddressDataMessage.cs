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
    /// Represents a Get Address Data server message.
    /// </summary>
    public class GetAddressDataMessage : ServerMessage 
    {
        #region Member Variables
        public Address Properties = new Address();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetAddressDataMessage class.
        /// </summary>
        public GetAddressDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetAddressDataMessage class
        /// with the specified address id.
        /// </summary>
        /// <param name="addressId">The id of the address to get 
        /// data for.</param>
        public GetAddressDataMessage(int addressId)
        {
            m_id = 18050; // Get Address Data
            Properties.AddressID = addressId;
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

            // Address Id
            requestWriter.Write(Properties.AddressID);

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

                // Address1
                ushort stringLen = responseReader.ReadUInt16();
                Properties.Address1 = new string(responseReader.ReadChars(stringLen));

                // Address1
                stringLen = responseReader.ReadUInt16();
                Properties.Address2  = new string(responseReader.ReadChars(stringLen));

                // City
                stringLen = responseReader.ReadUInt16();
                Properties.City  = new string(responseReader.ReadChars(stringLen));

                // State
                stringLen = responseReader.ReadUInt16();
                Properties.State   = new string(responseReader.ReadChars(stringLen));

                // Zip
                stringLen = responseReader.ReadUInt16();
                Properties.Zipcode   = new string(responseReader.ReadChars(stringLen));

                // Country
                stringLen = responseReader.ReadUInt16();
                Properties.Country   = new string(responseReader.ReadChars(stringLen));

            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Address Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Address Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion
    }
}
