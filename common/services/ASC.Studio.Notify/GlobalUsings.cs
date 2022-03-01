global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using System.Threading;
global using System.Threading.Tasks;

global using ASC.Api.Core;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Mapping;
global using ASC.Common.Utils;
global using ASC.Core.Notify;
global using ASC.Notify;
global using ASC.Web.Studio.Core.Notify;

global using Autofac.Extensions.DependencyInjection;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Hosting.WindowsServices;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;
