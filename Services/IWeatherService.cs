using WeatherApp.Models;

namespace WeatherApp.Services;

public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(string city);
    Task<WeatherData> GetWeatherByCoordinatesAsync(double latitude, double longitude);
}
