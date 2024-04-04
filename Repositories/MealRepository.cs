using Book.App.Models;

namespace Book.App.Repositories;

public class MealRepository : Repository<MealModel>, IMealRepository
{
    public MealRepository(ApplicationDbContext context) : base(context)
    {
    }
}