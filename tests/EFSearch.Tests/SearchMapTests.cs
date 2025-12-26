using EFSearch.Mapping;
using EFSearch.Models;
using EFSearch.Tests.TestEntities;

namespace EFSearch.Tests;

public class SearchMapTests
{
    [Fact]
    public void Map_WithExplicitFieldName_MapsCorrectly()
    {
        var map = new SearchMap<Product>()
            .Map("product_name", p => p.Name);

        Assert.True(map.HasField("product_name"));
        Assert.False(map.HasField("Name"));
    }

    [Fact]
    public void Map_WithPropertyExpression_UsesPropertyName()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name);

        Assert.True(map.HasField("Name"));
    }

    [Fact]
    public void Map_CaseInsensitive_FindsField()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name);

        Assert.True(map.HasField("name"));
        Assert.True(map.HasField("NAME"));
        Assert.True(map.HasField("Name"));
    }

    [Fact]
    public void Map_Fluent_ChainsMethods()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name)
            .Map(p => p.Price)
            .Map(p => p.Category);

        Assert.True(map.HasField("Name"));
        Assert.True(map.HasField("Price"));
        Assert.True(map.HasField("Category"));
    }

    [Fact]
    public void TryGetProperty_MappedField_ReturnsTrue()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name);

        var result = map.TryGetProperty("Name", out var propertyInfo);

        Assert.True(result);
        Assert.NotNull(propertyInfo);
        Assert.Equal("Name", propertyInfo!.Name);
    }

    [Fact]
    public void TryGetProperty_UnmappedField_ReturnsFalse()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name);

        var result = map.TryGetProperty("Price", out var propertyInfo);

        Assert.False(result);
        Assert.Null(propertyInfo);
    }

    [Fact]
    public void MappedFields_ReturnsAllMappedFields()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name)
            .Map(p => p.Price)
            .Map("custom", p => p.Category);

        var fields = map.MappedFields.ToList();

        Assert.Equal(3, fields.Count);
        Assert.Contains("Name", fields);
        Assert.Contains("Price", fields);
        Assert.Contains("custom", fields);
    }

    [Fact]
    public void AllowOperators_RestrictsOperators()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name)
            .AllowOperators(FilterOperator.Equals, FilterOperator.Contains);

        Assert.True(map.IsOperatorAllowed(FilterOperator.Equals));
        Assert.True(map.IsOperatorAllowed(FilterOperator.Contains));
        Assert.False(map.IsOperatorAllowed(FilterOperator.GreaterThan));
        Assert.False(map.IsOperatorAllowed(FilterOperator.LessThan));
    }

    [Fact]
    public void DefaultOperators_AllowsAll()
    {
        var map = new SearchMap<Product>()
            .Map(p => p.Name);

        Assert.True(map.IsOperatorAllowed(FilterOperator.Equals));
        Assert.True(map.IsOperatorAllowed(FilterOperator.NotEquals));
        Assert.True(map.IsOperatorAllowed(FilterOperator.GreaterThan));
        Assert.True(map.IsOperatorAllowed(FilterOperator.Contains));
    }

    [Fact]
    public void Map_InvalidExpression_ThrowsException()
    {
        var map = new SearchMap<Product>();

        Assert.Throws<ArgumentException>(() =>
            map.Map(p => p.Name.Length));
    }
}
