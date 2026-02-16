using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using WeatherMcpServer;
using ModelContextProtocol;

var builder = WebApplication.CreateBuilder(args);

// Configure logging to stderr
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add MCP server with HTTP transport and tools from assembly
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapMcp();

await app.RunAsync();

/// <summary>
/// Weather tools for querying weather information
/// </summary>
[McpServerToolType]
public static class WeatherTools
{
    // /// <summary>
    // /// Gets weather information for a specified city at a given date and time
    // /// Returns raw JSON string
    // /// </summary>
    // /// <param name="httpClient">HTTP client for making API calls</param>
    // /// <param name="countryName">The name of the country</param>
    // /// <param name="cityName">The name of the city</param>
    // /// <param name="dateTimeInUtc">The date and time in UTC (ISO 8601 format, e.g., 2024-01-13T12:00:00Z)</param>
    // /// <param name="cancellationToken">Cancellation token</param>
    // /// <returns>Weather information as JSON string</returns>
    // [McpServerTool(Name = "GetWeatherForCity")]
    // [Description("Gets weather information for a specified city at a given date and time. Uses Open-Meteo public weather API.")]
    // // [Description(@"
    // // Gets weather information for a specified city at a given date and time. 
    
    // // Uses Open-Meteo public weather API.
    
    // // When parameters are not provided, defaults to Auckland, New Zealand, and tomorrow's date.
    
    // // Temperature information can be mapped to the following:
    // // - temperature1 = temperature_2m: Temperature at 2 meters above ground (°C)
    // // - temperature2 = temperature_80m: Temperature at 80 meters above ground (°C)
    // // - temperature3 = temperature_soil6cm: Soil temperature at 6 cm depth (°C)
    // // - temperature4 = temperature_soil0cm: Soil temperature at 0 cm depth (°C)
    // // - temperature5 = apparent_temperature: Apparent temperature (°C)")]
    // public static async Task<string> GetWeatherForCityAsSerializedJsonString(
    //     HttpClient httpClient,
    //     [Description("The name of the city. When not provided, use Auckland as default value.")] string? cityName = "",
    //     [Description("The name of the country. When not provided, use New Zealand as default value")] string? countryName = "",
    //     [Description("The date and time in UTC (ISO 8601 format, e.g., 2024-01-13T12:00:00Z), use tomorrow date if not provided")] string? dateTimeInUtc = "",
    //     CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         if (string.IsNullOrWhiteSpace(countryName) || string.IsNullOrWhiteSpace(cityName) || string.IsNullOrWhiteSpace(dateTimeInUtc))
    //         {
    //             return JsonSerializer.Serialize(new { error = "Missing required parameters: countryName, cityName, or dateTimeInUtc" });
    //         }

    //         // Validate and parse the date
    //         if (!DateTime.TryParse(dateTimeInUtc, out var parsedDate))
    //         {
    //             return JsonSerializer.Serialize(new { error = $"Invalid dateTimeInUtc format: {dateTimeInUtc}. Please use ISO 8601 format (e.g., 2024-01-13T12:00:00Z)" });
    //         }

    //         // Get coordinates for the city using geocoding API
    //         var coordinates = await GetCityCoordinates(httpClient, cityName, countryName, cancellationToken);
    //         if (coordinates == null)
    //         {
    //             return JsonSerializer.Serialize(new { error = $"Could not find coordinates for {cityName}, {countryName}" });
    //         }

    //         // Fetch weather data from Open-Meteo API
    //         var weatherData = await GetWeatherData(httpClient, coordinates.Value.latitude, coordinates.Value.longitude, parsedDate, cancellationToken);
            
    //         if (weatherData == null)
    //         {
    //             return JsonSerializer.Serialize(new { error = "Failed to fetch weather data from API" });
    //         }

    //         var result = new
    //         {
    //             city = cityName,
    //             country = countryName,
    //             dateTime = dateTimeInUtc,
    //             coordinates = new { latitude = coordinates.Value.latitude, longitude = coordinates.Value.longitude },
    //             weather = weatherData
    //         };
            
    //         return JsonSerializer.Serialize(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return JsonSerializer.Serialize(new { error = $"Error processing request: {ex.Message}" });
    //     }
    // }

