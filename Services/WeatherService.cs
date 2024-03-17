using System.Text.Json;

namespace Book.App.Services;

public class WeatherService
{
    private static string key = Environment.GetEnvironmentVariable("OPEN_WEATHER_API_KEY");
    private string url = $"https://api.openweathermap.org/data/3.0/onecall?appid={key}";
    private HttpClient _client = new HttpClient();

    public WeatherService()
    {
        if (key == null)
        {
            throw new Exception("Open Weather API Key not found");
        }

        _client.BaseAddress = new Uri(url);
    }


    // public async Task<WeatherModel> GetWeather(double lat, double lon) {
    //     var response = await _client.GetAsync($"{url}&lat={lat}&lon={lon}");
    //     var content = await response.Content.ReadAsStringAsync();
    //     return JsonSerializer.Deserialize<WeatherModel>(content);

    // }

}