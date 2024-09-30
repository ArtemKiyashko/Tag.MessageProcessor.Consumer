using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Tag.MessageProcessor.Consumer.Helpers;
using Telegram.Bot;

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
            var bot = await _telegramBotClient.GetMeAsync();

            var tgUpdate = await MessageValidator.TryGetBotTgUpdate(message, messageActions, bot);

            var command = CommandParser.GetTgCommand(tgUpdate);
        }
    }
}
