using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class JsonColorConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Color color = (Color)value;
        writer.WriteStartObject();
        writer.WritePropertyName("r");
        writer.WriteValue(color.r);
        writer.WritePropertyName("g");
        writer.WriteValue(color.g);
        writer.WritePropertyName("b");
        writer.WriteValue(color.b);
        writer.WritePropertyName("a");
        writer.WriteValue(color.a);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return Color.black;
        }

        JObject obj = JObject.Load(reader);
        var r = obj.Value<float>("r");
        var g = obj.Value<float>("g");
        var b = obj.Value<float>("b");
        var a = obj.Value<float>("a");
        return new Color(r, g, b, a);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color);
    }
}