// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared.Data
{
    /// <summary>
    /// Represents a Card Level returned from the server.
    /// </summary>
    public struct CardLevelItem
    {
        public int CardLevelId;
        public int LevelColor;
        public string Multiplier;
        public string CardLevelName;
        public bool IsActive;
        public List<string> PaperCardColors;// RALLY US4547

        public override string ToString()
        {
            return CardLevelName;
        }
    }

    /// <summary>
    /// Represents a Get Card Level message.
    /// </summary>
    public class GetCardLevelMessage : ServerMessage
    {
        protected const int MinResponseMessageLength = 6;
        public int CardLevelId { protected get; set; }
        public List<CardLevelItem> CardLevelList { get; protected set; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetCardLevelMessage 
        /// </summary>
        public GetCardLevelMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetCardLevelMessage 
        /// </summary>
        public GetCardLevelMessage(int cardLevelId)
        {
            m_id = 6031; // MessageId
            CardLevelId = cardLevelId;
            CardLevelList = new List<CardLevelItem>();
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns the list of card levels from the server that match the sent in card level ID. 
        ///   If zero is sent in, returns all the card levels.
        /// </summary>
        /// <param name="cardLevelId"></param>
        /// <returns></returns>
        public static List<CardLevelItem> CardLevels(int cardLevelId = 0)
        {
            var msg = new GetCardLevelMessage { CardLevelId = cardLevelId };
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("GetCardLevelMessage: " + ex.Message);
            }
            return msg.CardLevelList;
        }

        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Card Cut Id
            requestWriter.Write(CardLevelId);

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
            var responseStream = new MemoryStream(m_responsePayload);
            var responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Card Level");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count.
                var itemCount = responseReader.ReadUInt16();

                // Clear the array.
                CardLevelList.Clear();

                // Get all the items.
                for (ushort x = 0; x < itemCount; x++)
                {
                    var cardLevelListItem = new CardLevelItem
                    {
                        CardLevelId = responseReader.ReadInt32(),
                        LevelColor = responseReader.ReadInt32()
                    };
                    // Multiplier text
                    cardLevelListItem.Multiplier = ServerMessage.ReadString(responseReader);

                    // Card Level Name
                    cardLevelListItem.CardLevelName = ServerMessage.ReadString(responseReader);

                    cardLevelListItem.IsActive = responseReader.ReadBoolean();

                    CardLevelList.Add(cardLevelListItem);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Card Level", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Card Level", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }
}
