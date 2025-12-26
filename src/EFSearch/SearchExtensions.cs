using EFSearch.Internal;
using EFSearch.Mapping;
using EFSearch.Models;

namespace EFSearch;

/// <summary>
/// Provides extension methods for applying search operations to IQueryable.
/// </summary>
public static class SearchExtensions
{
    /// <summary>
    /// Applies a search request to a queryable, including filtering, sorting, and pagination.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to apply the search to.</param>
    /// <param name="request">The search request containing filters, sorts, and pagination.</param>
    /// <param name="map">The search map defining allowed fields and operators.</param>
    /// <returns>A SearchResult containing the paged items and pagination info.</returns>
    public static SearchResult<T> ApplySearch<T>(
        this IQueryable<T> query,
        SearchRequest request,
        SearchMap<T> map)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(map);

        ValidateRequest(request);

        // Apply filters
        if (request.Filters.Count > 0)
        {
            var predicate = ExpressionBuilder.BuildPredicate(request.Filters, map);
            query = query.Where(predicate);
        }

        // Get total count before pagination
        var totalCount = query.Count();

        // Apply sorting
        if (request.Sorts.Count > 0)
        {
            query = SortBuilder.ApplySorting(query, request.Sorts, map);
        }

        // Apply pagination
        var skip = (request.PageNumber - 1) * request.PageSize;
        var items = query.Skip(skip).Take(request.PageSize).ToList();

        return new SearchResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Applies filters from a search request to a queryable.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to apply filters to.</param>
    /// <param name="request">The search request containing filters.</param>
    /// <param name="map">The search map defining allowed fields and operators.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> ApplyFilters<T>(
        this IQueryable<T> query,
        SearchRequest request,
        SearchMap<T> map)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(map);

        if (request.Filters.Count > 0)
        {
            var predicate = ExpressionBuilder.BuildPredicate(request.Filters, map);
            query = query.Where(predicate);
        }

        return query;
    }

    /// <summary>
    /// Applies sorting from a search request to a queryable.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to apply sorting to.</param>
    /// <param name="request">The search request containing sorts.</param>
    /// <param name="map">The search map defining allowed fields.</param>
    /// <returns>The sorted queryable.</returns>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        SearchRequest request,
        SearchMap<T> map)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(map);

        if (request.Sorts.Count > 0)
        {
            query = SortBuilder.ApplySorting(query, request.Sorts, map);
        }

        return query;
    }

    /// <summary>
    /// Applies pagination from a search request to a queryable.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The queryable to apply pagination to.</param>
    /// <param name="request">The search request containing pagination info.</param>
    /// <returns>The paginated queryable.</returns>
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        SearchRequest request)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(request);

        ValidateRequest(request);

        var skip = (request.PageNumber - 1) * request.PageSize;
        return query.Skip(skip).Take(request.PageSize);
    }

    private static void ValidateRequest(SearchRequest request)
    {
        if (request.PageNumber < 1)
        {
            throw new ArgumentException("PageNumber must be at least 1.", nameof(request));
        }

        if (request.PageSize < 1)
        {
            throw new ArgumentException("PageSize must be at least 1.", nameof(request));
        }
    }
}
