using System.Linq.Expressions;
using System.Reflection;
using EFSearch.Mapping;
using EFSearch.Models;

namespace EFSearch.Internal;

/// <summary>
/// Builds sort expressions from search sort directives.
/// </summary>
internal static class SortBuilder
{
    private static readonly MethodInfo OrderByMethod = typeof(Queryable).GetMethods()
        .First(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);
    
    private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods()
        .First(m => m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);
    
    private static readonly MethodInfo ThenByMethod = typeof(Queryable).GetMethods()
        .First(m => m.Name == nameof(Queryable.ThenBy) && m.GetParameters().Length == 2);
    
    private static readonly MethodInfo ThenByDescendingMethod = typeof(Queryable).GetMethods()
        .First(m => m.Name == nameof(Queryable.ThenByDescending) && m.GetParameters().Length == 2);

    /// <summary>
    /// Applies sorting to a queryable.
    /// </summary>
    public static IOrderedQueryable<T> ApplySorting<T>(
        IQueryable<T> query,
        IEnumerable<SearchSort> sorts,
        SearchMap<T> map)
    {
        IOrderedQueryable<T>? orderedQuery = null;
        var isFirst = true;

        foreach (var sort in sorts)
        {
            if (!map.TryGetProperty(sort.Field, out var propertyInfo) || propertyInfo == null)
            {
                throw new InvalidOperationException($"Sort field '{sort.Field}' is not mapped.");
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType);
            var lambda = Expression.Lambda(delegateType, property, parameter);

            if (isFirst)
            {
                var method = sort.Direction == SortDirection.Ascending
                    ? OrderByMethod.MakeGenericMethod(typeof(T), propertyInfo.PropertyType)
                    : OrderByDescendingMethod.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
                
                orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, [query, lambda])!;
                isFirst = false;
            }
            else
            {
                var method = sort.Direction == SortDirection.Ascending
                    ? ThenByMethod.MakeGenericMethod(typeof(T), propertyInfo.PropertyType)
                    : ThenByDescendingMethod.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
                
                orderedQuery = (IOrderedQueryable<T>)method.Invoke(null, [orderedQuery, lambda])!;
            }
        }

        return orderedQuery ?? (IOrderedQueryable<T>)query;
    }
}
