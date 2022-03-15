namespace ASC.Security.Cryptography;

[Scope]
public class EmailValidationKeyProvider
{
    public enum ValidationResult
    {
        Ok,
        Invalid,
        Expired
    }

    private readonly ILog _logger;
    private static readonly DateTime _from = new DateTime(2010, 01, 01, 0, 0, 0, DateTimeKind.Utc);
    internal readonly TimeSpan ValidEmailKeyInterval;
    internal readonly TimeSpan ValidAuthKeyInterval;
    private readonly MachinePseudoKeys _machinePseudoKeys;
    private readonly TenantManager _tenantManager;

    public EmailValidationKeyProvider(MachinePseudoKeys machinePseudoKeys, TenantManager tenantManager, IConfiguration configuration, IOptionsMonitor<ILog> options)
    {
        _machinePseudoKeys = machinePseudoKeys;
        _tenantManager = tenantManager;
        if (!TimeSpan.TryParse(configuration["email:validinterval"], out var validInterval))
        {
            validInterval = TimeSpan.FromDays(7);
        }
        if (!TimeSpan.TryParse(configuration["auth:validinterval"], out var authValidInterval))
        {
            authValidInterval = TimeSpan.FromHours(1);
        }

        ValidEmailKeyInterval = validInterval;
        ValidAuthKeyInterval = authValidInterval;
        _logger = options.CurrentValue;
    }

    public string GetEmailKey(string email)
    {
        return GetEmailKey(_tenantManager.GetCurrentTenant().Id, email);
    }

    public string GetEmailKey(int tenantId, string email)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(email);

        email = FormatEmail(tenantId, email);

        var ms = (long)(DateTime.UtcNow - _from).TotalMilliseconds;
        var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));

        return string.Format("{0}.{1}", ms, DoStringFromBytes(hash));
    }

    private string FormatEmail(int tenantId, string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        try
        {
            return string.Format("{0}|{1}|{2}", email.ToLowerInvariant(), tenantId, Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()));
        }
        catch (Exception e)
        {
            _logger.Fatal("Failed to format tenant specific email", e);

            return email.ToLowerInvariant();
        }
    }

    public ValidationResult ValidateEmailKey(string email, string key)
    {
        return ValidateEmailKey(email, key, TimeSpan.MaxValue);
    }

    public ValidationResult ValidateEmailKey(string email, string key, TimeSpan validInterval)
    {
        var result = ValidateEmailKeyInternal(email, key, validInterval);
        _logger.DebugFormat("validation result: {0}, source: {1} with key: {2} interval: {3} tenant: {4}", result, email, key, validInterval, _tenantManager.GetCurrentTenant().Id);

        return result;
    }

    private ValidationResult ValidateEmailKeyInternal(string email, string key, TimeSpan validInterval)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(email);
        ArgumentNullException.ThrowIfNull(key);

        email = FormatEmail(_tenantManager.GetCurrentTenant().Id, email);
        var parts = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return ValidationResult.Invalid;
        }

        if (!long.TryParse(parts[0], out var ms))
        {
            return ValidationResult.Invalid;
        }

        var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));
        var key2 = DoStringFromBytes(hash);
        var key2_good = string.Equals(parts[1], key2, StringComparison.OrdinalIgnoreCase);
        if (!key2_good)
        {
            return ValidationResult.Invalid;
        }

        var ms_current = (long)(DateTime.UtcNow - _from).TotalMilliseconds;

        return validInterval >= TimeSpan.FromMilliseconds(ms_current - ms) ? ValidationResult.Ok : ValidationResult.Expired;
    }

    internal static string DoStringFromBytes(byte[] data)
    {
        var str = Convert.ToBase64String(data);
        str = str.Replace("=", "").Replace("+", "").Replace("/", "").Replace("\\", "");

        return str.ToUpperInvariant();
    }

    internal static byte[] GetMashineHashedData(byte[] salt, byte[] data)
    {
        var alldata = new byte[salt.Length + data.Length];
        Array.Copy(data, alldata, data.Length);
        Array.Copy(salt, 0, alldata, data.Length, salt.Length);

        return Hasher.Hash(alldata, HashAlg.SHA256);
    }
}

