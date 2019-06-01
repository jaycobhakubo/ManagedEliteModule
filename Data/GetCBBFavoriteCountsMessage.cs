// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2009 GameTech
// International, Inc.

// Rally US507 - CBB Favorites

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Player Favorite CBB Numbers Counts server message.
    /// </summary>
    public class GetPlayerCBBFavoriteCountsMessage : ServerMessage
    {
        #region Member Variables
        protected int m_playerId;
        protected Dictionary<short, int> m_counts = new Dictionary<short, int>();
        protected List<string> m_favoriteNumbers = new List<string>();
        protected bool m_eraseFavorites = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the
        /// GetPlayerCBBFavoriteCountsMessage class with the specified player
        /// id.
        /// </summary>
        /// <param name="playerId">The id of the player whose counts to
        /// return.</param>
        public GetPlayerCBBFavoriteCountsMessage(int playerId)
        {
            m_id = 8033; // Get Player Favorite CBB Numbers Counts
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

            // Erase?
            requestWriter.Write(m_eraseFavorites);

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
            m_counts.Clear();

            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of numbers.
                ushort numberCount = responseReader.ReadUInt16();

                if (numberCount != 0)
                {
                    short numsRequired = 0;
                    ushort stringLen;
                    string CBBNumbers = "";

                    // Read all the favorites.
                    for (ushort x = 0; x < numberCount; x++)
                    {
                        // Numbers Required
                        numsRequired = responseReader.ReadByte();

                        try
                        {
                            m_counts[numsRequired] = m_counts[numsRequired] + 1;
                        }
                        catch (KeyNotFoundException)
                        {
                            m_counts[numsRequired] = 1;
                        }

                        // The numbers                    
                        stringLen = responseReader.ReadUInt16();
                        CBBNumbers = new string(responseReader.ReadChars(stringLen));

                        m_favoriteNumbers.Add(CBBNumbers);
                    }
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Player Favorite CBB Numbers Counts", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Player Favorite CBB Numbers Counts", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the player whose counts to return.
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
        /// Get/set if this message will erase all of the player's favorite CBB numbers.
        /// </summary>
        public bool EraseFavorites
        {
            get
            {
                return m_eraseFavorites;
            }

            set
            {
                m_eraseFavorites = value;
            }
        }

        /// <summary>
        /// Gets the count of favorites recieved from the server.
        /// </summary>
        public IDictionary<short, int> FavoriteCounts
        {
            get
            {
                return m_counts;
            }
        }

        //Gets a list of favorite card numbers in sorted order
        public List<string> FavoriteNumbers
        {
            get
            {
                return m_favoriteNumbers;
            }
        }

        #endregion
    }
}