using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;

namespace ASC.Core.Users;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmployeeType
{
    All = 0,
    User = 1,
    Visitor = 2
}
