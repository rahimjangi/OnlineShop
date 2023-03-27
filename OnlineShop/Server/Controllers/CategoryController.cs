using Microsoft.AspNetCore.Mvc;
using OnlineShop.Shared;

namespace OnlineShop.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<Category>>>> GetCategories()
    {
        var result = await _categoryService.GetCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<ServiceResponse<Category>>> GetCategory(int categoryId)
    {
        return Ok(await _categoryService.GetCategoryAsync(categoryId));
    }

}
