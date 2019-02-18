#region copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2011 GameTech
// International, Inc.
#endregion

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Class to hold the accrual account information
    /// </summary>
    public class AccrualAccount
    {
        /// <summary>
        /// The accrual account ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The percentage to increase
        /// </summary>
        public string Percentage { get; set; }

        /// <summary>
        /// The name of the account
        /// </summary>
        public string Name { get; set; }
    }
}
