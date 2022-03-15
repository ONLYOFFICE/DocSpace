namespace ASC.Web.Core.Mobile
{
    [Scope(typeof(MobileAppInstallRegistrator), typeof(CachedMobileAppInstallRegistrator))]
    public interface IMobileAppInstallRegistrator
    {
        void RegisterInstall(string userEmail, MobileAppType appType);
        bool IsInstallRegistered(string userEmail, MobileAppType? appType);
    }
}
