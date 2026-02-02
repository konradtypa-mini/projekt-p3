namespace citybikesnyc
{
    public class Queries
    {
        
        public static void LongestShortest(List<Trip> trips)
        {
            // najdłuzszy i najkrotszy trip i ile lat mieli ich rowerzyści
            
            var shortest = trips.MinBy(t => t.TripDuration);
            var longest = trips.MaxBy(t => t.TripDuration);
            int currentYear = 2014;

            Console.WriteLine("1. Ekstremalne trasy i wiek:");
            
            if (shortest != null)
            {
                string age = shortest.BirthYear.HasValue ? $"{currentYear - shortest.BirthYear} lat" : "Wiek nieznany";
                Console.WriteLine($"   Najkrótsza: {shortest.TripDuration}s ({age})");
            }

            if (longest != null)
            {
                string age = longest.BirthYear.HasValue ? $"{currentYear - longest.BirthYear} lat" : "Wiek nieznany";
                Console.WriteLine($"   Najdłuższa: {longest.TripDuration / 3600.0:F1}h ({age})");
            }
        }

        public static void PopularStation(List<Trip> trips)
        {
            // Najczęściej używana stacja i ile z niej korzystało kobiet a ile mężczyzn
            var stat = trips
                .GroupBy(t => t.StartStationName)
                .Select(g => new
                {
                    Name = g.Key,
                    Total = g.Count(),
                    Men = g.Count(t => t.Gender == "1"),
                    Women = g.Count(t => t.Gender == "2")
                })
                .MaxBy(x => x.Total);

            Console.WriteLine("2. Najpopularniejsza stacja:");
            if (stat != null)
            {
                Console.WriteLine($"   {stat.Name}");
                Console.WriteLine($"   Razem: {stat.Total} | Mężczyźni: {stat.Men} | Kobiety: {stat.Women}");
            }
        }

        public static void TopBikeRoutes(List<Trip> trips)
        {
            // Najczęściej używany rower i jego najdłuższa i najkrótsza trasa
            var topBike = trips
                .GroupBy(t => t.BikeId)
                .Select(g => new
                {
                    BikeId = g.Key,
                    Count = g.Count(),
                    MaxTrip = g.Max(t => t.TripDuration),
                    MinTrip = g.Min(t => t.TripDuration)
                })
                .MaxBy(x => x.Count);

            Console.WriteLine("3. Najczęściej uzywany rower:");
            if (topBike != null)
            {
                Console.WriteLine($"   ID: {topBike.BikeId} (Użyty {topBike.Count} razy)");
                Console.WriteLine($"   Rekordy: Min {topBike.MinTrip}s | Max {topBike.MaxTrip / 60.0:F1} min");
            }
        }

        public static void RainImpact(List<Trip> trips, Dictionary<DateTime, WeatherInfo> weather)
        {
            // Średni czas przejazdu w deszczu vs bez deszczu
            var joined = trips
                .Where(t => weather.ContainsKey(t.StartTime.Date))
                .Select(t => new { Trip = t, Weather = weather[t.StartTime.Date] })
                .ToList();

            if (joined.Count == 0)
            {
                Console.WriteLine("Pogoda: Brak danych do analizy");
                return;
            }

            var stats = joined
                .GroupBy(x => x.Weather.Precipitation > 0)
                .Select(g => new
                {
                    IsRainy = g.Key,
                    AvgDuration = g.Average(x => x.Trip.TripDuration),
                    Count = g.Count()
                })
                .ToList();

            Console.WriteLine("4. Wpływ deszczu na średni czas jazdy:");
            foreach (var s in stats)
            {
                string aura = s.IsRainy ? "Deszcz" : "Brak deszczu";
                Console.WriteLine($"   {aura}: {s.AvgDuration:F0} sekund (na podstawie {s.Count} przejazdów)");
            }
        }

        public static void TemperatureImpact(List<Trip> trips, Dictionary<DateTime, WeatherInfo> weather)
        {
            // liczba wypożyczeń w zależności od temperatury
            var joined = trips
                .Where(t => weather.ContainsKey(t.StartTime.Date))
                .Select(t => new { Trip = t, Weather = weather[t.StartTime.Date] })
                .ToList();

            if (joined.Count == 0)
            {
                Console.WriteLine("Pogoda: Brak danych do analizy");
                return;
            }

            var tempStats = joined
                .GroupBy(x => 
                {
                    double t = x.Weather.AvgTemp;
                    if (t < 5) return "Bardzo Zimno (<5C)";
                    if (t < 15) return "Chłodno (5-15C)";
                    if (t < 25) return "Ciepło (15-25C)";
                    return "Gorąco (>25C)";
                })
                .Select(g => new
                {
                    Category = g.Key,
                    TripCount = g.Count()
                })
                .OrderBy(x => x.Category)
                .ToList();

            Console.WriteLine("5. Popularność roweru a temperatura:");
            foreach (var s in tempStats)
            {
                Console.WriteLine($"   {s.Category}: {s.TripCount} wypożyczeń");
            }
        }
    }
}