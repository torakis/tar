namespace TarWebApi.Models;

public interface IMqttSettings
{
    public string ClientId { get; set; }
    public string BrokerAddress { get; set; }
    public int BrokerPort { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool CleanSession { get; set; }
}

