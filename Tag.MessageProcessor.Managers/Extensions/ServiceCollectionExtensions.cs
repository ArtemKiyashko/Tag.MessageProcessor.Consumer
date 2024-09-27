using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Tag.MessageProcessor.Repositories;

namespace Tag.MessageProcessor.Managers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChatManager(this IServiceCollection services, ChatOptions chatOptions)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.UseCredential(new ManagedIdentityCredential());
            if (chatOptions.ServiceUri is not null)
                clientBuilder.AddTableServiceClient(chatOptions.ServiceUri);
            else
            {
                if (string.IsNullOrEmpty(chatOptions.TableConnectionString))
                    throw new ArgumentException($"{nameof(chatOptions.ServiceUri)} or {nameof(chatOptions.TableConnectionString)} required");
                clientBuilder.AddTableServiceClient(chatOptions.TableConnectionString);
            }
        });

        services.AddSingleton<IChatRepository, ChatRepository>((builder) =>
        {
            var tableServiceClient = builder.GetRequiredService<TableServiceClient>();
            var tableClient = tableServiceClient.GetTableClient(chatOptions.ChatTable);
            tableClient.CreateIfNotExists();
            return new ChatRepository(tableClient);
        });
        services.AddScoped<IChatTitleRepository, ChatTitleRepository>((builder) =>
        {
            var tableServiceClient = builder.GetRequiredService<TableServiceClient>();
            var tableClient = tableServiceClient.GetTableClient(chatOptions.ChatTitleTable);
            tableClient.CreateIfNotExists();
            return new ChatTitleRepository(tableClient);
        });
        return services;
    }
}
