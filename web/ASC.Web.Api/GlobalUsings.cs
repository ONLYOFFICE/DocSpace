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

global using System.Collections.Specialized;
global using System.Globalization;
global using System.Net;
global using System.Net.Mail;
global using System.Net.Sockets;
global using System.Security;
global using System.ServiceModel.Security;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Web;

global using ASC.ActiveDirectory.Base;
global using ASC.ActiveDirectory.Base.Settings;
global using ASC.ActiveDirectory.ComplexOperations;
global using ASC.Api.Collections;
global using ASC.Api.Core;
global using ASC.Api.Core.Convention;
global using ASC.Api.Core.Extensions;
global using ASC.Api.Core.Security;
global using ASC.Api.Settings;
global using ASC.Api.Settings.Smtp;
global using ASC.Api.Utils;
global using ASC.AuditTrail;
global using ASC.AuditTrail.Mappers;
global using ASC.AuditTrail.Models;
global using ASC.AuditTrail.Repositories;
global using ASC.AuditTrail.Types;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Log;
global using ASC.Common.Mapping;
global using ASC.Common.Radicale;
global using ASC.Common.Radicale.Core;
global using ASC.Common.Security.Authorizing;
global using ASC.Common.Threading;
global using ASC.Common.Utils;
global using ASC.Common.Web;
global using ASC.Core;
global using ASC.Core.Billing;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.Notify;
global using ASC.Core.Common.Notify.Push;
global using ASC.Core.Common.Quota;
global using ASC.Core.Common.Quota.Features;
global using ASC.Core.Common.Security;
global using ASC.Core.Common.Settings;
global using ASC.Core.Configuration;
global using ASC.Core.Data;
global using ASC.Core.Encryption;
global using ASC.Core.Security.Authentication;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Data.Backup;
global using ASC.Data.Backup.Contracts;
global using ASC.Data.Backup.EF.Context;
global using ASC.Data.Storage.Configuration;
global using ASC.Data.Storage.Encryption;
global using ASC.Data.Storage.Migration;
global using ASC.EventBus.Abstractions;
global using ASC.FederatedLogin;
global using ASC.FederatedLogin.Helpers;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.FederatedLogin.Profile;
global using ASC.Feed;
global using ASC.Feed.Data;
global using ASC.Files.Core;
global using ASC.Files.Core.Core;
global using ASC.Files.Core.EF;
global using ASC.Files.Core.Helpers;
global using ASC.Files.Core.Resources;
global using ASC.Files.Core.Security;
global using ASC.Files.Core.Services.OFormService;
global using ASC.Files.Core.VirtualRooms;
global using ASC.Geolocation;
global using ASC.IPSecurity;
global using ASC.MessagingSystem;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.EF.Model;
global using ASC.Notify.Cron;
global using ASC.Security.Cryptography;
global using ASC.Web.Api;
global using ASC.Web.Api.ApiModel.RequestsDto;
global using ASC.Web.Api.ApiModel.ResponseDto;
global using ASC.Web.Api.ApiModels.RequestsDto;
global using ASC.Web.Api.ApiModels.ResponseDto;
global using ASC.Web.Api.Core;
global using ASC.Web.Api.Log;
global using ASC.Web.Api.Mapping;
global using ASC.Web.Api.Models;
global using ASC.Web.Api.Routing;
global using ASC.Web.Core;
global using ASC.Web.Core.Helpers;
global using ASC.Web.Core.Mobile;
global using ASC.Web.Core.Notify;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Quota;
global using ASC.Web.Core.RemovePortal;
global using ASC.Web.Core.Sms;
global using ASC.Web.Core.Users;
global using ASC.Web.Core.Utility;
global using ASC.Web.Core.Utility.Settings;
global using ASC.Web.Core.WebZones;
global using ASC.Web.Core.WhiteLabel;
global using ASC.Web.Files.Services.DocumentService;
global using ASC.Web.Studio.Core;
global using ASC.Web.Studio.Core.Notify;
global using ASC.Web.Studio.Core.Quota;
global using ASC.Web.Studio.Core.SMS;
global using ASC.Web.Studio.Core.Statistic;
global using ASC.Web.Studio.Core.TFA;
global using ASC.Web.Studio.UserControls.CustomNavigation;
global using ASC.Web.Studio.UserControls.FirstTime;
global using ASC.Web.Studio.UserControls.Management;
global using ASC.Web.Studio.UserControls.Management.SingleSignOnSettings;
global using ASC.Web.Studio.Utility;
global using ASC.Webhooks.Core;
global using ASC.Webhooks.Core.EF.Model;

global using Autofac;

global using AutoMapper;

global using Google.Authenticator;

global using Joonasw.AspNetCore.SecurityHeaders.Csp.Builder;

global using MailKit.Security;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Hosting.WindowsServices;
global using Microsoft.Extensions.Primitives;

global using MimeKit;

global using ProtoBuf;

global using static ASC.ActiveDirectory.Base.Settings.LdapSettings;
global using static ASC.Security.Cryptography.EmailValidationKeyProvider;

global using SecurityContext = ASC.Core.SecurityContext;
