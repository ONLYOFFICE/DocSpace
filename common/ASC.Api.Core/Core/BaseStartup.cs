// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using Flurl.Util;

namespace ASC.Api.Core;

public abstract class BaseStartup
{
    private const string CustomCorsPolicyName = "Basic";
    private const string BasicAuthScheme = "Basic";
    private const string MultiAuthSchemes = "MultiAuthSchemes";

    protected readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly string _corsOrigin;

    protected virtual bool AddControllersAsServices { get; }
    protected virtual bool ConfirmAddScheme { get; }
    protected virtual bool AddAndUseSession { get; }
    protected DIHelper DIHelper { get; }
    protected bool LoadProducts { get; set; } = true;
    protected bool LoadConsumers { get; } = true;
    protected bool WebhooksEnabled { get; set; }

    public BaseStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;

        _corsOrigin = _configuration["core:cors"];

        DIHelper = new DIHelper();

        if (bool.TryParse(_configuration["core:products"], out var loadProducts))
        {
            LoadProducts = loadProducts;
        }
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomHealthCheck(_configuration);
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddHttpClient();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = null;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            var knownProxies = _configuration.GetSection("core:hosting:forwardedHeadersOptions:knownProxies").Get<List<String>>();
            var knownNetworks = _configuration.GetSection("core:hosting:forwardedHeadersOptions:knownNetworks").Get<List<String>>();

            if (knownProxies != null && knownProxies.Count > 0)
            {
                foreach (var knownProxy in knownProxies)
                {
                    options.KnownProxies.Add(IPAddress.Parse(knownProxy));
                }
            }


