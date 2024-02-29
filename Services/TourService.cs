using Book.App.ViewModels;

namespace Book.App.Services;

public class TourService
{
    public readonly List<TourViewModel> _tours = new()
    {
        new TourViewModel { Id = 1, Price = 53000, Name = "Mystical Machu Picchu Expedition", Description = "Embark on a journey to the heart of the Andes Mountains and uncover the ancient mysteries of Machu Picchu. Trek through lush forests, traverse rugged terrains, and marvel at the awe-inspiring ruins of this UNESCO World Heritage Site. Led by expert guides, this expedition promises an immersive cultural experience as you learn about the fascinating history and architecture of the Inca civilization. Get ready to be captivated by the breathtaking beauty and enigmatic allure of Machu Picchu.", ImageUrl = "https://via.placeholder.com/150" },
        new TourViewModel { Id = 2, Price = 100, Name = "Safari Adventure: Explore the Serengeti", Description = "Embark on a thrilling safari adventure through the iconic landscapes of the Serengeti. Witness the majestic wildlife in their natural habitat as you traverse vast savannahs and acacia-dotted plains. From awe-inspiring lion prides to graceful giraffes and elusive leopards, every moment promises an unforgettable encounter with Africa's Big Five and beyond. With expert guides and luxurious accommodations, this safari experience offers the perfect blend of excitement and comfort in the heart of the wild.", ImageUrl = "https://via.placeholder.com/150" },
        new TourViewModel { Id = 3, Price = 20000, Name = "Cultural Odyssey: Discover Ancient Rome", Description = "Step back in time and immerse yourself in the grandeur of Ancient Rome on this captivating cultural odyssey. Explore iconic landmarks such as the Colosseum, Roman Forum, and Pantheon, as you unravel the rich tapestry of Rome's history and heritage. Wander through cobblestone streets lined with centuries-old architecture, and indulge in delectable Italian cuisine amidst the charming ambiance of local trattorias. With knowledgeable guides leading the way, this journey promises a captivating blend of ancient wonders and modern delights.", ImageUrl = "https://via.placeholder.com/150" }
    };

    public List<TourViewModel> GetTours()
    {
        return _tours;
    }

    public void AddTour(TourViewModel tour)
    {
        tour.Id = _tours.Max(t => t.Id) + 1;
        _tours.Add(tour);
    }


    public TourViewModel? GetTour(int id)
    {
        return _tours.FirstOrDefault(t => t.Id == id);
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

    public void UpdateTour(TourViewModel tour)
    {
        var existingTour = _tours.FirstOrDefault(t => t.Id == tour.Id);
        if (existingTour != null)
        {
            existingTour.Name = tour.Name;
            existingTour.Description = tour.Description;
            existingTour.ImageUrl = tour.ImageUrl;
            existingTour.Price = tour.Price;
        }
    }

    public void DeleteTour(int id)
    {
        var existingTour = _tours.FirstOrDefault(t => t.Id == id);
        if (existingTour != null)
        {
            _tours.Remove(existingTour);
        }
    }
}