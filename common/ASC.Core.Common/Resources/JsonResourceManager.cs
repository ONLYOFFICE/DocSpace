using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using ASC.Common.Utils;
using Newtonsoft.Json.Linq;

namespace ASC.Core.Common.Resources
{
    public class JsonResourceManager
    {
        private static string DirName { get; set; }
        static JsonResourceManager()
        {
            DirName = ConfigurationManager.AppSettings["core:resources"];
        }

        public string FileName { get; }

        public ConcurrentDictionary<string, JObject> KeyValue { get; set; }

        public JsonResourceManager(string fileName)
        {
            FileName = Path.GetFileNameWithoutExtension(fileName) + ".json";
            KeyValue = new ConcurrentDictionary<string, JObject>();
        }

        public string GetString(string key)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            JToken token;

            var resources = KeyValue.GetOrAdd(cultureInfo.Name, FromFile);
            token = resources.SelectToken(key);
            if (token != null)
            {
                return token.ToString();
            }

            resources = KeyValue.GetOrAdd(cultureInfo.Parent.Name, FromFile);
            token = resources.SelectToken(key);
            if (token != null)
            {
                return token.ToString();
            }

            resources = KeyValue.GetOrAdd("en", FromFile);
            token = resources.SelectToken(key);
            if (token != null)
            {
                return token.ToString();
            }

            return key;
        }

        JObject FromFile(string culture)
        {
            var filePath = Path.GetFullPath(Path.Combine(DirName, culture, FileName));
            if (!File.Exists(filePath)) return new JObject();

            return JObject.Parse(File.ReadAllText(filePath));
        }
    }
}
