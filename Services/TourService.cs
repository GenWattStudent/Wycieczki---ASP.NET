using Book.App.Filters;
using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class TourService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _tourFolder = "Tours";
    private readonly string _waypointFolder = "waypoints";
    private readonly FileService _fileService;

    public TourService(ApplicationDbContext dbContext, FileService fileService)
    {
        _dbContext = dbContext;
        _fileService = fileService;
    }

    public async Task<List<TourModel>> GetTours()
    {
        return await _dbContext.Tours.Include(t => t.Images)
                                    .Include(t => t.Reservations).ThenInclude(r => r.User)
                                    .Where(t => t.StartDate >= DateTime.Now).ToListAsync();
    }

    public async Task<List<TourModel>> GetTours(FilterModel filterModel)
    {
        var tours = _dbContext.Tours.Include(t => t.Images).Include(t => t.Reservations).ThenInclude(r => r.User).Where(t => t.StartDate >= DateTime.Now);
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
        return await _dbContext.Tours.Include(t => t.Images).Include(t => t.Users).Where(t => t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now).ToListAsync();
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

        _dbContext.Tours.Add(tour);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TourModel?> GetTour(int id)
    {
        return await _dbContext.Tours
                            .Include(t => t.Waypoints).ThenInclude(w => w.Images)
                            .Include(t => t.Reservations).ThenInclude(r => r.User).ThenInclude(u => u.Contact)
                            .Include(t => t.Images).FirstOrDefaultAsync(t => t.Id == id);
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

    public void RemoveImage(string imageUrl)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task EditTour(TourModel tour, EditTourModel editTourModel)
    {
        // Edit tour, add images. Update waypoints that already existed in db and add new ones
        using (var transaction = _dbContext.Database.BeginTransaction())
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

                await _dbContext.SaveChangesAsync();

                transaction.Commit();
            }
        }
    }

    public void DeleteTour(int id)
    {
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            try
            {
                var tour = _dbContext.Tours.Include(t => t.Waypoints).Include(t => t.Images).FirstOrDefault(t => t.Id == id);

                if (tour != null)
                {
                    foreach (var image in tour.Images)
                    {
                        RemoveImage(image.ImageUrl);
                    }

                    // remove images from db
                    _dbContext.Images.RemoveRange(tour.Images);
                    _dbContext.SaveChanges();

                    _dbContext.Tours.Remove(tour);
                    // Delete all waypoints images
                    foreach (var waypoint in tour.Waypoints)
                    {
                        foreach (var image in waypoint.Images)
                        {
                            RemoveImage(image.ImageUrl);
                        }
                    }

                    _dbContext.SaveChanges();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }

    public async Task DeleteImage(int id)
    {
        var image = await _dbContext.Images.FirstOrDefaultAsync(i => i.Id == id);
        if (image != null)
        {
            RemoveImage(image.ImageUrl);
            _dbContext.Images.Remove(image);
            await _dbContext.SaveChangesAsync();
        }
    }
}