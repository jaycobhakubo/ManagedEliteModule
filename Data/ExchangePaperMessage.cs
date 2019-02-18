using System;
using System.IO;
using System.Text;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    public class ExchangePaperMessage : ServerMessage
    {
        #region Local Fields

        private readonly string m_serial;
        private readonly int m_receiptId;
        private readonly int m_damagedAudit;
        private readonly int m_replacementAudit;
        #endregion

        #region Constructor

        private ExchangePaperMessage(string serial, int damagedAudit, int replacementAudit, int receiptId)
        {
            m_id = 36048;
            m_strMessageName = "Exchange Paper";

            //init with default values
            m_serial = serial;
            m_damagedAudit = damagedAudit;
            m_replacementAudit = replacementAudit;
            m_receiptId = receiptId;
        }
        
        #endregion
     
        #region Properties

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
            
            //receipt ID
            requestWriter.Write(m_receiptId);

            //Damaged Audit Number
            requestWriter.Write(m_damagedAudit);

            //Replacement Audit Number
            requestWriter.Write(m_replacementAudit);

            //Serial number
            WriteString(requestWriter, m_serial);
            
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
            base.UnpackResponse();//if return code != 0 then error

            //if (m_responsePayload == null)
            //    throw new ServerCommException("Server communication lost.");

            //if (m_responsePayload.Length < sizeof(int))
            //    throw new MessageWrongSizeException("Message payload size is too small.");

            //// Check the return code.
            //m_returnCode = BitConverter.ToInt32(m_responsePayload, 0);

            ////this m_returncode is actually an ID referencing a status of a paper.
            ////if (m_returnCode != (int)GTIServerReturnCode.Success)
            ////    throw new ServerException((GTIServerReturnCode)m_returnCode, "Server Error Code: " + m_returnCode.ToString());

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);
            
            //Close the streams.
            responseReader.Close();
        }

        #endregion

        public static int ExchangePaper(string serial, int damagedAudit, int replacementAudit, int receiptId)
        {
            var msg = new ExchangePaperMessage(serial, damagedAudit, replacementAudit, receiptId);
            try
            {
                msg.Send();
            }
            catch (Exception)
            {

            }
            return msg.ReturnCode;
        }

        public static int ExchangePaper(PaperExchangeItem item, int replacementAudit)
        {
            var msg = new ExchangePaperMessage(item.Serial, item.Audit, replacementAudit, item.ReceiptID);
            try
            {
                msg.Send();
            }
            catch (Exception)
            {

            }
            return msg.ReturnCode;
        }
    }
}
