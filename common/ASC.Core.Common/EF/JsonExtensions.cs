using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ASC.Core.Common.EF
{
    public static class JsonExtensions
    {
        public static string JsonValue(string column, [NotParameterized] string path)
        {
            //not using
            return column + path;
        }
    }

    public static class DbFunctionExtension
    {
        public static void AddDbFunction(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
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

                    return SqlFunctionExpression.Create("JSON_EXTRACT", res, typeof(string), null);
                });
        }
    }
}
