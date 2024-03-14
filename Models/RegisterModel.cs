using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class Contact
{
    [EmailAddress]
    [Required]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;
    public int Phone { get; set; }

    public Contact()
    {
    }

    public Contact(ContactModel contact)
    {
        Email = contact.Email;
        Phone = contact.Phone;
    }
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public Address()
    {
    }

    public Address(AddressModel address)
    {
        Street = address.Street;
        City = address.City;
        Zip = address.Zip;
        Country = address.Country;
    }
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
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required]
    public Contact Contact { get; set; } = new();
    public Address Address { get; set; } = new();
    [Display(Name = "Profile Image")]
    public IFormFile? Image { get; set; }

    public RegisterModel()
    {
    }

    public RegisterModel(UserModel userModel)
    {
        Username = userModel.Username;
        Contact = userModel.Contact != null ? new Contact(userModel.Contact) : new();
        Address = userModel.Address != null ? new Address(userModel.Address) : new();
    }
}
