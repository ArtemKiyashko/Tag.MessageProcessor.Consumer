using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Tag.MessageProcessor.Consumer.Helpers;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Dtos;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer
{
    public class ConsumerTgMessage(ILogger<ConsumerTgMessage> logger, ITelegramBotClient telegramBotClient, IChatManager chatManager)
    {
        private readonly ILogger<ConsumerTgMessage> _logger = logger;
        private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;
        private readonly IChatManager _chatManager = chatManager;

        [Function(nameof(ConsumerTgMessage))]
        public async Task Run(
            [ServiceBusTrigger(topicName: "tgmessages", subscriptionName: "messageprocessor", IsSessionsEnabled = true, Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            var bot = await _telegramBotClient.GetMeAsync();

            var tgUpdate = await MessageValidator.TryGetBotTgUpdate(message, messageActions, bot);

            var command = CommandParser.GetTgCommand(tgUpdate);

            // ignore if it is not a direct command
            if (command is null)
                return;

            var chatDto = new ChatDto
            {
                Title = tgUpdate.Message.Chat.Title,
                ChatTgId = tgUpdate.Message.Chat.Id
            };

            switch (command.CommandType)
            {
                case TgCommandTypes.NewChat:
                    await _chatManager.InsertChat(chatDto);
                    break;
                case TgCommandTypes.NewTitle:
                    await _chatManager.UpdateChatTitle(chatDto);
                    break;
                case TgCommandTypes.PrivateChat:
                    await _telegramBotClient.SendTextMessageAsync(tgUpdate.Message.Chat.Id, "Add me to the group and set chat manager permissions");
                    break;
                case TgCommandTypes.CustomPrompt:
                    await _chatManager.SetCustomPromptToChat(chatDto, command.TgCommandArguments);
                    break;

            }
        }
    }
}
