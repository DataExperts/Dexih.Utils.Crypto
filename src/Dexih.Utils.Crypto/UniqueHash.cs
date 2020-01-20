using System;
using System.Security.Cryptography;
using System.Text;

namespace Dexih.Utils.Crypto
{
    public static class UniqueHash
    {
        /// <summary>
        /// Computes a unique SHA256 hash value for the specified string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CreateHash(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}