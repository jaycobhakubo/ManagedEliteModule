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
    public class FinalizeCardPositionMapMessage : ServerMessage
    {
        #region Private Members
        int m_mapId;
        #endregion

        #region Public Properties
        #endregion

        public FinalizeCardPositionMapMessage(int mapId)
        {
            m_id = 6101;
            m_strMessageName = "Finalize Card Position Map";
            m_mapId = mapId;
        }

        #region Member Methods

        public static void FinalizeCardPositionMap(int mapId)
        {
            var msg = new FinalizeCardPositionMapMessage(mapId);
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

                // Set the bytes to be sent.
                m_requestPayload = requestStream.ToArray();

                // Close the streams.
                requestWriter.Close();
            }
        }
        #endregion
    }

}
