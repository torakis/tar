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

// Configure MQTT client as a singleton
builder.Services.AddSingleton<IMqttClient>(sp =>
{
    var factory = new MqttFactory();
    return factory.CreateMqttClient();
});

// Add MqttService as a singleton and ensure it connects at startup
builder.Services.AddSingleton<IMqttService, MqttService>(sp =>
{
    var mqttClient = sp.GetRequiredService<IMqttClient>();
    var mqttService = new MqttService(mqttClient);
    mqttService.ConnectAsync().Wait(); // Connect to MQTT broker during startup
    return mqttService;
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

