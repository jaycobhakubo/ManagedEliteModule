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
    /// <summary>
    /// Represents a message that returns the list of bingo star card positions from the server
    /// </summary>
    public class GetCardPositionMapsMessage : ServerMessage
    {
        #region Private Members
        private List<CardPositionMapHandle> m_positionMaps;
        #endregion

        #region Public Properties

        public List<CardPositionMapHandle> PositionMaps
        {
            get { return m_positionMaps; }
        }
        #endregion

        public GetCardPositionMapsMessage()
        {
            m_id = 6096;
            m_strMessageName = "Get Card Position Maps";
            m_positionMaps = new List<CardPositionMapHandle>();
        }

        #region Member Methods

        /// <summary>
        /// Returns the list of star position maps
        /// </summary>
        /// <param name="gamingDate"></param>
        /// <returns></returns>
        public static List<CardPositionMapHandle> GetPositionMaps()
        {
            var msg = new GetCardPositionMapsMessage();
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception(msg.MessageName + " Message: " + ex.Message);
            }

            return msg.PositionMaps;
        }

        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            using (var requestStream = new MemoryStream())
            using (var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode))
            {
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
            using (var responseStream = new MemoryStream(m_responsePayload))
            using (var reader = new BinaryReader(responseStream, Encoding.Unicode))
            {
                // Try to unpack the data.
                try
                {
                    // Seek past return code.
                    reader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                    // Get the count of the star maps.
                    ushort count = reader.ReadUInt16();

                    // Clear the Product Item array.
                    m_positionMaps = new List<CardPositionMapHandle>(count);

                    for (ushort x = 0; x < count; x++)
                    {
                        var starMap = new CardPositionMapHandle();

                        starMap.Id = reader.ReadInt32();
                        starMap.PositionMapName = ReadString(reader);
                        starMap.IsActive = reader.ReadBoolean();
                        starMap.PositionsCovered = reader.ReadByte();
                        starMap.SequenceLength = reader.ReadByte();
                        starMap.NumSequences = reader.ReadInt32();
                        starMap.PositionMapGUID = ReadString(reader);
                        starMap.PositionMapPath = ReadString(reader);

                        m_positionMaps.Add(starMap);
                    }
                }
                catch (EndOfStreamException e)
                {
                    throw new MessageWrongSizeException(m_strMessageName, e);
                }
                catch (Exception e)
                {
                    throw new ServerException(m_strMessageName, e);
                }
            }
        }
        #endregion
    }
}
