// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Collections.Generic;
using GTI.Modules.Shared;

namespace GTI.Modules.Shared
{
   

    public class OperatorFee
    {
        protected int m_deviceID = 0;
        protected string m_fee = "";

        public int DeviceId
        {
            get { return m_deviceID; }
            set { m_deviceID = value; }
        }

        public string Fee
        {
            get { return m_fee; }
            set { m_fee = value; }
        }
    }
    /// <summary>
    /// Represents a bingo operator (charity or business).
    /// </summary>
    public class Operator
    {
        public Operator()
        {
            OperatorFeeList = new List<OperatorFee>();
        }

        #region Member Variables
        protected object m_syncRoot = new object();
        protected int m_id = 0;
        protected string m_name = string.Empty;
        
       
       
       
        protected decimal m_fixedFee = 0M;
        protected decimal m_travelerFee = 0M;
        protected decimal m_trackerFee = 0M;
        protected decimal m_explorerFee = 0M; // Rally TA7729
        protected decimal m_traveler2Fee; // PDTS 964, Rally US765
        protected decimal m_tabletFee = 0M; //TA12156

        protected string m_TaxPayerID = "";
        protected int m_billingAddressID = 0;
        protected decimal m_HallRent = 0M;
        protected decimal m_PercentOfProfitsToCharity = 0M;
        protected decimal m_PercentPrizesToState = 0M;
        protected int m_cashMethodID = 0;
        protected string m_address1 = "";
        protected string m_address2 = "";
        protected string m_city = "";
        protected string m_state = "";
        protected string m_zip = "";
        protected string m_country = "";

        protected string m_billingAddress1 = "";
        protected string m_billingAddress2 = "";
        protected string m_billingCity = "";
        protected string m_billingState = "";
        protected string m_billingZip = "";
        protected string m_billingCountry = "";

        protected int m_playerTierCalcId = 0;
        protected Int16 m_operatorFeeCount = 0;
        protected List<OperatorFee> m_operatorFeeList;

        //System Settings
        protected string m_phone = "";
        protected string m_modem = "";
 
      
        protected bool m_IsActive = true;
        protected string m_Licence = "";
        protected string m_code = "";
        protected string m_contactName = "";
        protected int m_addressID = 0;
        protected decimal m_maxPointsPerSess = 0M;
        protected decimal m_maxPointsPerDay = 0M;
      
        

        protected int m_companyID = 0;
        
       
      
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current Operator.
        /// </summary>
        /// <returns>A string that represents the current Operator.</returns>
        public override string ToString()
        {
            return m_name;
        }

        public decimal GetDeviceFeeById(int deviceId)
        {
            decimal fee = 0M;

            if (deviceId == Device.Explorer.Id)
                fee = ExplorerDeviceFee;
            else if (deviceId == Device.Fixed.Id)
                fee = FixedDeviceFee;
            else if (deviceId == Device.Tablet.Id)
                fee = TabletDeviceFee;
            else if (deviceId == Device.Tracker.Id)
                fee = TrackerDeviceFee;
            else if (deviceId == Device.Traveler.Id)
                fee = TravelerDeviceFee;
            else if (deviceId == Device.Traveler2.Id)
                fee = Traveler2DeviceFee;

            return fee;
        }
        #endregion

        #region Member Properties

       
        public string Phone
        {
            get
            {
                return m_phone;
            }
            set
            {
                m_phone = value;
            }
        }
        public string Modem
        {
            get
            {
                return m_modem;
            }
            set
            {
                m_modem = value;
            }
        }
        public string Licence
        {
            get
            {
                return m_Licence;
            }
            set
            {
                m_Licence = value;
            }
        }
        public string Code
        {
            get
            {
                return m_code;
            }
            set
            {
                m_code = value;
            }
        }
        public string ContactName
        {
            get
            {
                return m_contactName;
            }
            set
            {
                m_contactName = value;
            }
        }
       
        public int AddressID
        {
            get
            {
                return m_addressID;
            }
            set
            {
                m_addressID = value;
            }
        }
       
        public int CompanyID
        {
            get
            {
                return m_companyID;
            }
            set
            {
                m_companyID = value;
            }
        }
       
