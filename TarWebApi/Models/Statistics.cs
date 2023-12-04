
namespace TarWebApi.Models;

public class Statistics
{    
    public DateTime? Day { get; set; }
    public StatisticsDetails? Temperature { get; set; }
    public StatisticsDetails? Humidity { get; set; }
    public StatisticsDetails? Co2 { get; set; }
    public StatisticsDetails? Pm25 { get; set; }
    public StatisticsDetails? Pressure { get; set; }
    public StatisticsDetails? Precipitation { get; set; }
    public StatisticsDetails? WindSpeed { get; set; }
    public StatisticsDetails? WindDirection { get; set; }
}

public class StatisticsDetails
{
    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
    public decimal? Avg { get; set; }
}
