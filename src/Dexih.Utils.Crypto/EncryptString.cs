using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dexih.Utils.Crypto
{

    
    /// <summary>
    /// Code from http://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
    /// </summary>
    public static class EncryptString
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 128;
        private const int KeySizeDiv8 = Keysize / 8;

        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray(); 
        

        // This constant determines the number of iterations for the password bytes generation function.
        //private const int DerivationIterations = 1000;

        public static string GenerateRandomKey(int length = 50)
        {
            var randomBytes = new byte[length * 4];

            using (var randomNumber = RandomNumberGenerator.Create())
            {
                randomNumber.GetBytes(randomBytes);
            }

            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var rnd = BitConverter.ToUInt32(randomBytes, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string Encrypt(string plainText, string key, int derivationIterations = 100)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return "";
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidEncryptionKeyException("There is no encryption key specified.");
            }
            
            var encryptBytes = Encrypt(Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(key),
                derivationIterations);
            return Convert.ToBase64String(encryptBytes);
        }
        
        /// <summary>
        /// Encrypts the string value using the passPhase as the encryption key.
        /// </summary>
        /// <param name="plainText">String to encrypt</param>
        /// <param name="key">Encryption Key</param>
        /// <param name="derivationIterations"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] plainTextBytes, byte[] key, int derivationIterations = 100)
        {
            try
            {
                // Salt and IV is randomly generated each time, but is prepended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                var saltStringBytes = Generate256BitsOfRandomEntropy();
                var ivStringBytes = Generate256BitsOfRandomEntropy();
                
                using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, derivationIterations))
                {
                    var keyBytes = password.GetBytes(KeySizeDiv8);
                    using (var symmetricKey = Aes.Create()) 
                    {
                        if (symmetricKey == null)
                        {
                            throw new InvalidEncryptionKeyException("Failed to create the encryption key");
                        }

                        symmetricKey.BlockSize = Keysize;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;

                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        using (var memoryStream = new MemoryStream())
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                            var cipherTextBytes = new byte[KeySizeDiv8 * 2 + memoryStream.Length];
                            Array.Copy(saltStringBytes, cipherTextBytes, KeySizeDiv8);
                            Array.Copy(ivStringBytes, 0, cipherTextBytes, KeySizeDiv8,  KeySizeDiv8);
                            Array.Copy(memoryStream.ToArray(), 0, cipherTextBytes, KeySizeDiv8 * 2,  memoryStream.Length);
                            return cipherTextBytes;
                        }
                    }
                }
            }
            catch (InvalidEncryptionKeyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidEncryptionException("The encrypt function encountered an unknown error.  See inner exception for details.", ex);
            }
        }

        public static string Decrypt(string cipherText, string key, int derivationIterations = 100)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return "";
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidEncryptionKeyException("There is no encryption key specified.");
            }

            try
            {
                var value = Decrypt(Convert.FromBase64String(cipherText), Encoding.UTF8.GetBytes(key),
                    derivationIterations);
                return Encoding.UTF8.GetString(value);
            }
            catch (FormatException ex)
            {
                throw new InvalidEncryptionTextException("The text was not encrypted in the required format.", ex);
            }
        }

        /// <summary>
        /// Decrypts a string using the encryption key.
        /// </summary>
        /// <param name="cipherText">The encrypted value to decrypt.</param>
        /// <param name="key">The encryption key used to initially encrypt the string.</param>
        /// <param name="derivationIterations"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] cipherBytes, byte[] key, int derivationIterations = 100)
        {
            try
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = cipherBytes;
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = CopyArray(cipherTextBytesWithSaltAndIv, 0, KeySizeDiv8);
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = CopyArray(cipherTextBytesWithSaltAndIv, KeySizeDiv8, KeySizeDiv8);
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = CopyArray(cipherTextBytesWithSaltAndIv, KeySizeDiv8 * 2, cipherTextBytesWithSaltAndIv.Length - ((KeySizeDiv8) * 2));

                using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, derivationIterations))
                using (var symmetricKey = Aes.Create())
                {
                    if (symmetricKey == null)
                    {
                        throw new InvalidEncryptionKeyException("Failed to create the encryption key");
                    }

                    symmetricKey.BlockSize = Keysize;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    var keyBytes = password.GetBytes(KeySizeDiv8);

                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        var plainTextBytes = new byte[cipherTextBytes.Length];
                        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        return CopyArray(plainTextBytes, 0, decryptedByteCount);
                    }
                }
            }
            catch (InvalidEncryptionKeyException)
            {
                throw;
            }
            catch (InvalidEncryptionTextException)
            {
                throw;
            }
            catch (CryptographicException ex)
            {
                throw new InvalidEncryptionTextException(
                    "The string could not be decrypted.  This is most likely due to an invalid encryption key or data.",
                    ex);

            }
            catch (FormatException ex)
            {
                throw new InvalidEncryptionTextException("The text was not encrypted in the required format.", ex);
            }
            catch(Exception ex)
            {
                throw new InvalidEncryptionException("The decrypt function encountered an unknown error.  See inner exception for details.", ex);
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[KeySizeDiv8]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = RandomNumberGenerator.Create()) // // not supported in .net core new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        
        internal static byte[] CopyArray(byte[] bytes, int start, int count)
        {
            var newBytes = new byte[count];
            Array.Copy(bytes, start, newBytes, 0, count);
            return newBytes;
        }
    }
}
