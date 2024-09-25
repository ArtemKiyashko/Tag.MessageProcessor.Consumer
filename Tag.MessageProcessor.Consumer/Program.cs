using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Extensions;

IConfiguration _functionConfig;
ChatOptions _chatOptions = new();

_functionConfig = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        _functionConfig.GetSection(nameof(ChatOptions)).Bind(_chatOptions);

        services.AddChatManager(_chatOptions);
        
    })
    .Build();

host.Run();
