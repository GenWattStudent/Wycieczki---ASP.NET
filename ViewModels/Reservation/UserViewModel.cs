using Book.App.Models;

namespace Book.App.ViewModels
{
    public class UserViewModel
    {
        public RegisterModel RegisterModel { get; set; } = new();
        public string? ImagePath { get; set; }
        public TravelAgencyModel? TravelAgency { get; set; }

        public UserViewModel(UserModel userModel)
        {
            RegisterModel = new(userModel);
            ImagePath = userModel.ImagePath;
            TravelAgency = userModel.TravelAgency;
        }

        public UserViewModel()
        {
        }
    }
}