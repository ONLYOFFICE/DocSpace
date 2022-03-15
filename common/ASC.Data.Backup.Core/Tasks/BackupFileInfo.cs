namespace ASC.Data.Backup.Tasks;

public class BackupFileInfo
{
    public string Domain { get; set; }
    public string Module { get; set; }
    public string Path { get; set; }
    public int Tenant { get; set; }

    public BackupFileInfo() { }

    public BackupFileInfo(string domain, string module, string path, int tenant = -1)
    {
        Domain = domain;
        Module = module;
        Path = path;
        Tenant = tenant;
    }

    public XElement ToXElement()
    {
        var xElement = new XElement("file",
                            new XElement("domain", Domain),
                            new XElement("module", Module),
                            new XElement("path", Path));

        if (Tenant != -1)
        {
            xElement.Add(new XElement("tenant", Tenant));
        }

        return xElement;
    }

    public static BackupFileInfo FromXElement(XElement el)
    {
        return new BackupFileInfo
        {
            Domain = el.Element("domain").ValueOrDefault(),
            Module = el.Element("module").ValueOrDefault(),
            Path = el.Element("path").ValueOrDefault(),
            Tenant = Convert.ToInt32(el.Element("tenant").ValueOrDefault() ?? "-1")
        };
    }
}
