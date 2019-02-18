using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class SetActiveTenderTypesMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 4;
        #endregion

        #region Member Variables
        protected int[] m_arrTenderIDs;
        protected byte[] m_arrTenderActives;
        #endregion

        #region Constructors

        public SetActiveTenderTypesMessage(int[] arrTenderIDs, byte[] arrTenderActives)
        {
            m_id = 37034;
            m_strMessageName = "Set Active Tender Types";
            m_arrTenderIDs = arrTenderIDs;
            m_arrTenderActives = arrTenderActives;
        }

        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server
        /// </summary>
        protected override void PackRequest()
        {
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Tender Type Count
            Int16 numTenders = (Int16)m_arrTenderIDs.Length;
            requestWriter.Write(numTenders);

            // Tender Type List
            for (short i = 0; i < numTenders; ++i)
            {
                // TenderType ID
                requestWriter.Write(m_arrTenderIDs[i]);
                // Active
                requestWriter.Write(m_arrTenderActives[i]);
            }

            // close the stream
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

        #endregion
    }
}
