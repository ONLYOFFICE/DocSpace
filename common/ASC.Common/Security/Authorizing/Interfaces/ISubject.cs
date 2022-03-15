namespace ASC.Common.Security.Authorizing;

public interface ISubject : IIdentity
{
    Guid ID { get; }
}
