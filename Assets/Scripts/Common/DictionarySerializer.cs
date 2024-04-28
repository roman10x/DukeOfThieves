using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RotaryHeart.Lib.SerializableDictionaryPro;

namespace DukeOfThieves.Common
{
    public class DictionarySerializer<TKey,TValue> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SerializableDictionary<TKey, TValue>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        { 
            var data = new SerializableDictionary<TKey, TValue>();
            var jObj = JObject.ReadFrom(reader);
            List<TKey> keys = JsonConvert.DeserializeObject<List<TKey>>(jObj["keys"].ToString());
            List<TValue> values  = JsonConvert.DeserializeObject<List<TValue>>(jObj["values"].ToString());
  
            for (int i = 0; i < keys.Count; i++)
            {
                data.Add(keys[i], values[i]);
            }
            
            return data;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<TKey> _keys = new List<TKey>();
            List<TValue> _values = new List<TValue>();

            var dict = (SerializableDictionary<TKey, TValue>)value;
            if (dict.Keys.Count > 0)
            {
                foreach (var kvp in dict)
                {
                    _keys.Add(kvp.Key);

                    _values.Add(kvp.Value);
                }

                writer.WriteStartObject();
                writer.WritePropertyName("keys");
                writer.WriteRawValue(JsonConvert.SerializeObject(_keys));
                
                writer.WritePropertyName("values");
                writer.WriteRawValue(JsonConvert.SerializeObject(_values));
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNull();
            }

        }
    }
}