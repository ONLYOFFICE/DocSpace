global using System.Net;
global using System.Reflection;
global using System.Runtime.Caching;
global using System.Runtime.InteropServices;
global using System.Text.RegularExpressions;

global using ASC.Api.Core;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.Notify;
global using ASC.Core.Common.Notify.Telegram;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.Notify.Messages;
global using ASC.TelegramService;
global using ASC.TelegramService.Core;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Hosting.WindowsServices;
global using Microsoft.Extensions.Options;

global using Telegram.Bot;
global using Telegram.Bot.Args;
global using Telegram.Bot.Types;