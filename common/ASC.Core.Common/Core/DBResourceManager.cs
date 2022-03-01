/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace TMResourceData;

public class DBResourceManager : ResourceManager
{
    public static readonly bool WhiteLableEnabled = false;
    private readonly ConcurrentDictionary<string, ResourceSet> _resourceSets = new ConcurrentDictionary<string, ResourceSet>();

    public DBResourceManager(string filename, Assembly assembly)
                : base(filename, assembly)
    {
    }

    public DBResourceManager(
        IConfiguration configuration,
        IOptionsMonitor<ILog> option,
        DbContextManager<ResourceDbContext> dbContext,
        string filename,
        Assembly assembly)
                : base(filename, assembly)
    {
        _configuration = configuration;
        _option = option;
        _dbContext = dbContext;
    }

    public static void PatchAssemblies(IOptionsMonitor<ILog> option)
    {
        AppDomain.CurrentDomain.AssemblyLoad += (_, a) => PatchAssembly(option, a.LoadedAssembly);
        Array.ForEach(AppDomain.CurrentDomain.GetAssemblies(), a => PatchAssembly(option, a));
    }

    public static void PatchAssembly(IOptionsMonitor<ILog> option, Assembly a, bool onlyAsc = true)
    {
        var log = option.CurrentValue;

        if (!onlyAsc || Accept(a))
        {
            var types = Array.Empty<Type>();
            try
            {
                types = a.GetTypes();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                log.WarnFormat("Can not GetTypes() from assembly {0}, try GetExportedTypes(), error: {1}", a.FullName, rtle.Message);
                foreach (var e in rtle.LoaderExceptions)
                {
                    log.Info(e.Message);
                }

                try
                {
                    types = a.GetExportedTypes();
                }
                catch (Exception err)
                {
                    log.ErrorFormat("Can not GetExportedTypes() from assembly {0}: {1}", a.FullName, err.Message);
                }
            }
            foreach (var type in types)
            {
                var prop = type.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                if (prop != null)
                {
                    var rm = (ResourceManager)prop.GetValue(type);
                    if (!(rm is DBResourceManager))
                    {
                        var dbrm = new DBResourceManager(rm.BaseName, a);
                        type.InvokeMember("resourceMan", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, type, new object[] { dbrm });
                    }
                }
            }
        }
    }

    private static bool Accept(Assembly a)
    {
        var n = a.GetName().Name;

        return (n.StartsWith("ASC.") || n.StartsWith("App_GlobalResources")) && a.GetManifestResourceNames().Length > 0;
    }

    public override Type ResourceSetType => typeof(DBResourceSet);

    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<ILog> _option;
    private readonly DbContextManager<ResourceDbContext> _dbContext;

    protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
    {
        _resourceSets.TryGetValue(culture.Name, out var set);
        if (set == null)
        {
            var invariant = culture == CultureInfo.InvariantCulture ? base.InternalGetResourceSet(CultureInfo.InvariantCulture, true, true) : null;
            set = new DBResourceSet(_configuration, _option, _dbContext, invariant, culture, BaseName);
            _resourceSets.AddOrUpdate(culture.Name, set, (k, v) => set);
        }

        return set;
    }

    class DBResourceSet : ResourceSet
    {
        private const string NeutralCulture = "Neutral";

        private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(120); // for performance
        private readonly object _locker = new object();
        private readonly MemoryCache _cache;
        private readonly ResourceSet _invariant;
        private readonly string _culture;
        private readonly string _fileName;
        private readonly ILog _logger;
        private readonly DbContextManager<ResourceDbContext> _dbContext;

        public DBResourceSet(
            IConfiguration configuration,
            IOptionsMonitor<ILog> option,
            DbContextManager<ResourceDbContext> dbContext,
            ResourceSet invariant,
            CultureInfo culture,
            string filename)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            _dbContext = dbContext;
            _logger = option.CurrentValue;

            try
            {
                var defaultValue = ((int)_cacheTimeout.TotalMinutes).ToString();
                _cacheTimeout = TimeSpan.FromMinutes(Convert.ToInt32(configuration["resources:cache-timeout"] ?? defaultValue));
            }
            catch (Exception err)
            {
                _logger.Error(err);
            }

            _invariant = invariant;
            _culture = invariant != null ? NeutralCulture : culture.Name;
            _fileName = filename.Split('.').Last() + ".resx";
            _cache = MemoryCache.Default;
        }

        public override string GetString(string name, bool ignoreCase)
        {
            var result = (string)null;
            try
            {
                var dic = GetResources();
                dic.TryGetValue(name, out result);
            }
            catch (Exception err)
            {
                _logger.ErrorFormat("Can not get resource from {0} for {1}: GetString({2}), {3}", _fileName, _culture, name, err);
            }

            if (_invariant != null && result == null)
            {
                result = _invariant.GetString(name, ignoreCase);
            }

            return result;
        }

