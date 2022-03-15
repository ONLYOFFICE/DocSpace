namespace ASC.TelegramService.Core;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string name)
    {
        Name = name.ToLowerInvariant();
    }

    public string Name { get; private set; }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class ParamParserAttribute : Attribute
{
    public ParamParserAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; private set; }
}
