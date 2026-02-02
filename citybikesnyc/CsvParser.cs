using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace citybikesnyc
{
    public class CsvParser
    {
        public static Trip? MapCsvLineToTrip(string line)
        {
            try
            {
                var p = line.Split(',');
                if (p.Length < 15) return null;

                return new Trip
                {
                    TripDuration = int.Parse(p[0].Trim('"')),
                    StartTime = DateTime.Parse(p[1].Trim('"'), CultureInfo.InvariantCulture),
                    StartStationName = p[4].Trim('"'),
                    BikeId = p[11].Trim('"'),
                    BirthYear = int.TryParse(p[13].Trim('"'), out int y) ? y : (int?)null,
                    Gender = p[14].Trim('"') // 1 - male, 2 - female
                };
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<DateTime, WeatherInfo> LoadWeatherManual(string path)
        {
            var dict = new Dictionary<DateTime, WeatherInfo>();
            try
            {
                string[] lines = File.ReadAllLines(path);

                // analiza nagłówka, zeby znaleźć numery kolumn
                var headerParts = lines[0].Split(',').Select(h => h.Trim('"').Trim()).ToList();

                int dateIdx = headerParts.IndexOf("DATE");
                int prcpIdx = headerParts.IndexOf("PRCP"); // opady
                int tmaxIdx = headerParts.IndexOf("TMAX"); // max temp
                int tminIdx = headerParts.IndexOf("TMIN"); // min temp
                
                if (dateIdx == -1)
                {
                    Console.WriteLine("Błąd czytania CSV Pogody");
                    return dict;
                }

                // czytanie wierszy danych
                foreach (var line in lines.Skip(1))
                {
                    var p = line.Split(',');
                    if (p.Length <= dateIdx) continue;
                    
                    if (DateTime.TryParse(p[dateIdx].Trim('"'), CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out DateTime date))
                    {
                        double prcp = 0, tmax = 0, tmin = 0;
                        
                        if (prcpIdx != -1 && p.Length > prcpIdx)
                            double.TryParse(p[prcpIdx].Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture,
                                out prcp);

                        if (tmaxIdx != -1 && p.Length > tmaxIdx)
                            double.TryParse(p[tmaxIdx].Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture,
                                out tmax);

                        if (tminIdx != -1 && p.Length > tminIdx)
                            double.TryParse(p[tminIdx].Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture,
                                out tmin);
                        
                        if (!dict.ContainsKey(date))
                        {
                            dict.Add(date, new WeatherInfo
                            {
                                Date = date,
                                Precipitation = prcp,
                                AvgTemp = (tmax + tmin) / 2.0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return dict;
        }
    }
}