using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MongoDB.Driver;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;
using System.Text.Json;

namespace TarWebApi.Services;
public class MqttBackgroundService : BackgroundService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly IMongoCollection<Measurement> _measurementsCollection;
    private readonly ILogger<MqttBackgroundService> _logger;

    public MqttBackgroundService(IMqttClient mqttClient, 
        MqttClientOptions mqttOptions,
        IStationStoreDatabaseSettings settings, 
        IMongoClient mongoClient,
        ILogger<MqttBackgroundService> logger)
    {
        _mqttClient = mqttClient;
        _mqttOptions = mqttOptions;
        _logger = logger;
        var db = mongoClient.GetDatabase(settings.DatabaseName);
        _measurementsCollection = db.GetCollection<Measurement>(settings.MeasurementsCollectionName);

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {            
            var messageString = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            if (string.IsNullOrEmpty(messageString)) return;

            Measurement measurement = JsonSerializer.Deserialize<Measurement>(messageString);
            
            _logger.LogInformation($"Received message: {messageString} on topic: {e.ApplicationMessage.Topic}");

            // Extract the weather station name from the topic
            var stationName = ExtractStationNameFromTopic(e.ApplicationMessage.Topic);

            _logger.LogInformation($"Topic: {stationName}");
            
            // Save the message and station name to the database
            await SaveMessageToDatabase(stationName, measurement);
        };


        // Connect to the MQTT broker
        await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
        Console.WriteLine("Connected to MQTT broker.");

        // Subscribe to the specified topic
        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic("demokritos/weather/#").Build(), stoppingToken);
        Console.WriteLine("Subscribed to topic: demokritos/weather/#");
        _logger.LogInformation("Subscribed to topic: demokritos/weather/#");



        // Keep the application running to listen for messages
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken); // Keep the loop running
        }

        // Disconnect from the MQTT broker when the application is stopped
        await _mqttClient.DisconnectAsync();
    }

    private string ExtractStationNameFromTopic(string topic)
    {
        // Assuming the topic is in the format demokritos/weather/{station-name}
        var topicParts = topic.Split('/');
        if (topicParts.Length > 2)
        {
            return topicParts.Last(); // This will return the station name like ws-demokritos-2
        }
        return string.Empty; // Fallback in case the topic format is unexpected
    }


    private async Task SaveMessageToDatabase(string topic, Measurement message)
    {
        try
        {
            var request = new CreateMeasurementRequest() { Measurement = message };
            request.Measurement.StationId = topic;

            message.Date = message.Timestamp != null?
                DateTimeOffset.FromUnixTimeMilliseconds((long)message.Timestamp).UtcDateTime :
                DateTime.Now;

            await _measurementsCollection.InsertOneAsync(request.Measurement);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogInformation($"Error saving to db: {ex.Message}");
        }
    }
}
