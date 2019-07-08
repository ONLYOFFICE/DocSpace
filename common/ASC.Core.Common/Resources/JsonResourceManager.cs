using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace ASC.Core.Common.Resources
{
    public class JsonResourceManager
    {
        private const string ClientApp = "ClientApp";
        private const string Locales = "locales";

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
            var dirPath = GetDirName(Path.Combine(ClientApp, "build", Locales)) ?? GetDirName(Path.Combine(ClientApp, "public", Locales));
            if(string.IsNullOrEmpty(dirPath)) return new JObject();

            var files = Directory.GetFiles(dirPath, FileName, SearchOption.AllDirectories);
            if (!files.Any()) return new JObject();

            var filePath = files.FirstOrDefault(r => Path.GetFileName(Path.GetDirectoryName(r)) == culture);
            if(string.IsNullOrEmpty(filePath)) return new JObject();

            return JObject.Parse(File.ReadAllText(filePath));
        }

        string GetDirName(string dirName)
        {
            var dirPath = Path.GetFullPath(dirName);
            return Directory.Exists(dirPath) ? dirPath : null;
        }
    }
}
