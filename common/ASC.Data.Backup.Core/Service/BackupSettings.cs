using System;
using System.Collections.Generic;

using ASC.Common.Utils;

namespace ASC.Data.Backup.Service
{
    public class BackupSettings
    {
        public string UpgradesPath { get; set; }

        public int Limit { get; set; }

        public ServiceConfigurationElement Service { get; set; }
        public SchedulerConfigurationElement Scheduler { get; set; }
        public CleanerConfigurationElement Cleaner { get; set; }
        public WebConfigCollection WebConfigs { get; set; }

        public class SchedulerConfigurationElement
        {

            public TimeSpan Period { get; set; }
            public int WorkerCount { get; set; }
        }

        public class CleanerConfigurationElement
        {
            public TimeSpan Period { get; set; }
        }

        public class ServiceConfigurationElement
        {
            public int WorkerCount { get; set; }
        }

        public class WebConfigCollection
        {
            public string CurrentRegion { get; set; }


            public List<WebConfigElement> Elements { get; set; }

            public string CurrentPath
            {
                get
                {
                    if (Elements == null)
                    {
                        Elements = new List<WebConfigElement>();
                    }
                    if (Elements.Count == 0)
                    {
                        return CrossPlatform.PathCombine("..", "..", "WebStudio");
                    }
                    if (Elements.Count == 1)
                    {
                        return Elements[0].Path;
                    }
                    return GetPath(CurrentRegion);
                }
            }

            public string GetPath(string region)
            {

                foreach (var el in Elements)
                {
                    if (el.Region == region)
                    {
                        return el.Path;
                    }
                }
                return null;
            }

        }

        public class WebConfigElement
        {
            public string Region { get; }

            public string Path { get; }

            public WebConfigElement(string region, string path)
            {
                Region = region;
                Path = path;
            }
        }
    }


}

