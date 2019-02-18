#region copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2011 GameTech
// International, Inc.
#endregion

using System.Collections.Generic;
using GTI.Modules.Shared.Business;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Class to hold the accural informaiton
    /// </summary>
    public class Accrual : IEqualityComparer<Accrual>
    {
        private List<SchedProgram> m_programs = new List<SchedProgram>();

        /// <summary>
        /// The accrualID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Accrual Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Increase type
        /// </summary>
        public byte IncreaseType { get; set; }

        /// <summary>
        /// Increase amount
        /// </summary>
        public string IncreaseAmount { get; set; }

        /// <summary>
        /// Witholding amount
        /// </summary>
        public string WitholdingAmount { get; set; }

        /// <summary>
        /// List of accrual accounts
        /// </summary>
        public List<AccrualAccount> AccrualAccounts { get; set; }

        /// <summary>
        /// List of session Programs associated with Accrual
        /// </summary>
        public List<SchedProgram> Programs
        {
            get { return m_programs; }
            set
            {
                if(value == null)
                    m_programs = new List<SchedProgram>();
                else
                    m_programs = value;
            }
        }

        /// <summary>
        /// Accrual Type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Is checked
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Accrual applies (auto-associates) with all session programs
        /// </summary>
        public bool AllPrograms { get; set; }

        /// <summary>
        /// Accrual applies (auto-associates) to sessionless sales
        /// </summary>
        public bool AppliesToSessionlessSales { get; set; }
        
        /// <summary>
        /// Overrides the to string operation
        /// </summary>
        /// <returns>the accural name</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Overrides the equality operator
        /// </summary>
        /// <param name="obj">the object to compare</param>
        /// <returns>true if the objects ids are equal else false</returns>
        public override bool Equals(object obj)
        {
            Accrual accrual = obj as Accrual;
            if(accrual != null)
            {
                return accrual.Id.Equals(Id);
            }
            return false;
        }

        /// <summary>
        /// Overrides the get hash code
        /// </summary>
        /// <returns>the hash code of the accrual</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


        #region IEqualityComparer<Accural> Members
        /// <summary>
        /// Checks to see if two accruals are equal
        /// </summary>
        /// <param name="x">the first accrual</param>
        /// <param name="y">the second accrual</param>
        /// <returns>true if the ids are equal else false</returns>
        public bool Equals(Accrual x, Accrual y)
        {
            if(x != null && y != null)
            {
                return x.Id.Equals(y.Id);
            }
            return false;
        }

        /// <summary>
        /// Gets the accrual hash code
        /// </summary>
        /// <param name="obj">the accrual to check</param>
        /// <returns>the integer</returns>
        public int GetHashCode(Accrual obj)
        {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}
