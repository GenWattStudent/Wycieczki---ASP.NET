using System.Linq.Expressions;
using Book.App.Filters;
using Book.App.Models;
using Book.App.Repositories;
using Book.App.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class TourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly FileService _fileService;

    private readonly string _tourFolder = "Tours";
    private readonly string _waypointFolder = "waypoints";

    public TourService(ITourRepository tourRepository, FileService fileService, ApplicationDbContext dbContext)
    {
        _tourRepository = tourRepository;
        _fileService = fileService;
        _dbContext = dbContext;
    }

    public async Task<List<TourModel>> GetTours(FilterModel filterModel)
    {
        List<Expression<Func<TourModel, bool>>> criterias = new() { t => t.StartDate >= DateTime.Now };
        var tours = await _tourRepository.GetBySpec(new TourFilterSpecification(filterModel, criterias));
        return tours;
    }

    public async Task<List<TourModel>> GetActiveTours()
    {
        return await _tourRepository.GetBySpec(new ActiveToursSpecification());
    }

    public async Task SaveTourImages(List<IFormFile> images, TourModel tour)
    {
        var imageUrls = await SaveImages(images, _tourFolder);

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
    }

    public async Task<TourModel> AddTour(TourModel tour)
    {
        _tourRepository.Add(tour);
        await _tourRepository.SaveAsync();

        return tour;
    }

    public async Task<TourModel?> GetTour(int id)
    {
        return await _tourRepository.GetSingleBySpec(new TourSpecification(id));
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
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            try
            {
                var dbTour = await GetTour(tour.Id);

                if (dbTour != null)
                {
                    Console.WriteLine("EditTour: " + tour.Description);
                    dbTour.EditTour(tour);
                    Console.WriteLine("EditTour2 " + dbTour.Description);
                    // Add new waypoints
                    foreach (var waypoint in editTourModel.Waypoints.Where(wm => wm.Id == 0))
                    {
                        List<string> imageUrlsToAdd = new();

                        if (waypoint.Images != null)
                        {
                            imageUrlsToAdd = await SaveImages(waypoint.Images, _waypointFolder);
                        }

                        var imagesToAdd = new List<ImageModel>();

                        foreach (var imageUrl in imageUrlsToAdd)
                        {
                            imagesToAdd.Add(new ImageModel
                            {
                                ImageUrl = imageUrl
                            });
                        }

                        var newWaypoint = new WaypointModel(waypoint);
                        newWaypoint.Images = imagesToAdd;
                        dbTour.Waypoints.Add(newWaypoint);
                    }

                    if (editTourModel.Images != null && editTourModel.Images.Count > 0)
                    {
                        await SaveTourImages(editTourModel.Images, dbTour);
                    }

                    await _tourRepository.SaveAsync();

                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("EditTour: Error");
                transaction.Rollback();
            }
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
                    _dbContext.Images.RemoveRange(tour.Images);

                    await _tourRepository.SaveAsync();
                    await _tourRepository.Remove(tour.Id);
                    // Delete all waypoints images
                    foreach (var waypoint in tour.Waypoints)
                    {
                        foreach (var image in waypoint.Images)
                        {
                            await _fileService.DeleteFile(image.ImageUrl);
                        }
                    }

                    await _tourRepository.SaveAsync();
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