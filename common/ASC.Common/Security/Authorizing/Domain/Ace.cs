namespace ASC.Common.Security.Authorizing;

[Serializable]
public class Ace
{
    public Guid ActionId { get; set; }
    public AceType Reaction { get; set; }

    public Ace(Guid actionId, AceType reaction)
    {
        ActionId = actionId;
        Reaction = reaction;
    }
}
