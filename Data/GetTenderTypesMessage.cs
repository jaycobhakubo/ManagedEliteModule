using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    public class GetTenderTypesMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected List<TenderTypeValue> m_tenders;
        #endregion

        #region Constructor

        public GetTenderTypesMessage()
        {
            m_id = 37033;
            m_strMessageName = "Get Tender Types";
            m_tenders = new List<TenderTypeValue>();
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
                throw new MessageWrongSizeException("Get Tender Types");

            try
            {
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                int ttCount = responseReader.ReadInt16();
                int stringLen;

                for (int i = 0; i < ttCount; ++i)
                {
                    TenderTypeValue newValue = new TenderTypeValue();

                    newValue.TenderTypeID = responseReader.ReadInt16();
                    
                    stringLen = responseReader.ReadUInt16();
                    newValue.TenderName = new string(responseReader.ReadChars(stringLen));

                    newValue.IsActive = responseReader.ReadByte();

                    m_tenders.Add(newValue);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Tender Types", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Tender Types", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        #endregion

        #region Member Properties

        public List<TenderTypeValue> TenderTypes
        {
            get { return m_tenders; }
        }

        #endregion
    }
}
