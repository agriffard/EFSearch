using System.Linq.Expressions;
using EFSearch.Mapping;
using EFSearch.Models;

namespace EFSearch.Internal;

/// <summary>
/// Builds sort expressions from search sort directives.
/// </summary>
internal static class SortBuilder
{
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
            var lambda = Expression.Lambda(property, parameter);

            if (isFirst)
            {
                orderedQuery = sort.Direction == SortDirection.Ascending
                    ? Queryable.OrderBy(query, (dynamic)lambda)
                    : Queryable.OrderByDescending(query, (dynamic)lambda);
                isFirst = false;
            }
            else
            {
                orderedQuery = sort.Direction == SortDirection.Ascending
                    ? Queryable.ThenBy(orderedQuery!, (dynamic)lambda)
                    : Queryable.ThenByDescending(orderedQuery!, (dynamic)lambda);
            }
        }

        return orderedQuery ?? (IOrderedQueryable<T>)query;
    }
}
