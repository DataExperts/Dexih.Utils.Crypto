using System;

namespace Dexih.Utils.Crypto
{

    public class InvalidEncryptionException : Exception
    {
        public InvalidEncryptionException() { }
        public InvalidEncryptionException(string message)
            : base(message) { }
        public InvalidEncryptionException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class InvalidEncryptionKeyException : Exception
    {
        public InvalidEncryptionKeyException() { }
        public InvalidEncryptionKeyException(string message)
            : base(message) { }
        public InvalidEncryptionKeyException(string message, Exception inner)
            : base(message, inner) { }
        
    }
    
    public class InvalidEncryptionTextException : Exception
    {
        public InvalidEncryptionTextException() { }
        public InvalidEncryptionTextException(string message)
            : base(message) { }
        public InvalidEncryptionTextException(string message, Exception inner)
            : base(message, inner) { }
        
    }
}