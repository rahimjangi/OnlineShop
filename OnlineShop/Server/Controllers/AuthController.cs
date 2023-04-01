using Microsoft.AspNetCore.Mvc;
using OnlineShop.Shared;

namespace OnlineShop.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegister userRegister)
    {
        var response = await _authService.Register(
            new User
            {
                Email = userRegister.Email
            },
            userRegister.Password
        );
        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }
}
