using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MongoDB.Driver;
using TarWebApi.Models;
using TarWebApi.Models.Contracts;

namespace TarWebApi.Services;
public class MqttBackgroundService : BackgroundService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly IMongoCollection<Measurement> _measurementsCollection;

    public MqttBackgroundService(IMqttClient mqttClient, 
        MqttClientOptions mqttOptions,
        IStationStoreDatabaseSettings settings, 
        IMongoClient mongoClient)
    {
        _mqttClient = mqttClient;
        _mqttOptions = mqttOptions;
        var db = mongoClient.GetDatabase(settings.DatabaseName);
        _measurementsCollection = db.GetCollection<Measurement>(settings.MeasurementsCollectionName);

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set up the message handling logic
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            Console.WriteLine($"Received message: {message} on topic: {e.ApplicationMessage.Topic}");

            // Save the message to the database
            await SaveMessageToDatabase(e.ApplicationMessage.Topic, message);
        };

        // Connect to the MQTT broker
        await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
        Console.WriteLine("Connected to MQTT broker.");

        // Subscribe to the specified topic
        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic("demokritos/weather/#").Build(), stoppingToken);
        Console.WriteLine("Subscribed to topic: demokritos/weather/#");


        // Keep the application running to listen for messages
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken); // Keep the loop running
        }

        // Disconnect from the MQTT broker when the application is stopped
        await _mqttClient.DisconnectAsync();
    }

    private async Task SaveMessageToDatabase(string topic, string message)
    {
        try
        {
            var request = new CreateMeasurementRequest()
            {
                Measurement = new Measurement()
                {
                    StationId = message
                }
            };
            await _measurementsCollection.InsertOneAsync(request.Measurement);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
