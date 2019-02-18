using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace GTI.Modules.Shared
{
    public class SetPlayerMagCardPINMessage : ServerMessage
    {
        #region Member Variables
        protected int m_playerId;
        protected int m_PIN;
        #endregion

        public SetPlayerMagCardPINMessage(int playerId, int PIN)
        {
            m_id = 8040;
			m_playerId = playerId;
            m_PIN = PIN;
        }

        #region Member Methods

        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write(m_playerId);
            requestWriter.Write(m_PIN);

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
        }

        #endregion
    }
}
