namespace Book.App.Models;

public enum OrderDirection
{
    Asc,
    Desc
}

public enum OrderBy
{
    Date,
    Price
}

public class FilterModel
{
    public string? SearchString { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public OrderBy OrderBy { get; set; } = OrderBy.Date;
    public OrderDirection OrderDirection { get; set; } = OrderDirection.Asc;
}
