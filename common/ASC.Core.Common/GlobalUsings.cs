global using System;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Configuration;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.Http;
global using System.Reflection;
global using System.Resources;
global using System.Runtime.Caching;
global using System.Runtime.Serialization;
global using System.Security;
global using System.Security.Authentication;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Security.Principal;
global using System.ServiceModel;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Web;
global using System.Xml;

global using Amazon;
global using Amazon.Runtime;
global using Amazon.SimpleEmail;
global using Amazon.SimpleEmail.Model;

global using ASC.Collections;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Logging;
global using ASC.Common.Mapping;
global using ASC.Common.Module;
global using ASC.Common.Notify.Engine;
global using ASC.Common.Notify.Patterns;
global using ASC.Common.Security;
global using ASC.Common.Security.Authentication;
global using ASC.Common.Security.Authorizing;
global using ASC.Common.Utils;
global using ASC.Common.Web;
global using ASC.Core;
global using ASC.Core.Billing;
global using ASC.Core.Caching;
global using ASC.Core.Common;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.EF.Model.Mail;
global using ASC.Core.Common.EF.Model.Resource;
global using ASC.Core.Common.Notify;
global using ASC.Core.Common.Notify.Jabber;
global using ASC.Core.Common.Notify.Push;
global using ASC.Core.Common.Notify.Telegram;
global using ASC.Core.Common.Security;
global using ASC.Core.Common.Settings;
global using ASC.Core.Configuration;
global using ASC.Core.Data;
global using ASC.Core.Notify;
global using ASC.Core.Notify.Jabber;
global using ASC.Core.Notify.Senders;
global using ASC.Core.Security.Authentication;
global using ASC.Core.Security.Authorizing;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Notify;
global using ASC.Notify.Channels;
global using ASC.Notify.Cron;
global using ASC.Notify.Engine;
global using ASC.Notify.Messages;
global using ASC.Notify.Model;
global using ASC.Notify.Patterns;
global using ASC.Notify.Recipients;
global using ASC.Notify.Sinks;
global using ASC.Security.Cryptography;
global using ASC.Web.Studio.Utility;

global using Autofac;

global using AutoMapper;

global using MailKit.Security;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.EntityFrameworkCore.Query;
global using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using MimeKit;

global using Newtonsoft.Json;

global using NVelocity;
global using NVelocity.App.Events;

global using Telegram.Bot;

global using static ASC.Security.Cryptography.EmailValidationKeyProvider;
