using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pizza.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        Ongebakken,
        Gebakken,
        Bezorgd
    }
}
