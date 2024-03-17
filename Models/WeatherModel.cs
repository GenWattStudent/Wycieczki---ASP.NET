namespace Book.App.Models;

public class WeatherModel
{
    public int QueryCost { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ResolvedAddress { get; set; }
    public string Address { get; set; }
    public string Timezone { get; set; }
    public double Tzoffset { get; set; }
    public List<DayWeather> Days { get; set; }
}

public class DayWeather
{
    public string Datetime { get; set; }
    public long DatetimeEpoch { get; set; }
    public double Tempmax { get; set; }
    public double Tempmin { get; set; }
    public double Temp { get; set; }
    public double FeelsLikeMax { get; set; }
    public double FeelsLikeMin { get; set; }
    public double FeelsLike { get; set; }
    public double Dew { get; set; }
    public double Humidity { get; set; }
    public double Precip { get; set; }
    public double Precipprob { get; set; }
    public double Precipcover { get; set; }
    public List<string> Preciptype { get; set; }
    public double Pressure { get; set; }
    public double Snow { get; set; }
    public double Snowdepth { get; set; }
    public double Windgust { get; set; }
    public double Windspeed { get; set; }
    public double Winddir { get; set; }
    public double Cloudcover { get; set; }
    public double Visibility { get; set; }
    public double Solarradiation { get; set; }
    public double Solarenergy { get; set; }
    public double Uvindex { get; set; }
    public string Sunrise { get; set; }
    public string Sunset { get; set; }
    public string Conditions { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}
