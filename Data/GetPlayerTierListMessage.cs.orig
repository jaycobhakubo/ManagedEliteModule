// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Player Tier List server message.
    /// </summary>
    public class GetPlayerTierListMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_operatorId = 0;
        protected List<PlayerLoyaltyTier> m_tiers = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetPlayerTierListMessage class.
        /// </summary>
        public GetPlayerTierListMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetPlayerTierListMessage class
        /// with the specified operator id.
        /// </summary>
        /// <param name="operatorId">The id of the operator to get 
        /// tiers for.</param>
        public GetPlayerTierListMessage(int operatorId)
        {
            m_id = 8018; // Get Player Tier List
            m_operatorId = operatorId;
            m_tiers = new List<PlayerLoyaltyTier>();
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

            // Operator Id
            requestWriter.Write(m_operatorId);

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

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Player Tier List");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of tiers.
                ushort tierCount = responseReader.ReadUInt16();

                // Clear the tiers array.
                m_tiers.Clear();

                // Read all the tiers
                for(ushort x = 0; x < tierCount; x++)
                {
                    PlayerLoyaltyTier tier = new PlayerLoyaltyTier();

                    // Tier Id
                    tier.Id = responseReader.ReadInt32();

                    // Tier Name
                    ushort stringLen = responseReader.ReadUInt16();
                    tier.Name = new string(responseReader.ReadChars(stringLen));

                    // Tier Level
                    tier.Level = responseReader.ReadInt32();

                    // Points Per Hour
                    stringLen = responseReader.ReadUInt16();
                    string tempDec = new string(responseReader.ReadChars(stringLen));

                    if(tempDec != string.Empty)
                        tier.PointsPerHour = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                    m_tiers.Add(tier);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Player Tier List", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Player Tier List", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the operator to get tiers for.
        /// </summary>
        public int OperatorId
        {
            get
            {
                return m_operatorId;
            }
            set
            {
                m_operatorId = value;
            }
        }

        /// <summary>
        /// Gets the player loyalty tiers received from the server.
        /// </summary>
        public PlayerLoyaltyTier[] Tiers
        {
            get
            {
                return m_tiers.ToArray();
            }
        }
        #endregion
    }
}
