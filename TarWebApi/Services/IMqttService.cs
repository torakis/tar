namespace TarWebApi.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

public interface IMqttService
{
    Task ConnectAsync();
    Task DisconnectAsync();
    Task PublishAsync(string topic, string payload);
    Task<string> SubscribeAsync(string topic);
}