using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MQTTnet;
using MQTTnet.Client;
using TarWebApi.Models;
using TarWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<StationStoreDatabaseSettings>(builder.Configuration.GetSection(nameof(StationStoreDatabaseSettings)));
builder.Services.AddSingleton<IStationStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<StationStoreDatabaseSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetValue<string>("StationStoreDatabaseSettings:ConnectionString")));
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IMeasurementService, MeasurementService>();

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
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.Run();

