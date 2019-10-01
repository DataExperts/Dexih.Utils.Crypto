//using System;
//using System.Text.Json;
//
//namespace Dexih.Utils.Crypto
//{
//    public class Json
//    {
//        public static string SerializeObject(object value, string encryptionKey)
//        {
//            if(value == null)
//            {
//                return null;
//            }
//            
//            var options = new JsonSerializerOptions();
//           
//            // options.Converters.Add(new EncryptConverter(encryptionKey));
//            return JsonSerializer.Serialize(value, options);
//        }
//
//        public static T DeserializeObject<T>(string value, string encryptionKey)
//        {
//            if(string.IsNullOrEmpty(value))
//            {
//                return default(T);
//            }
//
////            var options = new JsonSerializerOptions();
////            options.Converters.Add(new EncryptConverter(encryptionKey));
//
//            return JsonSerializer.Deserialize<T>(value);
//        }
//
////		public static T JTokenToObject<T>(JToken value, string encryptionKey)
////		{
////			return DeserializeObject<T>(value.ToString(), encryptionKey);
////			//if (encryptionKey == null)
////			//{
////			//	return value.ToObject<T>(new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });
////			//}
////
////			//return value.ToObject<T>(new JsonSerializer { ContractResolver = new EncryptedStringPropertyResolver(encryptionKey) });
////		}
////
////        public static JToken JTokenFromObject(object value, string encryptionKey)
////        {
////            if(value == null)
////            {
////                return null;
////            }
////            return JToken.FromObject(value, new JsonSerializer { ContractResolver = new EncryptedStringPropertyResolver(encryptionKey) });
////        }
//        
//    }
//}
