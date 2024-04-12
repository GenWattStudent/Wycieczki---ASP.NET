using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public enum Role
{
    Admin,
    User,
    AgencyAdmin,
    AgencyManager,
    AgencyUser
}

public class RoleModel : BaseEntity
{
    [Required]
    public Role Role { get; set; }
    public List<UserModel> Users { get; set; } = new();
}

public class UserModel : BaseEntity
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    public int? ContactId { get; set; }
    public string? ImagePath { get; set; }
    public int? TravelAgencyId { get; set; }

    public ContactModel? Contact { get; set; }
    public PreferencesModel Preferences { get; set; }
    public AddressModel? Address { get; set; }
    public TravelAgencyModel? TravelAgency { get; set; }

    public List<TourModel> Tours { get; set; } = new();
    public List<ReservationModel> Reservations { get; set; } = new();
    public List<RoleModel> Roles { get; set; } = new();

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
            Zip = registerModel.Address.Zip,
            Country = registerModel.Address.Country
        };
        Preferences = new PreferencesModel();
    }

    public UserModel()
    {
    }
}