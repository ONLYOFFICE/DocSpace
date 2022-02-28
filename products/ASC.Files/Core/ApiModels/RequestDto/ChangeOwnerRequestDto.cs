namespace ASC.Files.Core.Model;

public class ChangeOwnerRequestDto : BaseBatchRequestDto
{
    public Guid UserId { get; set; }
}
