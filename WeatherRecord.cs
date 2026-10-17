namespace WeatherDataAnalyzer;

// Represents one daily observation from the historical weather CSV.
public class WeatherRecord
{
    public string Location { get; set; } = "";
    public DateTime Date { get; set; }
    public double Precipitation { get; set; }
    public double TemperatureMax { get; set; }
    public double TemperatureMin { get; set; }
    public double Wind { get; set; }
    public string Weather { get; set; } = "";

    // The mean daily temperature is useful for comparing days and years.
    public double MeanTemperature => (TemperatureMax + TemperatureMin) / 2.0;

    // Daily temperature range shows how much temperatures varied that day.
    public double TemperatureRange => TemperatureMax - TemperatureMin;
}
