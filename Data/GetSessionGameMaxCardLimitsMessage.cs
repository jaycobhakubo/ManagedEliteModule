using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.Logging;

namespace GTI.Modules.Shared.Data
{
    public class GetCategoryMaxCardLimitsPerGameMessage : ServerMessage
    {        
        #region Member Variables
        protected int m_sessionPlayedId;

        private readonly List<Tuple<int, GameCategory>> m_gameMaxCardLimit; 
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GetGameCardsMessage class 
        /// with the specified parameters.
        /// </summary>
        /// <param name="sessionPlayedId">The id of the session played who's 
        /// cards to return.</param>
        private GetCategoryMaxCardLimitsPerGameMessage(int sessionPlayedId)
        {
            m_id = 6085; // Get Game Category Max Card Limit Per Game
            m_sessionPlayedId = sessionPlayedId;
            m_gameMaxCardLimit = new List<Tuple<int, GameCategory>>();
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
            
            // Session Played Id
            requestWriter.Write(m_sessionPlayedId);

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

                // Get the count of games.
                ushort gameCount = responseReader.ReadUInt16();

                for (int i = 0; i < gameCount; i++)
                {
                    var sessionGamePlayedId = responseReader.ReadInt32();
                    
                    var gameCategory = new GameCategory
                    {
                        Id = responseReader.ReadInt32(),
                        Name = ReadString(responseReader),
                        MaxCardLimit = responseReader.ReadInt32()
                    };

                    m_gameMaxCardLimit.Add(new Tuple<int, GameCategory>(sessionGamePlayedId, gameCategory));
                }

            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Session Linear Game Numbers", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Session Linear Game Numbers", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        public static List<Tuple<int, GameCategory>> GetCategoryMaxCardLimitPerGame(int sessionPlayedId)
        {
            var message = new GetCategoryMaxCardLimitsPerGameMessage(sessionPlayedId);

            try
            {
                message.Send();
            }
            catch (Exception ex)
            {
                StackFrame frame = new StackFrame(1, true);
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                var error = string.Format("Error sending GetCategoryMaxCardLimitPerGame. {0}", ex.Message);
                Logger.LogWarning(error, fileName, lineNumber);

                throw new Exception(error);
            }

            return message.m_gameMaxCardLimit;
        }

        #endregion

    }
}
