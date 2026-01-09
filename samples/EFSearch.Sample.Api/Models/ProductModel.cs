using EFSearch.Mapping;

namespace EFSearch.Sample.Api.Models;

/// <summary>
/// Product model for API responses with category name projection.
/// </summary>
public class ProductModel
{
    [Searchable]
    public int Id { get; set; }
    
    [Searchable]
    public string Name { get; set; } = string.Empty;
    
    [Searchable]
    public string CategoryName { get; set; } = string.Empty;
    
    [Searchable]
    public decimal Price { get; set; }
    
    [Searchable]
    public int Stock { get; set; }
    
    [Searchable]
    public bool IsActive { get; set; }
    
    [Searchable]
    public DateTime CreatedAt { get; set; }
}
