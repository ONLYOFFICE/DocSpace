namespace ASC.Files.Core.ApiModels.RequestDto;

public class CheckConversionRequestDto<T>
{
    public T FileId { get; set; }
    public bool Sync { get; set; }
    public bool StartConvert { get; set; }
    public int Version { get; set; }
    public string Password { get; set; }
}
