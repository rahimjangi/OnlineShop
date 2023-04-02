using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.Server.Data;
using OnlineShop.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace OnlineShop.Server.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IConfiguration _config;

    public AuthService(DataContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<ServiceResponse<string>> Login(string email, string password)
    {
        //AppSettings
        var response = new ServiceResponse<string>();
        var user = await _context.Users.FirstOrDefaultAsync(p => p.Email.ToLower().Equals(email.ToLower()));
        if (user == null)
        {
            response.Success = false;
            response.Message = "User not found";
        }
        else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "User or password is not correct!";
        }
        else
        {
            response.Data = CreateToken(user);
        }


        return response;
    }

    private string? CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Email)
        };
        var key =
            new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        if (await UserExists(user.Email))
        {
            return new ServiceResponse<int>
            {
                Success = false,
                Message = "User already exists."
            };
        }
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordSalt = passwordSalt;
        user.PasswordHash = passwordHash;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new ServiceResponse<int>
        {
            Data = user.Id,
            Success = true,
            Message = "Registration Successful"
        };
    }

    public async Task<bool> UserExists(string email)
    {
        if (await _context.Users.AnyAsync(p => p.Email.ToLower().Equals(email.ToLower())))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreatePasswordHash(string password, out byte[] paswordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            paswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == userId);
        if (user == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Data = false,
                Message = "User not found."
            };
        }
        CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        await _context.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Success = true,
            Data = true,
            Message = "Password has been changed!"
        };
    }
}