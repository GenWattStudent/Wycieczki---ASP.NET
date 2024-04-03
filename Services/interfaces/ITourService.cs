using Book.App.Models;

namespace Book.App.Services;

public interface ITourService
{
    Task<List<TourModel>> GetTours(FilterModel filterModel);
    Task<List<TourModel>> GetActiveTours();
    Task<TourModel> AddTour(TourModel tour);
    Task<TourModel?> GetTour(int id);
    Task<List<string>> SaveImages(List<IFormFile> images, string folder);
    Task EditTour(TourModel tour, EditTourModel editTourModel);
    Task DeleteTour(int id);
}