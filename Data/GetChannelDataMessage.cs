// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013-2016 FortuNet

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class GetChannelDataMessage : ServerMessage
    {
        #region Member Variables
        public Channel[] items = new Channel[0];
        private int channelId = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetChannelDataMessage class.
        /// </summary>
        GetChannelDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes new instance of GetChannelDataMessage class
        /// with specified channel id
        /// </summary>
        /// <param name="channelId">The id of the channel to get data for</param>
        public GetChannelDataMessage(int channelId)
        {
            m_id = 18163; // Get Channel Data
            this.channelId = channelId;
        }
        #endregion

        #region Member Methods
        public static List<Channel> GetList()
        {
            GetChannelDataMessage msg = new GetChannelDataMessage(0);
            try
            {
                msg.Send();
            }
            catch (Exception)
            {
                return null;
            }

            List<Channel> channels = new List<Channel>();
            int count = msg.items.Length;
            for (int n = 0; n < count; ++n)
            {
                channels.Add(msg.items[n]);
            }

            return channels;
        }

        protected override void PackRequest()
        {
            // Create the streams that will be written to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Channel Id
            requestWriter.Write((int)channelId);

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
                items = new Channel[count];
                for (int i = 0; i < count; i++)
                {
                    Channel tmp = new Channel();
                    tmp.ChannelID = responseReader.ReadInt32();

                    ushort stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        tmp.ChannelName = new string(responseReader.ReadChars(stringLen));

                    tmp.ChannelNumber = responseReader.ReadInt32();
                    tmp.ChannelFrequency = responseReader.ReadInt32();
                    tmp.MajorChannel = responseReader.ReadInt32();
                    tmp.MinorChannel = responseReader.ReadInt32();
                    tmp.Enabled = (responseReader.ReadByte() == 1) ? true : false;

                    items.SetValue(tmp, i);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Channel Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Channel Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }
}