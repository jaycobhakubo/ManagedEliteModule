// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Player Comps server message.
    /// </summary>
    public class RedeemCompMessage : ServerMessage 
    {
        #region Member Variables
        protected int m_playerId = 0;
        protected int m_compAwardId = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RedeemCompMessage class.
        /// </summary>
        public RedeemCompMessage()
            : this(0,0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RedeemCompMessage class
        /// with the specified player & comp. award ids.
        /// </summary>
        /// <param name="playerId">The id of the player who is 
        /// redeeming.</param>
        /// <param name="compAwardId">The id of the comp the player 
        /// is redeeming.</param>
        public RedeemCompMessage(int playerId, int compAwardId)
        {
            m_id = 18034; // Get Player Comps
            m_playerId = playerId;
            m_compAwardId = compAwardId;
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

            // Player Id (4Bytes)
            requestWriter.Write(m_playerId);

            // Comp Award Id (4Bytes)
            requestWriter.Write(m_compAwardId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }
        #endregion
    }
}
