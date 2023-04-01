using Microsoft.EntityFrameworkCore;
using OnlineShop.Server.Data;
using OnlineShop.Shared;
using System.Security.Cryptography;

namespace OnlineShop.Server.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly DataContext _context;

    public AuthService(DataContext context)
    {
        _context = context;
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
        using (var hmac = new HMACSHA256())
        {
            passwordSalt = hmac.Key;
            paswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}