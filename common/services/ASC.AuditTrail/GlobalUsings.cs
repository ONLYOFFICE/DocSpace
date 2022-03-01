global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Reflection;

global using ASC.AuditTrail.Mappers;
global using ASC.Common;
global using ASC.Common.Logging;
global using ASC.Core.Common.EF;
global using ASC.Core.Users;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.Data;
global using ASC.MessagingSystem.Models;
global using ASC.Web.Studio.Utility;
global using ASC.AuditTrail.Attributes;
global using ASC.Web.Core.Files;
global using ASC.Web.Files.Classes;
global using ASC.Web.Files.Utils;
global using ASC.AuditTrail.Models;
global using ASC.AuditTrail.Models.Mappings;
global using ASC.Common.Mapping;

global using Autofac;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Options;

global using Newtonsoft.Json;

global using AutoMapper;

global using CsvHelper;

global using CsvHelper.Configuration;