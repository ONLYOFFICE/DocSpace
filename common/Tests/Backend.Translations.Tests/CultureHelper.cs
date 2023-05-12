using System.Globalization;

namespace ResXManager.Infrastructure;

public static class CultureHelper
{
    public static bool IsValidCultureName(string? languageName)
    {
        try
        {
            if (string.IsNullOrEmpty(languageName))
                return false;

            // pseudo-locales:
            if (languageName.StartsWith("qps-", StringComparison.Ordinal))
                return true;

            // #376: support Custom dialect resource
            var culture = new CultureInfo(languageName);
            while (!culture.IsNeutralCulture)
            {
                culture = culture.Parent;
            }

            return WellKnownNeutralCultures.Contains(culture.Name);
        }
        catch
        {
            return false;
        }
    }

    private static class WellKnownNeutralCultures
    {
        private static readonly string[] _sortedNeutralCultureNames = GetSortedNeutralCultureNames();

        public static bool Contains(string cultureName)
        {
            return Array.BinarySearch(_sortedNeutralCultureNames, cultureName, StringComparer.OrdinalIgnoreCase) >= 0;
        }

        private static string[] GetSortedNeutralCultureNames()
        {
            var allCultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

            var cultureNames = allCultures.Select(culture => culture.IetfLanguageTag)
                .Concat(allCultures.Select(culture => culture.Name))
                .Distinct()
                .ToArray();

            Array.Sort(cultureNames, StringComparer.OrdinalIgnoreCase);

            return cultureNames;
        }
    }

    /// <summary>
    /// Gets all system specific cultures.
    /// </summary>
    public static IEnumerable<CultureInfo> SpecificCultures => WellKnownSpecificCultures.Value;

    private static class WellKnownSpecificCultures
    {
        public static readonly CultureInfo[] Value = GetSpecificCultures();

        private static CultureInfo[] GetSpecificCultures()
        {
            var specificCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(c => c.GetAncestors().Any())
                .OrderBy(c => c.DisplayName)
                .ToArray();

            return specificCultures;
        }
    }

    public static IEnumerable<CultureInfo> GetAncestors(this CultureInfo self)
    {
        var item = self.Parent;

        while (!string.IsNullOrEmpty(item.Name))
        {
            yield return item;
            item = item.Parent;
        }
    }
}