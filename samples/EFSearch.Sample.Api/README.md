# EFSearch Sample API

## Overview
This sample API demonstrates the EFSearch library for Entity Framework Core, which provides powerful search, filtering, sorting, and pagination capabilities.

## Configuration Approaches

### Attribute-Based Configuration (Recommended)

Mark properties as searchable using the `[Searchable]` attribute:

```csharp
public class ProductModel
{
    [Searchable]
    public int Id { get; set; }
    
    [Searchable]
    public string Name { get; set; }
    
    [Searchable]
    public decimal Price { get; set; }
    
    // This property is not searchable
    public string InternalNotes { get; set; }
}
```

In your controller:

```csharp
public class ProductsController : ControllerBase
{
    // Build SearchMap once from attributes (cached)
    private static readonly SearchMap<ProductModel> _searchMap = 
        SearchMapBuilder.FromAttributes<ProductModel>();

    [HttpPost("search")]
    public ActionResult<SearchResult<ProductModel>> Search([FromBody] SearchRequest request)
    {
        var query = GetProductModelQuery();
        var result = query.ApplySearch(request, _searchMap);
        return Ok(result);
    }
}
```

**Custom field names:**
```csharp
[Searchable("product_name")]  // Use different name in API
public string Name { get; set; }
```

### Fluent Configuration (Alternative)

You can still configure mappings programmatically:

```csharp
private readonly SearchMap<ProductModel> _searchMap = new SearchMap<ProductModel>()
    .Map(p => p.Id)
    .Map(p => p.Name)
    .Map("product_price", p => p.Price)  // Custom field name
    .AllowOperators(FilterOperator.Equals, FilterOperator.Contains);  // Restrict operators
```

## Database Configuration

### Option 1: InMemory Database (Default)
```json
{
  "DatabaseProvider": "InMemory"
}
```

### Option 2: SQLite Database (Recommended for SQL Logging)
```json
{
  "DatabaseProvider": "SQLite"
}
```

## SQL Query Logging

When using SQLite, SQL queries are logged to the console. Example:

```sql
SELECT "p"."Id", "p"."Name", "c"."Name" AS "CategoryName", "p"."Price", "p"."Stock", "p"."IsActive", "p"."CreatedAt"
FROM "Products" AS "p"
INNER JOIN "Categories" AS "c" ON "p"."CategoryId" = "c"."Id"
ORDER BY "c"."Name", "p"."Price" DESC
LIMIT @__p_0 OFFSET @__p_1
```

## Testing

Use the requests in `EFSearch.Sample.Api.http`:

```http
POST {{EFSearch.Sample.Api_HostAddress}}/api/products/search
Content-Type: application/json

{
  "filters": [
    {
      "field": "Price",
      "operator": "LessThan",
      "value": 100
    }
  ],
  "sorts": [
    {
      "field": "CategoryName",
      "direction": "Ascending"
    }
  ],
  "pageNumber": 1,
  "pageSize": 15
}
```

### Available Filter Operators
- Equals
- NotEquals
- GreaterThan
- GreaterThanOrEqual
- LessThan
- LessThanOrEqual
- Contains
- StartsWith
- EndsWith

### Sort Directions
- Ascending
- Descending
