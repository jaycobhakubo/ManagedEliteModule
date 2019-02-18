#region copyright
// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2011 GameTech
// International, Inc.
#endregion

namespace GTI.Modules.Shared
{
    public class PayoutSchedule
    {
        /// <summary>
        /// The Payout Schedule ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The payout Schedule Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The payout schedule is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Ovverrides the ToString
        /// </summary>
        /// <returns>The payoutScheduleName</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Determines if the payout schedule equals another object
        /// </summary>
        /// <param name="obj">an object</param>
        /// <returns>true if the object is a payout schedule and the id is the same</returns>
        public override bool Equals(object obj)
        {
            PayoutSchedule payoutSchedule = obj as PayoutSchedule;
            if (payoutSchedule != null && payoutSchedule.Id == Id)
                return true;
            return false;
        }

        /// <summary>
        /// Overrrides the get hash code
        /// </summary>
        /// <returns>a hash code of the Id</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
