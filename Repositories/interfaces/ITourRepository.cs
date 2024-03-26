using Book.App.Models;

namespace Book.App.Repositories;

public interface ITourRepository : IRepository<TourModel>
{
    IQueryable<TourModel> GetActiveTours();
    IQueryable<TourModel> GetTour(int id);
    IQueryable<TourModel> GetTours();
}