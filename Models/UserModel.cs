using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public enum Role
{
    Admin,
    User
}

public class UserModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public Role Role { get; set; } = Role.User;
    public List<TourModel> Tours { get; set; } = new();
    public ContactModel? Contact { get; set; }
    public PreferencesModel Preferences { get; set; }
    public AddressModel? Address { get; set; }
}