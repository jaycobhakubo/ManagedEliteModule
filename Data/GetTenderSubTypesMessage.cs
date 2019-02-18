using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class GetTenderSubTypesMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;

        public class TenderSubTypeInfo
        {
            public int ID;
            public string description;
            public string displayName;

            public TenderSubTypeInfo()
            {
                ID = 0;
                description = string.Empty;
                displayName = string.Empty;
            }

            public TenderSubTypeInfo(int ID, string desc, string name)
            {
                this.ID = ID;
                this.description = desc;
                this.displayName = name;
            }
        }
        #endregion

        #region Member Variables
        protected List<TenderSubTypeInfo> m_subTypes;
        #endregion

        #region Constructor

        public GetTenderSubTypesMessage()
        {
            m_id = 37038;
            m_strMessageName = "Get Tender Sub-Types";
            m_subTypes = new List<TenderSubTypeInfo>();
        }

        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server
        /// </summary>
        protected override void PackRequest()
        {
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
                throw new MessageWrongSizeException("Get Tender Sub-Types");

            try
            {
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                int ttCount = responseReader.ReadInt16();
                int stringLen;

                for (int i = 0; i < ttCount; ++i)
                {
                    TenderSubTypeInfo newValue = new TenderSubTypeInfo();

                    newValue.ID = responseReader.ReadInt32();

                    stringLen = responseReader.ReadUInt16();
                    newValue.description = new string(responseReader.ReadChars(stringLen));

                    stringLen = responseReader.ReadUInt16();
                    newValue.displayName = new string(responseReader.ReadChars(stringLen));

                    m_subTypes.Add(newValue);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Tender Sub-Types", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Tender Sub-Types", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        #endregion

        #region Member Properties

        public List<TenderSubTypeInfo> TenderSubTypes
        {
            get { return m_subTypes; }
        }

        #endregion
    }
}
