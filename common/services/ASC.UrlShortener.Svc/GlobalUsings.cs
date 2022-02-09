global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.IO;
global using System.Threading;
global using System.Threading.Tasks;

global using ASC.Api.Core;
global using ASC.Common;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Logging;
global using ASC.Common.Module;
global using ASC.Common.Utils;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;
