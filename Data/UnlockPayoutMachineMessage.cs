// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2008 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents an Unlock Payout Machine server message.
    /// </summary>
    public class UnlockPayoutMachineMessage : ServerMessage
    {
        #region Member Variables

        protected int m_machineId;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UnlockPayoutMachineMessage class.
        /// </summary>
        /// <param name="machineId">The id of the machine to unlock.</param>
        public UnlockPayoutMachineMessage(int machineId)
        {
            m_id = 18126; // Unlock Payout Machine
            m_strMessageName = "Unlock Payout Machine";
            m_machineId = machineId;
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

            // Machine Id
            requestWriter.Write(m_machineId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the machine to unlock.
        /// </summary>
        public int MachineId
        {
            get
            {
                return m_machineId;
            }
            set
            {
                m_machineId = value;
            }
        }
        #endregion
    }
}
