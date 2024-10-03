using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MQTTnet;
using MQTTnet.Client;
using Serilog;
using TarWebApi.Models;
using TarWebApi.Services;
using System.IO; // For working with directories

var builder = WebApplication.CreateBuilder(args);

// Create logs directory if it doesn't exist
string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);  // Create the logs directory
}

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()  // Set the minimum log level
    .WriteTo.Console()  // Log to console
    .WriteTo.File(Path.Combine(logDirectory, "tar.log"), rollingInterval: RollingInterval.Day)  // Log to a file with daily rolling
    .CreateLogger();

// Replace default logger with Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.Configure<StationStoreDatabaseSettings>(builder.Configuration.GetSection(nameof(StationStoreDatabaseSettings)));
builder.Services.AddSingleton<IStationStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<StationStoreDatabaseSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetValue<string>("StationStoreDatabaseSettings:ConnectionString")));
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IMeasurementService, MeasurementService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind MqttSettings from appsettings.json
builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));

// Register MQTT Client as a singleton
builder.Services.AddSingleton<IMqttClient>(sp =>
{
    var mqttFactory = new MqttFactory();
    return mqttFactory.CreateMqttClient();
});

// Use MqttSettings to configure MqttClientOptions
builder.Services.AddSingleton<MqttClientOptions>(sp =>
{
    var mqttSettings = sp.GetRequiredService<IOptions<MqttSettings>>().Value;
    return new MqttClientOptionsBuilder()
        .WithClientId(mqttSettings.ClientId)
        .WithTcpServer(mqttSettings.BrokerAddress, mqttSettings.BrokerPort)
        .WithCredentials(mqttSettings.Username, mqttSettings.Password)
        .WithCleanSession(mqttSettings.CleanSession)
        .Build();
});

builder.Services.AddHostedService<MqttBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();

// Ensure to flush the Serilog logs on shutdown
Log.CloseAndFlush();
