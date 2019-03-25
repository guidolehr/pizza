using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace IngredientFunction.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        [EnumMember(Value = "Ongebakken")]
        Ongebakken,

        [EnumMember(Value = "Gebakken")]
        Gebakken,

        [EnumMember(Value = "Bezorgd")]
        Bezorgd
    }
}
