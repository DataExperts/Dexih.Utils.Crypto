//using System;
//using System.Buffers;
//using System.Buffers.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//
//namespace Dexih.Utils.Crypto
//{
//    public class EncryptConverter: JsonConverter<string>
//    {
////        public EncryptConverter(string key)
////        {
////            _key = key;
////        }
//
//        private readonly string _key = "";
//
//        /// <inheritdoc />
//        public override bool CanConvert(Type objectType) {
//            return objectType == typeof(string);
//        }
//        
//        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
//        {
//            var value = EncryptString.Decrypt(reader.GetString(), _key);
//            return value;
//        }
//
//        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
//        {
//            var encrypt = EncryptString.Encrypt(value, _key);
//            writer.WriteStringValue(encrypt);
//        }
//    }
//}