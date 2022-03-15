namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public interface IModule : IWebItem
    {
        Guid ProjectId { get; }

        string ModuleSysName { get; }

        new ModuleContext Context { get; }
    }
}
