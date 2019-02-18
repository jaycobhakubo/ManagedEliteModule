#region Copyright
// This is an unpublished work protected under the copyright laws of the United
// States and other countries.  All rights reserved.  Should publication occur
// the following will apply:  © 2010 GameTech International, Inc.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Staff Bank List server message.
    /// </summary>
    public class GetStaffBankListMessage : ServerMessage
    {
        #region Member Variables
        private readonly List<Bank> m_banks = new List<Bank>();
        private DateTime m_gamingDate;
        private readonly int m_staffId;
        #endregion

        #region Constructors
        public GetStaffBankListMessage(int staffId)
        {
            m_id = 37041;
            m_staffId = staffId;
            m_strMessageName = "Get Staff Bank List";
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

            // Staff id
            requestWriter.Write(m_staffId);

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
            // Clear existing data.
            m_banks.Clear();
            
            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Bank Count
                ushort bankCount = responseReader.ReadUInt16();

                // Get all the banks.
                for (ushort x = 0; x < bankCount; x++)
                {
                    Bank bank = new Bank
                    {
                        Type = BankType.Regular,
                        Id = responseReader.ReadInt32(),// Bank Id
                        Session = responseReader.ReadInt32()// Bank session
                    };

                    //gaming date
                    ushort stringLen = responseReader.ReadUInt16();
                    bank.GamingDate = DateTime.Parse(new string(responseReader.ReadChars(stringLen)));
                    
                    // Staff Name
                    stringLen = responseReader.ReadUInt16();
                    bank.Name = new string(responseReader.ReadChars(stringLen));

                    m_banks.Add(bank);
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

        #endregion

        #region Member Properties

        public IEnumerable<Bank> Banks
        {
            get
            {
                return m_banks;
            }
        }

        #endregion

    }
}
