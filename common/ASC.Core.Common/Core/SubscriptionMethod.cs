namespace ASC.Core;

[Serializable]
public class SubscriptionMethod : IMapFrom<DbSubscriptionMethod>
{
    public int Tenant { get; set; }
    public string Source { get; set; }
    public string Action { get; set; }
    public string Recipient { get; set; }
    public string[] Methods { get; set; }

    public static implicit operator SubscriptionMethod(SubscriptionMethodCache cache)
    {
        return new SubscriptionMethod()
        {
            Tenant = cache.Tenant,
            Source = cache.SourceId,
            Action = cache.ActionId,
            Recipient = cache.RecipientId
        };
    }

    public static implicit operator SubscriptionMethodCache(SubscriptionMethod cache)
    {
        return new SubscriptionMethodCache
        {
            Tenant = cache.Tenant,
            SourceId = cache.Source,
            ActionId = cache.Action,
            RecipientId = cache.Recipient
        };
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbSubscriptionMethod, SubscriptionMethod>()
            .ForMember(dest => dest.Methods, opt => opt
                .MapFrom(src => src.Sender.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)));
    }
}
