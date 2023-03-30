using Microsoft.AspNetCore.Mvc;
using OnlineShop.Shared;
using OnlineShop.Shared.Dtos;

namespace OnlineShop.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{

    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
    {
        var response = await _productService.GetProductsAsync();
        return Ok(response);
    }


    [HttpGet("category/{categoryUri}")]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts(string categoryUri)
    {
        var response = await _productService.GetProductsByCategoryAsync(categoryUri);
        return Ok(response);
    }


    [HttpGet("search/{searchText}/{page}")]
    public async Task<ActionResult<ServiceResponse<ProductSearchResult>>> SearchProducts(string searchText, int page = 1)
    {
        var response = await _productService.SearchProducts(searchText, page);
        return Ok(response);
    }


    [HttpGet("searchsuggestions/{searchText}")]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsSearchSuggestions(string searchText)
    {
        var response = await _productService.GetProductSearchSuggestions(searchText);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int id)
    {
        var response = await _productService.GetProductAsync(id);
        return Ok(response);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetFeaturedProducts()
    {
        var response = await _productService.GetFeaturedProducts();
        return Ok(response);
    }
}
