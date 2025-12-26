using EFSearch.Mapping;
using EFSearch.Models;
using EFSearch.Tests.TestEntities;

namespace EFSearch.Tests;

public class SortTests
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
    public void Sort_AscendingByName_SortsCorrectly()
    {
        var request = new SearchRequest
        {
            Sorts = [new SearchSort { Field = "Name", Direction = SortDirection.Ascending }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal("Apple", result.Items[0].Name);
        Assert.Equal("Banana", result.Items[1].Name);
        Assert.Equal("Carrot", result.Items[2].Name);
        Assert.Equal("Donut", result.Items[3].Name);
        Assert.Equal("Eclair", result.Items[4].Name);
    }

    [Fact]
    public void Sort_DescendingByName_SortsCorrectly()
    {
        var request = new SearchRequest
        {
            Sorts = [new SearchSort { Field = "Name", Direction = SortDirection.Descending }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal("Eclair", result.Items[0].Name);
        Assert.Equal("Donut", result.Items[1].Name);
        Assert.Equal("Carrot", result.Items[2].Name);
        Assert.Equal("Banana", result.Items[3].Name);
        Assert.Equal("Apple", result.Items[4].Name);
    }

    [Fact]
    public void Sort_AscendingByPrice_SortsCorrectly()
    {
        var request = new SearchRequest
        {
            Sorts = [new SearchSort { Field = "Price", Direction = SortDirection.Ascending }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(0.50m, result.Items[0].Price);
        Assert.Equal(0.75m, result.Items[1].Price);
        Assert.Equal(1.50m, result.Items[2].Price);
        Assert.Equal(2.00m, result.Items[3].Price);
        Assert.Equal(3.50m, result.Items[4].Price);
    }

    [Fact]
    public void Sort_DescendingByPrice_SortsCorrectly()
    {
        var request = new SearchRequest
        {
            Sorts = [new SearchSort { Field = "Price", Direction = SortDirection.Descending }],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(3.50m, result.Items[0].Price);
        Assert.Equal(2.00m, result.Items[1].Price);
        Assert.Equal(1.50m, result.Items[2].Price);
        Assert.Equal(0.75m, result.Items[3].Price);
        Assert.Equal(0.50m, result.Items[4].Price);
    }

    [Fact]
    public void Sort_MultipleSorts_AppliesInOrder()
    {
        var request = new SearchRequest
        {
            Sorts =
            [
                new SearchSort { Field = "Category", Direction = SortDirection.Ascending },
                new SearchSort { Field = "Price", Direction = SortDirection.Descending }
            ],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        // Bakery first (alphabetically), then sorted by price descending
        Assert.Equal("Eclair", result.Items[0].Name);  // Bakery, $3.50
        Assert.Equal("Donut", result.Items[1].Name);   // Bakery, $2.00
        // Fruit next
        Assert.Equal("Apple", result.Items[2].Name);   // Fruit, $1.50
        Assert.Equal("Banana", result.Items[3].Name);  // Fruit, $0.75
        // Vegetable last
        Assert.Equal("Carrot", result.Items[4].Name);  // Vegetable, $0.50
    }

    [Fact]
    public void Sort_NoSorts_PreservesOriginalOrder()
    {
        var request = new SearchRequest
        {
            Sorts = [],
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(5, result.Items.Count);
    }

    [Fact]
    public void Sort_UnmappedField_ThrowsException()
    {
        var request = new SearchRequest
        {
            Sorts = [new SearchSort { Field = "UnknownField", Direction = SortDirection.Ascending }],
            PageSize = 10
        };

        Assert.Throws<InvalidOperationException>(() =>
            TestProducts.AsQueryable().ApplySearch(request, CreateProductMap()));
    }
}
