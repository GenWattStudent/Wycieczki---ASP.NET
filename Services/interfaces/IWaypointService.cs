using Book.App.Models;

namespace Book.App.Services;

public interface IWaypointService
{
    Task<WaypointModel?> GetWaypoint(int id);
    Task RemoveWaypointImages(WaypointModel waypointModel);
    Task AddImagesToWaypoint(List<IFormFile> formFiles, WaypointModel waypointModel);
    Task Delete(int id);
    Task Edit(AddTourWaypointsModel editTourWaypointsModel);
    Task AddImages(List<IFormFile> formFiles, WaypointModel waypointModel);
    Task Add(List<AddTourWaypointsModel> addTourWaypointsModel, int tourId);
}