#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 FortuNet, Inc.
#endregion

//US4323: (US4319) POS: Automatically award a discount
//  - added spend levels
//  - added restricted products
//US4321: (US4319) Discount based on quantity
//4320: Limit how many times a discount can be used

using System.Collections.Generic;
using System;

namespace GTI.Modules.Shared.Business
{
    public class DiscountItem
    {
        #region Contructors
        public DiscountItem()
        {
            SpendLevels = new List<SpendLevel>();
            RestrictedProductIds = new List<int>();
            DiscountSchedule = new List<Schedule>();
            AdvancedQuantityDiscount = new QuantityDiscount
            {
                BuyQuantity = 1,
                GetQuantity = 1
            };
            RestrictedPackageIds = new List<int>();
            MinimumPacksEligibleIds = new List<int>();
        }
        #endregion

        #region Enums

        public enum AdvanceDiscountType
        {
            None = 0,
            Quantity = 1,
            SpendLevel = 2,
        }
        public enum AwardTypes
        {
            Manual = 1,
            Automatic = 2
        }
        
        #endregion

        #region Properties

        public int DiscountId;
        public DiscountType Type;
        public AdvanceDiscountType AdvancedType;
        public decimal DiscountAmount;
        public decimal PointsPerDollar;
        public bool IsActive;
        public string DiscountName;
        public AwardTypes DiscountAwardType; 
        public bool IsPlayerRequired;
        public List<SpendLevel> SpendLevels;
        public List<int> RestrictedProductIds;
        /// <summary>
        /// The first date the discount is able to be used. (inclusive)
        /// </summary>
        public DateTime? StartDate;
        /// <summary>
        /// The date the discount is no longer able to be used. (exclusive)
        /// </summary>
        public DateTime? EndDate;
        public decimal MaximumDiscount;
        public decimal MinimumSpend;
        public byte MinimumPacks;
        /// <summary>
        /// The Package Ids that can count toward minimum packs
        /// </summary>
        public List<int> MinimumPacksEligibleIds;
        public bool AllowPartialDiscounts;
        public int MaximumUsePerSession; //4320
        /// <summary>
        /// The schedule of valid days for the discount
        /// </summary>
        public List<Schedule> DiscountSchedule
        {
            get;
            set;
        }
        /// US4942
        /// <summary>
        /// Whether or not to ignore any validations earned from ignored packages when calculating the min spend
        /// </summary>
        public bool IgnoreValidationsForIgnoredPackages
        {
            get;
            set;
        }
        /// <summary>
        /// The list of package IDs to ignore when calculating the min spend
        /// </summary>
        public List<int> RestrictedPackageIds
        {
            get;
            set;
        }

        /// <summary>
        /// The advanced quantity discount
        /// 
        /// </summary>
        public readonly QuantityDiscount AdvancedQuantityDiscount;

        #endregion

        public override string ToString()
        {
            return DiscountName;
        }

        public class SpendLevel
        {
            public SpendLevel()
            {
            }

            public SpendLevel(SpendLevel spendlevel)
            {
                Sequence = spendlevel.Sequence;
                SpendMinValue = spendlevel.SpendMinValue;
                SpendMaxValue = spendlevel.SpendMaxValue;
                SpendValue = spendlevel.SpendValue;
            }

            public int Sequence;
            public decimal SpendMinValue;
            public decimal SpendMaxValue;
            public decimal SpendValue;
        }

        public class Schedule
        {
            /// <summary>
            /// The day of the week. If null, then the session applies to all days.
            /// </summary>
            public DayOfWeek? DayOfWeek;
            /// <summary>
            /// The session number. If null, the day applies to all sessions.
            /// </summary>
            public int? SessionNumber;

            /// <summary>
            /// Used to determine whether or not two schedules are effectively the same. Note: more of a "fuzzy" equal since 'null' means 'all'
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is Schedule)
                {
                    Schedule sch = (obj as Schedule);
                    if (!sch.DayOfWeek.HasValue || sch.DayOfWeek == DayOfWeek
                        && !sch.SessionNumber.HasValue || sch.SessionNumber == SessionNumber)
                        return true;
                }
                return false;
            }
        }

        public class QuantityDiscount
        {
            /// <summary>
            /// The buy quantity
            /// </summary>
            public int BuyQuantity { get; set; }
            /// <summary>
            /// The buy package identifier
            /// </summary>
            public int BuyPackageId { get; set; }
            /// <summary>
            /// The get quantity
            /// </summary>
            public int GetQuantity { get; set; }
            /// <summary>
            /// The get package identifier
            /// </summary>
            public int GetPackageId { get; set; }
        }
    }

}
