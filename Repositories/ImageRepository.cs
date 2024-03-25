using Book.App.Models;

namespace Book.App.Repositories;

public class ImageRepository : Repository<ImageModel>, IImageRepository
{
    public ImageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}