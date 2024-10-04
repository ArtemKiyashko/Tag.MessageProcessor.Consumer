using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Tag.MessageProcessor.Managers.Dtos;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class NewChatCommand(IChatManager chatManager, IGenerateRequestManager generateRequestManager, ITelegramBotClient telegramBotClient) : ICommandHandler
{
    private readonly IChatManager _chatManager = chatManager;
    private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

    public async Task ProcessCommand(TgData tgData)
    {
        var chatDto = new ChatDto{
            ChatTgId = tgData.ChatTgId,
            Title = tgData.ChatTitle
        };

        await _chatManager.InsertChat(chatDto);
        var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(chatDto.ChatTgId);
        if (chatRequested is null)
            return;
        await _telegramBotClient.SendTextMessageAsync(chatDto.ChatTgId,
            $"Привет! Я буду автоматически генерировать аватарки для чата используя в качестве промпта его название. Я только что поставил в очередь новую генерацию [{chatRequested.AlternativePrompt ?? chatRequested.Title}]. Ожидайте...");
    }
}
