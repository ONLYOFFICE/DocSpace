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

global using System.Globalization;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq.Expressions;
global using System.Net;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Security;
global using System.Security.Authentication;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading.Channels;
global using System.Threading.RateLimiting;
global using System.Web;

global using ASC.Api.Core;
global using ASC.Api.Core.Auth;
global using ASC.Api.Core.Convention;
global using ASC.Api.Core.Core;
global using ASC.Api.Core.Extensions;
global using ASC.Api.Core.Log;
global using ASC.Api.Core.Middleware;
global using ASC.Api.Core.Routing;
global using ASC.Api.Core.Security;
global using ASC.AuditTrail.Repositories;
global using ASC.AuditTrail.Types;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Caching.Settings;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Log;
global using ASC.Common.Logging;
global using ASC.Common.Security.Authorizing;
global using ASC.Common.Threading;
global using ASC.Common.Utils;
global using ASC.Common.Web;
global using ASC.Core;
global using ASC.Core.Billing;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.Hosting;
global using ASC.Core.Common.Hosting.Interfaces;
global using ASC.Core.Common.Notify.Engine;
global using ASC.Core.Common.Quota;
global using ASC.Core.Common.Quota.Features;
global using ASC.Core.Common.Security;
global using ASC.Core.Common.Settings;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.EventBus;
global using ASC.EventBus.Abstractions;
global using ASC.EventBus.ActiveMQ;
global using ASC.EventBus.Extensions.Logger;
global using ASC.EventBus.RabbitMQ;
global using ASC.Feed.Context;
global using ASC.IPSecurity;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.EF.Context;
global using ASC.MessagingSystem.EF.Model;
global using ASC.Notify.Engine;
global using ASC.Security.Cryptography;
global using ASC.Web.Api.Routing;
global using ASC.Web.Core;
global using ASC.Web.Core.Helpers;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Quota;
global using ASC.Web.Core.Users;
global using ASC.Web.Studio.Utility;
global using ASC.Webhooks.Core;
global using ASC.Webhooks.Core.EF.Context;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using AutoMapper;

global using Confluent.Kafka;

global using HealthChecks.UI.Client;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
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
global using Microsoft.AspNetCore.Mvc.Routing;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.Routing.Constraints;
global using Microsoft.AspNetCore.Routing.Patterns;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Primitives;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Net.Http.Headers;

global using NLog;
global using NLog.AWS.Logger;
global using NLog.Config;
global using NLog.Web;

global using RabbitMQ.Client;

global using RedisRateLimiting;
global using RedisRateLimiting.AspNetCore;

global using StackExchange.Redis;
global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;

global using LogLevel = Microsoft.Extensions.Logging.LogLevel;
