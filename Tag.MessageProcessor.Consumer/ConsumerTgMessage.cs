using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Tag.MessageProcessor.Consumer.Helpers;
using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Dtos;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Tag.MessageProcessor.Consumer
{
    public class ConsumerTgMessage(
        ILogger<ConsumerTgMessage> logger,
        ITelegramBotClient telegramBotClient,
        IChatManager chatManager,
        IGenerateRequestManager generateRequestManager)
    {
        private readonly ILogger<ConsumerTgMessage> _logger = logger;
        private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;
        private readonly IChatManager _chatManager = chatManager;
        private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;

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

            var chatDto = new ChatDto
            {
                Title = tgData.ChatTitle,
                ChatTgId = tgData.ChatTgId
            };

            try
            {
                switch (tgData.TgCommand.CommandType)
                {
                    case TgCommandTypes.NewChat:
                        await HandleNewChat(chatDto);
                        break;
                    case TgCommandTypes.NewTitle:
                        await HandleNewTitle(chatDto);
                        break;
                    case TgCommandTypes.CustomPrompt:
                        await HandleCustomPrompt(tgData.TgCommand, chatDto);
                        break;
                    case TgCommandTypes.PrivateChat:
                        await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId, "Добавть меня в групповой чат и выдай разрешение на упраление чатом, чтобы я мог менять аватарки :)");
                        break;
                    case TgCommandTypes.GenerateAvatar:
                        await HandleGenerate(chatDto);
                        break;
                    case TgCommandTypes.RemoveChat:
                        await _chatManager.DeleteChat(chatDto.ChatTgId);
                        break;
                    case TgCommandTypes.ChatMigrate:
                        await _chatManager.MigrateChat(long.Parse(tgData.TgCommand.TgCommandArguments), tgData.ChatTgId);
                        break;
                }
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 403)
            {
                await _chatManager.DeleteChat(chatDto.ChatTgId);
            }
        }

        private async Task HandleGenerate(ChatDto chatDto)
        {
            var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(chatDto.ChatTgId);
            if (chatRequested is null)
                return;
            await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId,
                $"Запустил генерацию для текущего названия [{chatRequested.AlternativePrompt ?? chatRequested.Title}]");
        }

        private async Task HandleCustomPrompt(TgCommand command, ChatDto chatDto)
        {
            if (string.IsNullOrEmpty(command.TgCommandArguments) || command.TgCommandArguments.Length < 3)
            {
                await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId, $"[{command.TgCommandArguments}] слишком короткий промпт");
                return;
            }
            await _chatManager.SetCustomPromptToChat(chatDto, command.TgCommandArguments);
            var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(chatDto.ChatTgId);
            if (chatRequested is null)
                return;
            await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId,
                $"Альтернативный промпт для названия [{chatDto.Title}]: [{chatRequested.AlternativePrompt}]\nЗапустил генерацию...");
        }

        private async Task HandleNewChat(ChatDto chatDto)
        {
            await _chatManager.InsertChat(chatDto);
            var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(chatDto.ChatTgId);
            if (chatRequested is null)
                return;
            await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId,
                $"Привет! Я буду автоматически генерировать аватарки для чата используя в качестве промпта его название. Я только что поставил в очередь новую генерацию [{chatRequested.AlternativePrompt ?? chatRequested.Title}]. Ожидайте...");
        }

        private async Task HandleNewTitle(ChatDto chatDto)
        {
            await _chatManager.UpdateChatTitle(chatDto);
            var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(chatDto.ChatTgId);
            if (chatRequested is null)
                return;
            await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId,
                $"Обнаружено новое название чата. Начал генерацию аватарки [{chatRequested.AlternativePrompt ?? chatRequested.Title}]");
        }
    }
}
