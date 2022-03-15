namespace ASC.Core;

[Serializable]
public class AzRecord : IMapFrom<Acl>
{
    public Guid Subject { get; set; }
    public Guid Action { get; set; }
    public string Object { get; set; }
    public AceType AceType { get; set; }
    public int Tenant { get; set; }

    public AzRecord() { }

    public AzRecord(Guid subjectId, Guid actionId, AceType reaction)
        : this(subjectId, actionId, reaction, default(string))
    {
    }

    public AzRecord(Guid subjectId, Guid actionId, AceType reaction, string fullId)
    {
        Subject = subjectId;
        Action = actionId;
        AceType = reaction;
        Object = fullId;
    }

    public static implicit operator AzRecord(AzRecordCache cache)
    {
        var result = new AzRecord()
        {
            Tenant = cache.Tenant
        };


        if (Guid.TryParse(cache.SubjectId, out var subjectId))
        {
            result.Subject = subjectId;
        }

        if (Guid.TryParse(cache.ActionId, out var actionId))
        {
            result.Action = actionId;
        }

        result.Object = cache.ObjectId;

        if (Enum.TryParse<AceType>(cache.Reaction, out var reaction))
        {
            result.AceType = reaction;
        }

        return result;
    }

    public static implicit operator AzRecordCache(AzRecord cache)
    {
        return new AzRecordCache
        {
            SubjectId = cache.Subject.ToString(),
            ActionId = cache.Action.ToString(),
            ObjectId = cache.Object,
            Reaction = cache.AceType.ToString(),
            Tenant = cache.Tenant
        };
    }

    public override bool Equals(object obj)
    {
        return obj is AzRecord r &&
            r.Tenant == Tenant &&
            r.Subject == Subject &&
            r.Action == Action &&
            r.Object == Object &&
            r.AceType == AceType;
    }

    public override int GetHashCode()
    {
        return Tenant.GetHashCode() ^
            Subject.GetHashCode() ^
            Action.GetHashCode() ^
            (Object ?? string.Empty).GetHashCode() ^
            AceType.GetHashCode();
    }
}
