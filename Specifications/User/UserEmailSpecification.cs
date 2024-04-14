using Book.App.Models;

namespace Book.App.Specifications;

public class UserEmailSpecification : BaseSpecification<UserModel>
{
    public UserEmailSpecification(string email)
    {
        AddInclude(x => x.Contact);
        ApplyCriteria(x => x.Contact.Email == email);
    }
}