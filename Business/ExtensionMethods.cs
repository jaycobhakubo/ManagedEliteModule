using System;

namespace GTI.Modules.Shared
{
    public static class ExtensionMethods
    {
        private static readonly char[] m_base32 = { 
                '0','1','2','3','4','5','6','7',
                '8','9','a','b','c','d','e','f',
                'g','h','i','j','k','l','m','n',
                'o','p','q','r','s','t','u','v'
            };

        /// <summary>
        /// Converts base  10 integer to base 32 value. 
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static string Base10To32(this int number)
        {
            string hexnumbers = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
            string hex = "";
            do
            {
                var remainder = number % 32;
                number = number / 32;
                hex = hexnumbers[remainder] + hex;
            }
            while (number > 0);
            return hex;
        }

        /// <summary>
        /// Converts base  32 string to base 10 value. 
        /// </summary>
        /// <param name="base32Number">The number.</param>
        /// <returns>returns -1 if invalid characters</returns>
        public static int Base32To10(this string base32Number)
        {
            long temp = 0;

            foreach (char val in base32Number.ToLowerInvariant())
            {
                temp = temp << 5;
                int idx = Array.IndexOf(m_base32, val);

                if (idx == -1)
                    return -1;

                temp += idx;
            }

            //return int max or the value
            return temp > int.MaxValue ? int.MaxValue: (int)temp;
        }
    }
}