using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Core
{
    public class CustomJsonOptionsWrapper : IConfigureOptions<MvcNewtonsoftJsonOptions>
    {
        readonly IHttpContextAccessor ServiceProvider;
        public CustomJsonOptionsWrapper(IHttpContextAccessor serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public void Configure(MvcNewtonsoftJsonOptions options)
        {
            options.SerializerSettings.ContractResolver = new ResponseContractResolver(ServiceProvider);
        }
    }
}
