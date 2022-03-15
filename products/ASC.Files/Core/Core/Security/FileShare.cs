namespace ASC.Files.Core.Security;

public enum FileShare
{
    None,
    ReadWrite,
    Read,
    Restrict,
    Varies,
    Review,
    Comment,
    FillForms,
    CustomFilter
}

public class FileShareConverter : System.Text.Json.Serialization.JsonConverter<FileShare>
{
    public override FileShare Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == System.Text.Json.JsonTokenType.Number && reader.TryGetInt32(out var result))
        {
            return (FileShare)result;
        }
        else
        {
            if (reader.TokenType == JsonTokenType.String && Enum.TryParse<FileShare>(reader.GetString(), out var share))
            {
                return share;
            }
            else
            {
                return FileShare.None;
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, FileShare value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((int)value);
    }
}
