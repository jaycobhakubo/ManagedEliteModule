#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2008 GameTech
// International, Inc.
#endregion

// PDTS 1098

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// The possible return codes from Is Linked Bingo Hall Module Running 
    /// server message.
    /// </summary>
    public enum IsLinkedBingoOnlineReturnCode
    {
        Online = 1
    }

    /// <summary>
    /// Represents the Is Linked Bingo Hall Module Running server message.
    /// </summary>
    public class IsLinkedBingoOnlineMessge : ServerMessage
    {
        #region Member Variables
        protected bool m_isOnline;
        protected string m_version;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the IsLinkedBingoOnlineMessge class.
        /// </summary>
        public IsLinkedBingoOnlineMessge()
        {
            m_id = 28000; // Is Linked Bingo Hall Module Running
            m_strMessageName = "Is Linked Bingo Hall Module Running";
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        protected override void UnpackResponse()
        {
            m_isOnline = false;
            m_version = string.Empty;

            try
            {
                base.UnpackResponse();
            }
            catch(ServerException e)
            {
                if(e.ReturnCode != GTIServerReturnCode.MsgHandlerNotFound &&
                   (IsLinkedBingoOnlineReturnCode)e.ReturnCode != IsLinkedBingoOnlineReturnCode.Online)
                    throw;
            }

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                if(ServerReturnCode != GTIServerReturnCode.MsgHandlerNotFound)
                {
                    // Version of the Linked Bingo Hall Module.
                    ushort stringLen = responseReader.ReadUInt16();
                    string tempStr = new string(responseReader.ReadChars(stringLen));

                    if(!string.IsNullOrEmpty(tempStr))
                    {
                        m_isOnline = true;
                        m_version = tempStr;
                    }
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

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets whether the linked bingo module is installed and online.
        /// </summary>
        public bool IsOnline
        {
            get
            {
                return m_isOnline;
            }
        }

        /// <summary>
        /// Gets the version number of the module if it is online, otherwise 
        /// an empty string.
        /// </summary>
        public string Version
        {
            get
            {
                return m_version;
            }
        }
        #endregion
    }
}