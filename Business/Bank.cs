#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2010 GameTech
// International, Inc.
#endregion

// Rally TA7465
//US4436: Close a bank from the POS

using System;
using GTI.Modules.Shared;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Enumerates the types of banks.
    /// </summary>
    public enum BankType
    {
        All = 0,
        Master = 1,
        Regular = 2
    }

    public enum BankOpenType
    {
        B3Only = -2,
        None = -1,
        Open = 0,
        New = 1,
        ReOpen= 2

    }

    /// <summary>
    /// Represents a staff's cash drawer.
    /// </summary>
    public class Bank
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Bank class.
        /// </summary>
        public Bank()
        {
            Currencies = new List<BankCurrency>();
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Sorts the currencies in the Currencies list by ISO code.
        /// </summary>
        public void Sort()
        {
            ((List<BankCurrency>)Currencies).Sort();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, Session);
        }

        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public BankType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the staff who owns this bank (or 0 if the
        /// bank is owned by a machine).
        /// </summary>
        public int StaffId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of the machine who owns this bank (or 0 if the
        /// bank is owned by a staff).
        /// </summary>
        public int MachineId
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the gaming session of the bank
        /// </summary>
        public int Session { get; set; }

        /// <summary>
        /// Gets or sets the gaming date of the bank
        /// </summary>
        public DateTime GamingDate { get; set; }

        /// <summary>
        /// Gets a list of currency totals in this bank.
        /// </summary>
        public IList<BankCurrency> Currencies
        {
            get;
            private set;
        }

        #endregion
    }

    /// <summary>
    /// Represents a currency total for a bank
    /// </summary>
    public class BankCurrency : Currency
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BankCurrency class.
        /// </summary>
        /// <param name="isoCurrencyCode">The three-character ISO 4217 currency
        /// code.</param>
        /// <exception cref="System.ArgumentException">isoCurrencyCode is
        /// invalid.</exception>
        public BankCurrency(string isoCurrencyCode)
            : base(isoCurrencyCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the BankCurrency class.
        /// </summary>
        /// <param name="currency">The currency to copy.</param>
        public BankCurrency(BankCurrency currency)
            : base(currency)
        {
            Total = currency.Total;
        }


        public BankCurrency(Currency currency)
            : base(currency)
        {
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// The total amount for this currency.
        /// </summary>
        public decimal Total
        {
            get;
            set;
        }
        #endregion
    }
}