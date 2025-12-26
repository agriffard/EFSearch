namespace EFSearch.Models;

/// <summary>
/// Represents a complete search request with filters, sorting, and pagination.
/// </summary>
public class SearchRequest
{
    /// <summary>
    /// The list of filters to apply. Multiple filters are combined with AND logic.
    /// </summary>
    public List<SearchFilter> Filters { get; set; } = [];

    /// <summary>
    /// The list of sort directives to apply.
    /// </summary>
    public List<SearchSort> Sorts { get; set; } = [];

    /// <summary>
    /// The page number for pagination (1-based).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
