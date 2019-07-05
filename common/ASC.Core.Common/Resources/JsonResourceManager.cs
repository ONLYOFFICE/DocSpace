using System.Collections.Concurrent;
using System.IO;
using System.Linq;
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
            DirName = "ClientApp";
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
            var dirPath = Path.GetFullPath(DirName);
            if(!Directory.Exists(dirPath)) return new JObject();

            var files = Directory.GetFiles(dirPath, FileName, SearchOption.AllDirectories);
            if (!files.Any()) return new JObject();

            var filePath = files.FirstOrDefault(r => Path.GetFileName(Path.GetDirectoryName(r)) == culture);
            if(string.IsNullOrEmpty(filePath)) return new JObject();

            return JObject.Parse(File.ReadAllText(filePath));
        }
    }
}
