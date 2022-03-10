namespace ASC.Files.Core.ApiModels.RequestDto;

public class CreateFileRequestDto<T>
{
    public string Title { get; set; }
    public T TemplateId { get; set; }
    public bool EnableExternalExt { get; set; }
}
