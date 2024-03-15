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
        return await _dbContext.Tours.Include(t => t.Images).Include(t => t.Users).Where(t => t.StartDate >= DateTime.Now).ToListAsync();
    }

    public async Task AddTour(TourModel tour, AddTourModel addTourModel, List<string> imageUrls)
    {
        foreach (var waypoint in addTourModel.Waypoints)
        {
            List<string> waypointImageUrl = new();

            if (waypoint.Images != null)
            {
                SaveImages(waypoint.Images, _waypointFolder, out waypointImageUrl);
            }

            tour.Waypoints.Add(new WaypointModel
            {
                Name = waypoint.Name,
                Latitude = waypoint.Lat,
                Longitude = waypoint.Lng,
                Description = waypoint.Description,
                IsRoad = waypoint.IsRoad,
                Type = waypoint.Type
            });

            foreach (var imageUrl in waypointImageUrl)
            {
                tour.Waypoints.Last().Images.Add(new ImageModel
                {
                    ImageUrl = imageUrl
                });
            }
        }

        foreach (var imageUrl in imageUrls)
        {
            tour.Images.Add(new ImageModel
            {
                ImageUrl = imageUrl
            });
        }

        _dbContext.Tours.Add(tour);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TourModel?> GetTour(int id)
    {
        return await _dbContext.Tours.Include(t => t.Waypoints).ThenInclude(w => w.Images).Include(t => t.Users).ThenInclude(u => u.Contact).Include(t => t.Images).FirstOrDefaultAsync(t => t.Id == id);
    }

    public void SaveImages(List<IFormFile> images, string folder, out List<string> imageUrls)
    {
        imageUrls = new List<string>();

        foreach (var image in images)
        {
            SaveImage(image, folder, out string imageUrl);
            imageUrls.Add(imageUrl);
        }
    }

    public void SaveImage(IFormFile image, string folder, out string imageUrl)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folder, fileName);

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image.CopyTo(fileStream);
        }
        imageUrl = $"/images/{folder}/{fileName}";
    }

    public void RemoveImage(string imageUrl)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task EditTour(TourModel tour, EditTourModel editTourModel, List<string> imageUrls)
    {
        // Edit tour, add images. Update waypoints that already existed in db and add new ones
        using (var transaction = _dbContext.Database.BeginTransaction())
        {
            var dbTour = await GetTour(tour.Id);

            if (dbTour != null)
            {
                dbTour.Name = tour.Name;
                dbTour.Description = tour.Description;
                dbTour.Price = tour.Price;
                dbTour.StartDate = tour.StartDate;
                dbTour.EndDate = tour.EndDate;
                dbTour.MaxUsers = tour.MaxUsers;

                // Add new images
                foreach (var imageUrl in imageUrls)
                {
                    if (!dbTour.Images.Any(i => i.ImageUrl == imageUrl))
                    {
                        dbTour.Images.Add(new ImageModel
                        {
                            ImageUrl = imageUrl
                        });
                    }
                }

                // Add new waypoints
                foreach (var waypoint in editTourModel.Waypoints.Where(wm => wm.Id == 0))
                {
                    List<string> imageUrlsToAdd = new();

                    if (waypoint.Images != null)
                    {
                        SaveImages(waypoint.Images, _waypointFolder, out imageUrlsToAdd);
                    }

                    var imagesToAdd = new List<ImageModel>();

                    foreach (var imageUrl in imageUrlsToAdd)
                    {
                        imagesToAdd.Add(new ImageModel
                        {
                            ImageUrl = imageUrl
                        });
                    }

                    dbTour.Waypoints.Add(new WaypointModel
                    {
                        Name = waypoint.Name,
                        Latitude = waypoint.Lat,
                        Longitude = waypoint.Lng,
                        Description = waypoint.Description,
                        Images = imagesToAdd,
                        IsRoad = waypoint.IsRoad,
                        Type = waypoint.Type
                    });
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