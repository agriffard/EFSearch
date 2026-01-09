using EFSearch.Models;

namespace EFSearch.Mapping;

/// <summary>
/// Marks a property as searchable, allowing it to be used in search filters and sorts.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SearchableAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the field name to use in search requests.
    /// If not specified, the property name is used.
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Gets or sets the allowed filter operators for this property.
    /// If null or empty, all operators are allowed.
    /// </summary>
    public FilterOperator[]? AllowedOperators { get; set; }

    /// <summary>
    /// Initializes a new instance of the SearchableAttribute class.
    /// </summary>
    public SearchableAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SearchableAttribute class with a custom field name.
    /// </summary>
    /// <param name="fieldName">The field name to use in search requests.</param>
    public SearchableAttribute(string fieldName)
    {
        FieldName = fieldName;
    }
}
