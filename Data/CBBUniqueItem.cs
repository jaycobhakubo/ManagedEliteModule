#region Copyright

// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2019 All rights reserved

#endregion


using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using GTI.Modules.Shared;
using System.Linq;

namespace GTI.Modules.Shared.Data
{
    public class CBBUniqueItem : IEquatable<CBBUniqueItem>
    {
        #region Member Variables
        public int sessionPlayedID;
        public int gameCategoryID;
        public string cbbNumbers;
        #endregion

        #region Constructors
        public CBBUniqueItem()
        {
            sessionPlayedID = 0;
            gameCategoryID = 0;
            cbbNumbers = "";
        }

        public CBBUniqueItem(int sessionPlayedID, int gameCategoryID, string cbbNumbers, bool sortNumbers = false)
        {
            SessionPlayedID = sessionPlayedID;
            GameCategoryID = gameCategoryID;
            CBBNumbers = sortNumbers ? SortNumbers(cbbNumbers) : cbbNumbers;
        }

        public CBBUniqueItem(int sessionPlayedID, int gameCategoryID, byte[] cbbNumbers, int howManyNumbers = 0)
        {
            SessionPlayedID = sessionPlayedID;
            GameCategoryID = gameCategoryID;
            CBBNumbers = SortNumbers(cbbNumbers, howManyNumbers);
        }
        #endregion


        #region Member Methods

        public static string SortNumbers(string numbersList)
        {
            StringBuilder sortedNumbers = new StringBuilder();
            string[] numbers = numbersList.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            numbers.OrderBy(n => Convert.ToInt32(n));

            foreach (string s in numbers)
            {
                int num = Convert.ToInt32(s);

                if (num != 0)
                    sortedNumbers.Append(s.Trim() + ",");
            }

            if (sortedNumbers.Length > 0)
                sortedNumbers.Remove(sortedNumbers.Length - 1, 1); //remove the last ","

            return sortedNumbers.ToString();
        }

        public static string SortNumbers(byte[] numbersList, int howManyNumbers = 0)
        {
            StringBuilder sortedNumbers = new StringBuilder();
            byte[] numbers = new byte[howManyNumbers != 0? howManyNumbers : numbersList.Count()];

            numbersList.CopyTo(numbers, 0);
            numbers.OrderBy(n => Convert.ToInt32(n));

            foreach (byte b in numbers)
            {
                if (b != 0)
                    sortedNumbers.Append(b.ToString() + ",");
            }

            if (sortedNumbers.Length > 0)
                sortedNumbers.Remove(sortedNumbers.Length - 1, 1); //remove the last ","

            return sortedNumbers.ToString();
        }

        public static byte[] NumbersFromString(string numbers)
        {
            try
            {
                string[] nums = numbers.Split(',');
                byte[] face = new byte[nums.Length];

                for (int c = 0; c < nums.Length; c++)
                    face[c] = Convert.ToByte(nums[c]);

                return face;
            }
            catch
            {
                throw new Exception("Invalid card face.");
            }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            CBBUniqueItem cbbDupObj = obj as CBBUniqueItem;

            if (cbbDupObj == null)
                return false;
            else
                return Equals(cbbDupObj);
        }

        public bool Equals(CBBUniqueItem other)
        {
            if (other == null)
                return false;

            if (this.SessionPlayedID == other.SessionPlayedID &&
                this.GameCategoryID == other.GameCategoryID &&
                this.CBBNumbers == other.CBBNumbers)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() ^ sessionPlayedID.GetHashCode() ^ gameCategoryID.GetHashCode() ^
                    cbbNumbers.GetHashCode());
        }
        #endregion

        #region Member Properties

        public int SessionPlayedID
        {
            get
            {
                return sessionPlayedID;
            }

            set
            {
                sessionPlayedID = value;
            }
        }

        public int GameCategoryID
        {
            get
            {
                return gameCategoryID;
            }

            set
            {
                gameCategoryID = value;
            }
        }

        public string CBBNumbers
        {
            get
            {
                return cbbNumbers;
            }

            set
            {
                cbbNumbers = value;
            }
        }

        #endregion
    }
}
