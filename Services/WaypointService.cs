using Book.App.Models;
using Book.App.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class WaypointService : IWaypointService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly ITourService _tourService;

    public WaypointService(ApplicationDbContext context, IFileService fileService, ITourService tourService)
    {
        _context = context;
        _fileService = fileService;
        _tourService = tourService;
    }

    public async Task<WaypointModel?> Get(int id)
    {
        return await _context.Waypoints.Include(w => w.Images).Include(w => w.Tour).FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task RemoveImages(WaypointModel waypointModel)
    {
        foreach (var image in waypointModel.Images)
        {
            var imageToDelete = await _context.Images.FindAsync(image.Id);
            if (imageToDelete != null)
            {
                _fileService.DeleteFile(imageToDelete.ImageUrl);
                _context.Images.Remove(imageToDelete);
            }
        }
    }
    public async Task Delete(WaypointModel waypoint)
    {
        _context.Waypoints.Remove(waypoint);
        await RemoveImages(waypoint);
        await _context.SaveChangesAsync();
    }

    public async Task<WaypointModel?> Edit(WaypointModel waypoint)
    {
        var waypointDb = await Get(waypoint.Id);

        if (waypointDb == null)
        {
            return null;
        }

        waypointDb.Edit(waypoint);

        _context.Waypoints.Update(waypointDb);
        await _context.SaveChangesAsync();

        return waypointDb;
    }

    public async Task AddImages(List<IFormFile> formFiles, WaypointModel waypointModel)
    {
        foreach (var formFile in formFiles)
        {
            var imageUrl = await _fileService.SaveFile(formFile, "waypoints");
            var image = new ImageModel
            {
                ImageUrl = imageUrl,
            };
            waypointModel.Images.Add(image);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Add(List<AddTourWaypointsViewModel> addTourWaypointsModel, int tourId)
    {
        var tour = await _tourService.GetById(tourId);

        if (tour != null)
        {
            foreach (var waypointModel in addTourWaypointsModel)
            {
                var waypoint = tour.Waypoints.FirstOrDefault(w => w.Id == waypointModel.Id && w.Id != 0);

                if (waypoint != null)
                {
                    waypoint.Edit(waypointModel);
                    _context.Waypoints.Update(waypoint);
                }
                else
                {
                    waypointModel.Name = "Waypoint " + (tour.Waypoints.Count + 1);
                    var newWaypoint = new WaypointModel(waypointModel);
                    Console.WriteLine(newWaypoint.Name);
                    tour.Waypoints.Add(newWaypoint);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}