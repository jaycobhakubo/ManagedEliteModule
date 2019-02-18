// This is an unpublished work protected under the copyright laws of the
// United States and other countries.  All rights reserved.  Should
// publication occur the following will apply:  © 2007 GameTech
// International, Inc.

using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace GTI.Modules.Shared
{
    /// <summary>
    /// This class exposes helpful functions related to security.
    /// </summary>
    public class SecurityHelper
    {
        #region Static Methods
        /// <summary>
        /// Returns a hash based on the password passed in.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The password's hashed value.</returns>
        public static byte[] HashPassword(string password)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            
            return sha1.ComputeHash(Encoding.Unicode.GetBytes(password));
        }
        #endregion
    }
}
