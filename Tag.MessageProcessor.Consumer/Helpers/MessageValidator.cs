using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Tag.MessageProcessor.Consumer.ViewModels;
using Telegram.Bot.Types;

namespace Tag.MessageProcessor.Consumer.Helpers;

public static class MessageValidator
{
    public static async Task<BotTgUpdate> TryGetBotTgUpdate(ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, User bot)
    {
        try
        {
            var tgUpdate = JsonConvert.DeserializeObject<BotTgUpdate>(message.Body.ToString())
                ?? throw new ArgumentException("Cannot deserialize message body as telegram Update", nameof(message));

            if (bot.Username is null)
                throw new ArgumentException("Bot username cannot be null", nameof(bot));

            tgUpdate.BotUsername = bot.Username;

            return tgUpdate;
        }
        catch (Exception)
        {
            await messageActions.DeadLetterMessageAsync(message);
            throw;
        }
    }
}
