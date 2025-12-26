# EFSearch

A dynamic, type-safe search library for .NET that transforms search requests into Expression Trees for IQueryable<T>. Perfect for building APIs with filtering, sorting, and pagination capabilities.

## Features

- **Dynamic Filtering**: Build complex filter expressions from field/operator/value combinations
- **Type-Safe Mapping**: Whitelist allowed fields with compile-time type safety
- **Flexible Sorting**: Support for multiple sort fields and directions
- **Built-in Pagination**: Automatic Skip/Take with total count and page info
- **Security First**: Only whitelisted fields can be queried, preventing SQL injection
- **EF Core Compatible**: Generates LINQ expressions that translate efficiently to SQL

## Installation

```bash
dotnet add package EFSearch
```

## Quick Start

### 1. Define your entity

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
```

### 2. Create a search map (whitelist)

```csharp
var map = new SearchMap<Product>()
    .Map(p => p.Id)
    .Map(p => p.Name)
    .Map(p => p.Category)
    .Map(p => p.Price)
    .Map(p => p.IsActive);
```

### 3. Build a search request

```csharp
var request = new SearchRequest
{
    Filters =
    [
        new SearchFilter { Field = "Category", Operator = FilterOperator.Equals, Value = "Electronics" },
        new SearchFilter { Field = "Price", Operator = FilterOperator.LessThanOrEqual, Value = 100.00m }
    ],
    Sorts =
    [
        new SearchSort { Field = "Price", Direction = SortDirection.Ascending }
    ],
    PageNumber = 1,
    PageSize = 10
};
```

### 4. Apply the search

```csharp
var result = dbContext.Products.ApplySearch(request, map);

// result.Items       - The items for the current page
// result.TotalCount  - Total matching items across all pages
// result.TotalPages  - Number of pages
// result.PageNumber  - Current page number
// result.HasNextPage - Whether there's a next page
```

## Filter Operators

| Operator | Description |
|----------|-------------|
| `Equals` | Exact match (==) |
| `NotEquals` | Not equal (!=) |
| `GreaterThan` | Greater than (>) |
| `GreaterThanOrEqual` | Greater than or equal (>=) |
| `LessThan` | Less than (<) |
| `LessThanOrEqual` | Less than or equal (<=) |
| `Contains` | String contains |
| `StartsWith` | String starts with |
| `EndsWith` | String ends with |

## Custom Field Names

Map API field names to entity properties:

```csharp
var map = new SearchMap<Product>()
    .Map("product_name", p => p.Name)
    .Map("product_category", p => p.Category);
```

## Restrict Operators

Limit allowed operators for security:

```csharp
var map = new SearchMap<Product>()
    .Map(p => p.Name)
    .AllowOperators(FilterOperator.Equals, FilterOperator.Contains);
```

## Individual Operations

Apply filtering, sorting, or pagination separately:

```csharp
// Filter only
var filtered = query.ApplyFilters(request, map);

// Sort only
var sorted = query.ApplySorting(request, map);

// Paginate only
var paged = query.ApplyPagination(request);
```

## API Usage Example

```csharp
[HttpGet("products")]
public IActionResult GetProducts([FromQuery] SearchRequest request)
{
    var map = new SearchMap<Product>()
        .Map(p => p.Name)
        .Map(p => p.Category)
        .Map(p => p.Price);

    var result = _dbContext.Products.ApplySearch(request, map);
    
    return Ok(result);
}
```

## License

MIT