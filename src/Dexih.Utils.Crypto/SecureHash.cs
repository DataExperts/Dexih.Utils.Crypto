// Based on algorithm
// https://github.com/defuse/password-hashing/blob/master/PasswordStorage.cs

using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Dexih.Utils.Crypto
{
    class InvalidHashException : Exception
    {
        public InvalidHashException() { }
        public InvalidHashException(string message)
            : base(message) { }
        public InvalidHashException(string message, Exception inner)
            : base(message, inner) { }
    }

    class CannotPerformOperationException : Exception
    {
        public CannotPerformOperationException() { }
        public CannotPerformOperationException(string message)
            : base(message) { }
        public CannotPerformOperationException(string message, Exception inner)
            : base(message, inner) { }
    }
    
    public static class SecureHash
    {
        // These constants may be changed without breaking existing hashes.
        private const int SALT_BYTES = 24;
        private const int HASH_BYTES = 18;
        private const int PBKDF2_ITERATIONS = 64000;
        private static readonly char[] delimiter = { ':' };

        // These constants define the encoding and may not be changed.
        private const int HASH_SECTIONS = 5;
        private const int HASH_ALGORITHM_INDEX = 0;
        private const int ITERATION_INDEX = 1;
        private const int HASH_SIZE_INDEX = 2;
        private const int SALT_INDEX = 3;
        private const int PBKDF2_INDEX = 4;

        public static string CreateHash(string password)
        {
            if (password == null)
            {
                return null;
            }
            
            // Generate a random salt
            byte[] salt = new byte[SALT_BYTES];
            try {
                
                using (var csprng = RandomNumberGenerator.Create()) {
                        csprng.GetBytes(salt);
                }
            } catch (CryptographicException ex) {
                throw new CannotPerformOperationException(
                    "Random number generator not available.",
                    ex
                );
            } catch (ArgumentNullException ex) {
                throw new CannotPerformOperationException(
                    "Invalid argument given to random number generator.",
                    ex
                );
            }

            byte[] hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, PBKDF2_ITERATIONS, HASH_BYTES);

            // format: algorithm:iterations:hashSize:salt:hash
            string parts = "sha256:" +
                PBKDF2_ITERATIONS +
                ":" +
                hash.Length +
                ":" +
                Convert.ToBase64String(salt) +
                ":" +
                Convert.ToBase64String(hash);
            
            return parts;
        }

        public static bool ValidateHash(string password, string goodHash)
        {
            var split = goodHash.Split(delimiter);

            if (split.Length != HASH_SECTIONS) {
                throw new InvalidHashException(
                    "Fields are missing from the password hash."
                );
            }

            // We only support SHA256
            if (split[HASH_ALGORITHM_INDEX] != "sha256") {
                throw new CannotPerformOperationException(
                    "Unsupported hash type."
                );
            }

            int iterations;
            try {
                iterations = int.Parse(split[ITERATION_INDEX]);
            } catch (ArgumentNullException ex) {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            } catch (FormatException ex) {
                throw new InvalidHashException(
                    "Could not parse the iteration count as an integer.",
                    ex
                );
            } catch (OverflowException ex) {
                throw new InvalidHashException(
                    "The iteration count is too large to be represented.",
                    ex
                );
            }

            if (iterations < 1) {
                throw new InvalidHashException(
                    "Invalid number of iterations. Must be >= 1."
                );
            }

            byte[] salt;
            try {
                salt = Convert.FromBase64String(split[SALT_INDEX]);
            }
            catch (ArgumentNullException ex) {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            } catch (FormatException ex) {
                throw new InvalidHashException(
                    "Base64 decoding of salt failed.",
                    ex
                );
            }

            byte[] hash;
            try {
                hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
            }
            catch (ArgumentNullException ex) {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            } catch (FormatException ex) {
                throw new InvalidHashException(
                    "Base64 decoding of pbkdf2 output failed.",
                    ex
                );
            }

            int storedHashSize;
            try {
                storedHashSize = int.Parse(split[HASH_SIZE_INDEX]);
            } catch (ArgumentNullException ex) {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            } catch (FormatException ex) {
                throw new InvalidHashException(
                    "Could not parse the hash size as an integer.",
                    ex
                );
            } catch (OverflowException ex) {
                throw new InvalidHashException(
                    "The hash size is too large to be represented.",
                    ex
                );
            }

            if (storedHashSize != hash.Length) {
                throw new InvalidHashException(
                    "Hash length doesn't match stored hash length."
                );
            }

            var testHash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++) {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}