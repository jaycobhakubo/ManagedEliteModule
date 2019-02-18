// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Drawing;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Get Player Image server message.
    /// </summary>
    public class GetPlayerImageMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 8;
        #endregion

        #region Member Variables
        protected int m_playerId = 0;
        protected Bitmap m_image = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetPlayerImageMessage class.
        /// </summary>
        public GetPlayerImageMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetPlayerImageMessage class
        /// with the specified player id.
        /// </summary>
        /// <param name="playerId">The id of the player who's 
        /// picture to get.</param>
        public GetPlayerImageMessage(int playerId)
        {
            m_id = 8020; // Get Player Image
            m_playerId = playerId;
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

            // Player Id
            requestWriter.Write(m_playerId);

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
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Player Image");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Data Length
                int dataLength = responseReader.ReadInt32();

                // Image Data
                if(dataLength > 0)
                {
                    // Attempt to load it into a bitmap object.
                    MemoryStream picStream = new MemoryStream(responseReader.ReadBytes(dataLength));

                    // Rally US493
                    // Create a temporary copy of the image because we can't
                    // keep the picStream open for the lifetime of the image.
                    Bitmap tempBitmap = new Bitmap(picStream);
                    m_image = new Bitmap(tempBitmap);

                    tempBitmap.Dispose();
                    tempBitmap = null;
                    picStream.Close();
                    picStream.Dispose();
                    picStream = null;
                }
                else
                    m_image = null;
            }
            catch(EndOfStreamException e)
            {
                m_image = null;
                throw new MessageWrongSizeException("Get Player Image", e);
            }
            catch(Exception e)
            {
                m_image = null;
                throw new ServerException("Get Player Image", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the player who's picture to get.
        /// </summary>
        public int PlayerId
        {
            get
            {
                return m_playerId;
            }
            set
            {
                m_playerId = value;
            }
        }

        /// <summary>
        /// The player's picture retieved from the server or null if the 
        /// player has no picture.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return m_image;
            }
        }
        #endregion
    }
}
