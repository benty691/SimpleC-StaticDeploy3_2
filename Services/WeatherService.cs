using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    private const string BaseUrl = "https://wttr.in";

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        try
        {
            // wttr.in returns JSON format with ?format=j1 parameter
            var url = $"{BaseUrl}/{Uri.EscapeDataString(city)}?format=j1";
            
            _logger.LogInformation("Fetching weather data for city: {City}", city);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<WttrApiResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (weatherResponse == null || weatherResponse.Current_Condition == null || !weatherResponse.Current_Condition.Any())
            {
                throw new InvalidOperationException("Failed to deserialize weather response");
            }

            var currentCondition = weatherResponse.Current_Condition.First();
            var nearestArea = weatherResponse.Nearest_Area?.FirstOrDefault();

            return new WeatherData
            {
                Location = nearestArea?.AreaName?.FirstOrDefault()?.Value ?? city,
                Temperature = TryParseDouble(currentCondition.Temp_C),
                Description = currentCondition.WeatherDesc?.FirstOrDefault()?.Value ?? "Unknown",
                Humidity = TryParseDouble(currentCondition.Humidity),
                WindSpeed = TryParseDouble(currentCondition.WindspeedKmph),
                Timestamp = DateTime.UtcNow
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching weather data for city: {City}", city);
            throw new InvalidOperationException($"Failed to fetch weather data for {city}. Please check the city name and try again.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while processing weather data for city: {City}", city);
            throw new InvalidOperationException("Failed to process weather data response.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching weather data for city: {City}", city);
            throw;
        }
    }

    public async Task<WeatherData> GetWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        try
        {
            // wttr.in supports coordinates in format: lat,lon
            var url = $"{BaseUrl}/{latitude},{longitude}?format=j1";
            
            _logger.LogInformation("Fetching weather data for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<WttrApiResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (weatherResponse == null || weatherResponse.Current_Condition == null || !weatherResponse.Current_Condition.Any())
            {
                throw new InvalidOperationException("Failed to deserialize weather response");
            }

            var currentCondition = weatherResponse.Current_Condition.First();
            var nearestArea = weatherResponse.Nearest_Area?.FirstOrDefault();

            return new WeatherData
            {
                Location = nearestArea?.AreaName?.FirstOrDefault()?.Value ?? $"{latitude}, {longitude}",
                Temperature = TryParseDouble(currentCondition.Temp_C),
                Description = currentCondition.WeatherDesc?.FirstOrDefault()?.Value ?? "Unknown",
                Humidity = TryParseDouble(currentCondition.Humidity),
                WindSpeed = TryParseDouble(currentCondition.WindspeedKmph),
                Timestamp = DateTime.UtcNow
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching weather data for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            throw new InvalidOperationException($"Failed to fetch weather data for coordinates {latitude}, {longitude}. Please check the coordinates and try again.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while processing weather data for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            throw new InvalidOperationException("Failed to process weather data response.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching weather data for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            throw;
        }
    }

    private static double TryParseDouble(string value)
    {
        if (double.TryParse(value, out double result))
        {
            return result;
        }
        return 0.0;
    }
}
