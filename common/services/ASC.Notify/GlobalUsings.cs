global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Threading;
global using System.Threading.Tasks;

global using ASC.Api.Core;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.Settings;
global using ASC.Core.Notify.Senders;
global using ASC.Notify;
global using ASC.Notify.Config;
global using ASC.Notify.Messages;
global using ASC.Web.Core;
global using ASC.Web.Core.WhiteLabel;
global using ASC.Web.Studio.Core.Notify;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Google.Protobuf.Collections;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.Hosting.WindowsServices;

global using Newtonsoft.Json;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;
