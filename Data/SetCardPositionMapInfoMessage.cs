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
    public class SetCardPositionMapInfoMessage : ServerMessage
    {
        #region Private Members
        private CardPositionMapHandle m_updatedPositionMap;
        private CardPositionMapHandle m_pendingPositionMap;
        #endregion

        #region Public Properties

        public CardPositionMapHandle PendingPositionMap
        {
            get { return m_pendingPositionMap; }
        }

        public CardPositionMapHandle UpdatedPositionMap
        {
            get { return m_updatedPositionMap; }
        }
        #endregion

        public SetCardPositionMapInfoMessage(CardPositionMapHandle pendingInfo)
        {
            m_id = 6097;
            m_strMessageName = "Set Card Position Map Info";
            m_pendingPositionMap = pendingInfo;
            m_updatedPositionMap = null;
        }

        #region Member Methods

        public static CardPositionMapHandle SetCardPositionMapInfo(CardPositionMapHandle pendingInfo)
        {
            var msg = new SetCardPositionMapInfoMessage(pendingInfo);
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception(msg.MessageName + " Message: " + ex.Message);
            }

            return msg.UpdatedPositionMap;
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

                requestWriter.Write(m_pendingPositionMap.Id);

                var mapName = m_pendingPositionMap.PositionMapName ?? String.Format("[ID: {0}]", m_pendingPositionMap.Id);
                requestWriter.Write((ushort)mapName.Length);
                requestWriter.Write(mapName.ToCharArray());

                requestWriter.Write(m_pendingPositionMap.IsActive);
                requestWriter.Write(m_pendingPositionMap.PositionsCovered);
                requestWriter.Write(m_pendingPositionMap.SequenceLength);
                requestWriter.Write(m_pendingPositionMap.NumSequences);

                var guidStr = m_pendingPositionMap.PositionMapGUID ?? String.Empty;
                guidStr = guidStr.Replace("-", "");
                requestWriter.Write((ushort)guidStr.Length);
                requestWriter.Write(guidStr.ToCharArray());

                var pathStr = m_pendingPositionMap.PositionMapPath ?? String.Empty;
                requestWriter.Write((ushort)pathStr.Length);
                requestWriter.Write(pathStr.ToCharArray());

                // Set the bytes to be sent.
                m_requestPayload = requestStream.ToArray();

                // Close the streams.
                requestWriter.Close();
            }
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            using(var responseStream = new MemoryStream(m_responsePayload))
            using(var reader = new BinaryReader(responseStream, Encoding.Unicode))
            {
                // Try to unpack the data.
                try
                {
                    // Seek past return code.
                    reader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                    var cpm = new CardPositionMapHandle();
                    cpm.Id = reader.ReadInt32();
                    cpm.PositionMapName = ReadString(reader);
                    cpm.IsActive = reader.ReadBoolean();
                    cpm.PositionsCovered = reader.ReadByte();
                    cpm.SequenceLength = reader.ReadByte();
                    cpm.NumSequences = reader.ReadInt32();
                    cpm.PositionMapGUID = ReadString(reader);
                    cpm.PositionMapPath = ReadString(reader);

                    m_updatedPositionMap = cpm;

                }
                catch(EndOfStreamException e)
                {
                    throw new MessageWrongSizeException(m_strMessageName, e);
                }
                catch(Exception e)
                {
                    throw new ServerException(m_strMessageName, e);
                }
            }
        }
        #endregion
    }

}
