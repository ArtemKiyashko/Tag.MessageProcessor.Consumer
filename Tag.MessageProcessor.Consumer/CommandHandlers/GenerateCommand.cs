using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class GenerateCommand(IGenerateRequestManager generateRequestManager, ITelegramBotClient telegramBotClient, IChatManager chatManager) : ICommandHandler
{
    private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;
    private readonly IChatManager _chatManager = chatManager;

    public async Task ProcessCommand(TgData tgData)
    {
        var currentChat = await _chatManager.GetChatById(tgData.ChatTgId);
        if (currentChat is not null && !currentChat.Title.Equals(tgData.ChatTitle))
        {
            currentChat.Title = tgData.ChatTitle;
            await _chatManager.UpdateChatTitle(currentChat);
        }
        
        var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(tgData.ChatTgId);
        
        if (chatRequested is null)
            return;

        await _telegramBotClient.SendTextMessageAsync(tgData.ChatTgId,
            $"Запустил генерацию для текущего названия [{chatRequested.AlternativePrompt ?? chatRequested.Title}]");
    }
}
