using Core.Common;
using Infrastructure.Common;
using RabbitMQ.Client;
using Worker;
using Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<BackgroundWorker>();

builder.Services.AddCore();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<TaskConsumer>();

var host = builder.Build();
host.Run();
