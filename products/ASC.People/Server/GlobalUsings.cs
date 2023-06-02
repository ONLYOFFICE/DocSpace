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

global using System.Net.Mail;
global using System.Security;
global using System.Security.Claims;
global using System.ServiceModel.Security;
global using System.Web;
global using System.Globalization;
global using ASC.Api.Core;
global using ASC.Api.Core.Convention;
global using ASC.Api.Core.Extensions;
global using ASC.Api.Core.Model;
global using ASC.Api.Utils;
global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Utils;
global using ASC.Common.Web;
global using ASC.Core;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.Quota;
global using ASC.Core.Common.Quota.Features;
global using ASC.Core.Common.Settings;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Data.Reassigns;
global using ASC.FederatedLogin;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.FederatedLogin.Profile;
global using ASC.Files.Core;
global using ASC.Files.Core.Core;
global using ASC.Files.Core.EF;
global using ASC.Files.Core.Resources;
global using ASC.Files.Core.Security;
global using ASC.Files.Core.VirtualRooms;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.EF.Model;
global using ASC.People;
global using ASC.People.Api;
global using ASC.People.ApiModels.RequestDto;
global using ASC.People.ApiModels.ResponseDto;
global using ASC.People.Log;
global using ASC.People.Resources;
global using ASC.Security.Cryptography;
global using ASC.Web.Api.Models;
global using ASC.Web.Api.Routing;
global using ASC.Web.Core;
global using ASC.Web.Core.Mobile;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Quota;
global using ASC.Web.Core.Users;
global using ASC.Web.Files;
global using ASC.Web.Studio.Core;
global using ASC.Web.Studio.Core.Notify;
global using ASC.Web.Studio.Utility;

global using Autofac;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.Extensions;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Hosting.WindowsServices;
global using Microsoft.Extensions.Logging;

global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Formats;

global using Module = ASC.Api.Core.Module;
global using SecurityContext = ASC.Core.SecurityContext;
