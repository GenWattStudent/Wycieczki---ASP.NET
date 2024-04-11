using Book.App.Models;

namespace Book.App.ViewModels;

public class WeatherItemViewModel
{
    public DayWeather? Weather { get; set; }
    public int Index { get; set; }
    public string Id { get; set; } = Guid.NewGuid().ToString();
}