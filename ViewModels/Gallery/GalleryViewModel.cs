using Book.App.Models;

namespace Book.App.ViewModels;

public class GalleryViewModel
{
    public int TourId { get; set; }
    public int AgencyId { get; set; }
    public List<ImageModel> Images { get; set; } = new();
    public string Redirect { get; set; } = string.Empty;
}