using BusinessLogic;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDataAccessServices();
builder.Services.AddBusinessLogicServices();
builder.Services.AddHostedService<SchedulerService>();
builder.Logging.AddConsole();

using var host = builder.Build();
await host.StartAsync();