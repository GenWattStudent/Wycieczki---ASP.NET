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
    Task Edit(AddTourViewModel editTourModel);
    Task Delete(int id);
    Task<List<TourModel>> GetVisible(FilterModel filterModel);
    Task<List<TourModel>> GetByAgencyIdAsync(int id);
    int GetCount(FilterModel filterModel);
}