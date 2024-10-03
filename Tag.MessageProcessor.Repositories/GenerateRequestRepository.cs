using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Tag.MessageProcessor.Repositories.Entities;

namespace Tag.MessageProcessor.Repositories;

public class GenerateRequestRepository(ServiceBusSender serviceBusSender) : IGenerateRequestRepository
{
    private readonly ServiceBusSender _serviceBusSender = serviceBusSender;

    public async Task SendRequest(GenerateRequestEntity entity)
    {
        var sbMessage = new ServiceBusMessage(JsonSerializer.Serialize(entity))
        {
            SessionId = entity.ChatTgId.ToString()
        };

        await _serviceBusSender.SendMessageAsync(sbMessage);
    }
}
