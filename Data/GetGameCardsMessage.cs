// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.


//US4804: Linear game numbers in the Edge system.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using GTI.Modules.Shared;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Game Cards server message.
    /// </summary>
    public class GetGameCardsMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 8;
        #endregion

        #region Member Variables
        protected int m_registerReceiptId;
        protected int m_sessionPlayedId;
        protected bool m_getFaces;
        protected CardLevel[] m_levels;
        protected bool m_sameCards;
        protected bool m_consecutiveCards;
        protected List<BingoGame> m_games = new List<BingoGame>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetGameCardsMessage class.
        /// </summary>
        public GetGameCardsMessage()
            : this(0, 0, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetGameCardsMessage class 
        /// with the specified parameters.
        /// </summary>
        /// <param name="registerReceptId">The id of the sale who's cards to 
        /// return.</param>
        /// <param name="sessionPlayedId">The id of the session played who's 
        /// cards to return.</param>
        /// <param name="getFaces">Whether to return the card faces.</param>
        /// <param name="levels">An array of CardLevel objects that will be used 
        /// when creating cards.</param>
        public GetGameCardsMessage(int registerReceptId, int sessionPlayedId, bool getFaces, CardLevel[] levels)
        {
            m_id = 6020; // Get Game Cards
            m_registerReceiptId = registerReceptId;
            m_sessionPlayedId = sessionPlayedId;
            m_getFaces = getFaces;
            m_levels = levels;
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

            // Register Receipt Id
            requestWriter.Write(m_registerReceiptId);

            // Session Played Id
            requestWriter.Write(m_sessionPlayedId);

            // Get Card Faces
            requestWriter.Write(m_getFaces);

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
                throw new MessageWrongSizeException("Get Game Cards");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Same Cards
                m_sameCards = responseReader.ReadBoolean();

                // Consecutive Cards
                m_consecutiveCards = responseReader.ReadBoolean();

                // Get the count of games.
                ushort gameCount = responseReader.ReadUInt16();

                // Clear any existing games.
                m_games.Clear();

                // Read all the game data.
                for(ushort currentGame = 0; currentGame < gameCount; currentGame++)
                {
                    BingoGame game = new BingoGame();

                    // Game Number
                    game.LinearNumber = responseReader.ReadInt32();

                    // Display Game Number
                    game.DisplayNumber = responseReader.ReadInt32();

                    // Game Type Id
                    game.Type = (GameType)responseReader.ReadInt32();

                    // Set whether this game has consecutive cards.
                    game.ConsecutiveCards = m_consecutiveCards;

                    // Get the count of card types.
                    ushort cardTypeCount = responseReader.ReadUInt16();

                    for(ushort currentType = 0; currentType < cardTypeCount; currentType++)
                    {
                        // Card Type Id
                        CardType cardType = (CardType)responseReader.ReadInt32();

                        // Rally DE2312 - Selling CBB cards returns an error.
                        CardMedia mediaType = (CardMedia)responseReader.ReadInt32();

                        // Get the count of levels.
                        ushort levelCount = responseReader.ReadUInt16();

                        for(ushort currentLevel = 0; currentLevel < levelCount; currentLevel++)
                        {
                            // Card Level Id
                            int cardLevelId = responseReader.ReadInt32();

                            // Rally US505
                            CardLevel level = null;

                            if(game.Type != GameType.CrystalBall && game.Type != GameType.PickYurPlatter) // Rally TA6385
                            {
                                // Try to find the level in the array we have.
                                if(m_levels == null || m_levels.Length == 0)
                                    throw new ModuleException(Resources.UnknownCardLevel);

                                foreach(CardLevel lvl in m_levels)
                                {
                                    if(lvl.Id == cardLevelId)
                                    {
                                        level = lvl;
                                        break;
                                    }
                                }

                                // Rally US229 & US505
                                if(level == null)
                                    throw new ModuleException(Resources.UnknownCardLevel);
                            }

                            // Get the count of cards.
                            ushort cardCount = responseReader.ReadUInt16();

                            for(ushort currentCard = 0; currentCard < cardCount; currentCard++)
                            {
                                BingoCard card = null;

                                // Rally TA5749
                                // First Card
                                bool firstCard = responseReader.ReadBoolean();

                                // Card Number
                                int cardNum = responseReader.ReadInt32();

                                // PDTS 1098
                                // Rally DE2312
                                // Create the card.
                                card = BingoCardFactory.CreateBingoCard(game.Type, cardType, mediaType);
                                card.Number = cardNum;
                                card.Level = level;
                                card.IsStartingCard = firstCard;
                                // END: TA5749

                                // Rally US505
                                // Rally TA6385
                                if(m_getFaces || game.Type == GameType.CrystalBall || game.Type == GameType.PickYurPlatter)
                                {
                                    // Rally US498
                                    card.ParseFaceData(responseReader);
                                }

                                game.AddCard(card);
                            }
                        }
                    }

                    m_games.Add(game);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Game Cards", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Game Cards", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the register receipt id of the sale who's cards to 
        /// return.
        /// </summary>
        public int RegisterReceiptId
        {
            get
            {
                return m_registerReceiptId;
            }
            set
            {
                m_registerReceiptId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the session played who's cards to return.
        /// </summary>
        public int SessionPlayedId
        {
            get
            {
                return m_sessionPlayedId;
            }
            set
            {
                m_sessionPlayedId = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to return the card faces.
        /// </summary>
        public bool GetFaces
        {
            get
            {
                return m_getFaces;
            }
            set
            {
                m_getFaces = value;
            }
        }

        /// <summary>
        /// Gets whether these casds are the same for each game.
        /// </summary>
        public bool SameCards
        {
            get
            {
                return m_sameCards;
            }
        }

        /// <summary>
        /// Gets whether the card numbers are in order.
        /// </summary>
        public bool ConsecutiveCards
        {
            get
            {
                return m_consecutiveCards;
            }
        }

        /// <summary>
        /// Gets the games recieved from the server.
        /// </summary>
        public BingoGame[] Games
        {
            get
            {
                return m_games.ToArray();
            }
        }
        #endregion
    }
}
