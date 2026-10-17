using System.Globalization;

namespace WeatherDataAnalyzer;

// Reads and converts rows from the CSV dataset.
public static class WeatherDataLoader
{
    public static List<WeatherRecord> Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Weather CSV was not found.", filePath);
        }

        string[] lines = File.ReadAllLines(filePath);
        var records = new List<WeatherRecord>();

        if (lines.Length < 2)
        {
            return records;
        }

        // Find column positions by header names rather than assuming their order.
        string[] headers = lines[0].Split(',');
        int locationIndex = FindColumn(headers, "location");
        int dateIndex = FindColumn(headers, "date");
        int precipitationIndex = FindColumn(headers, "precipitation");
        int maxIndex = FindColumn(headers, "temp_max");
        int minIndex = FindColumn(headers, "temp_min");
        int windIndex = FindColumn(headers, "wind");
        int weatherIndex = FindColumn(headers, "weather");

        for (int i = 1; i < lines.Length; i++)
        {
            // This dataset's text fields do not contain commas, so a simple split is sufficient.
            string[] columns = lines[i].Split(',');

            int requiredMaxIndex = new[] { locationIndex, dateIndex, maxIndex, minIndex }.Max();
            if (columns.Length <= requiredMaxIndex)
            {
                continue;
            }

            // Skip rows that have missing or invalid required measurements.
            if (!DateTime.TryParse(columns[dateIndex], CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime date) ||
                !TryNumber(columns[maxIndex], out double tempMax) ||
                !TryNumber(columns[minIndex], out double tempMin))
            {
                continue;
            }

            double precipitation = ReadOptionalNumber(columns, precipitationIndex);
            double wind = ReadOptionalNumber(columns, windIndex);
            string weather = ReadOptionalText(columns, weatherIndex);

            records.Add(new WeatherRecord
            {
                Location = columns[locationIndex].Trim(),
                Date = date,
                Precipitation = precipitation,
                TemperatureMax = tempMax,
                TemperatureMin = tempMin,
                Wind = wind,
                Weather = weather
            });
        }

        return records;
    }

    private static int FindColumn(string[] headers, string name)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        throw new InvalidDataException($"The CSV is missing the required column '{name}'.");
    }

    private static bool TryNumber(string value, out double number)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number)
            && double.IsFinite(number);
    }

    private static double ReadOptionalNumber(string[] columns, int index)
    {
        if (index >= 0 && index < columns.Length && TryNumber(columns[index], out double value))
        {
            return value;
        }

        return 0.0;
    }

    private static string ReadOptionalText(string[] columns, int index)
    {
        return index >= 0 && index < columns.Length ? columns[index].Trim() : "";
    }
}
