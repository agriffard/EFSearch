using EFSearch.Mapping;
using EFSearch.Models;
using EFSearch.Sample.Api.Data;
using EFSearch.Sample.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EFSearch.Sample.Api.Controllers;

/// <summary>
/// API controller for managing products with search capabilities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly SampleDbContext _dbContext;
    private readonly SearchMap<Product> _searchMap;

    public ProductsController(SampleDbContext dbContext)
    {
        _dbContext = dbContext;
        
        // Configure the search map with whitelisted fields
        _searchMap = new SearchMap<Product>()
            .Map(p => p.Id)
            .Map(p => p.Name)
            .Map(p => p.Category)
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
    public ActionResult<SearchResult<Product>> Search([FromBody] SearchRequest request)
    {
        var result = _dbContext.Products.ApplySearch(request, _searchMap);
        return Ok(result);
    }

    /// <summary>
    /// Gets all products with optional pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A paged result of products.</returns>
    [HttpGet]
    public ActionResult<SearchResult<Product>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var request = new SearchRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        var result = _dbContext.Products.ApplySearch(request, _searchMap);
        return Ok(result);
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <returns>The product if found.</returns>
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _dbContext.Products.Find(id);
        
        if (product == null)
        {
            return NotFound();
        }
        
        return Ok(product);
    }
}
