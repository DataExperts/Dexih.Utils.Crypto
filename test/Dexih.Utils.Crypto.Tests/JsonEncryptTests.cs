//
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using Xunit;
//
//namespace Dexih.Utils.Crypto.Tests
//{
//    
//    public class JsonEncryptTests
//    {
//        private class EncryptClass
//        {
//            public string PlainText { get; set; }
//            
//            [JsonConverter(typeof(EncryptConverter))]
//            public string StringValue { get; set; }
//            
//            public int NumberValue { get; set; }
//        }
//
//        [Fact]
//        public void JsonEncrypt()
//        {
//            var test = new EncryptClass()
//            {
//                PlainText = "don't encrypt",
//                StringValue = "encrypt this",
//                NumberValue = 123
//            };
//
//            var key = "EncryptKey";
//
//            var serialized = Json.SerializeObject(test, key);
//
//            // check values were encrypted
//            var document = JsonDocument.Parse(serialized);
//            Assert.Equal(test.PlainText, document.RootElement.GetProperty("PlainText").GetString());
//            Assert.Equal(EncryptString.Encrypt(test.StringValue, key), document.RootElement.GetProperty("StringValue").GetString());
//        }
//    }
//}