using Book.App.Models;

namespace Book.App.Filters;

public class PriceFilter : IFilter
{
    private readonly decimal? _minPrice;
    private readonly decimal? _maxPrice;

    public PriceFilter(decimal? minPrice, decimal? maxPrice)
    {
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }


    public IQueryable<TourModel> Process(IQueryable<TourModel> tours)
    {
        if (_minPrice.HasValue && _minPrice.Value > 0)
        {
            tours = tours.Where(t => t.Price >= _minPrice);
        }

        if (_maxPrice.HasValue && _maxPrice.Value > 0)
        {
            tours = tours.Where(t => t.Price <= _maxPrice);
        }

        return tours;
    }

}