namespace Book.App.Models;

public class ImageSliderModel
{
    public List<ImageModel> Images { get; set; } = new();
    public int SliderId { get; set; }
}