using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Dtos;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class NewTitleCommand(IChatManager chatManager, IGenerateRequestManager generateRequestManager, ITelegramBotClient telegramBotClient) : ICommandHandler
{
    private readonly IChatManager _chatManager = chatManager;
    private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

    public async Task ProcessCommand(TgData tgData)
    {
        var chatDto = new ChatDto
        {
            ChatTgId = tgData.ChatTgId,
            Title = tgData.ChatTitle
        };

        await _chatManager.UpdateChatTitle(chatDto);
        var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(chatDto.ChatTgId);
        if (chatRequested is null)
            return;
        await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId,
            $"Обнаружено новое название чата. Начал генерацию аватарки [{chatRequested.AlternativePrompt ?? chatRequested.Title}]");
    }
}
