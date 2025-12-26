using EFSearch.Mapping;
using EFSearch.Models;
using EFSearch.Tests.TestEntities;

namespace EFSearch.Tests;

public class PaginationTests
{
    private static readonly List<Product> TestProducts =
    [
        new() { Id = 1, Name = "Item01", Category = "A", Price = 1.00m, Stock = 10, IsActive = true },
        new() { Id = 2, Name = "Item02", Category = "A", Price = 2.00m, Stock = 20, IsActive = true },
        new() { Id = 3, Name = "Item03", Category = "A", Price = 3.00m, Stock = 30, IsActive = true },
        new() { Id = 4, Name = "Item04", Category = "B", Price = 4.00m, Stock = 40, IsActive = true },
        new() { Id = 5, Name = "Item05", Category = "B", Price = 5.00m, Stock = 50, IsActive = true },
        new() { Id = 6, Name = "Item06", Category = "B", Price = 6.00m, Stock = 60, IsActive = true },
        new() { Id = 7, Name = "Item07", Category = "C", Price = 7.00m, Stock = 70, IsActive = true },
        new() { Id = 8, Name = "Item08", Category = "C", Price = 8.00m, Stock = 80, IsActive = true },
        new() { Id = 9, Name = "Item09", Category = "C", Price = 9.00m, Stock = 90, IsActive = true },
        new() { Id = 10, Name = "Item10", Category = "D", Price = 10.00m, Stock = 100, IsActive = true },
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
    public void Pagination_FirstPage_ReturnsCorrectItems()
    {
        var request = new SearchRequest
        {
            PageNumber = 1,
            PageSize = 3
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(10, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(4, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
        Assert.Equal("Item01", result.Items[0].Name);
        Assert.Equal("Item02", result.Items[1].Name);
        Assert.Equal("Item03", result.Items[2].Name);
    }

    [Fact]
    public void Pagination_SecondPage_ReturnsCorrectItems()
    {
        var request = new SearchRequest
        {
            PageNumber = 2,
            PageSize = 3
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(10, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
        Assert.Equal("Item04", result.Items[0].Name);
        Assert.Equal("Item05", result.Items[1].Name);
        Assert.Equal("Item06", result.Items[2].Name);
    }

    [Fact]
    public void Pagination_LastPage_ReturnsCorrectItems()
    {
        var request = new SearchRequest
        {
            PageNumber = 4,
            PageSize = 3
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(10, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(4, result.PageNumber);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
        Assert.Equal("Item10", result.Items[0].Name);
    }

    [Fact]
    public void Pagination_WithFilters_CorrectTotalCount()
    {
        var request = new SearchRequest
        {
            Filters = [new SearchFilter { Field = "Category", Operator = FilterOperator.Equals, Value = "A" }],
            PageNumber = 1,
            PageSize = 2
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public void Pagination_PageSizeGreaterThanTotal_ReturnsAllItems()
    {
        var request = new SearchRequest
        {
            PageNumber = 1,
            PageSize = 100
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(10, result.TotalCount);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void Pagination_ExactMultiple_CorrectTotalPages()
    {
        var request = new SearchRequest
        {
            PageNumber = 1,
            PageSize = 5
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(10, result.TotalCount);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public void Pagination_InvalidPageNumber_ThrowsException()
    {
        var request = new SearchRequest
        {
            PageNumber = 0,
            PageSize = 10
        };

        Assert.Throws<ArgumentException>(() =>
            TestProducts.AsQueryable().ApplySearch(request, CreateProductMap()));
    }

    [Fact]
    public void Pagination_InvalidPageSize_ThrowsException()
    {
        var request = new SearchRequest
        {
            PageNumber = 1,
            PageSize = 0
        };

        Assert.Throws<ArgumentException>(() =>
            TestProducts.AsQueryable().ApplySearch(request, CreateProductMap()));
    }

    [Fact]
    public void Pagination_PageBeyondTotal_ReturnsEmptyItems()
    {
        var request = new SearchRequest
        {
            PageNumber = 100,
            PageSize = 10
        };

        var result = TestProducts.AsQueryable().ApplySearch(request, CreateProductMap());

        Assert.Equal(10, result.TotalCount);
        Assert.Empty(result.Items);
    }
}
