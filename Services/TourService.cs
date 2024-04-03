using System.Linq.Expressions;
using Book.App.Models;
using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;

namespace Book.App.Services;

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileService _fileService;

    public TourService(IUnitOfWork unitOfWork, IFileService fileService, ApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _dbContext = dbContext;
    }

    public async Task<List<TourModel>> GetTours(FilterModel filterModel)
    {
        List<Expression<Func<TourModel, bool>>> criterias = new() { t => t.StartDate >= DateTime.Now };
        var tours = await _unitOfWork.tourRepository.GetBySpec(new TourFilterSpecification(filterModel, criterias));
        return tours;
    }

    public async Task<List<TourModel>> GetActiveTours()
    {
        return await _unitOfWork.tourRepository.GetBySpec(new ActiveToursSpecification());
    }

    public async Task<TourModel> AddTour(TourModel tour)
    {
        _unitOfWork.tourRepository.Add(tour);
        await _unitOfWork.SaveAsync();

        return tour;
    }

    public async Task<TourModel?> GetTour(int id)
    {
        return await _unitOfWork.tourRepository.GetSingleBySpec(new TourSpecification(id));
    }

    public async Task<List<string>> SaveImages(List<IFormFile> images, string folder)
    {
        var imageUrls = new List<string>();

        foreach (var image in images)
        {
            var path = await _fileService.SaveFile(image, folder);
            imageUrls.Add(path);
        }

        return imageUrls;
    }
    public async Task EditTour(TourModel tour, EditTourModel editTourModel)
    {
        try
        {
            var dbTour = await GetTour(tour.Id);

            if (dbTour != null)
            {
                dbTour.EditTour(tour);
                await _unitOfWork.SaveAsync();
            }
        }
        catch (Exception)
        {
            Console.WriteLine("EditTour: Error");
        }
    }

    public async Task DeleteTour(int id)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            try
            {
                var tour = await GetTour(id);

                if (tour != null)
                {
                    foreach (var image in tour.Images)
                    {
                        await _fileService.DeleteFile(image.ImageUrl);
                    }

                    // remove images from db
                    _unitOfWork.imageRepository.RemoveRange(tour.Images);

                    await _unitOfWork.SaveAsync();
                    await _unitOfWork.tourRepository.Remove(tour.Id);
                    // Delete all waypoints images
                    foreach (var waypoint in tour.Waypoints)
                    {
                        foreach (var image in waypoint.Images)
                        {
                            await _fileService.DeleteFile(image.ImageUrl);
                        }
                    }

                    await _unitOfWork.SaveAsync();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }
}