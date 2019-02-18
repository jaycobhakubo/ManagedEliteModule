// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Text;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Set Package Item Message
    /// </summary>
    public class GetValidationPackagesMessage : ServerMessage
    {
        protected const int MinResponseMessageLength = 6;

        #region Constructors
        public GetValidationPackagesMessage()
        {
            m_id = 18219; // Get Default Validation Package Message
            ValidationPackages = new List<PackageItem>();
        }
        #endregion

        #region Member Methods
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

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

                var count = responseReader.ReadInt16();

                for (int i = 0; i < count; i++)
                {
                    var id = responseReader.ReadInt32();
                    var nameLength = responseReader.ReadUInt16();
                    var name = new string(responseReader.ReadChars(nameLength));
                    var isDefault = responseReader.ReadBoolean();

                    var package = new PackageItem
                    {
                        PackageId = id,
                        PackageName = name
                    };

                    ValidationPackages.Add(package);

                    if (isDefault)
                    {
                        DefaultValidationPackage = package;
                    }
                }
                        
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

        /// <summary>
        /// Gets or sets the validation packages.
        /// </summary>
        /// <value>
        /// The validation packages.
        /// </value>
        public List<PackageItem> ValidationPackages { get; private set; }

        public PackageItem DefaultValidationPackage { get; private set; }
        #endregion
    }
}
