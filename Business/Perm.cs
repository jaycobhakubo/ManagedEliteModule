#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2008 GameTech
// International, Inc.
#endregion

// PDTS 1098

using System;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a permutation of bingo card numbers.
    /// </summary>
    public abstract class Perm : IEquatable<Perm>
    {
        #region Member Methods
        /// <summary>
        /// Determines whether two Perm instances are equal.
        /// </summary>
        /// <param name="obj">The Perm to compare with the 
        /// current Perm.</param>
        /// <returns>true if the specified Perm is equal to the current 
        /// Perm; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Perm perm = obj as Perm;

            if(perm == null)
                return false;
            else
                return Equals(perm);
        }

        /// <summary>
        /// Serves as a hash function for a Perm. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Perm.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether two Perm instances are equal.
        /// </summary>
        /// <param name="other">The Perm to compare with the 
        /// current Perm.</param>
        /// <returns>true if the specified Perm is equal to the current 
        /// Perm; otherwise, false.</returns>
        public bool Equals(Perm other)
        {
            return (other != null && GetType().Equals(other.GetType()));
        }
        #endregion
    }

    /// <summary>
    /// Represents a permutation of bingo card numbers in the United States.
    /// </summary>
    public class USPerm : Perm, IEquatable<USPerm>
    {
        #region Member Variables
        protected int m_id;
        protected string m_name;
        protected string m_manufacturer;
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current USPerm.
        /// </summary>
        /// <returns>A string that represents the current USPerm.</returns>
        public override string ToString()
        {
            if(!string.IsNullOrEmpty(m_name))
                return m_name;
            else
                return m_id.ToString();
        }

        /// <summary>
        /// Determines whether two USPerm instances are equal.
        /// </summary>
        /// <param name="obj">The USPerm to compare with the 
        /// current USPerm.</param>
        /// <returns>true if the specified USPerm is equal to the current 
        /// USPerm; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            USPerm perm = obj as USPerm;

            if(perm == null)
                return false;
            else
                return Equals(perm);
        }

        /// <summary>
        /// Serves as a hash function for a USPerm. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current USPerm.</returns>
        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }

        /// <summary>
        /// Determines whether two USPerm instances are equal.
        /// </summary>
        /// <param name="other">The USPerm to compare with the 
        /// current USPerm.</param>
        /// <returns>true if the specified USPerm is equal to the current 
        /// USPerm; otherwise, false.</returns>
        public bool Equals(USPerm other)
        {
            return (base.Equals(other) && m_id == other.m_id);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the perm's id in the system.
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
        /// Gets or sets the name of the perm.
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
        /// Gets or sets the name of the company who created the perm.
        /// </summary>
        public string Manufacturer
        {
            get
            {
                return m_manufacturer;
            }
            set
            {
                m_manufacturer = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a permutation of bingo card numbers in the United Kingdom.
    /// </summary>
    public class UKPerm : Perm
    {
        #region Member Variables
        protected short m_tdm;
        protected short m_series;
        protected int m_serialStart;
        protected int m_serialEnd;
        protected int m_start;
        protected int m_pageStep;
        protected GameType m_gameType;
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current UKPerm.
        /// </summary>
        /// <returns>A string that represents the current UKPerm.</returns>
        public override string ToString()
        {
            return m_tdm.ToString();
        }

        /// <summary>
        /// Determines whether two UKPerm instances are equal.
        /// </summary>
        /// <param name="obj">The UKPerm to compare with the 
        /// current UKPerm.</param>
        /// <returns>true if the specified UKPerm is equal to the current 
        /// UKPerm; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            UKPerm perm = obj as UKPerm;

            if(perm == null)
                return false;
            else
                return Equals(perm);
        }

        /// <summary>
        /// Serves as a hash function for a UKPerm. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current UKPerm.</returns>
        public override int GetHashCode()
        {
            return (m_tdm ^ m_series);
        }

        /// <summary>
        /// Determines whether two UKPerm instances are equal.
        /// </summary>
        /// <param name="other">The UKPerm to compare with the 
        /// current UKPerm.</param>
        /// <returns>true if the specified UKPerm is equal to the current 
        /// UKPerm; otherwise, false.</returns>
        public bool Equals(UKPerm other)
        {
            return (base.Equals(other) && m_tdm == other.m_tdm && m_series == other.m_series);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the TDM number.
        /// </summary>
        public short TDM
        {
            get
            {
                return m_tdm;
            }
            set
            {
                m_tdm = value;
            }
        }

        /// <summary>
        /// Gets or sets the series number.
        /// </summary>
        public short Series
        {
            get
            {
                return m_series;
            }
            set
            {
                m_series = value;
            }
        }

        /// <summary>
        /// Gets or sets the start of the serial number range.
        /// </summary>
        public int SerialStart
        {
            get
            {
                return m_serialStart;
            }
            set
            {
                m_serialStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the end of the serial number range.
        /// </summary>
        public int SerialEnd
        {
            get
            {
                return m_serialEnd;
            }
            set
            {
                m_serialEnd = value;
            }
        }

        /// <summary>
        /// Gets or sets the perm's start number.
        /// </summary>
        public int Start
        {
            get
            {
                return m_start;
            }
            set
            {
                m_start = value;
            }
        }

        /// <summary>
        /// Gets or sets the page step.
        /// </summary>
        public int PageStep
        {
            get
            {
                return m_pageStep;
            }
            set
            {
                m_pageStep = value;
            }
        }

        /// <summary>
        /// Gets or sets the perm's game type.
        /// </summary>
        public GameType GameType
        {
            get
            {
                return m_gameType;
            }
            set
            {
                m_gameType = value;
            }
        }
        #endregion
    }
}
