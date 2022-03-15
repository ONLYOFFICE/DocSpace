namespace System;

public static class ArgumentNullOrEmptyException
{
    public static void ThrowIfNullOrEmpty(string argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
