using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class GetSaleTendersMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected List<SaleTender> m_saleTenders;
        protected int m_receiptRegisterID = 0;
        #endregion

        #region Constructor
        
        public GetSaleTendersMessage(int registerReceiptID = 0)
        {
            m_id = 18231;
            m_strMessageName = "Get Sale Tenders";
            m_saleTenders = new List<SaleTender>();
            m_receiptRegisterID = registerReceiptID;
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

            // receipt ID
            requestWriter.Write(m_receiptRegisterID);

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
                throw new MessageWrongSizeException("Get Sale Tenders");

            try
            {
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                int ttCount = responseReader.ReadInt16();
                int stringLen;

                for (int i = 0; i < ttCount; ++i)
                {
                    SaleTender newTender = new SaleTender();

                    newTender.RegisterReceiptTenderID = responseReader.ReadInt32();
                    newTender.RegisterReceiptID = responseReader.ReadInt32();

                    stringLen = responseReader.ReadUInt16();
                    newTender.DTStamp = Convert.ToDateTime(new string(responseReader.ReadChars(stringLen)));

                    newTender.TenderTypeID = (TenderType)responseReader.ReadInt32();

                    newTender.TenderSubTypeID = responseReader.ReadInt32();

                    newTender.TransactionTypeID = (TransactionType)responseReader.ReadInt32();
                    
                    stringLen = responseReader.ReadUInt16();
                    newTender.IsoCode = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newTender.Amount = Convert.ToDecimal(new string(responseReader.ReadChars(stringLen)));
                    
                    stringLen = responseReader.ReadUInt16();
                    newTender.DefaultAmount = Convert.ToDecimal(new string(responseReader.ReadChars(stringLen)));

                    stringLen = responseReader.ReadUInt16();
                    newTender.DefaultTax = Convert.ToDecimal(new string(responseReader.ReadChars(stringLen)));

                    stringLen = responseReader.ReadUInt16();
                    newTender.ReferenceNumber = new string(responseReader.ReadChars(stringLen));
                    
                    stringLen = responseReader.ReadUInt16();
                    newTender.AuthorizationCode = new string(responseReader.ReadChars(stringLen));
                    
                    stringLen = responseReader.ReadUInt16();
                    newTender.ReceiptDescription = new string(responseReader.ReadChars(stringLen));
                    
                    newTender.OriginalRegisterReceiptTenderID = responseReader.ReadInt32();
                    
                    stringLen = responseReader.ReadUInt16();
                    newTender.AdditionalCustomerText = new string(responseReader.ReadChars(stringLen));
                    
                    stringLen = responseReader.ReadUInt16();
                    newTender.AdditionalMerchantText = new string(responseReader.ReadChars(stringLen));

                    int paymentStatus = responseReader.ReadInt32();

                    newTender.IsUnresolvedPayment = paymentStatus == 1;
                    newTender.IsResolvedPayment = paymentStatus == 2;

                    stringLen = responseReader.ReadUInt16();
                    newTender.AdditionalErrorText = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newTender.AdditionalTextForPaymentResolutionNotes = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newTender.ExchangeRate = Convert.ToDecimal(new string(responseReader.ReadChars(stringLen)));

                    m_saleTenders.Add(newTender);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Sale Tenders", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Sale Tenders", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        #endregion

        #region Member Properties

        public int RegisterReceiptID
        {
            get
            {
                return m_receiptRegisterID;
            }

            set
            {
                if(m_receiptRegisterID != value)
                    m_saleTenders = new List<SaleTender>();

                m_receiptRegisterID = value;
            }
        }

        public List<SaleTender> SaleTenders
        {
            get
            {
                return m_saleTenders;
            }
        }

        #endregion
    }
}
