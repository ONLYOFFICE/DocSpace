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


using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;

using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace TMResourceData
{
    public class DBResourceManager : ResourceManager
    {
        public static bool WhiteLableEnabled = false;
        private readonly ConcurrentDictionary<string, ResourceSet> resourceSets = new ConcurrentDictionary<string, ResourceSet>();

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
            Configuration = configuration;
            Option = option;
            DbContext = dbContext;
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
                var types = new Type[0];
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
            return (n.StartsWith("ASC.") || n.StartsWith("App_GlobalResources")) && a.GetManifestResourceNames().Any();
        }


        public override Type ResourceSetType
        {
            get { return typeof(DBResourceSet); }
        }

        public IConfiguration Configuration { get; }
        public IOptionsMonitor<ILog> Option { get; }
        public DbContextManager<ResourceDbContext> DbContext { get; }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            resourceSets.TryGetValue(culture.Name, out var set);
            if (set == null)
            {
                var invariant = culture == CultureInfo.InvariantCulture ? base.InternalGetResourceSet(CultureInfo.InvariantCulture, true, true) : null;
                set = new DBResourceSet(Configuration, Option, DbContext, invariant, culture, BaseName);
                resourceSets.AddOrUpdate(culture.Name, set, (k, v) => set);
            }
            return set;
        }


        class DBResourceSet : ResourceSet
        {
            private const string NEUTRAL_CULTURE = "Neutral";

            private readonly TimeSpan cacheTimeout = TimeSpan.FromMinutes(120); // for performance
            private readonly object locker = new object();
            private readonly MemoryCache cache;
            private readonly ResourceSet invariant;
            private readonly string culture;
            private readonly string filename;
            private readonly ILog log;

            public IConfiguration Configuration { get; }
            public IOptionsMonitor<ILog> Option { get; }
            public DbContextManager<ResourceDbContext> DbContext { get; }

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
                    throw new ArgumentNullException("culture");
                }
                if (string.IsNullOrEmpty(filename))
                {
                    throw new ArgumentNullException("filename");
                }

                Configuration = configuration;
                Option = option;
                DbContext = dbContext;
                log = option.CurrentValue;

                try
                {
                    var defaultValue = ((int)cacheTimeout.TotalMinutes).ToString();
                    cacheTimeout = TimeSpan.FromMinutes(Convert.ToInt32(configuration["resources:cache-timeout"] ?? defaultValue));
                }
                catch (Exception err)
                {
                    log.Error(err);
                }

                this.invariant = invariant;
                this.culture = invariant != null ? NEUTRAL_CULTURE : culture.Name;
                this.filename = filename.Split('.').Last() + ".resx";
                cache = MemoryCache.Default;
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
                    log.ErrorFormat("Can not get resource from {0} for {1}: GetString({2}), {3}", filename, culture, name, err);
                }

                if (invariant != null && result == null)
                {
                    result = invariant.GetString(name, ignoreCase);
                }

                return result;
            }

            public override IDictionaryEnumerator GetEnumerator()
            {
                var result = new Hashtable();
                if (invariant != null)
                {
                    foreach (DictionaryEntry e in invariant)
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
                    log.Error(err);
                }
                return result.GetEnumerator();
            }

            private Dictionary<string, string> GetResources()
            {
                var key = string.Format("{0}/{1}", filename, culture);
                if (!(cache.Get(key) is Dictionary<string, string> dic))
                {
                    lock (locker)
                    {
                        dic = cache.Get(key) as Dictionary<string, string>;
                        if (dic == null)
                        {
                            var policy = cacheTimeout == TimeSpan.Zero ? null : new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.Add(cacheTimeout) };
                            cache.Set(key, dic = LoadResourceSet(filename, culture), policy);
                        }
                    }
                }
                return dic;
            }

            private Dictionary<string, string> LoadResourceSet(string filename, string culture)
            {
                using var context = DbContext.Get("tmresource");
                var q = context.ResData
                    .Where(r => r.CultureTitle == culture)
                    .Join(context.ResFiles, r => r.FileId, a => a.Id, (d, f) => new { data = d, files = f })
                    .Where(r => r.files.ResName == filename);

                return q
                    .ToDictionary(r => r.data.Title, r => r.data.TextValue, StringComparer.InvariantCultureIgnoreCase);
            }
        }
    }

    public class WhiteLabelHelper
    {
        private readonly ILog log;
        private readonly ConcurrentDictionary<int, string> whiteLabelDictionary;
        public string DefaultLogoText;

        public IConfiguration Configuration { get; }

        public WhiteLabelHelper(IConfiguration configuration, IOptionsMonitor<ILog> option)
        {
            log = option.Get("ASC.Resources");
            whiteLabelDictionary = new ConcurrentDictionary<int, string>();
            DefaultLogoText = "";
            Configuration = configuration;
        }

        public void SetNewText(int tenantId, string newText)
        {
            try
            {
                whiteLabelDictionary.AddOrUpdate(tenantId, r => newText, (i, s) => newText);
            }
            catch (Exception e)
            {
                log.Error("SetNewText", e);
            }
        }

        public void RestoreOldText(int tenantId)
        {
            try
            {
                whiteLabelDictionary.TryRemove(tenantId, out var text);
            }
            catch (Exception e)
            {
                log.Error("RestoreOldText", e);
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
                    if (tenant == null) return resourceValue;

                    if (whiteLabelDictionary.TryGetValue(tenant.TenantId, out var newText))
                    {
                        var newTextReplacement = newText;

                        if (resourceValue.Contains("<") && resourceValue.Contains(">") || resourceName.StartsWith("pattern_"))
                        {
                            newTextReplacement = HttpUtility.HtmlEncode(newTextReplacement);
                        }
                        if (resourceValue.Contains("{0"))
                        {
                            //Hack for string which used in string.Format
                            newTextReplacement = newTextReplacement.Replace("{", "{{").Replace("}", "}}");
                        }

                        var replPattern = Configuration["resources:whitelabel-text.replacement.pattern"] ?? "(?<=[^@/\\\\]|^)({0})(?!\\.com)";
                        var pattern = string.Format(replPattern, DefaultLogoText);
                        //Hack for resource strings with mails looked like ...@onlyoffice... or with website http://www.onlyoffice.com link or with the https://www.facebook.com/pages/OnlyOffice/833032526736775

                        return Regex.Replace(resourceValue, pattern, newTextReplacement, RegexOptions.IgnoreCase).Replace("™", "");
                    }
                }
                catch (Exception e)
                {
                    log.Error("ReplaceLogo", e);
                }
            }

            return resourceValue;
        }
    }

    public static class WhiteLabelHelperExtension
    {
        public static IServiceCollection AddWhiteLabelHelperService(this IServiceCollection services)
        {
            services.TryAddSingleton<WhiteLabelHelper>();
            return services;
        }
    }
}
