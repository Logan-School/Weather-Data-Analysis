using WeatherDataAnalyzer;

// Start the application and load the weather observations from the CSV file.
Console.WriteLine("Historical Weather Data Analyzer");
Console.WriteLine();

string csvPath = Path.Combine(AppContext.BaseDirectory, "weather.csv");

try
{
    List<WeatherRecord> records = WeatherDataLoader.Load(csvPath);

    if (records.Count == 0)
    {
        Console.WriteLine("No valid weather records were found in weather.csv.");
        return;
    }

    Console.WriteLine($"Loaded {records.Count} valid weather records.");
    Console.WriteLine($"Date range: {records.Min(r => r.Date):yyyy-MM-dd} to {records.Max(r => r.Date):yyyy-MM-dd}");
    Console.WriteLine();

    // Each report method answers a separate question using filtering, sorting, aggregation, or conversion of the data.
    WeatherAnalyzer.AnswerQuestions(records);
}
catch (FileNotFoundException)
{
    Console.WriteLine("Could not find weather.csv.");
    Console.WriteLine("Download the real dataset and place weather.csv beside the project file.");
}
catch (Exception ex)
{
    Console.WriteLine($"The data could not be analyzed: {ex.Message}");
}
