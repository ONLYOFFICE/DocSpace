using System;

using ASC.Common.Utils;

namespace ASC.Feed.Configuration
{
    public class FeedSettings
    {
        public string ServerRoot { get; set; }

        public TimeSpan AggregatePeriod { get; set; }

        public TimeSpan AggregateInterval { get; set; }

        public TimeSpan RemovePeriod { get; set; }

        public static FeedSettings GetInstance(ConfigurationExtension configuration)
        {
            var result = configuration.GetSetting<FeedSettings>("feed");

            if (string.IsNullOrEmpty(result.ServerRoot))
            {
                result.ServerRoot = "http://*/";
            }

            if (result.AggregatePeriod == TimeSpan.Zero)
            {
                result.AggregatePeriod = TimeSpan.FromMinutes(5);
            }

            if (result.AggregateInterval == TimeSpan.Zero)
            {
                result.AggregateInterval = TimeSpan.FromDays(14);
            }

            if (result.RemovePeriod == TimeSpan.Zero)
            {
                result.RemovePeriod = TimeSpan.FromDays(1);
            }

            return result;
        }
    }
}
