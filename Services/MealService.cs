using Book.App.Models;
using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;

namespace Book.App.Services;

public class MealService : IMealService
{
    private readonly IUnitOfWork _unitOfWork;

    public MealService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MealModel> Add(MealModel mealModel)
    {
        Console.WriteLine($"Adding meal {mealModel.Id} {mealModel.TourId}");
        _unitOfWork.mealRepository.Add(mealModel);
        await _unitOfWork.SaveAsync();
        return mealModel;
    }

    public async Task<List<MealModel>> GetAllByTourId(int tourId)
    {
        var mealSpecification = new MealSpecification();
        mealSpecification.GetByTourId(tourId);
        return await _unitOfWork.mealRepository.GetBySpec(mealSpecification);
    }

    public async Task<MealModel> Delete(int id)
    {
        var meal = await _unitOfWork.mealRepository.GetById(id);

        if (meal == null)
        {
            throw new Exception("Meal not found");
        }

        _unitOfWork.mealRepository.Remove(meal);
        await _unitOfWork.SaveAsync();
        return meal;
    }

    public async Task Update(MealModel mealModel)
    {
        Console.WriteLine($"Updating meal {mealModel.Id} {mealModel.TourId}");

        //Consloe write all the properties of mealModel
        foreach (var prop in mealModel.GetType().GetProperties())
        {
            Console.WriteLine($"{prop.Name} = {prop.GetValue(mealModel, null)}");
        }

        _unitOfWork.mealRepository.Update(mealModel);
        await _unitOfWork.SaveAsync();
    }
}