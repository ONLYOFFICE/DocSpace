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

global using System.Collections;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Extensions;
global using System.Globalization;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Net.Mime;
global using System.Reflection;
global using System.Runtime.Serialization;
global using System.Security;
global using System.Security.Cryptography;
global using System.Security.Principal;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Web;
global using System.Xml;

global using AppLimit.CloudComputing.SharpBox;
global using AppLimit.CloudComputing.SharpBox.Exceptions;
global using AppLimit.CloudComputing.SharpBox.StorageProvider;
global using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

global using ASC.Api.Collections;
global using ASC.Api.Core;
global using ASC.Api.Core.Model;
global using ASC.Api.Core.Security;
global using ASC.Api.Utils;
global using ASC.AuditTrail;
global using ASC.AuditTrail.Models;
global using ASC.AuditTrail.Repositories;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Log;
global using ASC.Common.Mapping;
global using ASC.Common.Security.Authentication;
global using ASC.Common.Security.Authorizing;
global using ASC.Common.Threading;
global using ASC.Common.Utils;
global using ASC.Common.Web;
global using ASC.Core;
global using ASC.Core.ChunkedUploader;
global using ASC.Core.Common;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.Quota;
global using ASC.Core.Common.Quota.Features;
global using ASC.Core.Common.Settings;
global using ASC.Core.Notify.Socket;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Data.Storage;
global using ASC.Data.Storage.DataOperators;
global using ASC.ElasticSearch;
global using ASC.ElasticSearch.Core;
global using ASC.ElasticSearch.Service;
global using ASC.EventBus.Abstractions;
global using ASC.EventBus.Events;
global using ASC.FederatedLogin;
global using ASC.FederatedLogin.Helpers;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.FederatedLogin.Profile;
global using ASC.Files.Core;
global using ASC.Files.Core.ApiModels;
global using ASC.Files.Core.ApiModels.RequestDto;
global using ASC.Files.Core.ApiModels.ResponseDto;
global using ASC.Files.Core.Core;
global using ASC.Files.Core.Core.Entries;
global using ASC.Files.Core.Core.Thirdparty;
global using ASC.Files.Core.Core.Thirdparty.ProviderDao;
global using ASC.Files.Core.Data;
global using ASC.Files.Core.EF;
global using ASC.Files.Core.Entries;
global using ASC.Files.Core.Helpers;
global using ASC.Files.Core.IntegrationEvents.Events;
global using ASC.Files.Core.Log;
global using ASC.Files.Core.Mapping;
global using ASC.Files.Core.Resources;
global using ASC.Files.Core.Security;
global using ASC.Files.Core.Services.NotifyService;
global using ASC.Files.Core.Services.OFormService;
global using ASC.Files.Core.Thirdparty;
global using ASC.Files.Core.VirtualRooms;
global using ASC.Files.Thirdparty;
global using ASC.Files.Thirdparty.Box;
global using ASC.Files.Thirdparty.Dropbox;
global using ASC.Files.Thirdparty.GoogleDrive;
global using ASC.Files.Thirdparty.OneDrive;
global using ASC.Files.Thirdparty.ProviderDao;
global using ASC.Files.Thirdparty.SharePoint;
global using ASC.Files.Thirdparty.Sharpbox;
global using ASC.Files.ThumbnailBuilder;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.EF.Model;
global using ASC.Notify.Engine;
global using ASC.Notify.Model;
global using ASC.Notify.Patterns;
global using ASC.Notify.Recipients;
global using ASC.Security.Cryptography;
global using ASC.Web.Api.Models;
global using ASC.Web.Core;
global using ASC.Web.Core.Files;
global using ASC.Web.Core.Notify;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Quota;
global using ASC.Web.Core.Users;
global using ASC.Web.Core.Utility;
global using ASC.Web.Core.Utility.Skins;
global using ASC.Web.Core.WhiteLabel;
global using ASC.Web.Files;
global using ASC.Web.Files.Api;
global using ASC.Web.Files.Classes;
global using ASC.Web.Files.Configuration;
global using ASC.Web.Files.Core;
global using ASC.Web.Files.Core.Compress;
global using ASC.Web.Files.Core.Entries;
global using ASC.Web.Files.Core.Search;
global using ASC.Web.Files.Helpers;
global using ASC.Web.Files.HttpHandlers;
global using ASC.Web.Files.Services.DocumentService;
global using ASC.Web.Files.Services.FFmpegService;
global using ASC.Web.Files.Services.WCFService;
global using ASC.Web.Files.Services.WCFService.FileOperations;
global using ASC.Web.Files.ThirdPartyApp;
global using ASC.Web.Files.Utils;
global using ASC.Web.Studio.Core;
global using ASC.Web.Studio.Core.Notify;
global using ASC.Web.Studio.Utility;

