
using ASC.Common;
using ASC.Common.Utils;

namespace ASC.Webhooks.Service
{
    [Singletone]
    public class Settings
    {
        public Settings()
        {

        }
        public Settings(ConfigurationExtension configuration)
        {
            var cfg = configuration.GetSetting<Settings>("webhooks");
            RepeatCount = cfg.RepeatCount ?? 5;
            ThreadCount = cfg.ThreadCount ?? 1;
        }

        public int? RepeatCount { get; }
        public int? ThreadCount { get; }

    }
}
