using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class RegisterModel
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
