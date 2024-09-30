using Telegram.Bot.Types;

namespace Tag.MessageProcessor.Consumer.ViewModels;

public class BotTgUpdate : Update
{
    public required string BotUsername { get; set; }
}
