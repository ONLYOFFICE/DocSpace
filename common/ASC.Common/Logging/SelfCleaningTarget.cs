using LogLevel = NLog.LogLevel;

namespace ASC.Common.Logging;

[Target("SelfCleaning")]
public class SelfCleaningTarget : FileTarget
{
    private static DateTime _lastCleanDate;
    private static int? _cleanPeriod;

    protected override void Write(IList<AsyncLogEventInfo> logEvents)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        var buffer = new List<AsyncLogEventInfo>();

        foreach (var logEvent in logEvents)
        {
            buffer.Add(logEvent);
            if (buffer.Count < 10)
            {
                continue;
            }

            base.Write(buffer);
            buffer.Clear();
        }

        base.Write(buffer);
    }

    protected override void Write(LogEventInfo logEvent)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        base.Write(logEvent);
    }

    private static int GetCleanPeriod()
    {
        if (_cleanPeriod != null)
        {
            return _cleanPeriod.Value;
        }

        var value = 30;

        const string key = "cleanPeriod";

            if (LogManager.Configuration.Variables.TryGetValue(key, out var variable))
        {
            if (variable != null && !string.IsNullOrEmpty(variable.Text))
            {
                int.TryParse(variable.Text, out value);
            }
        }

        _cleanPeriod = value;

        return value;
    }

    private void Clean()
    {
        var filePath = string.Empty;
        var dirPath = string.Empty;

        try
        {
            if (FileName == null)
            {
                return;
            }

            filePath = ((NLog.Layouts.SimpleLayout)FileName).Text;
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            dirPath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(dirPath))
            {
                return;
            }
            if (!Path.IsPathRooted(dirPath))
            {
                dirPath = CrossPlatform.PathCombine(AppDomain.CurrentDomain.BaseDirectory, dirPath);
            }

            var directory = new DirectoryInfo(dirPath);
            if (!directory.Exists)
            {
                return;
            }

            var files = directory.GetFiles();
            var cleanPeriod = GetCleanPeriod();

            foreach (var file in files.Where(file => (DateTime.UtcNow.Date - file.CreationTimeUtc.Date).Days > cleanPeriod))
            {
                file.Delete();
            }
        }
        catch (Exception err)
        {
            base.Write(new LogEventInfo
            {
                Exception = err,
                Level = LogLevel.Error,
                    Message = $"file: {filePath}, dir: {dirPath}, mess: {err.Message}",
                LoggerName = "SelfCleaningTarget"
            });
        }
    }
}
