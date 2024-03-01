using Book.App.Models;

namespace Book.App.ViewModels
{
    public class UserViewModel
    {
        public string Name { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}