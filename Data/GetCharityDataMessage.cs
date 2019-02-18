// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013 FortuNet

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class GetCharityDataMessage : ServerMessage
    {
        #region Member Variables
        public Charity[] items = new Charity[0];
        private int charityId = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetCharityDataMessage class.
        /// </summary>
        GetCharityDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetLocationDataMessage class
        /// with the specified location id.
        /// </summary>
        /// <param name="locationId">The id of the location to get 
        /// data for (or 0 for all locations).</param>
        public GetCharityDataMessage(int charityId)
        {
            m_id = 18199; // Get Charity Data
            this.charityId = charityId;
        }
        #endregion

        #region Member Methods
        public static List<Charity> GetList()
        {
            GetCharityDataMessage msg = new GetCharityDataMessage(0);
            try
            {
                msg.Send();
            }
            catch (Exception)
            {
                return null;
            }

            List<Charity> charities = new List<Charity>();
            int count = msg.items.Length;
            for (int n = 0; n < count; ++n)
            {
                charities.Add(msg.items[n]);
            }

            return charities;
        }

        protected override void PackRequest()
        {
            // Create the streams that will be written to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Charity Id
            requestWriter.Write((int)charityId);

            // Set the bytes to be sent
            m_requestPayload = requestStream.ToArray();

            // close the streams
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                ushort count = responseReader.ReadUInt16();
                items = new Charity[count];
                for (int i = 0; i < count; i++)
                {
                    Charity tmp = new Charity();
                    tmp.CharityId = responseReader.ReadInt32();

                    ushort stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Address1 = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Address2 = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.City = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.State = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.PostalCode = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Country = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Name = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.License = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.TaxId = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Contact = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Phone = new string(responseReader.ReadChars(stringLen));

                    tmp.Active = (responseReader.ReadByte() == 1) ? true : false;

                    items.SetValue(tmp, i);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Charity Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Charity Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        #endregion
    }
}
