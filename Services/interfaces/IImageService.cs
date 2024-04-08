namespace Book.App.Services;

public interface IImageService
{
    Task Delete(int id);
    Task AddImagesToTour(List<IFormFile> files, int tourId);
    Task AddImagesToWaypoint(List<IFormFile> files, int waypointId);
}