// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents an address in the system.
    /// </summary>
    public class Address
    {
        #region Member Variables
        protected int m_addressId = 0;
        protected string m_address1 = string.Empty;
        protected string m_address2 = string.Empty;
        protected string m_city = string.Empty;
        protected string m_state = string.Empty;
        protected string m_zip = string.Empty;
        protected string m_country = string.Empty;
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the address.
        /// </summary>
        public int AddressID
        {
            get { return m_addressId; }
            set { m_addressId = value; }
        }

        /// <summary>
        /// Gets or sets the Address1
        /// </summary>
        public string Address1
        {
            get { return m_address1; }
            set { m_address1 = value; }
        }

        /// <summary>
        /// Gets or sets the Address2
        /// </summary>
        public string Address2
        {
            get { return m_address2; }
            set { m_address2 = value; }
        }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string City
        {
            get { return m_city; }
            set { m_city = value; }
        }

        /// <summary>
        /// Gets or Sets the state.
        /// </summary>
        public string State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        /// <summary>
        /// Gets or Sets the state.
        /// </summary>
        public string Zipcode
        {
            get { return m_zip; }
            set { m_zip = value; }
        }

        /// <summary>
        /// Gets or Sets the state.
        /// </summary>
        public string Country
        {
            get { return m_country; }
            set { m_country = value; }
        }
        #endregion
    }
}
