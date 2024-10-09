using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Dtos;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class CustomPromptCommand(IChatManager chatManager, IGenerateRequestManager generateRequestManager, ITelegramBotClient telegramBotClient) : ICommandHandler
{
    private readonly IChatManager _chatManager = chatManager;
    private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

    public async Task ProcessCommand(TgData tgData)
    {
        var currentChat = await _chatManager.GetChatById(tgData.ChatTgId);
        if (currentChat is not null && !currentChat.Title.Equals(tgData.ChatTitle))
        {
            currentChat.Title = tgData.ChatTitle;
            await _chatManager.UpdateChatTitle(currentChat);
        }

        var chatDto = new ChatDto
        {
            ChatTgId = tgData.ChatTgId,
            Title = tgData.ChatTitle
        };

        if (string.IsNullOrEmpty(tgData.TgCommand.TgCommandArguments) || tgData.TgCommand.TgCommandArguments.Length < 3)
        {
            await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId, $"[{tgData.TgCommand.TgCommandArguments}] слишком короткий промпт");
            return;
        }
        await _chatManager.SetCustomPromptToChat(chatDto, tgData.TgCommand.TgCommandArguments);
        var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(chatDto.ChatTgId);
        if (chatRequested is null)
            return;
        await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId,
            $"Альтернативный промпт для названия [{chatDto.Title}]: [{chatRequested.AlternativePrompt}]\nЗапустил генерацию...");
    }
}
