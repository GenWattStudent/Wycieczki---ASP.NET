using Book.App.Models;

namespace Book.App.Specifications;

public class ReservationSpecification : BaseSpecification<ReservationModel>
{
    public ReservationSpecification(int userId) : base()
    {
        SetIncudes();
        ApplyCriteria(r => r.User.Id == userId);
    }

    private void SetIncudes()
    {
        Includes.Add(r => r.Tour);
        Includes.Add(r => r.User);
        IncludeStrings.Add("Tour.Images");
        IncludeStrings.Add("Tour.Waypoints");
        IncludeStrings.Add("Tour.Waypoints.Images");
        IncludeStrings.Add("Tour.Meals");
    }

    public void FilterByTour(int tourId)
    {
        ApplyCriteria(r => r.Tour.Id == tourId);
    }

    public void ActiveAndFutureReservations()
    {
        ApplyCriteria(r => r.Tour.StartDate >= DateTime.Now || r.Tour.EndDate >= DateTime.Now);
        ApplyOrderBy(r => r.Tour.StartDate);
    }

    public void HistoryReservations()
    {
        ApplyCriteria(r => r.Tour.EndDate < DateTime.Now);
        ApplyOrderByDescending(r => r.Tour.StartDate);
    }

    public void ClosestReservation()
    {
        ApplyOrderBy(r => r.Tour.StartDate);
        ApplyOrderBy(r => r.Tour.EndDate);
        ApplyCriteria(r => r.Tour.StartDate >= DateTime.Now || r.Tour.EndDate >= DateTime.Now);
    }
}