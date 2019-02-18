// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Collections.Generic;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a user of the system.
    /// </summary>
    public class Staff : IComparable, IComparable<Staff>
    {
        #region Member Variables
        protected object m_syncRoot = new object();
        protected int m_id = 0;
        protected string m_lastName = string.Empty;
        protected string m_firstName = string.Empty;
        protected DateTime m_birthDate = DateTime.MinValue;
        protected DateTime m_hireDate = DateTime.MinValue;
        protected int m_loginNum = 0;
        protected bool m_isActive = false;
        protected bool m_AcctLocked = false;
        protected string m_homePhone = string.Empty;
        protected string m_otherPhone = string.Empty;
        protected string m_govIssuedIdNum = string.Empty;
        protected string m_magCardNum = string.Empty;
        protected Address m_address = new Address();
        protected bool m_leftHanded = false;
        protected Dictionary<EliteModule, List<int>> m_modulesAndFeatures = new Dictionary<EliteModule, List<int>>();
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current Staff.
        /// </summary>
        /// <returns>A string that represents the current 
        /// Staff (FirstName LastName).</returns>
        public override string ToString()
        {
            string returnVal = string.Empty;
            if (m_firstName != string.Empty)
                returnVal = m_firstName;

            if (m_lastName != string.Empty)
            {
                if(returnVal != string.Empty)
                    returnVal += " ";

                returnVal += m_lastName;
            }

            //if(m_lastName != string.Empty)
            //    returnVal = m_lastName;

            //if(m_firstName != string.Empty)
            //{
            //    if(returnVal != string.Empty)
            //        returnVal += ", ";

            //    returnVal += m_firstName;
            //}

            return returnVal;
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// obj. Zero - This instance is equal to obj. Greater than zero - 
        /// This instance is greater than obj.</returns>
        /// <exception cref="System.ArgumentException">obj is not a 
        /// Staff.</exception>
        public int CompareTo(object obj)
        {
            Staff staff = obj as Staff;

            if(staff == null)
                throw new ArgumentException("obj");

            return CompareTo(staff);
        }

        /// <summary>
        /// Compares the current instance with another object 
        /// of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this 
        /// instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative 
        /// order of the objects being compared. The return value has 
        /// these meanings: Less than zero - This instance is less than 
        /// other. Zero - This instance is equal to other. Greater than zero - 
        /// This instance is greater than other.</returns>
        public int CompareTo(Staff other)
        {
            if(other == null)
                return 1;
            else
                return ToString().CompareTo(other.ToString());
        }

        /// <summary>
        /// Specifies the current staff has access to the passed in module.
        /// </summary>
        /// <param name="module">The module the staff has permission 
        /// to.</param>
        public void AddModule(EliteModule module)
        {
            if(!m_modulesAndFeatures.ContainsKey(module))
                m_modulesAndFeatures.Add(module, new List<int>());
        }

        /// <summary>
        /// Specifies the current staff has access to the passed in module 
        /// feature.
        /// </summary>
        /// <param name="module">The module the feature belongs to.</param>
        /// <param name="moduleFeatureId">The module feature the staff has 
        /// permission to.</param>
        public void AddModuleFeature(EliteModule module, int moduleFeatureId)
        {
            // Add the module, if needed.
            AddModule(module);

            if(!m_modulesAndFeatures[module].Contains(moduleFeatureId))
                m_modulesAndFeatures[module].Add(moduleFeatureId);
        }

        /// <summary>
        /// Checks to see if the current staff has permission to the 
        /// specified module.
        /// </summary>
        /// <param name="module">The module to check.</param>
        /// <returns>true if the user has access to the module; otherwise 
        /// false.</returns>
        public bool CheckModule(EliteModule module)
        {
            return m_modulesAndFeatures.ContainsKey(module);
        }

        /// <summary>
        /// Checks to see if the current staff has permission to the specified 
        /// module feature.
        /// </summary>
        /// <param name="module">The module the feature belongs to.</param>
        /// <param name="moduleFeatureId">The id of the feature to 
        /// check.</param>
        /// <returns>true if the user has access to the feature; otherwise 
        /// false.</returns>
        public bool CheckModuleFeature(EliteModule module, int moduleFeatureId)
        {
            if(!CheckModule(module))
                return false;
            else
                return m_modulesAndFeatures[module].Contains(moduleFeatureId);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets an object that can be used to synchronize access to 
        /// the staff.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return m_syncRoot;
            }
        }

        /// <summary>
        /// Gets or sets the staff's id.
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
        /// Gets or sets the staff's last name.
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
        /// Gets or sets the staff's first name.
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
        /// Gets or sets the staff's birth date.
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

        /// <summary>
        /// Gets or sets the staff's hire date.
        /// </summary>
        public DateTime HireDate
        {
            get
            {
                return m_hireDate;
            }
            set
            {
                m_hireDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff's login number.
        /// </summary>
        public int LoginNumber
        {
            get
            {
                return m_loginNum;
            }
            set
            {
                m_loginNum = value;
            }
        }

        public bool IsKiosk
        {
            get
            {
                return m_loginNum < 0 && string.Compare(m_firstName, "kiosk", true) == 0;
            }
        }

        /// <summary>
        /// Gets or sets whether the staff is active.
        /// </summary>
        public bool IsActive
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


        /// <summary>
        /// Gets or sets whether the staff Acctount is Locked.
        /// </summary>
        public bool AcctLocked
        {
            get
            {
                return m_AcctLocked;
            }
            set
            {
                m_AcctLocked = value;
            }
        }
        /// <summary>
        /// Gets or sets the staff's home phone number.
        /// </summary>
        public string HomePhone
        {
            get
            {
                return m_homePhone;
            }
            set
            {
                m_homePhone = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff's optional secondary phone number.
        /// </summary>
        public string OtherPhone
        {
            get
            {
                return m_otherPhone;
            }
            set
            {
                m_otherPhone = value;
            }
        }

        /// <summary>
        /// Gets or sets the staff's government issued id number.
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
        /// Gets or sets the staff's magnetic card number.
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
        /// Gets or sets the staff's address.
        /// </summary>
        public Address Address
        {
            get
            {
                return m_address;
            }
            set
            {
                m_address = value;
            }
        }

        /// <summary>
        /// Gets or sets if the staff member is left handed. 
        /// </summary>
        public bool LeftHanded
        {
            get
            {
                return m_leftHanded;
            }
            set
            {
                m_leftHanded = value;
            }
        }

        /// <summary>
        /// Gets or sets if the staff member is right handed. 
        /// </summary>
        public bool RightHanded
        {
            get
            {
                return !m_leftHanded;
            }
            set
            {
                m_leftHanded = !value;
            }
        }
      
        #endregion
    }
}
