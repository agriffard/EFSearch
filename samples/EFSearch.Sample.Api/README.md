# SQL Query Logging Guide

## Overview
The Sample API now supports SQL query logging to verify database operations, including joins between Product and Category tables.

## Configuration

### Option 1: InMemory Database (Default)
The InMemory provider is enabled by default. It will log query execution details but not actual SQL queries (since InMemory doesn't use SQL).

**appsettings.json:**
```json
{
  "DatabaseProvider": "InMemory"
}
```

### Option 2: SQLite Database (Recommended for SQL Logging)
To see actual SQL queries with JOINs, switch to SQLite:

**appsettings.json:**
```json
{
  "DatabaseProvider": "SQLite"
}
```

## Viewing Logs

When you run the application, SQL queries will be logged to the console. 

### Example Output with SQLite:

When you call the `/api/products/search` endpoint, you'll see SQL like:

```sql
SELECT "p"."Id", "p"."Name", "c"."Name" AS "CategoryName", "p"."Price", "p"."Stock", "p"."IsActive", "p"."CreatedAt"
FROM "Products" AS "p"
INNER JOIN "Categories" AS "c" ON "p"."CategoryId" = "c"."Id"
ORDER BY "c"."Name", "p"."Price" DESC
LIMIT @__p_0 OFFSET @__p_1
```

This confirms that:
1. The JOIN between Products and Categories is working correctly
2. The CategoryName is being retrieved from the Categories table
3. Sorting by CategoryName translates to sorting by the joined Category table's Name column

## Testing

Run any of the HTTP requests in `EFSearch.Sample.Api.http` and check the console output to verify the SQL queries.

**Recommended test:**
```http
POST {{EFSearch.Sample.Api_HostAddress}}/api/products/search
Content-Type: application/json

{
  "filters": [],
  "sorts": [
    {
      "field": "CategoryName",
      "direction": 0
    }
  ],
  "pageNumber": 1,
  "pageSize": 15
}
```

This will show the SQL query with the JOIN and ORDER BY on the Category.Name field.
