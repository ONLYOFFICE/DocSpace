using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.Data.Storage;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ASC.Thumbnails.Svc
{
    public interface IThumbnailHelper
    {
        void MakeThumbnail(string url, bool async, bool notOverride, int tenantID);
        string GetThumbnailUrl(string Url, BookmarkingThumbnailSize size);
        string GetThumbnailUrlForUpdate(string Url, BookmarkingThumbnailSize size);
        void DeleteThumbnail(string Url);
    }

    public class ThumbnailHelper
    {
        private IThumbnailHelper Helper { get; set; }
        private IConfiguration Configuration { get; set; }

        public ThumbnailHelper(WebSiteThumbnailHelper webSiteThumbnailHelper, ServiceThumbnailHelper serviceThumbnailHelper, NullThumbnailHelper nullThumbnailHelper, IConfiguration configuration)
        {
            Configuration = configuration;
            if (HasService)
            {
                Helper =  serviceThumbnailHelper;
            }
            if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.Xbox)
            {
                Helper = nullThumbnailHelper;
            }
            Helper = webSiteThumbnailHelper;
        }


        public bool HasService
        {
            get { return Configuration["bookmarking:thumbnail-url"] != null; }
        }

        public string ServiceUrl
        {
            get { return Configuration["bookmarking:thumbnail-url"]; }
        }
/*
        public static IThumbnailHelper Instance
        {
            get
            {
                if (HasService)
                {
                    return ServiceHelper;
                }
                if (Environment.OSVersion.Platform == PlatformID.MacOSX ||
                    Environment.OSVersion.Platform == PlatformID.Unix ||
                    Environment.OSVersion.Platform == PlatformID.Xbox)
                {
                    return NullHelper;
                }
                return ProcessHelper;
            }
        }*/
    }

    public class ServiceThumbnailHelper : IThumbnailHelper
    {
        private readonly string CoreMachineKey;

        public ServiceThumbnailHelper()
        {
            CoreMachineKey = ConfigurationManager.AppSettings["core:machinekey"];
        }

        public void MakeThumbnail(string url, bool async, bool notOverride, int tenantID)
        {

        }

        public string GetThumbnailUrl(string Url, BookmarkingThumbnailSize size)
        {
            return string.Format("/thumb.ashx?url={0}", Url);
        }

        public string GetThumbnailUrlForUpdate(string Url, BookmarkingThumbnailSize size)
        {
            return GetThumbnailUrl(Url, size);
        }

        public void DeleteThumbnail(string Url)
        {

        }
    }

    public class NullThumbnailHelper : IThumbnailHelper
    {
        public void MakeThumbnail(string url, bool async, bool notOverride,  int tenantID)
        {
        }

        public void DeleteThumbnail(string Url)
        {
        }

        public string GetThumbnailUrl(string Url, BookmarkingThumbnailSize size)
        {
            return null;
        }

        public string GetThumbnailUrlForUpdate(string Url, BookmarkingThumbnailSize size)
        {
            return GetThumbnailUrl(Url, size);
        }
    }

    public class WebSiteThumbnailHelper : IThumbnailHelper
    {
        private List<Uri> ProcessedUrls { get; set; }
        private TenantManager TenantManager { get; set;}
        private StorageFactory StorageFactory { get; set;}
        private IHttpContextAccessor HttpContextAccessor { get; set;}
        public WebSiteThumbnailHelper(TenantManager tenantManager, StorageFactory storageFactory, IHttpContextAccessor httpContextAccessor)
        {
            ProcessedUrls = new List<Uri>();
            TenantManager = tenantManager;
            StorageFactory = storageFactory;
            HttpContextAccessor = httpContextAccessor;
        }

        private IDataStore getStore()
        {
            return StorageFactory.GetStorage(TenantManager.CurrentTenant.TenantId.ToString(), BookmarkingRequestConstants.BookmarkingStorageManagerID);
        }

        private IDataStore getStore(int tenant)
        {
            return StorageFactory.GetStorage(tenant.ToString(), BookmarkingRequestConstants.BookmarkingStorageManagerID);
        }


        public void MakeThumbnail(string url)
        {
            MakeThumbnail(url, true, true, TenantManager.CurrentTenant.TenantId);
        }

        public void MakeThumbnail(string url, bool async, bool notOverride, int tenantID)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return;

                if (notOverride)
                {
                    var fileName = GetFileNameByUrl(HttpUtility.UrlEncode(url), BookmarkingSettings.ThumbSmallSize);
                    if (getStore(tenantID).IsFile(string.Empty, fileName))
                    {
                        return;
                    }
                }

                List<object> p = new List<object>();
                p.Add(url);
                p.Add(HttpContextAccessor.HttpContext);
                p.Add(tenantID);
                ThreadPool.QueueUserWorkItem(MakeThumbnailCallback, p);

                //if (!async) thread.Join();

            }
            catch { }
        }

        private void MakeThumbnailCallback(object p)
        {
            #region Sanity Check

            var url = string.Empty;
            var context = HttpContextAccessor.HttpContext;
            int tenant = 0;

            try
            {
                if (p is List<Object>)
                {
                    var s = p as List<object>;
                    if (s.Count == 3)
                    {
                        if (s[0] is string)
                        {
                            url = s[0] as string;
                        }
                        if (s[1] is HttpContext)
                        {
                            context = s[1] as HttpContext;
                        }
                        if (s[2] is int)
                        {
                            tenant = (int)s[2];
                        }
                    }
                }
            }
            catch { }

            #endregion

            var outFileName = string.Empty;
            Process ps = null;
            int psid = -1;
            Uri uri = null;
            try
            {
                //Check true url
                if (!string.IsNullOrEmpty(url) && context != null && Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    lock (ProcessedUrls)
                    {
                        if (ProcessedUrls.Contains(uri)) return;//Screen ih bin processing go away!

                        ProcessedUrls.Add(uri);

                    }
                    //We got normal url
                    //Map server path
                    var appDataDir = VirtualPathUtility.ToAbsolute("~/Products/Community/Modules/Bookmarking/App_Data");
                    var screenShoterName = Path.Combine(appDataDir, "IECapt.exe");

                    if (File.Exists(screenShoterName))
                    {
                        outFileName = Path.Combine(appDataDir, Path.Combine("screens", Guid.NewGuid() + ".png"));
                        if (!Directory.Exists(Path.GetDirectoryName(outFileName)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
                        }
                        var arguments = BuildArguments(uri, outFileName);
                        //Launch process
                        using (ps = new Process())
                        {
                            ps.StartInfo = new ProcessStartInfo(screenShoterName, arguments);
                            ps.StartInfo.CreateNoWindow = true;
                            ps.Start();
                            psid = ps.Id;
                            if (ps.WaitForExit(15000))//Wait 15 sec and close 
                            {
                                //Ta da. File created
                                if (File.Exists(outFileName))
                                {
                                    //Upload!
                                    //Warning! Huge memory overhead!
                                    using (Image image = Image.FromFile(outFileName))
                                    {
                                        using (
                                            Image clipImage = new Bitmap(BookmarkingSettings.ThumbSmallSize.Width,
                                                                         BookmarkingSettings.ThumbSmallSize.Height))
                                        {
                                            using (var graphics = Graphics.FromImage(clipImage))
                                            {
                                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                                //Bicubic is better for minimizing image.
                                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                                graphics.DrawImage(image,
                                                                   Rectangle.FromLTRB(0, 0,
                                                                                      BookmarkingSettings.ThumbSmallSize
                                                                                          .
                                                                                          Width,
                                                                                      BookmarkingSettings.ThumbSmallSize
                                                                                          .
                                                                                          Height),
                                                                   Rectangle.FromLTRB(0, 0,
                                                                                      BookmarkingSettings.BrowserSize.
                                                                                          Width,
                                                                                      BookmarkingSettings.BrowserSize.
                                                                                          Height),
                                                                   GraphicsUnit.Pixel
                                                    );
                                                using (var ms = new MemoryStream())
                                                {
                                                    clipImage.Save(ms, BookmarkingSettings.CaptureImageFormat);
                                                    ms.Position = 0;
                                                    IDataStore store = getStore( tenant);
                                                    var fileName = GetFileNameByUrl(HttpUtility.UrlEncode(url), BookmarkingSettings.ThumbSmallSize);
                                                    store.Save(string.Empty, fileName, ms);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Process hasn't exited
                                //finally will kill it
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (ps != null)
                {
                    try
                    {
                        ps.Kill();
                    }
                    catch
                    {
                        //Don't throw
                    }
                }
                //try kill
                if (psid != -1)
                {
                    try
                    {
                        var proc = Process.GetProcessById(psid);
                        if (proc != null)
                        {
                            proc.Kill();
                        }
                    }
                    catch
                    {
                        //Don't throw
                    }
                }
                if (!string.IsNullOrEmpty(outFileName) && File.Exists(outFileName))
                {
                    File.Delete(outFileName);
                }

                lock (ProcessedUrls)
                {
                    if (uri != null && ProcessedUrls.Contains(uri))
                    {
                        ProcessedUrls.Remove(uri);
                    }
                }
            }

        }


        private static string BuildArguments(Uri uri, string outFileName)
        {
            return string.Format("--url=\"{0}\" --out=\"{1}\" --delay={2} --max-wait={4} --min-width={3} --silent ",
                        uri, outFileName, 1000, BookmarkingSettings.BrowserSize.Width, 10000);

        }


        public string GetThumbnailUrl(string Url, BookmarkingThumbnailSize size)
        {
            var fileName = GetFileNameByUrl(Url, size);

            return getStore().IsFile(string.Empty, fileName) ? getStore().GetUri(string.Empty, fileName).ToString() : string.Empty;
        }

        public string GetThumbnailUrlForUpdate(string Url, BookmarkingThumbnailSize size)
        {
            return GetThumbnailUrl(Url, size);
        }

        #region DeleteThumbnail
        public void DeleteThumbnail(string Url)
        {
            try
            {
                var fileName = GetFileNameByUrl(Url, BookmarkingSettings.ThumbSmallSize);
                getStore(TenantManager.CurrentTenant.TenantId).Delete(fileName);
            }
            catch (FileNotFoundException) { }
        }

        #endregion

        private static string GetSHA256(string text)
        {
            var ue = new UnicodeEncoding();
            var message = ue.GetBytes(text);

            var hashString = new SHA256Managed();
            var hashValue = hashString.ComputeHash(message);

            var hex = new StringBuilder();
            foreach (byte x in hashValue) hex.AppendFormat("{0:x2}", x);
            return hex.ToString();
        }

        private static string GetFileNameByUrl(string url, BookmarkingThumbnailSize size)
        {
            string sizeString = size == null ? string.Empty : size.ToString();
            return string.Format("{0}{1}.{2}", GetSHA256(url), sizeString, BookmarkingSettings.CaptureImageFormat.ToString());
        }
    }


    public static class ThumbnailHelperExtension
    {
        public static DIHelper AddThumbnailHelperService(this DIHelper services)
        {
            services.TryAddScoped<ThumbnailHelper>();
            services.TryAddScoped<WebSiteThumbnailHelper>();
            services.TryAddScoped<ServiceThumbnailHelper>();
            services.TryAddScoped<NullThumbnailHelper>();
            return services
                .AddTenantManagerService()
                .AddStorageFactoryService();
        }
    }
}
