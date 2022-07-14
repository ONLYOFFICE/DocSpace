﻿// (c) Copyright Ascensio System SIA 2010-2022
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
global using System.Globalization;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text;

global using ASC.AuditTrail.Log;
global using ASC.AuditTrail.Mappers;
global using ASC.AuditTrail.Models;
global using ASC.AuditTrail.Models.Mappings;
global using ASC.AuditTrail.Types;
global using ASC.Common;
global using ASC.Common.Mapping;
global using ASC.Core;
global using ASC.Core.Common.EF;
global using ASC.Core.Users;
global using ASC.MessagingSystem.Core;
global using ASC.MessagingSystem.Data;
global using ASC.MessagingSystem.Models;
global using ASC.Web.Core.Files;
global using ASC.Web.Files.Classes;
global using ASC.Web.Files.Utils;
global using ASC.Web.Studio.Utility;

global using Autofac;

global using AutoMapper;

global using CsvHelper;
global using CsvHelper.Configuration;

global using Microsoft.Extensions.Logging;

global using NetEscapades.EnumGenerators;

global using Newtonsoft.Json;
