using Book.App.Models;

namespace Book.App.ViewModels
{
    public class UserViewModel
    {
        public string Name { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string? ImagePath { get; set; }
        public ContactModel? Contact { get; set; }
        public AddressModel? Address { get; set; }

        public UserViewModel(UserModel userModel)
        {
            Name = userModel.Username;
            Role = userModel.Role;
            ImagePath = userModel.ImagePath;
            Contact = userModel.Contact;
            Address = userModel.Address;
        }

        public UserViewModel()
        {
        }
    }
}