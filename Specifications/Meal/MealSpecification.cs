using Book.App.Models;

namespace Book.App.Specifications;

public class MealSpecification : BaseSpecification<MealModel>
{
    public void GetByTourId(int tourid)
    {
        ApplyCriteria(x => x.TourId == tourid);
    }
}