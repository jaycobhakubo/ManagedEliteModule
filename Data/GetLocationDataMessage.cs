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
    public class GetLocationDataMessage : ServerMessage 
    {
        #region Member Variables
        public Location[] Items = new Location[0];
        private int m_locationId = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetLocationDataMessage class.
        /// </summary>
        public GetLocationDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetLocationDataMessage class
        /// with the specified location id.
        /// </summary>
        /// <param name="locationId">The id of the location to get 
        /// data for (or 0 for all locations).</param>
        public GetLocationDataMessage(int locationId)
        {
            m_id = 18055; // Get Location Data
            m_locationId = locationId;
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

            // Location Id
            requestWriter.Write(m_locationId);

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
                UInt16 count = responseReader.ReadUInt16();
                Items = new Location[count];
                for (int i = 0; i < count; i++)
                {
                    Location tmp = new Location();
                    tmp.LocationID = responseReader.ReadInt32();
                    tmp.SubLocationID = responseReader.ReadInt32();
                    tmp.CompanyID = responseReader.ReadInt32();
                    tmp.AddressID = responseReader.ReadInt32();
                    tmp.Active = (responseReader.ReadByte() == 1) ? true : false;

                    ushort stringLen = responseReader.ReadUInt16();
                    if (stringLen >0)
                        tmp.Name = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Phone = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.Modem = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                    tmp.RoomName = new string(responseReader.ReadChars(stringLen));

                    Items.SetValue(tmp, i);
                }
                //tmp = null;
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Location Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Location Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion
    }
}
