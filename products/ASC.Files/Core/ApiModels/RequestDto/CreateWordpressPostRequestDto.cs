namespace ASC.Files.Core.ApiModels.RequestDto;

public class CreateWordpressPostRequestDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int Status { get; set; }
}
