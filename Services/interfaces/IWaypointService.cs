using Book.App.Models;
using Book.App.ViewModels;

namespace Book.App.Services;

public interface IWaypointService
{
    Task<WaypointModel?> Get(int id);
    Task RemoveImages(WaypointModel waypointModel);
    Task Delete(int id);
    Task Edit(AddTourWaypointsViewModel editTourWaypointsModel);
    Task AddImages(List<IFormFile> formFiles, WaypointModel waypointModel);
    Task Add(List<AddTourWaypointsViewModel> addTourWaypointsModel, int tourId);
}