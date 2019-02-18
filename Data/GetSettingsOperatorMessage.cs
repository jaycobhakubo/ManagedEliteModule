// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Workstation Settings server message.
    /// </summary>
    public class GetSettingsOperatorMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_OperatorID = 0;
        protected Setting m_settingID = Setting.ActivityTimeout;
        protected int m_workstationId = 0;
        //protected ArrayList m_settings = null;
        protected Dictionary <int,SettingValue> m_settings = new Dictionary<int,SettingValue>(); //<int setingID, its setting value>
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetSettingsMessage class.
        /// </summary>
        public GetSettingsOperatorMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetSettingsMessage class
        /// with the specified machine and category id.
        /// </summary>
        /// <param name="machineId">The machine to get the 
        /// settings for.</param>
        /// <param name="categoryId">The category of settings to get.</param>
        public GetSettingsOperatorMessage(int iOperatorID)
            : this (iOperatorID,0)
        {}
         public GetSettingsOperatorMessage(int iOperatorID, int settingID)
         {
            m_id = 18057; // Get Operator Settings
            m_OperatorID = iOperatorID;
            m_settingID = (Setting) settingID;
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
            requestWriter.Write(m_OperatorID);

            // Get all settings
            requestWriter.Write((int) m_settingID);

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
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Operator Settings");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of settings.
                int settingsCount = responseReader.ReadInt16();

                // Clear the settings array.
                m_settings.Clear();

                // Read all the settings.
                for (int x = 0; x < settingsCount; x++)
                {
                    SettingValue setting;

                    // Parameter Id
                    setting.Id = responseReader.ReadInt32();

                    // Category Id
                    setting.Category = responseReader.ReadInt32();

                    // Length of the parameter.
                    ushort stringLen = responseReader.ReadUInt16();

                    // The parameter value.
                    setting.Value = new string(responseReader.ReadChars(stringLen));

                    m_settings.Add(setting.Id,setting);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Operator Settings", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Operator Settings", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the operator id to pass to the server.
        /// </summary>
        public int OperatorID
        {
            get
            {
                return m_OperatorID;
            }
            set
            {
                m_OperatorID = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the category to retrieve.
        /// </summary>
        public Setting SettingID
        {
            get
            {
                return m_settingID;
            }
            set
            {
                m_settingID = value;
            }
        }

        public Dictionary<int, SettingValue> SettingsDictionary
        {
            get { return m_settings; }
        }
        /// <summary>
        /// Gets the workstation settings received from the server.
        /// </summary>
        //public SettingValue[] Settings
        public Dictionary<int,SettingValue>.ValueCollection Settings
        {
            get
            {
                //return (SettingValue[])m_settings.ToArray(typeof(SettingValue));
                return m_settings.Values;
            }
        }
        #endregion
    }
}
