using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    public class SetHallSettings : ServerMessage
    {
        #region member variables

        protected int m_hallId;
        protected string m_hallName;
        protected string m_endOfDay;
        protected decimal m_salesTax;
        protected bool m_printBarCode;
        #endregion

        #region constructor
        public SetHallSettings()
        {
            m_id = 25015;
        }
        #endregion

        #region Methods
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            //set Hall ID
            requestWriter.Write(m_hallId);
            requestWriter.Write((UInt16)m_hallName.Length);
            if (!string.IsNullOrEmpty(m_hallName))
            {
                requestWriter.Write(m_hallName.ToCharArray());
            }

            //set end of day
            requestWriter.Write((UInt16)m_endOfDay.Length);
            if (!string.IsNullOrEmpty(m_endOfDay))
            {
                requestWriter.Write(m_endOfDay.ToCharArray());
            }

            //Set print bar code
            requestWriter.Write((byte)(m_printBarCode ? 1 : 0));

            //Set Sales Tax
            requestWriter.Write((UInt16)m_salesTax.ToString().Length);
            if (!string.IsNullOrEmpty(m_salesTax.ToString()))
            {
                requestWriter.Write(m_salesTax.ToString().ToCharArray());
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

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            // Seek past return code.
            responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);
            
            responseReader.Close();
        }
        #endregion
        public int HallId
        {
            get { return m_hallId; }
            set { m_hallId = value; }
        }

        public string HallName
        {
            get { return m_hallName; }
            set { m_hallName = value; }
        }

        public string EndOfDay
        {
            get { return m_endOfDay; }
            set { m_endOfDay = value; }
        }

        public decimal SalesTax
        {
            get { return m_salesTax; }
            set { m_salesTax = value; }
        }

        public bool PrintBarCode
        {
            get { return m_printBarCode; }
            set { m_printBarCode = value; }
        }

    }
}
