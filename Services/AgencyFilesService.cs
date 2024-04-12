
using Book.App.Models;
using Book.App.Repositories.UnitOfWork;

namespace Book.App.Services;

public class AgencyFilesService : IAgencyFilesService
{
    private readonly IAgencyService _agencyService;
    private readonly IFileService _fileService;
    private readonly IUnitOfWork _unitOfWork;
    public readonly string galleryFileName = "agencyGallery";

    public AgencyFilesService(IAgencyService agencyService, IFileService fileService, IUnitOfWork unitOfWork)
    {
        _agencyService = agencyService;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
    }

    public async Task AddGalleryAsync(int agencyId, List<IFormFile> files)
    {
        var agency = await _agencyService.GetByIdAsync(agencyId);

        if (agency == null)
        {
            throw new Exception("Agency not found");
        }

        var filePaths = await _fileService.SaveFiles(files, galleryFileName);

        foreach (var filePath in filePaths)
        {
            agency.Images.Add(new ImageModel { ImageUrl = filePath });
        }

        _unitOfWork.agencyRepository.Update(agency);
        await _unitOfWork.SaveAsync();
    }
}