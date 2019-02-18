#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007-2010 GameTech
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
    /// Represents the Get Currency Definition List server message.
    /// </summary>
    public class GetCurrencyDefinitionListMessage : ServerMessage
    {
        #region Member Variables
        private List<Currency> m_currencies = new List<Currency>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetCurrencyDefinitionListMessage
        /// class.
        /// </summary>
        /// <param name="isoCurrencyCode">The three-character ISO 4217 currency
        /// code to return or blank for all currencies.</param>
        /// <param name="returnInactive">true to return inactive currencies;
        /// otherwise false.</param>
        public GetCurrencyDefinitionListMessage(string isoCurrencyCode, bool returnInactive)
        {
            m_id = 37001; // Get Currency Definition List
            m_strMessageName = "Get Currency Definition List";

            ISOCurrencyCode = isoCurrencyCode;
            ReturnInactive = returnInactive;
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

            // Currency ISO Code
            if(!string.IsNullOrEmpty(ISOCurrencyCode))
            {
                requestWriter.Write((ushort)ISOCurrencyCode.Length);
                requestWriter.Write(ISOCurrencyCode.ToCharArray());
            }
            else
                requestWriter.Write((ushort)0);

            // Show Inactive Currencies
            requestWriter.Write(ReturnInactive);

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
            // Clear the list.
            m_currencies.Clear();

            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of currencies.
                ushort currencyCount = responseReader.ReadUInt16();

                // Read all the currencies.
                for(ushort x = 0; x < currencyCount; x++)
                {
                    // Currency ISO
                    ushort stringLen = responseReader.ReadUInt16();
                    Currency currency = new Currency(new string(responseReader.ReadChars(stringLen)));

                    // Is Default
                    currency.IsDefault = responseReader.ReadBoolean();

                    // Is Active
                    currency.IsActive = responseReader.ReadBoolean();

                    // Denom Count
                    ushort denomCount = responseReader.ReadUInt16();

                    // Read all the denominations.
                    for(ushort y = 0; y < denomCount; y++)
                    {
                        Denomination denom = new Denomination();

                        // Denom Id
                        denom.Id = responseReader.ReadInt32();

                        // Denom Type
                        denom.Type = (DenominationType)Enum.Parse(typeof(DenominationType), responseReader.ReadInt32().ToString(CultureInfo.InvariantCulture));

                        // Allow Acceptor
                        denom.AllowAcceptor = responseReader.ReadBoolean();

                        // Is Active
                        denom.IsActive = responseReader.ReadBoolean();

                        // Denom Name
                        stringLen = responseReader.ReadUInt16();
                        denom.Name = new string(responseReader.ReadChars(stringLen));

                        // Denom Value
                        stringLen = responseReader.ReadUInt16();
                        string tempDec = new string(responseReader.ReadChars(stringLen));

                        if(!string.IsNullOrEmpty(tempDec))
                            denom.Value = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                        //Denom order US5380
                        denom.Order = responseReader.ReadInt16();

                        currency.AddDenomination(denom);
                    }

                    m_currencies.Add(currency);
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
        /// Gets or sets the three-character ISO 4217 currency code to return
        /// or blank for all currencies.
        /// </summary>
        public string ISOCurrencyCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to return inactive currencies.
        /// </summary>
        public bool ReturnInactive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of currencies returned from the server.
        /// </summary>
        public IEnumerable<Currency> Currencies
        {
            get
            {
                return m_currencies;
            }
        }
        #endregion
    }
    // END: TA7465
}
