namespace ASC.Files.Core;

[DataContract(Namespace = "")]
public enum Thumbnail
{
    [EnumMember(Value = "0")] Waiting = 0,
    [EnumMember(Value = "1")] Created = 1,
    [EnumMember(Value = "2")] Error = 2,
    [EnumMember(Value = "3")] NotRequired = 3
}
