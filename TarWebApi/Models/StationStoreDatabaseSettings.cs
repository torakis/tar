namespace TarWebApi.Models;

public class StationStoreDatabaseSettings : IStationStoreDatabaseSettings
{
    public string ConnectionString { get; set; } = String.Empty;
    public string DatabaseName { get; set; } = String.Empty;
    public string StationsCollectionName { get; set; } = String.Empty;
    public string MeasurementsCollectionName { get; set; } = String.Empty;
    public string SurveysCollectionName { get; set; } = String.Empty;
    public string SurveyAnswersCollectionName { get; set; } = String.Empty;
}

