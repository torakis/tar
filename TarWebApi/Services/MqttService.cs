using System.Text;
using MQTTnet.Packets;

namespace TarWebApi.Services;
using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;


public class MqttService : IMqttService
{
    private readonly IMqttClient _mqttClient;
    private TaskCompletionSource<string> _messageReceivedTask;

    public MqttService(IMqttClient mqttClient)
    {
        _mqttClient = mqttClient;
        _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;
    }
    
    private Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        if (_messageReceivedTask.Task.IsCompleted) return Task.CompletedTask;
        var receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        _messageReceivedTask.SetResult(receivedMessage);
        return Task.CompletedTask;
    }

    public async Task ConnectAsync()
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883) // Replace with your MQTT broker's IP and port
            .WithClientId("TarApp") // Choose a client ID for your application
            .Build();

        await _mqttClient.ConnectAsync(options);
    }

    public async Task DisconnectAsync()
    {
        await _mqttClient.DisconnectAsync();
    }

    public async Task PublishAsync(string topic, string payload)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
            .WithRetainFlag(false)
            .Build();

        await _mqttClient.PublishAsync(message);
    }

    public async Task<string> SubscribeAsync(string topic)
    {
        _messageReceivedTask = new TaskCompletionSource<string>();
        
        var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(f => 
            {
                f.WithTopic(topic);
            })
            .Build();


        Console.WriteLine("Connected to MQTT broker successfully.");
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Set a timeout for the subscription


        // Subscribe to a topic
        await _mqttClient.SubscribeAsync(topic);
        
        try
        {
            await _mqttClient.SubscribeAsync(subscribeOptions, cts.Token);
            // Wait for the message to be received with a timeout
            var delayTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(_messageReceivedTask.Task, delayTask);
            if (completedTask == delayTask)
            {
                throw new TimeoutException("Timeout while waiting for message.");
            }
            return await _messageReceivedTask.Task;
        }
        catch (OperationCanceledException)
        {
            return "Subscription operation was canceled.";
            throw;
        }
        catch (Exception ex)
        {
            return ex.Message;
            throw;
        }

        // Callback function when a message is received
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
            return Task.CompletedTask;
        };
    }
}