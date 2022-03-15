namespace ASC.Core.Tenants;

[Singletone]
public class TenantDomainValidator
{
    private static readonly Regex _validDomain = new Regex("^[a-z0-9]([a-z0-9-]){1,98}[a-z0-9]$",
                                                          RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private readonly int _minLength;
    private const int _maxLength = 100;

    public TenantDomainValidator(IConfiguration configuration)
    {
        _minLength = 6;

        if (int.TryParse(configuration["web:alias:min"], out var defaultMinLength))
        {
            _minLength = Math.Max(1, Math.Min(_maxLength, defaultMinLength));
        }
    }

    public void ValidateDomainLength(string domain)
    {
        if (string.IsNullOrEmpty(domain)
            || domain.Length < _minLength || _maxLength < domain.Length)
        {
            throw new TenantTooShortException("The domain name must be between " + _minLength + " and " + _maxLength + " characters long.", _minLength, _maxLength);
        }
    }

    public static void ValidateDomainCharacters(string domain)
    {
        if (!_validDomain.IsMatch(domain))
        {
            throw new TenantIncorrectCharsException("Domain contains invalid characters.");
        }
    }
}
