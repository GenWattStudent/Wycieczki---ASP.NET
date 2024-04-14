using System.ComponentModel.DataAnnotations;
using Book.App.Models;

namespace Book.App.ViewModels;

public class RegisterViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
    public ContactModel Contact { get; set; } = new();
    public AddressModel Address { get; set; } = new();
    [Display(Name = "Profile Image")]
    public IFormFile? Image { get; set; }

    public RegisterViewModel()
    {
    }

    public RegisterViewModel(UserModel userModel)
    {
        Username = userModel.Username;
        Contact = userModel.Contact != null ? new ContactModel(userModel.Contact) : new();
        Address = userModel.Address != null ? new AddressModel(userModel.Address) : new();
    }
}
