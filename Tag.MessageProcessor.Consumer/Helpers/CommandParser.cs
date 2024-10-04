using Tag.MessageProcessor.Consumer.ViewModels;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Tag.MessageProcessor.Consumer.Helpers;

public static class CommandParser
{
    public static TgData? GetTgData(BotTgUpdate tgUpdate) => tgUpdate.Type switch
    {
        UpdateType.Message => tgUpdate.Message.GetMessageData(tgUpdate.BotUsername),
        UpdateType.MyChatMember => tgUpdate.MyChatMember.GetChatMemberData(tgUpdate.BotUsername),
        _ => default,
    };

    public static TgData? GetChatMemberData(this ChatMemberUpdated? chatMemberUpdated, string botName)
    {
        if (chatMemberUpdated is null || botName is null)
            return default;
        
        if (!botName.Equals(chatMemberUpdated.NewChatMember.User.Username, StringComparison.OrdinalIgnoreCase))
            return default;
        
        TgCommand? tgCommand = default;

        if (chatMemberUpdated.NewChatMember.Status == ChatMemberStatus.Left ||
            chatMemberUpdated.NewChatMember.Status == ChatMemberStatus.Kicked ||
            chatMemberUpdated.NewChatMember.Status == ChatMemberStatus.Restricted)
            tgCommand = new TgCommand(TgCommandTypes.RemoveChat, null, null);
        
        if (chatMemberUpdated.NewChatMember.Status == ChatMemberStatus.Member)
            tgCommand = new TgCommand(TgCommandTypes.NewChat, null, null);
        
        if (tgCommand is null)
            return default;

        return new TgData(tgCommand, chatMemberUpdated.Chat.Title, chatMemberUpdated.Chat.Id);
    }
    public static TgData? GetMessageData(this Message? tgMessage, string botName)
    {
        if (tgMessage is null)
            return default;
        
        if (tgMessage.MigrateToChatId.HasValue)
            return new TgData(
                new TgCommand(TgCommandTypes.ChatMigrate, null, tgMessage.Chat.Id.ToString()),
                tgMessage.Chat.Title,
                tgMessage.MigrateToChatId.Value);

        if (!string.IsNullOrEmpty(tgMessage.NewChatTitle))
            return new TgData(
                new TgCommand(TgCommandTypes.NewTitle, null, tgMessage.NewChatTitle),
                tgMessage.NewChatTitle,
                tgMessage.Chat.Id);

        var commandTuple = tgMessage.GetMessageCommand(botName);
        if (!commandTuple.HasValue)
            return default;

        if (tgMessage.Chat.Type == ChatType.Private || tgMessage.Chat.Type == ChatType.Sender)
            return new TgData(
                new TgCommand(TgCommandTypes.PrivateChat, commandTuple.Value.name, commandTuple.Value.arguments),
                tgMessage.Chat.Title,
                tgMessage.Chat.Id);

        var messageCommandType = GetMessageCommandType(commandTuple.Value.name);

        return messageCommandType == TgCommandTypes.Unknown ? 
            default : 
            new TgData(
                new TgCommand(messageCommandType, commandTuple.Value.name, commandTuple.Value.arguments),
                tgMessage.Chat.Title,
                tgMessage.Chat.Id);
    }

    public static (string name, string arguments)? GetMessageCommand(this Message tgMessage, string botName)
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
