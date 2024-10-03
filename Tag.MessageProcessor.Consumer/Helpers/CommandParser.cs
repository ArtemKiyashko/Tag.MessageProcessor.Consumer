using Tag.MessageProcessor.Consumer.ViewModels;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Tag.MessageProcessor.Consumer.Helpers;

public static class CommandParser
{
    public static TgCommand? GetTgCommand(BotTgUpdate tgUpdate) => tgUpdate.Type switch
    {
        UpdateType.Message => tgUpdate.Message.GetMessageCommand(tgUpdate.BotUsername),
        _ => default,
    };

    public static TgCommand? GetMessageCommand(this Message? tgMessage, string botName)
    {
        if (tgMessage is null)
            return default;

        if (!string.IsNullOrEmpty(tgMessage.NewChatTitle))
            return new TgCommand(TgCommandTypes.NewTitle, null, tgMessage.NewChatTitle);

        var commandRawTuple = tgMessage.GetMessageCommandRaw(botName);
        if (!commandRawTuple.HasValue)
            return default;

        if (tgMessage.Chat.Type == ChatType.Private || tgMessage.Chat.Type == ChatType.Sender)
            return new TgCommand(TgCommandTypes.PrivateChat, commandRawTuple.Value.name, commandRawTuple.Value.arguments);

        var messageCommandType = GetMessageCommandType(commandRawTuple.Value.name);

        return messageCommandType == TgCommandTypes.Unknown ? 
            default : 
            new TgCommand(messageCommandType, commandRawTuple.Value.name, commandRawTuple.Value.arguments);
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
        "/prompt" => TgCommandTypes.CustomPrompt,
        _ => TgCommandTypes.Unknown
    };
}
