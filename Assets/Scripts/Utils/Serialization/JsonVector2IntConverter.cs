using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class JsonVector2IntConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Vector2Int obj = (Vector2Int)value;
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(obj.x);
        writer.WritePropertyName("y");
        writer.WriteValue(obj.y);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return Vector2Int.zero;
        }

        JObject obj = JObject.Load(reader);
        var x = obj.Value<int>("x");
        var y = obj.Value<int>("y");
        return new Vector2Int(x, y);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2Int);
    }
}