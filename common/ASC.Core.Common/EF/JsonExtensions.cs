using System.Collections.Generic;
using System.Linq.Expressions;

using ASC.Core.Common.EF.Model;

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
}
