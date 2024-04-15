using Book.App.Models;
using Book.App.ViewModels;

namespace Book.App.Services;

public interface IWaypointService
{
    Task<WaypointModel?> Get(int id);
    Task RemoveImages(WaypointModel waypointModel);
    Task Delete(WaypointModel waypoint);
    Task<WaypointModel?> Edit(WaypointModel waypoint);
    Task AddImages(List<IFormFile> formFiles, WaypointModel waypointModel);
    Task Add(List<AddTourWaypointsViewModel> addTourWaypointsModel, int tourId);
}