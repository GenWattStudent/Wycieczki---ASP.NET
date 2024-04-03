using Book.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class WeatherController : Controller
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IWeatherService _weatherService;

    public WeatherController(ILogger<WeatherController> logger, WeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    public async Task<IActionResult> Index(int lat, int lon)
    {
        var weather = await _weatherService.GetWeather(lat, lon);
        return View(weather);
    }
}