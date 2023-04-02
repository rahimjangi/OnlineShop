using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Shared;

public class UserChangePassword
{
    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [Required, Compare(nameof(Password), ErrorMessage = "Password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
