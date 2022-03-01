global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Globalization;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.Http;
global using System.Reflection;
global using System.Runtime.Serialization;
global using System.Security;
global using System.Security.Authentication;
global using System.Security.Claims;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Web;
global using System.Xml.Linq;

global using ASC.Api.Core;
global using ASC.Api.Core.Auth;
global using ASC.Api.Core.Convention;
global using ASC.Api.Core.Core;
global using ASC.Api.Core.Middleware;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Logging;
global using ASC.Common.Mapping;
global using ASC.Common.Notify.Engine;
global using ASC.Common.Utils;
global using ASC.Common.Web;
global using ASC.Core;
global using ASC.Core.Common.EF;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Security.Cryptography;
global using ASC.Web.Api.Routing;
global using ASC.Web.Core;
global using ASC.Web.Core.Helpers;
global using ASC.Web.Core.Users;
global using ASC.Web.Studio.Utility;
global using ASC.Webhooks.Core;

global using Autofac;

global using Confluent.Kafka;

global using HealthChecks.UI.Client;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ApplicationModels;
global using Microsoft.AspNetCore.Mvc.Authorization;
global using Microsoft.AspNetCore.Mvc.Controllers;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc.Formatters;
global using Microsoft.AspNetCore.Mvc.Routing;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.Routing.Patterns;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Primitives;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Serialization;

global using NLog;
global using NLog.Extensions.Logging;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;
global using ASC.Api.Core.Extensions;