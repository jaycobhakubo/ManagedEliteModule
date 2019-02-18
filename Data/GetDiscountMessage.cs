#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 FortuNet, Inc.
#endregion

//US4323: (US4319) POS: Automatically award a discount
//  - added spend levels
//  - added restricted products
//US4321: (US4319) Discount based on quantity
//US4320: (US4319) Limit how many times a discount can be used.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared.Data
{
    public class GetDiscountMessage : ServerMessage
    {        
        #region Constants and Data Types

        protected const int MinResponseMessageLength = 6;
        private bool m_getDailyProductIds;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GetDiscountMessage class.
        /// </summary>
        /// <param name="discountId"></param>
        private GetDiscountMessage(int discountId, bool getDailyProductIds)
        {
            m_id = 18044;
            DiscountId = discountId;
            m_getDailyProductIds = getDailyProductIds;
            DiscountItems = new List<DiscountItem>();
        }

        #endregion

        #region Member Variables
        
        public int DiscountId { get; set; }
        
        public List<DiscountItem> DiscountItems { get; protected set; }

        #endregion
        
        #region Member Methods
        
        public static List<DiscountItem> GetDiscountList(int discountId = 0, bool getDailyProductIds = false)
        {
            var msg = new GetDiscountMessage(discountId, getDailyProductIds);
            try
            {
                msg.Send();
            }
            catch (ServerCommException ex)
            {
                throw new Exception("GetDiscountMessage: " + ex.Message);
            }
            return msg.DiscountItems;
        }

        protected override void PackRequest()
        {
            // Create the streams we will be writing to.
            var requestStream = new MemoryStream();
            var requestWriter = new BinaryWriter(requestStream, Encoding.Unicode);

            // Discount Id
            requestWriter.Write(DiscountId);

            //value to determine if we get the product ID's from 
            //ProductItem (false) table or DailyPackageProductID (true) table
            // Get daily product Ids
            requestWriter.Write(m_getDailyProductIds);

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
                throw new MessageWrongSizeException("Get Discount Item");

            decimal? tempDecimal = null;

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count.
                ushort itemCount = responseReader.ReadUInt16();

                // Clear the Item array.
                DiscountItems.Clear();

                // Get all the Items
                for (ushort x = 0; x < itemCount; x++)
                {
                    var discount = new DiscountItem();

                    //discount ID
                    discount.DiscountId = responseReader.ReadInt32();

                    //Discount Type Id
                    discount.Type = (DiscountType)responseReader.ReadInt32();

                    // Discount Amount
                    tempDecimal = ReadDecimal(responseReader);
                    if (tempDecimal.HasValue)
                        discount.DiscountAmount = tempDecimal.Value;
                    else
                        throw new ServerCommException("'Discount Amount' is not a valid decimal");

                    // Points Per Dollar
                    tempDecimal = ReadDecimal(responseReader);
                    if (tempDecimal.HasValue)
                        discount.PointsPerDollar = tempDecimal.Value;
                    else
                        throw new ServerCommException("'Points Per Dollar' is not a valid decimal");

                    // Discount Name
                    discount.DiscountName = ReadString(responseReader);

                    //discount award type ID
                    discount.DiscountAwardType = (DiscountItem.AwardTypes)responseReader.ReadInt32();

                    //requires player
                    discount.IsPlayerRequired = responseReader.ReadBoolean();

                    //Is Active
                    discount.IsActive = responseReader.ReadBoolean();

                    //Spend Level count
                    ushort spendLevelCount = responseReader.ReadUInt16();

                    //Read Spend level List
                    for (int i = 0; i < spendLevelCount; i++)
                    {
                        var spendLevel = new DiscountItem.SpendLevel();

                        //sequence number
                        spendLevel.Sequence = responseReader.ReadInt32();

                        //min value
                        tempDecimal = ReadDecimal(responseReader);
                        if (tempDecimal.HasValue)
                            spendLevel.SpendMinValue = tempDecimal.Value;
                        else
                            throw new ServerCommException(String.Format("'Spend level {0} min value' is not a valid decimal", i));

                        //max value
                        tempDecimal = ReadDecimal(responseReader);
                        if (tempDecimal.HasValue)
                            spendLevel.SpendMaxValue = tempDecimal.Value;
                        else
                            throw new ServerCommException(String.Format("'Spend level {0} max value' is not a valid decimal", i));

                        //spend value
                        tempDecimal = ReadDecimal(responseReader);
                        if (tempDecimal.HasValue)
                            spendLevel.SpendValue = tempDecimal.Value;
                        else
                            throw new ServerCommException(String.Format("'Spend level {0} spend value' is not a valid decimal", i));

                        discount.SpendLevels.Add(spendLevel);
                    }

                    //restricted product count
                    ushort restrictedProductCount = responseReader.ReadUInt16();

                    for (int i = 0; i < restrictedProductCount; i++)
                    {
                        //restricted Product Id
                        var productId = responseReader.ReadInt32();
                        discount.RestrictedProductIds.Add(productId);
                    }

                    // Start Date
                    if (responseReader.ReadBoolean())
                        discount.StartDate = ReadDateTime(responseReader);

                    // End Date
                    if (responseReader.ReadBoolean())
                        discount.EndDate = ReadDateTime(responseReader);

                    // Allow Partial Discounts
                    discount.AllowPartialDiscounts = responseReader.ReadBoolean();

                    // Maximum Discount
                    tempDecimal = ReadDecimal(responseReader);
                    if (tempDecimal.HasValue)
                        discount.MaximumDiscount = tempDecimal.Value;
                    //else
                    //    throw new ServerCommException("'Maximum Discount' is not a valid decimal"); // for testing

                    // Minimum Spend
                    tempDecimal = ReadDecimal(responseReader);
                    if (tempDecimal.HasValue)
                        discount.MinimumSpend = tempDecimal.Value;

                    discount.MinimumPacks = responseReader.ReadByte();
                    ushort qtyContributingPackCount = responseReader.ReadUInt16();

                    for(int i = 0; i < qtyContributingPackCount; i++)
                    {
                        var packageId = responseReader.ReadInt32();
                        discount.MinimumPacksEligibleIds.Add(packageId);
                    }

                    //else
                    //    throw new ServerCommException("'Minimum Spend' is not a valid decimal"); // for testing

                    // Schedule list RALLY US4617
                    ushort scheduleCount = responseReader.ReadUInt16();
                    for (int i = 0; i < scheduleCount; i++)
                    {
                        byte tmp = 0;
                        DiscountItem.Schedule sched = new DiscountItem.Schedule();
                        // Day of Week
                        tmp = responseReader.ReadByte();
                        if (tmp == 0)
                            sched.DayOfWeek = null;
                        else
                            sched.DayOfWeek = (DayOfWeek)(tmp - 1);
                        // Session Number
                        sched.SessionNumber = responseReader.ReadByte();
                        if (sched.SessionNumber.Value == 0)
                            sched.SessionNumber = null;

                        discount.DiscountSchedule.Add(sched);
                    }

                    //US4320 Maximum Use Per Session
                    discount.MaximumUsePerSession = responseReader.ReadByte();

                    // US4942 Ignore Validations for ignored packages
                    discount.IgnoreValidationsForIgnoredPackages = responseReader.ReadBoolean();

                    // US4942 Restricted Package Count
                    ushort packageCount = responseReader.ReadUInt16();
                    for (int i = 0; i < packageCount; i++)
                    {
                        //restricted Package Id
                        discount.RestrictedPackageIds.Add(responseReader.ReadInt32());
                    }

                    //US4321
                    //advance type
                    discount.AdvancedType = (DiscountItem.AdvanceDiscountType)responseReader.ReadInt32();

                    //US4321 buy quantity
                    discount.AdvancedQuantityDiscount.BuyQuantity = responseReader.ReadInt32();

                    //US4321 buy package ID
                    discount.AdvancedQuantityDiscount.BuyPackageId = responseReader.ReadInt32();

                    //US4321 get quantity
                    discount.AdvancedQuantityDiscount.GetQuantity = responseReader.ReadInt32();

                    //US4321 get package ID
                    discount.AdvancedQuantityDiscount.GetPackageId = responseReader.ReadInt32();

                    DiscountItems.Add(discount);
                }
            }
            catch (EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Discount Item", e);
            }
            catch (ServerCommException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new ServerException("Get Discount Item", e);
            }

            // Close the streams.
            responseReader.Close();
        }

        #endregion
    }
}
