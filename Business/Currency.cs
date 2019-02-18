#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2010 GameTech
// International, Inc.
#endregion

// Rally TA7465 - Support sale currencies at the POS.

using System;
using System.Collections.Generic;
using System.Globalization;
using GTI.Modules.Shared.Properties;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents how customers pay for products or are awarded prizes in the
    /// system.
    /// </summary>
    public class Currency : IEquatable<Currency>, IComparable<Currency>, IComparable
    {
        #region Member variables
        protected int m_precision = 2;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Currency class.
        /// </summary>
        /// <param name="isoCurrencyCode">The three-character ISO 4217 currency
        /// code.</param>
        /// <exception cref="System.ArgumentException">isoCurrencyCode is
        /// invalid.</exception>
        public Currency(string isoCurrencyCode)
        {
            SetRegionByISO(isoCurrencyCode);

            Denominations = new List<Denomination>();
            IsActive = true;
        }

        /// <summary>
        /// Initializes a new instance of the Currency class.
        /// </summary>
        /// <param name="currency">The currency to copy.</param>
        public Currency(Currency currency)
        {
            RegionInfo = currency.RegionInfo;
            ExchangeRate = currency.ExchangeRate;
            IsDefault = currency.IsDefault;
            IsActive = currency.IsActive;

            Denominations = new List<Denomination>();

            foreach(Denomination denom in currency.Denominations)
            {
                Denomination newDenom = new Denomination(denom);
                AddDenomination(newDenom);
            }
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Finds the first RegionInfo for the specified currency code.
        /// </summary>
        /// <param name="isoCurrencyCode">The three-character ISO 4217 currency
        /// code.</param>
        /// <exception cref="System.ArgumentException">isoCurrencyCode is
        /// invalid.</exception>
        private void SetRegionByISO(string isoCurrencyCode)
        {
            if(string.IsNullOrEmpty(isoCurrencyCode))
                throw new ArgumentException("isoCurrencyCode");

            RegionInfo = null;

            try
            {
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                foreach(CultureInfo info in cultures)
                {
                    RegionInfo region = new RegionInfo(info.LCID);

                    if(region.ISOCurrencySymbol == isoCurrencyCode)
                    {
                        RegionInfo = region;
                        break;
                    }
                }
            }
            catch
            {
            }

            if(RegionInfo == null) // We didn't find the currency.
                throw new ArgumentException("isoCurrencyCode");
        }

        /// <summary>
        /// Adds a Denomination to the currency's list.
        /// </summary>
        /// <param name="denom">The denomination to add.</param>
        public void AddDenomination(Denomination denom)
        {
            if(!Denominations.Contains(denom))
                Denominations.Add(denom);
        }

        /// <summary>
        /// Removes all denominations from the currency.
        /// </summary>
        public void ClearDenominations()
        {
            Denominations.Clear();
        }

        /// <summary>
        /// Returns a string that represents the current Currency.
        /// </summary>
        /// <returns>A string that represents the current 
        /// Currency.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Determines whether two Currency instances are equal. 
        /// </summary>
        /// <param name="obj">The Currency to compare with the 
        /// current Currency.</param>
        /// <returns>true if the specified Currency is equal to the current 
        /// Currency; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Currency currency = obj as Currency;

            if(currency == null)
                return false;
            else
                return Equals(currency);
        }

        /// <summary>
        /// Determines whether two Currency instances are equal. 
        /// </summary>
        /// <param name="other">The Currency to compare with the 
        /// current Currency.</param>
        /// <returns>true if the specified Currency is equal to the current 
        /// Currency; otherwise, false.</returns>
        public bool Equals(Currency other)
        {
            bool equal = false;

            // Are all the members equal?
            equal = (other != null &&
                     RegionInfo.ISOCurrencySymbol == other.RegionInfo.ISOCurrencySymbol &&
                     ExchangeRate == other.ExchangeRate &&
                     IsDefault == other.IsDefault &&
                     IsActive == other.IsActive);

            // Do we have the same number of denoms?
            if(Denominations.Count != other.Denominations.Count)
                equal = false;

            // Check all the denoms.
            if(equal)
            {
                foreach(Denomination denom in Denominations)
                {
                    if(!other.Denominations.Contains(denom))
                    {
                        equal = false;
                        break;
                    }
                }
            }

            return equal;
        }

        /// <summary>
        /// Serves as a hash function for a Currency. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Currency.</returns>
        public override int GetHashCode()
        {
            int hash = (RegionInfo.ISOCurrencySymbol.GetHashCode() ^
                        ExchangeRate.GetHashCode() ^
                        IsDefault.GetHashCode() ^
                        IsActive.GetHashCode());

            foreach(Denomination denom in Denominations)
            {
                hash ^= denom.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Compares the current Currency with another Currency.
        /// </summary>
        /// <param name="other">A Currency to compare with this
        /// Currency.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order 
        /// of the objects being compared. The return value has the following 
        /// meanings: Less than zero - This object is less than the other 
        /// parameter. Zero - This object is equal to other. Greater than 
        /// zero - This object is greater than other.</returns>
        public int CompareTo(Currency other)
        {
            if(other == null)
                return 1;
            else
                return ISOCode.CompareTo(other.ISOCode);
        }

        /// <summary>
        /// Compares the current Currency with another Currency.
        /// </summary>
        /// <param name="obj">A Currency to compare with this
        /// Currency.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order 
        /// of the objects being compared. The return value has the following 
        /// meanings: Less than zero - This object is less than the other 
        /// parameter. Zero - This object is equal to other. Greater than 
        /// zero - This object is greater than other.</returns>
        /// <exception cref="System.ArgumentException">obj is not a 
        /// Currency.</exception>
        public int CompareTo(object obj)
        {
            Currency currency = obj as Currency;

            if(currency == null)
                throw new ArgumentException(Resources.NotAClass + "Currency");
            else
                return CompareTo(currency);
        }

        /// <summary>
        /// Formats a decimal using the standard "C" format, but uses this
        /// Currency's symbol.
        /// </summary>
        /// <param name="amount">The amount to be formatted.</param>
        /// <returns>The formatted string with this Currency's
        /// symbol.</returns>
        public string FormatCurrencyString(decimal amount)
        {
            // Get the current culture in order to format the currency string.
            NumberFormatInfo info = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();

            // Change the currency symbol to this currency's symbol.
            info.CurrencySymbol = Symbol;

            return amount.ToString("C", info);
        }

        /// <summary>
        /// Converts an amount of this currency to the default currency.
        /// </summary>
        /// <param name="amount">Amount in this currency to convert.</param>
        /// <returns>Amount in the default currency.</returns>
        public decimal ConvertFromThisCurrencyToDefaultCurrency(decimal amount)
        {
            decimal decimalShift = (decimal)Math.Pow(10, Precision);

            // The amount is always in the favor of the hall, so round down to the number of decimal places given in Precision.
            return (decimal)Math.Sign(amount)*decimal.Floor(Math.Abs(amount) / ExchangeRate * decimalShift) / decimalShift;
        }

        /// <summary>
        /// Converts an amount in the default currency to this currency.
        /// </summary>
        /// <param name="amount">Amount in default currency to convert.</param>
        /// <returns>Amount in this currency.</returns>
        public decimal ConvertFromDefaultCurrencyToThisCurrency(decimal amount)
        {
            decimal smallestAmount = SmallestAmountForThisCurrency;
            decimal sign = (decimal)Math.Sign(amount);
            decimal defaultAmount = Math.Abs(amount);
            decimal thisCurrencyGuess = sign * decimal.Round(defaultAmount * ExchangeRate, Precision, MidpointRounding.AwayFromZero);

            while (ConvertFromThisCurrencyToDefaultCurrency(thisCurrencyGuess) != defaultAmount)
                thisCurrencyGuess += smallestAmount;

            return sign * thisCurrencyGuess;

            

//            return (decimal)Math.Sign(amount) * decimal.Round(Math.Abs(amount) * ExchangeRate, Precision, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Static Methods
        /// <summary>
        /// Formats a decimal using the standard "C" format, but uses specified
        /// currency's symbol.
        /// </summary>
        /// <param name="isoCurrencyCode">The three-character ISO 4217 currency
        /// code.</param>
        /// <param name="amount">The amount to be formatted.</param>
        /// <returns>The formatted string with this Currency's
        /// symbol.</returns>
        /// <exception cref="System.ArgumentException">isoCurrencyCode is
        /// invalid.</exception>
        public static string FormatCurrencyString(string isoCurrencyCode, decimal amount)
        {
            Currency currency = new Currency(isoCurrencyCode);

            return currency.FormatCurrencyString(amount);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the RegionInfo that holds the data about the currency.
        /// </summary>
        private RegionInfo RegionInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the English name of the currency.
        /// </summary>
        public string Name
        {
            get
            {
                return RegionInfo.CurrencyEnglishName;
            }
        }

        /// <summary>
        /// Gets the three-character ISO 4217 currency code.
        /// </summary>
        public string ISOCode
        {
            get
            {
                return RegionInfo.ISOCurrencySymbol;
            }
        }

        /// <summary>
        /// Gets the currency symbol.
        /// </summary>
        public string Symbol
        {
            get
            {
                return RegionInfo.CurrencySymbol;
            }
        }

        /// <summary>
        /// Gets or sets this currency's exchange rate versus the system's
        /// default currency.
        /// </summary>
        public decimal ExchangeRate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of denominations the represent this currency.
        /// </summary>
        public IList<Denomination> Denominations
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets whether this is the system's default currency.
        /// </summary>
        public bool IsDefault
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this currency is active.
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of decimal places to use when converting 
        /// from one currency to another. Default = 2.
        /// </summary>
        public int Precision
        {
            get
            {
                return m_precision;
            }

            set
            {
                m_precision = value;
            }
        }

        /// <summary>
        /// Returns the number to divide the integral representation of
        /// the currency by to get the fractional representation.
        /// IE: decimal dollars = (decimal)pennies / IntegerToDecimalDivisor; 
        /// </summary>
        public decimal IntegerToDecimalDivisor
        {
            get
            {
                return (decimal)Math.Pow(10, Precision);
            }
        }

        /// <summary>
        /// Returns the smallest amount for this currency based on the set precision.
        /// </summary>
        public decimal SmallestAmountForThisCurrency
        {
            get
            {
                return 1M / (decimal)Math.Pow(10, Precision);
            }
        }

        #endregion
    }
}