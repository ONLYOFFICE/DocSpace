namespace ASC.AuditTrail.Mappers;

internal class MessageMaps
{
    public string ActionTypeTextResourceName { get; set; }
    public string ActionTextResourceName { get; set; }
    public string ProductResourceName { get; set; }
    public string ModuleResourceName { get; set; }

    public string GetActionTypeText()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ActionTypeTextResourceName);
        }
        catch
        {
            return null;
        }
    }

    public string GetActionText()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ActionTextResourceName);
        }
        catch
        {
            return null;
        }
    }

    public string GetProduct()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ProductResourceName);
        }
        catch
        {
            return null;
        }
    }

    public string GetModule()
    {
        try
        {
            return AuditReportResource.ResourceManager.GetString(ModuleResourceName);
        }
        catch
        {
            return null;
        }
    }
}