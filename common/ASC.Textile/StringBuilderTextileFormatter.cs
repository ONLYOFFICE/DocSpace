namespace Textile;

public class StringBuilderTextileFormatter : IOutputter
{
    private StringBuilder _stringBuilder = null;

    public StringBuilderTextileFormatter()
    {
    }

    public string GetFormattedText()
    {
        return _stringBuilder.ToString();
    }

    #region IOutputter Members

    public void Begin()
    {
        _stringBuilder = new StringBuilder();
    }

    public void End()
    {
    }

    public void Write(string text)
    {
        _stringBuilder.Append(text);
    }

    public void WriteLine(string line)
    {
        _stringBuilder.AppendLine(line);
    }

    #endregion
}
