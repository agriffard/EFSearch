using System.Text.Json.Serialization;

namespace EFSearch.Models;

/// <summary>
/// Defines the available filter operators.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FilterOperator
{
    /// <summary>
    /// Equals comparison (==).
    /// </summary>
    Equals,

    /// <summary>
    /// Not equals comparison (!=).
    /// </summary>
    NotEquals,

    /// <summary>
    /// Greater than comparison (>).
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Greater than or equal comparison (>=).
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Less than comparison (<).
    /// </summary>
    LessThan,

    /// <summary>
    /// Less than or equal comparison (<=).
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// String contains comparison.
    /// </summary>
    Contains,

    /// <summary>
    /// String starts with comparison.
    /// </summary>
    StartsWith,

    /// <summary>
    /// String ends with comparison.
    /// </summary>
    EndsWith
}
