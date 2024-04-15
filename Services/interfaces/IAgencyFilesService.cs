namespace Book.App.Services;

public interface IAgencyFilesService
{
    Task AddGalleryAsync(int agencyId, List<IFormFile> files);
}