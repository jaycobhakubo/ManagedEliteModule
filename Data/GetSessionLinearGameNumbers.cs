using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.Logging;

namespace GTI.Modules.Shared.Data
{
    public class GetSessionLinearGameNumbers : ServerMessage
    {        
        #region Member Variables
        protected int m_sessionPlayedId;

        private readonly List<BingoGame> m_linearGameList; 
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GetGameCardsMessage class 
        /// with the specified parameters.
        /// </summary>
        /// <param name="sessionPlayedId">The id of the session played who's 
        /// cards to return.</param>
        private GetSessionLinearGameNumbers(int sessionPlayedId)
        {
            m_id = 6081; // Get Session Linear Game Numbers
            m_sessionPlayedId = sessionPlayedId;
            m_linearGameList = new List<BingoGame>();
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
                    // Get the count of games.
                    int gameSequence = responseReader.ReadInt32();
                    
                    // Get the count of games.
                    int displayGameNumber = responseReader.ReadInt32();
                    
                    // Get the count of games.
                    int linearGameNumber = responseReader.ReadInt32();

                    // Get the count of games.
                    int continuationGameCount = responseReader.ReadInt32();

                    var bingoGame = new BingoGame()
                    {
                        LinearNumber = gameSequence,
                        DisplayNumber = displayGameNumber,
                        LinearDisplayNumber = linearGameNumber,
                        ContinuationGameCount = continuationGameCount
                    };

                    m_linearGameList.Add(bingoGame);
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

        public static List<BingoGame> GetLinearBingoGameList(int sessionPlayedId)
        {
            var message = new GetSessionLinearGameNumbers(sessionPlayedId);

            try
            {
                message.Send();
            }
            catch (Exception ex)
            {
                StackFrame frame = new StackFrame(1, true);
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                var error = string.Format("Error sending Get Session Linear Game Numbers. {0}", ex.Message);
                Logger.LogWarning(error, fileName, lineNumber);

                throw new Exception(error);
            }

            return message.m_linearGameList;
        }

        #endregion

    }
}
