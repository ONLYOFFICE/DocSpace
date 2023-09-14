// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.FederatedLogin.Profile;

[DebuggerDisplay("{DisplayName} ({Id})")]
public class LoginProfile
{
    public const string QueryParamName = "up";
    public const string QuerySessionParamName = "sup";
    public const string QueryCacheParamName = "cup";

    public string Id
    {
        get => GetField(WellKnownFields.Id);
        internal set => SetField(WellKnownFields.Id, value);
    }

    public string Link
    {
        get => GetField(WellKnownFields.Link);
        internal set => SetField(WellKnownFields.Link, value);
    }

    public string Name
    {
        get => GetField(WellKnownFields.Name);
        internal set => SetField(WellKnownFields.Name, value);
    }

    public string DisplayName
    {
        get => GetField(WellKnownFields.DisplayName);
        internal set => SetField(WellKnownFields.DisplayName, value);
    }

    public string EMail
    {
        get => GetField(WellKnownFields.Email);
        internal set { SetField(WellKnownFields.Email, value); }
    }

    public string Avatar
    {
        get => GetField(WellKnownFields.Avatar);
        internal set => SetField(WellKnownFields.Avatar, value);
    }

    public string Gender
    {
        get => GetField(WellKnownFields.Gender);
        internal set => SetField(WellKnownFields.Gender, value);
    }

    public string FirstName
    {
        get => GetField(WellKnownFields.FirstName);
        internal set => SetField(WellKnownFields.FirstName, value);
    }

    public string LastName
    {
        get => GetField(WellKnownFields.LastName);
        internal set => SetField(WellKnownFields.LastName, value);
    }

    public string MiddleName
    {
        get => GetField(WellKnownFields.MiddleName);
        internal set => SetField(WellKnownFields.MiddleName, value);
    }

    public string Salutation
    {
        get => GetField(WellKnownFields.Salutation);
        internal set => SetField(WellKnownFields.Salutation, value);
    }

    public string BirthDay
    {
        get => GetField(WellKnownFields.BirthDay);
        internal set => SetField(WellKnownFields.BirthDay, value);
    }

    public string Locale
    {
        get => GetField(WellKnownFields.Locale);
        internal set => SetField(WellKnownFields.Locale, value);
    }

    public string TimeZone
    {
        get => GetField(WellKnownFields.Timezone);
        internal set => SetField(WellKnownFields.Timezone, value);
    }

    public string AuthorizationResult
    {
        get => GetField(WellKnownFields.Auth);
        internal set => SetField(WellKnownFields.Auth, value);
    }

    public string AuthorizationError
    {
        get => GetField(WellKnownFields.AuthError);
        internal set => SetField(WellKnownFields.AuthError, value);
    }

    public string Provider
    {
        get => GetField(WellKnownFields.Provider);
        internal set => SetField(WellKnownFields.Provider, value);
    }

    public string RealmUrl
    {
        get => GetField(WellKnownFields.RealmUrl);
        internal set => SetField(WellKnownFields.RealmUrl, value);
    }

    public string Hash
    {
        get => _signature?.Create(HashId);
        set => throw new NotImplementedException();
    }

    public string Serialized
    {
        get => Transport();
        set => throw new NotImplementedException();
    }

    public string UserDisplayName
    {
        get
        {
            if (!string.IsNullOrEmpty(DisplayName))
            {
                return DisplayName;
            }

            var combinedName = string.Join(" ",
                                           new[] { FirstName, MiddleName, LastName }.Where(
                                               x => !string.IsNullOrEmpty(x)).ToArray());
            if (string.IsNullOrEmpty(combinedName))
            {
                combinedName = Name;
            }

            return combinedName;
        }
    }

    public string UniqueId => $"{Provider}/{Id}";
    public string HashId => HashHelper.MD5(UniqueId);
    public bool IsFailed => !string.IsNullOrEmpty(AuthorizationError);
    public bool IsAuthorized => !IsFailed;

    private const char KeyValueSeparator = '→';
    private const char PairSeparator = '·';

    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private IDictionary<string, string> _fields = new Dictionary<string, string>();

    public LoginProfile GetMinimalProfile()
    {
        var profileNew = new LoginProfile(_signature, _instanceCrypto)
        {
            Provider = Provider,
            Id = Id
        };

        return profileNew;
    }

    public static bool HasProfile(HttpContext context)
    {
        return context != null && HasProfile(context.Request);
    }

    public static bool HasProfile(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Uri(request.GetDisplayUrl()).HasProfile();
    }

