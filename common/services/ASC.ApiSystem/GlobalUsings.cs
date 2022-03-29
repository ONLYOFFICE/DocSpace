global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Net.Http.Headers;
global using System.Runtime.InteropServices;
global using System.Security;
global using System.Security.Authentication;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using System.Web;
global using System.Web.Http.Filters;

global using ASC.ApiSystem.Classes;
global using ASC.ApiSystem.Controllers;
global using ASC.ApiSystem.Interfaces;
global using ASC.ApiSystem.Models;

global using Autofac;
global using Autofac.Extensions.DependencyInjection;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using Newtonsoft.Json.Linq;

global using StackExchange.Redis.Extensions.Core.Configuration;
global using StackExchange.Redis.Extensions.Newtonsoft;