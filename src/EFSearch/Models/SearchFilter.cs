namespace EFSearch.Models;

/// <summary>
/// Represents a single filter condition for a search query.
/// </summary>
public class SearchFilter
{
    /// <summary>
    /// The name of the field to filter on.
    /// </summary>
    public required string Field { get; set; }

    /// <summary>
    /// The filter operator to apply.
    /// </summary>
    public FilterOperator Operator { get; set; } = FilterOperator.Equals;

    /// <summary>
    /// The value to compare against.
    /// </summary>
    public object? Value { get; set; }
}
