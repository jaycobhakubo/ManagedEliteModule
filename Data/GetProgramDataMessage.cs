using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared.Data
{
    public class GetProgramDataMessage : ServerMessage
    {
        private const int MinResponseMessageLength = 6;

        private Int32 m_nOperatorId = 0;
        protected Program[] m_arrProgramData = null;

        private GetProgramDataMessage()
        {
            m_id = 18074; // Message ID
            m_strMessageName = "Get Program Data";
        }

        public GetProgramDataMessage(Int32 nOperatorId)
        {
            m_nOperatorId = nOperatorId;
            m_id = 18074; // Message ID
            m_strMessageName = "Get Program Data";
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Request Params
            requestWriter.Write(m_nOperatorId);

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
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException(m_strMessageName);

            // Try to unpack the data.
            try
            {
                // Seek past return code
                responseReader.BaseStream.Seek(sizeof(Int32), SeekOrigin.Begin);

                // Get the category count
                Int16 wCount = responseReader.ReadInt16();

                // Allocate the array
                m_arrProgramData = new Program[wCount];

                // Read the programs
                for(int i = 0; i < wCount; i++)
                {
                    m_arrProgramData[i] = new Program { ProgramId = responseReader.ReadInt32() };
                    Int16 wStringLen = responseReader.ReadInt16();
                    m_arrProgramData[i].Name = new string(responseReader.ReadChars(wStringLen));
                    m_arrProgramData[i].IsActive = responseReader.ReadBoolean();
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

        // Properties
        public Program[] arrProgramData
        {
            get
            {
                return m_arrProgramData;
            }
        }

        public static IList<Program> GetProgramData(int operatorId)
        {
            var m = new GetProgramDataMessage(operatorId);
            m.Send();

            if(m.ServerReturnCode == GTIServerReturnCode.Success)
                return m.arrProgramData;
            else
                return null;
        }
    }
}
