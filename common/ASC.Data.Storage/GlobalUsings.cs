global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Runtime.Serialization;
global using System.Security.Cryptography;
global using System.ServiceModel;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Web;

global using Amazon;
global using Amazon.CloudFront;
global using Amazon.CloudFront.Model;
global using Amazon.S3;
global using Amazon.S3.Model;
global using Amazon.S3.Transfer;
global using Amazon.Util;

global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Logging;
global using ASC.Common.Threading;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.ChunkedUploader;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.Settings;
global using ASC.Core.Encryption;
global using ASC.Core.Notify;
global using ASC.Core.Tenants;
global using ASC.Data.Storage;
global using ASC.Data.Storage.Configuration;
global using ASC.Data.Storage.DiscStorage;
global using ASC.Data.Storage.Encryption;
global using ASC.Data.Storage.GoogleCloud;
global using ASC.Data.Storage.RackspaceCloud;
global using ASC.Data.Storage.S3;
global using ASC.Notify.Messages;
global using ASC.Protos.Migration;
global using ASC.Security.Cryptography;

global using Google.Apis.Auth.OAuth2;
global using Google.Cloud.Storage.V1;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;

global using net.openstack.Core.Domain;
global using net.openstack.Providers.Rackspace;

global using static Google.Cloud.Storage.V1.UrlSigner;

global using MimeMapping = ASC.Common.Web.MimeMapping;

