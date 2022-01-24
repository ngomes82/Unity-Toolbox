using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class JsonVector3Converter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector3 obj = (Vector3)value;
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(obj.x);
        writer.WritePropertyName("y");
        writer.WriteValue(obj.y);
        writer.WritePropertyName("z");
        writer.WriteValue(obj.z);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return Vector3.zero;
        }

        JObject obj = JObject.Load(reader);
        var x = obj.Value<float>("x");
        var y = obj.Value<float>("y");
        var z = obj.Value<float>("z");
        return new Vector3(x, y, z);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3);
    }
}