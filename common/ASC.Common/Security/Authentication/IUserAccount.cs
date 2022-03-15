namespace ASC.Common.Security.Authentication;

public interface IUserAccount : IAccount
{
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    string Title { get; }
    int Tenant { get; }
}
