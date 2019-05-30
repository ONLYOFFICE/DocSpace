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

        public abstract string Serialize(SuccessApiResponse response);

        public abstract string Serialize(ErrorApiResponse response);

        public string WrapAndWrite(HttpStatusCode statusCode, string response, Exception error = null)
        {
            if (error != null)
            {
                var result = CommonApiResponse.CreateError(statusCode, error);
                return Serialize(result);
            }
            else
            {
                var result = CommonApiResponse.Create(statusCode, Deserialize(response));
                return Serialize(result);
            }
        }
    }

    class JsonResponseParser : ResponseParser
    {
        public JsonSerializerSettings Settings { get; }
        public JsonResponseParser()
        {
            Settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public override object Deserialize(string response)
        {
            return JsonConvert.DeserializeObject(response);
        }

        public override string Serialize(SuccessApiResponse response)
        {
            response.Count = response.Response is JObject ? 1 : ((response.Response as JArray)?.Count ?? 0);
            return JsonConvert.SerializeObject(response, Settings);
        }

        public override string Serialize(ErrorApiResponse response)
        {
            return JsonConvert.SerializeObject(response, Settings);
        }
    }
    class XmlResponseParser : ResponseParser
    {
        public override object Deserialize(string response)
        {
            return XDocument.Parse(response);
        }

        public override string Serialize(SuccessApiResponse response)
        {
            var count = 0;

            var result = new XElement("result");
            var responseElements = new List<XElement>();
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

            result.Add(new XElement(nameof(response.Status).ToCamelCase(), response.Status));
            result.Add(new XElement(nameof(response.StatusCode).ToCamelCase(), (int)response.StatusCode));
            result.Add(responseElements);

            var doc = new XDocument(result);
            return doc.ToString();

            XElement GetResponse(XElement xElement)
            {
                return ToLowerCamelCase(new XElement(nameof(response.Response).ToCamelCase(), xElement.Elements().Select(ToLowerCamelCase)));
            }
        }

        public override string Serialize(ErrorApiResponse response)
        {
            var result = new XElement("result");
            result.Add(new XElement(nameof(response.Status).ToCamelCase(), response.Status));
            result.Add(new XElement(nameof(response.StatusCode).ToCamelCase(), (int)response.StatusCode));
            result.Add(new XElement(nameof(response.Error).ToCamelCase(), response.Error));

            var doc = new XDocument(result);

            return doc.ToString();
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
