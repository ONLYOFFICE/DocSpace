namespace ASC.Web.Studio.Core;

public class OpensourceGiftSettings : ISettings
{
    public bool Readed { get; set; }

    #region ISettings Members

    public Guid ID
    {
        get { return new Guid("{1F4FEA2C-2D9F-47A6-ADEF-CEC4D1E1E243}"); }
    }

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return new OpensourceGiftSettings { Readed = false };
    }

    #endregion
}