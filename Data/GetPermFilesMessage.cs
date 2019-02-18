#region Copyright
// This is an unpublished work protected under the copyright laws of the United
// States and other countries.  All rights reserved.  Should publication occur
// the following will apply:  © 2011 GameTech International, Inc.
#endregion

//US4059 Add ability to set the perm file for the product

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Perm Files server message.
    /// </summary>
    public class GetPermFilesMessage : ServerMessage
    {
        #region Member Variables
        private readonly List<KeyValuePair<int, string>> m_permFileList = new List<KeyValuePair<int, string>>();
        #endregion

        #region Contructors
        /// <summary>
        /// Initializes a new instance of the GetPermFilesMessage
        /// class.
        /// </summary>
        public GetPermFilesMessage()
        {
            m_id = 6070;
            m_strMessageName = "Get Manufacturer Perm Files";
        }

        #endregion

        #region Member Methods
        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        /// be used to write any request data necessary.
        protected override void PackRequest()
        {
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        /// be used to read any response data necessary.
        protected override void UnpackResponse()
        {
            base.UnpackResponse();
            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            m_permFileList.Clear();

            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                var permFileCount = responseReader.ReadUInt16();

                for (int i = 0; i < permFileCount; i++)
                {
                    //read in ID
                    var permFileId = responseReader.ReadInt32();

                    //read filename length
                    var filenameLength = responseReader.ReadUInt16();

                    //read in filename
                    var permFilename = new string(responseReader.ReadChars(filenameLength));

                    //add to permFileList
                    m_permFileList.Add(new KeyValuePair<int, string>(permFileId, permFilename));
                }
            }
            catch (Exception ex)
            {
                throw new ServerException(m_strMessageName, ex);
            }
        }

        private static int CompareByValue(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
        {
            return x.Value.CompareTo(y.Value);
        }
        #endregion

        #region Member Properties

        public List<KeyValuePair<int, string>> PermFiles
        {
            get
            {
                return m_permFileList;
            }
        }

        public static List<KeyValuePair<int, string>> GetList(bool sortByName = false)
        {
            var msg = new GetPermFilesMessage();
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("GetProductGroupMessage: " + ex.Message);
            }
            if(sortByName)
                msg.PermFiles.Sort(CompareByValue);
            return msg.PermFiles;
        }
        #endregion
    }
}
