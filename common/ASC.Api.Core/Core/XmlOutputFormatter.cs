using System.Threading.Tasks;
using System.Xml.Linq;

using ASC.Common.Web;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ASC.Api.Core.Core
{
    public class XmlOutputFormatter : IOutputFormatter
    {
        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return context.ContentType == MimeMapping.GetMimeMapping(".xml");
        }

        public Task WriteAsync(OutputFormatterWriteContext context)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateParseHandling = DateParseHandling.None
            };

            var responseJson = JsonConvert.SerializeObject(context.Object, Formatting.Indented, settings);
            responseJson = JsonConvert.DeserializeObject<XDocument>("{\"result\":" + responseJson + "}", settings).ToString(SaveOptions.None);
            return context.HttpContext.Response.WriteAsync(responseJson);
        }
    }
}
