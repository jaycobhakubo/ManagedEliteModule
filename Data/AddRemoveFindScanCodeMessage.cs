// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2017 GameTech
// International, Inc.

using System;
using System.IO;
using System.Text;

namespace GTI.Modules.Shared
{
    public enum Operation
    {
        Remove = 0,
        Add = 1,
        Find = 2
    }

    public enum ItemType
    {
        Product = 0,
        Package = 1
    }

    /// <summary>
    /// Represents an Add/Remove Scan Code Message
    /// </summary>
    public class AddRemoveFindScanCodeMessage : ServerMessage
    {
        protected const int MinResponseMessageLength = 6;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SetPackageItemMessage class.
        /// </summary>
        public AddRemoveFindScanCodeMessage()
            : this(Operation.Add, ItemType.Product, 0, "")
        {
        }

        /// <summary>
        /// Initializes a new instance of the SetPackageItemMessage class.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="isPackage"></param>
        /// <param name="itemId"></param>
        /// <param name="scanCode"></param>
        public AddRemoveFindScanCodeMessage(Operation operation, ItemType itemType, int itemId, string scanCode)
        {
            m_id = 18249; // Add/Remove Scan Code
            OperationMode = operation;
            ItemIs = itemType;
            ItemID = itemId;
            ScanCode = scanCode;

            WasSuccessful = true;
            ItemUsingScanCodeID = 0;
            ItemUsingScanCodeType = ItemType.Product;
            ItemUsingScanCodeName = string.Empty;
        }

        #endregion

        #region Member Methods
        
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            requestWriter.Write((int)OperationMode);
            requestWriter.Write((int)ItemIs);
            requestWriter.Write(ItemID);
            WriteString(requestWriter, ScanCode);

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
                throw new MessageWrongSizeException("Add/Remove Scan Code");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                WasSuccessful = responseReader.ReadUInt16() == 0;
                ItemUsingScanCodeType = (ItemType)responseReader.ReadInt32();
                ItemUsingScanCodeID = responseReader.ReadInt32();

                UInt16 stringLength = responseReader.ReadUInt16();

                ItemUsingScanCodeName = new string(responseReader.ReadChars(stringLength));
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Add/Remove/Find Scan Code", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Add/Remove/Find Scan Code", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        
        /// <summary>
        /// Gets or sets the operation mode.
        /// </summary>
        public Operation OperationMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item type.
        /// </summary>
        public ItemType ItemIs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the Item ID.
        /// </summary>
        public int ItemID
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or Sets the scan code.
        /// </summary>
        public string ScanCode
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets if the operation succeeded.
        /// </summary>
        public bool WasSuccessful
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of item using the scan code.
        /// </summary>
        public ItemType ItemUsingScanCodeType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the item the scan code is already assigned to.
        /// </summary>
        public string ItemUsingScanCodeName { get; set; }

        /// <summary>
        /// Gets the ID of the item the scan code is already assigned to.
        /// </summary>
        public int ItemUsingScanCodeID
        {
            get;
            set;
        }

        #endregion
    }
}
