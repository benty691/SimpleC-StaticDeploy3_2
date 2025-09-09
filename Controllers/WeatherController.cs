using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet("{city}")]
    public async Task<ActionResult<WeatherData>> GetWeather(string city)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("City name cannot be empty");
            }

            var weatherData = await _weatherService.GetWeatherAsync(city);
            return Ok(weatherData);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while fetching weather for city: {City}", city);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching weather for city: {City}", city);
            return StatusCode(500, "An unexpected error occurred while fetching weather data");
        }
    }

    /// <summary>
    /// Gets weather data for specific coordinates
    /// </summary>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <returns>Weather data for the specified coordinates</returns>
    [HttpGet("coordinates")]
    public async Task<ActionResult<WeatherData>> GetWeatherByCoordinates([FromQuery] double latitude, [FromQuery] double longitude)
    {
        try
        {
            if (latitude < -90 || latitude > 90)
            {
                return BadRequest("Latitude must be between -90 and 90 degrees");
            }

            if (longitude < -180 || longitude > 180)
            {
                return BadRequest("Longitude must be between -180 and 180 degrees");
            }

            var weatherData = await _weatherService.GetWeatherByCoordinatesAsync(latitude, longitude);
            return Ok(weatherData);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while fetching weather for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching weather for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            return StatusCode(500, "An unexpected error occurred while fetching weather data");
        }
    }
}
