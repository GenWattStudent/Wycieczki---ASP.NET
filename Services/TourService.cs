using System.Linq.Expressions;
using AutoMapper;
using Book.App.Models;
using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;
using Book.App.ViewModels;

namespace Book.App.Services;

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public TourService(IUnitOfWork unitOfWork, IFileService fileService, ApplicationDbContext dbContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<TourModel>> GetVisible(FilterModel filterModel)
    {
        List<Expression<Func<TourModel, bool>>> criterias = new() { t => t.StartDate >= DateTime.Now, t => t.IsVisible };
        var tours = await _unitOfWork.tourRepository.GetBySpec(new TourFilterSpecification(filterModel, criterias));
        return tours;
    }

    public async Task<List<TourModel>> Get(FilterModel filterModel)
    {
        List<Expression<Func<TourModel, bool>>> criterias = new() { t => t.StartDate >= DateTime.Now };
        var tours = await _unitOfWork.tourRepository.GetBySpec(new TourFilterSpecification(filterModel, criterias));
        return tours;
    }

    public async Task<List<TourModel>> GetActiveTours()
    {
        return await _unitOfWork.tourRepository.GetBySpec(new ActiveToursSpecification());
    }

    public async Task<TourModel> Add(TourModel tour)
    {
        _unitOfWork.tourRepository.Add(tour);
        await _unitOfWork.SaveAsync();

        return tour;
    }

    public async Task<TourModel?> GetById(int id)
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
    public async Task Edit(AddTourViewModel addTourModel)
    {
        var dbTour = await GetById(addTourModel.Id);

        if (dbTour != null)
        {
            _mapper.Map(addTourModel, dbTour);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task Delete(int id)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            try
            {
                var tour = await GetById(id);

                if (tour != null)
                {
                    foreach (var image in tour.Images)
                    {
                        _fileService.DeleteFile(image.ImageUrl);
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
                            _fileService.DeleteFile(image.ImageUrl);
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

    public async Task<List<TourModel>> GetByAgencyIdAsync(int id)
    {
        return await _unitOfWork.tourRepository.GetBySpec(new TourByAgencyIdSpecification(id));
    }

    public int GetCount(FilterModel filterModel)
    {
        List<Expression<Func<TourModel, bool>>> criterias = new() { t => t.StartDate >= DateTime.Now, t => t.IsVisible };
        var newFilterModel = new FilterModel(filterModel)
        {
            Page = 0
        };
        return _unitOfWork.tourRepository.Count(new TourFilterSpecification(newFilterModel, criterias));
    }
}