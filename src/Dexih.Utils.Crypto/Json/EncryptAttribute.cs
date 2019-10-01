//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//
//namespace Dexih.Utils.Crypto
//{
//    [AttributeUsage(AttributeTargets.Property)]
//    public class JsonEncryptAttribute : Attribute
//    {
//    }
//
//    public class EncryptedStringPropertyResolver : CamelCasePropertyNamesContractResolver
//    {
//        private readonly string _encryptionKey;
//
//        public EncryptedStringPropertyResolver(string encryptionKey)
//        {
//            _encryptionKey = encryptionKey;
//        }
//
//        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
//        {
//            var props = base.CreateProperties(type, memberSerialization);
//
//            if (_encryptionKey != null)
//            {
//                // Find all string properties that have a [JsonEncrypt] attribute applied
//                // and attach an EncryptedStringValueProvider instance to them
//                foreach (var prop in props.Where(p => p.PropertyType == typeof(string)))
//                {
//                    var pi = type.GetProperty(prop.UnderlyingName);
//                    if (pi != null && pi.GetCustomAttribute(typeof(JsonEncryptAttribute), true) != null)
//                    {
//                        prop.ValueProvider =
//                            new EncryptedStringValueProvider(pi, _encryptionKey);
//                    }
//
//                }
//            }
//
//            return props;
//        }
//
//        private class EncryptedStringValueProvider : IValueProvider
//        {
//            private readonly PropertyInfo _targetProperty;
//            private readonly string _encryptionKey;
//
//            public EncryptedStringValueProvider(PropertyInfo targetProperty, string encryptionKey)
//            {
//                _targetProperty = targetProperty;
//                _encryptionKey = encryptionKey;
//            }
//
//            // GetValue is called by Json.Net during serialization.
//            // The target parameter has the object from which to read the unencrypted string;
//            // the return value is an encrypted string that gets written to the JSON
//            public object GetValue(object target)
//            {
//                var value = (string)_targetProperty.GetValue(target);
//                if(string.IsNullOrEmpty(value))
//                {
//                    return null;
//                }
//
//                string encryptedValue;
//                try
//                {
//                    encryptedValue = EncryptString.Encrypt(value, _encryptionKey, 1000);
//
//                }
//                catch(Exception ex)
//                {
//                    throw new AggregateException("Encryption failed on property " + _targetProperty.Name + ".  See inner exception for details.", ex);
//                }
//
//                return encryptedValue;
//            }
//
//            // SetValue gets called by Json.Net during deserialization.
//            // The value parameter has the encrypted value read from the JSON;
//            // target is the object on which to set the decrypted value.
//            public void SetValue(object target, object value)
//            {
//                if(string.IsNullOrEmpty((string)value))
//                {
//                    _targetProperty.SetValue(target, null);
//                }
//
//                string decryptedValue;
//                try
//                {
//                    decryptedValue = EncryptString.Decrypt((string)value, _encryptionKey, 1000);
//                } 
//                catch(Exception ex)
//                {
//                    throw new AggregateException("Decryption failed on property " + _targetProperty.Name + ".  See inner exception for details.", ex);
//                }
//
//                _targetProperty.SetValue(target, decryptedValue);
//            }
//
//        }
//    }
//}