    /// <summary>
    /// Gets weather information for a specified city at a given date and time
    /// Returns structured WeatherInfo object
    /// </summary>
    /// <param name="httpClient">HTTP client for making API calls</param>
    /// <param name="countryName">The name of the country</param>
    /// <param name="cityName">The name of the city</param>
    /// <param name="dateTimeInUtc">The date and time in UTC (ISO 8601 format, e.g., 2024-01-13T12:00:00Z)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weather information as JSON string</returns>
    // [McpServerTool(Name = "GetWeatherForCity", UseStructuredContent = true)]
    // [Description("Gets weather information for a specified city at a given date and time. Uses Open-Meteo public weather API.")]
    // public static async Task<WeatherInfo> GetWeatherForCityAsStructuredContent(
    //     HttpClient httpClient,
    //     [Description("The name of the city. When not provided, use Auckland as default value.")] string? cityName = "",
    //     [Description("The name of the country. When not provided, use New Zealand as default value")] string? countryName = "",
    //     [Description("The date and time in UTC (ISO 8601 format, e.g., 2024-01-13T12:00:00Z), use tomorrow date if not provided")] string? dateTimeInUtc = "",
    //     CancellationToken cancellationToken = default)
    //     {
    //         // Use the new WeatherInfo, CityInfo, and WeatherData classes
    //         if (string.IsNullOrWhiteSpace(countryName) || string.IsNullOrWhiteSpace(cityName) || string.IsNullOrWhiteSpace(dateTimeInUtc))
    //         {
    //             throw new McpException("Missing required parameters: countryName, cityName, or dateTimeInUtc");
    //         }

    //         if (!DateTime.TryParse(dateTimeInUtc, out var parsedDate))
    //         {
    //             throw new McpException($"Invalid dateTimeInUtc format: {dateTimeInUtc}. Please use ISO 8601 format (e.g., 2024-01-13T12:00:00Z)");
    //         }

    //         var coordinates = await GetCityCoordinates(httpClient, cityName, countryName, cancellationToken);
    //         if (coordinates == null)
    //         {
    //             throw new McpException($"Could not find coordinates for {cityName}, {countryName}");
    //         }

    //         var weatherDataObj = await GetWeatherData(httpClient, coordinates.Value.latitude, coordinates.Value.longitude, parsedDate, cancellationToken);
    //         if (weatherDataObj == null)
    //         {
    //             throw new McpException("Failed to fetch weather data from API");
    //         }

    //         // Map to WeatherData class
    //         var weatherData = new WeatherMcpServer.WeatherData
    //         {
    //             Time = (string)weatherDataObj.GetType().GetProperty("time")?.GetValue(weatherDataObj) ?? string.Empty,
    //             Temperature1 = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature1")?.GetValue(weatherDataObj)),
    //             Temperature2 = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature2")?.GetValue(weatherDataObj)),
    //             Temperature3 = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature3")?.GetValue(weatherDataObj)),
    //             Temperature4 = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature4")?.GetValue(weatherDataObj)),
    //             Temperature5 = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature5")?.GetValue(weatherDataObj)),
    //             RelativeHumidityPercent = Convert.ToInt32(weatherDataObj.GetType().GetProperty("relative_humidity_percent")?.GetValue(weatherDataObj)),
    //             PrecipitationMm = Convert.ToDouble(weatherDataObj.GetType().GetProperty("precipitation_mm")?.GetValue(weatherDataObj)),
    //             WeatherCode = Convert.ToInt32(weatherDataObj.GetType().GetProperty("weather_code")?.GetValue(weatherDataObj)),
    //             WeatherDescription = (string)weatherDataObj.GetType().GetProperty("weather_description")?.GetValue(weatherDataObj) ?? string.Empty,
    //             WindSpeedKmh = Convert.ToDouble(weatherDataObj.GetType().GetProperty("wind_speed_kmh")?.GetValue(weatherDataObj))
    //         };

    //         var cityInfo = new WeatherMcpServer.CityInfo
    //         {
    //             City = cityName,
    //             Country = countryName,
    //             Coordinates = new WeatherMcpServer.Coordinates
    //             {
    //                 Latitude = coordinates.Value.latitude,
    //                 Longitude = coordinates.Value.longitude
    //             }
    //         };

    //         var weatherInfo = new WeatherMcpServer.WeatherInfo
    //         {
    //             CityInfo = cityInfo,
    //             WeatherData = weatherData,
    //             DateTime = dateTimeInUtc
    //         };

    //         return weatherInfo;
    //     }

