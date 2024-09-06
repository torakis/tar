namespace TarWebApi.Models;

public class MqttSettings : IMqttSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string BrokerAddress { get; set; } = string.Empty;
    public int BrokerPort { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool CleanSession { get; set; }
}

