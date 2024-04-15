using Book.App.Models;

namespace Book.App.Specifications;

public class TourByAgencyIdSpecification : BaseSpecification<TourModel>
{
    public TourByAgencyIdSpecification(int agencyId) : base()
    {
        Includes.Add(t => t.Images);
        Includes.Add(t => t.Reservations);
        Includes.Add(t => t.Waypoints);
        Includes.Add(t => t.Users);
        Includes.Add(t => t.TravelAgency);

        ApplyCriteria(t => t.TravelAgencyId == agencyId);
    }
}