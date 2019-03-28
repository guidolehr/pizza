﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PizzaModels
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        ongebakken,
        gebakken
    }
}
