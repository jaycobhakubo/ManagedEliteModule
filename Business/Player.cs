// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

//4320: Limit how many times a discount can be used.

using System;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using GTI.Modules.Shared;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a player of the system.
    /// </summary>
    public class Player
    {
        #region Member Variables
        protected int m_id = 0;
        protected string m_firstName = string.Empty;
        protected string m_middleInitial = string.Empty;
        protected string m_lastName = string.Empty;
        protected string m_govIssuedIdNum = string.Empty;
        protected DateTime m_birthDate;
        protected string m_email = string.Empty;
        protected string m_playerIdent = string.Empty;
        protected string m_phoneNum = string.Empty;
        protected string m_gender = string.Empty;
        protected byte[] m_pinNum = new byte[DataSizes.PasswordHash]; // Rally TA1583
        protected string m_address1 = string.Empty;
        protected string m_address2 = string.Empty;
        protected string m_city = string.Empty;
        protected string m_state = string.Empty;
        protected string m_zip = string.Empty;
        protected string m_country = string.Empty;
        protected DateTime m_joinDate;
        protected DateTime m_lastVisit;
        protected int m_visitCount = 0;
        protected decimal m_pointsBalance = 0M;
        protected bool m_pointsUpToDate = true;
        protected string m_comment = string.Empty;
        protected string m_magCardNum = string.Empty;
        protected bool m_weGotThePlayerCardPIN = false;
        private int m_playerCardPIN = 0;
        private string m_playerCard = string.Empty;
        protected bool m_playerCardPINError = false;
        protected string m_ErrorMessage = string.Empty;
        protected Bitmap m_image = null;
        protected PlayerLoyaltyTier m_tier = null;
        protected List<PlayerComp> m_comps = new List<PlayerComp>();
        protected decimal m_totalSpend = 0; //player's spend since enrollment, read only
        protected Dictionary<string, bool> m_receiptNumbers; //player receipts for the day and flag if presold, readonly //US5591: player center presold receipts added flag
        protected bool m_IsLoggedIn = false; //if player is logged in to a unit, readonly
        protected bool m_thirdPartyInterfaceDown = false;
        private decimal m_RefundableCredit = 0;
        private decimal m_NonRefundableCredit = 0;
        private decimal m_cashOnlyCredit = 0; // TTP 50114
        protected bool m_usedCouponScreen = false;
        protected object m_scheduledSalesObject = null;
        protected int m_age;

        // JW 1-22-2008 
        private bool mbolIsCreditOnline = true;

        //private  int no_OfDaysPlayed = 0;
        //protected int no_OfSessionPlayed = 0;
        //protected DateTime gamingDate;
        //protected int sessionNumber = 0;
        //protected string DaysOfWeek = string.Empty;

        private Dictionary<short, int> m_cbbFavoriteCount = new Dictionary<short, int>(); // Rally US507
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Player class.
        /// </summary>
        public Player()
        {
            // Rally US493 - Player Center Statuses
            ActiveStatusList = new List<PlayerStatus>();
            DiscountUsageDictionary = new Dictionary<int, int>();
        }

        /// <summary>
        /// Initializes a new instance of the Player class.  This constructor 
        /// will call various server message in order to populate the instance 
        /// variables.
        /// </summary>
        /// <param name="playerId">The id of the player to create.</param>
        /// <param name="operatorId">The id of the operator this player 
        /// belongs to.</param>
        /// <param name="PIN">The PIN for the third party system.</param>
        /// <exception cref="GTI.Modules.Shared.ServerCommException">The server 
        /// did not response to a message request.</exception>
        /// <exception cref="GTI.Modules.Shared.MessageWrongSizeException">The 
        /// server sent a message that was an unexpected size.</exception>
        /// <exception cref="GTI.Modules.Shared.ServerException">The server 
        /// returned a negative return code or a problem occured while 
        /// unpackaging a message.</exception>
        public Player(int playerId, int operatorId, int PIN = 0, bool thirdPartyPlayerSync = true, bool pointsAreGood = false)
        {
            // Rally TA1583
            GetPlayerDataMessage getMsg = new GetPlayerDataMessage(playerId, PIN);//JC-A

            getMsg.ThirdPartyPlayerSync = thirdPartyPlayerSync;
            getMsg.PointsAreGood = pointsAreGood;
            
            // Send the message.
            getMsg.Send();

            // Fill in the data.
            m_id = playerId;
            m_firstName = getMsg.FirstName;
            m_middleInitial = getMsg.MiddleInitial;
            m_lastName = getMsg.LastName;
            m_govIssuedIdNum = getMsg.GovIssuedIdNumber;
            m_birthDate = getMsg.BirthDate;
            m_age = getMsg.Age;
            m_email = getMsg.Email;
            m_playerIdent = getMsg.PlayerIdentity;
            m_phoneNum = getMsg.PhoneNumber;
            m_gender = getMsg.Gender;
            m_pinNum = getMsg.PinNumber;
            m_address1 = getMsg.Address1;
            m_address2 = getMsg.Address2;
            m_city = getMsg.City;
            m_state = getMsg.State;
            m_zip = getMsg.Zip;
            m_country = getMsg.Country;
            m_joinDate = getMsg.JoinDate;
            m_lastVisit = getMsg.LastVisit;
            m_pointsBalance = getMsg.PointsBalance;
            m_visitCount = getMsg.VisitCount;
            m_comment = getMsg.Comment;
            m_magCardNum = getMsg.MagCardNumber;
            m_totalSpend = getMsg.TotalSpend;
            m_IsLoggedIn = getMsg.IsLoggedIn;
            m_pointsUpToDate = getMsg.PointsUpToDate;
            m_playerCardPINError = getMsg.PlayerCardPINError;
            m_thirdPartyInterfaceDown = getMsg.ThirdPartyInterfaceDown;
            m_ErrorMessage = getMsg.ErrorMessage;

            // Find the player's tier
            GetPlayerTierListMessage tierMsg = new GetPlayerTierListMessage(operatorId);
            tierMsg.Send();

            PlayerLoyaltyTier[] playerTiers = tierMsg.Tiers;

            if(getMsg.PlayerTierId > 0 && playerTiers != null)
            {
                foreach(PlayerLoyaltyTier tier in playerTiers)
                {
                    if(tier.Id == getMsg.PlayerTierId)
                    {
                        m_tier = tier;
                        break;
                    }
                }
            }
            else
                m_tier = null;

            // Get the player's picture.
            GetPlayerImageMessage getPicMsg = new GetPlayerImageMessage();
            getPicMsg.PlayerId = m_id;

            getPicMsg.Send();
            m_image = getPicMsg.Image;

            // Get the player's comps.
            GetPlayerCompsMessage getCompMsg = new GetPlayerCompsMessage(m_id);

            getCompMsg.Send();
            m_comps.AddRange(getCompMsg.Comps);

            // TTP 50067
            // Get the current gaming date.
            GetGamingDateMessage gamingMsg = new GetGamingDateMessage(operatorId);
            gamingMsg.Send();

            // Get the player's receipts
            GetPlayerReceipts getReceiptsMsg = new GetPlayerReceipts(m_id, operatorId, gamingMsg.GamingDate);
                
            getReceiptsMsg.Send();
            ReceiptNumbers = getReceiptsMsg.Receipts;

            // Rally US493
            // Get the player's statuses
            ActiveStatusList = GetPlayerStatusCode.GetPlayerStatus(m_id);

            // Rally US507
            GetPlayerCBBFavoriteCountsMessage favoritesMsg = new GetPlayerCBBFavoriteCountsMessage(m_id);
            favoritesMsg.Send();

            foreach(KeyValuePair<short, int> pair in favoritesMsg.FavoriteCounts)
            {
                m_cbbFavoriteCount.Add(pair.Key, pair.Value);
            }

            CreditModuleOnline col = new CreditModuleOnline();

            /*          Debug Code             */
            // mbolIsCreditOnline = false;
            
            if (col.IsCreditModuleOnline)
            {
                // Get the player's credit.
                GetCreditMessage creditMsg = new GetCreditMessage(m_id);

                creditMsg.Send();

                m_RefundableCredit = creditMsg.RefundableCredit;
                m_NonRefundableCredit = creditMsg.NonRefundableCredit;
                m_cashOnlyCredit = creditMsg.CashOnlyCredit; // TTP 50114
            }
            else
            {
                mbolIsCreditOnline = false;
            }

            GetPlayerMagCardPINMessage PINMsg = new GetPlayerMagCardPINMessage(playerId);
            PINMsg.Send();

            m_playerCardPIN = PINMsg.PlayerMagCardPIN;
            m_playerCard = PINMsg.PlayerMagCard;
        }
        #endregion

        #region Member Methods
        // TTP 50114
        /// <summary>
        /// Returns a string that represents the current Player.
        /// </summary>
        /// <returns>A string that represents the current 
        /// Player.</returns>        
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// Returns a string that represents the current Player.
        /// </summary>
        /// <param name="reverseName">If true the name will be in the form of 
        /// [Last Name], [First Name] [Middle Initial]; otherwise it will 
        /// be [First Name] [Last Name].</param>
        /// <returns>A string that represents the current 
        /// Player.</returns>        
        public string ToString(bool reverseName)
        {
            string returnVal = string.Empty;

            if(reverseName)
            {
                if(!string.IsNullOrEmpty(m_lastName))
                    returnVal = m_lastName;

                if(!string.IsNullOrEmpty(m_firstName))
                {
                    if(!string.IsNullOrEmpty(returnVal))
                        returnVal += ", ";

                    returnVal += m_firstName;
                }

                if(!string.IsNullOrEmpty(m_middleInitial))
                {
                    if(!string.IsNullOrEmpty(returnVal))
                        returnVal += " ";

                    returnVal += m_middleInitial;
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(m_firstName))
                    returnVal = m_firstName;

                if(!string.IsNullOrEmpty(m_lastName))
                {
                    if(!string.IsNullOrEmpty(returnVal))
                        returnVal += " ";

                    returnVal += m_lastName;
                }
            }

            if(string.IsNullOrEmpty(returnVal.Trim()))
                returnVal = string.Format(CultureInfo.CurrentCulture, Resources.PlayerNoName, m_id);

            return returnVal;
        }

        // Rally US507
        /// <summary>
        /// Returns a count of cards the player has saved in the database for
        /// the specified pick count.
        /// </summary>
        /// <param name="pickCount">Pick count type of cards to return.</param>
        /// <returns>The count of cards.</returns>
        public int GetCBBFavoriteCount(short pickCount)
        {
            if(m_cbbFavoriteCount.ContainsKey(pickCount))
                return m_cbbFavoriteCount[pickCount];
            else
                return 0;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the player's ID.
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

        public object ScheduledSalesObject
        {
            get
            {
                return m_scheduledSalesObject;
            }

            set
            {
                m_scheduledSalesObject = value;
            }
        }

        /// <summary>
        ///  Tests for the Credit Module Operational for this install.
        ///  Returns:
        ///  True = Yes it is.
        ///  False = guess!
        /// </summary>
        public bool IsCreditOnline
        {
            get { return mbolIsCreditOnline; }
            set { mbolIsCreditOnline =value; }
        }

        /// <summary>
        /// Gets or sets the player's first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return m_firstName;
            }
            set
            {
                m_firstName = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's middle initial.
        /// </summary>
        public string MiddleInitial
        {
            get
            {
                return m_middleInitial;
            }
            set
            {
                m_middleInitial = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return m_lastName;
            }
            set
            {
                m_lastName = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's government issued id number.
        /// </summary>
        public string GovIssuedIdNumber
        {
            get
            {
                return m_govIssuedIdNum;
            }
            set
            {
                m_govIssuedIdNum = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's birth date.
        /// </summary>
        public DateTime BirthDate
        {
            get
            {
                return m_birthDate;
            }
            set
            {
                m_birthDate = value;
            }
        }

        public int Age
        {
            get
            {
                return m_age;
            }
        }

        /// <summary>
        /// Gets or sets the player's email address.
        /// </summary>
        public string Email
        {
            get
            {
                return m_email;
            }
            set
            {
                m_email = value;
            }
        }

        /// <summary>
        /// Gets or sets a custom player id string.
        /// </summary>
        public string PlayerIdentity
        {
            get
            {
                return m_playerIdent;
            }
            set
            {
                m_playerIdent = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's phone number.
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                return m_phoneNum;
            }
            set
            {
                m_phoneNum = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's gender.
        /// </summary>
        public string Gender
        {
            get
            {
                return m_gender;
            }
            set
            {
                m_gender = value;
            }
        }

        // Rally TA1583
        /// <summary>
        /// Gets or sets the player's pin number.
        /// </summary>
        public byte[] PinNumber
        {
            get
            {
                return m_pinNum;
            }
            set
            {
                m_pinNum = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's address line 1.
        /// </summary>
        public string Address1
        {
            get
            {
                return m_address1;
            }
            set
            {
                m_address1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's address line 2.
        /// </summary>
        public string Address2
        {
            get
            {
                return m_address2;
            }
            set
            {
                m_address2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's city.
        /// </summary>
        public string City
        {
            get
            {
                return m_city;
            }
            set
            {
                m_city = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's state.
        /// </summary>
        public string State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's zip.
        /// </summary>
        public string Zip
        {
            get
            {
                return m_zip;
            }
            set
            {
                m_zip = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's country.
        /// </summary>
        public string Country
        {
            get
            {
                return m_country;
            }
            set
            {
                m_country = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's join date.
        /// </summary>
        public DateTime JoinDate
        {
            get
            {
                return m_joinDate;
            }
            set
            {
                m_joinDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's last visit.
        /// </summary>
        public DateTime LastVisit
        {
            get
            {
                return m_lastVisit;
            }
            set
            {
                m_lastVisit = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's point balance.
        /// </summary>
        public decimal PointsBalance
        {
            get
            {
                return m_pointsBalance;
            }
            set
            {
                m_pointsBalance = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the PointsBalance is up to date.
        /// If using a third party system, the points must be requested and might require a PIN. If the points
        /// are not loaded, this will return false.
        /// </summary>
        public bool PointsUpToDate
        {
            get
            {
                return m_pointsUpToDate;
            }

            set
            {
                m_pointsUpToDate = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the player's visit count.
        /// </summary>
        public int VisitCount
        {
            get
            {
                return m_visitCount;
            }
            set
            {
                m_visitCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's comment.
        /// </summary>
        public string Comment
        {
            get
            {
                return m_comment;
            }
            set
            {
                m_comment = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's magnetic card number.
        /// </summary>
        public string MagneticCardNumber
        {
            get
            {
                return m_magCardNum;
            }
            set
            {
                m_magCardNum = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's image.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return m_image;
            }
            set
            {
                m_image = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's loyalty tier (null indicates the
        /// player doesn't have a tier).
        /// </summary>
        public PlayerLoyaltyTier LoyaltyTier
        {
            get
            {
                return m_tier;
            }
            set
            {
                m_tier = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's current comps.
        /// </summary>
        public List<PlayerComp> Comps
        {
            get
            {
                return m_comps;
            }
            set
            {
                m_comps = value;
            }
        }


        public decimal TotalSpend //player's spend since enrollment
        { 
            get
            {
                return m_totalSpend;
            }
            set
            {
                m_totalSpend = value;
            }
        
        }

        //US5591: player center presold receipts added flag
        public Dictionary<string, bool> ReceiptNumbers //player receipts for the day
        {
            get
            {
                return m_receiptNumbers;
            }
            set
            {
                m_receiptNumbers = value;
            }
        
        }
        public bool IsLoggedIn //if player is logged in to a unit
        {
            get
            {
                return m_IsLoggedIn;
            }
            set
            {
                m_IsLoggedIn = value;
            }
        }

        public List<PlayerStatus> ActiveStatusList { get; set; }

        public decimal RefundableCredit
        {
            get { return m_RefundableCredit; }
            set { m_RefundableCredit = value; }
        }
        public decimal NonRefundableCredit
        {
            get { return m_NonRefundableCredit; }
            set { m_NonRefundableCredit = value; }
        }

        // TTP 50114
        /// <summary>
        /// Gets or sets the player's cash only credit (credit that cannot be 
        /// used, only cashed out).
        /// </summary>
        public decimal CashOnlyCredit
        {
            get
            {
                return m_cashOnlyCredit;
            }
            set
            {
                m_cashOnlyCredit = value;
            }
        }

        /// <summary>
        /// Gets if there was a problem with the player card PIN.
        /// </summary>
        public bool PlayerCardPINError
        {
            get
            {
                return m_playerCardPINError;
            }

            set
            {
                m_playerCardPINError = value;
            }
        }

        /// <summary>
        /// Gets / sets if we have the player card PIN.
        /// </summary>
        public bool WeHaveThePlayerCardPIN
        {
            get
            {
                return m_weGotThePlayerCardPIN;
            }

            set
            {
                m_weGotThePlayerCardPIN = value;
            }
        }

        /// <summary>
        /// Gets or sets the player card PIN.
        /// </summary>
        public int PlayerCardPIN
        {
            get
            {
                return m_playerCardPIN;
            }

            set
            {
                m_playerCardPIN = value;
            }
        }

        /// <summary>
        /// Gets the player card.
        /// </summary>
        public string PlayerCard
        {
            get
            {
                return m_playerCard;
            }
        }

        /// <summary>
        /// Gets if the third party interface is down.
        /// </summary>
        public bool ThirdPartyInterfaceDown
        {
            get
            {
                return m_thirdPartyInterfaceDown;
            }

            set
            {
                m_thirdPartyInterfaceDown = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return m_ErrorMessage;
            }
        }

        public bool UsedCouponScreen
        {
            get
            {
                return m_usedCouponScreen;
            }

            set
            {
                m_usedCouponScreen = value;
            }
        }

        //US4320
        /// <summary>
        /// The discount usage dictionary. 
        /// Key- Discount ID
        /// Value - number of times the discount has been used
        /// </summary>
        public Dictionary<int, int> DiscountUsageDictionary;

        #endregion
    }

    /// <summary>
    /// Represents a player's loyalty tier in the system.
    /// </summary>
    public class PlayerLoyaltyTier
    {
        #region Member Variables
        protected int m_id = 0;
        protected string m_name = string.Empty;
        protected int m_level = 0;
        protected decimal m_pointsPerHour = 0M;
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current PlayerLoyaltyTier.
        /// </summary>
        /// <returns>A string that represents the current 
        /// PlayerLoyaltyTier.</returns>
        public override string ToString()
        {
            return m_name;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the tier's id.
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
        /// Gets or sets the tier's name.
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
        /// A numeric representation of the tier.
        /// </summary>
        public int Level
        {
            get
            {
                return m_level;
            }
            set
            {
                m_level = value;
            }
        }

        /// <summary>
        /// Gets or sets the points per hour earned by this tier.
        /// </summary>
        public decimal PointsPerHour
        {
            get
            {
                return m_pointsPerHour;
            }
            set
            {
                m_pointsPerHour = value;
            }
        }
        #endregion
    }
}
