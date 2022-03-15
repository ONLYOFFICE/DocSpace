namespace ASC.MessagingSystem.Models;

public class MessageUserData
{
    public int TenantId { get; init; }
    public Guid UserId { get; init; }

    public MessageUserData(int tenentId, Guid userId)
    {
        TenantId = tenentId;
        UserId = userId;
    }
}