    /// <summary>
    /// Gets weather information for a specified city at a given date and time
    /// Returns WeatherInfo object as natural language string
    /// </summary>
    /// <param name="httpClient">HTTP client for making API calls</param>
    /// <param name="countryName">The name of the country</param>
    /// <param name="cityName">The name of the city</param>
    /// <param name="dateTimeInUtc">The date and time in UTC (ISO 8601 format, e.g., 2024-01-13T12:00:00Z)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weather information as formatted human readable string</returns>
    [McpServerTool(Name = "GetWeatherForCity")]
    [Description("Gets weather information for a specified city at a given date and time. Uses Open-Meteo public weather API.")]
    public static async Task<string> GetWeatherForCityAsNaturalLanguage(
        HttpClient httpClient,
        [Description("The name of the city. When not provided, use Auckland as default value.")] string? cityName = "",
        [Description("The name of the country. When not provided, use New Zealand as default value")] string? countryName = "",
        [Description("The date and time in UTC (ISO 8601 format, e.g., 2024-01-13T12:00:00Z), use tomorrow date if not provided")] string? dateTimeInUtc = "",
        CancellationToken cancellationToken = default)
        {
            // Use the new WeatherInfo, CityInfo, and WeatherData classes
            if (string.IsNullOrWhiteSpace(countryName) || string.IsNullOrWhiteSpace(cityName) || string.IsNullOrWhiteSpace(dateTimeInUtc))
            {
                throw new McpException("Missing required parameters: countryName, cityName, or dateTimeInUtc");
            }

            if (!DateTime.TryParse(dateTimeInUtc, out var parsedDate))
            {
                throw new McpException($"Invalid dateTimeInUtc format: {dateTimeInUtc}. Please use ISO 8601 format (e.g., 2024-01-13T12:00:00Z)");
            }

            var coordinates = await GetCityCoordinates(httpClient, cityName, countryName, cancellationToken);
            if (coordinates == null)
            {
                throw new McpException($"Could not find coordinates for {cityName}, {countryName}");
            }

            var weatherDataObj = await GetWeatherData(httpClient, coordinates.Value.latitude, coordinates.Value.longitude, parsedDate, cancellationToken);
            if (weatherDataObj == null)
            {
                throw new McpException("Failed to fetch weather data from API");
            }

            // Map to WeatherData class
            var weatherData = new WeatherMcpServer.WeatherData
            {
                Time = (string)weatherDataObj.GetType().GetProperty("time")?.GetValue(weatherDataObj) ?? string.Empty,
                TemperatureAt2m    = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature_2m")?.GetValue(weatherDataObj)),
                TemperatureAt80m = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature_80m")?.GetValue(weatherDataObj)),
                SoilTemperatureAt0cm = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature_soil0cm")?.GetValue(weatherDataObj)),
                SoilTemperatureAt6cm = Convert.ToDouble(weatherDataObj.GetType().GetProperty("temperature_soil6cm")?.GetValue(weatherDataObj)),
                ApparentTemperature = Convert.ToDouble(weatherDataObj.GetType().GetProperty("apparent_temperature")?.GetValue(weatherDataObj)),
                RelativeHumidityPercent = Convert.ToInt32(weatherDataObj.GetType().GetProperty("relative_humidity_percent")?.GetValue(weatherDataObj)),
                PrecipitationMm = Convert.ToDouble(weatherDataObj.GetType().GetProperty("precipitation_mm")?.GetValue(weatherDataObj)),
                WeatherCode = Convert.ToInt32(weatherDataObj.GetType().GetProperty("weather_code")?.GetValue(weatherDataObj)),
                WeatherDescription = (string)weatherDataObj.GetType().GetProperty("weather_description")?.GetValue(weatherDataObj) ?? string.Empty,
                WindSpeedKmh = Convert.ToDouble(weatherDataObj.GetType().GetProperty("wind_speed_kmh")?.GetValue(weatherDataObj))
            };

            var cityInfo = new WeatherMcpServer.CityInfo
            {
                City = cityName,
                Country = countryName,
                Coordinates = new WeatherMcpServer.Coordinates
                {
                    Latitude = coordinates.Value.latitude,
                    Longitude = coordinates.Value.longitude
                }
            };

            var weatherInfo = new WeatherMcpServer.WeatherInfo
            {
                CityInfo = cityInfo,
                WeatherData = weatherData,
                DateTime = dateTimeInUtc
            };

            return weatherInfo.ToString();
        }


    /// <summary>
    /// Helper method to get city coordinates using Open-Meteo's geocoding API
    /// </summary>
    private static async Task<(double latitude, double longitude)?> GetCityCoordinates(
        HttpClient httpClient, 
        string cityName, 
        string countryName, 
        CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(cityName)}&count=5&language=en&format=json";
            var response = await httpClient.GetStringAsync(url, cancellationToken);
            var json = JsonDocument.Parse(response);

