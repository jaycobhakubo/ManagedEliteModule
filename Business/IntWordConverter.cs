#region Copyright
// This is an unpublished work protected under the copyright laws of the United
// States and other countries.  All rights reserved.  Should publication occur
// the following will apply:  © 2008-2016 FortuNet Inc, GameTech International, Inc.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTI.Modules.Shared
{
    public class IntWordConverter
    {
        /// <summary>
        /// Converts written-out words to their numerical equivelent
        /// range: +-999 billion
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static long WordsToNumber(string words)
        {
            Dictionary<string, int> units = new Dictionary<string, int>
            {
                {"zero", 0},{"a", 1},{"one", 1},{"two", 2},{"three", 3},{"four", 4},{"five", 5},{"six", 6},{"seven", 7},{"eight", 8},{"nine", 9},{"ten", 10},
                {"eleven", 11},{"twelve", 12},{"thirteen", 13},{"fourteen", 14},{"fifteen", 15},{"sixteen", 16},{"seventeen", 17},{"eighteen", 18},{"nineteen", 19},
                {"twenty", 20},{"thirty", 30},{"fourty", 40},{"fifty", 50},{"sixty", 60},{"seventy", 70},{"eighty", 80},{"ninety", 90}
            };

            Dictionary<string, int> hundreds = new Dictionary<string, int> { { "hundred", 100 }, { "thousand", 1000 }, { "million", 1000000 }, { "billion", 1000000000 } };

            bool isNeg = false;
            long number = 0;
            words = words.Trim();
            words = words.Replace(" and", "");
            words = words.Replace("  ", " ");
            string[] numbers = words.Split(new char[] { ' ', '-' });
            for (int i = 0; i < numbers.Length; i++)
            {
                if (i < numbers.Length - 1)
                    words = words.Remove(0, numbers[i].Length + 1);
                else
                    words = "";

                if (numbers[i].ToLower().Equals("negative") || numbers[i].ToLower().Equals("minus"))
                    isNeg = !isNeg;
                else if (units.ContainsKey(numbers[i]))
                {
                    number += units[numbers[i]];
                }
                else if (hundreds.ContainsKey(numbers[i]))
                {
                    if (number == 0)
                        number = 1;
                    number *= hundreds[numbers[i]];

                    if (i != numbers.Length - 1)
                    {
                        if (hundreds.ContainsKey(numbers[i + 1]))
                            number *= WordsToNumber(words);
                        else
                            number += WordsToNumber(words);
                    }
                    //number += WordsToNumber(words);
                    break;
                }
            }

            if (isNeg)
                number *= -1;
            return number;
        }

        /// <summary>
        /// Converts a number to its written-out equivelent
        /// range: +-999 billion (beyond that it will start to say "one-thousand billion")
        /// </summary>
        /// <param name="number">integer to convert</param>
        /// <returns>string representation of the integer</returns>
        public static string NumberToWords(long number)
        {
            int oneBillion = 1000000000, oneMillion = 1000000, oneThousand = 1000, oneHundred = 100;

            string words = "";

            if (number < 0)
                words = "minus " + NumberToWords(Math.Abs(number));
            else
            {
                if ((number / oneBillion) > 0)
                {
                    words += NumberToWords(number / oneBillion) + " billion ";
                    number %= oneBillion;
                }

                if ((number / oneMillion) > 0)
                {
                    words += NumberToWords(number / oneMillion) + " million ";
                    number %= oneMillion;
                }

                if ((number / oneThousand) > 0)
                {
                    words += NumberToWords(number / oneThousand) + " thousand ";
                    number %= oneThousand;
                }

                if ((number / oneHundred) > 0)
                {
                    words += NumberToWords(number / oneHundred) + " hundred ";
                    number %= oneHundred;
                }

                if (number > 0)
                {
                    string[] unitsMap = new string[]{ "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", 
                                    "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                    string[] tensMap = new string[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                    if (words != "")
                        words += "and ";

                    if (number < 20)
                        words += unitsMap[number];
                    else
                    {
                        words += tensMap[number / 10];
                        if ((number % 10) > 0)
                            words += "-" + unitsMap[number % 10];
                    }
                }
                else if (words == "")
                {
                    words = "zero";
                }
            }

            words = words.Replace("  ", " ").Trim();
            return words;
        }

    }
}
