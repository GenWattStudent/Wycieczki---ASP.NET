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
                await _fileService.DeleteFile(imageToDelete.ImageUrl);
                _context.Images.Remove(imageToDelete);
            }
        }
    }
    public async Task Delete(int id)
    {
        var waypoint = await _context.Waypoints.Include(w => w.Images).FirstOrDefaultAsync(w => w.Id == id);
        if (waypoint != null)
        {
            _context.Waypoints.Remove(waypoint);
            await RemoveImages(waypoint);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Edit(AddTourWaypointsViewModel editTourWaypointsModel)
    {
        var waypointToEdit = await _context.Waypoints.FindAsync(editTourWaypointsModel.Id);

        if (waypointToEdit != null)
        {
            waypointToEdit.Name = editTourWaypointsModel.Name;
            waypointToEdit.Description = editTourWaypointsModel.Description;
            waypointToEdit.Latitude = editTourWaypointsModel.Lat;
            waypointToEdit.Longitude = editTourWaypointsModel.Lng;
            waypointToEdit.IsRoad = editTourWaypointsModel.IsRoad;

            await AddImages(editTourWaypointsModel.Images, waypointToEdit);
            await _context.SaveChangesAsync();
        }
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
        Console.WriteLine("AddWaypoints: " + tourId);
        if (tour != null)
        {
            Console.WriteLine("AddWaypoints: " + addTourWaypointsModel.Count);
            foreach (var waypointModel in addTourWaypointsModel)
            {
                var waypoint = new WaypointModel(waypointModel);
                tour.Waypoints.Add(waypoint);
            }

            await _context.SaveChangesAsync();
        }
    }
}