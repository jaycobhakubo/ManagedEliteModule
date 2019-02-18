#region Copyright
// This is an unpublished work protected under the copyright laws of the United
// States and other countries.  All rights reserved.  Should publication occur
// the following will apply:  © 2008-2017 GameTech International, Inc.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTI.Modules.Shared;
using System.IO;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared.Data
{
    /// <summary>
    /// Represents a message that returns the sent in configured photo from the server
    /// </summary>
    public class GetProductItemsMessage : ServerMessage
    {
        #region Private Members
        private const int MinResponseMessageLength = 8;
        private int m_operatorID;
        private List<ProductItem> m_products;
        #endregion

        #region Public Properties

        public List<ProductItem> Packages
        {
            get { return m_products; }
        }
        #endregion

        public GetProductItemsMessage(int operatorId)
        {
            m_id = 18075;
            m_strMessageName = "Get Product Items";

            m_operatorID = operatorId;
            m_products = new List<ProductItem>();
        }

        #region Member Methods

        /// <summary>
        /// Returns the list of enabled product items
        /// </summary>
        /// <param name="gamingDate"></param>
        /// <returns></returns>
        public static List<ProductItem> GetProductItems(int id)
        {
            var msg = new GetProductItemsMessage(id);
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception(msg.MessageName + " Message: " + ex.Message);
            }

            return msg.Packages;
        }

        /// <summary>
        /// Prepares the request to be sent to the server.
        /// </summary>
        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            using (var requestStream = new MemoryStream())
            using (var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode))
            {
                // FIX : Per Dan 6/22 email (operator removed)
                // Operator Id
                //requestWriter.Write(m_operatorID);

                // Set the bytes to be sent.
                m_requestPayload = requestStream.ToArray();

                // Close the streams.
                requestWriter.Close();
            }
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        protected override void UnpackResponse()
        {
            base.UnpackResponse();

            // Create the streams we will be reading from.
            using (var responseStream = new MemoryStream(m_responsePayload))
            using (var reader = new BinaryReader(responseStream, Encoding.Unicode))
            {
                // Try to unpack the data.
                try
                {
                    // Seek past return code.
                    reader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                    // Get the count of Product Types.
                    var productItemCount = reader.ReadUInt16();

                    // Clear the Product Item array.
                    m_products = new List<ProductItem>();

                    // Get all the Product Items
                    for (ushort x = 0; x < productItemCount; x++)
                    {
                        var productItem = new ProductItem();

                        productItem.ProductItemId = reader.ReadInt32();

                        productItem.ProductTypeId = reader.ReadInt32();

                        productItem.SalesSourceId = reader.ReadInt32();

                        productItem.IsActive = reader.ReadBoolean();

                        // Product Item Name
                        productItem.ProductItemName = ReadString(reader);

                        // Product Type Name
                        productItem.ProductTypeName = ReadString(reader);

                        // Product Sales Source Name
                        productItem.ProductSalesSourceName = ReadString(reader);

                        // Product Group Id
                        productItem.ProductGroupId = reader.ReadInt32();

                        // Product Group Name
                        productItem.ProductGroupName = ReadString(reader);

                        //START RALLY TA 5744
                        // Paper Layout Id
                        productItem.PaperLayoutId = reader.ReadInt32();

                        // Paper Layout Name
                        productItem.PaperLayoutName = ReadString(reader);

                        // Paper Layout Count
                        productItem.PaperLayoutCount = reader.ReadInt32();
                        //END RALLY TA 5744

                        //START RALLY US1796
                        List<Accrual> accuralList = new List<Accrual>();
                        ushort accuralCount = reader.ReadUInt16();
                        for (ushort i = 0; i < accuralCount; i++)
                        {
                            Accrual accural = new Accrual();
                            accural.Id = reader.ReadInt32();
                            accural.Name = ReadString(reader);
                            accuralList.Add(accural);
                        }
                        productItem.AccuralList = accuralList;

                        // US2826
                        productItem.BarcodedPaper = reader.ReadBoolean();

                        //US4059 Perm File
                        productItem.PermFileId = reader.ReadInt32();

                        var cardColorSetId = reader.ReadInt32();

                        productItem.Validate = reader.ReadBoolean();

                        m_products.Add(productItem);
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
            }
        }
        #endregion
    }
}
