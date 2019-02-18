#region Copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007-2010 GameTech
// International, Inc.
#endregion

using System;

namespace GTI.Modules.Shared
{
    // Rally TA7465
    /// <summary>
    /// Represents a type of denomination.
    /// </summary>
    public enum DenominationType
    {
        Coin = 1,
        Bill = 2
    }

    /// <summary>
    /// Represents a single type of currency.
    /// </summary>
    public class Denomination : IEquatable<Denomination>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Denomination class.
        /// </summary>
        public Denomination()
        {
            Type = DenominationType.Bill;
            AllowAcceptor = true;
            IsActive = true;
        }

        /// <summary>
        /// Initializes a new instance of the Denomination class.
        /// </summary>
        /// <param name="id">The id of the denomination.</param>
        /// <param name="type">The type of the denomination (i.e. coin, bill,
        /// etc.)</param>
        /// <param name="name">The name of the denomination.</param>
        /// <param name="value">The value of the denomination in relation to 
        /// the currency's base denomination.</param>
        /// <param name="count">The count of this denomination.</param>
        /// <param name="allowAcceptor">Whether this denomination can be 
        /// accepted by the system.</param>
        /// <param name="isActive">Whether this denomination is active in the
        /// system.</param>
        /// <param name="order">Order</param>
        public Denomination(int id, DenominationType type, string name, decimal value, int count, bool allowAcceptor, bool isActive, short order = 0)
        {
            Id = id;
            Type = type;
            Name = name;
            Value = value;
            Count = count;
            AllowAcceptor = allowAcceptor;
            IsActive = isActive;
            Order = order;
        }

        /// <summary>
        /// Initializes a new instance of the Denomination class.
        /// </summary>
        /// <param name="denom">The denomination to copy.</param>
        public Denomination(Denomination denom)
        {
            Id = denom.Id;
            Type = denom.Type;
            Name = denom.Name;
            Value = denom.Value;
            Count = denom.Count;
            AllowAcceptor = denom.AllowAcceptor;
            IsActive = denom.IsActive;
        }
        #endregion

        #region Member Methods
        /// <summary>
        /// Returns a string that represents the current Denomination.
        /// </summary>
        /// <returns>A string that represents the current 
        /// Denomination.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Determines whether two Denomination instances are equal. 
        /// </summary>
        /// <param name="obj">The Denomination to compare with the 
        /// current Denomination.</param>
        /// <returns>true if the specified Denomination is equal to the current 
        /// Denomination; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            Denomination denom = obj as Denomination;

            if(denom == null)
                return false;
            else
                return Equals(denom);
        }

        /// <summary>
        /// Determines whether two Denomination instances are equal. 
        /// </summary>
        /// <param name="other">The Denomination to compare with the 
        /// current Denomination.</param>
        /// <returns>true if the specified Denomination is equal to the current 
        /// Denomination; otherwise, false.</returns>
        public bool Equals(Denomination other)
        {
            return (other != null && 
                    Id == other.Id &&
                    Type == other.Type &&
                    Name == other.Name &&
                    Value == other.Value &&
                    Count == other.Count &&
                    AllowAcceptor == other.AllowAcceptor &&
                    IsActive == other.IsActive);
        }

        /// <summary>
        /// Serves as a hash function for a Denomination. 
        /// GetHashCode is suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Denomination.</returns>
        public override int GetHashCode()
        {
            return (Id.GetHashCode() ^ 
                    Type.GetHashCode() ^ 
                    (Name != null ? Name.GetHashCode() : 0) ^ 
                    Value.GetHashCode() ^ 
                    Count.GetHashCode() ^
                    AllowAcceptor.GetHashCode() ^ 
                    IsActive.GetHashCode());
        }
        #endregion

        #region Member Properties
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type (i.e. coin, bill, etc.).
        /// </summary>
        public DenominationType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the denomination in relation to the 
        /// currency's base denomination.
        /// </summary>
        public decimal Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the count of this denomination.
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this denomination can be accepted 
        /// by the system.
        /// </summary>
        public bool AllowAcceptor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this denomination is active.
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }

        //US5380
        public short Order
        {
            get; 
            set;
        }
        #endregion

    }
    // END: TA7465
}