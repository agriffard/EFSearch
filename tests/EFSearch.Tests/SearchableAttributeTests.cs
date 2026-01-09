using EFSearch.Mapping;
using EFSearch.Models;

namespace EFSearch.Tests;

public class SearchableAttributeTests
{
    private class TestModel
    {
        [Searchable]
        public int Id { get; set; }

        [Searchable]
        public string Name { get; set; } = string.Empty;

        [Searchable("custom_price")]
        public decimal Price { get; set; }

        // Not searchable
        public string InternalNote { get; set; } = string.Empty;
    }

    [Fact]
    public void FromAttributes_MapsSearchableProperties()
    {
        var map = SearchMapBuilder.FromAttributes<TestModel>();

        Assert.True(map.HasField("Id"));
        Assert.True(map.HasField("Name"));
        Assert.True(map.HasField("custom_price"));
    }

    [Fact]
    public void FromAttributes_DoesNotMapNonSearchableProperties()
    {
        var map = SearchMapBuilder.FromAttributes<TestModel>();

        Assert.False(map.HasField("InternalNote"));
    }

    [Fact]
    public void FromAttributes_UsesCustomFieldName()
    {
        var map = SearchMapBuilder.FromAttributes<TestModel>();

        Assert.True(map.HasField("custom_price"));
        Assert.False(map.HasField("Price"));
    }

    [Fact]
    public void FromAttributes_CaseInsensitive()
    {
        var map = SearchMapBuilder.FromAttributes<TestModel>();

        Assert.True(map.HasField("id"));
        Assert.True(map.HasField("ID"));
        Assert.True(map.HasField("name"));
        Assert.True(map.HasField("NAME"));
    }

    [Fact]
    public void FromAttributes_ReturnsPropertyInfo()
    {
        var map = SearchMapBuilder.FromAttributes<TestModel>();

        var result = map.TryGetProperty("Name", out var propertyInfo);

        Assert.True(result);
        Assert.NotNull(propertyInfo);
        Assert.Equal("Name", propertyInfo!.Name);
        Assert.Equal(typeof(string), propertyInfo.PropertyType);
    }

    [Fact]
    public void FromAttributes_MappedFields_ReturnsAllSearchableFields()
    {
        var map = SearchMapBuilder.FromAttributes<TestModel>();

        var fields = map.MappedFields.ToList();

        Assert.Equal(3, fields.Count);
        Assert.Contains("Id", fields);
        Assert.Contains("Name", fields);
        Assert.Contains("custom_price", fields);
    }

    [Fact]
    public void SearchableAttribute_DefaultConstructor()
    {
        var attribute = new SearchableAttribute();

        Assert.Null(attribute.FieldName);
        Assert.Null(attribute.AllowedOperators);
    }

    [Fact]
    public void SearchableAttribute_WithFieldName()
    {
        var attribute = new SearchableAttribute("custom_name");

        Assert.Equal("custom_name", attribute.FieldName);
        Assert.Null(attribute.AllowedOperators);
    }

    [Fact]
    public void SearchableAttribute_WithAllowedOperators()
    {
        var attribute = new SearchableAttribute
        {
            AllowedOperators = [FilterOperator.Equals, FilterOperator.Contains]
        };

        Assert.Null(attribute.FieldName);
        Assert.NotNull(attribute.AllowedOperators);
        Assert.Equal(2, attribute.AllowedOperators.Length);
        Assert.Contains(FilterOperator.Equals, attribute.AllowedOperators);
        Assert.Contains(FilterOperator.Contains, attribute.AllowedOperators);
    }

    [Fact]
    public void FromAttributes_WithGlobalAllowedOperators()
    {
        var map = SearchMapBuilder.FromAttributes<TestModelWithOperators>()
            .AllowOperators(FilterOperator.Equals, FilterOperator.NotEquals);

        Assert.True(map.HasField("Id"));
        Assert.True(map.IsOperatorAllowed(FilterOperator.Equals));
        Assert.True(map.IsOperatorAllowed(FilterOperator.NotEquals));
        Assert.False(map.IsOperatorAllowed(FilterOperator.GreaterThan));
    }

    private class TestModelWithOperators
    {
        [Searchable]
        public int Id { get; set; }

        [Searchable]
        public string Name { get; set; } = string.Empty;
    }
}