global using AutoMapper;

global using Box.V2;
global using Box.V2.Auth;
global using Box.V2.Config;
global using Box.V2.Models;

global using DocuSign.eSign.Api;
global using DocuSign.eSign.Client;
global using DocuSign.eSign.Model;

global using Dropbox.Api;
global using Dropbox.Api.Files;

global using Google;
global using Google.Apis.Auth.OAuth2;
global using Google.Apis.Auth.OAuth2.Flows;
global using Google.Apis.Auth.OAuth2.Responses;
global using Google.Apis.Drive.v3;
global using Google.Apis.Services;

global using ICSharpCode.SharpZipLib.GZip;
global using ICSharpCode.SharpZipLib.Tar;
global using ICSharpCode.SharpZipLib.Zip;

global using JWT;
global using JWT.Algorithms;
global using JWT.Builder;
global using JWT.Exceptions;
global using JWT.Serializers;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Primitives;
global using Microsoft.Graph;
global using Microsoft.OneDrive.Sdk;
global using Microsoft.SharePoint.Client;

global using Nest;

global using NetEscapades.EnumGenerators;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;

global using ProtoBuf;

global using SixLabors.ImageSharp;

global using StackExchange.Redis;

global using static ASC.Files.Core.Data.AbstractDao;
global using static ASC.Files.Core.Helpers.DocumentService;
global using static ASC.Files.Core.Helpers.DocumentService.CommandResponse;
global using static ASC.Web.Files.Services.DocumentService.DocumentServiceTracker;
global using static ASC.Web.Files.Utils.FileTracker;

global using HttpException = ASC.Common.Web.HttpException;
global using License = ASC.Core.Billing.License;
global using SecurityContext = ASC.Core.SecurityContext;
global using Constants = ASC.Core.Users.Constants;
global using UserInfo = ASC.Core.Users.UserInfo;
global using FilesDbContext = ASC.Files.Core.EF.FilesDbContext;
global using CommandMethod = ASC.Files.Core.Helpers.DocumentService.CommandMethod;
global using FileShare = ASC.Files.Core.Security.FileShare;
global using Tag = ASC.Files.Core.Tag;
global using Thumbnail = ASC.Files.Core.Thumbnail;
global using FileType = ASC.Web.Core.Files.FileType;
global using Token = ASC.Web.Files.ThirdPartyApp.Token;
global using FileShareLink = ASC.Web.Files.Utils.FileShareLink;
global using SocketManager = ASC.Web.Files.Utils.SocketManager;
global using LogLevel = Microsoft.Extensions.Logging.LogLevel;
global using ContentType = System.Net.Mime.ContentType;
global using EnumMemberAttribute = System.Runtime.Serialization.EnumMemberAttribute;
global using JsonSerializer = System.Text.Json.JsonSerializer;
global using JsonTokenType = System.Text.Json.JsonTokenType;
global using JsonConverter = System.Text.Json.Serialization.JsonConverter;
global using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
global using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
