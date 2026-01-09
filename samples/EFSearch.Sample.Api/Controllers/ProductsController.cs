using EFSearch.Mapping;
using EFSearch.Models;
using EFSearch.Sample.Api.Data;
using EFSearch.Sample.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFSearch.Sample.Api.Controllers;

/// <summary>
/// API controller for managing products with search capabilities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly SampleDbContext _dbContext;
    private readonly SearchMap<ProductModel> _searchMap;

    public ProductsController(SampleDbContext dbContext)
    {
        _dbContext = dbContext;
        
        // Configure the search map with whitelisted fields for ProductModel
        _searchMap = new SearchMap<ProductModel>()
            .Map(p => p.Id)
            .Map(p => p.Name)
            .Map(p => p.CategoryName)
            .Map(p => p.Price)
            .Map(p => p.Stock)
            .Map(p => p.IsActive)
            .Map(p => p.CreatedAt);
    }

    /// <summary>
    /// Searches products with filtering, sorting, and pagination.
    /// </summary>
    /// <param name="request">The search request.</param>
    /// <returns>A paged result of products.</returns>
    [HttpPost("search")]
    public ActionResult<SearchResult<ProductModel>> Search([FromBody] SearchRequest request)
    {
        var query = GetProductModelQuery();
        var result = query.ApplySearch(request, _searchMap);
        return Ok(result);
    }

    /// <summary>
    /// Gets all products with optional pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A paged result of products.</returns>
    [HttpGet]
    public ActionResult<SearchResult<ProductModel>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var request = new SearchRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        var query = GetProductModelQuery();
        var result = query.ApplySearch(request, _searchMap);
        return Ok(result);
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <returns>The product if found.</returns>
    [HttpGet("{id}")]
    public ActionResult<ProductModel> GetById(int id)
    {
        var product = GetProductModelQuery()
            .FirstOrDefault(p => p.Id == id);
        
        if (product == null)
        {
            return NotFound();
        }
        
        return Ok(product);
    }

    private IQueryable<ProductModel> GetProductModelQuery()
    {
        return _dbContext.Products
            .Include(p => p.Category)
            .Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category!.Name,
                Price = p.Price,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
    }
}
