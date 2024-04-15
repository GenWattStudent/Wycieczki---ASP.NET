using Book.App.Models;

namespace Book.App.Specifications;

public class TourSpecification : BaseSpecification<TourModel>
{
    public TourSpecification(int id) : base()
    {
        Includes.Add(t => t.Images);
        Includes.Add(t => t.Reservations);
        Includes.Add(t => t.Waypoints);
        Includes.Add(t => t.Users);
        Includes.Add(t => t.Meals);
        Includes.Add(t => t.TravelAgency);
        IncludeStrings.Add("TravelAgency.Users");
        IncludeStrings.Add("Reservations.User");
        IncludeStrings.Add("Waypoints.Images");
        ApplyCriteria(t => t.Id == id);
    }
}