using Book.App.Models;
using Book.App.Repositories.UnitOfWork;

namespace Book.App.Services;

public class ImageService : IImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly string _tourFolder = "Tours";

    public ImageService(IUnitOfWork unitOfWork, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
    }

    public async Task Delete(int id)
    {
        var image = await _unitOfWork.imageRepository.GetById(id);

        if (image != null)
        {
            await _unitOfWork.imageRepository.Remove(image.Id);
            await _fileService.DeleteFile(image.ImageUrl);
            await _unitOfWork.imageRepository.SaveAsync();
        }
    }

    // public async Task<List<ImageModel>> GetTourImages(int tourId)
    // {
    //     // return await _imageRepository.GetBySpec;
    // }

    public async Task AddImagesToTour(List<IFormFile> files, int tourId)
    {
        var tour = await _unitOfWork.tourRepository.GetById(tourId);

        if (tour != null)
        {
            var imageUrls = await _fileService.SaveFiles(files, _tourFolder);

            foreach (var imageUrl in imageUrls)
            {
                if (!tour.Images.Any(i => i.ImageUrl == imageUrl))
                {
                    tour.Images.Add(new ImageModel
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            await _unitOfWork.imageRepository.SaveAsync();
        }
    }
}