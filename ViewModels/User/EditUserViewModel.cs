using Book.App.Models;

namespace Book.App.ViewModels;

public class EditUserViewModel
{
    public ContactModel Contact { get; set; } = new();
    public AddressModel Address { get; set; } = new();
    public IFormFile? Image { get; set; }

    public EditUserViewModel(UserModel userModel)
    {
        Contact = userModel.Contact;
        Address = userModel.Address;
    }

    public EditUserViewModel()
    {
    }
}