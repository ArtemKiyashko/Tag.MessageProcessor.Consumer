using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Tag.MessageProcessor.Consumer.Helpers;
using Tag.MessageProcessor.Managers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Tag.MessageProcessor.Consumer
{
    public class ConsumerTgMessage(
        ILogger<ConsumerTgMessage> logger,
        ICommandHandlerFactory commandHandlerFactory,
        ITelegramBotClient telegramBotClient,
        IChatManager chatManager)
    {
        private readonly ILogger<ConsumerTgMessage> _logger = logger;
        private readonly ICommandHandlerFactory _commandHandlerFactory = commandHandlerFactory;
        private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;
        private readonly IChatManager _chatManager = chatManager;

        [Function(nameof(ConsumerTgMessage))]
        public async Task Run(
            [ServiceBusTrigger(topicName: "tgmessages", subscriptionName: "messageprocessor", IsSessionsEnabled = true, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            var bot = await _telegramBotClient.GetMeAsync();
            var tgUpdate = await MessageValidator.TryGetBotTgUpdate(message, messageActions, bot);
            
            var tgData = CommandParser.GetTgData(tgUpdate);
            if (tgData is null)
                return;

            var commandHandler = _commandHandlerFactory.GetCommandHandler(tgData.TgCommand.CommandType);
            if (commandHandler is null)
                return;

            try
            {
                await commandHandler.ProcessCommand(tgData);
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 403)
            {
                await _chatManager.DeleteChat(tgData.ChatTgId);
            }
        }
    }
}
