using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dexih.Utils.Crypto
{
    public class EncryptConverter: JsonConverter<string>
    {
        public EncryptConverter(string key)
        {
            _key = key;
        }

        private string _key;
        
        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return EncryptString.Decrypt(reader.GetString(), _key, 100);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            var encrypt = EncryptString.Encrypt(value, _key, 100);
            writer.WriteStringValue(encrypt);
        }
    }
}