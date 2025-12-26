using EFSearch.Sample.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EFSearch.Sample.Api.Data;

/// <summary>
/// Database context for the sample API.
/// </summary>
public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed sample data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Pro", Category = "Electronics", Price = 1299.99m, Stock = 50, IsActive = true, CreatedAt = new DateTime(2024, 1, 15) },
            new Product { Id = 2, Name = "Wireless Mouse", Category = "Electronics", Price = 29.99m, Stock = 200, IsActive = true, CreatedAt = new DateTime(2024, 2, 10) },
            new Product { Id = 3, Name = "USB-C Cable", Category = "Electronics", Price = 12.99m, Stock = 500, IsActive = true, CreatedAt = new DateTime(2024, 3, 5) },
            new Product { Id = 4, Name = "Office Chair", Category = "Furniture", Price = 299.99m, Stock = 30, IsActive = true, CreatedAt = new DateTime(2024, 1, 20) },
            new Product { Id = 5, Name = "Standing Desk", Category = "Furniture", Price = 549.99m, Stock = 15, IsActive = true, CreatedAt = new DateTime(2024, 2, 25) },
            new Product { Id = 6, Name = "Monitor 27\"", Category = "Electronics", Price = 349.99m, Stock = 75, IsActive = true, CreatedAt = new DateTime(2024, 4, 1) },
            new Product { Id = 7, Name = "Keyboard Mechanical", Category = "Electronics", Price = 89.99m, Stock = 120, IsActive = true, CreatedAt = new DateTime(2024, 3, 15) },
            new Product { Id = 8, Name = "Desk Lamp", Category = "Furniture", Price = 45.99m, Stock = 80, IsActive = true, CreatedAt = new DateTime(2024, 5, 10) },
            new Product { Id = 9, Name = "Webcam HD", Category = "Electronics", Price = 79.99m, Stock = 60, IsActive = true, CreatedAt = new DateTime(2024, 4, 20) },
            new Product { Id = 10, Name = "Filing Cabinet", Category = "Furniture", Price = 189.99m, Stock = 25, IsActive = false, CreatedAt = new DateTime(2024, 1, 5) },
            new Product { Id = 11, Name = "Headphones Wireless", Category = "Electronics", Price = 149.99m, Stock = 90, IsActive = true, CreatedAt = new DateTime(2024, 6, 1) },
            new Product { Id = 12, Name = "Bookshelf", Category = "Furniture", Price = 129.99m, Stock = 40, IsActive = true, CreatedAt = new DateTime(2024, 5, 15) },
            new Product { Id = 13, Name = "Tablet 10\"", Category = "Electronics", Price = 399.99m, Stock = 55, IsActive = true, CreatedAt = new DateTime(2024, 7, 10) },
            new Product { Id = 14, Name = "Ergonomic Mouse Pad", Category = "Accessories", Price = 24.99m, Stock = 150, IsActive = true, CreatedAt = new DateTime(2024, 6, 20) },
            new Product { Id = 15, Name = "Monitor Stand", Category = "Accessories", Price = 59.99m, Stock = 70, IsActive = true, CreatedAt = new DateTime(2024, 8, 5) }
        );
    }
}
