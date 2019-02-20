// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013-2016 FortuNet


using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace GTI.Modules.Shared.Data
{
    public class SetPromoTextMessage : ServerMessage
    {
        #region Member variables and classes
        public class PromoInfo
        {
            public int promoGroupID;
            public string promoText;
        }

        private List<PromoInfo> m_promoData = new List<PromoInfo>();
        private int m_firstGroup = 0;
        private int m_lastGroup = 0;
        #endregion

        #region Member Properties
        public string[] PromoData
        {
            set
            {
                m_promoData.Clear();
                m_firstGroup = 1;
                m_lastGroup = 1;

                int currentGroupID = 1;

                foreach(string s in value)
                {
                    if(s.StartsWith("Group=", StringComparison.CurrentCultureIgnoreCase) && s.Length > 6)
                    {
                        int groupWas = currentGroupID;
                        
                        if(!Int32.TryParse(s.Substring(6), out currentGroupID))
                            currentGroupID = groupWas;

                        if (currentGroupID < m_firstGroup)
                            m_firstGroup = currentGroupID;

                        if (currentGroupID > m_lastGroup)
                            m_lastGroup = currentGroupID;
                    }
                    else
                    {
                        PromoInfo pi = new PromoInfo();

                        pi.promoGroupID = currentGroupID;
                        pi.promoText = (s == string.Empty? " " : s);
                        m_promoData.Add(pi);
                    }
                }
            }
        }
        #endregion
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Set Promo Text message
        /// </summary>
        SetPromoTextMessage()
        {
        }

        public SetPromoTextMessage(string[] promoData)
        {
            m_id = 18263; // Modify Channel Data Message
            PromoData = promoData;
        }

        public SetPromoTextMessage(List<PromoInfo> promoData)
        {
            m_id = 18263; // Modify Channel Data Message
            m_promoData = promoData;
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

            requestWriter.Write((int)m_promoData.Count);

            for (int currentGroup = m_firstGroup; currentGroup <= m_lastGroup; currentGroup++)
            {
                ushort groupLine = 0;

                foreach (PromoInfo pi in m_promoData.FindAll(i => i.promoGroupID == currentGroup))
                {
                    requestWriter.Write(pi.promoGroupID);
                    requestWriter.Write(++groupLine);
                    requestWriter.Write((ushort)pi.promoText.Length);
                    requestWriter.Write(pi.promoText.ToCharArray());
                }
            }

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
            base.UnpackResponse();
        }

        #endregion
    }
}