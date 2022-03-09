global using System.Data;
global using System.Diagnostics;
global using System.Globalization;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Reflection;
global using System.Runtime.Serialization;
global using System.Security.Cryptography.Pkcs;
global using System.Security.Cryptography.X509Certificates;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Web;
global using System.Xml.Linq;
global using System.Xml.XPath;

global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.Notify;
global using ASC.Core.Common.Notify.Telegram;
global using ASC.FederatedLogin;
global using ASC.FederatedLogin.Helpers;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.FederatedLogin.Profile;
global using ASC.Security.Cryptography;
global using ASC.Web.Core.Files;

global using Autofac;

global using DotNetOpenAuth.Messaging;
global using DotNetOpenAuth.OpenId;
global using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
global using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
global using DotNetOpenAuth.OpenId.RelyingParty;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.Extensions;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
