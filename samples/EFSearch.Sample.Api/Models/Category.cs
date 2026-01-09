namespace EFSearch.Sample.Api.Models;

/// <summary>
/// Category entity for the sample API.
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
