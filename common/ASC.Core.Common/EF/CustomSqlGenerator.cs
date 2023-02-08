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


namespace ASC.Core.Common.EF;
public class CustomMySqlMigrationsSqlGenerator : MySqlMigrationsSqlGenerator
{
    public CustomMySqlMigrationsSqlGenerator(
        MigrationsSqlGeneratorDependencies dependencies,
        ICommandBatchPreparer commandBatchPreparer,
#pragma warning disable EF1001 // Internal EF Core API usage.
        IMySqlOptions mySqlOptions)
#pragma warning restore EF1001 // Internal EF Core API usage.
        : base(dependencies, commandBatchPreparer, mySqlOptions)
    {
    }

    public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
    {
        NotNull(operations, "operations");

        Options = options;
        var migrationCommandListBuilder = new CustomMigrationCommandListBuilder(Dependencies);
        try
        {
            foreach (var operation in operations)
            {
                Generate(operation, model, migrationCommandListBuilder);
            }
        }
        finally
        {
            Options = MigrationsSqlGenerationOptions.Default;
        }

        var test = migrationCommandListBuilder.GetCommandList();

        return migrationCommandListBuilder.GetCommandList();
    }

    private void NotNull(IReadOnlyList<MigrationOperation> value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}

public class CustomMigrationCommandListBuilder : MigrationCommandListBuilder
{
    private string _operationContainer;
    private bool _isIndexOperation;

    public CustomMigrationCommandListBuilder(MigrationsSqlGeneratorDependencies dependencies)
    : base(dependencies) { }

    public override MigrationCommandListBuilder Append(string o)
    {
        if (o.StartsWith("INSERT INTO "))
        {
            o = o.Replace("INSERT INTO ", "INSERT IGNORE INTO ");
        }
        else if (o.StartsWith("CREATE TABLE "))
        {
            o = o.Replace("CREATE TABLE ", "CREATE TABLE IF NOT EXISTS ");
        }
        else if (o == "CREATE ")
        {
            _isIndexOperation = true;
        }

        if (_isIndexOperation == true)
        {
            _operationContainer += o;
        }
        else
        {
            base.Append(o);
        }

        return this;
    }

    public override MigrationCommandListBuilder AppendLine(string value)
    {
        if (_isIndexOperation == true)
        {
            AppendIndexOpeartion(_operationContainer);

            _operationContainer = "";
            _isIndexOperation = false;
        }

        return base.AppendLine(value);
    }

    private void AppendIndexOpeartion(string indexOperation)
    {
        const string startOpearation = "CREATE ";
        const string startUnique = "UNIQUE ";
        const string startIndexName = "INDEX `";
        const string startTableName = "` ON `";
        const string StartColumnsName = "` (`";
        const string endColumnsName = "`)";

        var separatingStrings = new[] { startOpearation, startUnique, startIndexName, startTableName, StartColumnsName, endColumnsName };
        var separatedOpearion = indexOperation.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);

        var indexName = separatedOpearion[0];
        var tableName = separatedOpearion[1];

        var createIndexForMySQL =
            $"set @x := (select count(*) from information_schema.statistics where table_name = '{tableName}' and index_name = '{indexName}' and table_schema = database()); " +
            $"set @sql := if (@x > 0, 'select ''Index exists.''', '{indexOperation};'); " +
            $"PREPARE stmt FROM @sql; " +
            $"EXECUTE stmt";

        switch (Dependencies.CurrentContext.Context.Database.ProviderName)
        {
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                base.Append(createIndexForMySQL);
                break;

            case "Microsoft.EntityFrameworkCore.SqlServer":
                base.Append(createIndexForMySQL);
                break;

            case "Microsoft.EntityFrameworkCore.MySql":
                base.Append(createIndexForMySQL);
                break;
            default:
                base.Append(createIndexForMySQL);
                break;
        }
    }
}
