namespace EFSearch.Tests.TestEntities;

/// <summary>
/// Test entity for unit tests.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
