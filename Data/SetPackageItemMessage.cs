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
    /// Represents a Set Package Item Message
    /// </summary>
    public class SetPackageItemMessage : ServerMessage
    {
        protected const int MinResponseMessageLength = 6;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SetPackageItemMessage class.
        /// </summary>
        public SetPackageItemMessage()
            : this(0, false, "", "", false, 0, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SetPackageItemMessage class.
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="chargeDeviceFee"></param>
        /// <param name="packageName"></param>
        /// <param name="receiptText"></param>
        public SetPackageItemMessage(int packageId, bool chargeDeviceFee, string packageName, string receiptText, bool overrideValidation, int validationQuantity, bool requiresValidation)
        {
            m_id = 18079; // Set Package Data
            PackageId = packageId;
            ChargeDeviceFee = chargeDeviceFee;
             PackageName = packageName;
            ReceiptText = receiptText;
            OverrideValidation = overrideValidation;
            ValidationQuantity = validationQuantity;
            RequiresValidation = requiresValidation;
        }
        #endregion

        #region Member Methods
        public static int SetPackage(int packageId, bool chargeDeviceFee, string packageName, string receiptText, bool overrideValidation, int validationQuantity, bool requiresValidation)
        {
            var msg = new SetPackageItemMessage(packageId, chargeDeviceFee, packageName, receiptText, overrideValidation, validationQuantity, requiresValidation);

            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("SetPackageItemMessage: " + ex.Message);
            }
            
            return msg.PackageId;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Package Id
            requestWriter.Write(PackageId);

            // Is ChargeDeviceFee
            requestWriter.Write(ChargeDeviceFee);

            // Package Name
            requestWriter.Write((ushort)PackageName.Length);
            requestWriter.Write(PackageName.ToCharArray());

            // Receipt Text
            requestWriter.Write((ushort)ReceiptText.Length);
            requestWriter.Write(ReceiptText.ToCharArray());

            //Override Validation
            requestWriter.Write(OverrideValidation);

            //Validation Quantity
            requestWriter.Write(ValidationQuantity);

            //Requires Validation to purchase
            requestWriter.Write(RequiresValidation);

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

                // Get the Package Item Id.
                PackageId = responseReader.ReadInt32();
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
        /// Gets or Sets the Package Id.
        /// </summary>
        public int PackageId { get; set; }

        /// <summary>
        /// Gets or Sets the Is Upsell flag.
        /// </summary>
        public bool ChargeDeviceFee { get; set; }

        /// <summary>
        /// Gets or Sets the Package Name.
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// Gets or Sets the Receipt Text.
        /// </summary>
        public string ReceiptText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [override validation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [override validation]; otherwise, <c>false</c>.
        /// </value>
        public bool OverrideValidation { get; set; }

        /// <summary>
        /// Gets or sets the validation quantity.
        /// </summary>
        /// <value>
        /// The validation quantity.
        /// </value>
        public int ValidationQuantity { get; set; }

        /// <summary>
        /// Gets or sets if the package requires validation mode in POS to be purchased.
        /// </summary>
        public bool RequiresValidation { get; set; }

        #endregion
    }
}
