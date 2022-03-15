namespace System
{
    public static class StringExtension
    {
        private static readonly Regex reStrict = new Regex(@"^(([^<>()[\]\\.,;:\s@\""]+"
                  + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");


        public static string HtmlEncode(this string str)
        {
            return !string.IsNullOrEmpty(str) ? HttpUtility.HtmlEncode(str) : str;
        }

        /// <summary>
        /// Replace ' on ′
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSingleQuote(this string str)
        {
            return str?.Replace('\'', '′');
        }

        public static bool TestEmailRegex(this string emailAddress)
        {
            emailAddress = (emailAddress ?? "").Trim();
            return !string.IsNullOrEmpty(emailAddress) && reStrict.IsMatch(emailAddress);
        }

        public static string GetMD5Hash(this string str)
        {
            var bytes = Encoding.Unicode.GetBytes(str);

            using var CSP = MD5.Create();

            var byteHash = CSP.ComputeHash(bytes);

            return byteHash.Aggregate(string.Empty, (current, b) => current + string.Format("{0:x2}", b));
        }

        public static int EnumerableComparer(this string x, string y)
        {
            var xIndex = 0;
            var yIndex = 0;

            while (xIndex < x.Length)
            {
                if (yIndex >= y.Length)
                    return 1;

                if (char.IsDigit(x[xIndex]) && char.IsDigit(y[yIndex]))
                {
                    var xBuilder = new StringBuilder();
                    while (xIndex < x.Length && char.IsDigit(x[xIndex]))
                    {
                        xBuilder.Append(x[xIndex++]);
                    }

                    var yBuilder = new StringBuilder();
                    while (yIndex < y.Length && char.IsDigit(y[yIndex]))
                    {
                        yBuilder.Append(y[yIndex++]);
                    }

                    long xValue;
                    try
                    {
                        xValue = Convert.ToInt64(xBuilder.ToString());
                    }
                    catch (OverflowException)
                    {
                        xValue = long.MaxValue;
                    }

                    long yValue;
                    try
                    {
                        yValue = Convert.ToInt64(yBuilder.ToString());
                    }
                    catch (OverflowException)
                    {
                        yValue = long.MaxValue;
                    }

                    int difference;
                    if ((difference = xValue.CompareTo(yValue)) != 0)
                        return difference;
                }
                else
                {
                    int difference;
                    if ((difference = string.Compare(x[xIndex].ToString(CultureInfo.InvariantCulture), y[yIndex].ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase)) != 0)
                        return difference;

                    xIndex++;
                    yIndex++;
                }
            }

            if (yIndex < y.Length)
                return -1;

            return 0;
        }
    }
}
