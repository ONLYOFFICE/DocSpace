namespace ASC.Common.Utils;

public class TextLoader : ResourceLoader
{
    public override void Init(Commons.Collections.ExtendedProperties configuration)
    {
        //nothing to configure
    }

    public override Stream GetResourceStream(string source)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(source));
    }

    public override long GetLastModified(NVelocity.Runtime.Resource.Resource resource)
    {
        return 1;
    }

    public override bool IsSourceModified(NVelocity.Runtime.Resource.Resource resource)
    {
        return false;
    }
}

public static class VelocityFormatter
{
    private static bool _initialized;
    private static readonly ConcurrentDictionary<string, Template> _patterns = new ConcurrentDictionary<string, Template>();

    public static string FormatText(string templateText, IDictionary<string, object> values)
    {
        var nvelocityContext = new VelocityContext();

        foreach (var tagValue in values)
        {
            nvelocityContext.Put(tagValue.Key, tagValue.Value);
        }

        return FormatText(templateText, nvelocityContext);
    }

    public static string FormatText(string templateText, VelocityContext context)
    {
        if (!_initialized)
        {
            var properties = new Commons.Collections.ExtendedProperties();
            properties.AddProperty("resource.loader", "custom");
            properties.AddProperty("custom.resource.loader.class", "ASC.Common.Utils.TextLoader; ASC.Common");
            properties.AddProperty("input.encoding", Encoding.UTF8.WebName);
            properties.AddProperty("output.encoding", Encoding.UTF8.WebName);
            Velocity.Init(properties);
            _initialized = true;
        }

        using var writer = new StringWriter();
        var key = templateText.GetHashCode().ToString();

        if (!_patterns.TryGetValue(key, out var template))
        {
            template = Velocity.GetTemplate(templateText);
            _patterns.TryAdd(key, template);
        }

        template.Merge(context, writer);

        return writer.GetStringBuilder().ToString();
    }
}
