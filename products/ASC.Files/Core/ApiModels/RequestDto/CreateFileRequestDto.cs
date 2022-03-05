namespace ASC.Files.Core.Model;

public class CreateFileRequestDto<T>
{
    public string Title { get; set; }
    public T TemplateId { get; set; }
    public bool EnableExternalExt { get; set; }
}
