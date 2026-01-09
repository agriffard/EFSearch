using System.Reflection;

namespace EFSearch.Mapping;

/// <summary>
/// Builds a SearchMap from SearchableAttribute annotations on a type.
/// </summary>
public static class SearchMapBuilder
{
    /// <summary>
    /// Creates a SearchMap by scanning the type T for SearchableAttribute annotations.
    /// </summary>
    /// <typeparam name="T">The entity type to scan.</typeparam>
    /// <returns>A SearchMap configured based on the attributes found.</returns>
    public static SearchMap<T> FromAttributes<T>()
    {
        var map = new SearchMap<T>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<SearchableAttribute>();
            if (attribute != null)
            {
                var fieldName = attribute.FieldName ?? property.Name;
                map.MapProperty(fieldName, property);
            }
        }

        return map;
    }
}
