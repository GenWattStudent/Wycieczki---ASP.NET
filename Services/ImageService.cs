using Book.App.Repositories;

namespace Book.App.Services;

public class ImageService
{
    private readonly IImageRepository _imageRepository;
    private readonly FileService _fileService;

    public ImageService(IImageRepository imageRepository, FileService fileService)
    {
        _imageRepository = imageRepository;
        _fileService = fileService;
    }

    public async Task DeleteImage(int id)
    {
        var image = await _imageRepository.GetById(id);

        if (image != null)
        {
            await _imageRepository.Delete(image.Id);
            await _fileService.DeleteFile(image.ImageUrl);
            await _imageRepository.SaveAsync();
        }
    }
}