        public decimal MaxPtsPerSession
        {
            get
            {
                return m_maxPointsPerSess;
            }
            set
            {
                m_maxPointsPerSess = value;
            }
        }
        public decimal MaxPointsPerDay
        {
            get
            {
                return m_maxPointsPerDay;
            }
            set
            {
                m_maxPointsPerDay = value;
            }
        }
       
        public bool IsActive
        {
            get
            {
                return m_IsActive;
            }
            set
            {
                m_IsActive = value;
            }
        }
     
        /// <summary>
        /// Gets an object that can be used to synchronize access to 
        /// the operator.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return m_syncRoot;
            }
        }

        /// <summary>
        /// Gets or sets the operator's id.
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the fee for fixed based units.
        /// </summary>
        public decimal FixedDeviceFee
        {
            get
            {
                return m_fixedFee;
            }
            set
            {
                m_fixedFee = value;
            }
        }

        /// <summary>
        /// Gets or sets the fee for a Traveler.
        /// </summary>
        public decimal TravelerDeviceFee
        {
            get
            {
                return m_travelerFee;
            }
            set
            {
                m_travelerFee = value;
            }
        }

        /// <summary>
        /// Gets or sets the fee for a Tracker.
        /// </summary>
        public decimal TrackerDeviceFee
        {
            get
            {
                return m_trackerFee;
            }
            set
            {
                m_trackerFee = value;
            }
        }

        // Rally TA7729
        /// <summary>
        /// Gets or sets the fee for the Explorer.
        /// </summary>
        public decimal ExplorerDeviceFee
        {
            get
            {
                return m_explorerFee;
            }
            set
            {
                m_explorerFee = value;
            }
        }

        // Rally US765
        /// <summary>
        /// Gets or sets the fee for the Traveler II.
        /// </summary>
        public decimal Traveler2DeviceFee 
        {
            get
            {
                return m_traveler2Fee;
            }
            set
            {
                m_traveler2Fee = value;
            }
        }

        //TA12156
        public decimal TabletDeviceFee
        {
            get
            {
                return m_tabletFee;
            }
            set
            {
                m_tabletFee = value;
            }
        }


        public string TaxPayerId
        {
            get { return m_TaxPayerID; }
            set { m_TaxPayerID = value; }
        }

        public int BillingAddressId
        {
            get { return m_billingAddressID; }
            set { m_billingAddressID = value; }
        }

        public decimal HallRent
        {
            get { return m_HallRent; }
            set { m_HallRent = value; }
        }

        public decimal PercentOfProfitsToCharity
        {
            get { return m_PercentOfProfitsToCharity; }
            set { m_PercentOfProfitsToCharity = value; }
        }

        public decimal PercentPrizesToState
        {
            get { return m_PercentPrizesToState; }
            set { m_PercentPrizesToState = value; }
        }

        public string Address1
        {
            get { return m_address1; }
            set { m_address1 = value; }
        }

        public string Address2
        {
            get { return m_address2; }
            set { m_address2 = value; }
        }

        public string City
        {
            get { return m_city; }
            set { m_city = value; }
        }

        public string State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public string Zip
        {
            get { return m_zip; }
            set { m_zip = value; }
        }

        public string BillingAddress1
        {
            get { return m_billingAddress1; }
            set { m_billingAddress1 = value; }
        }

        public string BillingAddress2
        {
            get { return m_billingAddress2; }
            set { m_billingAddress2 = value; }
        }

        public string BillingCity
        {
            get { return m_billingCity; }
            set { m_billingCity = value; }
        }

        public string BillingState
        {
            get { return m_billingState; }
            set { m_billingState = value; }
        }

        public string BillingZip
        {
            get { return m_billingZip; }
            set { m_billingZip = value; }
        }

        public string BillingCountry
        {
            get { return m_billingCountry; }
            set { m_billingCountry = value; }
        }

        public string Country
        {
            get { return m_country; }
            set { m_country = value; }
        }

        public int PlayerTierCalcId
        {
            get { return m_playerTierCalcId; }
            set { m_playerTierCalcId = value; }
        }

        public short OperatorFeeCount
        {
            get { return m_operatorFeeCount; }
            set { m_operatorFeeCount = value; }
        }

        public List<OperatorFee> OperatorFeeList
        {
            get { return m_operatorFeeList; }
            set { m_operatorFeeList = value; }
        }

        public int CashMethodID
        {
            get { return m_cashMethodID; }
            set { m_cashMethodID = value; }
        }

        #endregion
    }
}
