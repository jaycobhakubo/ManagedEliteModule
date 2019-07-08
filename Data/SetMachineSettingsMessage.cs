using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace GTI.Modules.Shared
{
    public class SetMachineSettingsMessage : ServerMessage
    {
        public struct SetMachineSettingsDataItem
        {
            public int settingId;
            public string settingValue;
            public bool useGlobalValue;
        }

        private int mMachineID = 0;
        private SetMachineSettingsDataItem[] mSettings;
        public SetMachineSettingsMessage(int machineID, SetMachineSettingsDataItem[] settings)
        {
            m_id = 18088;
            mMachineID = machineID;
            mSettings = new SetMachineSettingsDataItem[settings.Length];
            settings.CopyTo(mSettings, 0);
        }

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            //machine Id
            requestWriter.Write(mMachineID);

            //settings count
            requestWriter.Write((ushort)mSettings.Length);
            for (int iSetting = 0; iSetting < mSettings.Length; iSetting++)
            {
                requestWriter.Write(mSettings[iSetting].settingId);
                requestWriter.Write((ushort)mSettings[iSetting].settingValue.Length);
                if (mSettings[iSetting].settingValue.Length > 0)
                {
                    requestWriter.Write(mSettings[iSetting].settingValue.ToCharArray());
                }
                requestWriter.Write(mSettings[iSetting].useGlobalValue);
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

        }
        #endregion

    }
}
