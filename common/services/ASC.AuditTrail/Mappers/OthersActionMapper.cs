namespace ASC.AuditTrail.Mappers;

internal static class OthersActionsMapper
{
    public static Dictionary<MessageAction, MessageMaps> GetMaps() =>
        new Dictionary<MessageAction, MessageMaps>
        {
                    {
                        MessageAction.ContactAdminMailSent, new MessageMaps
                            {
                                ActionTypeTextResourceName = "SendActionType",
                                ActionTextResourceName = "ContactAdminMailSent",
                                ProductResourceName = "OthersProduct"
                            }
                    }
        };
}