using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dexih.Utils.Crypto
{
    public static class AsymmetricEncrypt
    {
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
                    // the combined string contains length (4 bytes), encryptedKey (length), encryptedData (remainder)
                    var combinedValue = BitConverter.GetBytes(encryptedKey.Length)
                        .Concat(encryptedKey.Concat(encryptedBytes)).ToArray();
                    return Convert.ToBase64String(combinedValue);
                }
                else
                {
                    var encryptedValue = rsa.Encrypt(valueBytes, true);
                    var combinedValue = BitConverter.GetBytes(0).Concat(encryptedValue).ToArray();
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
            var length = BitConverter.ToInt32(data.Take(4).ToArray(), 0);

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            
            if (length == 0)
            {
                var value = rsa.Decrypt(data.Skip(4).ToArray(), true);
                return Encoding.UTF8.GetString(value);
            }
            else
            {
                var encryptedKey = data.Skip(4).Take(length).ToArray();
                
                var key = rsa.Decrypt(encryptedKey, true);
                var decrypted = EncryptString.Decrypt(data.Skip(4+length).ToArray(), key);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }
}