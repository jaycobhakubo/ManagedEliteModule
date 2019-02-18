// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    public class GetDeviceTypeDataMessage : ServerMessage 
    {
        private Device[] m_devices;
        public GetDeviceTypeDataMessage()
        {
            m_id = 18028;
        }
        #region Member Methods
        
        /// <summary>
        /// The purpose of this message is to retrieve a list of all of the available Device types
        /// </summary>
        /// <returns></returns>
        public static Device[] GetDeviceTypeData()
        {
            var msg = new GetDeviceTypeDataMessage();
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("Get Cardset Color Level Data: " + ex.ToString());
            }
            return msg.Devices;
        }

        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
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

                ushort count = responseReader.ReadUInt16();
                m_devices = new Device[count];
                for (int iDevice = 0; iDevice < count; iDevice++)
                {
                    m_devices[iDevice].Id = responseReader.ReadInt32();
                    m_devices[iDevice].Name = ReadString(responseReader);
                    m_devices[iDevice].LoginConnectionType = (DeviceLoginConnectionType)responseReader.ReadInt32();
                }
            }
            catch (Exception ex)
            {
                MessageForm.Show( Resources.NoDeviceTypes + ex.Message, "ManagedEliteModule");
            }

            // Close the streams.
            responseReader.Close();
        }

        public Device[] Devices
        {
            get { return m_devices; }
        }
        #endregion
    }
}
