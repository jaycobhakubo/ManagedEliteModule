// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Drawing;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// Represents a level of a bingo card.
    /// </summary>
    public class CardLevel : IEquatable<CardLevel>
    {
        #region Member Variables
        protected int m_id;
        protected decimal m_multiplier;
        protected string m_name;
        protected Color m_color = Color.Empty;
        protected bool m_isActive = true; // Rally US611
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CardLevel class.
        /// </summary>
        public CardLevel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CardLevel class with the 
        /// specified id, multiplier, name, and color.
        /// </summary>
        /// <param name="id">The id of the level.</param>
        /// <param name="multiplier">The multipler of the level.</param>
        /// <param name="name">The name of the level.</param>
        /// <param name="color">The color of the level.</param>
        public CardLevel(int id, decimal multiplier, string name, int color)
            : this(id, multiplier, name, Color.FromArgb(color))
        {
        }

        /// <summary>
        /// Initializes a new instance of the CardLevel class with the 
        /// specified id, multiplier, name, and color.
        /// </summary>
        /// <param name="id">The id of the level.</param>
        /// <param name="multiplier">The multipler of the level.</param>
        /// <param name="name">The name of the level.</param>
        /// <param name="color">The color of the level.</param>
        public CardLevel(int id, decimal multiplier, string name, Color color)
        {
            m_id = id;
            m_multiplier = multiplier;
            m_name = name;
            m_color = color;
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Determines whether two CardLevel instances are equal.
        /// </summary>
        /// <param name="obj">The CardLevel to compare with the 
        /// current CardLevel.</param>
        /// <returns>true if the specified CardLevel is equal to the current 
        /// CardLevel; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            CardLevel level = obj as CardLevel;

            if(level == null)
                return false;
            else
                return Equals(level);
        }

        /// <summary>
        /// Serves as a hash function for a CardLevel. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current CardLevel.</returns>
        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }

        /// <summary>
        /// Determines whether two CardLevel instances are equal.
        /// </summary>
        /// <param name="other">The CardLevel to compare with the 
        /// current CardLevel.</param>
        /// <returns>true if the specified CardLevel is equal to the current 
        /// CardLevel; otherwise, false.</returns>
        public bool Equals(CardLevel other)
        {
           return (other != null &&
                   m_id == other.m_id);
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the level's id.
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
        /// Gets or sets the level's multiplier;
        /// </summary>
        public decimal Multiplier
        {
            get
            {
                return m_multiplier;
            }
            set
            {
                m_multiplier = value;
            }
        }

        /// <summary>
        /// Gets or sets the level's name.
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
        /// Gets or sets the level's color.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        // Rally US611
        /// <summary>
        /// Gets or sets whether the card level is active.
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
        #endregion
    }
}
