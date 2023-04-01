using OnlineShop.Shared;
using System.Net.Http.Json;

namespace OnlineShop.Client.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<string>> Login(UserLogin request)
    {
        var result = await _http.PostAsJsonAsync("api/auth/login", request);
        var response = await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        return response!;

    }

    public async Task<ServiceResponse<int>> Register(UserRegister request)
    {
        var result = await _http.PostAsJsonAsync("api/auth/register", request);
        var response = await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        return response!;
    }
}
