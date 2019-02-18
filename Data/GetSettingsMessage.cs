// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get System Settings server message.
    /// </summary>
    public class GetSettingsMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;       
        #endregion

        #region Member Variables
        protected int m_operatorId;
        protected int m_machineId;
        protected SettingsCategory m_category = SettingsCategory.AllCategories;
        protected Setting m_globalSettingId;
        protected SettingValue[] m_settings = new SettingValue[0];  // I changed this to an array so it will NOT be read only like a list
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetSettingsMessage class.  When 
        /// no parameters are specified, all default global settings are 
        /// returned.
        /// </summary>
        public GetSettingsMessage()
            : this(0, 0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetSettingsMessage class.  When a 
        /// machine and category are specified, no operator overridden settings
        /// are returned.
        /// </summary>
        /// <param name="machineId">The machine to get the 
        /// settings for.</param>
        /// <param name="category">The category of settings to get.</param>
        public GetSettingsMessage(int machineId, SettingsCategory category)
            : this(machineId, 0, category, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetSettingsMessage class.  When a 
        /// machine, operator, and category are specified, all settings are 
        /// returned.
        /// </summary>
        /// <param name="machineId">The machine to get the 
        /// settings for.</param>
        /// <param name="operatorId">The operator to get the settings 
        /// for.</param>
        /// <param name="category">The category of settings to get.</param>
        public GetSettingsMessage(int machineId, int operatorId, SettingsCategory category)
            : this(machineId, operatorId, category, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetSettingsMessage class.  When a 
        /// machine, operator, and category are specified, all settings are 
        /// returned.  In addition, if a globalSettingId is specified, then 
        /// only that setting is returned (or 0 for all settings).
        /// </summary>
        /// <param name="machineId">The machine to get the 
        /// settings for.</param>
        /// <param name="operatorId">The operator to get the settings 
        /// for.</param>
        /// <param name="category">The category of settings to get.</param>
        /// <param name="globalSettingId">The specific global setting to get or 
        /// 0 for all settings.</param>
        public GetSettingsMessage(int machineId, int operatorId, SettingsCategory category, Setting globalSettingId)
        {
            m_id = 18100; // Get System Settings
            m_machineId = machineId;
            m_operatorId = operatorId;
            m_category = category;
            m_globalSettingId = globalSettingId;
            m_settings = new SettingValue[0];
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

            // Operator Id
            requestWriter.Write(m_operatorId);

            // Machine Id
            requestWriter.Write(m_machineId);

            // Setting Category Id
            requestWriter.Write((int)m_category);

            // Global Setting Id
            requestWriter.Write((int)m_globalSettingId);

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
                throw new MessageWrongSizeException("Get System Settings");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of settings.
                ushort settingsCount = responseReader.ReadUInt16();

                // Clear the settings array.
				m_settings = new SettingValue[settingsCount];

                // Read all the settings.
                for(ushort x = 0; x < settingsCount; x++)
                {
                    SettingValue setting;

                    // Setting Id
                    setting.Id = responseReader.ReadInt32();

                    // Category Id
                    setting.Category = responseReader.ReadInt32();

                    // Length of the setting.
                    ushort stringLen = responseReader.ReadUInt16();

                    // The setting value.
                    setting.Value = new string(responseReader.ReadChars(stringLen));

                    m_settings[x] = setting;
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get System Settings", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get System Settings", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the opreator to get settings for (or 0 for all 
        /// machines).
        /// </summary>
        public int OperatorId
        {
            get
            {
                return m_operatorId;
            }
            set
            {
                m_operatorId = value;
            }
        }

        /// <summary>
        /// Gets or sets the machine to get settings for (or 0 for all 
        /// operators).
        /// </summary>
        public int MachineId
        {
            get
            {
                return m_machineId;
            }
            set
            {
                m_machineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the category to retrieve.
        /// </summary>
        public SettingsCategory Category
        {
            get
            {
                return m_category;
            }
            set
            {
                m_category = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the global setting to retrieve (or 0 for 
        /// all settings).
        /// </summary>
        public Setting GlobalSettingId
        {
            get
            {
                return m_globalSettingId;
            }
            set
            {
                m_globalSettingId = value;
            }
        }

        /// <summary>
        /// Gets the workstation settings received from the server.
        /// </summary>
        public SettingValue[] Settings
        {
            get
            {
                return m_settings;
            }
        }
        #endregion
    }
}
