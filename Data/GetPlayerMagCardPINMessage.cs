// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Player MagCard PIN server message.
    /// </summary>
    public class GetPlayerMagCardPINMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 8;       
        #endregion

        #region Member Variables
        protected int m_playerId;
        protected int m_PIN;
        protected string m_card;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetPlayerMagCardPINMessage class with no player.
        /// </summary>
        public GetPlayerMagCardPINMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetPlayerMagCardPINMessage class.
        /// </summary>
        /// <param name="playerId">The player ID for the player to work with.
        /// </param>
        public GetPlayerMagCardPINMessage(int playerId)
        {
            m_id = 8039; 
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

            // Player ID
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

            m_PIN = 0;

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Player Card PIN");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the PIN.
                m_PIN = responseReader.ReadInt32();
            
                int stringLen = responseReader.ReadUInt16();
                m_card = new string(responseReader.ReadChars(stringLen));
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Player Card PIN", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Player Card PIN", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the player ID to work with.
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
        /// Gets the PIN for the player card. 
        /// </summary>
        public int PlayerMagCardPIN
        {
            get
            {
                return m_PIN;
            }
        }

        /// <summary>
        /// Gets the player card. 
        /// </summary>
        public string PlayerMagCard
        {
            get
            {
                return m_card;
            }
        }

        #endregion
    }
}