public class EmailValidationKeyModel
{
    public string Key { get; set; }
    public EmployeeType? EmplType { get; set; }
    public string Email { get; set; }
    public Guid? UiD { get; set; }
    public ConfirmType? Type { get; set; }

    public void Deconstruct(out string key, out EmployeeType? emplType, out string email, out Guid? uiD, out ConfirmType? type)
    {
        (key, emplType, email, uiD, type) = (Key, EmplType, Email, UiD, Type);
    }
}

[Transient]
public class EmailValidationKeyModelHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmailValidationKeyProvider _provider;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly AuthManager _authentication;

    public EmailValidationKeyModelHelper(
        IHttpContextAccessor httpContextAccessor,
        EmailValidationKeyProvider provider,
        AuthContext authContext,
        UserManager userManager,
        AuthManager authentication)
    {
        _httpContextAccessor = httpContextAccessor;
        _provider = provider;
        _authContext = authContext;
        _userManager = userManager;
        _authentication = authentication;
    }

    public EmailValidationKeyModel GetModel()
    {
        var request = QueryHelpers.ParseQuery(_httpContextAccessor.HttpContext.Request.Headers["confirm"]);

        request.TryGetValue("type", out var type);

        ConfirmType? cType = null;
        if (Enum.TryParse<ConfirmType>(type, out var confirmType))
        {
            cType = confirmType;
        }

        request.TryGetValue("key", out var key);

        request.TryGetValue("emplType", out var emplType);
        Enum.TryParse<EmployeeType>(emplType, out var employeeType);

        request.TryGetValue("email", out var _email);
        request.TryGetValue("uid", out var userIdKey);
        Guid.TryParse(userIdKey, out var userId);

        return new EmailValidationKeyModel
        {
            Email = _email,
            EmplType = employeeType,
            Key = key,
            Type = cType,
            UiD = userId
        };
    }

    public ValidationResult Validate(EmailValidationKeyModel inDto)
    {
        var (key, emplType, email, uiD, type) = inDto;

        ValidationResult checkKeyResult;

        switch (type)
        {
            case ConfirmType.EmpInvite:
                checkKeyResult = _provider.ValidateEmailKey(email + type + (int)emplType, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.LinkInvite:
                checkKeyResult = _provider.ValidateEmailKey(type.ToString() + (int)emplType, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.PortalOwnerChange:
                checkKeyResult = _provider.ValidateEmailKey(email + type + uiD.HasValue, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.EmailChange:
                checkKeyResult = _provider.ValidateEmailKey(email + type + _authContext.CurrentAccount.ID, key, _provider.ValidEmailKeyInterval);
                break;
            case ConfirmType.PasswordChange:

                var hash = _authentication.GetUserPasswordStamp(_userManager.GetUserByEmail(email).Id).ToString("s");

                checkKeyResult = _provider.ValidateEmailKey(email + type + hash, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.Activation:
                checkKeyResult = _provider.ValidateEmailKey(email + type + uiD, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.ProfileRemove:
                // validate UiD
                var user = _userManager.GetUsers(uiD.GetValueOrDefault());
                if (user == null || user.Status == EmployeeStatus.Terminated || _authContext.IsAuthenticated && _authContext.CurrentAccount.ID != uiD)
                {
                    return ValidationResult.Invalid;
                }

                checkKeyResult = _provider.ValidateEmailKey(email + type + uiD, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.Wizard:
                checkKeyResult = _provider.ValidateEmailKey("" + type, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.PhoneActivation:
            case ConfirmType.PhoneAuth:
            case ConfirmType.TfaActivation:
            case ConfirmType.TfaAuth:
                checkKeyResult = _provider.ValidateEmailKey(email + type, key, _provider.ValidAuthKeyInterval);
                break;

            default:
                checkKeyResult = _provider.ValidateEmailKey(email + type, key, _provider.ValidEmailKeyInterval);
                break;
        }

        return checkKeyResult;
    }
}
