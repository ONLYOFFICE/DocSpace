namespace ASC.Files.Core.ApiModels.RequestDto;

public class ChangeOwnerRequestDto : BaseBatchRequestDto
{
    public Guid UserId { get; set; }
}
