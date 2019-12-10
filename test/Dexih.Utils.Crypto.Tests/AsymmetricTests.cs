using System;
using Xunit;
using Xunit.Abstractions;

namespace Dexih.Utils.Crypto.Tests
{
    public class AsymmetricTests
    {
        private readonly ITestOutputHelper output;

        public AsymmetricTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Theory]
        [InlineData("a")]
        [InlineData("1a")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        [InlineData("012345678901234567890123456789012345678901234567890123456789")]
        [InlineData("123123123123123")]
        [InlineData("123!@#$%^&*()_+-=`~\\|[]{};':\",./><?")]
        [InlineData("   ")]
        public void EncryptDecrypt(string TestValue)
        {
            var privateKey = AsymmetricEncrypt.GeneratePrivateKey();
            var publicKey = AsymmetricEncrypt.GeneratePublicKey(privateKey);
            
            //Use a for loop to similate gen sequence.
            var encryptString1 = AsymmetricEncrypt.Encrypt(TestValue, publicKey);
            var encryptString2 = AsymmetricEncrypt.Encrypt(TestValue, publicKey);
            Assert.False(string.IsNullOrEmpty(encryptString1));
            Assert.False(string.IsNullOrEmpty(encryptString2));
            Assert.NotEqual(encryptString1, encryptString2); //encryption is salted, so two encryptions should not be the same;

            output.WriteLine("Encrypt success.");

            //decrypt
            var decryptString1 = AsymmetricEncrypt.Decrypt(encryptString1, privateKey);
            Assert.Equal(TestValue, decryptString1);

            output.WriteLine("Decrypt1 success.");

        }
    }
}