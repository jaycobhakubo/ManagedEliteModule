// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Text;
namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Set Package Item Message
    /// </summary>
    public class GetValidationReceiptStatusMessage : ServerMessage
    {
        protected const int MinResponseMessageLength = 6;

        #region Constructors
        public GetValidationReceiptStatusMessage(int receiptId)
        {
            m_id = 18220; // Get Default Validation Package Message
            RegisterReceiptId = receiptId;
        }
        #endregion

        #region Member Methods

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            //write the operator ID
            requestWriter.Write(RegisterReceiptId);

            // Set the bytes to be sent.
            m_requestPayload = requestStream.ToArray();

            // Close the streams.
            requestWriter.Close();
        }

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            var responseStream = new MemoryStream(m_responsePayload);
            var responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Set Package Item");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                IsPaperValidated = responseReader.ReadBoolean();
                IsElectronicValidated = responseReader.ReadBoolean();
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Set Package Item", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Set Package Item", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties

        public bool IsPaperValidated { get; private set; }

        public bool IsElectronicValidated { get; private set; }

        public int RegisterReceiptId{ get; private set; }
        #endregion

    }
}
