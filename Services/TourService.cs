using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class TourService
{
    private readonly ApplicationDbContext _dbContext;

    public TourService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TourModel>> GetTours()
    {
        return await _dbContext.Tours.ToListAsync();
    }

    public async Task AddTour(TourModel tour, List<AddTourWaypointsModel> waypointModel)
    {
        foreach (var waypoint in waypointModel)
        {
            string imageUrl = string.Empty;

            if (waypoint.Image != null)
            {
                imageUrl = await SaveBase64Image(waypoint.Image.Split(",")[1]);
            }

            tour.Waypoints.Add(new WaypointModel
            {
                Name = waypoint.Name,
                Latitude = waypoint.Lat,
                Longitude = waypoint.Lng,
                Description = waypoint.Description,
                ImageUrl = imageUrl
            });
        }
        _dbContext.Tours.Add(tour);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<TourModel?> GetTour(int id)
    {
        return await _dbContext.Tours.Include(t => t.Waypoints).Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<string> SaveBase64Image(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        var fileName = $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "waypoints", fileName);

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        await File.WriteAllBytesAsync(filePath, bytes);
        return $"/images/waypoints/{fileName}";
    }

    public void SaveImage(IFormFile image, out string imageUrl)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image.CopyTo(fileStream);
        }
        imageUrl = $"/images/{fileName}";
    }

    public void RemoveImage(string imageUrl)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    // public async Task UpdateTour(TourViewModel tour)
    // {
    //     var existingTour = await _dbContext.Tours.FirstOrDefaultAsync(t => t.Id == tour.Id);

    //     if (existingTour != null)
    //     {
    //         existingTour.Name = tour.Name;
    //         existingTour.Description = tour.Description;
    //         existingTour.ImageUrl = tour.ImageUrl;
    //         existingTour.Price = tour.Price;

    //         await _dbContext.SaveChangesAsync();
    //     }
    // }

    public async Task DeleteTour(int id)
    {
        var tour = await _dbContext.Tours.Include(t => t.Waypoints).FirstOrDefaultAsync(t => t.Id == id);

        if (tour != null)
        {
            _dbContext.Tours.Remove(tour);
            // Delete all waypoints images
            foreach (var waypoint in tour.Waypoints)
            {
                if (!string.IsNullOrWhiteSpace(waypoint.ImageUrl))
                {
                    RemoveImage(waypoint.ImageUrl);
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}