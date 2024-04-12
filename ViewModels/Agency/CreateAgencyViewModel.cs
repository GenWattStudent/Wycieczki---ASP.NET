using Book.App.Models;

namespace Book.App.ViewModels;

public class CreateAgencyViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IFormFile? VideoFile { get; set; }
    public IFormFile? LogoFile { get; set; }
    public AddressModel Address { get; set; } = new AddressModel();
    public bool IsAccepted { get; set; } = false;
    public string Reason { get; set; } = string.Empty;

}