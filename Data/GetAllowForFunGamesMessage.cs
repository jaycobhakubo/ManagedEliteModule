// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007-2016 GameTech and FortuNet Inc
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;

namespace GTI.Modules.Shared
{
    public class GetAllowForFunGamesMessage : ServerMessage
    {
        #region Member Variables
        private int m_deviceID = 17; //defaults to checking for TED-E
        private bool m_FunGamesAllowed;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes new instance of GetAllowForFunGamesMessage 
        /// </summary>
        public GetAllowForFunGamesMessage()
        {
            m_id = 18222;
        }

        /// <summary>
        /// Initializes new instance of GetAllowForFunGamesMessage with deviceID other than 17
        /// </summary>
        public GetAllowForFunGamesMessage(int deviceID)
        {
            m_deviceID = deviceID;
            m_id = 18222;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Location Id
            requestWriter.Write(m_deviceID);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

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

                m_FunGamesAllowed = (responseReader.ReadByte() == 1) ? true : false;
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Allow Fun Game Data", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Allow Fun Game Data", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        public bool funGamesAllowed
        {
            get 
            { 
                return m_FunGamesAllowed; 
            }
        }
        #endregion
    }
}