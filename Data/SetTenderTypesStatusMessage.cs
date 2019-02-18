using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class SetTenderTypesStatusMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected List<TenderTypeValue> m_tenders;
        #endregion

        #region Constructor

        public SetTenderTypesStatusMessage()
        {
            m_id = 37034;
            m_strMessageName = "Set Tender Types Status";
            m_tenders = null;
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

            requestWriter.Write((ushort)m_tenders.Count);

            foreach (TenderTypeValue ttv in m_tenders)
            {
                requestWriter.Write((int)ttv.TenderTypeID);
                requestWriter.Write((byte)ttv.IsActive);
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

        public List<TenderTypeValue> TenderTypes
        {
            set { m_tenders = value; }
        }

        #endregion
    }
}
