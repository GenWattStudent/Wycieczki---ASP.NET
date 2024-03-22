using Book.App.Models;

namespace Book.App.Filters;

public interface IFilter
{
    IQueryable<TourModel> Process(IQueryable<TourModel> tour);
}