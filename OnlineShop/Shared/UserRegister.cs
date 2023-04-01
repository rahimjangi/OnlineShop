using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Shared;

public class UserRegister
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, StringLength(20, MinimumLength = 5)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "The password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
