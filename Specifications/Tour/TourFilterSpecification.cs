using System.Linq.Expressions;
using Book.App.Models;

namespace Book.App.Specifications;

public class TourFilterSpecification : BaseSpecification<TourModel>
{
    public TourFilterSpecification(FilterModel filterModel, List<Expression<Func<TourModel, bool>>>? criterias) : base()
    {
        Filter(filterModel, criterias);
    }

    public TourFilterSpecification(FilterModel filterModel) : base()
    {
        Filter(filterModel, null);
    }

    public void Filter(FilterModel filterModel, List<Expression<Func<TourModel, bool>>>? criterias)
    {
        // filters
        if (criterias != null)
        {
            foreach (var criteria in criterias)
            {
                ApplyCriteria(criteria);
            }
        }

        if (!string.IsNullOrEmpty(filterModel.SearchString))
        {
            ApplyCriteria(tour => tour.Name.Contains(filterModel.SearchString));
        }

        if (filterModel.MinPrice.HasValue && filterModel.MinPrice > 0)
        {
            ApplyCriteria(tour => tour.Price >= filterModel.MinPrice);
        }

        if (filterModel.MaxPrice.HasValue && filterModel.MaxPrice > 0)
        {
            ApplyCriteria(tour => tour.Price <= filterModel.MaxPrice);
        }

        if (filterModel.Page > 0 && filterModel.PageSize > 0)
        {
            ApplyPaging((filterModel.Page - 1) * filterModel.PageSize, filterModel.PageSize);
        }

        Includes.Add(t => t.Images);
        Includes.Add(t => t.Reservations);
        IncludeStrings.Add("Reservations.User");
        IncludeStrings.Add("TravelAgency.Users");

        // ordering by date or price
        switch (filterModel.OrderBy)
        {
            case Models.OrderBy.Date:
                if (filterModel.OrderDirection == OrderDirection.Asc)
                {
                    ApplyOrderBy(tour => tour.StartDate);
                }
                else
                {
                    ApplyOrderByDescending(tour => tour.StartDate);
                }
                break;
            case Models.OrderBy.Price:
                if (filterModel.OrderDirection == OrderDirection.Asc)
                {
                    ApplyOrderBy(tour => tour.Price);
                }
                else
                {
                    ApplyOrderByDescending(tour => tour.Price);
                }
                break;
        }
    }
}