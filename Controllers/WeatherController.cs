using Book.App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Book.App.Controllers;

[EnableRateLimiting("fixed")]
public class WeatherController : Controller
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IWeatherService _weatherService;

    public WeatherController(ILogger<WeatherController> logger, IWeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    public async Task<IActionResult> Index(int lat, int lon)
    {
        var weather = await _weatherService.Get(lat, lon);
        return View(weather);
    }

    public async Task<IActionResult> GetOneDayWeather(int lat, int lon)
    {
        var weather = await _weatherService.Get(lat, lon);

        if (weather == null)
        {
            TempData["Error"] = "Weather data not found";
            return NotFound();
        }

        return Json(weather.Days[0]);
    }
}