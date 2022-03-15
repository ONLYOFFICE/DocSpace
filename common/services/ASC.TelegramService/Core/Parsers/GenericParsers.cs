namespace ASC.TelegramService.Core.Parsers;

[ParamParser(typeof(int))]
public class IntParser : ParamParser<int>
{
    public override object FromString(string arg)
    {
        return int.Parse(arg);
    }

    public override string ToString(object arg)
    {
        return arg.ToString();
    }
}