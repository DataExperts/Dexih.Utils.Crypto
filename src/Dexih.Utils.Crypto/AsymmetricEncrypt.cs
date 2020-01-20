using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dexih.Utils.Crypto
{
    public static class AsymmetricEncrypt
    {
        private static readonly byte[] ZeroBytes = new byte[] {0x00, 0x00 ,0x00 ,0x00};
        /// <summary>
        /// Generates a new private key for asymmetric encryption.
        /// </summary>
        /// <returns></returns>
        public static string GeneratePrivateKey()
        {
            var rsa = new RSACryptoServiceProvider();
            return rsa.ToXmlString(true);
        }

        /// <summary>
        /// Returns the public portion of a private key
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string GeneratePublicKey(string privateKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            return rsa.ToXmlString(false);
        }

        public static string Encrypt(string value, string publicKey)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return Encrypt(Encoding.UTF8.GetBytes(value), publicKey);
        }

        /// <summary>
        /// Encrypts the data using the public key
        /// </summary>
        /// <param name="value"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string Encrypt(byte[] valueBytes, string publicKey)
        {
            // use rsa to encrypt the key
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                // for large keys use symmetric encryption, and then just use RSA to encrypt the key.
                if (valueBytes.Length > 50)
                {
                    // generate a symmetric new key.
                    byte[] key = new byte[20];
                    using (var randomNumberGenerator = RandomNumberGenerator.Create())
                    {
                        randomNumberGenerator.GetBytes(key);
                    }

                    var encryptedBytes = EncryptString.Encrypt(valueBytes, key);
                    var encryptedKey = rsa.Encrypt(key, true);

                    // combine the rsa and encrypted bytes.
                    // the combined string contains length (4 bytes), encryptedKey (length), encryptedBytes (remainder)
                    var combinedValue = new byte[4 + encryptedKey.Length + encryptedBytes.Length];
                    Array.Copy(BitConverter.GetBytes(encryptedKey.Length), combinedValue, 4);
                    Array.Copy(encryptedKey, 0, combinedValue, 4, encryptedKey.Length);
                    Array.Copy(encryptedBytes, 0, combinedValue, 4 + encryptedKey.Length, encryptedBytes.Length);
                    
                    return Convert.ToBase64String(combinedValue);
                }
                else
                {
                    var encryptedValue = rsa.Encrypt(valueBytes, true);
                    var combinedValue = new byte[4 + encryptedValue.Length];
                    Array.Copy(ZeroBytes, 0, combinedValue, 0, 4);
                    Array.Copy(encryptedValue, 0, combinedValue, 4, encryptedValue.Length);
                    return Convert.ToBase64String(combinedValue);
                }
            }
        }

        public static string Decrypt(string cypherString, string privateKey)
        {
            return Decrypt(Convert.FromBase64String(cypherString), privateKey);
        }

        /// <summary>
        /// Decrypts the data using the private key
        /// </summary>
        /// <param name="cypherString"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string Decrypt(byte[] data, string privateKey)
        {
            var lengthBytes = new byte[4];
            Array.Copy(data, lengthBytes, 4);
            var length = BitConverter.ToInt32(lengthBytes, 0);

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            
            if (length == 0)
            {
                var value = rsa.Decrypt(EncryptString.CopyArray(data, 4, data.Length - 4), true);
                return Encoding.UTF8.GetString(value);
            }
            else
            {
                var encryptedKey = new byte[length];
                Array.Copy(data, 4, encryptedKey, 0, length);
                var key = rsa.Decrypt(encryptedKey, true);
                var decrypted = EncryptString.Decrypt(EncryptString.CopyArray(data, 4+length,data.Length - 4 - length), key);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }
}