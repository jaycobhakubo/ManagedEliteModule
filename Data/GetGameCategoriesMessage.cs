// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.
using System;
using System.IO;
using System.Text;


namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a message to request the list of game categories from the server
    /// </summary>
    public class GetGameCategoriesMessage : ServerMessage
    {
        private const int MinResponseMessageLength = 6;

        // Properties
        public GameCategory[] GameCategoryList { get; private set; }

        public GetGameCategoriesMessage()
		{
		    GameCategoryList = null;
		    m_id = 18071; // Message ID
			m_strMessageName = "Get Game Categories";
		}

        public static GameCategory[] GetArray()
        {
            var msg = new GetGameCategoriesMessage();
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("GetGameCategoryMessage: " + ex.Message);
            }
            return msg.GameCategoryList;
        }

		protected override void PackRequest()
        {
            // Create the streams we will be writing to
            MemoryStream requestStream = new MemoryStream();
            BinaryWriter requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Request Params (none)

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

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException(m_strMessageName);

            // Try to unpack the data.
            try
            {
                // Seek past return code
                responseReader.BaseStream.Seek( sizeof(Int32), SeekOrigin.Begin);

				// Get the category count
				Int16 wCount = responseReader.ReadInt16();

				// Allocate the array
				GameCategoryList = new GameCategory[wCount];

				// Read the categories
                for (int i = 0; i < wCount; i++)
				{
                    GameCategoryList[i] = new GameCategory { Id = responseReader.ReadInt32() };
				    Int16 wStringLen = responseReader.ReadInt16();
					GameCategoryList[i].Name = new string(responseReader.ReadChars(wStringLen));

                    //US5328
				    GameCategoryList[i].MaxCardLimit = responseReader.ReadInt32();
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

            // Close the streams.
            responseReader.Close();
        }
    }
}
