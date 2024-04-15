using Book.App.Models;

namespace Book.App.Services;

public interface IMealService
{
    Task<MealModel> Add(MealModel mealModel);
    Task<List<MealModel>> GetAllByTourId(int tourId);
    Task<MealModel> Delete(int id);
    Task Update(MealModel mealModel);
}