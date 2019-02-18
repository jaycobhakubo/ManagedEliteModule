using System;
using System.IO;
using System.Text;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    public class GetPaperExchangeDataMessage : ServerMessage
    {
        #region Local Fields

        private readonly string m_serial;
        private readonly int m_audit;
        #endregion

        #region Constructor

        private GetPaperExchangeDataMessage(string serial, int audit)
        {
            m_id = 36046;
            m_strMessageName = "Get Paper Exchange Item";

            //init with default values
            m_serial = serial;
            m_audit = audit;
        }
        
        #endregion
     
        #region Properties

        public PaperExchangeItem Item { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Prepares the request to be sent to the server.  All subclasses must
        /// implement this method.
        /// </summary>
        protected override void PackRequest()
        {            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Get product from serial audit
            WriteString(requestWriter, m_serial);

            requestWriter.Write(m_audit);
            
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
            //// Reset the values.
            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                //receipt ID
                var receiptId = responseReader.ReadInt32();

                //Machine ID
                var machineId = responseReader.ReadInt32();

                //Audit #
                var audit = responseReader.ReadInt32();

                //Serial #
                var serial = ReadString(responseReader);
                
                //Product Name
                var productName = ReadString(responseReader);

                //Staff first name
                var staffFirstName = ReadString(responseReader);

                //Staff last name
                var staffLastName = ReadString(responseReader);

                //staff full name
                var staffName = string.Format("{0} {1}", staffFirstName, staffLastName);

                // Transaction Number
                var transNum = responseReader.ReadInt32();

                // Gaming Session
                var session = responseReader.ReadInt32();

                // Gaming Date 
                var bingoDay = ReadDateTime(responseReader) ?? DateTime.Now.Date;

                Item = new PaperExchangeItem
                {
                    Name = productName,
                    Serial = serial,
                    Audit = audit,
                    Cashier = staffName,
                    ReceiptID = receiptId,
                    Machine = machineId,
                    TransactionNumber = transNum,
                    SoldSession = session,
                    GamingDate = bingoDay,
                };
            }
            catch (EndOfStreamException e)
            {
                Item = null;
                throw new MessageWrongSizeException(m_strMessageName, e);
            }
            catch (Exception e)
            {
                Item = null;
                throw new ServerException(m_strMessageName, e);
            }

            //Close the streams.
            responseReader.Close();
        }

        #endregion

        public static GetPaperExchangeDataMessage GetPaperExchangeData(string serial, int audit, out PaperExchangeItem item)
        {
            var msg = new GetPaperExchangeDataMessage(serial, audit);
            try
            {
                msg.Send();
            }
            catch (Exception)
            {
                // ignored
            }

            item = msg.Item;
            return msg;
        }
    }
}
