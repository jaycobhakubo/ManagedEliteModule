using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GTI.Modules.Shared
{
    public class PlayerStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAlert { get; set; }
        public bool IsActive { get; set; }
        public bool Banned { get; set; }
        public string Banned_string { get; set; }
        public override string ToString() { return Name; }
    }

    public class GetPlayerStatusCode : ServerMessage
    {
        private int PlayerId { get; set;}
        private readonly List<PlayerStatus> playerStatusList;

        public GetPlayerStatusCode(int playerId)
        {
            m_id = 8026;
            PlayerId = playerId;
            playerStatusList = new List<PlayerStatus>();
        }

        #region Member Methods
        public static List<PlayerStatus> GetPlayerStatus(int playerId)
        {
            var msg = new GetPlayerStatusCode(playerId);
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("GetPlayerStatusCode: " + ex.Message);
            }
            return msg.playerStatusList;
        }
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write(PlayerId);

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

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count.
                var itemCount = responseReader.ReadUInt16();

                // Clear the array.
                playerStatusList.Clear();

                // Get all the items.
                for (ushort x = 0; x < itemCount; x++)
                {
                    var status = new PlayerStatus {Id = responseReader.ReadInt32()};
                    //  Name
                    var stringLen = responseReader.ReadUInt16();
                    status.Name = new string(responseReader.ReadChars(stringLen));

                    // Is Alert
                    status.IsAlert = responseReader.ReadBoolean();

                    // Banned
                    status.Banned = responseReader.ReadBoolean();

                    if (status.Banned == true)
                    {
                        status.Banned_string = "TRUE";
                    }
                    else
                    {
                        status.Banned_string = "FALSE";
                    }

                    playerStatusList.Add(status);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Player Status", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Player Status", e);
            }

            // Close the streams.
            responseReader.Close();
        }

       
        #endregion


    }
}
