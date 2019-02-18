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
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a UK Get Perm Def server message.
    /// </summary>
    public class UKGetPermDefMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected short m_tdm;
        protected short m_series;
        protected List<UKPerm> m_perms = new List<UKPerm>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UKGetPermDefMessage class.
        /// </summary>
        /// <param name="tdm">The TDM that specifies the series to be returned 
        /// or 0 for all TDMs.</param>
        /// <param name="series">The series to return or 0 for all series.</param>
        public UKGetPermDefMessage(short tdm, short series)
        {
            m_id =  32008;
            m_strMessageName = "UK Get Perm Def";
            m_tdm = tdm;
            m_series = series;
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

            // TDM
            requestWriter.Write(m_tdm);

            // Series
            requestWriter.Write(m_series);

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
            // Clear the perms.
            m_perms.Clear();

            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException(m_strMessageName);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Count of TDMs.
                ushort tdmCount = responseReader.ReadUInt16();

                for(ushort x = 0; x < tdmCount; x++)
                {
                    UKPerm perm = new UKPerm();

                    // TDM
                    perm.TDM = responseReader.ReadInt16();

                    // Series
                    perm.Series = responseReader.ReadInt16();

                    // Serial Start
                    perm.SerialStart = responseReader.ReadInt32();

                    // Serial End
                    perm.SerialEnd = responseReader.ReadInt32();

                    // Perm Start
                    perm.Start = responseReader.ReadInt32();

                    // Page Step
                    perm.PageStep = responseReader.ReadInt32();

                    // Game Type
                    perm.GameType = (GameType)responseReader.ReadInt32();

                    m_perms.Add(perm);
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
        /// Gets or sets the TDM that specifies the series to be returned or 0 
        /// for all TDMs.
        /// </summary>
        public short TDM
        {
            get
            {
                return m_tdm;
            }
            set
            {
                m_tdm = value;
            }
        }

        /// <summary>
        /// Gets or sets the series to return or 0 for all series.
        /// </summary>
        public short Series
        {
            get
            {
                return m_series;
            }
            set
            {
                m_series = value;
            }
        }

        /// <summary>
        /// Gets all perms retrieved from the server.
        /// </summary>
        public UKPerm[] Perms
        {
            get
            {
                return m_perms.ToArray();
            }
        }
        #endregion
    }
}
