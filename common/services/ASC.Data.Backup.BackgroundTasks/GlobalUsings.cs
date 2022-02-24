global using System.Runtime.InteropServices;
global using ASC.Api.Core;
global using ASC.Api.Core.Extensions;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Billing;
global using ASC.Core.Common.Hosting;
global using ASC.Core.Common.Hosting.Interfaces;
global using ASC.Data.Backup.BackgroundTasks;
global using ASC.Data.Backup.Contracts;
global using ASC.Data.Backup.Core.IntegrationEvents.Events;
global using ASC.Data.Backup.IntegrationEvents.EventHandling;
global using ASC.Data.Backup.Services;
global using ASC.Data.Backup.Storage;
global using ASC.EventBus.Abstractions;
global using ASC.Files.Core;
global using ASC.Web.Studio.Core.Notify;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Hosting.WindowsServices;
global using Microsoft.Extensions.Options;

global using Newtonsoft.Json;
