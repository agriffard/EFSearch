using System.Text.Json.Serialization;

namespace EFSearch.Models;

/// <summary>
/// Defines the sort direction.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    /// <summary>
    /// Ascending order (A-Z, 0-9).
    /// </summary>
    Ascending,

    /// <summary>
    /// Descending order (Z-A, 9-0).
    /// </summary>
    Descending
}
