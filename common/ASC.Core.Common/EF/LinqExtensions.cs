using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ASC.Core.Common.EF
{
    public static class LinqExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string name, bool sortOrderAsc)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == (sortOrderAsc ? "OrderBy" : "OrderByDescending") && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IOrderedQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string name, bool sortOrderAsc)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == (sortOrderAsc ? "ThenBy" : "ThenByDescending") && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IOrderedQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
        }


        private static PropertyInfo GetPropertyInfo(Type objType, string name)
        {
            var properties = objType.GetProperties();
            var matchedProperty = properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (matchedProperty == null)
                throw new ArgumentException("name");

            return matchedProperty;
        }
        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi)
        {
            var paramExpr = Expression.Parameter(objType);
            var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
            var expr = Expression.Lambda(propAccess, paramExpr);
            return expr;
        }
    }
}
