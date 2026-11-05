using ProductMessengerRabbitMQ.Worker;
using ProductMessengerRabbitMQ.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransitConfigurations(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
