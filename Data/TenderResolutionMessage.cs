using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class TenderResolutionMessage : ServerMessage
    {
        #region Member Variables
        protected List<SaleTender> m_saleTenders = null;
        #endregion

        #region Constructor
        
        public TenderResolutionMessage(List<SaleTender> saleTenders = null)
        {
            m_id = 18235;
            m_strMessageName = "Tender Resolution";
            SaleTenders = saleTenders;
        }

        #endregion

        #region Member Methods

        /// <summary>
        /// Packs the request, sends the message to the server, then
        /// unpacks the response if there are any tenders with 
        /// resolved or unresolved errors.
        /// </summary>
        public override void Send()
        {
            if(m_saleTenders != null && m_saleTenders.Count > 0)
                base.Send();
        }
        
        /// <summary>
        /// Prepares the request to be sent to the server
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);
            
            //count of tenders with resolved or unresolved errors
            requestWriter.Write((ushort) m_saleTenders.Count);

            foreach (SaleTender st in m_saleTenders)
            {
                //tender record ID
                requestWriter.Write(st.RegisterReceiptTenderID);

                //resolved status
                requestWriter.Write(st.IsResolvedPayment?1:0);

                //notes
                if (st.AdditionalTextForPaymentResolutionNotes != null)
                {
                    requestWriter.Write((ushort)st.AdditionalTextForPaymentResolutionNotes.Length);
                    requestWriter.Write(st.AdditionalTextForPaymentResolutionNotes.ToCharArray());
                }
                else
                {
                    requestWriter.Write((ushort)0);
                }
            }

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
        }

        #endregion

        #region Member Properties

        public List<SaleTender> SaleTenders
        {
            set
            {
                if (value != null)
                    m_saleTenders = value.FindAll(i => i.IsResolvedPayment || i.IsUnresolvedPayment);
                else
                    m_saleTenders = value;
            }
        }

        #endregion
    }
}
