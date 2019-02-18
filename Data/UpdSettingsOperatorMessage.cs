// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;

namespace GTI.Modules.Shared
{
    public class UpdSettingsOperatorMessage : ServerMessage
    {
 
        #region Member Variables
        protected int m_OperatorID = 0;
        protected Setting m_settingID;
        protected ArrayList m_settings = null;
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

        /// <summary>
        /// Gets the workstation settings received from the server.
        /// </summary>
        public SettingValue[] Settings
        {
            get
            {
                return (SettingValue[])m_settings.ToArray(typeof(SettingValue));
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UpdSettingsOperatorMessage class.
        /// </summary>
        public UpdSettingsOperatorMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UpdSettingsOperatorMessage class
        /// with the specified operator id.
        /// </summary>
        /// <param name="operatorId">The id of the operator to get 
        /// data for.</param>
        public UpdSettingsOperatorMessage(int OperatorID)
        {
            m_id = 18058; // Update Company Data
            m_OperatorID = OperatorID;
        }
        public UpdSettingsOperatorMessage(int OperatorID, ArrayList Settings)
        {
            m_id = 18058; // Update Company Data
            m_OperatorID = OperatorID;
            m_settings = Settings;
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

            
            requestWriter.Write(m_OperatorID);
            requestWriter.Write((ushort)m_settings.Count);
            //Loop our settings to send
            foreach(SettingValue mySetting in m_settings)
            {
                requestWriter.Write(mySetting.Id);
                requestWriter.Write(Convert.ToInt16(mySetting.Value.Length));
                requestWriter.Write(mySetting.Value.ToCharArray());
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
            try
            {
                // Seek past return code
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Upd Setting Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Upd Setting Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }

}
