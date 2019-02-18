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
    public class UpdAddressData :ServerMessage 
    {
                #region "Member Properties"

        public Address Properties = new Address();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class.
        /// </summary>
        public UpdAddressData()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetOperatorDataMessage class
        /// with the specified operator id.
        /// </summary>
        /// <param name="operatorId">The id of the operator to get 
        /// data for.</param>
        public UpdAddressData(int ID)
        {
            m_id = 18051; // Update address Data
            Properties.AddressID = ID;
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

            requestWriter.Write((int)Properties.AddressID);
            requestWriter.Write((ushort)Properties.Address1.Length);
            requestWriter.Write(Properties.Address1.ToCharArray());
            requestWriter.Write((ushort)Properties.Address2.Length);
            requestWriter.Write(Properties.Address2.ToCharArray());
            requestWriter.Write((ushort)Properties.City.Length);
            requestWriter.Write(Properties.City.ToCharArray());
            requestWriter.Write((ushort)Properties.State.Length);
            requestWriter.Write(Properties.State.ToCharArray());
            requestWriter.Write((ushort)Properties.Zipcode.Length);
            requestWriter.Write(Properties.Zipcode.ToCharArray());
            requestWriter.Write((ushort)Properties.Country.Length);
            requestWriter.Write(Properties.Country.ToCharArray());

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
                Properties.AddressID = responseReader.ReadInt32();

            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Upd Address Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Upd Address Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }
}
