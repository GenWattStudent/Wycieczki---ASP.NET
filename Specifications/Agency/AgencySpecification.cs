using Book.App.Models;

namespace Book.App.Specifications;

public class AgencySpecification : BaseSpecification<TravelAgencyModel>
{
    public AgencySpecification()
    {
        AddInclude(x => x.Address);
        AddInclude(x => x.Users);
        AddInclude(x => x.Images);
    }

    public void SetNotAccepted()
    {
        ApplyCriteria(x => !x.IsAccepted && string.IsNullOrEmpty(x.Reason));
    }

    public void HasAgency(int userId)
    {
        ApplyCriteria(x => x.Users.Any(u => u.Id == userId));
    }

    public void ByUserId(int userId)
    {
        IncludeStrings.Add("Users.Roles");
        ApplyCriteria(x => x.Users.Any(u => u.Id == userId));
    }

    public void ById(int id)
    {
        IncludeStrings.Add("Users.Roles");
        ApplyCriteria(x => x.Id == id);
    }
}