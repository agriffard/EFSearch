namespace EFSearch.Models;

/// <summary>
/// Represents a sort directive for a search query.
/// </summary>
public class SearchSort
{
    /// <summary>
    /// The name of the field to sort by.
    /// </summary>
    public required string Field { get; set; }

    /// <summary>
    /// The sort direction.
    /// </summary>
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}
