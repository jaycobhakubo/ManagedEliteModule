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
    /// Represents a message that returns a list of star code info
    /// </summary>
    public class GetStarCodeInfoMessage : ServerMessage
    {
        #region Private Members
        private List<StarCodeInfo> m_codeInfo;
        #endregion

        #region Public Properties

        public List<StarCodeInfo> CodeInfo { get { return m_codeInfo; } }
        #endregion

        public GetStarCodeInfoMessage()
        {
            m_id = 6098;
            m_strMessageName = "Get Star Card Info";
            m_codeInfo = new List<StarCodeInfo>();
        }

        #region Member Methods

        /// <summary>
        /// Returns the list of star code info
        /// </summary>
        /// <returns></returns>
        public static List<StarCodeInfo> GetStarCodeInfo()
        {
            var msg = new GetStarCodeInfoMessage();
            try
            {
                msg.Send();
            }
            catch(ServerCommException ex)
            {
                throw new Exception(msg.MessageName + " Message: " + ex.Message);
            }

            return msg.CodeInfo;
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

                    // Get the count of the star maps.
                    ushort count = reader.ReadUInt16();

                    // Clear the Product Item array.
                    m_codeInfo = new List<StarCodeInfo>(count);

                    for(ushort x = 0; x < count; x++)
                    {
                        var ci = new StarCodeInfo();
                        ci.Code = reader.ReadByte();
                        ci.Name = ReadString(reader);
                        ci.Description = ReadString(reader);
                        m_codeInfo.Add(ci);
                    }
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

    public class StarCodeInfo
    {
        public byte Code
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
    }
}
