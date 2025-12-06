using OrderService.Application.Common;
using OrderService.Application.Common.Interfaces;
using OrderService.Application.Strategies;
using OrderService.Infrastructure.Caching;
using OrderService.Infrastructure.ExternalServices;
using OrderService.Infrastructure.Messaging;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379")
//);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        Assembly.GetExecutingAssembly(), 
        Assembly.GetAssembly(typeof(IMessageProducer))!
    )
);
builder.Services.AddSingleton<ICacheService, DummyCacheService>();
builder.Services.AddScoped<IMessageProducer, DummyMessageProducer>();
//builder.Services.AddScoped<ICacheService, RedisCacheService>();
//builder.Services.AddScoped<IMessageProducer, KafkaProducer>();
//var baseUrl = builder.Configuration.GetSection("ExternalServices")["NotificationServiceBaseUrl"];
var baseUrl = builder.Configuration.GetSection("ExternalServices")["NotificationServiceBaseUrl"];

builder.Services.AddHttpClient<INotificationClient, NotificationClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl!);
});
builder.Services.AddScoped<IOrderProcessingStrategy, PendingOrderStrategy>();
builder.Services.AddScoped<IOrderProcessingStrategy, ConfirmedOrderStrategy>();
builder.Services.AddScoped<IOrderProcessingStrategy, ShippedOrderStrategy>();
builder.Services.AddScoped<IOrderProcessingStrategy, CanceledOrderStrategy>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}
app.Run();

