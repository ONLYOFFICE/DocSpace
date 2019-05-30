using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace ASC.Api.Core.Middleware
{
    abstract class ResponseParser
    {
        public abstract object Deserialize(string response);

        public abstract string Serialize(CommonApiResponse response);

        public string WrapAndWrite(HttpStatusCode statusCode, string response)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    var result1 = CommonApiResponse.CreateError(statusCode, new UnauthorizedAccessException());
                    return Serialize(result1);
                default:
                    var result = CommonApiResponse.Create(statusCode, Deserialize(response));
                    return Serialize(result);
            }
        }

        public string WrapAndWrite(HttpStatusCode statusCode, Exception error)
        {
            var result = CommonApiResponse.CreateError(statusCode, error);
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
            if (response.Status == 0)
            {
                response.Count = response.Response is JObject ? 1 : ((response.Response as JArray)?.Count ?? 0);
            }
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),

            };
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
            var count = 0;

            var result = new XElement("result");
            var responseElements = new List<XElement>();

            if (response.Response != null)
            {
                var root = ((XDocument)response.Response).Root;
                if (root.Name.LocalName.StartsWith("ArrayOf"))
                {
                    var elements = root.Elements();

                    foreach (var e in elements)
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
            }
            else if (response.Error != null)
            {
                responseElements.Add(new XElement(nameof(response.Error).ToCamelCase(), response.Error));
            }

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