            if (json.RootElement.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
            {
                // Find the best match for the country
                foreach (var result in results.EnumerateArray())
                {
                    var resultCountry = result.TryGetProperty("country", out var countryProp) ? countryProp.GetString() : "";
                    
                    // Match by country name (case-insensitive)
                    if (!string.IsNullOrEmpty(resultCountry) && 
                        resultCountry.Equals(countryName, StringComparison.OrdinalIgnoreCase))
                    {
                        var lat = result.GetProperty("latitude").GetDouble();
                        var lon = result.GetProperty("longitude").GetDouble();
                        return (lat, lon);
                    }
                }

                // If no exact country match, return the first result
                var firstResult = results[0];
                var latitude = firstResult.GetProperty("latitude").GetDouble();
                var longitude = firstResult.GetProperty("longitude").GetDouble();
                return (latitude, longitude);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Helper method to get weather data from Open-Meteo API
    /// </summary>
    private static async Task<object?> GetWeatherData(
        HttpClient httpClient, 
        double latitude, 
        double longitude, 
        DateTime dateTime, 
        CancellationToken cancellationToken)
    {
        try
        {
            var dateStr = dateTime.ToString("yyyy-MM-dd");
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,temperature_80m,soil_temperature_0cm,soil_temperature_6cm,apparent_temperature,relative_humidity_2m,precipitation,weather_code,wind_speed_10m&timezone=UTC&start_date={dateStr}&end_date={dateStr}";
            
            var response = await httpClient.GetStringAsync(url, cancellationToken);
            var json = JsonDocument.Parse(response);

            if (json.RootElement.TryGetProperty("hourly", out var hourly))
            {
                var times = hourly.GetProperty("time").EnumerateArray().ToList();
                var temperatures2m = hourly.GetProperty("temperature_2m").EnumerateArray().ToList();
                var temperatures80m = hourly.GetProperty("temperature_80m").EnumerateArray().ToList();
                var soilTemp0cm = hourly.GetProperty("soil_temperature_0cm").EnumerateArray().ToList();
                var soilTemp6cm = hourly.GetProperty("soil_temperature_6cm").EnumerateArray().ToList();
                var apparentTemps = hourly.GetProperty("apparent_temperature").EnumerateArray().ToList();
                var humidity = hourly.GetProperty("relative_humidity_2m").EnumerateArray().ToList();
                var precipitation = hourly.GetProperty("precipitation").EnumerateArray().ToList();
                var weatherCodes = hourly.GetProperty("weather_code").EnumerateArray().ToList();
                var windSpeeds = hourly.GetProperty("wind_speed_10m").EnumerateArray().ToList();

                // Find the closest hour to the requested time
                var requestedHour = dateTime.Hour;
                var index = Math.Min(requestedHour, times.Count - 1);

                var tempC = temperatures2m[index].GetDouble();
                var temp80mC = temperatures80m[index].GetDouble();
                var soil0cmC = soilTemp0cm[index].GetDouble();
                var soil6cmC = soilTemp6cm[index].GetDouble();
                var apparentC = apparentTemps[index].GetDouble();
                return new
                {
                    time = times[index].GetString(),
                    
                    // Property names adjusted to match expected output
                    temperature_2m = tempC,
                    temperature_80m = temp80mC,
                    temperature_soil0cm = soil0cmC,
                    temperature_soil6cm = soil6cmC,
                    apparent_temperature = apparentC,
                    
                    relative_humidity_percent = humidity[index].GetInt32(),
                    precipitation_mm = precipitation[index].GetDouble(),
                    weather_code = weatherCodes[index].GetInt32(),
                    weather_description = GetWeatherDescription(weatherCodes[index].GetInt32()),
                    wind_speed_kmh = windSpeeds[index].GetDouble()
                };
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Helper method to convert weather codes to descriptions
    /// </summary>
    private static string GetWeatherDescription(int code)
    {
        return code switch
        {
            0 => "Clear sky",
            1 or 2 or 3 => "Partly cloudy",
            45 or 48 => "Foggy",
            51 or 53 or 55 => "Drizzle",
            61 or 63 or 65 => "Rain",
            71 or 73 or 75 => "Snow",
            77 => "Snow grains",
            80 or 81 or 82 => "Rain showers",
            85 or 86 => "Snow showers",
            95 => "Thunderstorm",
            96 or 99 => "Thunderstorm with hail",
            _ => "Unknown"
        };
    }
}
