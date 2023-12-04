namespace TarWebApi.Models;

public interface IStationStoreDatabaseSettings
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
    string StationsCollectionName { get; set; }
    string MeasurementsCollectionName { get; set; }
}

