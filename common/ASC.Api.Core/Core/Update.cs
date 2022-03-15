namespace ASC.Api.Utils;

public static class Update
{
    public static T IfNotEquals<T>(T current, T @new)
    {
        if (!Equals(current, @new)) return @new;

        return current;
    }

    public static T IfNotEmptyAndNotEquals<T>(T current, T @new)
    {
        if (Equals(@new, default(T))) return current;

        if (!Equals(current, @new)) return @new;

        return current;
    }
}