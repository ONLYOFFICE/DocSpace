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

using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.Resource.Manager.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Resource.Manager.EF.Context;

public class MySqlResourceDbContext : ResourceDbContext { }
public class PostgreSqlResourceDbContext : ResourceDbContext { }
public class ResourceDbContext : BaseDbContext
{
    public DbSet<ResAuthors> Authors { get; set; }
    public DbSet<ResAuthorsFile> ResAuthorsFiles { get; set; }
    public DbSet<ResAuthorsLang> ResAuthorsLang { get; set; }
    public DbSet<ResCultures> ResCultures { get; set; }
    public DbSet<ResData> ResData { get; set; }
    public DbSet<ResFiles> ResFiles { get; set; }
    public DbSet<ResReserve> ResReserve { get; set; }
    protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
    {
        get
        {
            return new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlResourceDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlResourceDbContext() } ,
            };
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ModelBuilderWrapper
            .From(modelBuilder, _provider)
            .AddResAuthorsLang()
            .AddResAuthorsFile()
            .AddResCultures()
            .AddResFiles()
            .AddResData()
            .AddResAuthors()
            .AddResReserve();
    }
}

public static class ResourceDbExtension
{
    public static DIHelper AddResourceDbService(this DIHelper services)
    {
        return services.AddDbContextManagerService<ResourceDbContext>();
    }
}
