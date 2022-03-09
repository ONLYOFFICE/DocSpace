global using System.Collections;
global using System.Collections.Concurrent;
global using System.Data;
global using System.Globalization;
global using System.Linq.Expressions;
global using System.Text;

global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Logging;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.Settings;
global using ASC.Core.Tenants;
global using ASC.ElasticSearch.Core;
global using ASC.ElasticSearch.Service;

global using Autofac;

global using Elasticsearch.Net;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;

global using Nest;

global using Newtonsoft.Json;