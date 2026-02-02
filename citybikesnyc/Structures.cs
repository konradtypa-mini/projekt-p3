using System;

namespace citybikesnyc {
    
    public class Trip
    {
        public int TripDuration { get; set; }
        public DateTime StartTime { get; set; }
        public required string StartStationName { get; set; }
        public required string BikeId { get; set; }
        public int? BirthYear { get; set; }
        public required string Gender { get; set; }
    }

    public class WeatherInfo
    {
        public DateTime Date { get; set; }
        public double Precipitation { get; set; }
        public double AvgTemp { get; set; }
    }
}