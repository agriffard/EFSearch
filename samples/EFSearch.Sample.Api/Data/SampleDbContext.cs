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
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Furniture" },
            new Category { Id = 3, Name = "Accessories" }
        );

        // Seed sample data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop Pro", CategoryId = 1, Price = 1299.99m, Stock = 50, IsActive = true, CreatedAt = new DateTime(2024, 1, 15) },
            new Product { Id = 2, Name = "Wireless Mouse", CategoryId = 1, Price = 29.99m, Stock = 200, IsActive = true, CreatedAt = new DateTime(2024, 2, 10) },
            new Product { Id = 3, Name = "USB-C Cable", CategoryId = 1, Price = 12.99m, Stock = 500, IsActive = true, CreatedAt = new DateTime(2024, 3, 5) },
            new Product { Id = 4, Name = "Office Chair", CategoryId = 2, Price = 299.99m, Stock = 30, IsActive = true, CreatedAt = new DateTime(2024, 1, 20) },
            new Product { Id = 5, Name = "Standing Desk", CategoryId = 2, Price = 549.99m, Stock = 15, IsActive = true, CreatedAt = new DateTime(2024, 2, 25) },
            new Product { Id = 6, Name = "Monitor 27\"", CategoryId = 1, Price = 349.99m, Stock = 75, IsActive = true, CreatedAt = new DateTime(2024, 4, 1) },
            new Product { Id = 7, Name = "Keyboard Mechanical", CategoryId = 1, Price = 89.99m, Stock = 120, IsActive = true, CreatedAt = new DateTime(2024, 3, 15) },
            new Product { Id = 8, Name = "Desk Lamp", CategoryId = 2, Price = 45.99m, Stock = 80, IsActive = true, CreatedAt = new DateTime(2024, 5, 10) },
            new Product { Id = 9, Name = "Webcam HD", CategoryId = 1, Price = 79.99m, Stock = 60, IsActive = true, CreatedAt = new DateTime(2024, 4, 20) },
            new Product { Id = 10, Name = "Filing Cabinet", CategoryId = 2, Price = 189.99m, Stock = 25, IsActive = false, CreatedAt = new DateTime(2024, 1, 5) },
            new Product { Id = 11, Name = "Headphones Wireless", CategoryId = 1, Price = 149.99m, Stock = 90, IsActive = true, CreatedAt = new DateTime(2024, 6, 1) },
            new Product { Id = 12, Name = "Bookshelf", CategoryId = 2, Price = 129.99m, Stock = 40, IsActive = true, CreatedAt = new DateTime(2024, 5, 15) },
            new Product { Id = 13, Name = "Tablet 10\"", CategoryId = 1, Price = 399.99m, Stock = 55, IsActive = true, CreatedAt = new DateTime(2024, 7, 10) },
            new Product { Id = 14, Name = "Ergonomic Mouse Pad", CategoryId = 3, Price = 24.99m, Stock = 150, IsActive = true, CreatedAt = new DateTime(2024, 6, 20) },
            new Product { Id = 15, Name = "Monitor Stand", CategoryId = 3, Price = 59.99m, Stock = 70, IsActive = true, CreatedAt = new DateTime(2024, 8, 5) }
        );
    }
}
