// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2013 FortuNet


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a Charity in the system.
    /// </summary>
    public class Charity
    {
        #region Member Variables
        protected int charityId = 0;
        protected string address1 = string.Empty;
        protected string address2 = string.Empty;
        protected string city = string.Empty;
        protected string state = string.Empty;
        protected string zip = string.Empty;
        protected string country = string.Empty;
        protected string name = string.Empty;
        protected string license = string.Empty;
        protected string taxId = string.Empty;
        protected string contact = string.Empty;
        protected string phone = string.Empty;
        protected bool active = true;
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the charity.
        /// </summary>
        public int CharityId
        {
            get { return (charityId); }
            set { charityId = value; }
        }

        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        public string Address1
        {
            get { return (address1); }
            set { address1 = value; }
        }

        /// <summary>
        /// Gets or sets the second line of the address
        /// </summary>
        public string Address2
        {
            get { return (address2); }
            set { address2 = value; }
        }

        /// <summary>
        /// Gets or sets the city of the charity.
        /// </summary>
        public string City
        {
            get { return (city); }
            set { city = value; }
        }

        /// <summary>
        /// Gets or sets the state of the charity.
        /// </summary>
        public string State
        {
            get { return (state); }
            set { state = value; }
        }

        /// <summary>
        /// Gets or sets the postal code of the charity.
        /// </summary>
        public string PostalCode
        {
            get { return (zip); }
            set { zip = value; }
        }

        /// <summary>
        /// Gets or sets the country of the charity.
        /// </summary>
        public string Country
        {
            get { return (country); }
            set { country = value; }
        }

        /// <summary>
        /// Gets or sets the name of the charity.
        /// </summary>
        public string Name
        {
            get { return (name); }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the License number of the charity.
        /// </summary>
        public string License
        {
            get { return (license); }
            set { license = value; }
        }

        /// <summary>
        /// Gets or sets the tax id of the charity.
        /// </summary>
        public string TaxId
        {
            get { return (taxId); }
            set { taxId = value; }
        }

        /// <summary>
        /// Gets or sets the contact name of the charity.
        /// </summary>
        public string Contact
        {
            get { return (contact); }
            set { contact = value; }
        }

        /// <summary>
        /// Gets or sets the phone number of the charity.
        /// </summary>
        public string Phone
        {
            get { return (phone); }
            set { phone = value; }
        }

        /// <summary>
        /// Gets or sets the if the charity is active
        /// </summary>
        public bool Active
        {
            get { return (active); }
            set { active = value; }
        }

        #endregion
    }

    public class SessionCharity
    {
        #region MemberVariables
        private int sessionNumber = 0;
        private string name = string.Empty;
        private string licenseNumber = string.Empty;
        private string taxId = string.Empty;
        #endregion

        #region Constructor
        public SessionCharity()
            : this(0, string.Empty, string.Empty, string.Empty)
        {
        }

        public SessionCharity(int sessionNumber, string name, string license, string taxId)
        {
            this.sessionNumber = sessionNumber;
            this.name = name;
            this.licenseNumber = license;
            this.taxId = taxId;
        }
        #endregion

        #region Member Properties
        public int Session
        {
            get { return sessionNumber; }
            set { sessionNumber = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value;}
        }

        public string LicenseNumber
        {
            get { return licenseNumber; }
            set {licenseNumber = value;}
        }

        public string TaxPayerId
        {
            get { return taxId; }
            set { taxId = value; }
        }
        #endregion
    }
}
