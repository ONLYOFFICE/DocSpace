namespace ASC.Api.Utils;

public static class Validate
{
    public static T If<T>(this T item, Func<T, bool> @if, Func<T> then) where T : class
    {
        return @if(item) ? then() : item;
    }

    public static T IfNull<T>(this T item, Func<T> func) where T : class
    {
        return item.If((x) => x == default(T), func);
    }

    public static T ThrowIfNull<T>(this T item, Exception e) where T : class
    {
        return item.IfNull(() => { throw e; });
    }

    public static T NotFoundIfNull<T>(this T item) where T : class
    {
        return NotFoundIfNull(item, "Item not found");
    }

    public static T NotFoundIfNull<T>(this T item, string message) where T : class
    {
        return item.IfNull(() => { throw new ItemNotFoundException(message); });
    }

    public static T? NullIfDefault<T>(this T item) where T : struct
    {
        return EqualityComparer<T>.Default.Equals(item, default(T)) ? default(T?) : item;
    }
}