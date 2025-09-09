using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class WeatherData
{
    public string Location { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Timestamp { get; set; }
}

// wttr.in API response models
public class WttrApiResponse
{
    public CurrentCondition[] Current_Condition { get; set; } = Array.Empty<CurrentCondition>();
    public NearestArea[] Nearest_Area { get; set; } = Array.Empty<NearestArea>();
}

public class CurrentCondition
{
    [JsonPropertyName("temp_C")]
    public string Temp_C { get; set; } = string.Empty;
    
    [JsonPropertyName("humidity")]
    public string Humidity { get; set; } = string.Empty;
    
    [JsonPropertyName("windspeedKmph")]
    public string WindspeedKmph { get; set; } = string.Empty;
    
    [JsonPropertyName("weatherDesc")]
    public WeatherDesc[] WeatherDesc { get; set; } = Array.Empty<WeatherDesc>();
}

public class WeatherDesc
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public class NearestArea
{
    [JsonPropertyName("areaName")]
    public AreaName[] AreaName { get; set; } = Array.Empty<AreaName>();
}

public class AreaName
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
