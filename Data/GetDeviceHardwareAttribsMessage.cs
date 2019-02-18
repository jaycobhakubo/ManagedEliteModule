// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a hardware attribute of a device.
    /// </summary>
    public struct DeviceHardwareAttribute
    {
        public int DeviceId;
        public int HardwareAttributeId;
        public string DataValue;
    }

    /// <summary>
    /// Represents the Get Player Tier List server message.
    /// </summary>
    public class GetDeviceHardwareAttribsMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_deviceId = 0;
        protected int m_hardwareAttribId = 0;
        protected List<DeviceHardwareAttribute> m_attribs = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetDeviceHardwareAttribsMessage 
        /// class.
        /// </summary>
        public GetDeviceHardwareAttribsMessage()
            : this(0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetDeviceHardwareAttribsMessage 
        /// class with the specified device and hardware attribute ids.
        /// </summary>
        /// <param name="deviceId">The id of the device to return attributes 
        /// for or 0 for all devices.</param>
        /// <param name="hardwareAttribId">The id of the hardware attribute to 
        /// return or 0 for all attributes.</param>
        public GetDeviceHardwareAttribsMessage(int deviceId, int hardwareAttribId)
        {
            m_id = 18045; // Get Device Hardware Attributes
            m_deviceId = deviceId;
            m_hardwareAttribId = hardwareAttribId;
            m_attribs = new List<DeviceHardwareAttribute>();
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

            // Device Id
            requestWriter.Write(m_deviceId);

            // Hardware Attribute Id
            requestWriter.Write(m_hardwareAttribId);

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

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Device Hardware Attributes");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of attribs.
                ushort attribCount = responseReader.ReadUInt16();

                // Clear the attrib array.
                m_attribs.Clear();

                // Read all the attribs.
                for(ushort x = 0; x < attribCount; x++)
                {
                    DeviceHardwareAttribute attrib = new DeviceHardwareAttribute();

                    // Device Id
                    attrib.DeviceId = responseReader.ReadInt32();

                    // Hardware Attribute Id
                    attrib.HardwareAttributeId = responseReader.ReadInt32();

                    // Data Value
                    ushort stringLen = responseReader.ReadUInt16();
                    attrib.DataValue = new string(responseReader.ReadChars(stringLen));

                    m_attribs.Add(attrib);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Device Hardware Attributes", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Device Hardware Attributes", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the device to return attributes for or 0 
        /// for all devices.
        /// </summary>
        public int DeviceId
        {
            get
            {
                return m_deviceId;
            }
            set
            {
                m_deviceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the hardware attribute to return or 0 for 
        /// all attributes.
        /// </summary>
        public int HardwareAttributeId
        {
            get
            {
                return m_hardwareAttribId;
            }
            set
            {
                m_hardwareAttribId = value;
            }
        }

        /// <summary>
        /// Gets all the attributes received from the server.
        /// </summary>
        public DeviceHardwareAttribute[] Attributes
        {
            get
            {
                return m_attribs.ToArray();
            }
        }
        #endregion
    }
}
