using Tag.MessageProcessor.Consumer.ViewModels;
using Tag.MessageProcessor.Managers;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class GenerateCommand(IGenerateRequestManager generateRequestManager, ITelegramBotClient telegramBotClient) : ICommandHandler
{
    private readonly IGenerateRequestManager _generateRequestManager = generateRequestManager;
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

    public async Task ProcessCommand(TgData tgData)
    {
        var chatRequested = await _generateRequestManager.EnqueueGenerateRequest(tgData.ChatTgId);
        if (chatRequested is null)
            return;
        await _telegramBotClient.SendTextMessageAsync(tgData.ChatTgId,
            $"Запустил генерацию для текущего названия [{chatRequested.AlternativePrompt ?? chatRequested.Title}]");
    }
}
