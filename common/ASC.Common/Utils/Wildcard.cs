namespace ASC.Common.Utils;

public static class Wildcard
{
    public static bool WildcardMatch(this string input, string pattern)
    {
        return WildcardMatch(input, pattern, true);
    }

    public static bool WildcardMatch(this string input, string pattern, bool ignoreCase)
    {
        if (!string.IsNullOrEmpty(input))
        {
            return IsMatch(pattern, input, ignoreCase);
        }

        return false;
    }

    public static bool IsMatch(string pattern, string input)
    {
        return IsMatch(pattern, input, true);
    }

    public static bool IsMatch(string pattern, string input, bool ignoreCase)
    {
        var offsetInput = 0;
        var isAsterix = false;
            int i = 0;
            while (i < pattern.Length)
        {
            switch (pattern[i])
            {
                case '?':
                    isAsterix = false;
                    offsetInput++;
                    break;
                case '*':
                    isAsterix = true;

                    while (i < pattern.Length &&
                            pattern[i] == '*')
                    {
                        i++;
                    }

                    if (i >= pattern.Length)
                    {
                        return true;
                    }

                    continue;
                default:
                    if (offsetInput >= input.Length)
                    {
                        return false;
                    }

                    if ((ignoreCase
                                ? char.ToLower(input[offsetInput])
                                : input[offsetInput])
                        !=
                        (ignoreCase
                                ? char.ToLower(pattern[i])
                                : pattern[i]))
                    {
                        if (!isAsterix)
                        {
                            return false;
                        }

                        offsetInput++;

                        continue;
                    }
                    offsetInput++;
                    break;
            }
            i++;
        }

        if (i > input.Length)
        {
            return false;
        }

        while (i < pattern.Length && pattern[i] == '*')
        {
            ++i;
        }

            return offsetInput == input.Length;
    }
}
