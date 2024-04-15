using Book.App.Models;

namespace Book.App.Specifications;

public class UserSpecification : BaseSpecification<UserModel>
{
    public UserSpecification()
    {
        AddInclude(x => x.Contact);
        AddInclude(x => x.Address);
    }

    public UserSpecification(int id)
    {
        AddInclude(x => x.Contact);
        AddInclude(x => x.Address);
        ApplyCriteria(x => x.Id == id);
    }

    public UserSpecification(string username)
    {
        AddInclude(x => x.Contact);
        AddInclude(x => x.Address);
        AddInclude(x => x.Roles);
        AddInclude(x => x.TravelAgency);

        ApplyCriteria(x => x.Username.ToLower() == username.ToLower());
    }
}