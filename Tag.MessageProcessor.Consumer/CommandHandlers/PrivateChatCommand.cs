using Tag.MessageProcessor.Consumer.ViewModels;
using Telegram.Bot;

namespace Tag.MessageProcessor.Consumer.CommandHandlers;

public class PrivateChatCommand(ITelegramBotClient telegramBotClient) : ICommandHandler
{
    private readonly ITelegramBotClient _telegramBotClient = telegramBotClient;

    public Task ProcessCommand(TgData tgData) 
        => _telegramBotClient.SendTextMessageAsync(tgData.ChatTgId, "Добавь меня в групповой чат и выдай разрешение на упраление чатом, чтобы я мог менять аватарки :)");
}
