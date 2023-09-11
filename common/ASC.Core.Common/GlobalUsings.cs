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
global using System.Configuration;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Globalization;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Reflection;
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
global using System.Web;
global using System.Xml;

global using Amazon;
global using Amazon.Runtime;
global using Amazon.SimpleEmail;
global using Amazon.SimpleEmail.Model;

global using ASC.AuditTrail.Models;
global using ASC.Collections;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Data;
global using ASC.Common.Log;
global using ASC.Common.Logging;
global using ASC.Common.Mapping;
global using ASC.Common.Module;
global using ASC.Common.Notify.Engine;
global using ASC.Common.Notify.Patterns;
global using ASC.Common.Radicale;
global using ASC.Common.Radicale.Core;
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
global using ASC.Core.Common.Hosting.Extensions;
global using ASC.Core.Common.Hosting.Interfaces;
global using ASC.Core.Common.Log;
global using ASC.Core.Common.Notify;
global using ASC.Core.Common.Notify.IntegrationEvents.Events;
global using ASC.Core.Common.Notify.Jabber;
global using ASC.Core.Common.Notify.Push;
global using ASC.Core.Common.Notify.Telegram;
global using ASC.Core.Common.Quota;
global using ASC.Core.Common.Quota.Features;
global using ASC.Core.Common.Security;
global using ASC.Core.Common.Settings;
global using ASC.Core.Configuration;
global using ASC.Core.Data;
global using ASC.Core.Notify;
global using ASC.Core.Notify.Jabber;
global using ASC.Core.Notify.Senders;
global using ASC.Core.Notify.Socket;
global using ASC.Core.Security.Authentication;
global using ASC.Core.Security.Authorizing;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.EventBus.Abstractions;
global using ASC.EventBus.Events;
global using ASC.Geolocation;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.EF.Context;
global using ASC.MessagingSystem.EF.Model;
global using ASC.MessagingSystem.Mapping;
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
global using AutoMapper.QueryableExtensions;

global using Google.Apis.Auth.OAuth2;

global using MailKit.Security;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.EntityFrameworkCore.Migrations.Operations;
global using Microsoft.EntityFrameworkCore.Query;
global using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
global using Microsoft.EntityFrameworkCore.Update;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using MimeKit;

global using NetEscapades.EnumGenerators;

global using Newtonsoft.Json;

global using NVelocity;
global using NVelocity.App.Events;

global using Polly;

global using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
global using Pomelo.EntityFrameworkCore.MySql.Migrations;

global using ProtoBuf;

global using Telegram.Bot;

global using static ASC.Security.Cryptography.EmailValidationKeyProvider;

global using AppOptions = FirebaseAdmin.AppOptions;
global using FirebaseApp = FirebaseAdmin.FirebaseApp;
global using FirebaseAdminMessaging = FirebaseAdmin.Messaging;
global using JsonSerializer = System.Text.Json.JsonSerializer;
global using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
