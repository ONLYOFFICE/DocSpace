using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ASC.Files.Core.Services.OFormService
{
    public class OFormDataAttributes
    {
        [JsonPropertyName("file_oform")]
        public OFromFile File { get; set; }
    }

    public class OFromFile
    {
        public IEnumerable<OFromFileData> Data { get; set; }
    }

    public class OFromFileData
    {
        public int Id { get; set; }
        public OFromFileAttribute Attributes { get; set; }
    }

    public class OFromFileAttribute
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Ext { get; set; }
    }
}
