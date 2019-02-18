// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013-2016 FortuNet


using System;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class SetChannelData : ServerMessage
    {
        #region Member Properties
        public Channel m_channel = new Channel();
        public ushort length = 1;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Set Charity Data message
        /// </summary>
        SetChannelData()
        {
        }

        public SetChannelData(Channel channel)
        {
            m_id = 18223; // Modify Channel Data Message
            m_channel = channel;
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

            requestWriter.Write((ushort)length);

            requestWriter.Write((int)m_channel.ChannelID);
            requestWriter.Write((ushort)m_channel.ChannelName.Length);
            requestWriter.Write(m_channel.ChannelName.ToCharArray());
            requestWriter.Write((bool)m_channel.Enabled);

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
        }

        #endregion
    }

}