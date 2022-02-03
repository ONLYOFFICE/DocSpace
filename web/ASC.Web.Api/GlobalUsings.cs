global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Security;
global using System.Security.Authentication;
global using System.Threading;
global using System.Web;
global using System.Text.Json.Serialization;

global using ASC.Api.Core;
global using ASC.Api.Security;
global using ASC.Api.Utils;
global using ASC.AuditTrail;
global using ASC.AuditTrail.Models;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Billing;
global using ASC.Core.Common.Notify.Push;
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
global using ASC.Web.Core.Files;
global using ASC.Web.Core.Mobile;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Sms;
global using ASC.Web.Core.Users;
global using ASC.Web.Core.Utility;
global using ASC.Web.Core.WebZones;
global using ASC.Web.Studio.Core;
global using ASC.Web.Studio.Core.Notify;
global using ASC.Web.Studio.Core.SMS;
global using ASC.Web.Studio.Core.TFA;
global using ASC.Web.Studio.UserControls.Management;
global using ASC.Web.Studio.Utility;
global using ASC.AuditTrail.Repositories;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Options;

global using static ASC.Security.Cryptography.EmailValidationKeyProvider;

global using System.Collections.Specialized;
global using System.Net.Mail;
global using System.ServiceModel.Security;
global using System.Text.Json;
global using System.Text.RegularExpressions;

global using ASC.Api.Collections;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.Notify;
global using ASC.Core.Encryption;
global using ASC.Data.Backup;
global using ASC.Data.Backup.Contracts;
global using ASC.Data.Storage;
global using ASC.Data.Storage.Configuration;
global using ASC.Data.Storage.Encryption;
global using ASC.Data.Storage.Migration;
global using ASC.IPSecurity;
global using ASC.Web.Core.Utility.Settings;
global using ASC.Web.Core.WhiteLabel;
global using ASC.Web.Studio.Core.Quota;
global using ASC.Web.Studio.Core.Statistic;
global using ASC.Web.Studio.UserControls.CustomNavigation;
global using ASC.Web.Studio.UserControls.FirstTime;
global using ASC.Web.Studio.UserControls.Statistics;
global using ASC.Webhooks.Core;
global using ASC.Webhooks.Core.Dao.Models;

global using Google.Authenticator;

global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.Extensions.Caching.Memory;

global using ASC.Api.Settings.Smtp;
global using ASC.Core.Configuration;
global using ASC.FederatedLogin.Helpers;
global using System.IO;
global using System.Runtime.InteropServices;
global using System.Threading.Tasks;

global using Autofac.Extensions.DependencyInjection;

global using Microsoft.Extensions.Hosting;

global using ASC.Api.Settings;
global using ASC.Web.Api.Controllers;

global using Microsoft.Extensions.DependencyInjection;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;
