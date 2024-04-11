using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class AddressModel : BaseEntity
{
    [Required]
    public string Street { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string Zip { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    public int UserId { get; set; }
    public UserModel? User { get; set; } = new();

    public TravelAgencyModel? TravelAgency { get; set; }

    public AddressModel(Address address)
    {
        Street = address.Street;
        City = address.City;
        Zip = address.Zip;
        Country = address.Country;
    }

    public AddressModel()
    {
    }

    public void SetAddress(Address address)
    {
        Street = address.Street;
        City = address.City;
        Zip = address.Zip;
        Country = address.Country;
    }
}