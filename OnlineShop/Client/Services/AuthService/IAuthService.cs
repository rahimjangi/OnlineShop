using OnlineShop.Shared;

namespace OnlineShop.Client.Services.AuthService;

public interface IAuthService
{
    Task<ServiceResponse<int>> Register(UserRegister request);
}
