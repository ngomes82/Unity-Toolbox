using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class JsonVector3IntConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector3Int obj = (Vector3Int)value;
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
            return Vector3Int.zero;
        }

        JObject obj = JObject.Load(reader);
        var x = obj.Value<int>("x");
        var y = obj.Value<int>("y");
        var z = obj.Value<int>("z");
        return new Vector3Int(x, y, z);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3Int);
    }
}