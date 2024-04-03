namespace Book.App.Services;

public interface IImageService
{
    Task DeleteImage(int id);
    Task AddImagesToTour(List<IFormFile> files, int tourId);
}