        public override IDictionaryEnumerator GetEnumerator()
        {
            var result = new Hashtable();
            if (_invariant != null)
            {
                foreach (DictionaryEntry e in _invariant)
                {
                    result[e.Key] = e.Value;
                }
            }
            try
            {
                var dic = GetResources();
                foreach (var e in dic)
                {
                    result[e.Key] = e.Value;
                }
            }
            catch (Exception err)
            {
                _logger.Error(err);
            }

            return result.GetEnumerator();
        }

        private Dictionary<string, string> GetResources()
        {
            var key = $"{_fileName}/{_culture}";
            if (!(_cache.Get(key) is Dictionary<string, string> dic))
            {
                lock (_locker)
                {
                    dic = _cache.Get(key) as Dictionary<string, string>;
                    if (dic == null)
                    {
                        var policy = _cacheTimeout == TimeSpan.Zero ? null : new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.Add(_cacheTimeout) };
                        dic = LoadResourceSet(_fileName, _culture);
                        _cache.Set(key, dic, policy);
                    }
                }
            }

            return dic;
        }

        private Dictionary<string, string> LoadResourceSet(string filename, string culture)
        {
            using var context = _dbContext.Get("tmresource");
            var q = context.ResData
                .Where(r => r.CultureTitle == culture)
                .Join(context.ResFiles, r => r.FileId, a => a.Id, (d, f) => new { data = d, files = f })
                .Where(r => r.files.ResName == filename);

            return q
                .ToDictionary(r => r.data.Title, r => r.data.TextValue, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}

[Singletone]
public class WhiteLabelHelper
{
    private readonly ILog _logger;
    private readonly ConcurrentDictionary<int, string> _whiteLabelDictionary;
    public string DefaultLogoText { get; set; }

    private readonly IConfiguration _configuration;

    public WhiteLabelHelper(IConfiguration configuration, IOptionsMonitor<ILog> option)
    {
        _logger = option.Get("ASC.Resources");
        _whiteLabelDictionary = new ConcurrentDictionary<int, string>();
        DefaultLogoText = string.Empty;
        _configuration = configuration;
    }

    public void SetNewText(int tenantId, string newText)
    {
        try
        {
            _whiteLabelDictionary.AddOrUpdate(tenantId, r => newText, (i, s) => newText);
        }
        catch (Exception e)
        {
            _logger.Error("SetNewText", e);
        }
    }

    public void RestoreOldText(int tenantId)
    {
        try
        {
            _whiteLabelDictionary.TryRemove(tenantId, out var text);
        }
        catch (Exception e)
        {
            _logger.Error("RestoreOldText", e);
        }
    }

    internal string ReplaceLogo(TenantManager tenantManager, IHttpContextAccessor httpContextAccessor, string resourceName, string resourceValue)
    {
        if (string.IsNullOrEmpty(resourceValue))
        {
            return resourceValue;
        }
        if (!DBResourceManager.WhiteLableEnabled)
        {
            return resourceValue;
        }

        if (httpContextAccessor.HttpContext != null) //if in Notify Service or other process without HttpContext
        {
            try
            {
                var tenant = tenantManager.GetCurrentTenant(false);
                if (tenant == null)
                {
                    return resourceValue;
                }

                if (_whiteLabelDictionary.TryGetValue(tenant.Id, out var newText))
                {
                    var newTextReplacement = newText;

                    if (resourceValue.IndexOf('<') >= 0 && resourceValue.IndexOf('>') >= 0 || resourceName.StartsWith("pattern_"))
                    {
                        newTextReplacement = HttpUtility.HtmlEncode(newTextReplacement);
                    }
                    if (resourceValue.Contains("{0"))
                    {
                        //Hack for string which used in string.Format
                        newTextReplacement = newTextReplacement.Replace("{", "{{").Replace("}", "}}");
                    }

                    var replPattern = _configuration["resources:whitelabel-text.replacement.pattern"] ?? "(?<=[^@/\\\\]|^)({0})(?!\\.com)";
                    var pattern = string.Format(replPattern, DefaultLogoText);
                    //Hack for resource strings with mails looked like ...@onlyoffice... or with website http://www.onlyoffice.com link or with the https://www.facebook.com/pages/OnlyOffice/833032526736775

                    return Regex.Replace(resourceValue, pattern, newTextReplacement, RegexOptions.IgnoreCase).Replace("™", "");
                }
            }
            catch (Exception e)
            {
                _logger.Error("ReplaceLogo", e);
            }
        }

        return resourceValue;
    }
}
