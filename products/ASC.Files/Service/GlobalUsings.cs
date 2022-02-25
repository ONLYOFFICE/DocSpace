global using System.Collections.Concurrent;
global using System.Linq.Expressions;

global using ASC.Api.Core;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Common;
global using ASC.Core.Common.EF;
global using ASC.Core.Tenants;
global using ASC.ElasticSearch;
global using ASC.Feed;
global using ASC.Feed.Core;
global using ASC.Feed.Data;
global using ASC.Files.Core;
global using ASC.Files.Core.EF;
global using ASC.Files.Core.Security;
global using ASC.Files.ThumbnailBuilder;
global using ASC.ElasticSearch.Service;
global using ASC.Web.Core;
global using ASC.Web.Core.Files;
global using ASC.Web.Core.Users;
global using ASC.Web.Files;
global using ASC.Web.Files.Classes;
global using ASC.Web.Files.Core;
global using ASC.Web.Files.Core.Search;
global using ASC.Feed.Aggregator.Service;
global using ASC.Web.Files.Services.DocumentService;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Hosting.WindowsServices;

global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Formats.Png;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;
