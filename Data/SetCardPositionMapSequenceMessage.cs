#region Copyright
// This is an unpublished work protected under the copyright laws of the United
// States and other countries.  All rights reserved.  Should publication occur
// the following will apply:  © 2008-2018 GameTech International, Inc.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTI.Modules.Shared;
using System.IO;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared.Data
{
    public class SetCardPositionMapSequencesMessage : ServerMessage
    {
        private int m_mapId;
        private int m_firstSeqNum;
        private List<byte[]> m_sequences;
        #region Private Members
        #endregion

        #region Public Properties
        #endregion

        public SetCardPositionMapSequencesMessage(int mapId, int firstSeqNumber, List<byte[]> sequences)
        {
            m_id = 6100;
            m_strMessageName = "Set Card Position Map Sequence";
            m_mapId = mapId;
            m_firstSeqNum = firstSeqNumber;
            m_sequences = sequences;
        }

        #region Member Methods

        public static void SetCardPositionMapSequences(int mapId, int firstSeqNumber, List<byte[]> sequences)
        {
            var msg = new SetCardPositionMapSequencesMessage(mapId, firstSeqNumber, sequences);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception(msg.MessageName + " Message: " + ex.Message);
            }
        }

        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            using(var requestStream = new MemoryStream())
            using(var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode))
            {
                // Set the bytes to be sent.
                m_requestPayload = requestStream.ToArray();

                requestWriter.Write(m_mapId);
                requestWriter.Write(m_sequences.Count);
                requestWriter.Write(m_firstSeqNum);

                foreach(var seq in m_sequences)
                {
                    requestWriter.Write((byte)seq.Length);
                    requestWriter.Write(seq);
                }

                // Set the bytes to be sent.
                m_requestPayload = requestStream.ToArray();

                // Close the streams.
                requestWriter.Close();
            }
        }
        #endregion
    }

}
