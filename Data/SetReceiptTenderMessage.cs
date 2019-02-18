using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class SetReceiptTenderMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected SaleTender m_saleTender;
        protected int m_transactionNumber;
        #endregion

        #region Constructor

        public SetReceiptTenderMessage()
        {
            m_id = 18227;
            m_strMessageName = "Set Receipt Tender";
            m_saleTender = null;
        }

        public SetReceiptTenderMessage(SaleTender tender)
        {
            m_id = 18227;
            m_strMessageName = "Set Receipt Tender";
            m_saleTender = tender;
        }

        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams that will be written to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write(m_saleTender.RegisterReceiptID);
            requestWriter.Write((int)m_saleTender.TenderTypeID);
            requestWriter.Write((int)m_saleTender.TenderSubTypeID);
            requestWriter.Write((int)m_saleTender.TransactionTypeID);

            if (m_saleTender.IsoCode != null)
            {
                requestWriter.Write((ushort)m_saleTender.IsoCode.Length);
                requestWriter.Write(m_saleTender.IsoCode.ToCharArray());
            }
            else
            {
                requestWriter.Write((ushort)0);
            }

            requestWriter.Write((ushort)m_saleTender.Amount.ToString().Length);
            requestWriter.Write(m_saleTender.Amount.ToString().ToCharArray());
            
            requestWriter.Write((ushort)m_saleTender.DefaultAmount.ToString().Length);
            requestWriter.Write(m_saleTender.DefaultAmount.ToString().ToCharArray());
            
            if (m_saleTender.ReferenceNumber != null)
            {
                requestWriter.Write((ushort)m_saleTender.ReferenceNumber.Length);
                requestWriter.Write(m_saleTender.ReferenceNumber.ToCharArray());
            }
            else
            {
                requestWriter.Write((ushort)0);
            }
            
            if (m_saleTender.AuthorizationCode != null)
            {
                requestWriter.Write((ushort)m_saleTender.AuthorizationCode.Length);
                requestWriter.Write(m_saleTender.AuthorizationCode.ToCharArray());
            }
            else
            {
                requestWriter.Write((ushort)0);
            }
            
            if (m_saleTender.ReceiptDescription != null)
            {
                requestWriter.Write((ushort)m_saleTender.ReceiptDescription.Length);
                requestWriter.Write(m_saleTender.ReceiptDescription.ToCharArray());
            }
            else
            {
                requestWriter.Write((ushort)0);
            }
            
            if (m_saleTender.AdditionalCustomerText != null)
            {
                requestWriter.Write((ushort)m_saleTender.AdditionalCustomerText.Length);
                requestWriter.Write(m_saleTender.AdditionalCustomerText.ToCharArray());
            }
            else
            {
                requestWriter.Write((ushort)0);
            }
            
            if (m_saleTender.AdditionalMerchantText != null)
            {
                requestWriter.Write((ushort)m_saleTender.AdditionalMerchantText.Length);
                requestWriter.Write(m_saleTender.AdditionalMerchantText.ToCharArray());
            }
            else
            {
                requestWriter.Write((ushort)0);
            }

            if (m_saleTender.AdditionalErrorText != null)
            {
                requestWriter.Write((ushort)m_saleTender.AdditionalErrorText.Length);
                requestWriter.Write(m_saleTender.AdditionalErrorText.ToCharArray());
            }
            else
            {
                requestWriter.Write((ushort)0);
            }
            
            requestWriter.Write(m_saleTender.OriginalRegisterReceiptTenderID);

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
                throw new MessageWrongSizeException("Set Receipt Tender");

            try
            {
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                m_saleTender.RegisterReceiptTenderID = responseReader.ReadInt32();
                m_saleTender.RegisterReceiptID = responseReader.ReadInt32();
                m_transactionNumber = responseReader.ReadInt32();
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Set Receipt Tender", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Set Receipt Tender", e);
            }
        }

        #endregion

        #region Member Properties

        public SaleTender Tender
        {
            set { m_saleTender = value; }
        }

        public int TransactionNumber
        {
            get
            {
                return m_transactionNumber;
            }
        }

        #endregion
    }
}
