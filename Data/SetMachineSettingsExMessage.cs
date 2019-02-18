using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace GTI.Modules.Shared
{
    public class SetMachineSettingsExMessage : ServerMessage
    {
        private Int32[] m_arrMachineIDs;
        private SettingValue[] m_arrSettings;
		public SetMachineSettingsExMessage(Int32[] arrMachineIDs, SettingValue[] arrSettings)
        {
            m_id = 18118;
			m_arrMachineIDs = arrMachineIDs;
			m_arrSettings = arrSettings;
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

            // Machine count
			Int16 wMachineCount = (Int16)m_arrMachineIDs.Length;
			requestWriter.Write(wMachineCount);

			// Machine list
			for (int i = 0; i < wMachineCount; i++)
			{
				// Machine ID
				requestWriter.Write(m_arrMachineIDs[i]);

				// Settings count
				requestWriter.Write((ushort)m_arrSettings.Length);
				for (int iSetting = 0; iSetting < m_arrSettings.Length; iSetting++)
				{
					requestWriter.Write(m_arrSettings[iSetting].Id);
					requestWriter.Write((ushort)m_arrSettings[iSetting].Value.Length);
					if (m_arrSettings[iSetting].Value.Length > 0)
					{
						requestWriter.Write(m_arrSettings[iSetting].Value.ToCharArray());
					}
				}
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
