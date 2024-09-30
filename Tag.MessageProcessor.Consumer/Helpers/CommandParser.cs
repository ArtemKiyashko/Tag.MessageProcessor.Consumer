using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Tag.MessageProcessor.Consumer.Helpers;

public static class CommandParser
{
    public static TgCommand? GetTgCommand(Update tgUpdate, string botName)
    {
        switch (tgUpdate.Type){
            case UpdateType.Message:
                return tgUpdate.Message.GetMessageCommand(botName);
            default: return default;
        }
    }

    public static TgCommand? GetMessageCommand(this Message? tgMessage, string botName)
    {
        if (tgMessage is null)
            return default;
        var commandRawTuple = tgMessage.GetMessageCommandRaw(botName);
        if (!commandRawTuple.HasValue)
            return default;
        var messageCommandType = GetMessageCommandType(commandRawTuple.Value.name);
        return new TgCommand(messageCommandType, commandRawTuple.Value.name, commandRawTuple.Value.arguments);
    }

    public static (string name, string arguments)? GetMessageCommandRaw(this Message tgMessage, string botName)
    {
        var botCommand = tgMessage.Entities?.FirstOrDefault(e => e.Type == MessageEntityType.BotCommand);
        if (botCommand is null)
            return default;

        var commandNameSpan = tgMessage.Text.AsSpan(botCommand.Offset, botCommand.Length);
        var commandNameSegments = commandNameSpan.ToString().Split('@');
        if (commandNameSegments.Length > 1)
        {
            if (!botName.Equals(commandNameSegments[1], StringComparison.OrdinalIgnoreCase))
                return default;
        }

        var argumentsSpan = tgMessage.Text.AsSpan(botCommand.Offset + botCommand.Length);

        return (
            commandNameSegments[0], 
            argumentsSpan.ToString().Trim());
    }

    private static TgCommandTypes GetMessageCommandType(string commandRawName) => commandRawName.ToLower() switch
    {
        "/start" => TgCommandTypes.NewChat,
        "/generate" => TgCommandTypes.GenerateAvatar,
        _ => TgCommandTypes.Unknown
    };
}
