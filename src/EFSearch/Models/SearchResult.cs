namespace EFSearch.Models;

/// <summary>
/// Represents the result of a search query with pagination information.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public class SearchResult<T>
{
    /// <summary>
    /// The items in the current page.
    /// </summary>
    public required IReadOnlyList<T> Items { get; set; }

    /// <summary>
    /// The total number of items matching the search criteria.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
