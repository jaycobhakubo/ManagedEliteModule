using System;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    public class GetInventoryItemBySerialAuditMessage : ServerMessage
    {
        private readonly string m_serial;
        private readonly int m_audit;

        public GetInventoryItemBySerialAuditMessage(string serial, int audit)
        {
            m_id = 36049; // Message ID
            m_strMessageName = "Get Machine Data";

            m_serial = serial;
            m_audit = audit;
        }


        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Request Params

            //audit
            requestWriter.Write(m_audit);

            //serial
            requestWriter.Write((ushort)m_serial.Length);
            requestWriter.Write(m_serial.ToCharArray());

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

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of machines.
                InventoryItemId = responseReader.ReadInt32();

                var stringLen = responseReader.ReadInt16();
                ProductName = new string(responseReader.ReadChars(stringLen));

                stringLen = responseReader.ReadInt16();
                Price = new string(responseReader.ReadChars(stringLen));

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

        public int InventoryItemId { get; private set; }

        public string ProductName { get; private set; }

        public string Price { get; private set; }

    }
}
