using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Extensions;
using Telegram.Bot;
using Microsoft.Extensions.Logging;
using Tag.MessageProcessor.Consumer;

IConfiguration _functionConfig;
ChatOptions _chatOptions = new();
GenerateRequestOptions _generateRequestOptions = new();

_functionConfig = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        _functionConfig.GetSection(nameof(ChatOptions)).Bind(_chatOptions);
        _functionConfig.GetSection(nameof(GenerateRequestOptions)).Bind(_generateRequestOptions);

        services.AddChatManager(_chatOptions);
        services.AddGenerateRequestManager(_generateRequestOptions);
        services.AddSingleton<ITelegramBotClient>(factory => {
            var botToken = _functionConfig.GetValue<string>("TELEGRAM_BOT_TOKEN") ?? throw new ArgumentException("Bot token required", "TELEGRAM_BOT_TOKEN");
            return new TelegramBotClient(botToken);
        });
        services.AddGenerateRequestManager(_generateRequestOptions);

        services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();
        //ref: https://github.com/devops-circle/Azure-Functions-Logging-Tests/blob/master/Func.Isolated.Net7.With.AI/Program.cs#L46
        services.Configure<LoggerFilterOptions>(options =>
        {
            var toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

            if (toRemove is not null)
            {
                options.Rules.Remove(toRemove);
            }
        });
        
    })
    .Build();

host.Run();
