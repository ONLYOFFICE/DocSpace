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

global using System.Configuration;
global using System.Data;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Globalization;
global using System.Reflection;
global using System.Security.Cryptography;
global using System.ServiceModel;
global using System.Text;

global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Xml;
global using System.Xml.Linq;

global using ASC.Api.Utils;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Log;
global using ASC.Common.Threading;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Billing;
global using ASC.Core.ChunkedUploader;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Data.Backup;
global using ASC.Data.Backup.Contracts;
global using ASC.Data.Backup.Core;
global using ASC.Data.Backup.Core.Log;
global using ASC.Data.Backup.EF.Context;
global using ASC.Data.Backup.EF.Model;
global using ASC.Data.Backup.Exceptions;
global using ASC.Data.Backup.Extensions;
global using ASC.Data.Backup.Services;
global using ASC.Data.Backup.Storage;
global using ASC.Data.Backup.Tasks;
global using ASC.Data.Backup.Tasks.Data;
global using ASC.Data.Backup.Tasks.Modules;
global using ASC.Data.Backup.Utils;
global using ASC.Data.Storage;
global using ASC.Data.Storage.Configuration;
global using ASC.Data.Storage.DiscStorage;
global using ASC.Data.Storage.S3;
global using ASC.Data.Storage.DataOperators;
global using ASC.EventBus.Events;
global using ASC.Files.Core;
global using ASC.MessagingSystem.Core;
global using ASC.Notify.Cron;
global using ASC.Notify.Engine;
global using ASC.Notify.Model;
global using ASC.Notify.Patterns;
global using ASC.Notify.Recipients;
global using ASC.Security.Cryptography;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Users;
global using ASC.Web.Core.WhiteLabel;
global using ASC.Web.Files.Utils;
global using ASC.Web.Studio.Core;
global using ASC.Web.Studio.Core.Notify;
global using ASC.Web.Studio.Utility;

global using Autofac;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using MySql.Data.MySqlClient;

global using Newtonsoft.Json;

global using ProtoBuf;
