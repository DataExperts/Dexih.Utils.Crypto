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
            var hashString1 = HashString.CreateHash(testValue);
            var hashString2 = HashString.CreateHash(testValue);
            Assert.NotEqual(hashString1, hashString2); //two hashes in a row should not be equal as they are salted;

            var hashString3 = HashString.CreateHash(testValue + " ");

            Assert.True(HashString.ValidateHash(testValue, hashString1));
            Assert.True(HashString.ValidateHash (testValue, hashString2));

            Assert.False(HashString.ValidateHash (testValue, hashString3));
            Assert.False(HashString.ValidateHash(testValue + "1", hashString1 ));
        }
    }
}
