using Book.App.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Book.App.Services;

public class WeatherService : IWeatherService
{
    private static string key = Environment.GetEnvironmentVariable("OPEN_WEATHER_API_KEY");
    private string url = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/";
    private HttpClient _client = new HttpClient();
    private readonly IDistributedCache _cache;

    public WeatherService(IDistributedCache cache)
    {
        _cache = cache;
        if (key == null)
        {
            throw new Exception("Open Weather API Key not found");
        }

        _client.BaseAddress = new Uri(url);
    }

    public async Task<WeatherModel> GetWeatherFromApi(double lat, double lon)
    {
        var response = await _client.GetAsync($"{url}{lat},{lon}?key={key}&include=days&unitGroup=metric");
        var content = await response.Content.ReadAsStringAsync();
        var weatherData = JsonConvert.DeserializeObject<WeatherModel>(content);
        return weatherData;
    }


    public async Task<WeatherModel?> GetWeather(double lat, double lon)
    {
        if (string.IsNullOrEmpty(_cache.GetString($"{lat},{lon}")))
        {
            var weatherData = await GetWeatherFromApi(lat, lon);
            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            Console.WriteLine("Setting cache");
            _cache.SetString($"{lat},{lon}", JsonConvert.SerializeObject(weatherData), cacheOptions);
            return weatherData;
        }

        Console.WriteLine("Getting from cache");
        var cachedData = _cache.GetString($"{lat},{lon}");
        return JsonConvert.DeserializeObject<WeatherModel>(cachedData ?? "");

        // var weatherData = new WeatherModel
        // {
        //     QueryCost = 1,
        //     Latitude = 51.5074,
        //     Longitude = -0.1278,
        //     ResolvedAddress = "London, UK",
        //     Address = "London, UK",
        //     Timezone = "Europe/London",
        //     Tzoffset = 0.0,
        //     Days = new List<DayWeather>
        // {
        //     new DayWeather
        //     {
        //         Datetime = "2022-01-01",
        //         DatetimeEpoch = 1640995200,
        //         Tempmax = 8.0,
        //         Tempmin = 2.0,
        //         Temp = 5.0,
        //         FeelsLikeMax = 6.0,
        //         FeelsLikeMin = 0.0,
        //         FeelsLike = 3.0,
        //         Dew = 2.0,
        //         Humidity = 80.0,
        //         Precip = 0.5,
        //         Precipprob = 50.0,
        //         Precipcover = 25.0,
        //         Preciptype = new List<string> { "rain" },
        //         Pressure = 1000.0,
        //         Snow = 0.0,
        //         Snowdepth = 0.0,
        //         Windgust = 20.0,
        //         Sunrise = "07:00",
        //         Sunset = "17:00",
        //         Conditions = "Rain",
        //         Icon = "rain",
        //         Windspeed = 10.0,
        //         Winddir = 180.0,
        //         Cloudcover = 50.0,
        //         Visibility = 10.0,
        //         Description = "Light rain",
        //     },
        //     new DayWeather
        //     {
        //         Datetime = "2022-01-02",
        //         DatetimeEpoch = 1641081600,
        //         Tempmax = 10.0,
        //         Tempmin = 4.0,
        //         Temp = 7.0,
        //         FeelsLikeMax = 8.0,
        //         FeelsLikeMin = 2.0,
        //         FeelsLike = 5.0,
        //         Dew = 3.0,
        //         Humidity = 75.0,
        //         Precip = 0.0,
        //         Precipprob = 0.0,
        //         Precipcover = 0.0,
        //         Preciptype = new List<string> { },
        //         Pressure = 1005.0,
        //         Snow = 0.0,
        //         Snowdepth = 0.0,
        //         Windgust = 15.0,
        //         Sunrise = "07:00",
        //         Sunset = "17:00",
        //         Conditions = "Clear",
        //         Icon = "clear",
        //         Windspeed = 5.0,
        //         Winddir = 180.0,
        //         Cloudcover = 0.0,
        //         Visibility = 10.0,
        //         Description = "Clear",

        // }
        // }
        // };

        // return weatherData;
    }

}