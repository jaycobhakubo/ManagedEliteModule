// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

//US4852: Product Center > Coupons: Require spend

using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a player compensation
    /// </summary>
    public class PlayerComp
    {
        #region Member Variables

        protected int m_id = 0;
        protected int m_compAwardId = 0;
        protected int m_operatorId = 0;
        protected string m_name = string.Empty;
        protected string m_sname = string.Empty;
        protected bool m_credit = false;
        protected DateTime m_startDate;
        protected DateTime m_expires;
        protected decimal m_amount = 0;
        protected decimal m_percentDiscount;
        protected int? m_couponMaxUsage;
        protected DateTime? m_lastAwardedDate;
        protected CouponTypes m_couponType;
        protected int m_remainingComp;
        protected AwardTypes m_awardType;
        protected decimal m_unlockSpend;
        protected int m_sessionCount;
        protected bool m_partOfMultiPackageCoupon = false;
        protected int m_packageIdForMultiPackageCoupon = 0;
        protected int? m_programLimit;
        protected int? m_dailyLimit;
        protected int? m_weeklyLimit;
        protected int? m_monthlyLimit;
        protected int? m_yearlyLimit;
        private Dictionary<int, int?> m_specificProgramLimits = new Dictionary<int, int?>();
        private Dictionary<int, int?> m_sessionNumberLimits = new Dictionary<int, int?>();
        private Dictionary<DayOfWeek, int?> m_dayOfWeekLimits = new Dictionary<DayOfWeek, int?>();
        private Dictionary<int, int?> m_monthOfYearLimits = new Dictionary<int, int?>();
        private int? m_birthdayWindowDaysBefore;
        private int? m_birthdayWindowDaysFollowing;

        #endregion

        #region Constructors

        public PlayerComp()
        {
            RestrictedProductIds = new List<int>();
            RestrictedPackageIds = new List<int>();
            EarnedPackageIDs = new List<int>();
        }

        /// <summary>
        /// Copy Constructor. Sets the fields to the same as the one sent in
        /// </summary>
        /// <param name="copy"></param>
        public PlayerComp(PlayerComp copy)
        {
            this.Id = copy.Id;
            this.Name = copy.Name;
            this.StartDate = copy.StartDate;
            this.EndDate = copy.EndDate;
            this.Value = copy.Value;
            this.CouponMaxUsage = copy.CouponMaxUsage;
            this.CouponType = copy.CouponType;
            this.EarnedPackageIDs = copy.EarnedPackageIDs; // US4941
            this.AwardType = copy.AwardType;
            this.UnlockSpend = copy.UnlockSpend;
            this.UnlockSessionCount = copy.UnlockSessionCount;
            this.MinimumSpendToQualify = copy.MinimumSpendToQualify; //US4852
            this.RestrictedPackageIds = copy.RestrictedPackageIds;
            this.RestrictedProductIds = copy.RestrictedProductIds; //US4852
            this.IsPartOfMultiPackageCoupon = copy.IsPartOfMultiPackageCoupon;
            this.PackageID = copy.PackageID;
            this.IgnoreValidationsForIgnoredPackages = copy.IgnoreValidationsForIgnoredPackages;
            this.PercentDiscount = copy.PercentDiscount;
            this.CompAwardId = copy.CompAwardId;
            this.OperatorId = copy.OperatorId;
            this.ShortName = copy.ShortName;
            this.Credit = copy.Credit;
            this.LastAwardedDate = copy.LastAwardedDate;
            this.RemainingComp = copy.RemainingComp;

            this.ProgramLimit = copy.ProgramLimit;
            this.DailyLimit = copy.DailyLimit;
            this.WeeklyLimit = copy.WeeklyLimit;
            this.MonthlyLimit = copy.MonthlyLimit;
            this.YearlyLimit = copy.YearlyLimit;

            this.m_specificProgramLimits = new Dictionary<int, int?>(copy.m_specificProgramLimits);
            this.m_dayOfWeekLimits = new Dictionary<DayOfWeek, int?>(copy.DayOfWeekLimits);
            this.m_monthOfYearLimits = new Dictionary<int, int?>(copy.MonthOfYearLimits);
            this.m_sessionNumberLimits = new Dictionary<int, int?>(copy.m_sessionNumberLimits);

        }


        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current PlayerComp.
        /// </summary>
        /// <returns>A string that represents the current 
        /// PlayerComp.</returns>
        public override string ToString()
        {
            if(String.IsNullOrWhiteSpace(m_sname))
                return m_name;
            return m_sname;
        }

        /// <summary>
        /// Returns a description of this coupon
        /// </summary>
        /// <param name="packages">if this coupon is package-based, then it needs the packages to return a user-friendly name</param>
        /// <returns></returns>
        public string GetDescription(List<PackageItem> packages = null)
        {
            string description = CouponType.ToString();
            if(CouponType == PlayerComp.CouponTypes.FixedValue)
            {
                description = String.Format("{0:f} Off", Value);
            }
            else if(CouponType == PlayerComp.CouponTypes.PercentPackage)
            {
                string packageName = "Multiple Packages";

                if(EarnedPackageIDs.Count == 1)
                {
                    int packageID = EarnedPackageIDs.First();
                    PackageItem match = null;
                    if(packages != null)
                        match = packages.FirstOrDefault(x => x.PackageId == packageID); // find the package name for display

                    if(match != null)
                        packageName = match.PackageName;
                    else
                        packageName = "Package " + packageID;
                }
                description = String.Format("{0}% off {1}", Value.ToString(), packageName);
            }
            else // Alt Price Package
            {
                string packageName = "Multiple Packages";

                if(EarnedPackageIDs.Count == 1)
                {
                    int packageID = EarnedPackageIDs.First();
                    PackageItem match = null;
                    if(packages != null)
                        match = packages.FirstOrDefault(x => x.PackageId == packageID); // find the package name for display

                    if(match != null)
                        packageName = match.PackageName;
                    else
                        packageName = "Package " + packageID;
                }

                description = String.Format("Alt Price {0}", packageName);
            }

            return description;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the comp's id.
        /// </summary>
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        /// <summary>
        /// Gets or sets the comp's award id.
        /// </summary>
        public int CompAwardId
        {
            get { return m_compAwardId; }
            set { m_compAwardId = value; }
        }

        /// <summary>
        /// Gets or sets the id of the opeator this comp is assoicated with.
        /// </summary>
        public int OperatorId
        {
            get { return m_operatorId; }
            set { m_operatorId = value; }
        }

        /// <summary>
        /// Gets or sets the comp's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Gets or sets the comp's short name.
        /// </summary>
        public string ShortName
        {
            get { return m_sname; }
            set { m_sname = value; }
        }

        /// <summary>
        /// Gets or sets whether the comp's credit based.
        /// </summary>
        public bool Credit
        {
            get { return m_credit; }
            set { m_credit = value; }
        }

        /// <summary>
        /// Gets or sets the comp's expiration date.
        /// </summary>
        public DateTime EndDate
        {
            get { return m_expires; }
            set { m_expires = value; }
        }

        /// <summary>
        /// Gets or sets the comp's starting date.
        /// </summary>
        public DateTime StartDate
        {
            get { return m_startDate; }
            set { m_startDate = value; }
        }

        /// <summary>
        /// Gets or sets the comp's value amount.
        /// </summary>
        public decimal Value
        {
            get { return m_amount; }
            set { m_amount = value; }
        }

        /// <summary>
        /// Whether or not the comp is currently expired
        /// </summary>
        public bool IsExpired { get { return m_expires <= DateTime.Now; } }

        /// <summary>
        /// The last time this comp was awarded. If null, has not been awarded
        /// </summary>
        public DateTime? LastAwardedDate
        {
            get { return m_lastAwardedDate; }
            set { m_lastAwardedDate = value; }
        }

        /// <summary>
        /// The type of Comp this is.
        /// </summary>
        public CouponTypes CouponType
        {
            get { return m_couponType; }
            set { m_couponType = value; }
        }

        /// <summary>
        /// Gets or sets the maximum amount of times this coupon is allowed to be used.
        /// </summary>
        public int? CouponMaxUsage
        {
            get { return m_couponMaxUsage; }
            set { m_couponMaxUsage = value; }
        }

        /// <summary>
        /// The number of times this coupon can still be used.
        /// </summary>
        public int RemainingComp
        {
            get { return m_remainingComp; }
            set { m_remainingComp = value; }
        }

        /// <summary>
        /// The way this coupon can be awarded
        /// </summary>
        public AwardTypes AwardType
        {
            get { return m_awardType; }
            set { m_awardType = value; }
        }

        /// <summary>
        /// Percent discount to apply to package.
        /// </summary>
        public decimal PercentDiscount
        {
            get { return m_percentDiscount; }
            set { m_percentDiscount = value; }
        }

        /// <summary>
        /// The amount that needs to be spent per session to unlock this coupon
        /// </summary>
        public decimal UnlockSpend
        {
            get { return m_unlockSpend; }
            set { m_unlockSpend = value; }
        }

        /// <summary>
        /// The number of sessions they need to spend the unlock amount to unlock this coupon
        /// </summary>
        public int UnlockSessionCount
        {
            get { return m_sessionCount; }
            set { m_sessionCount = value; }
        }

        public int? WindowAwardDaysBefore
        {
            get { return m_birthdayWindowDaysBefore; }
            set { m_birthdayWindowDaysBefore = value; }
        }

        public int? WindowAwardDaysFollowing
        {
            get { return m_birthdayWindowDaysFollowing; }
            set { m_birthdayWindowDaysFollowing = value; }
        }

        //US4852
        /// <summary>
        /// Gets or sets the minimum spend to qualify.
        /// </summary>
        /// <value>
        /// The minimum spend to qualify.
        /// </value>
        public decimal MinimumSpendToQualify { get; set; }

        /// <summary>
        /// If the Coupon is an "alt price" type, then these are the packages that the coupon applies to.
        /// </summary>
        public List<int> EarnedPackageIDs
        {
            get;
            set;
        }

        //US4852
        /// <summary>
        /// Gets or sets the exclude from qualify product identifier list.
        /// </summary>
        /// <value>
        /// The exclude from qualify product identifier list.
        /// </value>
        public List<int> RestrictedProductIds { get; set; }

        /// <summary>
        /// Is this coupon one of a group of coupons generated to replace a multipackage coupon.
        /// </summary>
        public bool IsPartOfMultiPackageCoupon
        {
            get { return m_partOfMultiPackageCoupon; }
            set { m_partOfMultiPackageCoupon = value; }
        }

        /// <summary>
        /// If this coupon is one of a group of coupons generated to replace a multipackage coupon,
        /// this is the package ID for the package attached to this coupon.
        /// If this coupon has a single package attached to it, this is that package's ID.
        /// </summary>
        public int PackageID
        {
            get { return m_packageIdForMultiPackageCoupon; }
            set { m_packageIdForMultiPackageCoupon = value; }
        }

        /// US4932
        /// <summary>
        /// Gets or sets the exclude from qualify package identifier list.
        /// </summary>
        public List<int> RestrictedPackageIds { get; set; }

        /// US4932
        /// <summary>
        /// Gets or sets whether or not to ignore validations for excluded packages
        /// </summary>
        public bool IgnoreValidationsForIgnoredPackages { get; set; }

        /// <summary>
        /// The maximum number of times this coupon can be used per program/session
        /// </summary>
        public int? ProgramLimit
        {
            get { return m_programLimit; }
            set { m_programLimit = value < 0 ? null : value; }
        }

        /// <summary>
        /// The maximum number of times this coupon can be used per day
        /// </summary>
        public int? DailyLimit
        {
            get { return m_dailyLimit; }
            set { m_dailyLimit = value < 0 ? null : value; }
        }

        /// <summary>
        /// The maximum number of times this coupon can be used per week
        /// </summary>
        public int? WeeklyLimit
        {
            get { return m_weeklyLimit; }
            set { m_weeklyLimit = value < 0 ? null : value; }
        }

        /// <summary>
        /// The maximum number of times this coupon can be used per month
        /// </summary>
        public int? MonthlyLimit
        {
            get { return m_monthlyLimit; }
            set { m_monthlyLimit = value < 0 ? null : value; }
        }

        /// <summary>
        /// The maximum number of times this coupon can be used per year
        /// </summary>
        public int? YearlyLimit
        {
            get { return m_yearlyLimit; }
            set { m_yearlyLimit = value < 0 ? null : value; }
        }

        public Dictionary<int, int?> ProgramLimits
        {
            get { return m_specificProgramLimits; }
        }

        public Dictionary<int, int?> SessionNumberLimits
        {
            get { return m_sessionNumberLimits; }
        }

        public Dictionary<DayOfWeek, int?> DayOfWeekLimits
        {
            get { return m_dayOfWeekLimits; }
        }

        public Dictionary<int, int?> MonthOfYearLimits
        {
            get { return m_monthOfYearLimits; }
        }

        #endregion

        /// <summary>
        /// The type of discounts the coupon can be
        /// </summary>
        public enum CouponTypes
        {
            [Description("Fixed")]
            FixedValue = 1,
            [Description("Alt Price")]
            AltPricePackage = 2,
            [Description("Percent")]
            PercentPackage = 3,
        }

        /// <summary>
        /// The different ways this coupon can be awarded
        /// </summary>
        public enum AwardTypes
        {
            [Description("Manual")]
            Manual = 1,
            [Description("Auto")]
            Auto = 2,
            [Description("Birth Day")]
            Birth_Day = 3,
            [Description("Instant")]
            Instant = 4,
            [Description("Birth Week")]
            Birth_Week = 5,
            [Description("Birth Month")]
            Birth_Month = 6,
            [Description("Birth Window")]
            Birth_Window = 7,
        }

        public enum LimitPeriodType
        {
            Session = 1,
            Daily = 2,
            Weekly = 3,
            Monthly = 4,
            Yearly = 5,
        }

    }

    public class PlayerCompComparer : IEqualityComparer<PlayerComp>
    {
        public bool Equals(PlayerComp x, PlayerComp y)
        {
            return x.Id == y.Id && x.Name == y.Name && x.PackageID == y.PackageID;
        }

        public int GetHashCode(PlayerComp obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public class PlayerCompExpirationComparer : IComparer<PlayerComp>
    {
        public int Compare(PlayerComp x, PlayerComp y)
        {
            int result = 0;

            if((x.EndDate - DateTime.Now).TotalHours < (y.EndDate - DateTime.Now).TotalHours)
                result = -1;

            if((x.EndDate - DateTime.Now).TotalHours > (y.EndDate - DateTime.Now).TotalHours)
                result = 1;

            return result;
        }
    }
}
