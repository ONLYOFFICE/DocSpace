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

public static class IQueryableExtensions
{
    public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
    {
        var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
        var relationalCommandCache = enumerator.Private("_relationalCommandCache");
        var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
        var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

        var sqlGenerator = factory.Create();
        var command = sqlGenerator.GetCommand(selectExpression);

        var sql = command.CommandText;

        return sql;
    }

    private static object Private(this object obj, string privateField)
    {
        return obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }

    private static T Private<T>(this object obj, string privateField)
    {
        return (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}
