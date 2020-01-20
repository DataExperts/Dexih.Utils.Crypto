using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dexih.Utils.Crypto.Tests
{
    public class FunctionEncryptString
    {
        private readonly ITestOutputHelper output;

        public FunctionEncryptString(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void RandomKey()
        {
            var key = EncryptString.GenerateRandomKey(50);
            
            Assert.Equal(50, key.Length);
            
            // check no bad characters
            key = EncryptString.GenerateRandomKey(50000);
            
            Assert.False(key.Contains("/"));
            Assert.False(key.Contains("+"));
        }

        [Theory]
        [InlineData("a", "abc")]
        [InlineData("1a","abc")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "abc")]
        [InlineData("123123123123123", "abc")]
        [InlineData("123!@#$%^&*()_+-=`~\\|[]{};':\",./><?", "abc")]
        [InlineData("   ", "abc")]
        public void EncryptDecrypt(string TestValue, string Key)
        {
            //Use a for loop to simulate gen sequence.
            var encryptString1 = EncryptString.Encrypt(TestValue, Key, 1000);
            var encryptString2 = EncryptString.Encrypt(TestValue, Key, 1000);
            Assert.False(string.IsNullOrEmpty(encryptString1));
            Assert.False(string.IsNullOrEmpty(encryptString2));
            Assert.NotEqual(encryptString1, encryptString2); //encryption is salted, so two encryptions should not be the same;

            output.WriteLine("Encrypt success.");

            //decrypt
            var decryptString1 = EncryptString.Decrypt(encryptString1, Key, 1000);
            Assert.Equal(TestValue, decryptString1);

            output.WriteLine("Decrypt1 success.");

            //decypt with modified key.  this will usually throw, due to incompatible key.  when is does pass, ensure the values are not the same.
            try
            {
                var decryptstring = EncryptString.Decrypt(encryptString1, Key + " ", 1000);

                Assert.NotEqual(decryptstring, TestValue);
            } catch(Exception ex)
            {

            }

            output.WriteLine("Decrypt2 success.");
        }

        [Fact]
        public void DecryptBadKey()
        {
            var value = EncryptString.Encrypt("test value", "key", 100);

            Assert.Throws<InvalidEncryptionTextException>(() => EncryptString.Decrypt(value, "badKey", 100));
        }

        [Fact]
        public void DecryptBadText()
        {
            Assert.Throws<InvalidEncryptionTextException>(() => EncryptString.Decrypt("1231231", "key", 100));
        }

        [Theory]
        [InlineData(2000)]
        public void EncryptPerformance(int iterations)
        {
            for(int i = 0; i< iterations; i++)
            {
                var value = EncryptString.GenerateRandomKey(200);
                var key = EncryptString.GenerateRandomKey(50);
                EncryptDecrypt(value, key);
            }
        }
    }
}
