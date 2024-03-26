using Book.App.Filters;
using Book.App.Models;
using Book.App.Repositories;
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

    public async Task<List<TourModel>> GetTours()
    {
        return await _tourRepository.GetTours().ToListAsync();
    }

    public async Task<List<TourModel>> GetTours(FilterModel filterModel)
    {
        var tours = _tourRepository.GetTours();
        var filters = new List<IFilter> { new SearchFilter(filterModel.SearchString), new PriceFilter(filterModel.MinPrice, filterModel.MaxPrice) };

        tours = filters.Aggregate(tours, (current, filter) => filter.Process(current));

        switch (filterModel.OrderBy)
        {
            case OrderBy.Date:
                tours = filterModel.OrderDirection == OrderDirection.Asc ? tours.OrderBy(t => t.StartDate) : tours.OrderByDescending(t => t.StartDate);
                break;
            case OrderBy.Price:
                tours = filterModel.OrderDirection == OrderDirection.Asc ? tours.OrderBy(t => t.Price) : tours.OrderByDescending(t => t.Price);
                break;
        }

        return await tours.ToListAsync();

    }

    public async Task<List<TourModel>> GetActiveTours()
    {
        return await _tourRepository.GetActiveTours().ToListAsync();
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

    public async Task AddTour(TourModel tour, AddTourModel addTourModel)
    {
        foreach (var waypoint in addTourModel.Waypoints)
        {
            List<string> waypointImageUrls = new();

            if (waypoint.Images != null)
            {
                waypointImageUrls = await SaveImages(waypoint.Images, _waypointFolder);
            }

            tour.Waypoints.Add(new WaypointModel(waypoint));

            foreach (var imageUrl in waypointImageUrls)
            {
                tour.Waypoints.Last().Images.Add(new ImageModel
                {
                    ImageUrl = imageUrl
                });
            }
        }

        if (addTourModel.Images != null && addTourModel.Images.Count > 0)
        {
            await SaveTourImages(addTourModel.Images, tour);
        }

        _tourRepository.Add(tour);
        await _tourRepository.SaveAsync();
    }

    public async Task<TourModel?> GetTour(int id)
    {
        return await _tourRepository.GetTour(id).FirstOrDefaultAsync();
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
                    dbTour.EditTour(tour);

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
                var tour = await _tourRepository.GetTour(id).FirstOrDefaultAsync();

                if (tour != null)
                {
                    foreach (var image in tour.Images)
                    {
                        await _fileService.DeleteFile(image.ImageUrl);
                    }

                    // remove images from db
                    _dbContext.Images.RemoveRange(tour.Images);

                    await _tourRepository.SaveAsync();
                    await _tourRepository.Delete(tour.Id);
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