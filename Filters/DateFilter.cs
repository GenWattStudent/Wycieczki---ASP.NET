using Book.App.Models;

namespace Book.App.Filters;

public class DateFilter : IFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public DateFilter(DateTime? startDate, DateTime? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public IQueryable<TourModel> Process(IQueryable<TourModel> tours)
    {
        return tours.Where(t => (StartDate == null || t.StartDate >= StartDate) && (EndDate == null || t.StartDate <= EndDate));
    }
}