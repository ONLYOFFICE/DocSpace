namespace ASC.Web.Core.Helpers
{
    public static class GrammaticalHelper
    {
        public static string ChooseNumeralCase(int number, string nominative, string genitiveSingular, string genitivePlural)
        {
            if (
                System.Threading.Thread.CurrentThread.CurrentUICulture.ThreeLetterISOLanguageName.Equals("rus", StringComparison.InvariantCultureIgnoreCase))
            {
                int[] formsTable = { 2, 0, 1, 1, 1, 2, 2, 2, 2, 2 };

                number = Math.Abs(number);
                var res = formsTable[(((number % 100 / 10) != 1) ? 1 : 0) * (number % 10)];
                return res switch
                {
                    0 => nominative,
                    1 => genitiveSingular,
                    _ => genitivePlural,
                };
            }
            else
                return number == 1 ? nominative : genitivePlural;
        }
    }
}
