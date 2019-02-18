using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class GetSetSaleTextMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected string[] m_receiptText = new string[0];
        protected int m_registerReceiptID;
        protected int m_textType;
        protected bool m_Set = false;
        #endregion

        #region Constructor
        
        public GetSetSaleTextMessage(int registerReceiptID = 0)
        {
            m_id = 18233;
            m_strMessageName = "Get/Set Sale Text";
            m_registerReceiptID = registerReceiptID;
        }

        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            //Get or Set
            requestWriter.Write(m_Set?1:0);

            // receipt ID
            requestWriter.Write(m_registerReceiptID);

            // text type
            requestWriter.Write(m_textType);

            // text
            //convert the array into one big string
            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < m_receiptText.Length; x++)
                sb.Append(m_receiptText[x] + (x == m_receiptText.Length - 1?"":"\n"));

            string blob = sb.ToString();

            requestWriter.Write((ushort)blob.Length);

            if (blob.Length > 0)
                requestWriter.Write(blob.ToCharArray());
            
            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        /// <summary>
        /// Parses the response received from the server
        /// </summary>
        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get/Set Sale Text");

            try
            {
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                int stringLen = responseReader.ReadUInt16();

                if (stringLen == 0)
                {
                    m_receiptText = new string[0];
                }
                else
                {
                    string replyText = new string(responseReader.ReadChars(stringLen));

                    //convert the string to an array
                    replyText = replyText.Replace("\r\n", "\n");
                    m_receiptText = replyText.Split('\n');
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get/Set Sale Text", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get/Set Sale Text", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        #endregion

        #region Member Properties

        /// <summary>
        /// Get or set the receipt ID.
        /// </summary>
        public int RegisterReceiptID
        {
            get
            {
                return m_registerReceiptID;
            }

            set
            {
                m_registerReceiptID = value;
            }
        }

        /// <summary>
        /// Get or set the array of text.
        /// </summary>
        public string[] ReceiptText
        {
            get
            {
                return m_receiptText;
            }

            set
            {
                m_receiptText = value;
            }
        }

        /// <summary>
        /// Set a single string as the text.
        /// </summary>
        public string ReceiptLine
        {
            set
            {
                m_receiptText = new string[] {value};
            }
        }

        /// <summary>
        /// Get or set the type of text (1=Line item detail).
        /// </summary>
        public int TextType
        {
            get
            {
                return m_textType;
            }

            set
            {
                m_textType = value;
            }
        }

        /// <summary>
        /// Get or set if the Send method will get the text or save it.
        /// </summary>
        public bool GetText
        {
            get
            {
                return !m_Set;
            }

            set
            {
                m_Set = !value;
            }
        }

        /// <summary>
        /// Get or set if the Send method will get the text or save it.
        /// </summary>
        public bool SaveText
        {
            get
            {
                return m_Set;
            }

            set
            {
                m_Set = value;
            }
        }

        #endregion
    }
}
