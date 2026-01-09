namespace EFSearch.Sample.Api.Models;

/// <summary>
/// Product model for API responses with category name projection.
/// </summary>
public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
