// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2009 GameTech
// International, Inc.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    public class GetHallSettingsMessage : ServerMessage
    {
        #region member variables

        
        protected int m_hallID;
        protected string m_hallName;
        protected string m_centralServerName;
        protected int m_userDefinedId;
        protected string m_endOfDayTime;
        protected bool m_printBarCode;
        protected decimal m_salesTax;
        
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of GetHallSettingsMessage
        /// </summary>
        public GetHallSettingsMessage()
        {
            m_id = 25014;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

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
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);
                
                //HallID
                m_hallID = responseReader.ReadInt32();
                
                //HallName
                short stringLen = responseReader.ReadInt16();
                if (stringLen > 0)
                    m_hallName = new string(responseReader.ReadChars(stringLen));

                //Central Server Name
                stringLen = responseReader.ReadInt16();
                if (stringLen > 0)
                    m_centralServerName = new string(responseReader.ReadChars(stringLen));

                //User Defined ID
                m_userDefinedId = responseReader.ReadInt32();

                //EndOfDayTime
                stringLen = responseReader.ReadInt16();
                if (stringLen > 0)
                    m_endOfDayTime = new string(responseReader.ReadChars(stringLen));

                //PrintBarcode
                m_printBarCode = responseReader.ReadBoolean();

                //SalesTax
                stringLen = responseReader.ReadInt16();
               
                string tempDec = new string(responseReader.ReadChars(stringLen));
                if (tempDec != string.Empty)
                    m_salesTax = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Hall Settings", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Hall Settings", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// The ID for the hall
        /// </summary>
        public int HallId
        {
            get { return m_hallID; }
            set { m_hallID = value; }
        }
        /// <summary>
        /// The name of the Hall
        /// </summary>
        public string HallName
        {
            get { return m_hallName; }
            set { m_hallName = value; }
        }

        /// <summary>
        /// The end of the gaming day in string format
        /// </summary>
        public string EndOfDayTime
        {
            get { return m_endOfDayTime; }
            set { m_endOfDayTime = value; }
        }

        /// <summary>
        /// Enables the printing of bar codes
        /// </summary>
        public bool PrintBarCode
        {
            get { return m_printBarCode; }
            set { m_printBarCode = value; }
        }

        /// <summary>
        /// The sales tax the hall must charge
        /// </summary>
        public decimal SalesTax
        {
            get { return m_salesTax; }
            set { m_salesTax = value; }
        }

        public string CentralServerName
        {
            get { return m_centralServerName; }
            set { m_centralServerName = value; }
        }

        public int UserDefinedId
        {
            get { return m_userDefinedId; }
            set { m_userDefinedId = value; }
        }

        #endregion
    }
}
