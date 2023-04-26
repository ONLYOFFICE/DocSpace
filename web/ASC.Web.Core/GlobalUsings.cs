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

global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Data;
global using System.Diagnostics;
global using System.Globalization;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Net.Mail;
global using System.Net.Mime;
global using System.Reflection;
global using System.Runtime.Serialization;
global using System.Security;
global using System.Security.Authentication;
global using System.Security.Cryptography;
global using System.Security.Principal;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Web;
global using System.Xml;

global using ASC.AuditTrail.Repositories;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Log;
global using ASC.Common.Notify.Engine;
global using ASC.Common.Security;
global using ASC.Common.Security.Authentication;
global using ASC.Common.Security.Authorizing;
global using ASC.Common.Threading;
global using ASC.Common.Utils;
global using ASC.Common.Web;
global using ASC.Core;
global using ASC.Core.Billing;
global using ASC.Core.Common;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.Notify.Push;
global using ASC.Core.Common.Quota;
global using ASC.Core.Common.Quota.Features;
global using ASC.Core.Common.Security;
global using ASC.Core.Common.Settings;
global using ASC.Core.Common.WhiteLabel;
global using ASC.Core.Notify;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Data.Storage;
global using ASC.EventBus.Events;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.FederatedLogin.Profile;
global using ASC.IPSecurity;
global using ASC.MessagingSystem;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.EF.Model;
global using ASC.Notify;
global using ASC.Notify.Engine;
global using ASC.Notify.Messages;
global using ASC.Notify.Model;
global using ASC.Notify.Patterns;
global using ASC.Notify.Recipients;
global using ASC.Notify.Textile;
global using ASC.Security.Cryptography;
global using ASC.Web.Core;
global using ASC.Web.Core.Helpers;
global using ASC.Web.Core.HttpHandlers;
global using ASC.Web.Core.Log;
global using ASC.Web.Core.ModuleManagement.Common;
global using ASC.Web.Core.Notify;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Quota;
global using ASC.Web.Core.Sms;
global using ASC.Web.Core.Subscriptions;
global using ASC.Web.Core.Users;
global using ASC.Web.Core.Utility;
global using ASC.Web.Core.Utility.Settings;
global using ASC.Web.Core.Utility.Skins;
global using ASC.Web.Core.WebZones;
global using ASC.Web.Core.WhiteLabel;
global using ASC.Web.Studio.Core;
global using ASC.Web.Studio.Core.Notify;
global using ASC.Web.Studio.UserControls.Management;
global using ASC.Web.Studio.UserControls.Management.SingleSignOnSettings;
global using ASC.Web.Studio.Utility;

global using Google.Authenticator;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using MimeKit.Utils;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;

global using ProtoBuf;

global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Formats;
global using SixLabors.ImageSharp.Formats.Png;
global using SixLabors.ImageSharp.PixelFormats;
global using SixLabors.ImageSharp.Processing;

global using SkiaSharp;

global using TMResourceData;

global using Twilio.Clients;
global using Twilio.Rest.Api.V2010.Account;
global using Twilio.Types;

global using SecurityContext = ASC.Core.SecurityContext;
global using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
