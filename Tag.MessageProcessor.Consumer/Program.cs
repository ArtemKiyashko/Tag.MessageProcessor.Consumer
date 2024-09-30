using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Extensions;
using Telegram.Bot;

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

        services.AddSingleton<ITelegramBotClient>(factory => {
            var botToken = _functionConfig.GetValue<string>("TELEGRAM_BOT_TOKEN") ?? throw new ArgumentException("Bot token required", "TELEGRAM_BOT_TOKEN");
            return new TelegramBotClient(botToken);
        });
        
    })
    .Build();

host.Run();
