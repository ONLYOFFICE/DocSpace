using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ASC.Web.Api.Middleware
{
    public class ResponseWrapper
    {
        private readonly RequestDelegate next;

        public ResponseWrapper(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var currentBody = context.Response.Body;

            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await next(context);

            context.Response.Body = currentBody;
            memoryStream.Seek(0, SeekOrigin.Begin);

            ResponseParser responseParser;

            switch (context.Request.RouteValues["format"])
            {
                case "xml":
                    responseParser = new XmlResponseParser();
                    break;
                case "json":
                default:
                    responseParser = new JsonResponseParser();
                    break;
            }

            var readToEnd = new StreamReader(memoryStream).ReadToEnd();
            await context.Response.WriteAsync(responseParser.WrapAndWrite((HttpStatusCode)context.Response.StatusCode, readToEnd));
        }

    }

    public static class ResponseWrapperExtensions
    {
        public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseWrapper>();
        }
    }

    [DataContract]
    public class CommonApiResponse
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public HttpStatusCode StatusCode { get; set; }

        [DataMember]
        public object Response { get; set; }

        protected CommonApiResponse(HttpStatusCode statusCode, object response = null, int status = 0)
        {
            Status = status;
            StatusCode = statusCode;
            Response = response;
        }

        public static CommonApiResponse Create(HttpStatusCode statusCode, object response = null, int status = 0)
        {
            return new CommonApiResponse(statusCode, response, status);
        }
    }

    abstract class ResponseParser
    {
        public abstract object Deserialize(string response);

        public abstract string Serialize(CommonApiResponse response);

        public string WrapAndWrite(HttpStatusCode statusCode, string response)
        {
            var result = CommonApiResponse.Create(statusCode, Deserialize(response));
            return Serialize(result);
        }
    }

    class JsonResponseParser : ResponseParser
    {
        public override object Deserialize(string response)
        {
            return JsonConvert.DeserializeObject(response);
        }

        public override string Serialize(CommonApiResponse response)
        {
            response.Count = response.Response is JObject ? 1 : ((response.Response as JArray)?.Count ?? 0);
            var settings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return JsonConvert.SerializeObject(response, settings);
        }
    }
    class XmlResponseParser : ResponseParser
    {
        public override object Deserialize(string response)
        {
            return XDocument.Parse(response);
        }

        public override string Serialize(CommonApiResponse response)
        {
            var root = ((XDocument)response.Response).Root;
            var count = 0;

            var result = new XElement("result");
            var responseElements = new List<XElement>();

            if (root.Name.LocalName.StartsWith("ArrayOf"))
            {
                var elements = root.Elements();

                foreach(var e in elements)
                {
                    responseElements.Add(GetResponse(e));
                }

                count = elements.Count();
            }
            else
            {
                count = 1;
                responseElements.Add(GetResponse(root));
            }

            result.Add(new XElement(nameof(response.Count).ToCamelCase(), count));
            result.Add(new XElement(nameof(response.Status).ToCamelCase(), response.Status));
            result.Add(new XElement(nameof(response.StatusCode).ToCamelCase(), (int)response.StatusCode));
            result.Add(responseElements);

            var doc = new XDocument(result);
            return doc.ToString();

            XElement GetResponse(XElement xElement)
            {
                return ToLowerCamelCase(new XElement(nameof(response.Response).ToCamelCase(), xElement.Elements().Select(ToLowerCamelCase)));
            }

            XElement ToLowerCamelCase(XElement xElement)
            {
                var lowerXElement = new XElement(xElement.Name.LocalName.ToCamelCase());

                var elements = xElement.Elements();
                if (elements.Any())
                {
                    lowerXElement.Add(elements.Select(ToLowerCamelCase));
                }
                else
                {
                    lowerXElement.Add(xElement.Nodes());
                }

                return lowerXElement;
            }
        }
    }

    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }
}