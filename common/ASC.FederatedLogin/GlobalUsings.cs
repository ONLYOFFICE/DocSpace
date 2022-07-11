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

global using System.Data;
global using System.Diagnostics;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net.Http.Headers;
global using System.Reflection;
global using System.Runtime.Serialization;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Security.Cryptography.Pkcs;
global using System.Security.Cryptography.X509Certificates;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Web;
global using System.Xml.XPath;

global using ASC.Common;
global using ASC.Common.Caching;
global using ASC.Common.Log;
global using ASC.Common.Utils;
global using ASC.Core;
global using ASC.Core.Common.Configuration;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.EF.Context;
global using ASC.Core.Common.EF.Model;
global using ASC.Core.Common.Notify;
global using ASC.Core.Common.Notify.Telegram;
global using ASC.FederatedLogin;
global using ASC.FederatedLogin.Helpers;
global using ASC.FederatedLogin.LoginProviders;
global using ASC.FederatedLogin.Profile;
global using ASC.Security.Cryptography;

global using Autofac;

global using JWT.Algorithms;
global using JWT.Builder;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.Extensions;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;

global using SecurityContext = ASC.Core.SecurityContext;
