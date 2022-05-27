using System;

using ASC.Common;
using ASC.Common.Utils;

namespace ASC.Feed.Configuration
{
    [Singletone]
    public class FeedSettings
    {
        private string serverRoot;
        public string ServerRoot { get => serverRoot ?? "http://*/"; set { serverRoot = value; } }

        private TimeSpan aggregatePeriod;
        public TimeSpan AggregatePeriod { get => aggregatePeriod == TimeSpan.Zero ? TimeSpan.FromMinutes(5) : aggregatePeriod; set { aggregatePeriod = value; } }

        private TimeSpan aggregateInterval;
        public TimeSpan AggregateInterval { get => aggregateInterval == TimeSpan.Zero ? TimeSpan.FromDays(14) : aggregateInterval; set { aggregateInterval = value; } }

        private TimeSpan removePeriod;
        public TimeSpan RemovePeriod { get => removePeriod == TimeSpan.Zero ? TimeSpan.FromDays(1) : removePeriod; set { removePeriod = value; } }

        public FeedSettings(ConfigurationExtension configuration)
        {
            configuration.GetSetting("feed", this);
        }
    }
}
