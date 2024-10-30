using System.Linq.Expressions;
using System.Reflection;

namespace DogHouseService.BLL.Extensions
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool ascending)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new ArgumentException($"No property '{propertyName}' found on '{type.Name}'");

            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var keySelector = Expression.Lambda(propertyAccess, parameter);

            var method = ascending ? "OrderBy" : "OrderByDescending";
            var result = typeof(Queryable).GetMethods().Single(
                    m => m.Name == method
                         && m.IsGenericMethodDefinition
                         && m.GetGenericArguments().Length == 2
                         && m.GetParameters().Length == 2)
                .MakeGenericMethod(type, property.PropertyType)
                .Invoke(null, new object[] { source, keySelector });

            return (IOrderedQueryable<T>)result;
        }
    }
}
