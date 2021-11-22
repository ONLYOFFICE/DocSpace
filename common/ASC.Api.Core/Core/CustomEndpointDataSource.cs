using System.Collections.Generic;
using System.Linq;

using ASC.Web.Api.Routing;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;

namespace ASC.Api.Core.Core
{
    public class CustomEndpointDataSource : EndpointDataSource
    {
        public EndpointDataSource Source { get; }

        public override IReadOnlyList<Endpoint> Endpoints { get; }
        public CustomEndpointDataSource(EndpointDataSource source)
        {
            Source = source;
            var endpoints = Source.Endpoints.Cast<RouteEndpoint>();
            Endpoints = endpoints
                .SelectMany(r =>
                {
                    var endpoints = new List<RouteEndpoint>();

                    var attr = r.Metadata.OfType<CustomHttpMethodAttribute>().FirstOrDefault();
                    var enableFormat = attr == null || !attr.DisableFormat;

                    if (enableFormat)
                    {
                        endpoints.Add(new RouteEndpoint(r.RequestDelegate, RoutePatternFactory.Parse(r.RoutePattern.RawText + ".{format}"), r.Order, r.Metadata, r.DisplayName));
                    }
                    else
                    {
                        endpoints.Add(new RouteEndpoint(r.RequestDelegate, RoutePatternFactory.Parse(r.RoutePattern.RawText + ".json"), r.Order - 1, r.Metadata, r.DisplayName));
                        endpoints.Add(new RouteEndpoint(r.RequestDelegate, RoutePatternFactory.Parse(r.RoutePattern.RawText + ".xml"), r.Order - 1, r.Metadata, r.DisplayName));
                    }

                    return endpoints;
                })
                .ToList();
        }

        public override IChangeToken GetChangeToken()
        {
            return Source.GetChangeToken();
        }
    }

    public static class EndpointExtension
    {
        public static IEndpointRouteBuilder MapCustom(this IEndpointRouteBuilder endpoints)
        {
            endpoints.DataSources.Add(new CustomEndpointDataSource(endpoints.DataSources.First()));
            return endpoints;
        }
    }
}
