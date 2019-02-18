using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;

namespace GTI.Modules.Shared
{
    public class GetMachineSettingsOnlyMessage : ServerMessage
    {
        private const int MinResponseMessageLength = 6;

		protected Int32 m_nMachineId = 0;
		protected Int32 m_nSettingCategoryId = 0;  // 0 will return all machine settings
		protected SettingValue[] m_arrMachineSettings = new SettingValue[0];
        private GetMachineSettingsOnlyMessage()
        {
			m_id = 18025; // Message ID
			m_strMessageName = "Get Machine Settings Only";
        }

		public GetMachineSettingsOnlyMessage(Int32 nMachineId, Int32 nSettingCategoryId)
		{
			m_nMachineId = nMachineId;
			m_nSettingCategoryId = nSettingCategoryId;
			m_id = 18025; // Message ID
			m_strMessageName = "Get Machine Settings Only";
		}

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Request Params
			requestWriter.Write(m_nSettingCategoryId);
			requestWriter.Write(m_nMachineId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException(m_strMessageName);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of machines.
                Int16 wSettingCount = responseReader.ReadInt16();

                // Allocate the array.
				m_arrMachineSettings = new SettingValue[wSettingCount];

                // Read all the data structs
				Int16 wStringLen = 0;
				for (int i = 0; i < wSettingCount; i++)
				{
					m_arrMachineSettings[i].Id = responseReader.ReadInt32();
					m_arrMachineSettings[i].Category = responseReader.ReadInt32();
					wStringLen = responseReader.ReadInt16();
					m_arrMachineSettings[i].Value = new string(responseReader.ReadChars(wStringLen));
				}
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException(m_strMessageName, e);
            }
            catch (Exception e)
            {
                throw new ServerException(m_strMessageName, e);
            }

            // Close the streams.
            responseReader.Close();
        }

		public bool TryGetSettingValue( Setting nGlobalSettingId, out SettingValue result )
		{
			result = new SettingValue();

			int nCount = m_arrMachineSettings.Length;
			for ( int i=0 ; i<nCount ; i++ )
			{
				if ( m_arrMachineSettings[i].Id == (Int32)nGlobalSettingId )
				{
					result = m_arrMachineSettings[i];
					return true;
				}
			}

			// Setting not found
			return false;
		}

		// Properties
		public SettingValue[] MachineSettingsList
		{
			get
			{
				return m_arrMachineSettings;
			}
		}

    }
}
