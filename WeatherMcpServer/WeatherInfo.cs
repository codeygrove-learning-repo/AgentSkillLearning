using System.ComponentModel;
namespace WeatherMcpServer
{
    public class WeatherInfo
    {
        [Description("Information about the city, country, and coordinates.")]
        public CityInfo? CityInfo { get; set; }

        [Description("Weather data for the specified city and time.")]
        public WeatherData? WeatherData { get; set; }

        [Description("The date and time in UTC (ISO 8601 format, e.g., 2024-01-13T12:00:00Z) for which the weather data applies.")]
        public string? DateTime { get; set; }

        public override string ToString()
        {
            if (CityInfo == null || WeatherData == null)
                return "Weather unavailable.";

            var city = CityInfo.City ?? "Unknown";
            var country = CityInfo.Country ?? "Unknown";
            var dateTime = DateTime ?? WeatherData.Time ?? "N/A";

            return $@"Weather for {city}, {country} at {dateTime}:
{WeatherData.WeatherDescription ?? "Unknown"} - Temp 2m: {WeatherData.TemperatureAt2m:F1}°C, 80m: {WeatherData.TemperatureAt80m:F1}°C, feels like {WeatherData.ApparentTemperature:F1}°C
Humidity {WeatherData.RelativeHumidityPercent}%, Wind {WeatherData.WindSpeedKmh:F1} km/h, Precipitation {WeatherData.PrecipitationMm:F1} mm
Soil temps: 0cm={WeatherData.SoilTemperatureAt0cm:F1}°C, 6cm={WeatherData.SoilTemperatureAt6cm:F1}°C";
        }
    }

    public class CityInfo
    {
        [Description("The name of the city.")]
        public string? City { get; set; }

        [Description("The name of the country.")]
        public string? Country { get; set; }

        [Description("Geographical coordinates of the city.")]
        public Coordinates? Coordinates { get; set; }
    }

    public class Coordinates
    {
        [Description("Latitude of the city location.")]
        public double Latitude { get; set; }

        [Description("Longitude of the city location.")]
        public double Longitude { get; set; }
    }

    public class WeatherData
    {
        [Description("The time in UTC for the weather data (ISO 8601 format, e.g., 2024-01-13T12:00:00Z).")]
        public string? Time { get; set; }

        [Description("Temperature at 2 meters above ground (°C).")]
        public double TemperatureAt2m { get; set; }

        [Description("Temperature at 80 meters above ground (°C).")]
        public double TemperatureAt80m { get; set; }
        
        [Description("Soil temperature at 0 cm depth (°C).")]
        public double SoilTemperatureAt0cm { get; set; }
        
        [Description("Soil temperature at 6 cm depth (°C).")]
        public double SoilTemperatureAt6cm { get; set; }        

        [Description("Apparent temperature (°C).")]
        public double ApparentTemperature { get; set; }

        [Description("Relative humidity at 2 meters above ground (percent).")]
        public int RelativeHumidityPercent { get; set; }

        [Description("Precipitation in millimeters (mm).")]
        public double PrecipitationMm { get; set; }

        [Description("Weather code as provided by the Open-Meteo API.")]
        public int WeatherCode { get; set; }

        [Description("Human-readable weather description for the weather code.")]
        public string? WeatherDescription { get; set; }

        [Description("Wind speed at 10 meters above ground (km/h).")]
        public double WindSpeedKmh { get; set; }
    }
}
