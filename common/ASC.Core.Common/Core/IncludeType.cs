namespace ASC.Core.Users;

[Flags]
public enum IncludeType
{
    Distinct = 1,
    InParent = 2,
    InChild = 4,
    All = Distinct | InParent | InChild
}
