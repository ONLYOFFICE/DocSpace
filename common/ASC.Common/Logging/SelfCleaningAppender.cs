namespace ASC.Common.Logging;

public class SelfCleaningAppender : RollingFileAppender
{
    private static DateTime _lastCleanDate;
    private static int? _cleanPeriod;

    protected override void Append(LoggingEvent loggingEvent)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        base.Append(loggingEvent);
    }

    protected override void Append(LoggingEvent[] loggingEvents)
    {
        if (DateTime.UtcNow.Date > _lastCleanDate.Date)
        {
            _lastCleanDate = DateTime.UtcNow.Date;
            Clean();
        }

        base.Append(loggingEvents);
    }

    private static int GetCleanPeriod()
    {
        if (_cleanPeriod != null)
        {
            return _cleanPeriod.Value;
        }

        const string key = "CleanPeriod";

        var value = 30;

        var repo = log4net.LogManager.GetRepository(Assembly.GetCallingAssembly());
        if (repo != null && repo.Properties.GetKeys().Contains(key))
        {
            int.TryParse(repo.Properties[key].ToString(), out value);
        }

        _cleanPeriod = value;

        return value;
    }

    private void Clean()
    {
        try
        {
            if (string.IsNullOrEmpty(File))
            {
                return;
            }

            var fileInfo = new FileInfo(File);
            if (!fileInfo.Exists)
            {
                return;
            }

            var directory = fileInfo.Directory;
            if (directory == null || !directory.Exists)
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
            LogLog.Error(GetType(), err.Message, err);
        }
    }
}
