global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Threading;
global using System.Threading.Tasks;

global using ASC.Api.Core;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Tenants;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;

global using StackExchange.Redis.Extensions.Core.Configuration;