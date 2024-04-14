using System.ComponentModel.DataAnnotations;
using Book.App.Models;

namespace Book.App.ViewModels;

public class EditAgencyViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Display(Name = "Choose your promo video")]
    public IFormFile? VideoFile { get; set; }
    [Display(Name = "Choose your logo")]
    public IFormFile? LogoFile { get; set; }
    public AddressModel Address { get; set; } = new();
}