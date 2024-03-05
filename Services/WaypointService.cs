using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class WaypointService
{
    private readonly ApplicationDbContext _context;
    private readonly FileService _fileService;

    public WaypointService(ApplicationDbContext context, FileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<WaypointModel?> GetWaypoint(int id)
    {
        return await _context.Waypoints.FindAsync(id);
    }

    public async Task RemoveWaypointImages(WaypointModel waypointModel)
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

    public async Task AddImagesToWaypoint(List<IFormFile> formFiles, WaypointModel waypointModel)
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
    }

    public async Task Delete(int id)
    {
        var waypoint = await _context.Waypoints.Include(w => w.Images).FirstOrDefaultAsync(w => w.Id == id);
        if (waypoint != null)
        {
            _context.Waypoints.Remove(waypoint);
            await RemoveWaypointImages(waypoint);
            _context.SaveChanges();
            await _context.SaveChangesAsync();
        }
    }

    public async Task Edit(AddTourWaypointsModel editTourWaypointsModel)
    {
        var waypointToEdit = await _context.Waypoints.FindAsync(editTourWaypointsModel.Id);

        if (waypointToEdit != null)
        {
            waypointToEdit.Name = editTourWaypointsModel.Name;
            waypointToEdit.Description = editTourWaypointsModel.Description;
            waypointToEdit.Latitude = editTourWaypointsModel.Lat;
            waypointToEdit.Longitude = editTourWaypointsModel.Lng;
            waypointToEdit.IsRoad = editTourWaypointsModel.IsRoad;

            await AddImagesToWaypoint(editTourWaypointsModel.Images, waypointToEdit);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddImages(List<IFormFile> formFiles, WaypointModel waypointModel)
    {
        await AddImagesToWaypoint(formFiles, waypointModel);
        await _context.SaveChangesAsync();
    }
}