// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013-2016 FortuNet

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class GetPromoTextMessage : ServerMessage
    {
        #region Member Variables
        private List<PromoInfo> m_promoData = new List<PromoInfo>();
        private int m_operatorID = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes new instance of GetPromoTextMessage class
        public GetPromoTextMessage()
        {
            m_id = 18262; // Get Channel Data
        }
        #endregion
        
        #region Parameters
        
        //Get/Set the operator the text is for (0=current operator on server side).
        public int Operator
        {
            get
            {
                return m_operatorID;
            }

            set
            {
                m_operatorID = value;
            }
        }

        public List<PromoInfo> PromoData
        {
            get
            {
                return m_promoData;
            }
        }

        public string[] PromoDataForEditing
        {
            get
            {
                List<string> lines = new List<string>();
                int currentGroup = 0;

                foreach (PromoInfo pi in m_promoData)
                {
                    if (pi.promoGroupID != currentGroup)
                    {
                        string grp = string.Format("Group={0}", pi.promoGroupID);
                        currentGroup = pi.promoGroupID;
                        lines.Add(grp);
                    }

                    lines.Add(pi.promoText);
                }

                return lines.ToArray();
            }
        }
        #endregion

        #region Member Methods
        protected override void PackRequest()
        {
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write(m_operatorID);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                int count = responseReader.ReadInt32();

                m_promoData.Clear();

                for (int i = 0; i < count; i++)
                {
                    PromoInfo pi = new PromoInfo();

                    pi.promoGroupID = responseReader.ReadInt32();
                    pi.promoGroupLineNumber = responseReader.ReadUInt16();

                    ushort stringLen = responseReader.ReadUInt16();
                    if (stringLen > 0)
                        pi.promoText = new string(responseReader.ReadChars(stringLen));
                    else
                        pi.promoText = " ";

                    m_promoData.Add(pi);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Promo Text Message", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Promo Text Message", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion
    }
}