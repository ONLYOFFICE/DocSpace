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
global using System.Diagnostics;
global using System.Globalization;
global using System.Net.Mail;
global using System.Net.Security;
global using System.Net.Sockets;
global using System.Resources;
global using System.Security;
global using System.Security.Cryptography;
global using System.Security.Cryptography.X509Certificates;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;

global using ASC.ActiveDirectory.Base;
global using ASC.ActiveDirectory.Base.Data;
global using ASC.ActiveDirectory.Base.Expressions;
global using ASC.ActiveDirectory.Base.Settings;
global using ASC.ActiveDirectory.ComplexOperations;
global using ASC.ActiveDirectory.ComplexOperations.Data;
global using ASC.ActiveDirectory.Log;
global using ASC.ActiveDirectory.Novell;
global using ASC.ActiveDirectory.Novell.Data;
global using ASC.ActiveDirectory.Novell.Exceptions;
global using ASC.ActiveDirectory.Novell.Extensions;
global using ASC.Common;
global using ASC.Common.Security.Authorizing;
global using ASC.Common.Threading;
global using ASC.Core;
global using ASC.Core.Common.EF;
global using ASC.Core.Common.Settings;
global using ASC.Core.Tenants;
global using ASC.Core.Users;
global using ASC.Notify;
global using ASC.Notify.Engine;
global using ASC.Notify.Model;
global using ASC.Notify.Patterns;
global using ASC.Notify.Recipients;
global using ASC.Security.Cryptography;
global using ASC.Web.Core;
global using ASC.Web.Core.PublicResources;
global using ASC.Web.Core.Quota;
global using ASC.Web.Core.Users;
global using ASC.Web.Studio.Utility;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

global using Novell.Directory.Ldap;
global using Novell.Directory.Ldap.Controls;
global using Novell.Directory.Ldap.Rfc2251;
global using Novell.Directory.Ldap.Utilclass;
