// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Credit server message.
    /// </summary>
    public class GetCreditMessage : ServerMessage
    {
        #region Member Variables
        protected int m_playerId;
        protected decimal m_refundableCredit;
        protected decimal m_nonRefundableCredit;
        protected decimal m_cashOnlyCredit;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetCreditMessage class.
        /// </summary>
        public GetCreditMessage() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetCreditMessage class with the 
        /// specified player id.
        /// </summary>
        /// <param name="playerId">The id of the player who's credit to 
        /// return.</param>
        public GetCreditMessage(int playerId)
        {
            m_id = 20001; // Get Credit
            m_playerId = playerId;
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

            // Player Id
            requestWriter.Write(m_playerId);

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

                // Refundable Credit
                ushort stringLen = responseReader.ReadUInt16();
                string tempDec = new string(responseReader.ReadChars(stringLen));

                if(tempDec != string.Empty)
                    m_refundableCredit = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                // Non-Refundable Credit
                stringLen = responseReader.ReadUInt16();
                tempDec = new string(responseReader.ReadChars(stringLen));

                if(tempDec != string.Empty)
                    m_nonRefundableCredit = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                // TTP 50114
                // Cash Only Credit
                stringLen = responseReader.ReadUInt16();
                tempDec = new string(responseReader.ReadChars(stringLen));

                if(tempDec != string.Empty)
                    m_cashOnlyCredit = decimal.Parse(tempDec, CultureInfo.InvariantCulture);
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Credit", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Credit", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the player who's credit to return.
        /// </summary>
        public int PlayerId
        {
            get
            {
                return m_playerId;
            }
            set
            {
                m_playerId = value;
            }
        }

        /// <summary>
        /// Gets the refundable credit balance for the player.
        /// </summary>
        public decimal RefundableCredit
        {
            get
            {
                return m_refundableCredit;
            }
        }

        /// <summary>
        /// Gets the non-refundable credit balance for the player.
        /// </summary>
        public decimal NonRefundableCredit
        {
            get
            {
                return m_nonRefundableCredit;
            }
        }

        // TTP 50114
        /// <summary>
        /// Gets the cash only credit balance for the player.
        /// </summary>
        public decimal CashOnlyCredit
        {
            get
            {
                return m_cashOnlyCredit;
            }
        }
        #endregion
    }
}
