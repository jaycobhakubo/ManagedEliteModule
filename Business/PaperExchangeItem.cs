#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2016 FortuNet, Inc.
#endregion

using System;

namespace GTI.Modules.Shared.Business
{
    public class PaperExchangeItem
    {
        public string Name { get; set; }

        public string Serial { get; set; }

        public int Audit { get; set; }

        #region Sale Information

        /// <summary>
        /// The unique ID of the receipt this item was sold to
        /// </summary>
        public int ReceiptID { get; set; }
        /// <summary>
        /// The transaction number for the receipt this item was sold to
        /// </summary>
        public int TransactionNumber { get; set; }
        /// <summary>
        /// The session this item was sold to
        /// </summary>
        public int SoldSession { get; set; }
        /// <summary>
        /// the gaming date this item was sold on
        /// </summary>
        public DateTime GamingDate { get; set; }
        /// <summary>
        /// The machine this item was sold on
        /// </summary>
        public int Machine { get; set; }
        /// <summary>
        /// The cashier that sold this item
        /// </summary>
        public string Cashier { get; set; }

        #endregion
    }
}
