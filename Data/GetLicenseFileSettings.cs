#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2010 GameTech
// International, Inc.
#endregion

// Rally US1274
// Rally US1833 - Add expiration date to the license file message.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Enumerates by whom a setting can be changed.
    /// </summary>
    public enum SettingPermission
    {
        ReadOnly = 0,
        Admin = 1,
        Customer = 2
    }

    /// <summary>
    /// Enumerates if the setting value has a min/max.
    /// </summary>
    public enum SettingRange
    {
        NoRange = 0,
        MinRange = 1,
        MaxRange = 2
    }

    /// <summary>
    /// Represents a value from the license file (optionally, with permission
    /// and range).
    /// </summary>
    public class LicenseFileItem
    {
        //todo refactor to properties
        public int settingID;
        public byte settingPermission;
        public byte settingRange;
        public string value;
    }

    /// <summary>
    /// Represents a Get License File Settings server message.
    /// </summary>
    public class GetLicenseFileSettingsMessage : ServerMessage
    {
        #region Member Variables
        private List<LicenseFileItem> m_licenseFileItems = new List<LicenseFileItem>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetLicenseFileSettingsMessage
        /// class.
        /// </summary>
        /// <param name="licenseSettingsOnly">true to only return license
        /// settings; otherwise all license and global settings will be
        /// returned.</param>
        public GetLicenseFileSettingsMessage(bool licenseSettingsOnly)
        {
            m_id = 25016; // Get License File Settings
            m_strMessageName = "Get License File Settings";
            LicenseSettingsOnly = licenseSettingsOnly;
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

            // License File Only
            requestWriter.Write(LicenseSettingsOnly);

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
            // Clear previous values.
            ExpirationDate = DateTime.MinValue;
            m_licenseFileItems.Clear();

            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // License Expiration Date
                ushort stringLen = responseReader.ReadUInt16();
                string tempDate = new string(responseReader.ReadChars(stringLen));

                if(!string.IsNullOrEmpty(tempDate))
                    ExpirationDate = DateTime.Parse(tempDate, CultureInfo.InvariantCulture);

                // Setting Count
                ushort settingCount = responseReader.ReadUInt16();

                for(ushort x = 0; x < settingCount; x++)
                {
                    LicenseFileItem item = new LicenseFileItem();

                    // Id
                    item.settingID = responseReader.ReadInt32();

                    // Value
                    stringLen = responseReader.ReadUInt16();

                    if(stringLen > 0)
                        item.value = new string(responseReader.ReadChars(stringLen));

                    // Permission
                    item.settingPermission = responseReader.ReadByte();

                    // Range
                    item.settingRange = responseReader.ReadByte();

                    m_licenseFileItems.Add(item);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException(m_strMessageName, e);
            }
            catch(Exception e)
            {
                throw new ServerException(m_strMessageName, e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// true to only return license settings; otherwise all license and
        /// global settings will be returned.
        /// </summary>
        public bool LicenseSettingsOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the date on which the license file expires.
        /// </summary>
        public DateTime ExpirationDate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of license file items received from the server.
        /// </summary>
        public List<LicenseFileItem> LicenseFileItems
        {
            get
            {
                return m_licenseFileItems;
            }
        }

        /// <summary>
        /// Gets a list of license settings received from the server.
        /// </summary>
        public IEnumerable<LicenseSettingValue> LicenseSettings
        {
            get
            {
                List<LicenseSettingValue> settings = new List<LicenseSettingValue>();

                foreach(LicenseFileItem item in LicenseFileItems)
                {
                    if(item.settingID > (int)LicenseSetting.MinValueId)
                    {
                        LicenseSettingValue licValue = new LicenseSettingValue();
                        licValue.Id = item.settingID;
                        licValue.Value = item.value;

                        settings.Add(licValue);
                    }
                }

                return settings;
            }
        }
        #endregion
    }
}