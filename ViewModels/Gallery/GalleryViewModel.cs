using Book.App.Models;

namespace Book.App.ViewModels;

public class GalleryViewModel
{
    public int TourId { get; set; }
    public List<ImageModel> Images { get; set; } = new();
}