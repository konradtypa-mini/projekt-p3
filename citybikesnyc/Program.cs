using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace citybikesnyc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Analizowanie NYC bike trip data dla miesiąca (1-12): ");
            string input = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(input, out int month) || month < 1 || month > 12)
            {
                Console.WriteLine("Miesiąc musi być od 1 do 12.");
                return;
            }

            // ściezki plikow z danymi
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string tripsPath = Path.Combine(baseDir, "Data", "2014-citibike-tripdata");
            string weatherPath = Path.Combine(baseDir, "Data", "Weather");

            if (!Directory.Exists(tripsPath) ||  !Directory.Exists(weatherPath))
            {
                Console.WriteLine("brak dostępu do danych");
                return;
            }

            // pobranie odpowiedniego miesiąca
            string searchPattern = $"{month}_";
            string? monthDir = Directory.GetDirectories(tripsPath)
                .FirstOrDefault(d => Path.GetFileName(d).StartsWith(searchPattern));

            if (monthDir == null)
            {
                Console.WriteLine($"brak danych dla miesiąca {month}.");
                return;
            }

            // wczytywanie tripów
            List<Trip> trips = Directory.GetFiles(monthDir, "*.csv")
                .SelectMany(file => File.ReadLines(file).Skip(1))
                .Select(CsvParser.MapCsvLineToTrip)
                .Where(t => t != null)
                .Select(t => t!)
                .ToList();

            // wczytywanie pogody
            Dictionary<DateTime, WeatherInfo> weatherMap = new Dictionary<DateTime, WeatherInfo>();
            if (Directory.Exists(weatherPath))
            {
                var wFile = Directory.GetFiles(weatherPath, "*.csv").FirstOrDefault();
                if (wFile != null) weatherMap = CsvParser.LoadWeatherManual(wFile);
            }

            // wykonywanie querys
            Console.WriteLine($"\nAnaliza dla {trips.Count} tras:\n");

            Queries.LongestShortest(trips);
            Console.WriteLine();

            Queries.PopularStation(trips);
            Console.WriteLine();

            Queries.TopBikeRoutes(trips);
            Console.WriteLine();
            
            Queries.RainImpact(trips, weatherMap);
            Console.WriteLine();

            Queries.TemperatureImpact(trips, weatherMap);
        }
    }
}