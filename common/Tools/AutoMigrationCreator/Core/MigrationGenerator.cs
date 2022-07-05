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

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ASC.Core.Common.EF;

using AutoMigrationCreator.Core;

using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMigrationCreator;

public class MigrationGenerator
{
    private readonly BaseDbContext _dbContext;
    private readonly ProjectInfo _projectInfo;
    private readonly string _providerName;
    private readonly string _typeName;
    private string _contextFolderName;
    private readonly Regex _pattern = new Regex(@"\d+$", RegexOptions.Compiled);
    private string ContextFolderName
    {
        get
        {
            if (_contextFolderName == null)
            {
                _contextFolderName = _typeName[(_providerName.Length)..] + _providerName;
            }

            return _contextFolderName;
        }
    }

    public MigrationGenerator(BaseDbContext context, ProjectInfo projectInfo)
    {
        _dbContext = context;
        _projectInfo = projectInfo;
        _typeName = _dbContext.GetType().Name;
        _providerName = GetProviderName();
    }

    public void Generate()
    {
        var scaffolder = EFCoreDesignTimeServices.GetServiceProvider(_dbContext)
            .GetService<IMigrationsScaffolder>();

        var name = GenerateMigrationName();

        var migration = scaffolder.ScaffoldMigration(name,
            $"{_projectInfo.AssemblyName}", $"Migrations.{_providerName}.{ContextFolderName}");

        SaveMigration(migration);
    }

    private void SaveMigration(ScaffoldedMigration migration)
    {
        var path = Path.Combine(_projectInfo.Path, "Migrations", _providerName, ContextFolderName);

        Directory.CreateDirectory(path);

        var migrationPath = Path.Combine(path, $"{migration.MigrationId}{migration.FileExtension}");
        var designerPath = Path.Combine(path, $"{migration.MigrationId}.Designer{migration.FileExtension}");
        var snapshotPath = Path.Combine(path, $"{migration.SnapshotName}{migration.FileExtension}");

        File.WriteAllText(migrationPath, migration.MigrationCode);
        File.WriteAllText(designerPath, migration.MetadataCode);
        File.WriteAllText(snapshotPath, migration.SnapshotCode);
    }

    private string GetLastMigrationName()
    {
        var scaffolderDependecies = EFCoreDesignTimeServices.GetServiceProvider(_dbContext)
            .GetService<MigrationsScaffolderDependencies>();

        var lastMigration = scaffolderDependecies.MigrationsAssembly.Migrations.LastOrDefault();

        return lastMigration.Key;
    }

    private string GenerateMigrationName()
    {
        var last = GetLastMigrationName();

        if (string.IsNullOrEmpty(last))
        {
            return ContextFolderName;
        }

        var migrationNumber = _pattern.Match(last).Value;

        if (string.IsNullOrEmpty(migrationNumber))
        {
            return ContextFolderName + "_Upgrade1";
        }

        return ContextFolderName + "_Upgrade" + (int.Parse(migrationNumber) + 1);
    }

    private string GetProviderName()
    {
        var providers = Enum.GetNames(typeof(Provider));
        var lowerTypeName = _typeName.ToLower();
        var provider = providers.SingleOrDefault(p => lowerTypeName.Contains(p.ToLower()));

        if (provider == null)
        {
            throw new Exception("Provider not support");
        }

        return provider;
    }
}
