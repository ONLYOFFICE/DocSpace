using System;

using ASC.Common.Utils;

using Microsoft.Extensions.Configuration;

namespace ASC.Feed.Configuration
{
    public class FeedSettings
    {
        public string ServerRoot { get; set; }

        public TimeSpan AggregatePeriod { get; set; }

        public TimeSpan AggregateInterval { get; set; }

        public TimeSpan RemovePeriod { get; set; }

        public static FeedSettings GetInstance(IConfiguration configuration) => configuration.GetSetting<FeedSettings>("feed");
    }
}
