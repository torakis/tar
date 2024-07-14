using System.Text;
using Microsoft.AspNetCore.Mvc;
using TarWebApi.Models.Contracts;
using TarWebApi.Services;

namespace TarWebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MqttController : ControllerBase
{
    private readonly IMqttService _mqttService;
    public MqttController(IMqttService mqttService) =>
        _mqttService = mqttService;

    [HttpPost]
    [Route("CreateMessage")]
    public async Task<ActionResult<string>> CreateMessage([FromBody] MqttMessageDto messageDto)
    {
        try
        {
            await _mqttService.PublishAsync(messageDto.Topic, messageDto.Payload);
            return Ok(messageDto.Topic);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to publish message: {ex.Message}");
        }
    }
    
    [HttpPost]
    [Route("SubscribeToMessage")]
    public async Task<ActionResult<string>> SubscribeToMessage(string topic)
    {
        try
        {
            try
            {
                var response = await _mqttService.SubscribeAsync(topic);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to subscribe to topic: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to subscribe to topic: {ex.Message}");
        }
    }
}

public class MqttMessageDto
{
    public string Topic { get; set; }
    public string Payload { get; set; }
}