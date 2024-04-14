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
    [Required]
    public string Street { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string Zip { get; set; } = string.Empty;
    [Required]
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

