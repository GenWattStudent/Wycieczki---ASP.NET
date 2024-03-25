
using Book.App.Models;

namespace Book.App.Repositories;

public interface ITourRepository : IRepository<TourModel>
{
    Task<List<TourModel>> GetTours();
    Task<List<TourModel>> GetTours(FilterModel filterModel);
    Task<List<TourModel>> GetActiveTours();
    Task<TourModel?> GetTour(int id);
}