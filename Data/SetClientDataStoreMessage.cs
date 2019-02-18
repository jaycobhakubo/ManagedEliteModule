#region copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 Fortunet, Inc.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    /// <summary>
    /// Class to implement the message to set client data to the datastore.
    /// </summary>
    public class SetClientDataStoreMessage : ServerMessage
    {
        #region Fields

        int? m_operatorId;
        int? m_machineId;
        int? m_staffId;
        ClientDataStoreTypes m_clientDatastoreType;
        string m_data;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetClientDataStoreMessage class.
        /// </summary>
        public SetClientDataStoreMessage(ClientDataStoreTypes clientDatastoreType
            , int? operatorId
            , int? machineId
            , int? staffId
            , string dataPortion)
        {
            m_id = 18229; // Get Current Gaming Date

            m_operatorId = operatorId;
            m_machineId = machineId;
            m_staffId = staffId;
            m_clientDatastoreType = clientDatastoreType;
            m_data = dataPortion;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms, Encoding.Unicode))
            {
                bw.Write((int)m_clientDatastoreType);
                bw.Write(m_operatorId.HasValue ? m_operatorId.Value : (int)0);
                bw.Write(m_machineId.HasValue ? m_machineId.Value : (int)0);
                bw.Write(m_staffId.HasValue ? m_staffId.Value : (int)0);

                bw.Write(String.IsNullOrEmpty(m_data) ? (UInt16)0 : (UInt16)m_data.Length);
                if (!String.IsNullOrEmpty(m_data)) bw.Write(m_data.ToCharArray());

                m_requestPayload = ms.ToArray();
            }
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
