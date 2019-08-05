/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Api.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ASC.Api.Core
{
    public class ResponseContractResolver : DefaultContractResolver
    {
        public ServiceProvider Services { get; }

        public ResponseContractResolver(ServiceProvider services)
        {
            Services = services;
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true
            };
        }

        protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if(property.PropertyName == "response")
            {
                property.ItemConverter = new JsonStringConverter(Services);
            }

            return property;
        }
    }
    public class ResponseDataContractResolver : DefaultContractResolver
    {
        public List<string> Props { get; }
        public ResponseDataContractResolver(List<string> props)
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true
            };
            Props = props;
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var retval = base.CreateProperties(type, memberSerialization);

            retval = retval.Where(p => Props.Contains(p.PropertyName.ToLower())).ToList();

            return retval;
        }
    }
    public class JsonStringConverter : JsonConverter
    {
        public ServiceProvider Services { get; }

        public JsonStringConverter(ServiceProvider services)
        {
            Services = services;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var httpContext = new ApiContext(Services.GetService<IHttpContextAccessor>().HttpContext);
            var fields = httpContext.Fields;

            if (fields != null)
            {
                var props = fields.Select(r => r.ToLower()).ToList();

                var jsonSerializer = JsonSerializer.CreateDefault();
                jsonSerializer.DateParseHandling = DateParseHandling.None;
                jsonSerializer.ContractResolver = new ResponseDataContractResolver(props);
                jsonSerializer.Serialize(writer, value);
                return;
            }

            serializer.Serialize(writer, value);
        }
    }
}