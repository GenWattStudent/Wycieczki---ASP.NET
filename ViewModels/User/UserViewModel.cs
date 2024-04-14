using Book.App.Models;

namespace Book.App.ViewModels
{
    public class UserViewModel
    {
        public EditUserViewModel EditUserViewModel { get; set; } = new();
        public string? ImagePath { get; set; }
        public TravelAgencyModel? TravelAgency { get; set; }
        public string Username { get; set; } = string.Empty;

        public UserViewModel(UserModel userModel)
        {
            Username = userModel.Username;
            EditUserViewModel = new(userModel);
            ImagePath = userModel.ImagePath;
            TravelAgency = userModel.TravelAgency;
        }

        public UserViewModel()
        {
        }
    }
}