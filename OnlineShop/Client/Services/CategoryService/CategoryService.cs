
using OnlineShop.Shared;
using System.Net.Http.Json;

namespace OnlineShop.Client.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _http;

    public CategoryService(HttpClient http)
    {
        _http = http;
    }

    public List<Category> Categories { get; set; } = new List<Category>();

    public async Task GetCategoriesAsync()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/category");
        if (result != null && result.Data != null)
        {
            Categories = result.Data;
        }

    }

    public async Task<ServiceResponse<Category>> GetCategoryAsync(int categoryId)
    {
        var response = new ServiceResponse<Category>();
        var result = await _http.GetFromJsonAsync<ServiceResponse<Category>>($"api/category/{categoryId.ToString()}");
        if (result != null && result.Data != null)
        {
            response.Data = result.Data;

        }
        else
        {
            response.Success = false;
            response.Message = "Category does not exist";
        }
        return response;
    }
}
