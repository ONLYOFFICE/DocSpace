namespace ASC.Common.Logging;

public class SpecialFolderPathConverter : PatternConverter
{
    protected override void Convert(TextWriter writer, object state)
    {
        if (string.IsNullOrEmpty(Option))
        {
            return;
        }

        try
        {
            var result = string.Empty;
            const string CMD_LINE = "CommandLine:";

            if (Option.StartsWith(CMD_LINE))
            {
                var args = Environment.CommandLine.Split(' ');
                for (var i = 0; i < args.Length - 1; i++)
                {
                        if (args[i].Contains(Option, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = args[i + 1];
                    }
                }
            }
            else
            {
                var repo = log4net.LogManager.GetRepository(Assembly.GetCallingAssembly());
                if (repo != null)
                {
                    var realKey = Option;
                    foreach (var key in repo.Properties.GetKeys())
                    {
                        if (Path.DirectorySeparatorChar == '/' && key == "UNIX:" + Option)
                        {
                            realKey = "UNIX:" + Option;
                        }

                        if (Path.DirectorySeparatorChar == '\\' && key == "WINDOWS:" + Option)
                        {
                            realKey = "WINDOWS:" + Option;
                        }
                    }

                    var val = repo.Properties[realKey];
                    if (val is PatternString patternString)
                    {
                        patternString.ActivateOptions();
                        patternString.Format(writer);
                    }
                    else if (val != null)
                    {
                        result = val.ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                writer.Write(result);
            }
        }
        catch (Exception err)
        {
            LogLog.Error(GetType(), "Can not convert " + Option, err);
        }
    }
}