            if (knownNetworks != null && knownNetworks.Count > 0)
            {
                foreach (var knownNetwork in knownNetworks)
                {
                    var prefix = IPAddress.Parse(knownNetwork.Split("/")[0]);
                    var prefixLength = Convert.ToInt32(knownNetwork.Split("/")[1]);

                    options.KnownNetworks.Add(new IPNetwork(prefix, prefixLength));
                }
            }
        });

        var redisOptions = _configuration.GetSection("Redis").Get<RedisConfiguration>().ConfigurationOptions;
        var connectionMultiplexer = ConnectionMultiplexer.Connect(redisOptions);

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
            PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var userId = httpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;

                if (userId == null)
                {
                    userId = httpContext?.Connection.RemoteIpAddress.ToInvariantString();
                }

                var permitLimit = 1500;

                string partitionKey;

                partitionKey = $"sw_{userId}";

                return RedisRateLimitPartition.GetSlidingWindowRateLimiter(partitionKey, key => new RedisSlidingWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromMinutes(1),
                    ConnectionMultiplexerFactory = () => connectionMultiplexer
                });
            }),
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var userId = httpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                    string partitionKey;
                    int permitLimit;

                    if (userId == null)
                    {
                        userId = httpContext?.Connection.RemoteIpAddress.ToInvariantString();
                    }

                    if (String.Compare(httpContext.Request.Method, "GET", true) == 0)
                    {
                        permitLimit = 50;
                        partitionKey = $"cr_read_{userId}";

                    }
                    else
                    {
                        permitLimit = 15;
                        partitionKey = $"cr_write_{userId}";
                    }

                    return RedisRateLimitPartition.GetConcurrencyRateLimiter(partitionKey, key => new RedisConcurrencyRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        QueueLimit = 0,
                        ConnectionMultiplexerFactory = () => connectionMultiplexer
                    });
                }), 
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                   {
                       var userId = httpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;

                       if (userId == null)
                       {
                           userId = httpContext?.Connection.RemoteIpAddress.ToInvariantString();
                       }

                       var partitionKey = $"fw_post_put_{userId}";
                       var permitLimit = 10000;

                       if (!(String.Compare(httpContext.Request.Method, "POST", true) == 0 ||
                            String.Compare(httpContext.Request.Method, "PUT", true) == 0))
                       {
                           return RateLimitPartition.GetNoLimiter("no_limiter");
                       }

                       return RedisRateLimitPartition.GetFixedWindowRateLimiter(partitionKey, key => new RedisFixedWindowRateLimiterOptions
                       {
                           PermitLimit = permitLimit,
                           Window = TimeSpan.FromDays(1),
                           ConnectionMultiplexerFactory = () => connectionMultiplexer
                       });
                   }
            ));

            options.AddPolicy("sensitive_api", httpContext =>
            {
                var userId = httpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                var permitLimit = 5;
                var partitionKey = $"sensitive_api_{userId}";

                return RedisRateLimitPartition.GetSlidingWindowRateLimiter(partitionKey, key => new RedisSlidingWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromMinutes(1),
                    ConnectionMultiplexerFactory = () => connectionMultiplexer
                });
            });

            options.OnRejected = (context, ct) => RateLimitMetadata.OnRejected(context.HttpContext, context.Lease, ct);
        });

        services.AddScoped<EFLoggerFactory>();

        services.AddBaseDbContextPool<AccountLinkContext>()
                .AddBaseDbContextPool<CoreDbContext>()
                .AddBaseDbContextPool<TenantDbContext>()
                .AddBaseDbContextPool<UserDbContext>()
                .AddBaseDbContextPool<TelegramDbContext>()
                .AddBaseDbContextPool<FirebaseDbContext>()
                .AddBaseDbContextPool<CustomDbContext>()
                .AddBaseDbContextPool<UrlShortenerDbContext>()
                .AddBaseDbContextPool<WebstudioDbContext>()
                .AddBaseDbContextPool<InstanceRegistrationContext>()
                .AddBaseDbContextPool<IntegrationEventLogContext>()
                .AddBaseDbContextPool<FeedDbContext>()
                .AddBaseDbContextPool<MessagesContext>()
                .AddBaseDbContextPool<WebhooksDbContext>()
                .AddBaseDbContextPool<WebPluginDbContext>();

        if (AddAndUseSession)
        {
            services.AddSession();
        }

        DIHelper.Configure(services);

        Action<JsonOptions> jsonOptions = options =>
            {
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            };

        services.AddControllers().AddJsonOptions(jsonOptions);

        services.AddSingleton(jsonOptions);

        DIHelper.AddControllers();
        DIHelper.TryAdd<CultureMiddleware>();
        DIHelper.TryAdd<LoggerMiddleware>();
        DIHelper.TryAdd<IpSecurityFilter>();
        DIHelper.TryAdd<PaymentFilter>();
        DIHelper.TryAdd<ProductSecurityFilter>();
        DIHelper.TryAdd<TenantStatusFilter>();
        DIHelper.TryAdd<ConfirmAuthHandler>();
        DIHelper.TryAdd<BasicAuthHandler>();
        DIHelper.TryAdd<CookieAuthHandler>();
        DIHelper.TryAdd<WebhooksGlobalFilterAttribute>();


        if (!string.IsNullOrEmpty(_corsOrigin))
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: CustomCorsPolicyName,
                                  policy =>
                                  {
                                      policy.WithOrigins(_corsOrigin)
                                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials();
                                  });
            });
        }

        services.AddDistributedCache(_configuration);
        services.AddEventBus(_configuration);
        services.AddDistributedTaskQueue();
        services.AddCacheNotify(_configuration);

        services.RegisterFeature();

        DIHelper.TryAdd(typeof(IWebhookPublisher), typeof(WebhookPublisher));

        if (LoadProducts)
        {
            DIHelper.RegisterProducts(_configuration, _hostEnvironment.ContentRootPath);
        }

        services.AddOptions();

        services.AddMvcCore(config =>
        {
            config.Conventions.Add(new ControllerNameAttributeConvention());

            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            config.Filters.Add(new AuthorizeFilter(policy));
            config.Filters.Add(new TypeFilterAttribute(typeof(TenantStatusFilter)));
            config.Filters.Add(new TypeFilterAttribute(typeof(PaymentFilter)));
            config.Filters.Add(new TypeFilterAttribute(typeof(IpSecurityFilter)));
            config.Filters.Add(new TypeFilterAttribute(typeof(ProductSecurityFilter)));
            config.Filters.Add(new CustomResponseFilterAttribute());
            config.Filters.Add<CustomExceptionFilterAttribute>();
            config.Filters.Add(new TypeFilterAttribute(typeof(WebhooksGlobalFilterAttribute)));
        });

        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = MultiAuthSchemes;
            options.DefaultChallengeScheme = MultiAuthSchemes;
        }).AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>(CookieAuthenticationDefaults.AuthenticationScheme, a => { })
          .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>(BasicAuthScheme, a => { })
          .AddScheme<AuthenticationSchemeOptions, ConfirmAuthHandler>("confirm", a => { })
          .AddJwtBearer("Bearer", options =>
            {
                options.Authority = _configuration["core:oidc:authority"];
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        using var scope = ctx.HttpContext.RequestServices.CreateScope();

                        var securityContext = scope.ServiceProvider.GetService<ASC.Core.SecurityContext>();

                        var claimUserId = ctx.Principal.FindFirstValue("userId");

                        if (string.IsNullOrEmpty(claimUserId))
                        {
                            throw new Exception("Claim 'UserId' is not present in claim list");
                        }

                        var userId = new Guid(claimUserId);

                        await securityContext.AuthenticateMeWithoutCookieAsync(userId, ctx.Principal.Claims.ToList());
                    }
                };
            })
          .AddPolicyScheme(MultiAuthSchemes, JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authorizationHeader = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();

                    if (string.IsNullOrEmpty(authorizationHeader))
                    {
                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    }

                    if (authorizationHeader.StartsWith("Basic "))
                    {
                        return BasicAuthScheme;
                    }

                    if (authorizationHeader.StartsWith("Bearer "))
                    {
                        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                        var jwtHandler = new JwtSecurityTokenHandler();

                        if (jwtHandler.CanReadToken(token))
                        {
                            var issuer = jwtHandler.ReadJwtToken(token).Issuer;
                            if (!string.IsNullOrEmpty(issuer) && issuer.Equals(_configuration["core:oidc:authority"]))
                            {
                                return JwtBearerDefaults.AuthenticationScheme;
                            }
                        }
                    }

                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            });

        services.AddAutoMapper(GetAutoMapperProfileAssemblies());

        services.AddBillingHttpClient();

        services.AddSingleton(Channel.CreateUnbounded<NotifyRequest>());
        services.AddSingleton(svc => svc.GetRequiredService<Channel<NotifyRequest>>().Reader);
        services.AddSingleton(svc => svc.GetRequiredService<Channel<NotifyRequest>>().Writer);
        services.AddActivePassiveHostedService<NotifySenderService>(DIHelper);
        services.AddActivePassiveHostedService<NotifySchedulerService>(DIHelper);


        if (!_hostEnvironment.IsDevelopment())
        {
            services.AddStartupTask<WarmupServicesStartupTask>()
                    .TryAddSingleton(services);
        }
    }

    public static IEnumerable<Assembly> GetAutoMapperProfileAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.StartsWith("ASC."));
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();

        app.UseRouting();

        if (!string.IsNullOrEmpty(_corsOrigin))
        {
            app.UseCors(CustomCorsPolicyName);
        }

        if (AddAndUseSession)
        {
            app.UseSession();
        }

        app.UseSynchronizationContextMiddleware();

        app.UseAuthentication();

        app.UseRateLimiter();

        app.UseAuthorization();

        app.UseCultureMiddleware();

        app.UseLoggerMiddleware();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapCustomAsync(WebhooksEnabled, app.ApplicationServices).Wait();

            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapHealthChecks("/ready", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("services")
            });

            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });

        app.Map("/switch", appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                CustomHealthCheck.Running = !CustomHealthCheck.Running;
                await context.Response.WriteAsync($"{Environment.MachineName} running {CustomHealthCheck.Running}");
            });
        });


    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.Register(_configuration, LoadProducts, LoadConsumers);
    }
}

