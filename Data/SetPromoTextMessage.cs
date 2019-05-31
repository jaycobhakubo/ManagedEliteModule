// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013-2016 FortuNet


using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace GTI.Modules.Shared.Data
{
    public class PromoInfo
    {
        public int promoGroupID;
        public int promoGroupLineNumber;
        public string promoText;
    }

    public class SetPromoTextMessage : ServerMessage
    {
        #region Member variables and classes
        private int m_operatorID = 0;
        private List<PromoInfo> m_promoData = new List<PromoInfo>();
        private List<PromoInfo> m_originalPromoData = null;
        #endregion

        #region Member Properties

        public string[] PromoDataFromTextArray
        {
            set
            {
                m_promoData.Clear();

                int currentGroupID = 1;
                Dictionary<int, int> nextGroupLine = new Dictionary<int, int>();
                
                nextGroupLine.Add(1, 1);

                foreach(string s in value)
                {
                    if(s.StartsWith("Group=", StringComparison.CurrentCultureIgnoreCase) && s.Length > 6)
                    {
                        int groupWas = currentGroupID;
                        
                        if(!Int32.TryParse(s.Substring(6), out currentGroupID))
                            currentGroupID = groupWas;

                        if (!nextGroupLine.ContainsKey(currentGroupID))
                            nextGroupLine.Add(currentGroupID, 1);
                    }
                    else
                    {
                        PromoInfo pi = new PromoInfo();

                        pi.promoGroupID = currentGroupID;
                        nextGroupLine.TryGetValue(currentGroupID, out pi.promoGroupLineNumber);
                        nextGroupLine[currentGroupID] = pi.promoGroupLineNumber + 1;
                        pi.promoText = (s == string.Empty? " " : s);
                        m_promoData.Add(pi);
                    }
                }
            }
        }

        public List<PromoInfo> PromoData
        {
            get
            {
                return m_promoData;
            }

            set
            {
                m_promoData = value;
            }
        }

        public List<PromoInfo> OriginalPromoData
        {
            get
            {
                return m_originalPromoData;
            }

            set
            {
                m_originalPromoData = value;
            }
        }
        #endregion
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Set Promo Text message
        /// </summary>
        SetPromoTextMessage()
        {
            m_id = 18263; // Modify Channel Data Message
        }

        public SetPromoTextMessage(int operatorID, string[] promoData, List<PromoInfo> originalPromoData = null)
        {
            m_id = 18263; // Modify Channel Data Message
            PromoDataFromTextArray = promoData;
            OriginalPromoData = originalPromoData;
            m_operatorID = operatorID;
        }

        public SetPromoTextMessage(int operatorID, List<PromoInfo> promoData, List<PromoInfo> originalPromoData = null)
        {
            m_id = 18263; // Modify Channel Data Message
            m_promoData = promoData;
            OriginalPromoData = originalPromoData;
            m_operatorID = operatorID;
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

            requestWriter.Write(m_operatorID);

            Dictionary<string, object> orig = new Dictionary<string,object>();
            Dictionary<string, object> promo = new Dictionary<string,object>();

            if(m_originalPromoData != null)
            {
                foreach(PromoInfo pi in m_originalPromoData)
                    orig.Add(pi.promoGroupID.ToString()+","+pi.promoGroupLineNumber.ToString(), pi);
            }

            foreach(PromoInfo pi in m_promoData)
                promo.Add(pi.promoGroupID.ToString()+","+pi.promoGroupLineNumber.ToString(), pi);

            //lines to remove
            if (m_originalPromoData == null)
            {
                requestWriter.Write((int)0);
            }
            else
            {
                List<PromoInfo> remove = new List<PromoInfo>();

                foreach(KeyValuePair<string, object> o in orig)
                {
                    PromoInfo pi = (PromoInfo)o.Value;

                    if(!promo.ContainsKey(pi.promoGroupID.ToString()+","+pi.promoGroupLineNumber.ToString()))
                        remove.Add(pi);
                }

                requestWriter.Write((int)remove.Count);

                foreach (PromoInfo pi in remove)
                {
                    requestWriter.Write(pi.promoGroupID);
                    requestWriter.Write((ushort)pi.promoGroupLineNumber);
                }
            }

            //lines to add or update
            List<PromoInfo> addOrUpdate = new List<PromoInfo>();

            foreach (KeyValuePair<string, object> o in promo)
            {
                PromoInfo pi = (PromoInfo)o.Value;

                if (!orig.ContainsKey(pi.promoGroupID.ToString() + "," + pi.promoGroupLineNumber.ToString()) ||
                    ((PromoInfo)orig[pi.promoGroupID.ToString() + "," + pi.promoGroupLineNumber.ToString()]).promoText != pi.promoText)
                {
                    addOrUpdate.Add(pi);
                }
            }

            requestWriter.Write((int)addOrUpdate.Count);

            foreach(PromoInfo pi in addOrUpdate)
            {
                requestWriter.Write(pi.promoGroupID);
                requestWriter.Write((ushort)pi.promoGroupLineNumber);
                requestWriter.Write((ushort)pi.promoText.Length);
                requestWriter.Write(pi.promoText.ToCharArray());
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