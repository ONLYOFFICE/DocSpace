using ASC.Api.Core.Routing;

namespace ASC.Api.Core.Core;

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
                var constraintRouteAttr = r.Metadata.OfType<ConstraintRoute>().FirstOrDefault();
                var enableFormat = attr == null || !attr.DisableFormat;

                if (r.RoutePattern.Parameters.Any() && constraintRouteAttr != null)
                {
                    var routeValueDictionary = new RouteValueDictionary
                    {
                        { r.RoutePattern.Parameters.FirstOrDefault().Name, constraintRouteAttr.GetRouteConstraint() }
                    };

                    AddEndpoints(r.RoutePattern.Defaults, routeValueDictionary);

                }
                else
                {
                    AddEndpoints();
                }

                return endpoints;

                void AddEndpoints(IReadOnlyDictionary<string, object> defaults = null, RouteValueDictionary policies = null)
                {
                    endpoints.Add(new RouteEndpoint(r.RequestDelegate, RoutePatternFactory.Parse(r.RoutePattern.RawText, defaults, policies), r.Order, r.Metadata, r.DisplayName));

                    if (enableFormat)
                    {
                        endpoints.Add(new RouteEndpoint(r.RequestDelegate, RoutePatternFactory.Parse(r.RoutePattern.RawText + ".{format}", defaults, policies), r.Order, r.Metadata, r.DisplayName));
                    }
                    else
                    {
                        endpoints.Add(new RouteEndpoint(r.RequestDelegate, RoutePatternFactory.Parse(r.RoutePattern.RawText + ".json", defaults, policies), r.Order - 1, r.Metadata, r.DisplayName));
                        endpoints.Add(new RouteEndpoint(r.RequestDelegate, RoutePatternFactory.Parse(r.RoutePattern.RawText + ".xml", defaults, policies), r.Order - 1, r.Metadata, r.DisplayName));
                    }
                }

            }).ToList();
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
        endpoints.MapControllers();
        var sources = endpoints.DataSources.First();
        endpoints.DataSources.Clear();
        endpoints.DataSources.Add(new CustomEndpointDataSource(sources));

        return endpoints;
    }
}