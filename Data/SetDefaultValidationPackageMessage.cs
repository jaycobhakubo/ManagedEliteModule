// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Set Package Item Message
    /// </summary>
    public class SetDefaultValidationPackageMessage : ServerMessage
    {
        protected const int MinResponseMessageLength = 6;

        #region Constructors

        public SetDefaultValidationPackageMessage(int packageId)
        {
            m_id = 18218; // Set Default Validation Package Message
            PackageId = packageId;
        }
        #endregion

        #region Member Methods

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Package Id
            requestWriter.Write(PackageId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or Sets the Package Id.
        /// </summary>
        public int PackageId { get; set; }

        #endregion
    }
}
