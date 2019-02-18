using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    public class GetMachineDataMessage : ServerMessage
    {
        private const int MinResponseMessageLength = 6;

        protected short m_clientStatus = 0;
        protected int m_deviceType = 0;
        protected int m_unitNum = 0;
        protected int m_machineID = 0;
        protected int m_packNum = 0;
        protected Machine[] m_arrMachineData = null;

        private GetMachineDataMessage()
        {
			m_id = 25004; // Message ID
            m_strMessageName = "Get Machine Data";
        }

        public GetMachineDataMessage(short status)
            : this()
        {
            m_clientStatus = status;
        }

        public GetMachineDataMessage(int deviceType, short status, int unitNum, int machineID, int packNum)
            :this()
		{
            m_deviceType = deviceType;
            m_clientStatus = status;
            m_unitNum = unitNum;
            m_machineID = machineID;
            m_packNum = packNum;
		}

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Request Params
            requestWriter.Write(m_clientStatus);
            requestWriter.Write(m_deviceType);
            requestWriter.Write(m_unitNum);
            requestWriter.Write(m_machineID);
            requestWriter.Write(m_packNum);

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
                Int16 wMachineCount = responseReader.ReadInt16();

                // Allocate the array.
				m_arrMachineData = new Machine[wMachineCount];

                // Read all the data structs
				Int16 wStringLen = 0;
				for (int i = 0; i < wMachineCount; i++)
				{
                    m_arrMachineData[i] = new Machine();
					m_arrMachineData[i].Id = responseReader.ReadInt32();
					m_arrMachineData[i].DeviceType = Device.FromId(responseReader.ReadInt32());
					m_arrMachineData[i].LocationId = responseReader.ReadInt32();
					wStringLen = responseReader.ReadInt16();
					m_arrMachineData[i].ClientIdentifier = new string(responseReader.ReadChars(wStringLen));
					wStringLen = responseReader.ReadInt16();
					m_arrMachineData[i].Description = new string(responseReader.ReadChars(wStringLen));
					m_arrMachineData[i].IsEnabled = responseReader.ReadBoolean();
                    m_arrMachineData[i].UnitNumber =(short) responseReader.ReadInt32(); // Rally US20

                    // Rally US247                    
                    int playerId = responseReader.ReadInt32();
                    wStringLen = responseReader.ReadInt16();
                    string firstName = new string(responseReader.ReadChars(wStringLen));
                    wStringLen = responseReader.ReadInt16();
                    string lastName = new string(responseReader.ReadChars(wStringLen));

                    if(playerId != 0)
                    {
                        m_arrMachineData[i].AssignedPlayer = new Player();
                        m_arrMachineData[i].AssignedPlayer.Id = playerId;
                        m_arrMachineData[i].AssignedPlayer.FirstName = firstName;
                        m_arrMachineData[i].AssignedPlayer.LastName = lastName;
                    }
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

		// Properties
        public Machine[] MachineDataList
		{
			get
			{
				return m_arrMachineData;
			}
		}

    }
}
