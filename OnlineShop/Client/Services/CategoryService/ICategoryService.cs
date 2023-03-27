using OnlineShop.Shared;

namespace OnlineShop.Client.Services.CategoryService;

public interface ICategoryService
{
    List<Category> Categories { get; set; }
    Task GetCategoriesAsync();

    Task<ServiceResponse<Category>> GetCategoryAsync(int categoryId);
}
