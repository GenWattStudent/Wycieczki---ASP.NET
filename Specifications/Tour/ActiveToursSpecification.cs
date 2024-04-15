using Book.App.Models;

namespace Book.App.Specifications;

public class ActiveToursSpecification : BaseSpecification<TourModel>
{
    public ActiveToursSpecification() : base()
    {
        ApplyCriteria(t => t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now);
        Includes.Add(t => t.Images);
        Includes.Add(t => t.Reservations);
        IncludeStrings.Add("Reservations.User");
    }
}