namespace Book.App.Models;

public class ImageSliderViewModel
{
    public List<ImageModel> Images { get; set; } = new();
    public int SliderId { get; set; }
}