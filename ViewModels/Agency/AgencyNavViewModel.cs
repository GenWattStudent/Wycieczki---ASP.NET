using Book.App.Models;

namespace Book.App.ViewModels;

public class AgencyNavViewModel
{
    public TravelAgencyModel? TravelAgency { get; set; }
    public List<RoleModel> Roles { get; set; } = new();
}