    public static LoginProfile GetProfile(Signature signature, InstanceCrypto instanceCrypto, HttpContext context, IMemoryCache memoryCache)
    {
        if (context == null)
        {
            return new LoginProfile(signature, instanceCrypto);
        }

        return new Uri(context.Request.GetDisplayUrl()).GetProfile(context, memoryCache, signature, instanceCrypto);
    }

    public LoginProfile(Signature signature, InstanceCrypto instanceCrypto, string transport) : this(signature, instanceCrypto)
    {
        FromTransport(transport);
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    internal string GetField(string name)
    {
        return _fields.ContainsKey(name) ? _fields[name] : string.Empty;
    }

    internal void SetField(string name, string value)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (!string.IsNullOrEmpty(value))
        {
            if (_fields.ContainsKey(name))
            {
                _fields[name] = value;
            }
            else
            {
                _fields.Add(name, value);
            }
        }
        else
        {
            if (_fields.ContainsKey(name))
            {
                _fields.Remove(name);
            }
        }
    }

    internal Uri AppendProfile(Uri uri)
    {
        var value = Transport();

        return AppendQueryParam(uri, QueryParamName, value);
    }

    internal static LoginProfile FromError(Signature signature, InstanceCrypto instanceCrypto, Exception e)
    {
        var profile = new LoginProfile(signature, instanceCrypto) { AuthorizationError = e.Message };

        return profile;
    }

    internal Uri AppendCacheProfile(Uri uri, IMemoryCache memoryCache)
    {
        //gen key
        var key = HashHelper.MD5(Transport());
        memoryCache.Set(key, this, TimeSpan.FromMinutes(15));

        return AppendQueryParam(uri, QueryCacheParamName, key);
    }

    internal void ParseFromUrl(HttpContext context, Uri uri, IMemoryCache memoryCache)
    {
        var queryString = HttpUtility.ParseQueryString(uri.Query);
        if (!string.IsNullOrEmpty(queryString[QueryParamName]))
        {
            FromTransport(queryString[QueryParamName]);
        }
        else if (!string.IsNullOrEmpty(queryString[QueryCacheParamName]))
        {
            FromTransport((string)memoryCache.Get(queryString[QueryCacheParamName]));
        }
    }

    internal string ToSerializedString()
    {
        return string.Join(new string(PairSeparator, 1), _fields.Select(x => string.Join(new string(KeyValueSeparator, 1), new[] { x.Key, x.Value })).ToArray());
    }

    internal static LoginProfile CreateFromSerializedString(Signature signature, InstanceCrypto instanceCrypto, string serialized)
    {
        var profile = new LoginProfile(signature, instanceCrypto);
        profile.FromSerializedString(serialized);

        return profile;
    }

    internal void FromSerializedString(string serialized)
    {
        ArgumentNullException.ThrowIfNull(serialized);

        _fields = serialized.Split(PairSeparator).ToDictionary(x => x.Split(KeyValueSeparator)[0], y => y.Split(KeyValueSeparator)[1]);
    }

    internal string Transport()
    {
        return WebEncoders.Base64UrlEncode(_instanceCrypto.Encrypt(Encoding.UTF8.GetBytes(ToSerializedString())));
    }

    internal void FromTransport(string transportstring)
    {
        var serialized = _instanceCrypto.Decrypt(WebEncoders.Base64UrlDecode(transportstring));
        FromSerializedString(serialized);
    }

    internal LoginProfile(Signature signature, InstanceCrypto instanceCrypto)
    {
        _signature = signature;
        _instanceCrypto = instanceCrypto;
    }

    protected LoginProfile(Signature signature, InstanceCrypto instanceCrypto, SerializationInfo info) : this(signature, instanceCrypto)
    {
        ArgumentNullException.ThrowIfNull(info);

        var transformed = (string)info.GetValue(QueryParamName, typeof(string));
        FromTransport(transformed);
    }

    private static Uri AppendQueryParam(Uri uri, string keyvalue, string value)
    {
        var queryString = HttpUtility.ParseQueryString(uri.Query);
        if (!string.IsNullOrEmpty(queryString[keyvalue]))
        {
            queryString[keyvalue] = value;
        }
        else
        {
            queryString.Add(keyvalue, value);
        }
        var query = new StringBuilder();

        foreach (var key in queryString.AllKeys)
        {
            query.Append($"{key}={queryString[key]}&");
        }

        var builder = new UriBuilder(uri) { Query = query.ToString().TrimEnd('&') };

        return builder.Uri;
    }
}
