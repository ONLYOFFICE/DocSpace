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

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASC.Core.Common.EF.Model;

public class ModelBuilderWrapper
{
    private ModelBuilder ModelBuilder { get; set; }
    private Provider Provider { get; set; }

    private ModelBuilderWrapper(ModelBuilder modelBuilder, Provider provider)
    {
        ModelBuilder = modelBuilder;
        Provider = provider;
    }

    public static ModelBuilderWrapper From(ModelBuilder modelBuilder, DatabaseFacade database)
    {
        var provider = Provider.MySql;

        if (database.IsMySql())
        {
            provider = Provider.MySql;
        }
        else if (database.IsNpgsql())
        {
            provider = Provider.PostgreSql;
        }

        return new ModelBuilderWrapper(modelBuilder, provider);
    }

    public static ModelBuilderWrapper From(ModelBuilder modelBuilder, Provider provider)
    {
        return new ModelBuilderWrapper(modelBuilder, provider);
    }

    public ModelBuilderWrapper Add(Action<ModelBuilder> action, Provider provider)
    {
        if (provider == Provider)
        {
            action(ModelBuilder);
        }

        return this;
    }

    public ModelBuilderWrapper HasData<T>(params T[] data) where T : class
    {
        ModelBuilder.Entity<T>().HasData(data);

        return this;
    }

    public EntityTypeBuilder<T> Entity<T>() where T : class
    {
        return ModelBuilder.Entity<T>();
    }

    public void AddDbFunctions()
    {
        ModelBuilder
            .HasDbFunction(typeof(DbFunctionsExtension).GetMethod(nameof(DbFunctionsExtension.JsonValue))!)
            .HasTranslation(e =>
            {
                var res = new List<SqlExpression>();
                if (e is List<SqlExpression> list)
                {
                    if (list[0] is SqlConstantExpression key)
                    {
                        res.Add(new SqlFragmentExpression($"`{key.Value}`"));
                    }

                    if (list[1] is SqlConstantExpression val)
                    {
                        res.Add(new SqlConstantExpression(Expression.Constant($"$.{val.Value}"), val.TypeMapping));
                    }
                }

                return new SqlFunctionExpression("JSON_EXTRACT", res, true, res.Select((SqlExpression a) => false), typeof(string), null);
            });

        switch (Provider)
        {
            case Provider.MySql:
                ModelBuilder
                    .HasDbFunction(typeof(DbFunctionsExtension).GetMethod(nameof(DbFunctionsExtension.SubstringIndex), new[] { typeof(string), typeof(char), typeof(int) })!)
                    .HasName("SUBSTRING_INDEX");
                break;
            case Provider.PostgreSql:
                ModelBuilder
                    .HasDbFunction(typeof(DbFunctionsExtension).GetMethod(nameof(DbFunctionsExtension.SubstringIndex), new[] { typeof(string), typeof(char), typeof(int) })!)
                    .HasName("SPLIT_PART");
                break;
            default:
                throw new InvalidOperationException();
        }
    }
}
