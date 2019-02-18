// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{

    public class GetPackageItemMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Properties
        public int PackageId { get; set; }
        public List<PackageItem> PackageItems { get; protected set; }
        public PackageItem[] PackageArray
        {
            get { return PackageItems.ToArray(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetPackageItemMessage class.
        /// </summary>
        public GetPackageItemMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetProductItemMessage class.
        /// </summary>
        /// <param name="packageId"></param>
        public GetPackageItemMessage(int packageId)
        {
            m_id = 18078;
            PackageId = packageId;
            PackageItems = new List<PackageItem>();
        }
        #endregion

        #region Member Methods
        public static List<PackageItem> GetPackageList(int packageId)
        {
            var msg = new GetPackageItemMessage(packageId);
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("GetPackageItemMessage: " + ex.Message);
            }
            return msg.PackageItems;
        }
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

        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            var responseStream = new MemoryStream(m_responsePayload);
            var responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if (responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Package Item");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of Packages.
                var packageItemCount = responseReader.ReadUInt16();

                // Clear the Package Item array.
                PackageItems.Clear();

                // Get all the Package Items
                for (ushort x = 0; x < packageItemCount; x++)
                {
                    var packageItem = new PackageItem
                                      {
                                          PackageId = responseReader.ReadInt32(), 
                                          ChargeDeviceFee = responseReader.ReadBoolean()
                                      };

                    // Package Name
                    var stringLen = responseReader.ReadUInt16();
                    packageItem.PackageName = new string(responseReader.ReadChars(stringLen));

                    // Receipt Text
                    stringLen = responseReader.ReadUInt16();
                    packageItem.ReceiptText = new string(responseReader.ReadChars(stringLen));

                    // Package Price
                    stringLen = responseReader.ReadUInt16();
                    packageItem.PackagePrice = new string(responseReader.ReadChars(stringLen));
                    
                    //read override
                    packageItem.OverrideValidation = responseReader.ReadBoolean();

                    //read override quantity
                    packageItem.ValidationQuantity = responseReader.ReadInt32();

                    //read if requires validation to purchase
                    packageItem.RequiresValidation = responseReader.ReadBoolean();

                    PackageItems.Add(packageItem);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Package Item", e);
            }
            catch (Exception e)
            {
                throw new ServerException("Get Package Item", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

    }
}
