// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013 FortuNet


using System;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class SetCharityData : ServerMessage
    {
        #region Member Properties
        public Charity properties = new Charity();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Set Charity Data message
        /// </summary>
        SetCharityData()
        {
        }

        public SetCharityData(Charity charity)
        {
            m_id = 18198; // Set Charity Data Message
            properties = charity;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams that will be written to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write((int)properties.CharityId);
            requestWriter.Write((ushort)properties.Address1.Length);
            requestWriter.Write(properties.Address1.ToCharArray());
            requestWriter.Write((ushort)properties.Address2.Length);
            requestWriter.Write(properties.Address2.ToCharArray());
            requestWriter.Write((ushort)properties.City.Length);
            requestWriter.Write(properties.City.ToCharArray());
            requestWriter.Write((ushort)properties.State.Length);
            requestWriter.Write(properties.State.ToCharArray());
            requestWriter.Write((ushort)properties.PostalCode.Length);
            requestWriter.Write(properties.PostalCode.ToCharArray());
            requestWriter.Write((ushort)properties.Country.Length);
            requestWriter.Write(properties.Country.ToCharArray());
            requestWriter.Write((ushort)properties.Name.Length);
            requestWriter.Write(properties.Name.ToCharArray());
            requestWriter.Write((ushort)properties.License.Length);
            requestWriter.Write(properties.License.ToCharArray());
            requestWriter.Write((ushort)properties.TaxId.Length);
            requestWriter.Write(properties.TaxId.ToCharArray());
            requestWriter.Write((ushort)properties.Contact.Length);
            requestWriter.Write(properties.Contact.ToCharArray());
            requestWriter.Write((ushort)properties.Phone.Length);
            requestWriter.Write(properties.Phone.ToCharArray());
            requestWriter.Write((bool)properties.Active);

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
                properties.CharityId = responseReader.ReadInt32();

            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Set Charity Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Set Charity Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        #endregion
    }
}
