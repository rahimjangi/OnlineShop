using Microsoft.EntityFrameworkCore;
using OnlineShop.Server.Data;
using OnlineShop.Shared;

namespace OnlineShop.Server.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly DataContext _context;

    public CategoryService(DataContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<Category>>> GetCategoriesAsync()
    {
        var response = new ServiceResponse<List<Category>>();
        var categories = await _context.Categories.ToListAsync();
        if (categories != null && categories.Count != 0)
        {
            response.Success = true;
            response.Data = categories;
        }
        else
        {
            response.Message = "There is no category to show!";
        }

        return response;
    }

    public async Task<ServiceResponse<Category>> GetCategoryAsync(int categoryId)
    {
        var response = new ServiceResponse<Category>();
        var category = await _context.Categories.FindAsync(categoryId);
        if (category != null)
        {
            response.Success = true;
            response.Data = category;
        }
        else
        {
            response.Success = false;
            response.Message = "Category does not exist";
        }

        return response;
    }
}
