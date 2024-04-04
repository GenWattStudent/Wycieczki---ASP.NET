using Book.App.Models;
using Book.App.ViewModels;

namespace Book.App.Services;

public interface ITourService
{
    Task<List<TourModel>> Get(FilterModel filterModel);
    Task<List<TourModel>> GetActiveTours();
    Task<TourModel> Add(TourModel tour);
    Task<TourModel?> GetById(int id);
    Task<List<string>> SaveImages(List<IFormFile> images, string folder);
    Task Edit(TourModel tour, EditTourViewModel editTourModel);
    Task Delete(int id);
}