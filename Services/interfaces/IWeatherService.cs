using Book.App.Models;

namespace Book.App.Services;

public interface IWeatherService
{
    Task<WeatherModel> GetWeatherFromApi(double lat, double lon);
    Task<WeatherModel?> GetWeather(double lat, double lon);
}


