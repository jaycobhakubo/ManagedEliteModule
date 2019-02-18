// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a location in the system.
    /// </summary>
    public class Location
    {
        #region Member Variables
        protected int m_LocationID = 0;
        protected int m_SubLocationID = 0;
        protected int m_CompanyID = 0;
        protected int m_AddressID = 0;
        protected string m_Name = string.Empty;
        protected string m_Phone = string.Empty;
        protected string m_Modem = string.Empty;
        protected string m_RoomName = string.Empty;
        protected bool m_Active = true;
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id of the location.
        /// </summary>
        public int LocationID
        {
            get
            {
                return m_LocationID;
            }
            set
            {
                m_LocationID = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the address.
        /// </summary>
        public int AddressID
        {
            get
            {
                return m_AddressID;
            }
            set
            {
                m_AddressID = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the sub-location.
        /// </summary>
        public int SubLocationID
        {
            get
            {
                return m_SubLocationID;
            }
            set
            {
                m_SubLocationID = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the company.
        /// </summary>
        public int CompanyID
        {
            get
            {
                return m_CompanyID;
            }
            set
            {
                m_CompanyID = value;
            }
        }

        /// <summary>
        /// Gets or sets the location's name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the Phone.
        /// </summary>
        public string Phone
        {
            get
            {
                return m_Phone;
            }
            set
            {
                m_Phone = value;
            }
        }

        /// <summary>
        /// Gets or sets the Modem.
        /// </summary>
        public string Modem
        {
            get
            {
                return m_Modem;
            }
            set
            {
                m_Modem = value;
            }
        }

        /// <summary>
        /// Gets or sets the Room Name.
        /// </summary>
        public string RoomName
        {
            get
            {
                return m_RoomName;
            }
            set
            {
                m_RoomName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Active flag.
        /// </summary>
        public bool Active 
        {
            get
            {
                return m_Active;
            }
            set
            {
                m_Active = value;
            }
        }
        #endregion
    }
}
