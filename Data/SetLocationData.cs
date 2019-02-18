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
    public class SetLocationData: ServerMessage
    {
        private Location mSetLocation;
        #region Constructors
        public SetLocationData() : this(null) { }
        public  SetLocationData(Location setLocation)
        {
            m_id = 18056;
            mSetLocation = setLocation;
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
            requestWriter.Write(mSetLocation.LocationID);
            requestWriter.Write(mSetLocation.SubLocationID);
            requestWriter.Write(mSetLocation.CompanyID);
            requestWriter.Write(mSetLocation.AddressID);
            requestWriter.Write((byte)(mSetLocation.Active? 1:0));
            requestWriter.Write((UInt16)mSetLocation.Name.Length);
            if (mSetLocation.Name != null && mSetLocation.Name.Length > 0)
            {
                requestWriter.Write(mSetLocation.Name.ToCharArray());
            }
            requestWriter.Write((UInt16)mSetLocation.Phone.Length);
            if (mSetLocation.Phone != null && mSetLocation.Phone.Length > 0)
            {
                requestWriter.Write(mSetLocation.Phone.ToCharArray());
            }
            requestWriter.Write((UInt16)mSetLocation.Modem.Length);
            if (mSetLocation.Modem != null && mSetLocation.Modem.Length > 0)
            {
                requestWriter.Write(mSetLocation.Modem.ToCharArray());
            }
            requestWriter.Write((UInt16)mSetLocation.RoomName.Length);
            if (mSetLocation.RoomName != null && mSetLocation.RoomName.Length > 0)
            {
                requestWriter.Write(mSetLocation.RoomName.ToCharArray());
            }
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
            // Seek past return code.
            responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);
            int locationID = responseReader.ReadInt32();
            if ((locationID < 0) ||(mSetLocation.LocationID > 0 && mSetLocation.LocationID != locationID))
            {
                throw new ServerException("Location ID is wrong");
            }
            else 
            {
                mSetLocation.LocationID = locationID;
            }
            // Close the streams.
            responseReader.Close();
        }

        public Location LocationGot
        {
            get { return mSetLocation; }
            set { mSetLocation = value; }
        }
        #endregion
    }
}
