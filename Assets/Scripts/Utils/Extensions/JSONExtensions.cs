using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonExtensions
{
    static JsonConverter[] defaultCustomJsonConverters = new JsonConverter[]
    {
        new JsonVector2Converter(),
        new JsonVector2IntConverter(),
        new JsonVector3Converter(),
        new JsonVector3IntConverter(),
        new JsonColorConverter(),
        new JsonVector4Converter(),
        new JsonQuaternionConverter()
    };

    public static string SerializeJson(this object obj, Formatting formatting = Formatting.None)
    {
        return JsonConvert.SerializeObject(obj, formatting, new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Converters = defaultCustomJsonConverters
        });
    }

    public static T DeserializeJson<T>(this string jsonStr)
    {
        return JsonConvert.DeserializeObject<T>(jsonStr, defaultCustomJsonConverters);
    }
}
