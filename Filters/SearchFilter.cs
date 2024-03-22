using Book.App.Models;

namespace Book.App.Filters;

public class SearchFilter : IFilter
{
    public string? SearchString { get; set; }

    public SearchFilter(string? searchString)
    {
        SearchString = searchString;
    }

    public IQueryable<TourModel> Process(IQueryable<TourModel> tours)
    {
        if (string.IsNullOrEmpty(SearchString))
        {
            return tours;
        }
        return tours.Where(t => t.Name.Contains(SearchString) || t.Description.Contains(SearchString));
    }
}