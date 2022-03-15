namespace ASC.Data.Storage;

public static class Wildcard
{
    public static bool IsMatch(string pattern, string input)
    {
        return IsMatch(pattern, input, false);
    }

    public static bool IsMatch(string pattern, string input, bool caseInsensitive)
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
                    while (i < pattern.Length && pattern[i] == '*')
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
                    if ((caseInsensitive ? char.ToLower(input[offsetInput]) : input[offsetInput]) != (caseInsensitive ? char.ToLower(pattern[i]) : pattern[i]))
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
            } // end switch
            i++;
        } // end for

        // have we finished parsing our input?
        if (i > input.Length)
        {
            return false;
        }
        // do we have any lingering asterixes we need to skip?
        while (i < pattern.Length && pattern[i] == '*')
        {
            ++i;
        }
        // final evaluation. The index should be pointing at the
        // end of the string.
            return offsetInput == input.Length;
    }
}
