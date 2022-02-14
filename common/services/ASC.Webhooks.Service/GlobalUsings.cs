global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Runtime.InteropServices;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Threading;
global using System.Threading.Tasks;

global using ASC.Api.Core;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Web.Webhooks;
global using ASC.Webhooks.Core;
global using ASC.Webhooks.Service.Services;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Hosting.WindowsServices;
global using Microsoft.Extensions.Options;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;
