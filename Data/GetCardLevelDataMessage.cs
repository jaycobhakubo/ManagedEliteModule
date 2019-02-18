// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Card Level Data server message.
    /// </summary>
    public class GetCardLevelDataMessage : ServerMessage
    {
        #region Member Variables
        protected int m_levelId = 0;
        protected List<CardLevel> m_levels = new List<CardLevel>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetCardLevelDataMessage class.
        /// </summary>
        public GetCardLevelDataMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetCardLevelDataMessage class 
        /// with the specified level id.
        /// </summary>
        /// <param name="playerId">The id of the level to return or 0 for all 
        /// levels.</param>
        public GetCardLevelDataMessage(int levelId)
        {
            m_id = 6031; // Get Card Level Data
            m_levelId = levelId;
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

            // Level Id
            requestWriter.Write(m_levelId);

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

                // Get the count of levels.
                ushort levelCount = responseReader.ReadUInt16();

                // Clear the level array.
                m_levels.Clear();

                // Read all the levels.
                for(ushort x = 0; x < levelCount; x++)
                {
                    CardLevel level = new CardLevel();

                    // Level Id
                    level.Id = responseReader.ReadInt32();

                    // Color
                    level.Color = Color.FromArgb(responseReader.ReadInt32());

                    // Multiplier
                    ushort stringLen = responseReader.ReadUInt16();
                    string tempDec = new string(responseReader.ReadChars(stringLen));

                    if(!string.IsNullOrEmpty(tempDec))
                        level.Multiplier = decimal.Parse(tempDec, CultureInfo.InvariantCulture);

                    // Name
                    stringLen = responseReader.ReadUInt16();
                    level.Name = new string(responseReader.ReadChars(stringLen));

                    // Rally US611
                    // Is Active
                    level.IsActive = responseReader.ReadBoolean();

                    m_levels.Add(level);
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Card Level Data", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Card Level Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the level to return or 0 for all levels.
        /// </summary>
        public int LevelId
        {
            get
            {
                return m_levelId;
            }
            set
            {
                m_levelId = value;
            }
        }

        /// <summary>
        /// Gets a list of levels recieved from the server.
        /// </summary>
        public CardLevel[] Levels
        {
            get
            {
                return m_levels.ToArray();
            }
        }
        #endregion
    }
}
