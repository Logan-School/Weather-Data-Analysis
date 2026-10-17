using MathNet.Numerics.Statistics;

namespace WeatherDataAnalyzer;

// Contains the analysis methods. Each question uses at least one data operation.
public static class WeatherAnalyzer
{
    public static void AnswerQuestions(List<WeatherRecord> records)
    {
        AnswerQuestion1(records);
        AnswerQuestion2(records);
        AnswerQuestion3(records);
        AnswerQuestion4(records);
    }

    private static void AnswerQuestion1(List<WeatherRecord> records)
    {
        Console.WriteLine("QUESTION 1: How have average temperatures changed by year?");

        // Aggregate daily mean temperatures by year, then sort chronologically.
        var yearlyAverages = records
            .GroupBy(r => r.Date.Year)
            .Select(group => new
            {
                Year = group.Key,
                Average = Statistics.Mean(group.Select(r => r.MeanTemperature).ToArray()),
                Days = group.Count()
            })
            .OrderBy(item => item.Year)
            .ToList();

        foreach (var year in yearlyAverages)
        {
            Console.WriteLine($"{year.Year}: average daily temperature {year.Average:F1}° (from {year.Days} records)");
        }

        if (yearlyAverages.Count >= 2)
        {
            double change = yearlyAverages[^1].Average - yearlyAverages[0].Average;
            string direction = change > 0 ? "warmer" : change < 0 ? "cooler" : "about the same";
            Console.WriteLine($"Comparison: the last represented year was {Math.Abs(change):F1}° {direction} on average than the first.");
        }
        Console.WriteLine();
    }

    private static void AnswerQuestion2(List<WeatherRecord> records)
    {
        Console.WriteLine("QUESTION 2: Which days had the most extreme high temperatures?");

        // Filter to hot days, then sort from highest maximum temperature to lowest.
        var hotDays = records
            .Where(r => r.TemperatureMax >= 30.0)
            .OrderByDescending(r => r.TemperatureMax)
            .Take(5)
            .ToList();

        Console.WriteLine("Top five hottest days (threshold for a hot day: 30° in the dataset's temperature unit):");
        if (hotDays.Count == 0)
        {
            Console.WriteLine("No records met the hot-day threshold.");
        }
        else
        {
            foreach (var day in hotDays)
            {
                Console.WriteLine($"{day.Date:yyyy-MM-dd}: high {day.TemperatureMax:F1}°, low {day.TemperatureMin:F1}°, weather: {day.Weather}");
            }
        }
        Console.WriteLine();
    }

    private static void AnswerQuestion3(List<WeatherRecord> records)
    {
        Console.WriteLine("QUESTION 3: Which months have the highest average temperatures?");

        // Group by month number across all years, aggregate with the statistics library, and sort to identify the warmest months in this dataset.
        var monthlyAverages = records
            .GroupBy(r => r.Date.Month)
            .Select(group => new
            {
                Month = group.Key,
                Average = Statistics.Mean(group.Select(r => r.MeanTemperature).ToArray())
            })
            .OrderByDescending(item => item.Average)
            .ToList();

        foreach (var month in monthlyAverages)
        {
            Console.WriteLine($"{System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month.Month)}: {month.Average:F1}° average daily temperature");
        }
        if (monthlyAverages.Count > 0)
        {
            Console.WriteLine($"Warmest month in these records: {System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(monthlyAverages[0].Month)}.");
        }
        Console.WriteLine();
    }

    private static void AnswerQuestion4(List<WeatherRecord> records)
    {
        Console.WriteLine("QUESTION 4: Which days had unusually large temperature swings or heavy precipitation?");

        // MathNet.Numerics calculates standard deviation to help define an unusual temperature swing. Precipitation is also sorted to find the wettest days.
        double averageRange = Statistics.Mean(records.Select(r => r.TemperatureRange).ToArray());
        double rangeStdDev = Statistics.StandardDeviation(records.Select(r => r.TemperatureRange).ToArray());
        double unusualRangeThreshold = averageRange + rangeStdDev;

        var unusualDays = records
            .Where(r => r.TemperatureRange >= unusualRangeThreshold)
            .OrderByDescending(r => r.TemperatureRange)
            .Take(5)
            .ToList();

        Console.WriteLine($"Average daily temperature range: {averageRange:F1}°");
        Console.WriteLine($"Unusual swing threshold (average + standard deviation): {unusualRangeThreshold:F1}°");
        foreach (var day in unusualDays)
        {
            Console.WriteLine($"{day.Date:yyyy-MM-dd}: temperature range {day.TemperatureRange:F1}°");
        }

        var wettestDays = records
            .OrderByDescending(r => r.Precipitation)
            .Take(5)
            .ToList();

        Console.WriteLine("Five days with the most precipitation:");
        foreach (var day in wettestDays)
        {
            Console.WriteLine($"{day.Date:yyyy-MM-dd}: precipitation {day.Precipitation:F2}");
        }
        Console.WriteLine();
    }
}
