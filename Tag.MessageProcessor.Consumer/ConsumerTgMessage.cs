using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tag.MessageProcessor.Consumer.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Tag.MessageProcessor.Consumer
{
    public class ConsumerTgMessage(ILogger<ConsumerTgMessage> logger, ITelegramBotClient telegramBotClient)
    {
        private readonly ILogger<ConsumerTgMessage> _logger = logger;
        private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

        [Function(nameof(ConsumerTgMessage))]
        public async Task Run(
            [ServiceBusTrigger(topicName: "tgmessages", subscriptionName: "messageprocessor", IsSessionsEnabled = true, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            Update? tgUpdate;
            var bot = await _telegramBotClient.GetMeAsync();
            try
            {
                tgUpdate = JsonConvert.DeserializeObject<Update>(message.Body.ToString());
                if (tgUpdate is null)
                    throw new ArgumentException("Value cannot be null", nameof(tgUpdate));

                if (bot.Username is null)
                    throw new ArgumentException("Bot username cannot be null", nameof(bot.Username));
                var command = CommandParser.GetTgCommand(tgUpdate, bot.Username);
            }
            catch (Exception)
            {
                await messageActions.DeadLetterMessageAsync(message);
                throw;
            }
        }
    }
}
