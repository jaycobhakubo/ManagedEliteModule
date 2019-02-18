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
    /// Class to implement the message to get client data from the datastore.
    /// </summary>
    public class GetClientDataStoreMessage : ServerMessage
    {
        #region Fields

        int? m_operatorId;
        int? m_machineId;
        int? m_staffId;
        ClientDataStoreTypes m_clientDatastoreType;

        string m_responseData;

        #endregion

        #region Properties

        /// <summary>
        /// The data for this response.
        /// </summary>
        public string ResponseData { get; private set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetClientDataStoreMessage class.
        /// </summary>
        public GetClientDataStoreMessage(ClientDataStoreTypes clientDatastoreType
            , int? operatorId
            , int? machineId
            , int? staffId)
        {
            m_id = 18230; // Get Current Gaming Date

            m_operatorId = operatorId;
            m_machineId = machineId;
            m_staffId = staffId;
            m_clientDatastoreType = clientDatastoreType;

            m_responseData = String.Empty;
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

                m_requestPayload = ms.ToArray();
            }
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            using (MemoryStream ms = new MemoryStream(m_responsePayload))
            using (BinaryReader br = new BinaryReader(ms, Encoding.Unicode))
            {
                try
                {
                    // Seek past return code.
                    ms.Seek(sizeof(int), SeekOrigin.Begin);

                    // Read the data
                    ushort strLen = br.ReadUInt16();
                    ResponseData = new string(br.ReadChars(strLen));
                }
                catch (EndOfStreamException e)
                {
                    throw new MessageWrongSizeException("Get Client Data Store", e);
                }
                catch (Exception e)
                {
                    throw new ServerException("", e);
                }
            }
        }
        #endregion
    }
}
