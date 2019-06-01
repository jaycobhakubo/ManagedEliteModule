// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2012 GameTech
// International, Inc.

//US2131 add transaction date/time  to receipt reprints
//US2718 Adding charity information to the receipt
//US4027 Add the serial number and pack number to the receipt
//US4384: (US1592) POS: Create sales receipt
//US4397: (US1592) POS: B3 Hand Pay Receipt
//US4404: (US1592) POS: B3 Jackpot Payment Receipt
//US4395: (US1592) POS: B3 Unlock Accounts Receipt
//US4338: (US1592) POS: Redeem B3 Receipt
//US4698: POS: Denomination receipt
//DE12995: Error found in US4698: POS: Denomination receipt > Totals are cut off on the right side
//DE13133: Error found in US4395: (US1592) POS: B3 Unlock Accounts > Receipt does not include Not Valid For Cash
//DE13135: Error found in US4338: (US1592) POS: Redeem B3 > Redeem total is not printed in words
//DE13136: Error found in US4338: (US1592) POS: Redeem B3 > Does not include name and signature lines
//DE13137: Error found in US4338: (US1592) POS: Redeem B3 > Receipt does not include Wins
//DE13133: Error found in US4395: (US1592) POS: B3 Unlock Accounts > Receipt does not include Not Valid For Cash
//DE13132: Error found in US4395: (US1592) POS: B3 Unlock Accounts > Unlock receipt credit amount is not correct
//US4804: Linear game numbers in the Edge system.
//US4847: POS: Add the total due, total drop, over short to the denomination slip
//US4848: Add signature lines to void receipts

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTI.Modules.Shared.Business;
using GTI.Modules.Shared.Properties;
using IDAutomation.NetAssembly;

namespace GTI.Modules.Shared
{
    public class LineDetail
    {
        public int order;
        public string text;

        public LineDetail()
        {
            order = 0;
            text = String.Empty;
        }

        public LineDetail(int order, string text)
        {
            this.order = order;
            this.text = text;
        }

        public static List<string> GetSortedDetail(List<LineDetail> unsortedDetail)
        {
            //sort the detail
            List<string> sortedDetail = new List<string>();

            // Line Items in sorted order
            int minSortLevel = 100;
            int maxSortLevel = -100;

            //find how many sort levels there are
            foreach (LineDetail item in unsortedDetail)
            {
                if (item.order < minSortLevel)
                    minSortLevel = item.order;

                if (item.order > maxSortLevel)
                    maxSortLevel = item.order;
            }

            //add all the lines one sort level at a time (high to low)
            for (int level = maxSortLevel; level >= minSortLevel; level--)
            {
                foreach (LineDetail item in unsortedDetail)
                {
                    if (item.order == level)
                        sortedDetail.Add(item.text);
                }
            }

            return sortedDetail;
        }
    }

    /// <summary>
    /// Class for holding information about a tender for a transaction.
    /// </summary>
    public class SaleTender
    {
        #region Member variables
        protected int m_registerReceiptTenderID = 0; //ID in RegisterReceiptTender table
        protected int m_registerReceiptID = 0; //ID for receipt this tender belongs to
        protected DateTime m_DTStamp = DateTime.Now; //date when tender was made
        protected TenderType m_tenderTypeID = TenderType.Cash; //tender type
        protected int m_tenderSubTypeID = 0; //tender sub-type (like what credit card)
        protected TransactionType m_transactionTypeID = TransactionType.Sale; //type of transaction this tender was for
        protected string m_IsoCode = string.Empty; //3 character code for the tender currency's country
        protected decimal m_amount = 0M; //amount tendered in the tendering currency
        protected decimal m_defaultAmount = 0M; //the amount tendered converted to the default currency
        protected decimal m_defaultTax = 0M; //the tax for the entire sale in the default currency
        protected decimal m_exchangeRate = 1M;
        protected string m_referenceNumber = string.Empty; //from tender processing
        protected string m_authorizationCode = string.Empty; //from tender processing
        protected string m_receiptDescription = string.Empty; //tendering line for the receipt (like "Card MC1234")
        protected int m_originalRegisterReceiptTenderID = 0; //original receipt ID if this tender was for a voided transaction

        protected string m_additionalTextForCustomerReceipt = string.Empty; //Authorization information, etc. to be printed on the customer's receipt.
        protected string m_additionalTextForMerchantReceipt = string.Empty; //Authorization information, etc. to be printed on the merchant's receipt.
        protected string m_additionalTextForGeneralError = string.Empty; //Error message returned.
        protected string m_additionalTextForPaymentResolutonNotes = string.Empty; //Notes entered when payment error is resolved.
        #endregion

        #region Constructors
        public SaleTender()
        {
        }

        public SaleTender(int registerReceiptTenderID,
                            int registerReceiptID,
                            DateTime dateTime,
                            TenderType tenderTypeID,
                            int tenderSubTypeID,
                            TransactionType transactionTypeID,
                            string IsoCode,
                            decimal amount,
                            decimal defaultAmount,
                            decimal defaultTax,
                            string referenceNumber,
                            string authorizationCode,
                            string receiptDescription,
                            int originalRegisterReceiptTenderID,
                            decimal exchangeRate,
                            string customerText,
                            string merchantText,
                            string errorMessage
                         )
        {
            m_registerReceiptTenderID = registerReceiptTenderID;
            m_registerReceiptID = registerReceiptID;
            m_DTStamp = dateTime;
            m_tenderTypeID = tenderTypeID;
            m_tenderSubTypeID = tenderSubTypeID;
            m_transactionTypeID = transactionTypeID;
            m_IsoCode = IsoCode;
            m_amount = amount;
            m_defaultAmount = defaultAmount;
            m_defaultTax = defaultTax;
            m_referenceNumber = referenceNumber;
            m_authorizationCode = authorizationCode;
            m_receiptDescription = receiptDescription;
            m_originalRegisterReceiptTenderID = originalRegisterReceiptTenderID;
            m_exchangeRate = exchangeRate;
            m_additionalTextForCustomerReceipt = customerText;
            m_additionalTextForMerchantReceipt = merchantText;
            m_additionalTextForGeneralError = errorMessage;
        }

        public SaleTender(SaleTender st)
        {
            m_registerReceiptTenderID = st.RegisterReceiptTenderID;
            m_registerReceiptID = st.RegisterReceiptID;
            m_DTStamp = st.DTStamp;
            m_tenderTypeID = st.TenderTypeID;
            m_tenderSubTypeID = st.TenderSubTypeID;
            m_transactionTypeID = st.TransactionTypeID;
            m_IsoCode = st.IsoCode;
            m_amount = st.Amount;
            m_defaultAmount = st.DefaultAmount;
            m_defaultTax = st.DefaultTax;
            m_exchangeRate = st.ExchangeRate;
            m_referenceNumber = st.ReferenceNumber;
            m_authorizationCode = st.AuthorizationCode;
            m_receiptDescription = st.ReceiptDescription;
            m_originalRegisterReceiptTenderID = st.OriginalRegisterReceiptTenderID;
            m_additionalTextForCustomerReceipt = st.AdditionalCustomerText;
            m_additionalTextForMerchantReceipt = st.AdditionalMerchantText;
            m_additionalTextForGeneralError = st.m_additionalTextForGeneralError;
            m_additionalTextForPaymentResolutonNotes = st.m_additionalTextForPaymentResolutonNotes;
        }

        #endregion

        #region Member methods

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            string s;

            if(m_referenceNumber.Contains("%"))
                s = "Invoice: " + m_referenceNumber.Substring(0, m_referenceNumber.IndexOf('%'));
            else
                s = m_receiptDescription;

                        
            return s;
        }

        #endregion

        #region Member properties
        public int RegisterReceiptTenderID
        {
            get
            {
                return m_registerReceiptTenderID;
            }

            set
            {
                m_registerReceiptTenderID = value;
            }
        }

        public int RegisterReceiptID
        {
            get
            {
                return m_registerReceiptID;
            }

            set
            {
                m_registerReceiptID = value;
            }
        }

        public DateTime DTStamp
        {
            get
            {
                return m_DTStamp;
            }

            set
            {
                m_DTStamp = value;
            }
        }
        
        public TenderType TenderTypeID
        {
            get
            {
                return m_tenderTypeID;
            }

            set
            {
                m_tenderTypeID = value;
            }
        }

        public int TenderSubTypeID
        {
            get
            {
                return m_tenderSubTypeID;
            }

            set
            {
                m_tenderSubTypeID = value;
            }
        }
    
        public TransactionType TransactionTypeID
        {
            get
            {
                return m_transactionTypeID;
            }

            set
            {
                m_transactionTypeID = value;
            }
        }
            
        public string IsoCode
        {
            get
            {
                return m_IsoCode;
            }

            set
            {
                m_IsoCode = value;
            }
        }
        
        public decimal Amount
        {
            get
            {
                return m_amount;
            }

            set
            {
                m_amount = value;
            }
        }
        
        public decimal DefaultAmount
        {
            get
            {
                return m_defaultAmount;
            }

            set
            {
                m_defaultAmount = value;
            }
        }

        public decimal DefaultTax
        {
            get
            {
                return m_defaultTax;
            }

            set
            {
                m_defaultTax = value;
            }
        }

        public string ReferenceNumber
        {
            get
            {
                return m_referenceNumber;
            }

            set
            {
                m_referenceNumber = value;
            }
        }
        
        public string AuthorizationCode
        {
            get
            {
                return m_authorizationCode;
            }

            set
            {
                m_authorizationCode = value;
            }
        }
        
        public string ReceiptDescription
        {
            get
            {
                return m_receiptDescription;
            }

            set
            {
                m_receiptDescription = value;
            }
        }
        
        public int OriginalRegisterReceiptTenderID
        {
            get
            {
                return m_originalRegisterReceiptTenderID;
            }

            set
            {
                m_originalRegisterReceiptTenderID = value;
            }
        }

        public decimal ExchangeRate
        {
            get
            {
                return m_exchangeRate;
            }

            set
            {
                m_exchangeRate = value;
            }
        }

        public string AdditionalCustomerText
        {
            get
            {
                return m_additionalTextForCustomerReceipt;
            }

            set
            {
                m_additionalTextForCustomerReceipt = value;
            }
        }

        public string AdditionalMerchantText
        {
            get
            {
                return m_additionalTextForMerchantReceipt;
            }

            set
            {
                m_additionalTextForMerchantReceipt = value;
            }
        }

        public string AdditionalErrorText
        {
            get
            {
                return m_additionalTextForGeneralError;
            }

            set
            {
                m_additionalTextForGeneralError = value;
            }
        }

        public string AdditionalTextForPaymentResolutionNotes
        {
            get
            {
                return m_additionalTextForPaymentResolutonNotes;
            }

            set
            {
                m_additionalTextForPaymentResolutonNotes = value;
            }
        }

        public bool IsUnresolvedPayment
        {
            get;
            set;
        }

        public bool IsResolvedPayment
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// The abstract base class from which all receipts should derive.
    /// </summary>
    public abstract class Receipt
    {
        #region Constants and Data Types
        // Text Lengths - 80mm+
        protected const int NormalFontMediumMaxChars = 36;
        protected const int NormalFontSmallMaxChars = 40;

        // Text Lengths - 58mm
        protected const int SmallFontMediumMaxChars = 31;
        protected const int SmallFontSmallMaxChars = 32;

        // Fonts - 80mm
        protected readonly Font NormalFontExtraHuge = new Font("Lucida Console", 20F, FontStyle.Bold); // FIX: DE3136 - Pack number needs to be bigger.
        protected readonly Font NormalFontHuge = new Font("Lucida Console", 12F, FontStyle.Bold); // FIX: DE3136 - Pack number needs to be bigger.
        protected readonly Font NormalFontBig = new Font("Lucida Console", 10F, FontStyle.Bold);
        protected readonly Font NormalFontMedium = new Font("Lucida Console", 9F);
        protected readonly Font NormalFontSmall = new Font("Lucida Console", 8F);
        protected readonly Font NormalFontTiny = new Font("Lucida Console", 5F);

        // Fonts - 58mm
        protected readonly Font SmallFontExtraHuge = new Font("Lucida Console", 14F, FontStyle.Bold); // FIX: DE3136
        protected readonly Font SmallFontHuge = new Font("Lucida Console", 10F, FontStyle.Bold); // FIX: DE3136
        protected readonly Font SmallFontBig = new Font("Lucida Console", 8F, FontStyle.Bold);
        protected readonly Font SmallFontMedium = new Font("Lucida Console", 7F, FontStyle.Bold);
        protected readonly Font SmallFontSmall = new Font("Lucida Console", 7F);
        protected readonly Font SmallFontAlmostTiny = new Font("Lucida Console", 6.25F);
        protected readonly Font SmallFontTiny = new Font("Lucida Console", 5F);

        protected class TransactionCurrencyInfo
        {
            public string isoCode;
            public decimal exchangeRate;

            public TransactionCurrencyInfo()
            {
                isoCode = string.Empty;
                exchangeRate = 0M;
            }

            public TransactionCurrencyInfo(string isoCode, decimal exchangeRate)
            {
                this.isoCode = isoCode;
                this.exchangeRate = exchangeRate;
            }
        }
        #endregion

        #region Member Variables
        protected Printer m_printer;
        protected bool m_useSmallSizes;
        protected int m_fontMediumMaxChars;
        protected int m_fontSmallMaxChars;
        protected bool m_saleSuccess;
        protected string m_machineDesc = null;
        protected bool m_printPlayerIdentityAsAccountNumber = false;
        protected bool m_printPlayerID = true;

        protected Font m_fontExtraHuge;
        protected Font m_fontHuge; // FIX: DE3136 
        protected Font m_fontBig;
        protected Font m_fontMedium;
        protected Font m_fontSmall;
        protected Font m_fontAlmostTiny;
        protected Font m_fontTiny;
        #endregion

