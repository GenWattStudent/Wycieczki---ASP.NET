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
    public string? ImagePath { get; set; }

    public UserModel(RegisterModel registerModel)
    {
        Username = registerModel.Username;
        Password = registerModel.Password;
        Contact = new ContactModel
        {
            Email = registerModel.Contact.Email,
            Phone = registerModel.Contact.Phone
        };
        Address = new AddressModel
        {
            Street = registerModel.Address.Street,
            City = registerModel.Address.City,
            Zip = registerModel.Address.ZipCode,
            Country = registerModel.Address.Country
        };
        Preferences = new PreferencesModel();
    }

    public UserModel()
    {
    }
}