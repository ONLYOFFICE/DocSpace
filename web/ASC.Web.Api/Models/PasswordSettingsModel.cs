namespace ASC.Web.Api.Models
{
    public class PasswordSettingsModel
    {
        /// <summary>
        /// Minimal length password has
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Password must contains upper case
        /// </summary>
        public bool UpperCase { get; set; }

        /// <summary>
        /// Password must contains digits
        /// </summary>
        public bool Digits { get; set; }

        /// <summary>
        /// Password must contains special symbols
        /// </summary>
        public bool SpecSymbols { get; set; }
    }
}
