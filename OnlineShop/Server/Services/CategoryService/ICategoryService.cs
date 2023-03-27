using OnlineShop.Shared;

namespace OnlineShop.Server.Services.CategoryService;

public interface ICategoryService
{
    Task<ServiceResponse<List<Category>>> GetCategoriesAsync();

    Task<ServiceResponse<Category>> GetCategoryAsync(int categoryId);


}
