using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class AddressModel
{
    public int Id { get; set; }
    [Required]
    public string Street { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string Zip { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    public int UserId { get; set; }
    public UserModel User { get; set; } = new();
}