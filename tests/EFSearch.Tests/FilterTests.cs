using EFSearch.Mapping;
using EFSearch.Models;
using EFSearch.Tests.TestEntities;

namespace EFSearch.Tests;

public class FilterTests
{
    private static readonly List<Product> TestProducts =
    [
        new() { Id = 1, Name = "Apple", Category = "Fruit", Price = 1.50m, Stock = 100, IsActive = true },
        new() { Id = 2, Name = "Banana", Category = "Fruit", Price = 0.75m, Stock = 150, IsActive = true },
        new() { Id = 3, Name = "Carrot", Category = "Vegetable", Price = 0.50m, Stock = 200, IsActive = true },
        new() { Id = 4, Name = "Donut", Category = "Bakery", Price = 2.00m, Stock = 50, IsActive = false },
        new() { Id = 5, Name = "Eclair", Category = "Bakery", Price = 3.50m, Stock = 25, IsActive = true },
    ];

    private static SearchMap<Product> CreateProductMap()
    {
        return new SearchMap<Product>()
            .Map(p => p.Id)
            .Map(p => p.Name)
            .Map(p => p.Category)
            .Map(p => p.Price)
            .Map(p => p.Stock)
            .Map(p => p.IsActive);
    }

    [Fact]
    public void Filter_EqualsOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Category", Operator = FilterOperator.Equals, Value = "Fruit" }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Equal("Fruit", p.Category));
    }

    [Fact]
    public void Filter_NotEqualsOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Category", Operator = FilterOperator.NotEquals, Value = "Fruit" }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, p => Assert.NotEqual("Fruit", p.Category));
    }

    [Fact]
    public void Filter_GreaterThanOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Price", Operator = FilterOperator.GreaterThan, Value = 1.00m }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Price > 1.00m));
    }

    [Fact]
    public void Filter_GreaterThanOrEqualOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Price", Operator = FilterOperator.GreaterThanOrEqual, Value = 2.00m }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Price >= 2.00m));
    }

    [Fact]
    public void Filter_LessThanOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Stock", Operator = FilterOperator.LessThan, Value = 100 }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Stock < 100));
    }

    [Fact]
    public void Filter_LessThanOrEqualOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Stock", Operator = FilterOperator.LessThanOrEqual, Value = 100 }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Stock <= 100));
    }

    [Fact]
    public void Filter_ContainsOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Name", Operator = FilterOperator.Contains, Value = "an" }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Banana", result.Items[0].Name);
    }

    [Fact]
    public void Filter_StartsWithOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Name", Operator = FilterOperator.StartsWith, Value = "C" }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Carrot", result.Items[0].Name);
    }

    [Fact]
    public void Filter_EndsWithOperator_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Name", Operator = FilterOperator.EndsWith, Value = "e" }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Apple", result.Items[0].Name);
    }

    [Fact]
    public void Filter_MultipleFilters_CombinesWithAnd()
    {
        var request = new SearchRequest
        {
            Filters =
            [
                new SearchFilter { Field = "Category", Operator = FilterOperator.Equals, Value = "Bakery" },
                new SearchFilter { Field = "IsActive", Operator = FilterOperator.Equals, Value = true }
            ],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Eclair", result.Items[0].Name);
    }

    [Fact]
    public void Filter_NoFilters_ReturnsAllItems()
    {
        var request = new SearchRequest
        {
            Filters = [],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(5, result.TotalCount);
    }

    [Fact]
    public void Filter_UnmappedField_ThrowsException()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "UnknownField", Operator = FilterOperator.Equals, Value = "test" }],
            PageSize = 10
        };

        Assert.Throws<InvalidOperationException>(() =>
            TestProducts.AsQueryable().ApplySearch(request, CreateProductMap()));
    }

    [Fact]
    public void Filter_BooleanField_FiltersCorrectly()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "IsActive", Operator = FilterOperator.Equals, Value = false }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(1, result.TotalCount);
        Assert.False(result.Items[0].IsActive);
    }
}
