namespace ASC.Web.Core.Utility
{
    [Serializable]
    public sealed class PasswordSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("aa93a4d1-012d-4ccd-895a-e094e809c840"); }
        }

        public const int MaxLength = 30;

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

        public ISettings GetDefault(IConfiguration configuration)
        {
            var def = new PasswordSettings { MinLength = 8, UpperCase = false, Digits = false, SpecSymbols = false };

            if (int.TryParse(configuration["web.password.min"], out var defaultMinLength))
            {
                def.MinLength = Math.Max(1, Math.Min(MaxLength, defaultMinLength));
            }

            return def;
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return GetDefault(serviceProvider.GetService<IConfiguration>());
        }
    }
}