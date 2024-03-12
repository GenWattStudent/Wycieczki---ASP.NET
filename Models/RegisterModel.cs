using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class Contact
{
    public string Email { get; set; } = string.Empty;
    public int Phone { get; set; }
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

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
    [Required]
    public Contact Contact { get; set; } = new();
    public Address Address { get; set; } = new();
    public IFormFile? Image { get; set; }
}
