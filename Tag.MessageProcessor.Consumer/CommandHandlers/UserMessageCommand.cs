using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class UserMessageCommand(ITelegramBotClient telegramBotClient, IChatManager chatManager, IGenerateRequestManager generateRequestManager) : ICommandHandler
{
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;
    private readonly IChatManager _chatManager = chatManager;
    private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;

    public async Task ProcessCommand(TgData tgData)
    {
        var currentChat = await _chatManager.GetChatById(tgData.ChatTgId);
        if (currentChat is not null && !currentChat.Title.Equals(tgData.ChatTitle))
        {
            currentChat.Title = tgData.ChatTitle;
            await _chatManager.UpdateChatTitle(currentChat);
            var genChatDto = await _generateRequestManager.EnqueueGenerateRequest(currentChat.ChatTgId);

            if (genChatDto is null)
                return;

            await _telegramBotClient.SendTextMessageAsync(currentChat.ChatTgId,
                $"Обнаружено новое название чата. Начал генерацию аватарки [{genChatDto.AlternativePrompt ?? genChatDto.Title}]");
        }
    }
}
