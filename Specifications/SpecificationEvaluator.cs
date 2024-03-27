using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Specifications;

public class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        // modify the IQueryable using the specification's criteria expression
        if (spec.Criteria.Count > 0)
        {
            foreach (var criteria in spec.Criteria)
            {
                query = query.Where(criteria);
            }
        }

        // Includes all expression-based includes
        query = spec.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // Include any string-based include statements
        query = spec.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        // Apply ordering if expressions are set
        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        if (spec.GroupBy != null)
        {
            query = query.GroupBy(spec.GroupBy).SelectMany(x => x);
        }

        // Apply paging if enabled
        if (spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip)
                .Take(spec.Take);
        }

        // return the result of the query using the specification's criteria expression
        return query.AsSplitQuery();
    }
}