        #region Member Methods
        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected virtual void SetTextSizes(bool useSmallSizes)
        {
            m_useSmallSizes = useSmallSizes;

            if (m_useSmallSizes)
            {
                m_fontMediumMaxChars = SmallFontMediumMaxChars;
                m_fontSmallMaxChars = SmallFontSmallMaxChars;
                m_fontExtraHuge = SmallFontExtraHuge;
                m_fontHuge = SmallFontHuge; // FIX: DE3136
                m_fontBig = SmallFontBig;
                m_fontMedium = SmallFontMedium;
                m_fontSmall = SmallFontSmall;
                m_fontAlmostTiny = SmallFontAlmostTiny;
                m_fontTiny = SmallFontTiny;
            }
            else
            {
                m_fontMediumMaxChars = NormalFontMediumMaxChars;
                m_fontSmallMaxChars = NormalFontSmallMaxChars;
                m_fontExtraHuge = NormalFontExtraHuge;
                m_fontHuge = NormalFontHuge; // FIX: DE3136
                m_fontBig = NormalFontBig;
                m_fontMedium = NormalFontMedium;
                m_fontSmall = NormalFontSmall;
                m_fontAlmostTiny = NormalFontSmall;
                m_fontTiny = NormalFontTiny;
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Converts the pack number to a string.
        /// </summary>
        /// <param name="packNumber">The pack number.</param>
        /// <returns>The string representing the pack number.</returns>
        public static string PackNumberToString(int packNumber)
        {
            string returnVal = packNumber.ToString("D9").ToString(CultureInfo.CurrentCulture);

            if (returnVal.Length > 4)
                returnVal = returnVal.Insert(3, "-");

            if (returnVal.Length > 8)
                returnVal = returnVal.Insert(7, "-");

            return returnVal;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets whether to use small fonts on the receipt.
        /// </summary>
        public bool UseSmallSizes
        {
            get
            {
                return m_useSmallSizes;
            }
        }

        public bool SaleSuccess
        {
            get
            {
                return m_saleSuccess;
            }
            set
            {
                m_saleSuccess = value;
            }
        }
        
        public string MachineDesc
        {
            get
            {
                if (m_machineDesc == null) //assume this machine
                {
                    ModuleComm modComm = new ModuleComm();

                    m_machineDesc = modComm.GetMachineDescription();
                }

                return m_machineDesc;
            }

            set
            {
                m_machineDesc = value;
            }
        }
        #endregion
    }

    // Rally US1755
    /// <summary>
    /// Represents a printed sales receipt.
    /// </summary>
    public class SalesReceipt : Receipt, IEquatable<SalesReceipt>
    {
        #region Constants and Data Types
        protected const int SpacesBetweenCards = 1;

        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalPlayerLength = 16;
        protected const int NormalDeviceFeeLength = 12;
        protected const int NormalTaxesLength = 12;
        protected const int NormalTotalLength = 12;
        protected const int NormalTenderedLength = 12;
        protected const int NormalChangeLength = 12;
        protected const int NormalPtsRedeemedLength = 12;
        protected const int NormalTotalCardsColumns = 3;
        protected const int NormalTotalCardsGameLength = 3;
        protected const int NormalTotalCardsLength = 4;
        protected const int NormalCardNumberColumns = 4;
        protected const int NormalCardNumberGameLength = 3;
        protected const int NormalCardNumberColumn1Length = 9;
        protected const int NormalCardNumberLength = 7;
        protected const int NormalCBBCardNumbersPerLine = 8; // Rally US505, US2228
        protected const int NormalPaperPackInfoColumnLength = 10;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallPlayerLength = 16;
        protected const int SmallDeviceFeeLength = 12;
        protected const int SmallTaxesLength = 12;
        protected const int SmallTotalLength = 12;
        protected const int SmallTenderedLength = 12;
        protected const int SmallChangeLength = 12;
        protected const int SmallPtsRedeemedLength = 12;
        protected const int SmallTotalCardsColumns = 2;
        protected const int SmallTotalCardsGameLength = 3;
        protected const int SmallTotalCardsLength = 4;
        protected const int SmallCardNumberColumns = 3;
        protected const int SmallCardNumberGameLength = 3;
        protected const int SmallCardNumberColumn1Length = 9;
        protected const int SmallCardNumberLength = 7;
        protected const int SmallCBBCardNumbersPerLine = 8; // Rally US505
        protected const int SmallPaperPackInfoColumnLength = 10;
        #endregion

        #region Member Variables
        protected int m_headerColumn1Length;
        protected int m_headerColumn2Length;
        protected int m_headerColumn3Length;
        protected int m_headerMachineDescLength = 17;
        protected int m_playerLength;
        protected int m_deviceFeeLength;
        protected int m_taxesLength;
        protected int m_totalLength;
        protected int m_nonTaxedCouponLength;
        protected int m_tenderedLength;
        protected int m_changeLength;
        protected int m_ptsRedeemedLength;
        protected int m_totalCardsColumns;
        protected int m_totalCardsGameLength;
        protected int m_totalCardsLength;
        protected int m_cardNumberColumns;
        protected int m_cardNumberGameLength;
        protected int m_cardNumberColumn1Length;
        protected int m_paperPackInfoColumnLength;
        protected int m_cardNumberLength;
        protected int m_cbbCardNumbersPerLine; // Rally US505

        protected bool m_isReprint;
        protected bool m_isReturn; // Rally DE1863 - Add the wording "Return" on a receipt printed when in return mode.
        protected string m_operatorHeaderLine1;
        protected string m_operatorHeaderLine2;
        protected string m_operatorHeaderLine3;
        protected string m_operatorFooterLine1;
        protected string m_operatorFooterLine2;
        protected string m_operatorFooterLine3;
        protected string m_incompleteTransactionReceiptText1;
        protected string m_incompleteTransactionReceiptText2;
        protected int m_registerReceiptId; // Rally US505
        protected int m_number;
        protected DateTime m_gamingDate;
        protected DateTime m_transactionDate;
        protected int m_soldFromMachineId;
        protected string m_cashier;
        protected int m_packNumber;
        protected string m_deviceName;
        protected short m_unitNumber;
        // Rally US1638
        protected int m_machineId;
        protected string m_clientId;
        // END: US1638
        protected string m_serialNumber;
        protected bool m_machineAccount;
        protected int m_playerId;
        protected string m_playerIdentity;
        protected string m_playerName;
        protected decimal m_playerPts;
        protected decimal m_playerPtsEarned;
        protected decimal m_playerPtsRedeem;
        protected decimal m_qualifyingAmountForPoints;
        protected decimal m_pointsForQualifyingAmount;
        protected List<LineDetail> m_lineItems = new List<LineDetail>();
        // Rally TA7464
        protected string m_defaultCurrency;
        protected string m_saleCurrency;
        protected decimal m_exchangeRate;
        protected decimal m_grandTotal;
        protected decimal m_changeDue;
        private TransactionCurrencyInfo[] m_currenciesTendered;
        // END: TA7464
        protected decimal m_deviceFee;
        protected decimal m_prepaidAmount;
        protected decimal m_taxes;
        protected decimal m_total;
        protected decimal m_nonTaxedCouponTotal;
        protected decimal m_amountTendered;
        protected string m_disclaimer1;
        protected string m_disclaimer2;
        protected string m_disclaimer3;
        protected bool m_printLotto;
        protected BingoSession[] m_bingoSessions;
        protected List<BingoCard> m_sameCards = new List<BingoCard>();
        protected List<SessionCharity> m_charities = new List<SessionCharity>();
        protected List<string> m_paperPackInfoItems = new List<string>();
        protected SaleTender[] m_saleTenders;
        protected List<string> m_afterReceiptText = new List<string>();
        protected List<Tuple<string, int, DateTime>> m_drawingEntries = new List<Tuple<string, int, DateTime>>();
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two SalesReceipt instances are equal. 
        /// </summary>
        /// <param name="obj">The SalesReceipt to compare with the 
        /// current SalesReceipt.</param>
        /// <returns>true if the specified SalesReceipt is equal to the current 
        /// SalesReceipt; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            SalesReceipt receipt = obj as SalesReceipt;

            if (receipt == null)
                return false;
            else
                return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a SalesReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current SalesReceipt.</returns>
        public override int GetHashCode()
        {
            return m_number.GetHashCode();
        }

        /// <summary>
        /// Determines whether two SalesReceipt instances are equal. 
        /// </summary>
        /// <param name="other">The SalesReceipt to compare with the 
        /// current SalesReceipt.</param>
        /// <returns>true if the specified SalesReceipt is equal to the current 
        /// SalesReceipt; otherwise, false. </returns>
        public bool Equals(SalesReceipt other)
        {
            return (other != null && m_number == other.m_number);
        }

        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
                m_headerColumn3Length = SmallHeaderColumn3Length;
                m_playerLength = SmallPlayerLength;
                m_deviceFeeLength = SmallDeviceFeeLength;
                m_taxesLength = SmallTaxesLength;
                m_totalLength = SmallTotalLength;
                m_tenderedLength = SmallTenderedLength;
                m_changeLength = SmallChangeLength;
                m_ptsRedeemedLength = SmallPtsRedeemedLength;
                m_totalCardsColumns = SmallTotalCardsColumns;
                m_totalCardsGameLength = SmallTotalCardsGameLength;
                m_totalCardsLength = SmallTotalCardsLength;
                m_cardNumberColumns = SmallCardNumberColumns;
                m_cardNumberGameLength = SmallCardNumberGameLength;
                m_cardNumberColumn1Length = SmallCardNumberColumn1Length;
                m_cardNumberLength = SmallCardNumberLength;
                m_cbbCardNumbersPerLine = SmallCBBCardNumbersPerLine; // Rally US505
                m_paperPackInfoColumnLength = SmallPaperPackInfoColumnLength;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
                m_headerColumn3Length = NormalHeaderColumn3Length;
                m_playerLength = NormalPlayerLength;
                m_deviceFeeLength = NormalDeviceFeeLength;
                m_taxesLength = NormalTaxesLength;
                m_totalLength = NormalTotalLength;
                m_tenderedLength = NormalTenderedLength;
                m_changeLength = NormalChangeLength;
                m_ptsRedeemedLength = NormalPtsRedeemedLength;
                m_totalCardsColumns = NormalTotalCardsColumns;
                m_totalCardsGameLength = NormalTotalCardsGameLength;
                m_totalCardsLength = NormalTotalCardsLength;
                m_cardNumberColumns = NormalCardNumberColumns;
                m_cardNumberGameLength = NormalCardNumberGameLength;
                m_cardNumberColumn1Length = NormalCardNumberColumn1Length;
                m_cardNumberLength = NormalCardNumberLength;
                m_cbbCardNumbersPerLine = NormalCBBCardNumbersPerLine; // Rally US505
                m_paperPackInfoColumnLength = NormalPaperPackInfoColumnLength;
            }
        }

        /// <summary>
        /// Adds a sale line item to the receipt.
        /// </summary>
        public void AddLineItem(LineDetail line)
        {
            m_lineItems.Add(line);
        }

        //US2826
        /// <summary>
        /// Adds a paper pack info item to the receipt.
        /// </summary>
        public void AddPaperPackInfo(string text)
        {
            m_paperPackInfoItems.Add(text);
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber) )
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1))
                m_printer.AddLine(m_operatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine2))
                m_printer.AddLine(m_operatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine3))
                m_printer.AddLine(m_operatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine2) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected virtual void PrintHeader()
        {
            if (!SaleSuccess)
            {
                if (!string.IsNullOrWhiteSpace(IncompleteTransactionLine1))
                    m_printer.AddLine(IncompleteTransactionLine1, StringAlignment.Center, m_fontExtraHuge);

                if (!string.IsNullOrWhiteSpace(IncompleteTransactionLine2))
                    m_printer.AddLine(IncompleteTransactionLine2, StringAlignment.Center, m_fontExtraHuge);
            }


            // PDTS 964
            // Rally DE1863
            if (m_isReturn)
                m_printer.AddLine(Resources.ReturnReceiptTitle, StringAlignment.Center, m_fontBig);
            else
            {
                var title = IsPreSale ? Resources.PresaleReceiptTitle : Resources.SaleReceiptTitle;
                m_printer.AddLine(title, StringAlignment.Center, m_fontBig);
            }
            

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            PrintSaleHeader();

            if (IsPreSale)
            {
                m_printer.AddLine(PresalesProgramName, StringAlignment.Center, m_fontBig);
                m_printer.AddLine(PresalesGamingDate.ToShortDateString(), StringAlignment.Center, m_fontBig);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            PrintCharityHeader();
        }

        /// <summary>
        /// Prints the sale header information.
        /// </summary>
        protected void PrintSaleHeader()
        {
            string temp;

            // Receipt Number & Sold From Machine Id
            temp = Resources.ReceiptNumber.PadRight(m_headerColumn1Length) + m_number.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_soldFromMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_soldFromMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Gaming Date
            temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString();

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length+m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Cashier (Staff)
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + m_cashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            if (m_isReprint && m_transactionDate != null)
            {
                temp = Resources.ReceiptTransactionDate.PadRight(m_headerColumn1Length) + m_transactionDate.ToString();
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints information related to the charities sold to on the receipt
        /// </summary>
        protected void PrintCharityHeader()
        {
            if (m_charities != null && m_charities.Count > 0)
            {
                string temp;

                m_printer.AddLine(Resources.ReceiptChaityTitle, StringAlignment.Center, m_fontBig);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);

                for (int n = 0; n < m_charities.Count; ++n)
                {
                    temp = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSession, m_charities[n].Session).PadRight(m_headerColumn1Length);
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = Resources.ReceiptCharityName.PadRight(m_headerColumn1Length) + m_charities[n].Name;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = Resources.ReceiptCharityLicense.PadRight(m_headerColumn1Length) + m_charities[n].LicenseNumber;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = Resources.ReceiptCharityTaxId.PadRight(m_headerColumn1Length) + m_charities[n].TaxPayerId;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);
                }

                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints information related to the unit sold.
        /// </summary>
        protected void PrintUnitInfo()
        {
            // Pack Number
            if (m_packNumber != 0)
            {
                m_printer.AddLine(Resources.ReceiptPackNumber, StringAlignment.Center, m_fontBig); // FIX: DE3136
                m_printer.AddLine(PackNumberToString(m_packNumber), StringAlignment.Center, m_fontExtraHuge); // FIX: DE3136
            }

            // Device Name
            if (!string.IsNullOrEmpty(m_deviceName))
                m_printer.AddLine(m_deviceName, StringAlignment.Center, m_fontBig);

            // Unit Number
            if (m_unitNumber != 0)
                m_printer.AddLine(Resources.ReceiptUnitNumber + " " + m_unitNumber.ToString(CultureInfo.CurrentCulture), StringAlignment.Center, m_fontBig);

            // Rally US1638
            if (!m_machineAccount && m_machineId != 0)
                m_printer.AddLine(Resources.ReceiptMachineName + " " + m_machineId.ToString(CultureInfo.CurrentCulture), StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_clientId))
                m_printer.AddLine(Resources.ReceiptClientId + " " + m_clientId, StringAlignment.Center, m_fontBig);
            // END: US1638

            // Serial Number
            if (!string.IsNullOrEmpty(m_serialNumber))
                m_printer.AddLine(Resources.ReceiptSerialNumber + " " + m_serialNumber, StringAlignment.Center, m_fontBig);

            // Pack / Device Seperator
            if (m_packNumber != 0 || !string.IsNullOrEmpty(m_deviceName) ||
               m_unitNumber != 0 || !string.IsNullOrEmpty(m_serialNumber))
            {
                m_printer.AddLine("--------------------------", StringAlignment.Center, m_fontMedium); // FIX: DE3136
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints information related to the player sold to.
        /// </summary>       
        /// <param name="printPointInfo">Tuple with whether to print player point 
        /// information, player tracking interface ID, 
        /// if external system does the rating,
        /// and if we do the rating.</param>
        protected void PrintPlayerInfo(Tuple<bool, int, bool, bool> printPointInfo, bool printPtsRedeemed)
        {
            // TTP 50114
            // Player Id
            if (!m_machineAccount && m_printPlayerID && m_playerId != 0)
                m_printer.AddLine(Resources.ReceiptPlayerNumber.PadRight(m_playerLength) + m_playerId.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontMedium);
            
            if (m_printPlayerIdentityAsAccountNumber && !m_machineAccount && !string.IsNullOrWhiteSpace(m_playerIdentity))
                m_printer.AddLine(Resources.ReceiptPlayerIdentity.PadRight(m_playerLength) + m_playerIdentity, StringAlignment.Near, m_fontMedium);

            // Player Name
            if (!string.IsNullOrEmpty(m_playerName))
            {
                if (m_machineAccount)
                    m_printer.AddLine(Resources.ReceiptMachineName.PadRight(m_playerLength) + m_playerName, StringAlignment.Near, m_fontMedium);
                else
                    m_printer.AddLine(Resources.ReceiptPlayerName.PadRight(m_playerLength) + m_playerName, StringAlignment.Near, m_fontMedium);
            }

            // Player Info and Spacing.
            if ((!m_machineAccount && m_playerId != 0) || !string.IsNullOrEmpty(m_playerName))
            {
                if (!m_machineAccount && printPointInfo.Item1)
                {
                    string pointsStr = m_playerPts.ToString("N", CultureInfo.CurrentCulture);

                    if (m_playerPtsEarned != 0M || m_pointsForQualifyingAmount != 0M || m_playerPtsRedeem != 0M)
                    {
                        string earnedStr = m_playerPtsEarned.ToString("N", CultureInfo.CurrentCulture);
                        string earnedFromQualStr = m_pointsForQualifyingAmount.ToString("N", CultureInfo.CurrentCulture);
                        string redeemedStr = m_playerPtsRedeem.ToString("N", CultureInfo.CurrentCulture);
                        string qualifyingAmtStr = m_qualifyingAmountForPoints.ToString("N", CultureInfo.CurrentCulture);

                        decimal total = m_playerPts + m_playerPtsEarned + m_pointsForQualifyingAmount - m_playerPtsRedeem;
                        string totalStr = total.ToString("N", CultureInfo.CurrentCulture);

                        // Find out which value is the longest.
                        int max = Math.Max(pointsStr.Length, earnedStr.Length);
                        max = Math.Max(max, redeemedStr.Length);
                        max = Math.Max(max, totalStr.Length);
                        max = Math.Max(max, earnedFromQualStr.Length);
                        max = Math.Max(max, qualifyingAmtStr.Length);

                        // Now pad the strings.
                        pointsStr = pointsStr.PadLeft(max);
                        redeemedStr = redeemedStr.PadLeft(max);
                        earnedStr = earnedStr.PadLeft(max);
                        totalStr = totalStr.PadLeft(max);
                        earnedFromQualStr = earnedFromQualStr.PadLeft(max);
                        qualifyingAmtStr = qualifyingAmtStr.PadLeft(max);

                        if (m_qualifyingAmountForPoints != 0M && (printPointInfo.Item3 || printPointInfo.Item4)) //show the qualifying amount if there is one and someone is doing a rating
                            m_printer.AddLine(Resources.ReceiptPointQualifyingAmt.PadRight(m_playerLength) + qualifyingAmtStr, StringAlignment.Near, m_fontMedium);

                        if(printPointInfo.Item2 == 0) //no third party system, we can show the point balance
                            m_printer.AddLine(Resources.ReceiptPlayerPoints.PadRight(m_playerLength) + pointsStr, StringAlignment.Near, m_fontMedium);

                        if (m_playerPtsRedeem != 0M && ((m_playerPtsEarned != 0M && !printPointInfo.Item3) || printPointInfo.Item2 == 0)) //we have redeemed points to print and they will not be printed alone
                            m_printer.AddLine(Resources.ReceiptPlayerPointsUsed.PadRight(m_playerLength) + redeemedStr, StringAlignment.Near, m_fontMedium);

                        if (m_playerPtsEarned != 0M && !printPointInfo.Item3) //no third party system doing ratings, we can show the points earned
                            m_printer.AddLine(Resources.ReceiptPlayerPointsEarned.PadRight(m_playerLength) + earnedStr, StringAlignment.Near, m_fontMedium);

                        if (m_pointsForQualifyingAmount != 0M && !printPointInfo.Item3) //we are doing the rating, show the points from the qualifying amount if there are any
                            m_printer.AddLine(Resources.ReceiptPointsFromQualifyingAmt.PadRight(m_playerLength) + earnedFromQualStr, StringAlignment.Near, m_fontMedium);

                        if (printPointInfo.Item2 == 0) //no third party system, we can show the new point balance
                            m_printer.AddLine(Resources.ReceiptPlayerPointTotal.PadRight(m_playerLength) + totalStr, StringAlignment.Near, m_fontMedium);
                    }
                    else
                    {
                        if (printPointInfo.Item2 == 0) //no third party system, we can show the point balance
                            m_printer.AddLine(Resources.ReceiptPlayerPoints.PadRight(m_playerLength) + pointsStr, StringAlignment.Near, m_fontMedium);
                    }
                }

                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        protected void PrintDrawingEntryInfo()
        {
            if (DrawingEntries.Count == 0)
                return;

            Font ourFont = m_fontMedium;
            string longestText = "";
            float widest = 0;
            List<string> drawingText = new List<string>();

            drawingText.Add("Drawings:");

            //fit the text
            foreach (Tuple<string, int, DateTime> entry in DrawingEntries)
            {
                string text = entry.Item1;
                float w = m_printer.MeasureString(text, ourFont, StringAlignment.Near).Width;

                if (w > widest)
                {
                    widest = w;
                    longestText = text;
                }

                drawingText.Add(text);

                if (entry.Item3 != DateTime.MinValue)
                    text = "On " + entry.Item3.ToString("MM/dd/yy");
                else
                    text = "           ";

                text += "  Entries= " + entry.Item2.ToString();
                w = m_printer.MeasureString(text, ourFont, StringAlignment.Near).Width;

                if (w > widest)
                {
                    widest = w;
                    longestText = text;
                }

                drawingText.Add(text);
                drawingText.Add(" ");
            }

            if ((int)widest >= (int)m_printer.PrintableArea.Width)
            {
                float size = ourFont.Size - 1;

                ourFont = new Font(ourFont.FontFamily, size, FontStyle.Regular);

                while ((int)m_printer.MeasureString(longestText, ourFont, StringAlignment.Near).Width >= (int)m_printer.PrintableArea.Width && size > 2)
                {
                    size--;
                    ourFont = new Font(ourFont.FontFamily, size, FontStyle.Regular);
                }
            }

            foreach (string text in drawingText)
                m_printer.AddLine(text, StringAlignment.Near, ourFont);

            m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints information about the sale.
        /// </summary>
        /// <param name="printPointInfo">Whether to print player point information.</param>
        /// <param name="printSignatureLine">Whether to print a signature 
        /// line.</param>
        /// <param name="printPtsRedeemed">Whether to print the Points Redeemed
        /// header.</param>
        protected void PrintSaleInfo(bool printPointInfo, bool printSignatureLine, bool printPtsRedeemed, bool merchantCopy)
        {
            string temp;
            int headerWidth = m_fontMediumMaxChars;
            Font receiptBodyFont = m_useSmallSizes? m_fontAlmostTiny : m_fontSmall;

            // PDTS 584
            // US2139
            // Item Header
            if (!m_useSmallSizes && printPtsRedeemed)
            {
                m_printer.AddLine(Resources.ReceiptSalesItemHeader1, StringAlignment.Near, receiptBodyFont);
                m_printer.AddLine(Resources.ReceiptSalesItemHeader2, StringAlignment.Near, receiptBodyFont);

                headerWidth = Math.Max(Resources.ReceiptSalesItemHeader1.Length, Resources.ReceiptSalesItemHeader2.Length);
            }
            else if (!m_useSmallSizes && !printPtsRedeemed)
            {
                m_printer.AddLine(Resources.ReceiptSalesItemHeaderNoPts1, StringAlignment.Near, receiptBodyFont);

                headerWidth = Resources.ReceiptSalesItemHeaderNoPts1.Length;
            }
            else if (m_useSmallSizes && printPtsRedeemed)
            {
                m_printer.AddLine(Resources.ReceiptSalesSmallItemHeader1, StringAlignment.Near, receiptBodyFont);
                m_printer.AddLine(Resources.ReceiptSalesSmallItemHeader2, StringAlignment.Near, receiptBodyFont);

                headerWidth = Math.Max(Resources.ReceiptSalesSmallItemHeader1.Length, Resources.ReceiptSalesSmallItemHeader2.Length);
            }
            else // !m_useSmallSizes && !printPtsRedeemed
            {
                m_printer.AddLine(Resources.ReceiptSalesSmallItemHeaderNoPts1, StringAlignment.Near, receiptBodyFont);
            
                headerWidth = Resources.ReceiptSalesSmallItemHeaderNoPts1.Length;
            }

            temp = string.Empty.PadRight(headerWidth, '-');
            m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);

            // Line Items in sorted order
            foreach (string item in LineDetail.GetSortedDetail(m_lineItems))
                m_printer.AddLine(item, StringAlignment.Near, receiptBodyFont);

            m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);

//RAK subtotal
            //since the pre/post tax flag for coupons is not tied to a receipt, the non-taxed coupon total
            //is determined by the tax applied to the coupon.  If the sale has no tax applied, it is assumed
            //that the tax rate is zero and non-taxed coupons are treated like taxable coupons and included 
            //in the subtotal.
            decimal nonTaxedCoupons = (m_taxes == 0 ? 0 : m_nonTaxedCouponTotal);
            
            //all total column numbers are in default currency

            // Subtotal (show taxable total)
            string txt = m_useSmallSizes ? Resources.ReceiptSmallSubtotal : Resources.ReceiptSubtotal;

            temp = txt.PadLeft(m_fontSmallMaxChars - 2*m_totalLength);
            temp += Currency.FormatCurrencyString(m_defaultCurrency, m_total-m_taxes-m_deviceFee-nonTaxedCoupons+m_prepaidAmount).PadLeft(2*m_totalLength);
            m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);

            // Taxes (tax on subtotal)
            if (m_taxes != 0M)
            {
                temp = Resources.ReceiptSalesTaxes.PadLeft(m_fontSmallMaxChars - 2*m_taxesLength);
                temp += Currency.FormatCurrencyString(m_defaultCurrency, m_taxes).PadLeft(2*m_taxesLength);
                m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);
            }

            // Device Fee (not taxed)
            if (m_deviceFee != 0M)
            {
                txt = m_useSmallSizes ? Resources.ReceiptSmallSalesDeviceFee : Resources.ReceiptSalesDeviceFee;
                temp = txt.PadLeft(m_fontSmallMaxChars - 2 * m_deviceFeeLength);
                temp += Currency.FormatCurrencyString(m_defaultCurrency, m_deviceFee).PadLeft(2*m_deviceFeeLength);
                m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);
            }

            //Non-taxed coupons (coupons applied after tax based on global setting)
            if (nonTaxedCoupons != 0) //we have some coupons applied after the tax
            {
                temp = Resources.ReceiptCoupons.PadLeft(m_fontSmallMaxChars - 2*m_totalLength);
                temp += Currency.FormatCurrencyString(m_defaultCurrency, nonTaxedCoupons).PadLeft(2*m_totalLength);
                m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);
            }

            //Prepaid amount (includes taxes)
            if (m_prepaidAmount != 0) //we have a prepaid amount
            {
                temp = Resources.ReceiptPrepaidAmount.PadLeft(m_fontSmallMaxChars - 2 * m_totalLength);
                temp += Currency.FormatCurrencyString(m_defaultCurrency, -m_prepaidAmount).PadLeft(2 * m_totalLength);
                m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);
            }

            // Rally TA7464
            // Total
            txt = m_useSmallSizes ? Resources.ReceiptSmallSalesTotal : Resources.ReceiptSalesTotal;
            temp = string.Format(CultureInfo.CurrentCulture, txt.PadLeft(m_fontSmallMaxChars - 2*m_totalLength), m_defaultCurrency);
            temp += Currency.FormatCurrencyString(m_defaultCurrency, m_total).PadLeft(2*m_totalLength);
            m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);

            // Tenders
            if (m_saleTenders != null && m_saleTenders.Length > 0) //show the individual tenders
            {
                string desc;
                string altAmount;
                string amount;
                string authCode;
                int tenderCount = 0;
                bool noAltAmount = !m_saleTenders.ToList().Exists(t => t.IsoCode != m_defaultCurrency);

                //show each tender
                foreach (SaleTender st in m_saleTenders)
                {
                    if (st.ReceiptDescription == "Error" || st.ReceiptDescription == "Reconciliation text" || (st.TransactionTypeID == TransactionType.Void && st.RegisterReceiptID != m_registerReceiptId)) //don't show tenders that failed or are from voiding a PREVIOUS sale
                        continue;

                    tenderCount++;

                    //build the description
                    desc = (st.ReceiptDescription + ":").PadLeft(m_fontSmallMaxChars - 2 * m_tenderedLength);

                    //build the alternate currency amount
                    if (st.IsoCode == m_defaultCurrency)
                        altAmount = "".PadLeft(m_tenderedLength);
                    else
                        altAmount = (Currency.FormatCurrencyString(st.IsoCode, st.Amount) + " =").PadLeft(m_tenderedLength);

                    //build the default currency amount
                    amount = Currency.FormatCurrencyString(m_defaultCurrency, st.DefaultAmount).PadLeft(m_tenderedLength);

                    //build the auth code
                    authCode = string.Empty;

                    if (noAltAmount) //put the auth code where the alt currency would be
                    {
                        altAmount = st.AuthorizationCode.PadLeft(m_tenderedLength);
                    }
                    else //auth code gets its own line
                    {
                        if (!string.IsNullOrWhiteSpace(st.AuthorizationCode))
                            authCode = Resources.AuthCode.PadLeft(m_fontSmallMaxChars - 2*m_tenderedLength)+" "+ st.AuthorizationCode;
                    }

                    //print the line, breaking it up if needed
                    if (desc.Length > m_fontSmallMaxChars - 2 * m_tenderedLength) //description too long, put on its own line
                    {
                        m_printer.AddLine(desc, StringAlignment.Near, m_fontSmall);
                        desc = "".PadLeft(m_fontSmallMaxChars - 2*m_tenderedLength);
                    }

                    if (desc.Length + altAmount.Length > m_fontSmallMaxChars-m_tenderedLength) //alternate currency too long, put on its own line
                    {
                        m_printer.AddLine(desc+altAmount, StringAlignment.Near, m_fontSmall);

                        desc = "".PadLeft(m_fontSmallMaxChars - 2 * m_tenderedLength);
                        altAmount = "".PadLeft(m_tenderedLength);
                    }

                    if (desc.Length + altAmount.Length + amount.Length > m_fontSmallMaxChars) //amount too long, put on its own line
                    {
                        if ((desc+altAmount).Trim().Length != 0)
                            m_printer.AddLine(desc + altAmount, StringAlignment.Near, m_fontSmall);

                        amount = Currency.FormatCurrencyString(m_defaultCurrency, st.DefaultAmount).PadLeft(m_fontSmallMaxChars);
                        m_printer.AddLine(amount, StringAlignment.Near, receiptBodyFont);
                        
                        desc = string.Empty;
                        altAmount = string.Empty;
                        amount = string.Empty;
                    }

                    if((desc+altAmount+amount).Trim().Length != 0)
                        m_printer.AddLine(desc + altAmount + amount, StringAlignment.Near, receiptBodyFont);

                    if(authCode != string.Empty)
                        m_printer.AddLine(authCode, StringAlignment.Near, receiptBodyFont);
                }

                //show total tendered in default currency if more than one tender
                if (tenderCount > 1)
                {
                    txt = m_useSmallSizes ? Resources.ReceiptSmallSalesTendered : Resources.ReceiptSalesTendered;
                    temp = txt.PadLeft(m_fontSmallMaxChars - 2 * m_tenderedLength);
                    temp += Currency.FormatCurrencyString(m_defaultCurrency, m_amountTendered).PadLeft(2*m_tenderedLength);
                    m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);
                }
            }

            // Change Due
            if (m_changeDue > 0M)
            {
                temp = Resources.ReceiptSalesChange.PadLeft(m_fontSmallMaxChars - 2*m_changeLength);
                temp += Currency.FormatCurrencyString(m_defaultCurrency, m_changeDue).PadLeft(2*m_changeLength);
                m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);
            }
            // END: TA7464

            // Points Redeemed
            if (printPointInfo && m_playerPtsRedeem != 0M)
            {
                txt = m_useSmallSizes ? Resources.ReceiptSmallPlayerPointsRedeem : Resources.ReceiptPlayerPointsRedeem;
                temp = txt.PadLeft(m_fontSmallMaxChars - 2*m_ptsRedeemedLength);
                temp += m_playerPtsRedeem.ToString("N", CultureInfo.CurrentCulture).PadLeft(2*m_ptsRedeemedLength);
                m_printer.AddLine(temp, StringAlignment.Near, receiptBodyFont);
            }

            //Exchange rate(s)
            if (m_defaultCurrency != SaleCurrency) //we have at least one currency that is not the default, show exchange rates
            {
                m_printer.AddLine(" ", StringAlignment.Near, m_fontSmall);

                if (SaleCurrency != string.Empty) //just one currency and it is not the default
                {
                    if (m_saleTenders != null && m_saleTenders[0].ExchangeRate != 0M) //RALLY DE 7069 print out the exchange rate even if it is 1.000
                    {
//                        // Total for this currency
//                        temp = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSalesTotal.PadLeft(m_fontSmallMaxChars - m_totalLength), m_saleCurrency);
//                        temp += Currency.FormatCurrencyString(m_saleCurrency, m_grandTotal).PadLeft(m_totalLength);
//                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                        txt = m_useSmallSizes ? Resources.ReceiptSmallSalesExchangeRate : Resources.ReceiptSalesExchangeRate;
                        temp = txt.PadLeft(m_fontSmallMaxChars - 2 * m_totalLength);
                        temp += m_currenciesTendered[0].isoCode.PadRight(m_totalLength);
                        temp += m_currenciesTendered[0].exchangeRate.ToString("0.0000", CultureInfo.CurrentCulture).PadLeft(m_totalLength);
                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                    }
                }
                else //multiple rates
                {
                    //no alternate currency total is given since we have more than one.

                    txt = m_useSmallSizes ? Resources.ReceiptSmallSalesExchangeRate : Resources.ReceiptSalesExchangeRate;
                    temp = txt.PadLeft(m_fontSmallMaxChars - 2 * m_totalLength);

                    foreach (TransactionCurrencyInfo tci in m_currenciesTendered)
                    {
                        if (tci.isoCode != m_defaultCurrency)
                        {
                            temp += tci.isoCode.PadRight(m_totalLength);
                            temp += tci.exchangeRate.ToString("0.0000", CultureInfo.CurrentCulture).PadLeft(m_totalLength);
                            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                            temp = "".PadLeft(m_fontSmallMaxChars - 2*m_totalLength);
                        }
                    }
                }
            }

            //print extra tendering text
            if (m_saleTenders != null)
            {
                foreach (SaleTender st in m_saleTenders)
                {
                    string additionalText = (merchantCopy ? st.AdditionalMerchantText : st.AdditionalCustomerText);

                    if (additionalText != string.Empty)
                    {
                        m_printer.AddLine(" ", StringAlignment.Near, m_fontSmall);

                        string[] text = additionalText.Replace("\r\n", "\n").Split('\n');

                        foreach (string s in text)
                            m_printer.AddLine(s, StringAlignment.Near, m_fontSmall);
                    }

                    if (st.IsResolvedPayment && !string.IsNullOrWhiteSpace(st.AdditionalTextForPaymentResolutionNotes))
                    {
                        m_printer.AddLine("Note:", StringAlignment.Near, m_fontSmall);

                        string[] text = st.AdditionalTextForPaymentResolutionNotes.Split('\n');

                        foreach(string s in text)
                            m_printer.AddLine(s, StringAlignment.Near, m_fontSmall);
                    }
                }
            }

            // Print a signature line.
            if (printSignatureLine)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
            }
        }

        // US2826
        /// <summary>
        /// Prints Barcoded Paper Pack Info
        /// </summary>
        private void PrintPaperPackInfo()
        {
            //check to see if we have any barcoded paper packs
            if (m_paperPackInfoItems.Count == 0)
                return;

            //extra blank lines
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //print PaperPackSold title
            m_printer.AddLine(Resources.ReceiptPaperPacksSold, StringAlignment.Center, m_fontMedium);

            //dash line
            var dashLine = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);

            //add all these lines to the receipt
            foreach(string line in m_paperPackInfoItems)
                m_printer.AddLine(line, StringAlignment.Near, m_fontSmall);

            //print dash line
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);
        }

        // US2228
        // Rally TA5749
        /// <summary>
        /// Prints game card totals and numbers.
        /// </summary>       
        /// <param name="printCardNumberMode">How to print 
        /// card/start numbers.</param>
        protected void PrintBingoGameInfo(PrintCardNumberMode printCardNumberMode)
        {
            string temp;

            if (printCardNumberMode == PrintCardNumberMode.OnlyPrintCBBCardNumbers) //see if we have anything to print
            {
                if (m_bingoSessions == null)
                    return;

                List<BingoSession> CBBSessions = new List<BingoSession>();

                foreach (BingoSession session in m_bingoSessions) //gather the sessions with CBB games
                {
                    if (session.GameCount > 0)
                    {
                        foreach (BingoGame game in session.GetGames())
                        {
                            if (game.HasCrystalBallCards)
                            {
                                CBBSessions.Add(session);
                                break; //go to next session
                            }
                        }
                    }
                }

                if (CBBSessions.Count == 0) //no sessions with CBB, nothing to print
                    return;

                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(Resources.ReceiptSalesGameCardsHeader, StringAlignment.Center, m_fontMedium);

                temp = string.Empty.PadRight(m_fontMediumMaxChars, '-');
                m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);

                foreach (BingoSession session in CBBSessions)
                {
                    // Print out the session number.
                    m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSession, session.Number), StringAlignment.Near, m_fontSmall);
                    m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);

                    bool printedCBB = false;
                    int elec = 0, paper = 0;

                    foreach (BingoGame game in session.GetGames())
                    {
                        if (game.HasCrystalBallCards)
                        {
                            if (!printedCBB)
                            {
                                m_printer.AddLine(Resources.ReceiptCBBTitle, StringAlignment.Near, m_fontSmall);
                                printedCBB = true;
                            }

                            m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptGame, game.GetDisplayNumberToString), StringAlignment.Near, m_fontSmall);
                            PrintCBBCards(game.GetCards(), ref paper, ref elec);
                            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);
                        }
                    }
                }

                return;
            }

            if (m_bingoSessions != null)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(Resources.ReceiptSalesGameCardsHeader, StringAlignment.Center, m_fontMedium);

                temp = string.Empty.PadRight(m_fontMediumMaxChars, '-');
                m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);

                foreach (BingoSession session in m_bingoSessions)
                {
                    // Does this session have any games?
                    int paperCardCount = 0;
                    int elecCardCount = 0;
                    int totalCardCount = 0;

                    if (session.GameCount > 0)
                    {
                        // Print out the session number.
                        m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSession, session.Number), StringAlignment.Near, m_fontSmall);
                        m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);

                        // FIX: TA4696 - Print out card levels on receipt.
                        // Rally US505
                        // Print out any CBB cards first.
                        bool printedCBB = false;

                        foreach (BingoGame game in session.GetGames())
                        {
                            if (game.HasCrystalBallCards)
                            {
                                if (!printedCBB)
                                {
                                    m_printer.AddLine(Resources.ReceiptCBBTitle, StringAlignment.Near, m_fontSmall);
                                    printedCBB = true;
                                }

                                m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptGame, game.GetDisplayNumberToString), StringAlignment.Near, m_fontSmall);
                                PrintCBBCards(game.GetCards(), ref paperCardCount, ref elecCardCount);
                                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);
                            }
                        }

                        // Sort all the cards in this session by level, then game.
                        // TODO Possibly move this sorting to the server/database.
                        Dictionary<CardLevel, List<BingoGame>> cardInfo = new Dictionary<CardLevel, List<BingoGame>>();

                        foreach (BingoGame game in session.GetGames())
                        {
                            totalCardCount += game.TotalCards;

                            // Only sort non-CBB games.
                            if (!game.HasCrystalBallCards)
                            {
                                foreach (BingoCard card in game.GetCards())
                                {
                                    // Do we already have this level?
                                    if (!cardInfo.ContainsKey(card.Level))
                                        cardInfo.Add(card.Level, new List<BingoGame>());

                                    List<BingoGame> gameList = cardInfo[card.Level];

                                    // Is this game already in the list?
                                    // Even though we are creating new
                                    // temporary games to hold the cards,
                                    // IndexOf still works because the copy and
                                    // original have the same linear game number.
                                    int index = gameList.IndexOf(game);

                                    if (index == -1)
                                    {
                                        //US4804
                                        gameList.Add(new BingoGame(game.Type, game.LinearNumber, game.LinearDisplayNumber, game.DisplayNumber, game.ConsecutiveCards, game.ContinuationGameCount, UseLinearDisplayNumbers));
                                        index = gameList.Count - 1;
                                    }

                                    gameList[index].AddCard(card);

                                    // Add the running total.
                                    if (card.Media == CardMedia.Electronic)
                                        elecCardCount++;
                                    else
                                        paperCardCount++;
                                }
                            }
                        }

                        if (printCardNumberMode == PrintCardNumberMode.PrintGameCounts)
                            PrintGameTotals(cardInfo);
                        else
                            PrintCardNumbers(cardInfo, printCardNumberMode);
                        // END: TA4696

                        // Print out the total cards for this session.
                        m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);

                        if (elecCardCount > 0)
                            m_printer.AddLine(Resources.ReceiptSalesSessionElecCards + " " + elecCardCount.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontSmall);

                        if (paperCardCount > 0)
                            m_printer.AddLine(Resources.ReceiptSalesSessionPaperCards + " " + paperCardCount.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontSmall);

                        m_printer.AddLine(Resources.ReceiptSalesSessionTotalCards + " " + totalCardCount.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontSmall);
                        m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);
                    }
                }
            }
        }
        // END: TA5749

        // Rally US505
        /// <summary>
        /// Prints the specified CBB cards' numbers.
        /// </summary>
        /// <param name="cards">An array of CrystalBallCards to print.</param>
        /// <param name="paperCardCount">A reference to the running total of
        /// paper cards.</param>
        /// <permission cref="elecCardCount">A reference to the running total
        /// of electronic cards.</permission>
        /// <param name="elecCardCount"></param>
        protected void PrintCBBCards(BingoCard[] cards, ref int paperCardCount, ref int elecCardCount)
        {
            string temp = null;

            if (cards != null)
            {
                foreach (CrystalBallCard cbbCard in cards)
                {
                    int numCount = 0;

                    if (cbbCard.Face != null)
                    {
                        temp = cbbCard.Number.ToString(CultureInfo.CurrentCulture).PadLeft(m_cardNumberLength);

                        if (cbbCard.Media == CardMedia.Electronic)
                        {
                            temp += Resources.CBBElecCard;
                            elecCardCount++;
                        }
                        else
                        {
                            temp += Resources.CBBPaperCard;
                            paperCardCount++;
                        }

                        for (int x = 0; x < cbbCard.Face.Length; x++)
                        {
                            if (cbbCard.Face[x] != 0)
                            {
                                temp += " " + cbbCard.Face[x].ToString("D2", CultureInfo.CurrentCulture);

                                if (++numCount >= m_cbbCardNumbersPerLine)
                                {
                                    numCount = 0;
                                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                                    temp = string.Empty.PadLeft(m_cardNumberLength + Resources.CBBElecCard.Length);
                                }
                            }
                        }
                    }

                    // Add the last line.
                    if (numCount != 0)
                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                }
            }
        }

        // FIX: TA4696
        /// <summary>
        /// Prints the numbers of cards in each game for every level.
        /// </summary>
        /// <param name="cardInfo">The level/game/card information to
        /// print.</param>
        protected void PrintGameTotals(Dictionary<CardLevel, List<BingoGame>> cardInfo)
        {
            string temp;
            int col;

            if (cardInfo != null)
            {
                foreach (KeyValuePair<CardLevel, List<BingoGame>> pair in cardInfo)
                {
                    temp = string.Empty;
                    col = 1;

                    m_printer.AddLine(pair.Key.Name, StringAlignment.Near, m_fontSmall);

                    foreach (BingoGame game in pair.Value)
                    {
                        //updated padding due to linear game #
                        string tempGameNum = game.GetDisplayNumberToString.PadLeft(m_totalCardsColumns + 2);
                        string tempCards = game.TotalCards.ToString(CultureInfo.CurrentCulture).PadLeft(m_totalCardsLength).PadRight(5);
                        temp += string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSalesGameTotalCards, tempGameNum, tempCards);

                        // Send the current totals line to the printer.
                        if (++col > m_totalCardsColumns-1)
                        {
                            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                            col = 1;
                            temp = string.Empty;
                        }
                    }

                    // Print the last line.
                    if (!string.IsNullOrEmpty(temp))
                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                }
            }
        }

        // Rally TA5749
        /// <summary>
        /// Prints the cards in each game for every level.
        /// </summary>
        /// <param name="cardInfo">The level/game/card information to
        /// print.</param>
        /// <param name="printCardNumberMode">How to print 
        /// card/start numbers.</param>
        protected void PrintCardNumbers(Dictionary<CardLevel, List<BingoGame>> cardInfo, PrintCardNumberMode printCardNumberMode)
        {
            string temp;
            int col;

            if (cardInfo != null)
            {
                foreach (KeyValuePair<CardLevel, List<BingoGame>> pair in cardInfo)
                {
                    temp = string.Empty;
                    col = 1;

                    m_printer.AddLine(pair.Key.Name, StringAlignment.Near, m_fontSmall);

                    foreach (BingoGame game in pair.Value)
                    {
                        //US4804
                        temp = string.Format(CultureInfo.CurrentCulture,
                                Resources.ReceiptGame.PadRight(11), 
                                game.GetDisplayNumberToString);
                        
                        temp += (" " + game.TotalCards.ToString(CultureInfo.CurrentCulture) + " " + Resources.ReceiptSalesCards).PadRight(1);
                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                        int[] cardNums = game.GetCardNumbers(printCardNumberMode == PrintCardNumberMode.PrintStartNumbers);

                        temp = string.Empty.PadRight(m_cardNumberColumn1Length);

                        if (cardNums.Length > 0)
                        {
                            // Do we need to print out every card or just the starts and ends?
                            if (game.ConsecutiveCards && cardNums.Length % 2 == 0)
                            {
                                for (int x = 0; x < cardNums.Length; x += 2)
                                {
                                    temp = string.Empty.PadLeft(m_cardNumberColumn1Length);
                                    temp += " " + cardNums[x].ToString(CultureInfo.CurrentCulture).PadLeft(m_cardNumberLength) + " - " + cardNums[x + 1].ToString(CultureInfo.CurrentCulture).PadLeft(m_cardNumberLength);
                                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                                }
                            }
                            else
                            {
                                col = 1;

                                foreach (int num in cardNums)
                                {
                                    // Add the card number.
                                    temp += num.ToString(CultureInfo.CurrentCulture).PadLeft(m_cardNumberLength);

                                    // Send the current numbers to the printer.
                                    if (++col > m_cardNumberColumns)
                                    {
                                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                                        col = 1;
                                        temp = string.Empty.PadRight(m_cardNumberColumn1Length);
                                    }
                                }

                                // Print the last line.
                                if (temp.Length != m_cardNumberColumn1Length)
                                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                            }
                        }
                    }
                }
            }
        }
        // END: TA5749
        // END: TA4696
        // END: US2228

        /// <summary>
        /// Prints the bingo card faces.
        /// </summary>
        protected void PrintBingoGameCards()
        {
            if (m_bingoSessions != null)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

                string[] cards = null;

                // First, print all the same cards.
                if (m_sameCards.Count > 0)
                {
                    cards = BingoCardPrinterHelper.FormatTextBingoCards(m_printer, m_fontTiny, StringAlignment.Near, m_sameCards.ToArray(), SpacesBetweenCards, m_printLotto);

                    foreach (string line in cards)
                    {
                        m_printer.AddLine(line, StringAlignment.Near, m_fontTiny);
                    }
                }

                // Now print the cards from the sessions.
                foreach (BingoSession session in m_bingoSessions)
                {
                    // Only print the cards out if they aren't in the same 
                    // cards list.
                    if (!session.SameCards && session.GameCount > 0)
                    {
                        m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSession, session.Number), StringAlignment.Near, m_fontSmall);
                        m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);

                        foreach (BingoGame game in session.GetGames())
                        {
                            m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptGame, game.GetDisplayNumberToString), StringAlignment.Near, m_fontSmall);

                            cards = BingoCardPrinterHelper.FormatTextBingoCards(m_printer, m_fontTiny, StringAlignment.Near, game.GetCards(), SpacesBetweenCards, m_printLotto);

                            foreach (string line in cards)
                            {
                                m_printer.AddLine(line, StringAlignment.Near, m_fontTiny);
                            }
                        }
                    }
                }
            }
        }

        //US4848
        /// <summary>
        /// Prints the receipt's footer lines.
        /// </summary>
        protected virtual void PrintFooter()
        {
            PrintOperatorsFooter();
            PrintIsValidated();
            PrintBarcode();

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
        }

        protected virtual void PrintAfterReceiptText()
        {
            if (AfterReceiptText.Count == 0)
                return;

            m_printer.ClearLines();

            Font ourFont = m_fontMedium;
            string longestText = "";
            float widest = 0;
            bool lastLineWasBarcode = false;

            //fit the text
            foreach (string text in AfterReceiptText)
            {
                if (!((text.Length == 3 && text.ToUpper() == "CUT") || (text.Length > 8 && text.ToUpper().Substring(0, 8) == "CODE128="))) //not a barcode or cut code
                {
                    float w = m_printer.MeasureString(text, ourFont, StringAlignment.Center).Width;

                    if (w > widest)
                    {
                        widest = w;
                        longestText = text;
                    }
                }
            }

            if (widest > m_printer.PrintableArea.Width)
            {
                float size = ourFont.Size - 1;
                
                ourFont = new Font(ourFont.FontFamily, size, FontStyle.Regular);

                while (m_printer.MeasureString(longestText, ourFont, StringAlignment.Center).Width > m_printer.PrintableArea.Width && size > 2)
                {
                    size--;
                    ourFont = new Font(ourFont.FontFamily, size, FontStyle.Regular);
                }
            }

            foreach (string text in AfterReceiptText)
            {
                if (text.Length == 3 && text.ToUpper() == "CUT") //cut
                {
                    if (!lastLineWasBarcode)
                    {
                        m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
                        m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
                    }

                    m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
                    m_printer.Print();
                    m_printer.ClearLines();
                    lastLineWasBarcode = false;
                }
                else if (text.Length > 8 && text.ToUpper().Substring(0, 8) == "CODE128=") //print a barcode
                {
                    PrintBarcode(text.Substring(8));
                    lastLineWasBarcode = true;
                }
                else
                {
                    m_printer.AddLine(text, StringAlignment.Center, ourFont);
                    lastLineWasBarcode = false;
                }
            }

            if (!lastLineWasBarcode)
            {
                m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
            }

            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
            m_printer.Print();
        }

        /// <summary>
        /// Prints the operators footer.
        /// </summary>
        protected void PrintOperatorsFooter()
        {// Operator's footer and system date.
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(m_operatorFooterLine1))
                m_printer.AddLine(m_operatorFooterLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine2))
                m_printer.AddLine(m_operatorFooterLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine3))
                m_printer.AddLine(m_operatorFooterLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1) ||
               !string.IsNullOrEmpty(m_operatorFooterLine2) ||
               !string.IsNullOrEmpty(m_operatorFooterLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            m_printer.AddLine(Resources.ReceiptMalfunction1, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(Resources.ReceiptMalfunction2, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // PDTS 584
            Font disclaimerFont = null;

            if (!m_useSmallSizes)
                disclaimerFont = m_fontSmall;
            else
                disclaimerFont = m_fontTiny;

            if (!string.IsNullOrEmpty(m_disclaimer1))
                m_printer.AddLine(m_disclaimer1, StringAlignment.Center, disclaimerFont);

            if (!string.IsNullOrEmpty(m_disclaimer2))
                m_printer.AddLine(m_disclaimer2, StringAlignment.Center, disclaimerFont);

            if (!string.IsNullOrEmpty(m_disclaimer3))
                m_printer.AddLine(m_disclaimer3, StringAlignment.Center, disclaimerFont);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_disclaimer1) || !string.IsNullOrEmpty(m_disclaimer2) ||
               !string.IsNullOrEmpty(m_disclaimer3) || IsPaperProductsValidated || IsElectronicProductsValidated)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

        }

        /// <summary>
        /// Prints if cards are validated.
        /// </summary>
        protected void PrintIsValidated()
        {
            if (IsPaperProductsValidated)
            {
                m_printer.AddLine(Resources.ReceiptPaperValidated, StringAlignment.Center, m_fontHuge);
            }

            if (IsElectronicProductsValidated)
            {
                m_printer.AddLine(Resources.ReceiptElectronicValidated, StringAlignment.Center, m_fontHuge);
            }

            // Add some spaces.
            if (IsPaperProductsValidated || IsElectronicProductsValidated)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

        }
        
        /// <summary>
        /// Prints the barcode.
        /// </summary>
        protected void PrintBarcode()
        {
            FontEncoder fontEncoder = new FontEncoder();
            var encodedText = fontEncoder.Code128("F"+m_number.ToString("D9")+"TRN");
            m_printer.AddLine(encodedText, StringAlignment.Center, new Font(BarcodeHelper.XSmallFontName, m_useSmallSizes? 12 : 18, FontStyle.Regular));

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            if (!SaleSuccess)
            {
                m_printer.AddLine(IncompleteTransactionLine1, StringAlignment.Center, m_fontExtraHuge);
                m_printer.AddLine(IncompleteTransactionLine2, StringAlignment.Center, m_fontExtraHuge);
            }
        }

        protected void PrintBarcode(string toPrint)
        {
            FontEncoder fontEncoder = new FontEncoder();
            var encodedText = fontEncoder.Code128(toPrint);
            float size = 24f;
            Font ourFont = new Font(BarcodeHelper.XSmallFontName, size, FontStyle.Regular);

            while(m_printer.MeasureString(toPrint, ourFont, StringAlignment.Center).Width * 1.10f > m_printer.PrintableArea.Width && size > 3)
            {
                size--;
                ourFont = new Font(BarcodeHelper.XSmallFontName, size, FontStyle.Regular);
            }

            if (m_printer.MeasureString(toPrint, ourFont, StringAlignment.Center).Width * 1.10f <= m_printer.PrintableArea.Width)
            {
                m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(encodedText, StringAlignment.Center, ourFont);
                m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(" ", StringAlignment.Center, m_fontMedium);
            }
        }
        
        // US2139
        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="printAccountNumber">Whether to print the player identity field as the account number.</param>
        /// <param name="printPointInfo">Tuple with whether to print player point 
        /// information, player tracking interface ID, if external systen does the rating, and if we do the rating.</param>
        /// <param name="printSignatureLine">Whether to print a signature 
        /// line.</param>
        /// <param name="printPtsRedeemed">Whether to print the Points Redeemed
        /// header.</param>
        /// <param name="printCardNumberMode">How to print card/start numbers
        /// on the receipt.</param>
        /// <param name="printCardFaces">Whether to print the bingo cards 
        /// on the receipt.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void PrintPreview(Printer printer, bool printAccountNumber, bool printPlayerID, Tuple<bool, int, bool, bool> printPointInfo, bool printSignatureLine, bool printPtsRedeemed, PrintCardNumberMode printCardNumberMode, bool printCardFaces, bool merchantCopy = false)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printPlayerIdentityAsAccountNumber = printAccountNumber;
            m_printPlayerID = printPlayerID;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            // Print unit information.
            PrintUnitInfo();

            // Print player information.
            PrintPlayerInfo(printPointInfo, printPtsRedeemed);

            // Print drawing entry information
            PrintDrawingEntryInfo();

            // Print sales items.
            PrintSaleInfo(printPointInfo.Item1, printSignatureLine, printPtsRedeemed, merchantCopy);

            if (SaleSuccess)
            {
                // Print barcoded paper pack Serial Numbers and Audit Numbers
                PrintPaperPackInfo();

                // Print the games' card totals & numbers.
                PrintBingoGameInfo(printCardNumberMode);

                // Print the card faces.
                if (printCardFaces)
                    PrintBingoGameCards();
            }

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="printPointInfo">Tuple with whether to print player point 
        /// information, player tracking interface ID, if external system does the rating,
        /// and if we do the rating.</param>
        /// <param name="printSignatureLine">Whether to print a signature 
        /// line.</param>
        /// <param name="printPtsRedeemed">Whether to print the Points Redeemed
        /// header.</param>
        /// <param name="printCardNumberMode">How to print card/start numbers
        /// on the receipt.</param>
        /// <param name="printCardFaces">Whether to print the bingo cards 
        /// on the receipt.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, bool printAccount, bool printPlayerID, Tuple<bool, int, bool, bool> printPointInfo, bool printSignatureLine, bool printPtsRedeemed, PrintCardNumberMode printCardNumberMode, bool printCardFaces, short copies, bool merchantCopy = false)
        {
            PrintPreview(printer, printAccount, printPlayerID, printPointInfo, printSignatureLine, printPtsRedeemed, printCardNumberMode, printCardFaces, merchantCopy);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }

            PrintAfterReceiptText();
        }
        // END: US2139
        #endregion

        #region Member Properties

        //US4804
        /// <summary>
        /// Gets or sets an array of tenders.
        /// </summary>
        public SaleTender[] SaleTenders
        {
            get
            {
                return m_saleTenders;
            }

            set
            {
                //lump all cash in our currency into one tender
                List<SaleTender> tenders = value.ToList();
                SaleTender cashTender = tenders.Find(t => t.TenderTypeID == TenderType.Cash && t.Amount == t.DefaultAmount && t.DefaultAmount > 0);

                if (cashTender != null)
                {
                    decimal cash = tenders.FindAll(t => t.TenderTypeID == TenderType.Cash && t.Amount == t.DefaultAmount && t.DefaultAmount > 0).Sum(t => t.DefaultAmount);

                    if (cash != cashTender.DefaultAmount) //multiples to merge
                    {
                        tenders.RemoveAll(t => t.TenderTypeID == TenderType.Cash && t.Amount == t.DefaultAmount && t.DefaultAmount > 0);
                        cashTender.DefaultAmount = cash;
                        cashTender.Amount = cash;
                        tenders.Add(cashTender);
                    }
                }

                m_saleTenders = tenders.ToArray();

                //figure out what currencies were used
                string tmp = string.Empty;
                TransactionCurrencyInfo[] distinctTenders = new TransactionCurrencyInfo[m_saleTenders.Length];

                foreach (SaleTender st in m_saleTenders)
                {
                    string tmpIso = st.IsoCode + "^";

                    if (!tmp.Contains(tmpIso))
                    {
                        tmp = tmp + tmpIso;
                        distinctTenders[(tmp.Length / 4) - 1] = new TransactionCurrencyInfo(st.IsoCode, st.ExchangeRate);
                    }
                }

                int count = tmp.Length/4;

                m_currenciesTendered = new TransactionCurrencyInfo[count];

                for (int i = 0; i < count; i++)
                    m_currenciesTendered[i] = distinctTenders[i];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use linear display numbers].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use linear display numbers]; otherwise, <c>false</c>.
        /// </value>
        public bool UseLinearDisplayNumbers { get; set; }

        /// <summary>
        /// Gets or sets whether this receipt is a reprint.
        /// </summary>
        public bool IsReprint
        {
            get
            {
                return m_isReprint;
            }
            set
            {
                m_isReprint = value;
            }
        }

        // Rally DE1863
        /// <summary>
        /// Gets or sets whether this receipt is a return.
        /// </summary>
        public bool IsReturn
        {
            get
            {
                return m_isReturn;
            }
            set
            {
                m_isReturn = value;
            }
        }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1
        {
            get
            {
                return m_operatorHeaderLine1;
            }
            set
            {
                m_operatorHeaderLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2
        {
            get
            {
                return m_operatorHeaderLine2;
            }
            set
            {
                m_operatorHeaderLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3
        {
            get
            {
                return m_operatorHeaderLine3;
            }
            set
            {
                m_operatorHeaderLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's first receipt footer line.
        /// </summary>
        public string OperatorFooterLine1
        {
            get
            {
                return m_operatorFooterLine1;
            }
            set
            {
                m_operatorFooterLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt footer line.
        /// </summary>
        public string OperatorFooterLine2
        {
            get
            {
                return m_operatorFooterLine2;
            }
            set
            {
                m_operatorFooterLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt footer line.
        /// </summary>
        public string OperatorFooterLine3
        {
            get
            {
                return m_operatorFooterLine3;
            }
            set
            {
                m_operatorFooterLine3 = value;
            }
        }

        public string IncompleteTransactionLine1
        {
            get
            {
                return m_incompleteTransactionReceiptText1;
            }

            set
            {
                m_incompleteTransactionReceiptText1 = value;
            }
        }

        public string IncompleteTransactionLine2
        {
            get
            {
                return m_incompleteTransactionReceiptText2;
            }

            set
            {
                m_incompleteTransactionReceiptText2 = value;
            }
        }

        // Rally US505
        /// <summary>
        /// Gets or sets the id of the sale this receipt is associated with.
        /// </summary>
        public int RegisterReceiptId
        {
            get
            {
                return m_registerReceiptId;
            }
            set
            {
                m_registerReceiptId = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt transaction number.
        /// </summary>
        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
            set
            {
                m_gamingDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipts transaction date.
        /// </summary>
        public DateTime TransactionDate
        {
            get
            {
                return m_transactionDate;
            }
            set
            {
                m_transactionDate = value;
            }
        }
        /// <summary>
        /// Gets or sets the staff who made the sale.
        /// </summary>
        public string Cashier
        {
            get
            {
                return m_cashier;
            }
            set
            {
                m_cashier = value;
            }
        }

        /// <summary>
        /// Gets or sets the pack number for the sale (if applicable).
        /// </summary>
        public int PackNumber
        {
            get
            {
                return m_packNumber;
            }
            set
            {
                m_packNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device sold to (if applicable).
        /// </summary>
        public string DeviceName
        {
            get
            {
                return m_deviceName;
            }
            set
            {
                m_deviceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the unit number of the device sold to (if applicable).
        /// </summary>
        public short UnitNumber
        {
            get
            {
                return m_unitNumber;
            }
            set
            {
                m_unitNumber = value;
            }
        }

        // Rally US1638
        /// <summary>
        /// Gets or sets the id of the machine this sale is tied to (if
        /// applicable).
        /// </summary>
        public int MachineId
        {
            get
            {
                return m_machineId;
            }
            set
            {
                m_machineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the client id of machine this sale is tied to (if
        /// applicable).
        /// </summary>
        public string ClientId
        {
            get
            {
                return m_clientId;
            }
            set
            {
                m_clientId = value;
            }
        }
        // END: US1638

        /// <summary>
        /// Gets or sets the serial number of the device sold to
        /// (if applicable).
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return m_serialNumber;
            }
            set
            {
                m_serialNumber = value;
            }
        }

        // TTP 50114
        /// <summary>
        /// Gets or sets if the player sold to was a machine (if applicable).
        /// </summary>
        public bool MachineAccount
        {
            get
            {
                return m_machineAccount;
            }
            set
            {
                m_machineAccount = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the player sold to.
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

        public string PlayerIdentity
        {
            get
            {
                return m_playerIdentity;
            }

            set
            {
                m_playerIdentity = value;
            }
        }

        /// <summary>
        /// Gets or sets the first name of the player sold to.
        /// </summary>
        public string PlayerName
        {
            get
            {
                return m_playerName;
            }
            set
            {
                m_playerName = value;
            }
        }

        /// Gets or sets the number of points a player had before the sale.
        /// </summary>
        public decimal PlayerPoints
        {
            get
            {
                return m_playerPts;
            }
            set
            {
                m_playerPts = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of points the player earned for the sale.
        /// </summary>
        public decimal PlayerPointsEarned
        {
            get
            {
                return m_playerPtsEarned;
            }
            set
            {
                m_playerPtsEarned = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of points the player used for the sale.
        /// </summary>
        public decimal PlayerPointsRedeemed
        {
            get
            {
                return m_playerPtsRedeem;
            }
            set
            {
                m_playerPtsRedeem = value;
            }
        }

        public decimal QualifyingAmountForPoints
        {
            get
            {
                return m_qualifyingAmountForPoints;
            }
            set
            {
                m_qualifyingAmountForPoints = value;
            }
        }

        public decimal PointsForQualifyingAmount
        {
            get
            {
                return m_pointsForQualifyingAmount;
            }
            set
            {
                m_pointsForQualifyingAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the machine that made the sale.
        /// </summary>
        public int SoldFromMachineId
        {
            get
            {
                return m_soldFromMachineId;
            }
            set
            {
                m_soldFromMachineId = value;
            }
        }

        // Rally TA7464
        /// <summary>
        /// Gets or sets the ISO code of the default system currency.
        /// </summary>
        public string DefaultCurrency
        {
            get
            {
                return m_defaultCurrency;
            }
            set
            {
                m_defaultCurrency = value;
            }
        }

        /// <summary>
        /// Gets or sets the ISO code of the sale's currency.
        /// If more than one currency used to tender, the returned
        /// value is an empty string.
        /// </summary>
        public string SaleCurrency
        {
            get
            {
                return (m_currenciesTendered != null && m_currenciesTendered.Length != 0 ? (m_currenciesTendered.Length > 1 ? string.Empty : m_currenciesTendered[0].isoCode) : m_saleCurrency);
            }
            set
            {
                m_saleCurrency = value;
            }
        }

        /// <summary>
        /// Gets or sets the exchange rate of the sale.
        /// </summary>
        public decimal ExchangeRate
        {
            get
            {
                return m_exchangeRate;
            }
            set
            {
                m_exchangeRate = value;
            }
        }

        /// <summary>
        /// Gets or sets the grand total of the sale.
        /// </summary>
        public decimal GrandTotal
        {
            get
            {
                return m_grandTotal;
            }
            set
            {
                m_grandTotal = value;
            }
        }

        /// <summary>
        /// Gets or sets the non-taxable coupon total (a negative number).
        /// </summary>
        public decimal NonTaxedCouponTotal
        {
            get
            {
                return m_nonTaxedCouponTotal;
            }

            set
            {
                m_nonTaxedCouponTotal = value;
            }
        }

        /// <summary>
        /// Gets or sets the change/amount due for the sale.
        /// </summary>
        public decimal ChangeDue
        {
            get
            {
                return m_changeDue;
            }
            set
            {
                m_changeDue = value;
            }
        }
        // END: TA7464

        /// <summary>
        /// Gets or sets the amount charged for using a device (if applicable).
        /// </summary>
        public decimal DeviceFee
        {
            get
            {
                return m_deviceFee;
            }
            set
            {
                m_deviceFee = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount charged for taxes.
        /// </summary>
        public decimal Taxes
        {
            get
            {
                return m_taxes;
            }
            set
            {
                m_taxes = value;
            }
        }

        public decimal PrepaidAmount
        {
            get
            {
                return m_prepaidAmount;
            }

            set
            {
                m_prepaidAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets the total amount of the sale.
        /// </summary>
        public decimal Total
        {
            get
            {
                return m_total;
            }
            set
            {
                m_total = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount tendered for the sale.
        /// </summary>
        public decimal AmountTendered
        {
            get
            {
                return m_amountTendered;
            }
            set
            {
                m_amountTendered = value;
            }
        }

        /// <summary>
        /// Gets or sets the first disclaimer line.
        /// </summary>
        public string DisclaimerLine1
        {
            get
            {
                return m_disclaimer1;
            }
            set
            {
                m_disclaimer1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the second disclaimer line.
        /// </summary>
        public string DisclaimerLine2
        {
            get
            {
                return m_disclaimer2;
            }
            set
            {
                m_disclaimer2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the third disclaimer line.
        /// </summary>
        public string DisclaimerLine3
        {
            get
            {
                return m_disclaimer3;
            }
            set
            {
                m_disclaimer3 = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to print LOTTO instead of BINGO on cards.
        /// </summary>
        public bool PrintLotto
        {
            get
            {
                return m_printLotto;
            }
            set
            {
                m_printLotto = value;
            }
        }

        /// <summary>
        /// Gets or sets the array of bingo sessions to print on the receipt.
        /// </summary>
        public BingoSession[] BingoSessions
        {
            get
            {
                return m_bingoSessions;
            }
            set
            {
                m_bingoSessions = value;

                m_sameCards.Clear();

                // Pre-parse all the cards in the sessions so we can print them 
                // out fast.
                if (m_bingoSessions != null)
                {
                    foreach (BingoSession session in m_bingoSessions)
                    {
                        // Add the cards to the same cards list if applicable.
                        if (session.SameCards)
                        {
                            foreach (BingoGame games in session.GetGames())
                            {
                                foreach (BingoCard card in games.GetCards())
                                {
                                    if (!m_sameCards.Contains(card))
                                        m_sameCards.Add(card);
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<SessionCharity> SessionCharities
        {
            get
            {
                return m_charities;
            }

            set
            {
                m_charities = value;
            }
        }

        public List<string> AfterReceiptText
        {
            get
            {
                return m_afterReceiptText;
            }

            set
            {
                m_afterReceiptText = value;
            }
        }

        public List<Tuple<string, int, DateTime>> DrawingEntries
        {
            get
            {
                return m_drawingEntries;
            }

            set
            {
                m_drawingEntries = value;
            }
        }

        //US3509
        public bool IsElectronicProductsValidated;

        public bool IsPaperProductsValidated;

        public bool IsPreSale;

        public DateTime PresalesGamingDate;

        public string PresalesProgramName;

        #endregion
    }

    /// <summary>
    /// Represents a printed receipt that only contains bingo card faces.
    /// </summary>
    public class BingoCardReceipt : Receipt, IEquatable<BingoCardReceipt>
    {
        #region Constants and Data Types
        protected const string CardFontFace = "Lucida Console";
        protected const int SpacesBetweenCards = 2;

        /// <summary>
        /// The types of lines on this receipt.
        /// </summary>
        protected enum BingoCardReceiptLineType
        {
            Header,
            PageBreak,
            Card
        }

        /// <summary>
        /// Represents one line on the bingo card reciept.
        /// </summary>
        protected struct BingoCardReceiptLine
        {
            public BingoCardReceiptLineType Type;
            public string Line;

            /// <summary>
            /// Initializes the BingoCardReceiptLine structure.
            /// </summary>
            /// <param name="type">The type of line.</param>
            /// <param name="line">The string that is to be printed on this 
            /// line.</param>
            public BingoCardReceiptLine(BingoCardReceiptLineType type, string line)
            {
                Type = type;
                Line = line;
            }
        }
        #endregion

        #region Member Variables
        protected bool m_useDisputeHeader;
        protected bool m_suppressSessionLine;
        protected float m_pointSize = 5F;
        protected Font m_cardFont;
        protected List<BingoCardReceiptLine> m_lines = new List<BingoCardReceiptLine>();
        protected int m_number;
        protected DateTime m_gamingDate;
        protected string m_playerMagCard;
        protected bool m_printLotto;
        protected BingoSession[] m_bingoSessions;
        protected List<BingoCard> m_sameCards = new List<BingoCard>();
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two BingoCardReceipt instances are equal. 
        /// </summary>
        /// <param name="obj">The BingoCardReceipt to compare with the 
        /// current BingoCardReceipt.</param>
        /// <returns>true if the specified BingoCardReceipt is equal to the 
        /// current BingoCardReceipt; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            BingoCardReceipt receipt = obj as BingoCardReceipt;

            if (receipt == null)
                return false;
            else
                return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a BingoCardReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current BingoCardReceipt.</returns>
        public override int GetHashCode()
        {
            return m_number.GetHashCode();
        }

        /// <summary>
        /// Determines whether two BingoCardReceipt instances are equal. 
        /// </summary>
        /// <param name="other">The BingoCardReceipt to compare with the 
        /// current BingoCardReceipt.</param>
        /// <returns>true if the specified BingoCardReceipt is equal to the 
        /// current BingoCardReceipt; otherwise, false. </returns>
        public bool Equals(BingoCardReceipt other)
        {
            return (other != null && m_number == other.m_number);
        }

        /// <summary>
        /// Formats all the BingoCards in on this receipt to be printed out.
        /// </summary>
        protected void FormatCards()
        {
            if (m_bingoSessions != null)
            {
                string[] cards = null;

                // First, print all the same cards.
                if (m_sameCards.Count > 0)
                {
                    cards = BingoCardPrinterHelper.FormatTextBingoCards(m_printer, m_cardFont, StringAlignment.Near, m_sameCards.ToArray(), SpacesBetweenCards, m_printLotto);

                    foreach (string line in cards)
                    {
                        m_lines.Add(new BingoCardReceiptLine(BingoCardReceiptLineType.Card, line));
                    }
                }

                // Now print the cards from the sessions.
                foreach (BingoSession session in m_bingoSessions)
                {
                    // Only print the cards out if they aren't in the same 
                    // cards list.
                    if (!session.SameCards)
                    {
                        if (!m_suppressSessionLine)
                            m_lines.Add(new BingoCardReceiptLine(BingoCardReceiptLineType.Card, string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSession, session.Number)));

                        foreach (BingoGame game in session.GetGames())
                        {
                            //US4852
                            m_lines.Add(new BingoCardReceiptLine(BingoCardReceiptLineType.Card, string.Format(Resources.ReceiptGame, game.GetDisplayNumberToString)));

                            cards = BingoCardPrinterHelper.FormatTextBingoCards(m_printer, m_cardFont, StringAlignment.Near, game.GetCards(), SpacesBetweenCards, m_printLotto);

                            foreach (string line in cards)
                            {
                                m_lines.Add(new BingoCardReceiptLine(BingoCardReceiptLineType.Card, line));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="copies">The number of copies to print out.</param>
        public void Print(Printer printer, short copies)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Create the font we will be using for printing cards.
            m_cardFont = new Font(CardFontFace, m_pointSize);

            if (m_bingoSessions != null)
            {
                m_lines.Clear();

                // Print the card faces to the temporary List.
                FormatCards();

                if (m_lines.Count > 0)
                {
                    // Measure how many lines we can fit on a page, minus the header.
                    float headerHeight = printer.GetFontHeight(m_fontMedium);
                    float lineHeight = printer.GetFontHeight(m_cardFont);
                    int linesPerPage = (int)Math.Floor((printer.PrintableArea.Height - headerHeight) / lineHeight);

                    if (linesPerPage > 0)
                    {
                        // How many total pages are we going to print?
                        int totalPages = (int)Math.Ceiling(m_lines.Count / (decimal)linesPerPage);

                        if (totalPages <= 0)
                            totalPages = 1;

                        int currentPage = 1;
                        int currentLine = 0;

                        // Create the first page's header.
                        string header;

                        if (!m_useDisputeHeader)
                            header = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptBingoCardHeader, m_number, m_gamingDate.ToShortDateString(), currentPage, totalPages);
                        else
                            header = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptBingoCardDisputeHeader, m_playerMagCard, currentPage, totalPages);

                        m_lines.Insert(currentLine, new BingoCardReceiptLine(BingoCardReceiptLineType.Header, header));

                        currentPage++;
                        currentLine += (linesPerPage + 1);

                        // Loop through all the lines and insert headers and 
                        // page breaks.
                        while (currentPage <= totalPages)
                        {
                            if (string.IsNullOrEmpty(m_lines[currentLine].Line))
                            {
                                // Create a page break, then the header.
                                m_lines.Insert(currentLine, new BingoCardReceiptLine(BingoCardReceiptLineType.PageBreak, null));
                                currentLine++;

                                if (!m_useDisputeHeader)
                                    header = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptBingoCardHeader, m_number, m_gamingDate.ToShortDateString(), currentPage, totalPages);
                                else
                                    header = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptBingoCardDisputeHeader, m_playerMagCard, currentPage, totalPages);

                                m_lines.Insert(currentLine, new BingoCardReceiptLine(BingoCardReceiptLineType.Header, header));
                            }
                            else
                            {
                                // The line we want to insert a header on is 
                                // already occupied, so back up until we find a
                                // blank line.
                                while (currentLine > 0 && !string.IsNullOrEmpty(m_lines[currentLine].Line))
                                    currentLine--;

                                if (currentLine <= 0)
                                    break; // We can't fit anything on a page.

                                // Create a page break, then the header.
                                m_lines.Insert(currentLine, new BingoCardReceiptLine(BingoCardReceiptLineType.PageBreak, null));
                                currentLine++;

                                if (!m_useDisputeHeader)
                                    header = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptBingoCardHeader, m_number, m_gamingDate.ToShortDateString(), currentPage, totalPages);
                                else
                                    header = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptBingoCardDisputeHeader, m_playerMagCard, currentPage, totalPages);

                                m_lines.Insert(currentLine, new BingoCardReceiptLine(BingoCardReceiptLineType.Header, header));
                            }

                            currentPage++;
                            currentLine += (linesPerPage + 1);
                        }

                        // Send the lines to the printer.
                        m_printer.ClearLines();

                        foreach (BingoCardReceiptLine line in m_lines)
                        {
                            switch (line.Type)
                            {
                                case BingoCardReceiptLineType.Header:
                                    m_printer.AddLine(line.Line, StringAlignment.Center, m_fontMedium);
                                    break;

                                case BingoCardReceiptLineType.PageBreak:
                                    m_printer.AddPageBreak();
                                    break;

                                default:
                                case BingoCardReceiptLineType.Card:
                                    m_printer.AddLine(line.Line, StringAlignment.Near, m_cardFont);
                                    break;
                            }
                        }

                        for (int x = 0; x < copies; x++)
                        {
                            m_printer.Print();
                        }
                    }
                }
            }
        }
        #endregion

        #region Member Properties
        // TTP 50114
        /// <summary>
        /// Gets or sets whether this is receipt should use the dispute 
        /// resolution header.
        /// </summary>
        public bool UseDisputeHeader
        {
            get
            {
                return m_useDisputeHeader;
            }
            set
            {
                m_useDisputeHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to print the session line numbers.
        /// </summary>
        public bool SuppressSessionLine
        {
            get
            {
                return m_suppressSessionLine;
            }
            set
            {
                m_suppressSessionLine = value;
            }
        }

        /// <summary>
        /// Gets or sets the point size used to print the receipt.
        /// </summary>
        public float PointSize
        {
            get
            {
                return m_pointSize;
            }
            set
            {
                m_pointSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt transaction number.
        /// </summary>
        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
            set
            {
                m_gamingDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's mag. card number used with the dispute 
        /// resolution header.
        /// </summary>
        public string PlayerMagCardNumber
        {
            get
            {
                return m_playerMagCard;
            }
            set
            {
                m_playerMagCard = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to print LOTTO instead of BINGO on cards.
        /// </summary>
        public bool PrintLotto
        {
            get
            {
                return m_printLotto;
            }
            set
            {
                m_printLotto = value;
            }
        }

        /// <summary>
        /// Gets or sets the array of bingo sessions to print on the receipt.
        /// </summary>
        public BingoSession[] BingoSessions
        {
            get
            {
                return m_bingoSessions;
            }
            set
            {
                m_bingoSessions = value;

                m_sameCards.Clear();

                // Pre-parse all the cards in the sessions so we can print them 
                // out fast.
                if (m_bingoSessions != null)
                {
                    foreach (BingoSession session in m_bingoSessions)
                    {
                        // Add the cards to the same cards list if applicable.
                        if (session.SameCards)
                        {
                            foreach (BingoGame games in session.GetGames())
                            {
                                foreach (BingoCard card in games.GetCards())
                                {
                                    if (!m_sameCards.Contains(card))
                                        m_sameCards.Add(card);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a printed credit cash out receipt.
    /// </summary>
    public class CreditCashOutReceipt : Receipt, IEquatable<CreditCashOutReceipt>
    {
        #region Constants and Data Types
        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalBodyColumn1Length = 20; // Rally TA7465

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallBodyColumn1Length = 20; // Rally TA7465
        #endregion

        #region Member Variables
        protected int m_headerColumn1Length;
        protected int m_headerColumn2Length;
        protected int m_headerColumn3Length;
        protected int m_bodyColumn1Length;
        protected int m_headerMachineDescLength = 17;
        protected bool m_isReprint;
        protected string m_operatorHeaderLine1;
        protected string m_operatorHeaderLine2;
        protected string m_operatorHeaderLine3;
        protected string m_operatorFooterLine1;
        protected string m_operatorFooterLine2;
        protected string m_operatorFooterLine3;
        protected int m_number;
        protected DateTime m_gamingDate;
        protected int m_machineId;
        protected string m_cashier;
        protected bool m_machineAccount;
        protected int m_playerId;
        protected string m_playerName;
        // Rally TA7464
        protected string m_defaultCurrency;
        protected string m_cashOutCurrency;
        protected decimal m_exchangeRate;
        protected decimal m_cashOutAmountConverted;
        // END: TA7464
        protected decimal m_originalBalance;
        protected decimal m_cashOutAmount;
        protected decimal m_newBalance;
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two CreditCashOutReceipt instances are equal. 
        /// </summary>
        /// <param name="obj">The CreditCashOutReceipt to compare with the 
        /// current CreditCashOutReceipt.</param>
        /// <returns>true if the specified CreditCashOutReceipt is equal to the 
        /// current CreditCashOutReceipt; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            CreditCashOutReceipt receipt = obj as CreditCashOutReceipt;

            if (receipt == null)
                return false;
            else
                return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a CreditCashOutReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current CreditCashOutReceipt.</returns>
        public override int GetHashCode()
        {
            return m_number.GetHashCode();
        }

        /// <summary>
        /// Determines whether two CreditCashOutReceipt instances are equal. 
        /// </summary>
        /// <param name="other">The CreditCashOutReceipt to compare with the 
        /// current CreditCashOutReceipt.</param>
        /// <returns>true if the specified CreditCashOutReceipt is equal to the 
        /// current CreditCashOutReceipt; otherwise, false. </returns>
        public bool Equals(CreditCashOutReceipt other)
        {
            return (other != null && m_number == other.m_number);
        }

        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
                m_headerColumn3Length = SmallHeaderColumn3Length;
                m_bodyColumn1Length = SmallBodyColumn1Length;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
                m_headerColumn3Length = NormalHeaderColumn3Length;
                m_bodyColumn1Length = NormalBodyColumn1Length;
            }
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1))
                m_printer.AddLine(m_operatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine2))
                m_printer.AddLine(m_operatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine3))
                m_printer.AddLine(m_operatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine2) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected void PrintHeader()
        {
            // PDTS 584
            m_printer.AddLine(Resources.CreditCashOutReceiptTitle, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            string temp;

            // Receipt Number & Machine Id
            temp = Resources.ReceiptNumber.PadRight(m_headerColumn1Length) + m_number.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_machineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_machineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Gaming Date
            temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString();

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Cashier (Staff)
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + m_cashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the body of the receipt.
        /// </summary>
        protected void PrintBody()
        {
            // Player Id
            // TTP 50114
            if (!m_machineAccount && m_printPlayerID && m_playerId != 0)
                m_printer.AddLine(Resources.ReceiptPlayerNumber.PadRight(m_bodyColumn1Length) + m_playerId.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontMedium);

            // Player Name
            if (m_machineAccount)
                m_printer.AddLine(Resources.ReceiptMachineName.PadRight(m_bodyColumn1Length) + m_playerName, StringAlignment.Near, m_fontMedium);
            else
                m_printer.AddLine(Resources.ReceiptPlayerName.PadRight(m_bodyColumn1Length) + m_playerName, StringAlignment.Near, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Rally TA7465
            // Balances
            // What is the longest value?
            string org = Currency.FormatCurrencyString(m_defaultCurrency, m_originalBalance);
            string balance = Currency.FormatCurrencyString(m_defaultCurrency, m_newBalance);
            string cashOutOrg = Currency.FormatCurrencyString(m_defaultCurrency, m_cashOutAmount);
            string rate = m_exchangeRate.ToString("0.0000", CultureInfo.CurrentCulture);
            string cashOutConverted = Currency.FormatCurrencyString(m_cashOutCurrency, m_cashOutAmountConverted);

            int maxLenth = Math.Max(org.Length, balance.Length);
            maxLenth = Math.Max(maxLenth, cashOutOrg.Length);

            if (m_exchangeRate != 0M)//RALLY DE 7069 print out the exchange rate even if it is 1.000
                maxLenth = Math.Max(maxLenth, rate.Length);

            if (m_defaultCurrency != m_cashOutCurrency)
                maxLenth = Math.Max(maxLenth, cashOutConverted.Length);

            // Create the right-justified values.
            org = org.PadLeft(maxLenth);
            balance = balance.PadLeft(maxLenth);
            cashOutOrg = cashOutOrg.PadLeft(maxLenth);
            rate = rate.PadLeft(maxLenth);
            cashOutConverted = cashOutConverted.PadLeft(maxLenth);

            m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptCashOutOriginal, m_defaultCurrency).PadRight(m_bodyColumn1Length) + org, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptCashOutBalance, m_defaultCurrency).PadRight(m_bodyColumn1Length) + balance, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptCashOutCashedOut, m_defaultCurrency).PadRight(m_bodyColumn1Length) + cashOutOrg, StringAlignment.Near, m_fontMedium);

            if (m_exchangeRate != 0M && m_exchangeRate != 1M)
                m_printer.AddLine(Resources.ReceiptCashOutExchangeRate.PadRight(m_bodyColumn1Length) + rate, StringAlignment.Near, m_fontMedium);

            if (m_defaultCurrency != m_cashOutCurrency)
                m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.ReceiptCashOutCashedOut, m_cashOutCurrency).PadRight(m_bodyColumn1Length) + cashOutConverted, StringAlignment.Near, m_fontMedium);
            // END: TA7465
        }

        /// <summary>
        /// Prints the receipt's footer lines.
        /// </summary>
        protected void PrintFooter()
        {
            // Operator's footer and system date.
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(m_operatorFooterLine1))
                m_printer.AddLine(m_operatorFooterLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine2))
                m_printer.AddLine(m_operatorFooterLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine3))
                m_printer.AddLine(m_operatorFooterLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1) ||
               !string.IsNullOrEmpty(m_operatorFooterLine2) ||
               !string.IsNullOrEmpty(m_operatorFooterLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void PrintPreview(Printer printer)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            // Print then body.
            PrintBody();

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, short copies)
        {
            PrintPreview(printer);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets whether this receipt is a reprint.
        /// </summary>
        public bool IsReprint
        {
            get
            {
                return m_isReprint;
            }
            set
            {
                m_isReprint = value;
            }
        }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1
        {
            get
            {
                return m_operatorHeaderLine1;
            }
            set
            {
                m_operatorHeaderLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2
        {
            get
            {
                return m_operatorHeaderLine2;
            }
            set
            {
                m_operatorHeaderLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3
        {
            get
            {
                return m_operatorHeaderLine3;
            }
            set
            {
                m_operatorHeaderLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's first receipt footer line.
        /// </summary>
        public string OperatorFooterLine1
        {
            get
            {
                return m_operatorFooterLine1;
            }
            set
            {
                m_operatorFooterLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt footer line.
        /// </summary>
        public string OperatorFooterLine2
        {
            get
            {
                return m_operatorFooterLine2;
            }
            set
            {
                m_operatorFooterLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt footer line.
        /// </summary>
        public string OperatorFooterLine3
        {
            get
            {
                return m_operatorFooterLine3;
            }
            set
            {
                m_operatorFooterLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt transaction number.
        /// </summary>
        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the machine that cashed out the credit.
        /// </summary>
        public int MachineId
        {
            get
            {
                return m_machineId;
            }
            set
            {
                m_machineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
            set
            {
                m_gamingDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff who performed this cash out.
        /// </summary>
        public string Cashier
        {
            get
            {
                return m_cashier;
            }
            set
            {
                m_cashier = value;
            }
        }

        // TTP 50114
        /// <summary>
        /// Gets or sets if the player who cashed out was a machine.
        /// </summary>
        public bool MachineAccount
        {
            get
            {
                return m_machineAccount;
            }
            set
            {
                m_machineAccount = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the player cashed out.
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
        /// Gets or sets the name of the player cashed out.
        /// </summary>
        public string PlayerName
        {
            get
            {
                return m_playerName;
            }
            set
            {
                m_playerName = value;
            }
        }

        // Rally TA7464
        /// <summary>
        /// Gets or sets the ISO code of the default system currency.
        /// </summary>
        public string DefaultCurrency
        {
            get
            {
                return m_defaultCurrency;
            }
            set
            {
                m_defaultCurrency = value;
            }
        }

        /// <summary>
        /// Gets or sets the ISO code of the cash out currency.
        /// </summary>
        public string CashOutCurrency
        {
            get
            {
                return m_cashOutCurrency;
            }
            set
            {
                m_cashOutCurrency = value;
            }
        }

        /// <summary>
        /// Gets or sets the exchange rate of the cash out.
        /// </summary>
        public decimal ExchangeRate
        {
            get
            {
                return m_exchangeRate;
            }
            set
            {
                m_exchangeRate = value;
            }
        }

        /// <summary>
        /// Gets or sets the cash out amount converted to the cash out currency
        /// (if applicable).
        /// </summary>
        public decimal CashOutAmountConverted
        {
            get
            {
                return m_cashOutAmountConverted;
            }
            set
            {
                m_cashOutAmountConverted = value;
            }
        }
        // END: TA7465

        /// <summary>
        /// Gets or sets the player's original credit balance.
        /// </summary>
        public decimal OriginalBalance
        {
            get
            {
                return m_originalBalance;
            }
            set
            {
                m_originalBalance = value;
            }
        }

        /// <summary>
        /// Gets or sets the cash out amount.
        /// </summary>
        public decimal CashOutAmount
        {
            get
            {
                return m_cashOutAmount;
            }
            set
            {
                m_cashOutAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's new credit balance.
        /// </summary>
        public decimal NewBalance
        {
            get
            {
                return m_newBalance;
            }
            set
            {
                m_newBalance = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// This class prepares bingo cards to be printed to a printer.
    /// </summary>
    internal static class BingoCardPrinterHelper
    {
        #region Static Methods
        /// <summary>
        /// This method takes an array of bingo cards and formats them to be 
        /// printed on a text based printer.
        /// </summary>
        /// <param name="printer">The printer object that the cards are going 
        /// to be printed to.</param>
        /// <param name="font">The font that is going to be used to print out 
        /// the cards.</param>
        /// <param name="alignment">The StringAlignment that is going to be 
        /// used to print out the cards.</param>
        /// <param name="cards">The cards to be printed.</param>
        /// <param name="numOfSpaces">The number of spaces between the bingo 
        /// cards.</param>
        /// <param name="printLotto">Whether to print LOTTO instead of 
        /// BINGO.</param>
        /// <returns>An array of strings that are to be sent to a text based 
        /// printer.</returns>
        public static string[] FormatTextBingoCards(Printer printer, Font font, StringAlignment alignment, BingoCard[] cards, int numOfSpaces, bool printLotto)
        {
            List<string> printerLines = new List<string>();
            List<string> cardLines = new List<string>();
            Type lastCardType = null;
            int numColumns = 1;
            int currentColumn = 1;

            foreach (BingoCard card in cards)
            {
                // Have we changed card types?
                if (card.GetType() != lastCardType)
                {
                    // We need to start a new printer line.
                    if (cardLines.Count > 0 && !string.IsNullOrEmpty(cardLines[0]))
                    {
                        // Add the existing line to the printerLines.
                        foreach (string line in cardLines)
                        {
                            printerLines.Add(line);
                        }

                        printerLines.Add(string.Empty);
                    }

                    // Measure the card to see how many we can fit per line.
                    string testString = string.Empty.PadRight(card.FaceSize.Width + numOfSpaces, '0');
                    SizeF cardSize = printer.MeasureString(testString, font, alignment);

                    numColumns = (int)Math.Floor(printer.PrintableArea.Width / cardSize.Width);

                    cardLines.Clear();

                    for (int x = 0; x < card.FaceSize.Height; x++)
                    {
                        cardLines.Add(string.Empty);
                    }

                    lastCardType = card.GetType();
                    currentColumn = 1;
                }

                string[] face = card.ToString(printLotto);

                if (face.Length > 0)
                {
                    for (int currentLine = 0; currentLine < cardLines.Count; currentLine++)
                    {
                        cardLines[currentLine] += face[currentLine] + string.Empty.PadRight(numOfSpaces);
                    }

                    if (++currentColumn > numColumns)
                    {
                        for (int currentLine = 0; currentLine < cardLines.Count; currentLine++)
                        {
                            printerLines.Add(cardLines[currentLine]);
                            cardLines[currentLine] = string.Empty;
                        }

                        printerLines.Add(string.Empty);
                        currentColumn = 1;
                    }
                }
            }

            // Add the final line, if needed.
            if (cardLines.Count > 0 && !string.IsNullOrEmpty(cardLines[0]))
            {
                // Add the existing line to the printerLines.
                foreach (string line in cardLines)
                {
                    printerLines.Add(line);
                }

                printerLines.Add(string.Empty);
            }

            return printerLines.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// Represents a printed void receipt.
    /// </summary>
    public class VoidReceipt : SalesReceipt, IEquatable<VoidReceipt>
    {
        #region Member Variables
        protected int m_voidNumber;
        protected string m_voidCashier;
        protected int m_voidMachineId;
        protected DateTime m_voidDateTime;
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two VoidReceipt instances are equal. 
        /// </summary>
        /// <param name="obj">The VoidReceipt to compare with the 
        /// current VoidReceipt.</param>
        /// <returns>true if the specified VoidReceipt is equal to the current 
        /// VoidReceipt; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            VoidReceipt receipt = obj as VoidReceipt;

            if (receipt == null)
                return false;
            else
                return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a VoidReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current VoidReceipt.</returns>
        public override int GetHashCode()
        {
            return m_voidNumber.GetHashCode();
        }

        /// <summary>
        /// Determines whether two VoidReceipt instances are equal. 
        /// </summary>
        /// <param name="other">The VoidReceipt to compare with the 
        /// current VoidReceipt.</param>
        /// <returns>true if the specified VoidReceipt is equal to the current 
        /// VoidReceipt; otherwise, false. </returns>
        public bool Equals(VoidReceipt other)
        {
            return (other != null && m_voidNumber == other.m_voidNumber);
        }

        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected override void PrintHeader()
        {
            if (!SaleSuccess)
            {
                if(!string.IsNullOrWhiteSpace(IncompleteTransactionLine1))
                    m_printer.AddLine(IncompleteTransactionLine1, StringAlignment.Center, m_fontExtraHuge);

                if (!string.IsNullOrWhiteSpace(IncompleteTransactionLine2))
                    m_printer.AddLine(IncompleteTransactionLine2, StringAlignment.Center, m_fontExtraHuge);
            }

            //US5711
            m_printer.AddLine(IsRefund ? Resources.RefundReceiptTitle : Resources.VoidReceiptTitle,
                StringAlignment.Center, m_fontBig);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            if (IsRefund)
            {
                PrintRefundInfo();
            }
            else
            {
                PrintVoidInfo();
            }

            PrintSaleHeader();
            PrintCharityHeader();
        }

        //US4848 
        /// <summary>
        /// Prints the receipt's footer lines.
        /// </summary>
        protected override void PrintFooter()
        {
            PrintOperatorsFooter();

            //US4848 print signature lines
            PrintSignatureLines();

            PrintIsValidated();
            PrintBarcode();

            if (!SaleSuccess)
            {
                if (!string.IsNullOrWhiteSpace(IncompleteTransactionLine1))
                    m_printer.AddLine(IncompleteTransactionLine1, StringAlignment.Center, m_fontExtraHuge);

                if (!string.IsNullOrWhiteSpace(IncompleteTransactionLine2))
                    m_printer.AddLine(IncompleteTransactionLine2, StringAlignment.Center, m_fontExtraHuge);
            }

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
        }

        //US4848 
        protected void PrintSignatureLines()
        {
            for(int i = 0; i < NumberOfSignatureLines; i++)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

                m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
                m_printer.AddLine("Signature " + (i+1), StringAlignment.Center, m_fontSmall);

            }

            //add space
            if (NumberOfSignatureLines > 0)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints information related to the void.
        /// </summary>
        protected void PrintVoidInfo()
        {
            string temp;

            // Void Receipt Number & Machine Id
            temp = Resources.VoidReceiptNumber.PadRight(m_headerColumn1Length) + m_voidNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_voidMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_voidMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            //Machine name
            temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            // Cashier (Staff)
            temp = Resources.VoidReceiptStaff.PadRight(m_headerColumn1Length) + m_voidCashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            // Voided On
            m_printer.AddLine(Resources.VoidReceiptDate.PadRight(m_headerColumn1Length) + m_voidDateTime.ToString(), StringAlignment.Near, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        //US5711
        protected void PrintRefundInfo()
        {
            // Void Receipt Number & Machine Id
            //temp = Resources.VoidReceiptNumber.PadRight(m_headerColumn1Length) + m_voidNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            var temp = Resources.RefundReceiptNumber.PadRight(m_headerColumn1Length) + m_voidNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_voidMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_voidMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            //Machine name
            temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            // Cashier (Staff)
            temp = Resources.RefundReceiptStaff.PadRight(m_headerColumn1Length) + m_voidCashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            // Refund On
            m_printer.AddLine(Resources.RefundReceiptDate.PadRight(m_headerColumn1Length) + m_voidDateTime.ToString(), StringAlignment.Near, m_fontSmall);
            
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the new void transaction number
        /// </summary>
        public int VoidNumber
        {
            get
            {
                return m_voidNumber;
            }
            set
            {
                m_voidNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the cashier performing the void.
        /// </summary>
        public string VoidCashier
        {
            get
            {
                return m_voidCashier;
            }
            set
            {
                m_voidCashier = value;
            }
        }

        /// <summary>
        /// Gets or sets the machine performing the void.
        /// </summary>
        public int VoidMachineId
        {
            get
            {
                return m_voidMachineId;
            }
            set
            {
                m_voidMachineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the date and time of the void.
        /// </summary>
        public DateTime VoidDateTime
        {
            get
            {
                return m_voidDateTime;
            }
            set
            {
                m_voidDateTime = value;
            }
        }

        //US4848 
        /// <summary>
        /// Gets or sets the number of signature lines.
        /// </summary>
        /// <value>
        /// The number of signature lines.
        /// </value>
        public int NumberOfSignatureLines { get; set; }

        public bool IsRefund { get; set; }
        #endregion
    }

    /// <summary>
    /// Represents a printed unit transfer receipt.
    /// </summary>
    public class UnitTransferReceipt : Receipt, IEquatable<UnitTransferReceipt>
    {
        #region Constants and Data Types
        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        #endregion

        #region Member Variables
        protected int m_headerColumn1Length;
        protected int m_headerColumn2Length;
        protected int m_headerColumn3Length;
        protected int m_bodyColumn1Length;
        protected int m_headerMachineDescLength = 17;

        protected bool m_isReprint;
        protected string m_operatorHeaderLine1;
        protected string m_operatorHeaderLine2;
        protected string m_operatorHeaderLine3;
        protected string m_operatorFooterLine1;
        protected string m_operatorFooterLine2;
        protected string m_operatorFooterLine3;
        protected int m_number;
        protected DateTime m_gamingDate;
        protected int m_machineId;
        protected string m_cashier;
        protected int m_originalNumber;
        protected string m_fromDeviceName;
        protected short m_fromUnitNumber;
        protected string m_fromSerialNumber;
        protected string m_toDeviceName;
        protected short m_toUnitNumber;
        protected string m_toSerialNumber;
        protected int m_playerId; //DE131:
        protected string m_playerName; //DE131:
        protected List<SessionCharity> m_charities; //US2720
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two UnitTransferReceipt instances are equal. 
        /// </summary>
        /// <param name="obj">The UnitTransferReceipt to compare with the 
        /// current UnitTransferReceipt.</param>
        /// <returns>true if the specified UnitTransferReceipt is equal to the 
        /// current UnitTransferReceipt; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            UnitTransferReceipt receipt = obj as UnitTransferReceipt;

            if (receipt == null)
                return false;
            else
                return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a UnitTransferReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current UnitTransferReceipt.</returns>
        public override int GetHashCode()
        {
            return m_number.GetHashCode();
        }

        /// <summary>
        /// Determines whether two UnitTransferReceipt instances are equal. 
        /// </summary>
        /// <param name="other">The UnitTransferReceipt to compare with the 
        /// current UnitTransferReceipt.</param>
        /// <returns>true if the specified UnitTransferReceipt is equal to the 
        /// current UnitTransferReceipt; otherwise, false. </returns>
        public bool Equals(UnitTransferReceipt other)
        {
            return (other != null && m_number == other.m_number);
        }

        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
                m_headerColumn3Length = SmallHeaderColumn3Length;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
                m_headerColumn3Length = NormalHeaderColumn3Length;
            }

            // DE131:
            m_bodyColumn1Length = 16;
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }
        
        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1))
                m_printer.AddLine(m_operatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine2))
                m_printer.AddLine(m_operatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine3))
                m_printer.AddLine(m_operatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine2) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected void PrintHeader()
        {
            m_printer.AddLine(Resources.UnitTransferReceiptTitle, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            string temp;

            // Receipt Number & Machine Id
            temp = Resources.ReceiptNumber.PadRight(m_headerColumn1Length) + m_number.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_machineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_machineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Gaming Date
            temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString();

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Cashier (Staff)
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + m_cashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        //DE131:
        /// <summary>
        /// Prints information related to the player assigned to.
        /// </summary>
        protected void PrintPlayerInfo()
        {
            // Player Id
            if (m_printPlayerID && m_playerId != 0)
                m_printer.AddLine(Resources.ReceiptPlayerNumber.PadRight(m_bodyColumn1Length) + m_playerId.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontMedium);

            // Player Name
            if (!string.IsNullOrEmpty(m_playerName))
                m_printer.AddLine(Resources.ReceiptPlayerName.PadRight(m_bodyColumn1Length) + m_playerName, StringAlignment.Near, m_fontMedium);

            // Player Info and Spacing.
            if ((m_playerId != 0) || !string.IsNullOrEmpty(m_playerName))
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints information related to the units transferred.
        /// </summary>
        protected void PrintUnitInfo()
        {
            // Original Receipt #
            if (m_originalNumber > 0) //DE131:
            {
                m_printer.AddLine(Resources.UnitTransferReceiptOrigNum + " " + m_originalNumber.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontSmall);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

                // From Device Name
                m_printer.AddLine(m_fromDeviceName, StringAlignment.Center, m_fontBig);

                // From Unit Number
                m_printer.AddLine(Resources.ReceiptUnitNumber + " " + m_fromUnitNumber.ToString(CultureInfo.CurrentCulture), StringAlignment.Center, m_fontBig);

                // From Serial Number
                m_printer.AddLine(Resources.ReceiptSerialNumber + " " + m_fromSerialNumber, StringAlignment.Center, m_fontBig);

                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(Resources.UnitTransferReceiptTransfer, StringAlignment.Center, m_fontBig);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            // To Device Name
            m_printer.AddLine(m_toDeviceName, StringAlignment.Center, m_fontBig);

            // To Unit Number
            m_printer.AddLine(Resources.ReceiptUnitNumber + " " + m_toUnitNumber.ToString(CultureInfo.CurrentCulture), StringAlignment.Center, m_fontBig);

            // To Serial Number
            m_printer.AddLine(Resources.ReceiptSerialNumber + " " + m_toSerialNumber, StringAlignment.Center, m_fontBig);
        }

        /// <summary>
        /// Prints the receipt's footer.
        /// </summary>
        protected void PrintFooter()
        {
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Operator's footer lines.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1))
                m_printer.AddLine(m_operatorFooterLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine2))
                m_printer.AddLine(m_operatorFooterLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine3))
                m_printer.AddLine(m_operatorFooterLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1) ||
               !string.IsNullOrEmpty(m_operatorFooterLine2) ||
               !string.IsNullOrEmpty(m_operatorFooterLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

//            if (!SaleSuccess)
//                m_printer.AddLine(Resources.IncompleteTransactionTitle, StringAlignment.Center, m_fontExtraHuge);

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            // System date.
            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void PrintPreview(Printer printer)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();
            
            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            PrintCharityHeader();

            //DE131: Print the Player's information.
            PrintPlayerInfo();

            // Print unit information.
            PrintUnitInfo();

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints information related to the charities sold to on the receipt
        /// </summary>
        protected void PrintCharityHeader()
        {
            if (m_charities != null && m_charities.Count > 0)
            {
                string temp;

                m_printer.AddLine(Resources.ReceiptChaityTitle, StringAlignment.Center, m_fontBig);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);

                for (int n = 0; n < m_charities.Count; ++n)
                {
                    temp = string.Format(CultureInfo.CurrentCulture, Resources.ReceiptSession, m_charities[n].Session).PadRight(m_headerColumn1Length);
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = Resources.ReceiptCharityName.PadRight(m_headerColumn1Length) + m_charities[n].Name;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = Resources.ReceiptCharityLicense.PadRight(m_headerColumn1Length) + m_charities[n].LicenseNumber;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = Resources.ReceiptCharityTaxId.PadRight(m_headerColumn1Length) + m_charities[n].TaxPayerId;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontSmall);
                }

                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, short copies)
        {
            PrintPreview(printer);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets whether this receipt is a reprint.
        /// </summary>
        public bool IsReprint
        {
            get
            {
                return m_isReprint;
            }
            set
            {
                m_isReprint = value;
            }
        }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1
        {
            get
            {
                return m_operatorHeaderLine1;
            }
            set
            {
                m_operatorHeaderLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2
        {
            get
            {
                return m_operatorHeaderLine2;
            }
            set
            {
                m_operatorHeaderLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3
        {
            get
            {
                return m_operatorHeaderLine3;
            }
            set
            {
                m_operatorHeaderLine3 = value;
            }
        }

        //DE131:
        /// <summary>
        /// Get/Set the Body Column 1 Lenght.
        /// </summary>
        public int BodyColumn1Lenght
        {
            get
            {
                return m_bodyColumn1Length;
            }
            set
            {
                m_bodyColumn1Length = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's first receipt footer line.
        /// </summary>
        public string OperatorFooterLine1
        {
            get
            {
                return m_operatorFooterLine1;
            }
            set
            {
                m_operatorFooterLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt footer line.
        /// </summary>
        public string OperatorFooterLine2
        {
            get
            {
                return m_operatorFooterLine2;
            }
            set
            {
                m_operatorFooterLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt footer line.
        /// </summary>
        public string OperatorFooterLine3
        {
            get
            {
                return m_operatorFooterLine3;
            }
            set
            {
                m_operatorFooterLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt transaction number.
        /// </summary>
        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
            set
            {
                m_gamingDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the machine that performed the transfer.
        /// </summary>
        public int MachineId
        {
            get
            {
                return m_machineId;
            }
            set
            {
                m_machineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff who made the transfer.
        /// </summary>
        public string Cashier
        {
            get
            {
                return m_cashier;
            }
            set
            {
                m_cashier = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt number of the original transaction.
        /// </summary>
        public int OriginalNumber
        {
            get
            {
                return m_originalNumber;
            }
            set
            {
                m_originalNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device transferred from.
        /// </summary>
        public string FromDeviceName
        {
            get
            {
                return m_fromDeviceName;
            }
            set
            {
                m_fromDeviceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the unit number of the device transferred from.
        /// </summary>
        public short FromUnitNumber
        {
            get
            {
                return m_fromUnitNumber;
            }
            set
            {
                m_fromUnitNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the serial number of the device transferred from.
        /// </summary>
        public string FromSerialNumber
        {
            get
            {
                return m_fromSerialNumber;
            }
            set
            {
                m_fromSerialNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device transferred to.
        /// </summary>
        public string ToDeviceName
        {
            get
            {
                return m_toDeviceName;
            }
            set
            {
                m_toDeviceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the unit number of the device transferred to.
        /// </summary>
        public short ToUnitNumber
        {
            get
            {
                return m_toUnitNumber;
            }
            set
            {
                m_toUnitNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the serial number of the device transferred to.
        /// </summary>
        public string ToSerialNumber
        {
            get
            {
                return m_toSerialNumber;
            }
            set
            {
                m_toSerialNumber = value;
            }
        }

        //DE131:
        /// <summary>
        /// Get/Set the Player ID.
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

        //DE131:
        /// <summary>
        /// Get/Set the Player's Name.
        /// </summary>
        public string PlayerName
        {
            get
            {
                return m_playerName;
            }
            set
            {
                m_playerName = value;
            }
        }

        public List<SessionCharity> Charities
        {
            get { return m_charities; }
            set { m_charities = value; }
        }
        #endregion
    }

    /// <summary>
    /// Represents a printed payout receipt.
    /// </summary>
    public class PayoutReceipt : Receipt, IEquatable<PayoutReceipt>
    {
        #region Constants and Data Types
        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalBodyColumn1Length = 16;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallBodyColumn1Length = 16;
        #endregion

        #region Member Variables
        protected int m_headerColumn1Length;
        protected int m_headerColumn2Length;
        protected int m_headerColumn3Length;
        protected int m_bodyColumn1Length;
        protected int m_headerMachineDescLength = 17;

        protected bool m_isReprint;
        protected string m_operatorHeaderLine1;
        protected string m_operatorHeaderLine2;
        protected string m_operatorHeaderLine3;
        protected string m_operatorFooterLine1;
        protected string m_operatorFooterLine2;
        protected string m_operatorFooterLine3;
        protected int m_number;
        protected DateTime m_gamingDate;
        protected int m_machineId;
        protected string m_cashier;
        protected string m_payCashier;
        protected bool m_machineAccount;
        protected int m_playerId;
        protected string m_playerName;
        protected string m_gameName;
        protected string m_gameType;
        protected DateTime m_dateWon;
        protected decimal m_totalPrize;
        protected int m_numOfWinners;
        protected decimal m_amountPaid;
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two PayoutReceipt instances are equal.
        /// </summary>
        /// <param name="obj">The PayoutReceipt to compare with the 
        /// current PayoutReceipt.</param>
        /// <returns>true if the specified PayoutReceipt is equal to the 
        /// current PayoutReceipt; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            PayoutReceipt receipt = obj as PayoutReceipt;

            if (receipt == null)
                return false;
            else
                return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a PayoutReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current PayoutReceipt.</returns>
        public override int GetHashCode()
        {
            return m_number.GetHashCode();
        }

        /// <summary>
        /// Determines whether two PayoutReceipt instances are equal. 
        /// </summary>
        /// <param name="other">The PayoutReceipt to compare with the 
        /// current PayoutReceipt.</param>
        /// <returns>true if the specified PayoutReceipt is equal to the 
        /// current PayoutReceipt; otherwise, false. </returns>
        public bool Equals(PayoutReceipt other)
        {
            return (other != null && m_number == other.m_number);
        }

        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
                m_headerColumn3Length = SmallHeaderColumn3Length;
                m_bodyColumn1Length = SmallBodyColumn1Length;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
                m_headerColumn3Length = NormalHeaderColumn3Length;
                m_bodyColumn1Length = NormalBodyColumn1Length;
            }
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }
        
        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1))
                m_printer.AddLine(m_operatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine2))
                m_printer.AddLine(m_operatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine3))
                m_printer.AddLine(m_operatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine2) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected void PrintHeader()
        {
            m_printer.AddLine(Resources.PayOutReceiptTitle, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            string temp;

            // Receipt Number & Machine Id
            temp = Resources.ReceiptNumber.PadRight(m_headerColumn1Length) + m_number.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_machineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_machineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Gaming Date
            if (m_gamingDate == DateTime.MinValue)
                temp = "";
            else
                temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString();

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Cashier (Staff)
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + m_cashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            // Pay Cashier (Staff)
            if (!string.IsNullOrEmpty(m_payCashier))
            {
                temp = (Resources.PayoutReceiptPayStaff + ":").PadRight(m_headerColumn1Length) + m_payCashier;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints information related to the player sold to.
        /// </summary>
        protected void PrintPlayerInfo()
        {
            // Player Id
            if (!m_machineAccount && m_printPlayerID && m_playerId != 0)
                m_printer.AddLine(Resources.ReceiptPlayerNumber.PadRight(m_bodyColumn1Length) + m_playerId.ToString(CultureInfo.CurrentCulture), StringAlignment.Near, m_fontMedium);

            // Player Name
            if (!string.IsNullOrEmpty(m_playerName))
                m_printer.AddLine(Resources.ReceiptPlayerName.PadRight(m_bodyColumn1Length) + m_playerName, StringAlignment.Near, m_fontMedium);

            // Player Info and Spacing.
            if ((!m_machineAccount && m_playerId != 0) || !string.IsNullOrEmpty(m_playerName))
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the information related to the win.
        /// </summary>
        /// <param name="printSignatureLines">Whether to print a signature 
        /// lines.</param>
        protected void PrintWinningInfo(bool printSignatureLines)
        {
            // Game Name
            if (!string.IsNullOrEmpty(m_gameName))
                m_printer.AddLine(Resources.PayoutReceiptGame.PadRight(m_bodyColumn1Length) + m_gameName, StringAlignment.Near, m_fontMedium);

            // Game Type
            if (!string.IsNullOrEmpty(m_gameType))
                m_printer.AddLine(Resources.PayoutReceiptGameType.PadRight(m_bodyColumn1Length) + m_gameType, StringAlignment.Near, m_fontMedium);

            // Date Won
            if (m_dateWon != DateTime.MinValue)
                m_printer.AddLine(Resources.PayoutReceiptDateWon.PadRight(m_bodyColumn1Length) + m_dateWon.ToShortDateString(), StringAlignment.Near, m_fontMedium);

            // Total Prize
            if (m_totalPrize != 0M)
                m_printer.AddLine(Resources.PayoutReceiptTotalPrize.PadRight(m_bodyColumn1Length) + m_totalPrize.ToString("C", CultureInfo.CurrentCulture), StringAlignment.Near, m_fontMedium);

            // Number of Winners
            if (m_numOfWinners != 0)
                m_printer.AddLine(Resources.PayoutReceiptNumWinners.PadRight(m_bodyColumn1Length) + m_numOfWinners.ToString("D", CultureInfo.CurrentCulture), StringAlignment.Near, m_fontMedium);

            m_printer.AddLine(Resources.PayoutReceiptPaidOut.PadRight(m_bodyColumn1Length) + m_amountPaid.ToString("C", CultureInfo.CurrentCulture), StringAlignment.Near, m_fontMedium);

            // Winner and Paid By Signatures
            if (printSignatureLines)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

                m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
                //m_printer.AddLine(Resources.PayoutReceiptWinner, StringAlignment.Center, m_fontSmall);
                //Hardcode for generic signature line names for 3.6.6 Vegas
                m_printer.AddLine("Signature 1", StringAlignment.Center, m_fontSmall);

                if (!string.IsNullOrEmpty(m_payCashier))
                {
                    m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                    m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

                    m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
                    m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
                    //m_printer.AddLine(Resources.PayoutReceiptPayStaff, StringAlignment.Center, m_fontSmall);
                    //Hardcode for generic signature line names for 3.6.6 Vegas
                    m_printer.AddLine("Signature 2", StringAlignment.Center, m_fontSmall);
                }
            }
        }

        /// <summary>
        /// Prints the receipt's footer.
        /// </summary>
        protected void PrintFooter()
        {
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Operator's footer lines.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1))
                m_printer.AddLine(m_operatorFooterLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine2))
                m_printer.AddLine(m_operatorFooterLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine3))
                m_printer.AddLine(m_operatorFooterLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1) ||
               !string.IsNullOrEmpty(m_operatorFooterLine2) ||
               !string.IsNullOrEmpty(m_operatorFooterLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

//            if (!SaleSuccess)
//                m_printer.AddLine(Resources.IncompleteTransactionTitle, StringAlignment.Center, m_fontExtraHuge);

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            // System date.
            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="printSignatureLines">Whether to print a signature 
        /// lines.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void PrintPreview(Printer printer, bool printSignatureLines)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            // Print the player information.
            PrintPlayerInfo();

            // Print win information.
            PrintWinningInfo(printSignatureLines);

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="printSignatureLines">Whether to print a signature 
        /// lines.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, bool printSignatureLines, short copies)
        {
            PrintPreview(printer, printSignatureLines);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets whether this receipt is a reprint.
        /// </summary>
        public bool IsReprint
        {
            get
            {
                return m_isReprint;
            }
            set
            {
                m_isReprint = value;
            }
        }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1
        {
            get
            {
                return m_operatorHeaderLine1;
            }
            set
            {
                m_operatorHeaderLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2
        {
            get
            {
                return m_operatorHeaderLine2;
            }
            set
            {
                m_operatorHeaderLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3
        {
            get
            {
                return m_operatorHeaderLine3;
            }
            set
            {
                m_operatorHeaderLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's first receipt footer line.
        /// </summary>
        public string OperatorFooterLine1
        {
            get
            {
                return m_operatorFooterLine1;
            }
            set
            {
                m_operatorFooterLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt footer line.
        /// </summary>
        public string OperatorFooterLine2
        {
            get
            {
                return m_operatorFooterLine2;
            }
            set
            {
                m_operatorFooterLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt footer line.
        /// </summary>
        public string OperatorFooterLine3
        {
            get
            {
                return m_operatorFooterLine3;
            }
            set
            {
                m_operatorFooterLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt transaction number.
        /// </summary>
        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the machine that made the payout.
        /// </summary>
        public int MachineId
        {
            get
            {
                return m_machineId;
            }
            set
            {
                m_machineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
            set
            {
                m_gamingDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff who made the payout.
        /// </summary>
        public string Cashier
        {
            get
            {
                return m_cashier;
            }
            set
            {
                m_cashier = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff who actually gave the payout to the player 
        /// (if applicable).
        /// </summary>
        public string PayCashier
        {
            get
            {
                return m_payCashier;
            }
            set
            {
                m_payCashier = value;
            }
        }

        // TTP 50114
        /// <summary>
        /// Gets or sets if the player who won was a machine (if applicable).
        /// </summary>
        public bool MachineAccount
        {
            get
            {
                return m_machineAccount;
            }
            set
            {
                m_machineAccount = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the player who won (if applicable).
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
        /// Gets or sets the name of the player who won (if applicable).
        /// </summary>
        public string PlayerName
        {
            get
            {
                return m_playerName;
            }
            set
            {
                m_playerName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the game won.
        /// </summary>
        public string GameName
        {
            get
            {
                return m_gameName;
            }
            set
            {
                m_gameName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the type of game won.
        /// </summary>
        public string GameType
        {
            get
            {
                return m_gameType;
            }
            set
            {
                m_gameType = value;
            }
        }

        /// <summary>
        /// Gets or sets the date the prize was won.
        /// </summary>
        public DateTime DateWon
        {
            get
            {
                return m_dateWon;
            }
            set
            {
                m_dateWon = value;
            }
        }

        /// <summary>
        /// Gets or sets the total prize of the game.
        /// </summary>
        public decimal TotalPrize
        {
            get
            {
                return m_totalPrize;
            }
            set
            {
                m_totalPrize = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of winners for the payout.
        /// </summary>
        public int NumberOfWinners
        {
            get
            {
                return m_numOfWinners;
            }
            set
            {
                m_numOfWinners = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount paid out.
        /// </summary>
        public decimal AmountPaid
        {
            get
            {
                return m_amountPaid;
            }
            set
            {
                m_amountPaid = value;
            }
        }
        #endregion
    }

    // Rally TA7464
    /// <summary>
    /// Represents a printed bank adjustment receipt.
    /// </summary>
    public class BankAdjustmentReceipt : Receipt, IEquatable<BankAdjustmentReceipt>
    {
        #region Constants and Data Types
        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        #endregion

        #region Member Variables
        protected int m_headerColumn1Length;
        protected int m_headerColumn2Length;
        protected int m_headerMachineDescLength = 17;

        protected bool m_isReprint;
        protected string m_operatorHeaderLine1;
        protected string m_operatorHeaderLine2;
        protected string m_operatorHeaderLine3;
        protected TransactionType m_transType = TransactionType.InitialBankIssue;
        protected DateTime m_gamingDate;
        protected short m_session;
        protected int m_machineId;
        protected DateTime m_issuedDate;
        protected string m_issuedBy;
        protected string m_issuedTo;
        protected Dictionary<string, decimal> m_amounts = new Dictionary<string, decimal>();
        #endregion

        #region Member Methods
        /// <summary>
        /// Determines whether two BankAdjustmentReceipt instances are equal.
        /// </summary>
        /// <param name="obj">The BankAdjustmentReceipt to compare with the 
        /// current BankAdjustmentReceipt.</param>
        /// <returns>true if the specified BankAdjustmentReceipt is equal to 
        /// the current BankAdjustmentReceipt; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            BankAdjustmentReceipt receipt = obj as BankAdjustmentReceipt;

            if (receipt == null)
                return false;
            else
                return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a BankAdjustmentReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current 
        /// BankAdjustmentReceipt.</returns>
        public override int GetHashCode()
        {
            return m_transType.GetHashCode() ^ m_machineId.GetHashCode() ^ m_gamingDate.GetHashCode();
        }

        /// <summary>
        /// Determines whether two BankAdjustmentReceipt instances are equal. 
        /// </summary>
        /// <param name="other">The BankAdjustmentReceipt to compare with the 
        /// current BankAdjustmentReceipt.</param>
        /// <returns>true if the specified BankAdjustmentReceipt is equal to 
        /// the current BankAdjustmentReceipt; otherwise, false. </returns>
        public bool Equals(BankAdjustmentReceipt other)
        {
            return (other != null && m_transType == other.m_transType &&
                    m_machineId == other.m_machineId && m_gamingDate == other.m_gamingDate);
        }

        /// <summary>
        /// Adds an amount to be printed on the receipt.
        /// </summary>
        /// <param name="currency">The name of the currency adjusted.</param>
        /// <param name="amount">The adjustment amount.</param>
        public void AddAmount(string currency, decimal amount)
        {
            if (!m_amounts.ContainsKey(currency))
                m_amounts.Add(currency, 0);

            m_amounts[currency] = amount;
        }

        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
            }
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1))
                m_printer.AddLine(m_operatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine2))
                m_printer.AddLine(m_operatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine3))
                m_printer.AddLine(m_operatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine2) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected void PrintHeader()
        {
            if (m_transType == TransactionType.CloseBank)
                m_printer.AddLine(Resources.BankAdjustReceiptTitleClose, StringAlignment.Center, m_fontBig);
            else if (m_transType == TransactionType.BankDrop)
                m_printer.AddLine(Resources.BankAdjustReceiptTitleDrop, StringAlignment.Center, m_fontBig);
            else
                m_printer.AddLine(Resources.BankAdjustReceiptTitleIssue, StringAlignment.Center, m_fontBig);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            string temp;

            // Machine Id + desc
            temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_machineId.ToString(CultureInfo.CurrentCulture);

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Gaming Date
            if (m_gamingDate != DateTime.MinValue)
            {
                temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString();
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Session
            if (m_session != 0)
            {
                temp = Resources.SessionTitle.PadRight(m_headerColumn1Length) + m_session;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Issued On
            if (m_issuedDate != DateTime.MinValue)
            {
                temp = Resources.ReceiptIssuedOn.PadRight(m_headerColumn1Length) + m_issuedDate.ToString();
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Issued By
            if (!string.IsNullOrEmpty(m_issuedBy))
            {
                temp = Resources.ReceiptIssuedBy.PadRight(m_headerColumn1Length) + m_issuedBy;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Issued To
            if (!string.IsNullOrEmpty(m_issuedTo))
            {
                temp = Resources.ReceiptIssuedTo.PadRight(m_headerColumn1Length) + m_issuedTo;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
        }

        /// <summary>
        /// Prints information related to the bank adjustment.
        /// </summary>
        protected void PrintInfo()
        {
            // Adjustment Amounts
            foreach (KeyValuePair<string, decimal> pair in m_amounts)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Format(CultureInfo.CurrentCulture, Resources.BankAdjustReceiptCurrency, pair.Key), StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

                string val = Currency.FormatCurrencyString(pair.Key, pair.Value);
                m_printer.AddLine(" " + Resources.TenderTypeCash.PadRight(m_fontSmallMaxChars - Resources.TenderTypeCash.Length - val.Length) + val, StringAlignment.Near, m_fontSmall);
            }

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(m_issuedBy))
            {
                m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(Resources.ReceiptIssuedBy, StringAlignment.Center, m_fontSmall);
            }

            if (!string.IsNullOrEmpty(m_issuedTo))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

                m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(Resources.ReceiptIssuedTo, StringAlignment.Center, m_fontSmall);
            }
        }

        /// <summary>
        /// Prints the receipt's footer.
        /// </summary>
        protected void PrintFooter()
        {
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

//            if (!SaleSuccess)
//                m_printer.AddLine(Resources.IncompleteTransactionTitle, StringAlignment.Center, m_fontExtraHuge);

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            // System date.
            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void PrintPreview(Printer printer)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            // Print the adjustment information.
            PrintInfo();

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, short copies)
        {
            PrintPreview(printer);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets whether this receipt is a reprint.
        /// </summary>
        public bool IsReprint
        {
            get
            {
                return m_isReprint;
            }
            set
            {
                m_isReprint = value;
            }
        }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1
        {
            get
            {
                return m_operatorHeaderLine1;
            }
            set
            {
                m_operatorHeaderLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2
        {
            get
            {
                return m_operatorHeaderLine2;
            }
            set
            {
                m_operatorHeaderLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3
        {
            get
            {
                return m_operatorHeaderLine3;
            }
            set
            {
                m_operatorHeaderLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of adjustment being made.
        /// </summary>
        public TransactionType TransactionType
        {
            get
            {
                return m_transType;
            }
            set
            {
                m_transType = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the machine that made the adjustment.
        /// </summary>
        public int MachineId
        {
            get
            {
                return m_machineId;
            }
            set
            {
                m_machineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
            set
            {
                m_gamingDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the session associated with the bank, or 0 for no
        /// session.
        /// </summary>
        public short Session
        {
            get
            {
                return m_session;
            }
            set
            {
                m_session = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's issue date.
        /// </summary>
        public DateTime IssuedDate
        {
            get
            {
                return m_issuedDate;
            }
            set
            {
                m_issuedDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the source staff of the adjustment.
        /// </summary>
        public string IssuedBy
        {
            get
            {
                return m_issuedBy;
            }
            set
            {
                m_issuedBy = value;
            }
        }

        /// <summary>
        /// Gets or sets the destination staff of the adjustment.
        /// </summary>
        public string IssuedTo
        {
            get
            {
                return m_issuedTo;
            }
            set
            {
                m_issuedTo = value;
            }
        }
        #endregion
    }
    // END: TA7464

    public class RaffleReceipt : Receipt, IEquatable<RaffleReceipt>
    {
        #region Costants and Data Types
        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalBodyColumn1Length = 16;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallBodyColumn1Length = 16;
        #endregion


        #region Member Variables
        protected int m_headerColumn1Length;
        protected int m_headerColumn2Length;
        protected int m_headerColumn3Length;
        protected int m_bodyColumn1Length;

        protected bool m_isReprint;
        protected string m_operatorHeaderLine1;
        protected string m_operatorHeaderLine2;
        protected string m_operatorHeaderLine3;
        protected string m_operatorHeaderLine4;
        protected string m_operatorFooterLine1;
        protected string m_operatorFooterLine2;
        protected string m_operatorFooterLine3;
        protected int m_number;

        protected string m_headerLine1;
        protected string m_charity;
        protected string m_charityLicenseNumber;
        protected int m_raffleID;
        protected int m_numberOfWinners;
        protected int m_winnersCount;
        protected string m_raffleName;
        protected DateTime m_raffleDate;
        protected string m_raffleDescription;
        protected string m_rafflesWinnersName;
        protected int m_raffleWinnersID;
        protected string m_rafflesWinnerMagCard;
        protected string m_rafflesWinnerMailingAddress1;
        protected string m_rafflesWinnerMailingAddress2;
        protected string m_rafflesWinnerPhoneNumber;
        protected string m_raffleDisclaimer;

        #endregion

        #region Member Methods
        public override bool Equals(object obj)
        {
            RaffleReceipt receipt = obj as RaffleReceipt;
            if (receipt == null)
                return false;
            else
                return Equals(receipt);

        }

        public override int GetHashCode()
        {
            return m_number.GetHashCode();
        }

        public bool Equals(RaffleReceipt other)
        {
            return (other != null && m_number == other.m_number);
        }

        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
                m_headerColumn3Length = SmallHeaderColumn3Length;
                m_bodyColumn1Length = SmallBodyColumn1Length;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
                m_headerColumn3Length = NormalHeaderColumn3Length;
                m_bodyColumn1Length = NormalBodyColumn1Length;
            }
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1))
                m_printer.AddLine(m_operatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine2))
                m_printer.AddLine(m_operatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine3))
                m_printer.AddLine(m_operatorHeaderLine3, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine4))
                m_printer.AddLine(m_operatorHeaderLine4, StringAlignment.Center, m_fontBig);
            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine2) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }


        private void fixMultilineCharacterToFitOnReceipt(string stringtoModify, int type)
        {
            //check if the string contains newline
            if (stringtoModify.Contains(Environment.NewLine))
            {
                //Replace it with a diffirent character since its very hard to deal with this '\r\n'
                stringtoModify = System.Text.RegularExpressions.Regex.Replace(stringtoModify, @"\t|\n|\r", "~");

                //get the first line
                while (stringtoModify.IndexOf("~~") != -1) //loop untill this newline is = -1 
                {
                    string stringLine = stringtoModify.Substring(0, stringtoModify.IndexOf("~~")); //this is the current line 
                    fixStringThatAreTooLongAndItWillNotFitOnTheReceipt(stringLine, type);
                    stringtoModify = stringtoModify.Substring(stringtoModify.IndexOf("~~") + 2); //removed current line
                }
                fixStringThatAreTooLongAndItWillNotFitOnTheReceipt(stringtoModify, type);

            }
            else
            {
                fixStringThatAreTooLongAndItWillNotFitOnTheReceipt(stringtoModify, type);
            }
        }

        private void fixStringThatAreTooLongAndItWillNotFitOnTheReceipt(string stringToModify, int type)//1 raffle discription (font big); 2 raffle disclaimer font small
        {
            string addStringLine = "";
            m_raffleDescription = stringToModify;//assume m_raffleDescription as a private var in this method
            int maxcharLenght = 0;
            if (type == 1) { maxcharLenght = 28; } else if (type == 2) { maxcharLenght = 32; }

            while (m_raffleDescription.IndexOf(" ") != -1)
            {
                if ((addStringLine + m_raffleDescription.Substring(0, m_raffleDescription.IndexOf(" ") + 1)).Length < maxcharLenght) //28)
                {
                    addStringLine = addStringLine + m_raffleDescription.Substring(0, m_raffleDescription.IndexOf(" ") + 1);

                }
                else
                {
                    //if 1 then prize description if 2 then disclaimer
                    if (type == 1)
                    {
                        m_printer.AddLine(addStringLine, StringAlignment.Center, m_fontBig);
                    }
                    else if (type == 2)
                    {
                        m_printer.AddLine(addStringLine, StringAlignment.Center, m_fontSmall);
                    }
                    addStringLine = "";
                    addStringLine = addStringLine + m_raffleDescription.Substring(0, m_raffleDescription.IndexOf(" ") + 1);

                }
                //if (type == 1)
                //{
                m_raffleDescription = m_raffleDescription.Substring(m_raffleDescription.IndexOf(" ") + 1);
                //}
                //else if (type == 2)
                //{

                //}
            }

            if ((addStringLine.Length + m_raffleDescription.Length) < maxcharLenght) //28)
            {
                if (type == 1)
                {
                    m_printer.AddLine(addStringLine + m_raffleDescription, StringAlignment.Center, m_fontBig);
                }
                else if (type == 2)
                {
                    m_printer.AddLine(addStringLine + m_raffleDescription, StringAlignment.Center, m_fontSmall);
                }
            }
            else
            {
                if (type == 1)
                {
                    m_printer.AddLine(addStringLine, StringAlignment.Center, m_fontBig);
                    m_printer.AddLine(m_raffleDescription, StringAlignment.Center, m_fontBig);
                }
                else if (type == 2)
                {
                    m_printer.AddLine(addStringLine, StringAlignment.Center, m_fontSmall);
                    m_printer.AddLine(m_raffleDescription, StringAlignment.Center, m_fontSmall);
                }
            }

        }

        protected void PrintHeader()
        {
            m_printer.AddLine(m_headerLine1 + " Voucher", StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            string temp;

            string addStringLine = "";
            int countline = 0;
            //So much calculation for adding a new line if the character will not fit on the receipt.
            while (m_raffleName.IndexOf(" ") != -1)
            {
                if ((addStringLine + m_raffleName.Substring(0, m_raffleName.IndexOf(" ") + 1)).Length < 28)
                {
                    addStringLine = addStringLine + m_raffleName.Substring(0, m_raffleName.IndexOf(" ") + 1);

                }
                else
                {
                    if (countline == 0)
                    {
                        temp = "Name:".PadRight(m_headerColumn1Length) + addStringLine;
                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                    }
                    else
                    {
                        temp = "".PadRight(m_headerColumn1Length) + addStringLine;
                        m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                    }
                    addStringLine = "";
                    addStringLine = addStringLine + m_raffleName.Substring(0, m_raffleName.IndexOf(" ") + 1);
                    countline = countline + 1;
                }

                m_raffleName = m_raffleName.Substring(m_raffleName.IndexOf(" ") + 1);
            }

            if (countline == 0)
            {

                if ((addStringLine.Length + m_raffleName.Length) < 28)
                {
                    temp = "Name:".PadRight(m_headerColumn1Length) + addStringLine + m_raffleName;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                }
                else
                {
                    temp = "Name:".PadRight(m_headerColumn1Length) + addStringLine;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = "".PadRight(m_headerColumn1Length) + m_raffleName;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                }

            }
            else
            {
                if ((addStringLine.Length + m_raffleName.Length) < 30)
                {
                    temp = "".PadRight(m_headerColumn1Length) + addStringLine + m_raffleName;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                }
                else
                {
                    temp = "".PadRight(m_headerColumn1Length) + addStringLine;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

                    temp = "".PadRight(m_headerColumn1Length) + m_raffleName;
                    m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                }

            }

            if (m_numberOfWinners != 1) //If its a multiwin raffle then show this
            {
                temp = "Winner:".PadRight(m_headerColumn1Length) + (m_winnersCount.ToString(CultureInfo.CurrentCulture) + " of " + m_numberOfWinners.ToString(CultureInfo.CurrentCulture)).PadRight(m_headerColumn2Length);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            temp = "Date:".PadRight(m_headerColumn1Length) + m_raffleDate.Date.ToShortDateString();
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

        }


        protected void PrintPlayerRaffleWinner()
        {
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            fixMultilineCharacterToFitOnReceipt(m_raffleDescription, 1);
            m_printer.AddLine("--------------------------", StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            string temp;

            temp = "Player Name:".PadRight(m_headerColumn1Length) + m_rafflesWinnersName;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            temp = "Player #:".PadRight(m_headerColumn1Length) + m_raffleWinnersID.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            temp = "Mag Card #: ".PadRight(m_headerColumn1Length) + m_rafflesWinnerMagCard.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            if (m_rafflesWinnerMailingAddress1 != null)
            {
                temp = "Address:".PadRight(m_headerColumn1Length) + m_rafflesWinnerMailingAddress1;//m_rafflesWinnerMagCard.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length); 
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            if (m_rafflesWinnerMailingAddress2 != null)
            {
                temp = "".PadRight(m_headerColumn1Length) + m_rafflesWinnerMailingAddress2;//m_rafflesWinnerMagCard.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length); 
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            if (m_rafflesWinnerPhoneNumber != null && m_rafflesWinnerPhoneNumber != "")
            {
                temp = "Phone #:".PadRight(m_headerColumn1Length) + m_rafflesWinnerPhoneNumber;//m_rafflesWinnerMagCard.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length); 
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
        }

        protected void PrintFooter()
        {
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            m_printer.AddLine("x:________________________", StringAlignment.Center, m_fontMedium);
            //add disclaimer.


            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(m_raffleDisclaimer))
            {
                fixMultilineCharacterToFitOnReceipt(m_raffleDisclaimer, 2);
            }

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Operator's footer lines.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1))
                m_printer.AddLine(m_operatorFooterLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine2))
                m_printer.AddLine(m_operatorFooterLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine3))
                m_printer.AddLine(m_operatorFooterLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1) ||
               !string.IsNullOrEmpty(m_operatorFooterLine2) ||
               !string.IsNullOrEmpty(m_operatorFooterLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

//            if (!SaleSuccess)
//                m_printer.AddLine(Resources.IncompleteTransactionTitle, StringAlignment.Center, m_fontExtraHuge);

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            // System date.
            m_printer.AddLine(DateTime.Now.ToString(), StringAlignment.Center, m_fontMedium);
        }

        public void PrintPreview(Printer printer)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");


            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            PrintPlayerRaffleWinner();

            PrintFooter();
        }

        public void Print(Printer printer, short copies)
        {
            PrintPreview(printer);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }

        #endregion

        #region Member Properties



        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        public string OperatorHeaderLine1
        {
            get
            {
                return m_operatorHeaderLine1;
            }
            set
            {
                m_operatorHeaderLine1 = value;
            }
        }

        public string OperatorHeaderLine2
        {
            get
            {
                return m_operatorHeaderLine2;
            }
            set
            {
                m_operatorHeaderLine2 = value;
            }
        }

        public string OperatorHeaderLine3
        {
            get
            {
                return m_operatorHeaderLine3;
            }
            set
            {
                m_operatorHeaderLine3 = value;
            }
        }

        public string OperatorHeaderLine4
        {
            get
            {
                return m_operatorHeaderLine4;
            }
            set
            {
                m_operatorHeaderLine4 = value;
            }
        }

        public string OperatorFooterLine1
        {
            get
            {
                return m_operatorFooterLine1;
            }
            set
            {
                m_operatorFooterLine1 = value;
            }
        }

        public string OperatorFooterLine2
        {
            get
            {
                return m_operatorFooterLine2;
            }
            set
            {
                m_operatorFooterLine2 = value;
            }
        }

        public string OperatorFooterLine3
        {
            get
            {
                return m_operatorFooterLine3;
            }
            set
            {
                m_operatorFooterLine3 = value;
            }
        }

        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        public string HeaderLine1
        {
            get { return m_headerLine1; }
            set { m_headerLine1 = value; }
        }

        public string Charity
        {
            get { return m_charity; }
            set { m_charity = value; }
        }

        public string CharityLicenseNumber
        {
            get { return m_charityLicenseNumber; }
            set { m_charityLicenseNumber = value; }
        }

        public int RaffleID
        {
            get { return m_raffleID; }
            set { m_raffleID = value; }
        }

        public int NoOfWinners
        {
            get { return m_numberOfWinners; }
            set { m_numberOfWinners = value; }
        }

        public int WinnersCount
        {
            get { return m_winnersCount; }
            set { m_winnersCount = value; }
        }

        public string RaffleName
        {
            get { return m_raffleName; }
            set { m_raffleName = value; }
        }

        public DateTime RaflleDate
        {
            get { return m_raffleDate; }
            set { m_raffleDate = value; }
        }

        public string RaffleDescription
        {
            get { return m_raffleDescription; }
            set { m_raffleDescription = value; }
        }

        public string RafflesWinnerName
        {
            get { return m_rafflesWinnersName; }
            set { m_rafflesWinnersName = value; }
        }

        public int RafflesWinnersID
        {
            get { return m_raffleWinnersID; }
            set { m_raffleWinnersID = value; }
        }

        public string RafflesWinnerMagCard
        {
            get { return m_rafflesWinnerMagCard; }
            set { m_rafflesWinnerMagCard = value; }
        }

        public string RafflesWinnerMailingAddress1
        {
            get { return m_rafflesWinnerMailingAddress1; }
            set { m_rafflesWinnerMailingAddress1 = value; }
        }

        public string RafflesWinnerMailingAddress2
        {
            get { return m_rafflesWinnerMailingAddress2; }
            set { m_rafflesWinnerMailingAddress2 = value; }
        }

        public string RafflesWinnerPhoneNumber
        {
            get { return m_rafflesWinnerPhoneNumber; }
            set { m_rafflesWinnerPhoneNumber = value; }
        }

        public string RaffleDisclaimer
        {
            get
            {
                return m_raffleDisclaimer;
            }
            set
            {
                m_raffleDisclaimer = value;
            }
        }

        #endregion
    }

    public abstract class B3Receipt : Receipt, IEquatable<B3SalesReceipt>
    {
        #region Constants and Data Types
        protected const int SpacesBetweenCards = 1;

        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalPlayerLength = 16;
        protected const int NormalDeviceFeeLength = 12;
        protected const int NormalTaxesLength = 12;
        protected const int NormalTotalLength = 12;
        protected const int NormalTenderedLength = 12;
        protected const int NormalChangeLength = 12;
        protected const int NormalPtsRedeemedLength = 12;
        protected const int NormalTotalCardsColumns = 3;
        protected const int NormalTotalCardsGameLength = 3;
        protected const int NormalTotalCardsLength = 4;
        protected const int NormalCardNumberColumns = 4;
        protected const int NormalCardNumberGameLength = 3;
        protected const int NormalCardNumberColumn1Length = 9;
        protected const int NormalCardNumberLength = 7;
        protected const int NormalCBBCardNumbersPerLine = 8; // Rally US505, US2228
        protected const int NormalPaperPackInfoColumnLength = 10;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallPlayerLength = 16;
        protected const int SmallDeviceFeeLength = 12;
        protected const int SmallTaxesLength = 12;
        protected const int SmallTotalLength = 12;
        protected const int SmallTenderedLength = 12;
        protected const int SmallChangeLength = 12;
        protected const int SmallPtsRedeemedLength = 12;
        protected const int SmallTotalCardsColumns = 2;
        protected const int SmallTotalCardsGameLength = 3;
        protected const int SmallTotalCardsLength = 4;
        protected const int SmallCardNumberColumns = 3;
        protected const int SmallCardNumberGameLength = 3;
        protected const int SmallCardNumberColumn1Length = 9;
        protected const int SmallCardNumberLength = 7;
        protected const int SmallCBBCardNumbersPerLine = 8; // Rally US505
        protected const int SmallPaperPackInfoColumnLength = 10;
        #endregion

        #region Member Variables
        protected int m_headerColumn1Length;
        protected int m_headerColumn2Length;
        protected int m_headerColumn3Length;
        protected int m_headerMachineDescLength = 17;
        protected int m_playerLength;
        protected int m_deviceFeeLength;
        protected int m_taxesLength;
        protected int m_totalLength;
        protected int m_tenderedLength;
        protected int m_changeLength;
        protected int m_ptsRedeemedLength;
        protected int m_totalCardsColumns;
        protected int m_totalCardsGameLength;
        protected int m_totalCardsLength;
        protected int m_cardNumberColumns;
        protected int m_cardNumberGameLength;
        protected int m_cardNumberColumn1Length;
        protected int m_paperPackInfoColumnLength;
        protected int m_cardNumberLength;
        protected int m_cbbCardNumbersPerLine; // Rally US505

        protected bool m_isReprint;
        protected bool m_isReturn; // Rally DE1863 - Add the wording "Return" on a receipt printed when in return mode.
        protected string m_operatorHeaderLine1;
        protected string m_operatorHeaderLine2;
        protected string m_operatorHeaderLine3;
        protected string m_operatorFooterLine1;
        protected string m_operatorFooterLine2;
        protected string m_operatorFooterLine3;
        protected int m_registerReceiptId; // Rally US505
        protected DateTime m_gamingDate;
        protected DateTime m_transactionDate;
        protected int m_soldFromMachineId;
        protected string m_cashier;
        protected int m_packNumber;
        protected string m_deviceName;
        protected short m_unitNumber;
        // Rally US1638
        
        protected string m_clientId;
        // END: US1638
        protected string m_serialNumber;
        protected bool m_machineAccount;
        protected int m_playerId;
        protected string m_playerName;
        protected decimal m_playerPts;
        protected decimal m_playerPtsEarned;
        protected decimal m_playerPtsRedeem;
        // Rally TA7464
        protected string m_defaultCurrency;
        protected string m_saleCurrency;
        protected decimal m_exchangeRate;
        protected decimal m_grandTotal;
        protected decimal m_changeDue;
        // END: TA7464
        protected decimal m_deviceFee;
        protected decimal m_taxes;
        protected decimal m_total;
        protected decimal m_amountTendered;
        protected string m_disclaimer1;
        protected string m_disclaimer2;
        protected string m_disclaimer3;
        protected bool m_printLotto;
        #endregion

        protected B3Receipt(string sessionName, int sessionNumber, int accountNumber, int receiptNumber)
        {
            SessionName = sessionName;
            SessionNumber = sessionNumber;
            ReceiptNumber = receiptNumber;
            AccountNumber = accountNumber;
        }

        #region Member Methods
        /// <summary>
        /// Determines whether two SalesReceipt instances are equal. 
        /// </summary>
        /// <param name="obj">The SalesReceipt to compare with the 
        /// current SalesReceipt.</param>
        /// <returns>true if the specified SalesReceipt is equal to the current 
        /// SalesReceipt; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            B3SalesReceipt receipt = obj as B3SalesReceipt;

            if (receipt == null)
            {
                return false;
            }


            return Equals(receipt);
        }

        /// <summary>
        /// Serves as a hash function for a SalesReceipt. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current SalesReceipt.</returns>
        public override int GetHashCode()
        {
            return ReceiptNumber.GetHashCode();
        }

        public bool Equals(B3SalesReceipt other)
        {
            return other != null && ReceiptNumber == other.ReceiptNumber;
        }

        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
                m_headerColumn3Length = SmallHeaderColumn3Length;
                m_playerLength = SmallPlayerLength;
                m_deviceFeeLength = SmallDeviceFeeLength;
                m_taxesLength = SmallTaxesLength;
                m_totalLength = SmallTotalLength;
                m_tenderedLength = SmallTenderedLength;
                m_changeLength = SmallChangeLength;
                m_ptsRedeemedLength = SmallPtsRedeemedLength;
                m_totalCardsColumns = SmallTotalCardsColumns;
                m_totalCardsGameLength = SmallTotalCardsGameLength;
                m_totalCardsLength = SmallTotalCardsLength;
                m_cardNumberColumns = SmallCardNumberColumns;
                m_cardNumberGameLength = SmallCardNumberGameLength;
                m_cardNumberColumn1Length = SmallCardNumberColumn1Length;
                m_cardNumberLength = SmallCardNumberLength;
                m_cbbCardNumbersPerLine = SmallCBBCardNumbersPerLine; // Rally US505
                m_paperPackInfoColumnLength = SmallPaperPackInfoColumnLength;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
                m_headerColumn3Length = NormalHeaderColumn3Length;
                m_playerLength = NormalPlayerLength;
                m_deviceFeeLength = NormalDeviceFeeLength;
                m_taxesLength = NormalTaxesLength;
                m_totalLength = NormalTotalLength;
                m_tenderedLength = NormalTenderedLength;
                m_changeLength = NormalChangeLength;
                m_ptsRedeemedLength = NormalPtsRedeemedLength;
                m_totalCardsColumns = NormalTotalCardsColumns;
                m_totalCardsGameLength = NormalTotalCardsGameLength;
                m_totalCardsLength = NormalTotalCardsLength;
                m_cardNumberColumns = NormalCardNumberColumns;
                m_cardNumberGameLength = NormalCardNumberGameLength;
                m_cardNumberColumn1Length = NormalCardNumberColumn1Length;
                m_cardNumberLength = NormalCardNumberLength;
                m_cbbCardNumbersPerLine = NormalCBBCardNumbersPerLine; // Rally US505
                m_paperPackInfoColumnLength = NormalPaperPackInfoColumnLength;
            }
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1))
                m_printer.AddLine(m_operatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine2))
                m_printer.AddLine(m_operatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorHeaderLine3))
                m_printer.AddLine(m_operatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorHeaderLine1) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine2) ||
               !string.IsNullOrEmpty(m_operatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected virtual void PrintHeader()
        {
            m_printer.AddLine(HeaderTitle, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Receipt Number & Sold From Machine Id
            var temp = Resources.ReceiptNumber.PadRight(m_headerColumn1Length) + ReceiptNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_soldFromMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_soldFromMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Gaming Date
            temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString().PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Cashier (Staff)
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + m_cashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            //account number
            temp = Resources.AccountNumberString.PadRight(m_headerColumn1Length) + AccountNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            //add session number           
            if (!m_useSmallSizes)
            {
                temp += Resources.SessionTitle.PadRight(m_headerColumn3Length) + SessionNumber;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.SessionTitle.PadRight(m_headerColumn1Length) + SessionNumber;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the body.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void PrintBody()
        {
            m_printer.AddLine(SessionName, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            // Account Number
            m_printer.AddLine(AccountNumber.ToString(), StringAlignment.Center, m_fontExtraHuge);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            var temp = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the receipt's footer lines.
        /// </summary>
        protected virtual void PrintFooter()
        {
            // Operator's footer and system date.
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(m_operatorFooterLine1))
                m_printer.AddLine(m_operatorFooterLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine2))
                m_printer.AddLine(m_operatorFooterLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine3))
                m_printer.AddLine(m_operatorFooterLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1) ||
               !string.IsNullOrEmpty(m_operatorFooterLine2) ||
               !string.IsNullOrEmpty(m_operatorFooterLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            m_printer.AddLine(Resources.ReceiptMalfunction1, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(Resources.ReceiptMalfunction2, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // PDTS 584

            var disclaimerFont = !m_useSmallSizes ? m_fontSmall : m_fontTiny;

            if (!string.IsNullOrEmpty(m_disclaimer1))
                m_printer.AddLine(m_disclaimer1, StringAlignment.Center, disclaimerFont);

            if (!string.IsNullOrEmpty(m_disclaimer2))
                m_printer.AddLine(m_disclaimer2, StringAlignment.Center, disclaimerFont);

            if (!string.IsNullOrEmpty(m_disclaimer3))
                m_printer.AddLine(m_disclaimer3, StringAlignment.Center, disclaimerFont);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_disclaimer1) || !string.IsNullOrEmpty(m_disclaimer2) ||
               !string.IsNullOrEmpty(m_disclaimer3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

//            if (!SaleSuccess)
//                m_printer.AddLine(Resources.IncompleteTransactionTitle, StringAlignment.Center, m_fontExtraHuge);

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);
            
            FontEncoder fontEncoder = new FontEncoder();
            var encodedText = fontEncoder.Code128(AccountNumber.ToString());
            m_printer.AddLine(encodedText, StringAlignment.Center, new Font(BarcodeHelper.XSmallFontName, 20, FontStyle.Regular));
            
            m_printer.AddLine(DateTime.Now.ToString(CultureInfo.InvariantCulture), StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public virtual void PrintPreview(Printer printer)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            //Print Body
            PrintBody();

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, short copies)
        {
            PrintPreview(printer);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }
        #endregion

        #region Member Properties

        public string SessionName { get; private set; }

        public int SessionNumber { get; private set; }

        public int ReceiptNumber { get; private set; }

        public int AccountNumber { get; private set; }

        public string HeaderTitle { get; set; }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1
        {
            get
            {
                return m_operatorHeaderLine1;
            }
            set
            {
                m_operatorHeaderLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2
        {
            get
            {
                return m_operatorHeaderLine2;
            }
            set
            {
                m_operatorHeaderLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3
        {
            get
            {
                return m_operatorHeaderLine3;
            }
            set
            {
                m_operatorHeaderLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's first receipt footer line.
        /// </summary>
        public string OperatorFooterLine1
        {
            get
            {
                return m_operatorFooterLine1;
            }
            set
            {
                m_operatorFooterLine1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's second receipt footer line.
        /// </summary>
        public string OperatorFooterLine2
        {
            get
            {
                return m_operatorFooterLine2;
            }
            set
            {
                m_operatorFooterLine2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator's third receipt footer line.
        /// </summary>
        public string OperatorFooterLine3
        {
            get
            {
                return m_operatorFooterLine3;
            }
            set
            {
                m_operatorFooterLine3 = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate
        {
            get
            {
                return m_gamingDate;
            }
            set
            {
                m_gamingDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the receipts transaction date.
        /// </summary>
        public DateTime TransactionDate
        {
            get
            {
                return m_transactionDate;
            }
            set
            {
                m_transactionDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff who made the sale.
        /// </summary>
        public string Cashier
        {
            get
            {
                return m_cashier;
            }
            set
            {
                m_cashier = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device sold to (if applicable).
        /// </summary>
        public string DeviceName
        {
            get
            {
                return m_deviceName;
            }
            set
            {
                m_deviceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the machine that made the sale.
        /// </summary>
        public int SoldFromMachineId
        {
            get
            {
                return m_soldFromMachineId;
            }
            set
            {
                m_soldFromMachineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the first disclaimer line.
        /// </summary>
        public string DisclaimerLine1
        {
            get
            {
                return m_disclaimer1;
            }
            set
            {
                m_disclaimer1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the second disclaimer line.
        /// </summary>
        public string DisclaimerLine2
        {
            get
            {
                return m_disclaimer2;
            }
            set
            {
                m_disclaimer2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the third disclaimer line.
        /// </summary>
        public string DisclaimerLine3
        {
            get
            {
                return m_disclaimer3;
            }
            set
            {
                m_disclaimer3 = value;
            }
        }

        #endregion
    }

    public class B3SalesReceipt : B3Receipt
    {
        #region Constants and Data Types
        protected const int SpacesBetweenCards = 1;

        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalPlayerLength = 16;
        protected const int NormalDeviceFeeLength = 12;
        protected const int NormalTaxesLength = 12;
        protected const int NormalTotalLength = 12;
        protected const int NormalTenderedLength = 12;
        protected const int NormalChangeLength = 12;
        protected const int NormalPtsRedeemedLength = 12;
        protected const int NormalTotalCardsColumns = 3;
        protected const int NormalTotalCardsGameLength = 3;
        protected const int NormalTotalCardsLength = 4;
        protected const int NormalCardNumberColumns = 4;
        protected const int NormalCardNumberGameLength = 3;
        protected const int NormalCardNumberColumn1Length = 9;
        protected const int NormalCardNumberLength = 7;
        protected const int NormalCBBCardNumbersPerLine = 8; // Rally US505, US2228
        protected const int NormalPaperPackInfoColumnLength = 10;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallPlayerLength = 16;
        protected const int SmallDeviceFeeLength = 12;
        protected const int SmallTaxesLength = 12;
        protected const int SmallTotalLength = 12;
        protected const int SmallTenderedLength = 12;
        protected const int SmallChangeLength = 12;
        protected const int SmallPtsRedeemedLength = 12;
        protected const int SmallTotalCardsColumns = 2;
        protected const int SmallTotalCardsGameLength = 3;
        protected const int SmallTotalCardsLength = 4;
        protected const int SmallCardNumberColumns = 3;
        protected const int SmallCardNumberGameLength = 3;
        protected const int SmallCardNumberColumn1Length = 9;
        protected const int SmallCardNumberLength = 7;
        protected const int SmallCBBCardNumbersPerLine = 8; // Rally US505
        protected const int SmallPaperPackInfoColumnLength = 10;
        #endregion

        #region Member Variables
        protected int m_number;
        protected List<string> m_lineItems = new List<string>();
        // Rally TA7464
        #endregion

        public B3SalesReceipt(string sessionName, int sessionNumber, int accountNumber, int receiptNumber, decimal amount)
            : base(sessionName, sessionNumber, accountNumber, receiptNumber)
        {
            Amount = amount;
            HeaderTitle = Resources.B3SaleReceiptTitle;
        }


        #region Member Methods

        /// <summary>
        /// Prints the body.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void PrintBody()
        {
            m_printer.AddLine(SessionName, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            // Account Number
            m_printer.AddLine(AccountNumber.ToString(), StringAlignment.Center, m_fontExtraHuge); // FIX: DE3136

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            var temp = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //credits
            var credits = "   Credits".PadRight(20);
            credits += string.Format("{0:C}", Amount).PadLeft(15);
            m_printer.AddLine(credits, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //grand total
            var total = "   Total".PadRight(20);
            total += string.Format("{0:C}", Amount).PadLeft(15);
            m_printer.AddLine(total, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //tendered
            var tendered = "   Tendered".PadRight(20);
            tendered += string.Format("{0:C}", AmountTendered).PadLeft(15);
            m_printer.AddLine(tendered, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //change
            var change = "   Change".PadRight(20);
            var changeTotal = 0m;
            if (AmountTendered > Amount)
            {
                changeTotal = AmountTendered - Amount;
            }
            change += string.Format("{0:C}", changeTotal).PadLeft(15);
            m_printer.AddLine(change, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);
        }
        
        #endregion

        #region Member Properties

        public decimal Amount { get; private set; }

        /// <summary>
        /// Gets or sets whether this receipt is a reprint.
        /// </summary>
        public bool IsReprint
        {
            get
            {
                return m_isReprint;
            }
            set
            {
                m_isReprint = value;
            }
        }

        // Rally DE1863
        /// <summary>
        /// Gets or sets whether this receipt is a return.
        /// </summary>
        public bool IsReturn
        {
            get
            {
                return m_isReturn;
            }
            set
            {
                m_isReturn = value;
            }
        }


        // Rally US505
        /// <summary>
        /// Gets or sets the id of the sale this receipt is associated with.
        /// </summary>
        public int RegisterReceiptId
        {
            get
            {
                return m_registerReceiptId;
            }
            set
            {
                m_registerReceiptId = value;
            }
        }

        // Rally TA7464
        /// <summary>
        /// Gets or sets the ISO code of the default system currency.
        /// </summary>
        public string DefaultCurrency
        {
            get
            {
                return m_defaultCurrency;
            }
            set
            {
                m_defaultCurrency = value;
            }
        }

        /// <summary>
        /// Gets or sets the ISO code of the sale's currency.
        /// </summary>
        public string SaleCurrency
        {
            get
            {
                return m_saleCurrency;
            }
            set
            {
                m_saleCurrency = value;
            }
        }

        /// <summary>
        /// Gets or sets the exchange rate of the sale.
        /// </summary>
        public decimal ExchangeRate
        {
            get
            {
                return m_exchangeRate;
            }
            set
            {
                m_exchangeRate = value;
            }
        }

        /// <summary>
        /// Gets or sets the grand total of the sale.
        /// </summary>
        public decimal GrandTotal
        {
            get
            {
                return m_grandTotal;
            }
            set
            {
                m_grandTotal = value;
            }
        }

        /// <summary>
        /// Gets or sets the change/amount due for the sale.
        /// </summary>
        public decimal ChangeDue
        {
            get
            {
                return m_changeDue;
            }
            set
            {
                m_changeDue = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount tendered for the sale.
        /// </summary>
        public decimal AmountTendered
        {
            get
            {
                return m_amountTendered;
            }
            set
            {
                m_amountTendered = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to print LOTTO instead of BINGO on cards.
        /// </summary>
        public bool PrintLotto
        {
            get
            {
                return m_printLotto;
            }
            set
            {
                m_printLotto = value;
            }
        }

        #endregion
    }

    public class B3RedeemAccountReceipt : B3Receipt
    {
        //DE13137
        public B3RedeemAccountReceipt(string sessionName, int sessionNumber, int accountNumber, int receiptNumber,decimal winCredits, decimal credits, decimal total, bool isDoubleAccount)
            : base(sessionName, sessionNumber, accountNumber, receiptNumber)
        {
            HeaderTitle = Resources.B3RedeemReceiptTitle;
            Credits = credits;
            WinCredits = winCredits;    //DE13137
            Total = total;              //DE13137
            IsDoubleAcount = isDoubleAccount; //DE13137
        }

        public bool IsDoubleAcount { get; private set; }//DE13137

        public decimal Credits { get; private set; }    

        public decimal WinCredits { get; private set; } //DE13137

        public decimal Total { get; private set; }      //DE13137

        protected override void PrintBody()
        {
            m_printer.AddLine(SessionName, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Account Number
            m_printer.AddLine(AccountNumber.ToString(), StringAlignment.Center, m_fontExtraHuge);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            var dashLine = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);


            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //Credits
            var credits = "   Credits".PadRight(20);
            credits += string.Format("{0:C}", Credits).PadLeft(14);
            m_printer.AddLine(credits, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);


            //DE13137
            if (IsDoubleAcount)
            {
                //Win Credits
                var winCredits = "   Wins".PadRight(20);
                winCredits += string.Format("{0:C}", WinCredits).PadLeft(14);
                m_printer.AddLine(winCredits, StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            //Redeem Total
            var gameName = "   Redeem Total".PadRight(20);
            gameName += string.Format("{0:C}-", Total).PadLeft(15);
            m_printer.AddLine(gameName, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //DE13135: print total in words
            //get string
            var moneyString = string.Format("{0} dollars and {1} cents", 
                IntWordConverter.NumberToWords((int) Total),
                IntWordConverter.NumberToWords((int) (Total%1*100)));
            
            //split by spaces
            var moneyStringSplit = moneyString.Split(' ');
            var line = new StringBuilder();
            
            //iterate and print line when reached max
            foreach (var word in moneyStringSplit)
            {
                if (line.Length + word.Length + 1 > m_fontMediumMaxChars)
                {
                    m_printer.AddLine(line.ToString(), StringAlignment.Center, m_fontMedium);
                    line.Clear();
                }

                //append word and space
                line.Append(word).Append(' ');
            }
            //print last line of money string
            m_printer.AddLine(line.ToString(), StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);
        }

        protected override void PrintFooter()
        {
            //DE13136 Print name and signature line.
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Center, m_fontMedium);
            m_printer.AddLine("Name", StringAlignment.Center, m_fontMedium);

            // Print a signature line.
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Center, m_fontMedium);
            m_printer.AddLine("Signature", StringAlignment.Center, m_fontMedium);

            //barcode
            FontEncoder fontEncoder = new FontEncoder();
            var encodedText = fontEncoder.Code128(AccountNumber.ToString());
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(encodedText, StringAlignment.Center, new Font(BarcodeHelper.XSmallFontName, 20, FontStyle.Regular));

            m_printer.AddLine(DateTime.Now.ToString(CultureInfo.InvariantCulture), StringAlignment.Center, m_fontMedium);
        }
    }

    public class B3OutOfSessionRedeemAccountReceipt : B3Receipt
    {
        //DE13137
        public B3OutOfSessionRedeemAccountReceipt(string sessionName, int sessionSold, int sessionRedeemed, int accountNumber, int receiptNumber, decimal winCredits, decimal credits, decimal total, bool isDoubleAccount)
            : base(sessionName, sessionRedeemed, accountNumber, receiptNumber)
        {
            HeaderTitle = Resources.B3OutOfSessioNRedeemReceiptTitle;
            Credits = credits;
            WinCredits = winCredits;    //DE13137
            Total = total;              //DE13137
            IsDoubleAcount = isDoubleAccount; //DE13137
            SessionSold = sessionSold;
            SessionRedeemed = sessionRedeemed;
        }

        public bool IsDoubleAcount { get; private set; }

        public decimal Credits { get; private set; }

        public decimal WinCredits { get; private set; }

        public decimal Total { get; private set; }

        public int SessionSold { get; private set; }

        public int SessionRedeemed { get; private set; }

        protected override void PrintHeader()
        {
            m_printer.AddLine("Out Of Session", StringAlignment.Center, m_fontBig);
            m_printer.AddLine("Redemption Voucher", StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Receipt Number & Sold From Machine Id
            var temp = Resources.ReceiptNumber.PadRight(m_headerColumn1Length) + ReceiptNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp += Resources.ReceiptSoldFrom.PadRight(m_headerColumn3Length) + m_soldFromMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_soldFromMachineId.ToString(CultureInfo.CurrentCulture);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Gaming Date
            if (m_gamingDate.Year == 1980) //set because we have no open B3 session and don't know the gaming date for the sale session
                temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + "Closed".PadRight(m_headerColumn2Length);
            else
                temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString().PadRight(m_headerColumn2Length);

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            if (m_gamingDate.Year == 1980) //set because we have no open B3 session and don't know the gaming date for the sale session, give them the current date/time for reference
            {
                temp = Resources.ReceiptRedeemed.PadRight(m_headerColumn1Length) + DateTime.Now.ToString("g");
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Cashier (Staff)
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + m_cashier;
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            //account number
            temp = Resources.AccountNumberString.PadRight(m_headerColumn1Length) + AccountNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            var sessionSoldString = string.Format("Session Sold: ").PadLeft(25) + SessionSold;
            var sessionRedemptionString = string.Format("Session Redemption: ").PadLeft(25) + (SessionRedeemed == 0? "Closed" : SessionRedeemed.ToString());
            m_printer.AddLine(sessionSoldString, StringAlignment.Near, m_fontSmall);
            m_printer.AddLine(sessionRedemptionString, StringAlignment.Near, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        protected override void PrintBody()
        {
            m_printer.AddLine(SessionName, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Account Number
            m_printer.AddLine(AccountNumber.ToString(), StringAlignment.Center, m_fontExtraHuge);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            var dashLine = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);


            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //Credits
            var credits = "   Credits".PadRight(20);
            credits += string.Format("{0:C}", Credits).PadLeft(14);
            m_printer.AddLine(credits, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);


            //DE13137
            if (IsDoubleAcount)
            {
                //Win Credits
                var winCredits = "   Wins".PadRight(20);
                winCredits += string.Format("{0:C}", WinCredits).PadLeft(14);
                m_printer.AddLine(winCredits, StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            //Redeem Total
            var gameName = "   Redeem Total".PadRight(20);
            gameName += string.Format("{0:C}-", Total).PadLeft(15);
            m_printer.AddLine(gameName, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //DE13135: print total in words
            //get string
            var moneyString = string.Format("{0} dollars and {1} cents",
                IntWordConverter.NumberToWords((int)Total),
                IntWordConverter.NumberToWords((int)(Total % 1 * 100)));

            //split by spaces
            var moneyStringSplit = moneyString.Split(' ');
            var line = new StringBuilder();

            //iterate and print line when reached max
            foreach (var word in moneyStringSplit)
            {
                if (line.Length + word.Length + 1 > m_fontMediumMaxChars)
                {
                    m_printer.AddLine(line.ToString(), StringAlignment.Center, m_fontMedium);
                    line.Clear();
                }

                //append word and space
                line.Append(word).Append(' ');
            }
            //print last line of money string
            m_printer.AddLine(line.ToString(), StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);
        }

        protected override void PrintFooter()
        {
            //DE13136 Print name and signature line.
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Center, m_fontMedium);
            m_printer.AddLine("Name", StringAlignment.Center, m_fontMedium);

            // Print a signature line.
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Center, m_fontMedium);
            m_printer.AddLine("Signature", StringAlignment.Center, m_fontMedium);

            //barcode
            FontEncoder fontEncoder = new FontEncoder();
            var encodedText = fontEncoder.Code128(AccountNumber.ToString());
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(encodedText, StringAlignment.Center, new Font(BarcodeHelper.XSmallFontName, 20, FontStyle.Regular));

            m_printer.AddLine(DateTime.Now.ToString(CultureInfo.InvariantCulture), StringAlignment.Center, m_fontMedium);
        }
    }

    public class B3UnlockAccountReceipt : B3Receipt
    {
        public B3UnlockAccountReceipt(string sessionName, int sessionNumber, int accountNumber, int receiptNumber, decimal total)
            : base(sessionName, sessionNumber, accountNumber, receiptNumber)
        {
            HeaderTitle = Resources.B3UnlockReceiptTitle;
            CreditTotal = total;
        }

        public decimal CreditTotal { get; private set; }


        /// <summary>
        /// Prints the body.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void PrintBody()
        {
            m_printer.AddLine(SessionName, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            // Account Number
            m_printer.AddLine(AccountNumber.ToString(), StringAlignment.Center, m_fontExtraHuge); // FIX: DE3136

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            var temp = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //credits
            var credits = "   Credits".PadRight(20);
            credits += string.Format("{0:C}", CreditTotal).PadLeft(15);
            m_printer.AddLine(credits, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Near, m_fontMedium);

            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);
        }
        
        //DE13133
        /// <summary>
        /// Prints the receipt's footer lines.
        /// </summary>
        protected override void PrintFooter()
        {
            //DE13133 not valid for cash
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine("NOT VALID FOR CASH", StringAlignment.Center, m_fontBig);

            //barcode
            FontEncoder fontEncoder = new FontEncoder();
            var encodedText = fontEncoder.Code128(AccountNumber.ToString());
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(encodedText, StringAlignment.Center, new Font(BarcodeHelper.XSmallFontName, 20, FontStyle.Regular));

            //date
            m_printer.AddLine(DateTime.Now.ToString(CultureInfo.InvariantCulture), StringAlignment.Center, m_fontMedium);
        }
    }

    public class B3JackpotReceipt : B3Receipt
    {

        public B3JackpotReceipt(string sessionName, int sessionNumber, int accountNumber, decimal jackpotWin, decimal jackpotTrigger, string gameName, int gameNumber, string clientName, string date)
            : base(sessionName, sessionNumber, accountNumber, 0)
        {
            HeaderTitle = Resources.B3JackpotReceiptTitle;

            JackpotWin = jackpotWin;
            GameName = gameName;
            GameNumber = gameNumber;
            ClientName = clientName;
            JackpotTrigger = jackpotTrigger;
            JackpotDateTime = date;
        }

        public decimal JackpotWin { get; private set; }

        public decimal JackpotTrigger { get; private set; }

        public string GameName { get; private set; }

        public int GameNumber { get; private set; }

        public string ClientName { get; private set; }

        public string JackpotDateTime { get; private set; }

        protected override void PrintHeader()
        {
            m_printer.AddLine(HeaderTitle, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Gaming Date
            var temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + m_gamingDate.ToShortDateString().ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Center, m_fontSmall);

            // Machine ID & desc 
            temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + m_soldFromMachineId; //DE13573

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            // Cashier (Staff)
            if (!m_useSmallSizes)
            {
                //DE13572
                temp = Resources.ReceiptStaff.PadRight(m_headerColumn3Length) + m_cashier;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + m_cashier;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            //account number
            temp = Resources.AccountNumberString.PadRight(m_headerColumn1Length) + AccountNumber.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);

            //add session number           
            if (!m_useSmallSizes)
            {
                temp += Resources.SessionTitle.PadRight(m_headerColumn3Length) + SessionNumber;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = Resources.SessionTitle.PadRight(m_headerColumn1Length) + SessionNumber;
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        protected override void PrintBody()
        {
            m_printer.AddLine(SessionName, StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Account Number
            m_printer.AddLine(AccountNumber.ToString(), StringAlignment.Center, m_fontExtraHuge);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine("Jackpot Trigger", StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Format("{0:C}", JackpotTrigger), StringAlignment.Center, m_fontMedium); 

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);


            m_printer.AddLine(string.Format("{0:g}", DateTime.Parse(JackpotDateTime)), StringAlignment.Center, m_fontMedium); 
            var dashLine = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);


            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //jackpotWin
            var jackpotWin = "Jackpot Win".PadLeft(20);
            jackpotWin += string.Format("  {0:C}", JackpotWin).PadRight(15);
            m_printer.AddLine(jackpotWin, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //game Name
            var gameName = "Game Name".PadLeft(20);
            gameName += string.Format("  {0}", GameName).PadRight(15);
            m_printer.AddLine(gameName, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //game Number
            var gameNumber = "Game Number".PadLeft(20);
            gameNumber += string.Format("  {0}", GameNumber).PadRight(15);
            m_printer.AddLine(gameNumber, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //tendered
            var clientName = "Client Name".PadLeft(20);
            clientName += string.Format("  {0}", ClientName).PadRight(15);
            m_printer.AddLine(clientName, StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(dashLine, StringAlignment.Center, m_fontMedium);
        }

        protected override void PrintFooter()
        {
            //space
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //operator footer lines
            if (!string.IsNullOrEmpty(m_operatorFooterLine1))
                m_printer.AddLine(m_operatorFooterLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine2))
                m_printer.AddLine(m_operatorFooterLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(m_operatorFooterLine3))
                m_printer.AddLine(m_operatorFooterLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_operatorFooterLine1) ||
               !string.IsNullOrEmpty(m_operatorFooterLine2) ||
               !string.IsNullOrEmpty(m_operatorFooterLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

            //Malfunction disclaimer
            m_printer.AddLine(Resources.ReceiptMalfunction1, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(Resources.ReceiptMalfunction2, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);


            var disclaimerFont = !m_useSmallSizes ? m_fontSmall : m_fontTiny;

            if (!string.IsNullOrEmpty(m_disclaimer1))
                m_printer.AddLine(m_disclaimer1, StringAlignment.Center, disclaimerFont);

            if (!string.IsNullOrEmpty(m_disclaimer2))
                m_printer.AddLine(m_disclaimer2, StringAlignment.Center, disclaimerFont);

            if (!string.IsNullOrEmpty(m_disclaimer3))
                m_printer.AddLine(m_disclaimer3, StringAlignment.Center, disclaimerFont);

            // Add some spaces.
            if (!string.IsNullOrEmpty(m_disclaimer1) || !string.IsNullOrEmpty(m_disclaimer2) ||
               !string.IsNullOrEmpty(m_disclaimer3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }

//            if (!SaleSuccess)
//                m_printer.AddLine(Resources.IncompleteTransactionTitle, StringAlignment.Center, m_fontExtraHuge);

            if (m_isReprint)
                m_printer.AddLine(Resources.ReceiptReprint, StringAlignment.Center, m_fontBig);

            m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
            m_printer.AddLine("EMPLOYEE", StringAlignment.Center, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
            m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
            m_printer.AddLine("SUPERVISOR", StringAlignment.Center, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(DateTime.Now.ToString(CultureInfo.InvariantCulture), StringAlignment.Center, m_fontMedium);
        }
    }

    //US4698: POS: Denomination receipt
    public class BankDenominationsReceipt : Receipt
    {
        #region Constants and Data Types
        protected const int SpacesBetweenCards = 1;

        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalPlayerLength = 16;
        protected const int NormalDeviceFeeLength = 12;
        protected const int NormalTaxesLength = 12;
        protected const int NormalTotalLength = 12;
        protected const int NormalTenderedLength = 12;
        protected const int NormalChangeLength = 12;
        protected const int NormalPtsRedeemedLength = 12;
        protected const int NormalTotalCardsColumns = 3;
        protected const int NormalTotalCardsGameLength = 3;
        protected const int NormalTotalCardsLength = 4;
        protected const int NormalCardNumberColumns = 4;
        protected const int NormalCardNumberGameLength = 3;
        protected const int NormalCardNumberColumn1Length = 9;
        protected const int NormalCardNumberLength = 7;
        protected const int NormalCbbCardNumbersPerLine = 8; // Rally US505, US2228
        protected const int NormalPaperPackInfoColumnLength = 10;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallPlayerLength = 16;
        protected const int SmallDeviceFeeLength = 12;
        protected const int SmallTaxesLength = 12;
        protected const int SmallTotalLength = 12;
        protected const int SmallTenderedLength = 12;
        protected const int SmallChangeLength = 12;
        protected const int SmallPtsRedeemedLength = 12;
        protected const int SmallTotalCardsColumns = 2;
        protected const int SmallTotalCardsGameLength = 3;
        protected const int SmallTotalCardsLength = 4;
        protected const int SmallCardNumberColumns = 3;
        protected const int SmallCardNumberGameLength = 3;
        protected const int SmallCardNumberColumn1Length = 9;
        protected const int SmallCardNumberLength = 7;
        protected const int SmallCBBCardNumbersPerLine = 8; // Rally US505
        protected const int SmallPaperPackInfoColumnLength = 10;
        #endregion

        #region Member Variables
        private int m_headerColumn1Length;
        private int m_headerColumn2Length;
        private int m_headerColumn3Length;
        private int m_headerMachineDescLength = 17;
        private int m_playerLength;
        private int m_deviceFeeLength;
        private int m_taxesLength;
        private int m_totalLength;
        private int m_tenderedLength;
        private int m_changeLength;
        private int m_ptsRedeemedLength;
        private int m_totalCardsColumns;
        private int m_totalCardsGameLength;
        private int m_totalCardsLength;
        private int m_cardNumberColumns;
        private int m_cardNumberGameLength;
        private int m_cardNumberColumn1Length;
        private int m_paperPackInfoColumnLength;
        private int m_cardNumberLength;
        private int m_cbbCardNumbersPerLine; // Rally US505

        private bool m_isReprint;
        private bool m_isReturn; // Rally DE1863 - Add the wording "Return" on a receipt printed when in return mode.

        #endregion

        #region Constructor
        public BankDenominationsReceipt()
        {
            Denominations = new List<Denomination>();
            StaffName = string.Empty;
            SoldFromMachineId = 0;
            Session = 0;
        }
        #endregion
        
        #region Methods
        //DE12995
        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
                m_headerColumn3Length = SmallHeaderColumn3Length;
                m_playerLength = SmallPlayerLength;
                m_deviceFeeLength = SmallDeviceFeeLength;
                m_taxesLength = SmallTaxesLength;
                m_totalLength = SmallTotalLength;
                m_tenderedLength = SmallTenderedLength;
                m_changeLength = SmallChangeLength;
                m_ptsRedeemedLength = SmallPtsRedeemedLength;
                m_totalCardsColumns = SmallTotalCardsColumns;
                m_totalCardsGameLength = SmallTotalCardsGameLength;
                m_totalCardsLength = SmallTotalCardsLength;
                m_cardNumberColumns = SmallCardNumberColumns;
                m_cardNumberGameLength = SmallCardNumberGameLength;
                m_cardNumberColumn1Length = SmallCardNumberColumn1Length;
                m_cardNumberLength = SmallCardNumberLength;
                m_cbbCardNumbersPerLine = SmallCBBCardNumbersPerLine; // Rally US505
                m_paperPackInfoColumnLength = SmallPaperPackInfoColumnLength;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
                m_headerColumn3Length = NormalHeaderColumn3Length;
                m_playerLength = NormalPlayerLength;
                m_deviceFeeLength = NormalDeviceFeeLength;
                m_taxesLength = NormalTaxesLength;
                m_totalLength = NormalTotalLength;
                m_tenderedLength = NormalTenderedLength;
                m_changeLength = NormalChangeLength;
                m_ptsRedeemedLength = NormalPtsRedeemedLength;
                m_totalCardsColumns = NormalTotalCardsColumns;
                m_totalCardsGameLength = NormalTotalCardsGameLength;
                m_totalCardsLength = NormalTotalCardsLength;
                m_cardNumberColumns = NormalCardNumberColumns;
                m_cardNumberGameLength = NormalCardNumberGameLength;
                m_cardNumberColumn1Length = NormalCardNumberColumn1Length;
                m_cardNumberLength = NormalCardNumberLength;
                m_paperPackInfoColumnLength = NormalPaperPackInfoColumnLength;
            }
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(OperatorHeaderLine1))
                m_printer.AddLine(OperatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorHeaderLine2))
                m_printer.AddLine(OperatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorHeaderLine3))
                m_printer.AddLine(OperatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(OperatorHeaderLine1) ||
               !string.IsNullOrEmpty(OperatorHeaderLine2) ||
               !string.IsNullOrEmpty(OperatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected virtual void PrintHeader()
        {
            m_printer.AddLine("Bank Close Receipt", StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Receipt Number & Sold From Machine Id
            var temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + SoldFromMachineId.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            // Gaming Date
            temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + GamingDate.ToShortDateString();

            if (!m_useSmallSizes)
            {
                temp = temp.PadRight(m_headerColumn1Length + m_headerColumn2Length) + MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }
            else
            {
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
                temp = MachineDesc.PadRight(m_headerMachineDescLength).Substring(0, m_headerMachineDescLength);
                m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);
            }

            //Cashier
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + StaffName.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            //Session
            temp = "Session:".PadRight(m_headerColumn1Length) + Session.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the body.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void PrintBody()
        {
            var total = 0.0m;

            var temp = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);

            //Currency Code
            m_printer.AddLine(CurrencyCode.PadRight(m_headerColumn1Length), StringAlignment.Near, m_fontSmall);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //credits
            foreach (var denom in Denominations)
            {
                //DE12995
                var nameDefaultColumnWidth = 13;
                var countDefaultColumnWidth = 3;
                var countLength = denom.Count.ToString().Length;
                var name = denom.Name;

                //if name length 
                if (denom.Name.Length > nameDefaultColumnWidth - countLength + 1)
                {
                    name = denom.Name.Substring(0, nameDefaultColumnWidth - countLength);
                    name += ".";
                }

                var line = name.PadRight(name.Length + 1);
                line += string.Format("{0}x", denom.Count).PadLeft(nameDefaultColumnWidth + countDefaultColumnWidth - (name.Length + 1));
                line += string.Format("{0}", denom.Value.ToString("F")).PadLeft(8);
                line += string.Format("{0}", (denom.Value * denom.Count).ToString("F")).PadLeft(10);

                //update total
                total += denom.Value * denom.Count;

                //print denom
                m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);
            }

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //US4847
            //grand total
            var totalLine = "Total Drop: ".PadLeft(21);
            totalLine += string.Format("{0:C}", total).PadLeft(13);
            m_printer.AddLine(totalLine, StringAlignment.Near, m_fontMedium);

            //US4978
            // total paper sales
            if (TotalPaperSalesDue != 0)
            {
                totalLine = "Paper Sales: ".PadLeft(21);
                totalLine += string.Format("{0:C}", TotalPaperSalesDue).PadLeft(13);
                m_printer.AddLine(totalLine, StringAlignment.Near, m_fontMedium);
            }

            // US5024
            // total paper usage
            if (TotalPaperUsageDue != 0)
            {
                totalLine = "Paper Usage: ".PadLeft(21);
                totalLine += string.Format("{0:C}", TotalPaperUsageDue).PadLeft(13);
                m_printer.AddLine(totalLine, StringAlignment.Near, m_fontMedium);
            }

            //US4847
            totalLine = "Total Due: ".PadLeft(21);
            totalLine += string.Format("{0:C}", TotalDue).PadLeft(13);
            m_printer.AddLine(totalLine, StringAlignment.Near, m_fontMedium);

            //US4847
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1; 
            totalLine = "Over/Short: ".PadLeft(21);
            totalLine += string.Format(culture, "{0:C}", total - TotalDue).PadLeft(13);
            m_printer.AddLine(totalLine, StringAlignment.Near, m_fontMedium);


            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the receipt's footer lines.
        /// </summary>
        protected virtual void PrintFooter()
        {
            PrintSignatureLines();

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(DateTime.Now.ToString(CultureInfo.InvariantCulture), StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public virtual void PrintPreview(Printer printer)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            //Print Body
            PrintBody();

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, short copies)
        {
            PrintPreview(printer);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }

        //DE13632
        /// <summary>
        /// Prints the signature lines.
        /// </summary>
        public void PrintSignatureLines()
        {
            for (int i = 0; i < BankCloseSignatureLineCount; i++)
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine("X", StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Empty.PadRight(m_fontMediumMaxChars, '_'), StringAlignment.Near, m_fontMedium);
                m_printer.AddLine(string.Format("Signature {0}", i + 1), StringAlignment.Center, m_fontMedium);
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the id of the machine that made the sale.
        /// </summary>
        public int SoldFromMachineId { get; set; }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public short Session { get; set; }

        //US4847
        /// <summary>
        /// Gets or sets the total due.
        /// </summary>
        /// <value>
        /// The total due.
        /// </value>
        public decimal TotalDue { get; set; }

        /// US4978
        /// <summary>
        /// Gets or sets the total paper sales value that's included in the TotalDue
        /// </summary>
        public decimal TotalPaperSalesDue { get; set; }

        /// US4978
        /// <summary>
        /// Gets or sets the total paper usage value that is not included in the TotalDue
        /// </summary>
        public decimal TotalPaperUsageDue { get; set; }

        /// <summary>
        /// Gets or sets the name of the staff.
        /// </summary>
        /// <value>
        /// The name of the staff.
        /// </value>
        public string StaffName { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1 { get; set; }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2 { get; set; }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3 { get; set; }

        /// <summary>
        /// Gets or sets the denominations.
        /// </summary>
        /// <value>
        /// The denominations.
        /// </value>
        public List<Denomination> Denominations { get; set; }

        /// <summary>
        /// Gets or sets the bank close signature line count.
        /// </summary>
        /// <value>
        /// The bank close signature line count.
        /// </value>
        public int BankCloseSignatureLineCount { get; set; }

        #endregion
    }

    public class PaperExchangeReceipt : Receipt
    {
        #region Constants and Data Types
        protected const int SpacesBetweenCards = 1;

        // Text Lengths - 80mm+
        protected const int NormalHeaderColumn1Length = 13;
        protected const int NormalHeaderColumn2Length = 11;
        protected const int NormalHeaderColumn3Length = 9;
        protected const int NormalPlayerLength = 16;
        protected const int NormalDeviceFeeLength = 12;
        protected const int NormalTaxesLength = 12;
        protected const int NormalTotalLength = 12;
        protected const int NormalTenderedLength = 12;
        protected const int NormalChangeLength = 12;
        protected const int NormalPtsRedeemedLength = 12;
        protected const int NormalTotalCardsColumns = 3;
        protected const int NormalTotalCardsGameLength = 3;
        protected const int NormalTotalCardsLength = 4;
        protected const int NormalCardNumberColumns = 4;
        protected const int NormalCardNumberGameLength = 3;
        protected const int NormalCardNumberColumn1Length = 9;
        protected const int NormalCardNumberLength = 7;
        protected const int NormalCbbCardNumbersPerLine = 8; // Rally US505, US2228
        protected const int NormalPaperPackInfoColumnLength = 10;

        // Text Lengths - 58mm
        protected const int SmallHeaderColumn1Length = 13;
        protected const int SmallHeaderColumn2Length = 11;
        protected const int SmallHeaderColumn3Length = 9;
        protected const int SmallPlayerLength = 16;
        protected const int SmallDeviceFeeLength = 12;
        protected const int SmallTaxesLength = 12;
        protected const int SmallTotalLength = 12;
        protected const int SmallTenderedLength = 12;
        protected const int SmallChangeLength = 12;
        protected const int SmallPtsRedeemedLength = 12;
        protected const int SmallTotalCardsColumns = 2;
        protected const int SmallTotalCardsGameLength = 3;
        protected const int SmallTotalCardsLength = 4;
        protected const int SmallCardNumberColumns = 3;
        protected const int SmallCardNumberGameLength = 3;
        protected const int SmallCardNumberColumn1Length = 9;
        protected const int SmallCardNumberLength = 7;
        protected const int SmallPaperPackInfoColumnLength = 10;
        #endregion

        #region Member Variables
        private int m_headerColumn1Length;
        private int m_headerColumn2Length;

        private readonly PaperExchangeItem m_returnedItem;
        private readonly PaperExchangeItem m_exchangedItem;
        #endregion

        #region Constructor
        public PaperExchangeReceipt(PaperExchangeItem returnedItem, PaperExchangeItem exchangedItem)
        {
            m_returnedItem = returnedItem;
            m_exchangedItem = exchangedItem;
        }
        #endregion
        
        #region Methods
        //DE12995
        /// <summary>
        /// Sets the fonts and text sizes for this receipt.
        /// </summary>
        /// <param name="useSmallSizes">true to use smaller sizes; 
        /// otherwise false</param>
        protected override void SetTextSizes(bool useSmallSizes)
        {
            base.SetTextSizes(useSmallSizes);

            if (m_useSmallSizes)
            {
                m_headerColumn1Length = SmallHeaderColumn1Length;
                m_headerColumn2Length = SmallHeaderColumn2Length;
            }
            else
            {
                m_headerColumn1Length = NormalHeaderColumn1Length;
                m_headerColumn2Length = NormalHeaderColumn2Length;
            }
        }

        protected void PrintOperatorInfo()
        {
            if (!string.IsNullOrEmpty(OperatorName))
                m_printer.AddLine(OperatorName, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorAddress1))
                m_printer.AddLine(OperatorAddress1, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorAddress2))
                m_printer.AddLine(OperatorAddress2, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorCityStateZip))
                m_printer.AddLine(OperatorCityStateZip, StringAlignment.Center, m_fontMedium);

            if (!string.IsNullOrEmpty(OperatorPhoneNumber))
                m_printer.AddLine(OperatorPhoneNumber, StringAlignment.Center, m_fontMedium);

            // If anything was printed, add another line
            if (!string.IsNullOrEmpty(OperatorName) ||
               !string.IsNullOrEmpty(OperatorAddress1) ||
               !string.IsNullOrEmpty(OperatorAddress2) ||
               !string.IsNullOrEmpty(OperatorCityStateZip) ||
               !string.IsNullOrEmpty(OperatorPhoneNumber))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the operator's header lines.
        /// </summary>
        protected void PrintOperatorLines()
        {
            if (!string.IsNullOrEmpty(OperatorHeaderLine1))
                m_printer.AddLine(OperatorHeaderLine1, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorHeaderLine2))
                m_printer.AddLine(OperatorHeaderLine2, StringAlignment.Center, m_fontBig);

            if (!string.IsNullOrEmpty(OperatorHeaderLine3))
                m_printer.AddLine(OperatorHeaderLine3, StringAlignment.Center, m_fontBig);

            // Add some spaces.
            if (!string.IsNullOrEmpty(OperatorHeaderLine1) ||
               !string.IsNullOrEmpty(OperatorHeaderLine2) ||
               !string.IsNullOrEmpty(OperatorHeaderLine3))
            {
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
                m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            }
        }

        /// <summary>
        /// Prints the receipt header.
        /// </summary>
        protected virtual void PrintHeader()
        {
            m_printer.AddLine("Paper Exchange Receipt", StringAlignment.Center, m_fontBig);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            // Receipt Number & Sold From Machine Id
            var temp = Resources.ReceiptSoldFrom.PadRight(m_headerColumn1Length) + SoldFromMachineId.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            // Gaming Date
            temp = Resources.ReceiptGamingDate.PadRight(m_headerColumn1Length) + GamingDate.ToShortDateString().PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            //Cashier
            temp = Resources.ReceiptStaff.PadRight(m_headerColumn1Length) + StaffName.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            //Session
            temp = "Session:".PadRight(m_headerColumn1Length) + Session.ToString(CultureInfo.CurrentCulture).PadRight(m_headerColumn2Length);
            m_printer.AddLine(temp, StringAlignment.Near, m_fontSmall);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the body.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void PrintBody()
        {
            var temp = string.Empty.PadRight(m_fontMediumMaxChars, '-');
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            
            var returnItemString = "Returned Item:";
            var exchangeItemString = "Exchanged Item:";
            var serialItemHeader = "Serial:";
            var auditItemHeader = "Audit:";
            var receiptNumberHeader = "Receipt Num:";

            var nameDefaultColumnWidth = returnItemString.Length; // find the longest header to line up everything
            if (exchangeItemString.Length > nameDefaultColumnWidth)
                nameDefaultColumnWidth = exchangeItemString.Length;
            if (serialItemHeader.Length > nameDefaultColumnWidth)
                nameDefaultColumnWidth = serialItemHeader.Length;
            if (auditItemHeader.Length > nameDefaultColumnWidth)
                nameDefaultColumnWidth = auditItemHeader.Length;
            if (receiptNumberHeader.Length > nameDefaultColumnWidth)
                nameDefaultColumnWidth = receiptNumberHeader.Length;

            //Returned Item
            var line = returnItemString.PadLeft(nameDefaultColumnWidth);

            line += string.Format("{0}", m_returnedItem.Name).PadLeft(SmallFontMediumMaxChars - nameDefaultColumnWidth);
            m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);

            line = serialItemHeader.PadLeft(nameDefaultColumnWidth);
            line += string.Format("{0}", m_returnedItem.Serial).PadLeft(SmallFontMediumMaxChars - nameDefaultColumnWidth);
            m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);

            line = auditItemHeader.PadLeft(nameDefaultColumnWidth);
            line += string.Format("{0}", m_returnedItem.Audit).PadLeft(SmallFontMediumMaxChars - nameDefaultColumnWidth);
            m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);

            line = receiptNumberHeader.PadLeft(nameDefaultColumnWidth);
            line += string.Format("{0}", m_returnedItem.TransactionNumber).PadLeft(SmallFontMediumMaxChars - nameDefaultColumnWidth);
            m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);

            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);

            //Exchange Item

            line = exchangeItemString.PadLeft(nameDefaultColumnWidth);

            line += string.Format("{0}", m_exchangedItem.Name).PadLeft(SmallFontMediumMaxChars - nameDefaultColumnWidth);
            m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);

            line = serialItemHeader.PadLeft(nameDefaultColumnWidth);
            line += string.Format("{0}", m_exchangedItem.Serial).PadLeft(SmallFontMediumMaxChars - nameDefaultColumnWidth);
            m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);

            line = auditItemHeader.PadLeft(nameDefaultColumnWidth);
            line += string.Format("{0}", m_exchangedItem.Audit).PadLeft(SmallFontMediumMaxChars - nameDefaultColumnWidth);
            m_printer.AddLine(line, StringAlignment.Near, m_fontMedium);
            
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(temp, StringAlignment.Center, m_fontMedium);


            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
            m_printer.AddLine(string.Empty, StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prints the receipt's footer lines.
        /// </summary>
        protected virtual void PrintFooter()
        {
            m_printer.AddLine(DateTime.Now.ToString(CultureInfo.InvariantCulture), StringAlignment.Center, m_fontMedium);
        }

        /// <summary>
        /// Prepares the receipt to be previewed for the specified printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public virtual void PrintPreview(Printer printer)
        {
            if (printer == null)
                throw new ArgumentNullException("printer");

            m_printer = printer;
            m_printer.Margins = new Margins((int)m_printer.HardMarginX, (int)m_printer.HardMarginX, (int)m_printer.HardMarginY, (int)m_printer.HardMarginY);

            SetTextSizes(m_printer.Using58mmPaper);

            // Clear the the current lines from the printer object.
            m_printer.ClearLines();

            //Print operator's name, address, and phone number
            PrintOperatorInfo();

            // Print the operator's receipt lines.
            PrintOperatorLines();

            // Print the receipt's header.
            PrintHeader();

            //Print Body
            PrintBody();

            // Print the footer.
            PrintFooter();
        }

        /// <summary>
        /// Prints the receipt to the printer.
        /// </summary>
        /// <param name="printer">The printer to receipt will 
        /// be printed to.</param>
        /// <param name="copies">The number of copies to print out.</param>
        /// <exception cref="System.ArgumentNullException">printer is a null 
        /// reference.</exception>
        public void Print(Printer printer, short copies)
        {
            PrintPreview(printer);

            for (int x = 0; x < copies; x++)
            {
                m_printer.Print();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the id of the machine that made the sale.
        /// </summary>
        public int SoldFromMachineId { get; set; }

        /// <summary>
        /// Gets or sets the receipt's gaming date.
        /// </summary>
        public DateTime GamingDate { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public short Session { get; set; }

        //US4847
        /// <summary>
        /// Gets or sets the total due.
        /// </summary>
        /// <value>
        /// The total due.
        /// </value>
        public decimal TotalDue { get; set; }

        /// <summary>
        /// Gets or sets the name of the staff.
        /// </summary>
        /// <value>
        /// The name of the staff.
        /// </value>
        public string StaffName { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets/sets the operator's name
        /// </summary>
        public string OperatorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address1
        /// </summary>
        public string OperatorAddress1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's address2
        /// </summary>
        public string OperatorAddress2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's city, state, zip
        /// </summary>
        public string OperatorCityStateZip
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the operator's phone number
        /// </summary>
        public string OperatorPhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator's first receipt line.
        /// </summary>
        public string OperatorHeaderLine1 { get; set; }

        /// <summary>
        /// Gets or sets the operator's second receipt line.
        /// </summary>
        public string OperatorHeaderLine2 { get; set; }

        /// <summary>
        /// Gets or sets the operator's third receipt line.
        /// </summary>
        public string OperatorHeaderLine3 { get; set; }

        #endregion
    }
}