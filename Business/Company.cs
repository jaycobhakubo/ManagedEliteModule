// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a company in the system.
    /// </summary>
    public class Company
    {
        #region Member Variables
        protected int m_companyId = 0;
        protected int m_subCompanyId = 0;
        protected int m_addressId = 0;
        protected string m_name = string.Empty;
        protected string m_phone = string.Empty;
        protected string m_owner = string.Empty;
        protected bool m_isActive = true;
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the company.
        /// </summary>
        public int CompanyID
        {
            get
            {
                return m_companyId;
            }
            set
            {
                m_companyId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the sub-company.
        /// </summary>
        public int SubCompanyID
        {
            get
            {
                return m_subCompanyId;
            }
            set
            {
                m_subCompanyId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the address.
        /// </summary>
        public int AddressID
        {
            get
            {
                return m_addressId;
            }
            set
            {
                m_addressId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the company.
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
        /// Gets or sets the company's phone number.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the owner's name.
        /// </summary>
        public string Owner
        {
            get
            {
                return m_owner;
            }
            set
            {
                m_owner = value;
            }
        }


        /// <summary>
        /// Determins if the company record is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return m_isActive;
            }
            set
            {
                m_isActive = value;
            }
        }
        #endregion
    }
}
