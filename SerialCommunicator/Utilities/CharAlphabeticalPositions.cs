using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunicator.Utilities
{
    /// <summary>
    /// A helpful class for determining the size of characters based on their position in the alphabet
    /// </summary>
    public static class CharAlphabeticalPositions
    {
        public const string ALPHABET_MIXED = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Returns the size of a letter based on it's alphabetical position. 
        /// Accounts for capital letters, and non-letters (by returning their integer value).
        /// </summary>
        /// <param name="letter">The letter you want to find the alphabetical size of</param>
        /// <returns></returns>
        public static int CharToAlphabeticalPosition(char letter)
        {
            if (ALPHABET_MIXED.Contains(letter))
            {
                if (char.IsUpper(letter))
                    return UpperCaseCharToAlphabeticalPosition(letter);
                else
                    return LowerCaseCharToAlphabeticalPosition(letter);
            }
            else
            {
                return letter;
            }
        }

        /// <summary>
        /// Returns the size of a lower case letter based on it's alphabetical position.
        /// </summary>
        /// <param name="letter">The letter you want to find the alphabetical size of</param>
        /// <returns></returns>
        public static int LowerCaseCharToAlphabeticalPosition(char letter)
        {
            return (letter - 96);
        }

        /// <summary>
        /// Returns the size of a upper case letter based on it's alphabetical position.
        /// </summary>
        /// <param name="letter">The letter you want to find the alphabetical size of</param>
        /// <returns></returns>
        public static int UpperCaseCharToAlphabeticalPosition(char letter)
        {
            return (letter - 64);
        }
    }
}
