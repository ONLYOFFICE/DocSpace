global using System;
global using System.Globalization;
global using System.Linq;
global using System.Security.Authentication;
global using System.Threading;

global using ASC.Api.Core;
global using ASC.Api.Utils;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Common.Security;
global using ASC.Core.Common.Settings;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.FederatedLogin;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.FederatedLogin.Profile;
global using ASC.MessagingSystem;
global using ASC.Security.Cryptography;
global using ASC.Web.Api.Core;
global using ASC.Web.Api.Models;
global using ASC.Web.Api.Routing;
global using ASC.Web.Core;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Sms;
global using ASC.Web.Core.Users;
global using ASC.Web.Studio.Core;
global using ASC.Web.Studio.Core.Notify;
global using ASC.Web.Studio.Core.SMS;
global using ASC.Web.Studio.Core.TFA;
global using ASC.Web.Studio.Utility;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Options;

global using System.Web;

global using ASC.Common.Logging;

global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;