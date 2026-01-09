using System.Linq.Expressions;
using System.Reflection;
using EFSearch.Models;

namespace EFSearch.Mapping;

/// <summary>
/// Provides a type-safe mapping configuration for search operations on entity type T.
/// Maps external field names to entity properties (whitelist approach for security).
/// </summary>
/// <typeparam name="T">The entity type to configure mappings for.</typeparam>
public class SearchMap<T>
{
    private readonly Dictionary<string, PropertyInfo> _fieldMappings = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<FilterOperator> _allowedOperators;

    /// <summary>
    /// Initializes a new instance of the SearchMap class.
    /// </summary>
    public SearchMap()
    {
        _allowedOperators = new HashSet<FilterOperator>(Enum.GetValues<FilterOperator>());
    }

    /// <summary>
    /// Maps a field name to an entity property.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="fieldName">The external field name to use in search requests.</param>
    /// <param name="propertyExpression">The property expression on the entity.</param>
    /// <returns>The current SearchMap instance for fluent configuration.</returns>
    public SearchMap<T> Map<TProperty>(string fieldName, Expression<Func<T, TProperty>> propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(propertyExpression);

        var propertyInfo = GetPropertyInfo(propertyExpression);
        _fieldMappings[fieldName] = propertyInfo;
        return this;
    }

    /// <summary>
    /// Maps a property using its name as the field name.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">The property expression on the entity.</param>
    /// <returns>The current SearchMap instance for fluent configuration.</returns>
    public SearchMap<T> Map<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);

        var propertyInfo = GetPropertyInfo(propertyExpression);
        _fieldMappings[propertyInfo.Name] = propertyInfo;
        return this;
    }

    /// <summary>
    /// Maps a field name to a property directly (used internally for attribute-based configuration).
    /// </summary>
    /// <param name="fieldName">The external field name to use in search requests.</param>
    /// <param name="propertyInfo">The property info to map.</param>
    /// <returns>The current SearchMap instance for fluent configuration.</returns>
    internal SearchMap<T> MapProperty(string fieldName, PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(propertyInfo);

        _fieldMappings[fieldName] = propertyInfo;
        return this;
    }

    /// <summary>
    /// Restricts the allowed filter operators.
    /// </summary>
    /// <param name="operators">The operators to allow.</param>
    /// <returns>The current SearchMap instance for fluent configuration.</returns>
    public SearchMap<T> AllowOperators(params FilterOperator[] operators)
    {
        _allowedOperators.Clear();
        foreach (var op in operators)
        {
            _allowedOperators.Add(op);
        }
        return this;
    }

    /// <summary>
    /// Tries to get the property info for a field name.
    /// </summary>
    /// <param name="fieldName">The field name to look up.</param>
    /// <param name="propertyInfo">The property info if found.</param>
    /// <returns>True if the field was found, false otherwise.</returns>
    public bool TryGetProperty(string fieldName, out PropertyInfo? propertyInfo)
    {
        return _fieldMappings.TryGetValue(fieldName, out propertyInfo);
    }

    /// <summary>
    /// Checks if a field name is mapped.
    /// </summary>
    /// <param name="fieldName">The field name to check.</param>
    /// <returns>True if the field is mapped, false otherwise.</returns>
    public bool HasField(string fieldName)
    {
        return _fieldMappings.ContainsKey(fieldName);
    }

    /// <summary>
    /// Gets all mapped field names.
    /// </summary>
    public IEnumerable<string> MappedFields => _fieldMappings.Keys;

    /// <summary>
    /// Checks if an operator is allowed.
    /// </summary>
    /// <param name="op">The operator to check.</param>
    /// <returns>True if the operator is allowed, false otherwise.</returns>
    public bool IsOperatorAllowed(FilterOperator op)
    {
        return _allowedOperators.Contains(op);
    }

    private static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            // Ensure the member access is directly on the parameter (not nested like p.Name.Length)
            if (memberExpression.Member is PropertyInfo propertyInfo &&
                memberExpression.Expression is ParameterExpression)
            {
                return propertyInfo;
            }
        }

        throw new ArgumentException("Expression must be a direct property access expression.", nameof(propertyExpression));
    }
}
