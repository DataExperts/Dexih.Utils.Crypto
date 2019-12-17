using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dexih.Utils.Crypto.Tests
{
    public class FunctionHashString
    {
        [Theory]
        [InlineData("a")]
        [InlineData("1a")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("123123123123123")]
        [InlineData("123!@#$%^&*()_+-=`~\\|[]{};':\",./><?")]
        [InlineData("   ")]
        public void HashFunctions(string testValue)
        {
            //Use a for loop to similate gen sequence.
            var hashString1 = SecureHash.CreateHash(testValue);
            var hashString2 = SecureHash.CreateHash(testValue);
            Assert.NotEqual(hashString1, hashString2); //two hashes in a row should not be equal as they are salted;

            var hashString3 = SecureHash.CreateHash(testValue + " ");

            Assert.True(SecureHash.ValidateHash(testValue, hashString1));
            Assert.True(SecureHash.ValidateHash (testValue, hashString2));

            Assert.False(SecureHash.ValidateHash (testValue, hashString3));
            Assert.False(SecureHash.ValidateHash(testValue + "1", hashString1 ));

            var uniqueHash1 = UniqueHash.CreateHash(testValue);
            var uniqueHash2 = UniqueHash.CreateHash(testValue);
            Assert.Equal(uniqueHash1, uniqueHash2);
            
            var uniqueHash3 = UniqueHash.CreateHash(testValue + " ");
            Assert.NotEqual(uniqueHash1, uniqueHash3);
        }
    }
}
