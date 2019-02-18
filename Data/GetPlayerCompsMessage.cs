// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 Fortunet


//US4852: Product Center > Coupons: Require spend
//DE13275: Error found in US4932: Product Center > Coupons: Exclude packages from qualifying spend are included in the spend

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents the Get Player Comps server message.
    /// </summary>
    public class GetPlayerCompsMessage : ServerMessage
    {
        #region Constants and Data Types
        protected const int MinResponseMessageLength = 6;
        #endregion

        #region Member Variables
        protected int m_playerId = 0;
        protected List<PlayerComp> m_comps = null;
        protected bool m_splitMultiPackageCouponsIntoIndividualCoupons = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GetPlayerCompsMessage class.
        /// </summary>
        public GetPlayerCompsMessage()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetPlayerCompsMessage class
        /// with the specified player & operator ids.
        /// </summary>
        /// <param name="playerId">The id of the player to get comps 
        /// for.</param>
        /// <param name="operatorId">The id of the operator to whom 
        /// the player belongs.</param>
        public GetPlayerCompsMessage(int playerId)
        {
            m_id = 18033; // Get Player Comps
            m_playerId = playerId;
            m_comps = new List<PlayerComp>();
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

            // Player Id
            requestWriter.Write(m_playerId);

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
            base.UnpackResponse();

            // Create the streams we will be reading from.
            MemoryStream responseStream = new MemoryStream(m_responsePayload);
            BinaryReader responseReader = new BinaryReader(responseStream, Encoding.Unicode);

            // Check the response length.
            if(responseStream.Length < MinResponseMessageLength)
                throw new MessageWrongSizeException("Get Player Comp List");

            // Try to unpack the data.
            try
            {
                // Seek past return code.
                responseReader.BaseStream.Seek(sizeof(int), SeekOrigin.Begin);

                // Get the count of comps.
                ushort compCount = responseReader.ReadUInt16();

                // Clear the comps array.
                m_comps.Clear();

                // Setup our test container.
                string stemp = string.Empty;

                // Read all the comps.
                for(ushort x = 0; x < compCount; x++)
                {
                    PlayerComp comp = new PlayerComp();

                    // Comp Id
                    comp.Id = responseReader.ReadInt32();
                    
                    // Comp Award Id
                    comp.CompAwardId = responseReader.ReadInt32();
                    
                    // Comp Name
                    comp.Name = ReadString(responseReader);
                    
                    // Comp Expire Date
                    stemp = ReadString(responseReader);
                    
                    if (stemp != string.Empty)
                        comp.EndDate = DateTime.Parse(stemp, CultureInfo.InvariantCulture);
                    
                    // Comp Awarded Date
                    stemp = ReadString(responseReader);
                    
                    // Comp Value (0 if none).
                    stemp = ReadString(responseReader);
                    
                    if (stemp != string.Empty)
                        comp.Value = decimal.Parse(stemp, CultureInfo.InvariantCulture);
                    
                    // Remaining Comps
                    comp.RemainingComp = responseReader.ReadInt32();
                    
                    // Comp Type Id
                    comp.CouponType = (PlayerComp.CouponTypes)responseReader.ReadInt32();

                    if (comp.CouponType == PlayerComp.CouponTypes.PercentPackage)
                        comp.PercentDiscount = comp.Value / 100M;
                    else
                        comp.PercentDiscount = 0;

                    //Minimum spend to qualify //US4852
                    comp.MinimumSpendToQualify = decimal.Parse(ReadString(responseReader), CultureInfo.InvariantCulture);

                    //Restricted products (do not count these products toward any minimum spend).
                    //count of restricted products //US4852
                    var count = responseReader.ReadInt16();
                    
                    for (var i = 0; i < count; i++)
                    {
                        //restricted product ID //US4852
                        var restrictedProductId = responseReader.ReadInt32();
                    
                        comp.RestrictedProductIds.Add(restrictedProductId);
                    }

                    //Qualifying packages (list of packages attached to this coupon).
                    //count of packages //US4852
                    count = responseReader.ReadInt16();

                    for (var i = 0; i < count; i++)
                    {
                        var PackageId = responseReader.ReadInt32();

                        comp.EarnedPackageIDs.Add(PackageId);
                    }

                    //Restricted packages (do not count these packages toward any minimum spend).
                    //count of restricted products //US4852
                    count = responseReader.ReadInt16();

                    for (var i = 0; i < count; i++)
                    {
                        var restrictedPackageId = responseReader.ReadInt32();

                        //DE13275
                        comp.RestrictedPackageIds.Add(restrictedPackageId);
                    }

                    //Ignore validations on restricted packages
                    comp.IgnoreValidationsForIgnoredPackages = responseReader.ReadByte() != 0;

                    //split multiple package coupons into multiple individual coupons
                    if (comp.EarnedPackageIDs.Count > 1 && SplitMultiPackageCoupons)
                    {
                        PlayerComp splitComp;

                        for (int i = 0; i < comp.EarnedPackageIDs.Count; i++)
                        {
                            splitComp = new PlayerComp(comp);
                            splitComp.IsPartOfMultiPackageCoupon = true;
                            splitComp.PackageID = comp.EarnedPackageIDs[i];

                            m_comps.Add(splitComp);
                        }
                    }
                    else
                    {
                        if (comp.EarnedPackageIDs.Count == 1)
                            comp.PackageID = comp.EarnedPackageIDs[0];

                        m_comps.Add(comp);
                    }
                }
            }
            catch(EndOfStreamException e)
            {
                throw new MessageWrongSizeException("Get Player Comp List", e);
            }
            catch(Exception e)
            {
                throw new ServerException("Get Player Comp List", e);
            }

            // Close the streams.
            responseReader.Close();
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the player who's comps to get.
        /// </summary>
        public int PlayerId
        {
            get
            {
                return m_playerId;
            }
            set
            {
                m_playerId = value;
            }
        }

        /// <summary>
        /// Gets the array of comps received from the server.
        /// </summary>
        public PlayerComp[] Comps
        {
            get
            {
                return m_comps.ToArray();
            }
        }

        public bool SplitMultiPackageCoupons
        {
            get
            {
                return m_splitMultiPackageCouponsIntoIndividualCoupons;
            }

            set
            {
                m_splitMultiPackageCouponsIntoIndividualCoupons = value;
            }
        }

        #endregion
    }
}
