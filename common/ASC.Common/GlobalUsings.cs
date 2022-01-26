global using System;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Configuration;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Mail;
global using System.Reflection;
global using System.Runtime.Caching;
global using System.Runtime.Loader;
global using System.Security.Cryptography;
global using System.Security.Principal;
global using System.Text;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Web;
global using System.Xml.Linq;
global using System.Xml.XPath;

global using ARSoft.Tools.Net;
global using ARSoft.Tools.Net.Dns;

global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.DependencyInjection;
global using ASC.Common.Logging;
global using ASC.Common.Security;
global using ASC.Common.Security.Authorizing;
global using ASC.Common.Utils;
global using ASC.Security.Cryptography;

global using Autofac;
global using Autofac.Configuration;

global using AutoMapper;

global using Confluent.Kafka;
global using Confluent.Kafka.Admin;

global using Google.Protobuf;

global using JWT;
global using JWT.Algorithms;
global using JWT.Serializers;

global using log4net.Appender;
global using log4net.Config;
global using log4net.Core;
global using log4net.Util;

global using Microsoft.AspNetCore.Cryptography.KeyDerivation;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.Extensions;
global using Microsoft.AspNetCore.Http.Features;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Net.Http.Headers;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Serialization;

global using NLog;
global using NLog.Common;
global using NLog.Targets;

global using NVelocity;
global using NVelocity.App;
global using NVelocity.Runtime.Resource.Loader;

global using StackExchange.Redis.Extensions.Core.Abstractions;
