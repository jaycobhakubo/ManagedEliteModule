using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace GTI.Modules.Shared
{
    public class GetPlayerReceipts : ServerMessage 
    {
        private int mPlayerID = 0, mOperatorID = 0;
        private DateTime mCurrentDate;
        protected const int MinResponseMessageLength = 6;
        
        private Dictionary<string, bool> mReceipts;
        public GetPlayerReceipts(int playerID, int operatorID, DateTime currentDate)
        {
            m_id = 8023;
            mPlayerID = playerID;
            mOperatorID = operatorID;
            mCurrentDate = currentDate;
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

            requestWriter.Write(mPlayerID);
            requestWriter.Write(mOperatorID);

            // current date
            string tempDate = mCurrentDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            requestWriter.Write((ushort)tempDate.Length);
            requestWriter.Write(tempDate.ToCharArray());

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
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Player List");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of players.
                ushort playerCount = responseReader.ReadUInt16();

                mReceipts = new Dictionary<string, bool>();

                // Get all the players
                for (ushort x = 0; x < playerCount; x++)
                {
                    var receiptNumber = responseReader.ReadInt32().ToString();
                    //US5591: player center presold receipts added flag
                    var isPresold = responseReader.ReadBoolean();

                    //check to make sure we don't add duplicates
                    if (!mReceipts.ContainsKey(receiptNumber))
                    {
                        mReceipts.Add(receiptNumber, isPresold);
                    }
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Player Receipt", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Player Receipt", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion
        public Dictionary<string, bool> Receipts
        {
            get { return mReceipts; }
        }
    }
}
