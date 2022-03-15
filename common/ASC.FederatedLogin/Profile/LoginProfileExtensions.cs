namespace ASC.FederatedLogin.Profile;

public static class LoginProfileExtensions
{
    public static Uri AddProfile(this Uri uri, LoginProfile profile)
    {
        return profile.AppendProfile(uri);
    }

    public static Uri AddProfileSession(this Uri uri, LoginProfile profile, Microsoft.AspNetCore.Http.HttpContext context)
    {
        return profile.AppendSessionProfile(uri, context);
    }

    public static Uri AddProfileCache(this Uri uri, LoginProfile profile, IMemoryCache memoryCache)
    {
        return profile.AppendCacheProfile(uri, memoryCache);
    }

    public static LoginProfile GetProfile(this Uri uri, HttpContext context, IMemoryCache memoryCache, Signature signature, InstanceCrypto instanceCrypto)
    {
        var profile = new LoginProfile(signature, instanceCrypto);
        var queryString = HttpUtility.ParseQueryString(uri.Query);
        if (!string.IsNullOrEmpty(queryString[LoginProfile.QuerySessionParamName]) && context != null && context.Session != null)
        {
            return JsonConvert.DeserializeObject<LoginProfile>(context.Session.GetString(queryString[LoginProfile.QuerySessionParamName]));
        }
        if (!string.IsNullOrEmpty(queryString[LoginProfile.QueryParamName]))
        {
            profile.ParseFromUrl(context, uri, memoryCache);
            return profile;
        }
        if (!string.IsNullOrEmpty(queryString[LoginProfile.QueryCacheParamName]))
        {
            return (LoginProfile)memoryCache.Get(queryString[LoginProfile.QuerySessionParamName]);
        }

        return null;
    }

    public static bool HasProfile(this Uri uri)
    {
        var queryString = HttpUtility.ParseQueryString(uri.Query);

        return !string.IsNullOrEmpty(queryString[LoginProfile.QueryParamName])
            || !string.IsNullOrEmpty(queryString[LoginProfile.QuerySessionParamName])
            || !string.IsNullOrEmpty(queryString[LoginProfile.QueryCacheParamName]);
    }
}
