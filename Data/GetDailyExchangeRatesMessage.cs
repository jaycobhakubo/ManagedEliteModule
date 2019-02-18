#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2010 GameTech
// International, Inc.
#endregion

// Rally TA7465

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Daily Exchange Rates server message.
    /// </summary>
    public class GetDailyExchangeRatesMessage : ServerMessage
    {
        #region Member Variables
        private Dictionary<string, decimal> m_rateList = new Dictionary<string, decimal>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetDailyExchangeRatesMessage
        /// class.
        /// </summary>
        public GetDailyExchangeRatesMessage()
        {
            m_id = 37003; // Get Daily Exchange Rates
            m_strMessageName = "Get Daily Exchange Rates";
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        protected override void UnpackResponse()
        {
            // Clear the previous values.
            AreRatesSet = false;
            AreRatesLocked = false;
            m_rateList.Clear();

            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Rates Set
                AreRatesSet = responseReader.ReadBoolean();

                // Rates Locked
                AreRatesLocked = responseReader.ReadBoolean();

                // Default Currency ISO
                ushort stringLen = responseReader.ReadUInt16();
                DefaultCurrencyISO = new string(responseReader.ReadChars(stringLen));

                // Rate Count
                ushort rateCount = responseReader.ReadUInt16();

                // Rates
                for(ushort x = 0; x < rateCount; x++)
                {
                    // Currency ISO
                    stringLen = responseReader.ReadUInt16();
                    string iso = new string(responseReader.ReadChars(stringLen));

                    if(!m_rateList.ContainsKey(iso))
                        m_rateList.Add(iso, 0);

                    // Exchange Rate
                    stringLen = responseReader.ReadUInt16();
                    string tempDec = new string(responseReader.ReadChars(stringLen));

                    if(!string.IsNullOrEmpty(tempDec))
                        m_rateList[iso] = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException(m_strMessageName, e);
            }
            catch(Exception e)
            {
                throw new ServerException(m_strMessageName, e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets whether rates have already been set at least one for the
        /// current gaming date.
        /// </summary>
        public bool AreRatesSet
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether the rates can be changed.
        /// </summary>
        public bool AreRatesLocked
        {
            get;
            private set;
        }

        /// <summary>
        /// The ISO code for the default currency.
        /// </summary>
        public string DefaultCurrencyISO
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a list of ISO code/rate value pairs.
        /// </summary>
        public IDictionary<string, decimal> Rates
        {
            get
            {
                return m_rateList;
            }
        }
        #endregion
    }
}